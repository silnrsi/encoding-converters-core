using ECInterfaces;
using Newtonsoft.Json;
using System;
using System.Diagnostics;
using System.IO;
using System.IO.Pipes;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;

namespace SilEncConverters40.EncConverterHostExe
{
    /// <summary>
    /// Generic, engine-agnostic subprocess host: given a request describing any IEncConverter
    /// implementation (assembly + type name + Initialize() parameters) and one input string, it
    /// instantiates that converter in this fresh, isolated process - not whatever host process
    /// (Word, Paratext, etc.) the caller is actually running in - drives it through exactly the
    /// same Activator.CreateInstance -> Initialize -> Convert sequence
    /// EncConverters.InstantiateIEncConverter already uses, and prints the result.
    ///
    /// See Handover.md, "Generic subprocess fallback for host-process conflicts", for why this
    /// exists: a host process can have its own resident copy of something a converter also needs
    /// (e.g. Word's own WebView2Loader.dll), and Windows' DLL search order means our own copy never
    /// gets a chance to load in-process. Running in a brand new process sidesteps that entirely,
    /// since nothing else has loaded anything into it yet.
    ///
    /// Deliberately does NOT reuse AzureOpenAiExe/VertexAiExe's protocol as-is - it fixes two
    /// concrete weaknesses found there: (1) no weak shared-key encryption of the request file, since
    /// most transducer configs aren't secret, and the temp file actually gets deleted; (2) success
    /// and failure are distinguishable by exit code (0 = success, result on stdout; nonzero = error
    /// message on stderr), rather than everything landing as text on stdout with an always-0 exit.
    ///
    /// Optionally (EncConverterHostRequest.KeepAliveSeconds > 0) stays resident after answering the
    /// first Convert() call, serving further calls over a named pipe against the SAME already-
    /// initialized converter instance instead of exiting - see Handover.md, "Generic subprocess
    /// fallback for host-process conflicts", for the full protocol and why a naive "just don't exit"
    /// approach would deadlock every existing caller.
    /// </summary>
    public static class Program
    {
        // required for WebView2 (TechHindiSiteEncConverter's Edge path): COM apartment threading is
        // not something a converter can opt into per-call - it's fixed for the process's main thread
        // the first time anything touches it, and a plain console app's Main defaults to MTA with no
        // [STAThread] here. Confirmed necessary, not theoretical: without this, a direct, isolated
        // invocation of this exe against a WebView2-based request failed immediately (not a hang) with
        // "Cannot change thread mode after it is set. (0x80010106 (RPC_E_CHANGED_MODE))" - WebView2's
        // own attempt to establish an STA apartment loses the race against whatever implicitly
        // initializes MTA first. Harmless for every other converter type, which don't care about
        // apartment state at all.
        [STAThread]
        public static int Main(string[] args)
        {
            // every exit path below must go through HardExit, not a bare `return` from this method or
            // even Environment.Exit - confirmed necessary (not just theoretical) by a real hang:
            // TestPy3ScriptEncConverter_ViaSubprocess hung forever under net9.0-windows specifically
            // (net462/net48 were fine), in the caller's Process.StandardOutput.ReadToEnd(), even though
            // this process had already written its result and reached the one-shot `return 0` below.
            // Root-caused with a dotnet-dump: the *finalizer thread* was stuck inside
            // Python.Runtime.PythonEngine.OnProcessExit -> PythonEngine.Shutdown() -> Py.GIL() ->
            // PyGILState_Ensure(), permanently waiting to reacquire the Python GIL. Python.NET
            // registers an AppDomain.ProcessExit handler to shut Python down gracefully, and that
            // handler deadlocks - which fires (and blocks on) regardless of whether the process is
            // ending via a bare `return` from Main or an explicit Environment.Exit() call, so switching
            // to Environment.Exit() alone (an earlier cut of this fix) did not help. The reason this
            // never showed up under net462/net48: .NET Framework enforces roughly a 2-second timeout
            // on ProcessExit handlers before forcibly continuing shutdown anyway; modern .NET (Core/5+)
            // removed that timeout and waits indefinitely - so the exact same latent Python.NET
            // deadlock was always there, just silently papered over by Framework's timeout. Bypassing
            // ProcessExit entirely is correct here regardless of *why* a given converter's native
            // library might misbehave on shutdown - this process has no state worth an orderly
            // teardown for (IEncConverter has no Dispose member), so a genuine hard kill via
            // TerminateProcess (skips all managed shutdown machinery, including ProcessExit and
            // finalizers, unlike Environment.Exit) is both simpler and more robust than trying to
            // anticipate every native library's own shutdown quirks. Process.Kill() was considered and
            // rejected: it can't set a specific exit code, and this protocol depends on exit code 0
            // meaning success.
            var exitCode = Run(args);
            HardExit(exitCode);
            return exitCode;    // unreachable - HardExit never returns - but required to compile.
        }

        [DllImport("kernel32.dll")]
        private static extern IntPtr GetCurrentProcess();

        [DllImport("kernel32.dll", SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        private static extern bool TerminateProcess(IntPtr hProcess, uint uExitCode);

        /// <summary>Terminates this process immediately with the given exit code, bypassing
        /// AppDomain.ProcessExit/finalizers/all other managed shutdown machinery entirely - see the
        /// comment on Main above for why that's necessary here, not just a hygiene choice.</summary>
        private static void HardExit(int exitCode)
        {
            TerminateProcess(GetCurrentProcess(), unchecked((uint)exitCode));
        }

        private static int Run(string[] args)
        {
            Console.InputEncoding = Encoding.Unicode;
            Console.OutputEncoding = Encoding.Unicode;

            if ((args.Length == 0) || String.IsNullOrEmpty(args[0]))
            {
                Console.Error.WriteLine(
                    "Usage: EncConverterHostExe.exe <path-to-request-json-file>\n" +
                    "The request file is a JSON-serialized EncConverterHostRequest (see Handover.md,\n" +
                    "\"Generic subprocess fallback for host-process conflicts\").");
                return 1;
            }

            string requestFilespec = args[0];
            EncConverterHostRequest request;

            try
            {
                if (!File.Exists(requestFilespec))
                    throw new FileNotFoundException($"Request file not found: '{requestFilespec}'", requestFilespec);

                var json = File.ReadAllText(requestFilespec);
                request = JsonConvert.DeserializeObject<EncConverterHostRequest>(json);

                if (request == null)
                    throw new ApplicationException("Request file deserialized to null - malformed JSON?");
            }
            catch (Exception ex)
            {
                Console.Error.WriteLine("Couldn't read/parse the request file: " + BuildExceptionMessage(ex));
                return 1;
            }
            finally
            {
                // this is our own private temp file (the caller creates a fresh one per invocation) -
                // clean it up regardless of outcome, unlike the AzureOpenAiExe/VertexAiExe precedent,
                // which never deletes its equivalent temp file.
                TryDeleteFile(requestFilespec);
            }

            // if a keep-alive session was requested, open the pipe BEFORE paying any
            // instantiate/Initialize cost, so a bad SessionPipeName fails fast through the existing
            // stderr+exit-1 path below rather than surfacing after the caller already believes the
            // first call succeeded.
            NamedPipeServerStream pipeServer = null;
            if (request.KeepAliveSeconds > 0)
            {
                if (String.IsNullOrEmpty(request.SessionPipeName))
                {
                    Console.Error.WriteLine("KeepAliveSeconds > 0 requires a non-empty SessionPipeName.");
                    return 1;
                }

                try
                {
                    pipeServer = new NamedPipeServerStream(request.SessionPipeName, PipeDirection.InOut,
                        maxNumberOfServerInstances: 1, PipeTransmissionMode.Message, PipeOptions.None);
                }
                catch (Exception ex)
                {
                    Console.Error.WriteLine(
                        $"Couldn't open session pipe '{request.SessionPipeName}': " + BuildExceptionMessage(ex));
                    return 1;
                }
            }

            IEncConverter converter;

            try
            {
                converter = InstantiateConverter(request.AssemblyReference, request.ProgId);
            }
            catch (Exception ex)
            {
                Console.Error.WriteLine(
                    $"Couldn't instantiate '{request.ProgId}' from assembly '{request.AssemblyReference}': "
                    + BuildExceptionMessage(ex));
                return 1;
            }

            try
            {
                // per IEncConverter.Initialize()'s own doc comment: this must be called directly by
                // any client that instantiates an implementation class itself, rather than pulling it
                // from the EncConverters repository/collection (which normally does this for you).
                var lhsEncodingId = request.LhsEncodingId;
                var rhsEncodingId = request.RhsEncodingId;
                var conversionType = request.ConversionType;
                var processTypeFlags = request.ProcessTypeFlags;

                converter.Initialize(
                    request.ConverterName,
                    request.ConverterIdentifier,
                    ref lhsEncodingId,
                    ref rhsEncodingId,
                    ref conversionType,
                    ref processTypeFlags,
                    request.CodePageInput,
                    request.CodePageOutput,
                    bAdding: true);

                converter.DirectionForward = request.DirectionForward;
                converter.NormalizeOutput = request.NormalizeOutput;

                if (pipeServer == null)
                {
                    // exactly today's one-shot behavior, unchanged
                    var result = converter.Convert(request.InputText);
                    Console.Out.Write(result);
                    return 0;
                }

                // Keep-alive mode: no special first call via stdout. An earlier version of this tried
                // writing the first result to stdout and then calling Console.Out.Close()/
                // Console.Error.Close() to signal "done writing, but still running" to a caller
                // blocked in Process.StandardOutput.ReadToEnd() - confirmed EMPIRICALLY (via the
                // TestEncConverterHostKeepAlive tests, which hung for exactly KeepAliveSeconds before
                // "succeeding") that this does not work: .NET's Console stream Close() does not
                // reliably close the underlying OS pipe handle while the process itself keeps running,
                // so the caller's ReadToEnd() only unblocks once the whole process actually exits -
                // useless for this purpose. So: nothing is ever written to stdout in this mode: the
                // caller connects to the pipe for EVERY conversion, including the first one
                // (request.InputText is ignored here). Still close the handles for hygiene, just not
                // as a signaling mechanism.
                Console.Out.Close();
                Console.Error.Close();

                RunKeepAliveLoop(pipeServer, converter, TimeSpan.FromSeconds(request.KeepAliveSeconds));
                return 0;
            }
            catch (Exception ex)
            {
                // only reachable before the keep-alive loop starts (stderr is still open here) - the
                // Initialize() call itself failed. In keep-alive mode, a caller already connected (or
                // connecting) to the pipe simply sees the process exit without ever responding -
                // callers should treat that as an initialization failure.
                Console.Error.WriteLine(
                    $"'{request.ProgId}' failed to initialize or convert: " + BuildExceptionMessage(ex));
                return 1;
            }
        }

        /// <summary>
        /// Serves further Convert() calls, on the SAME already-initialized converter instance, over a
        /// Message-mode named pipe until either a Shutdown request arrives or the idle timer fires.
        /// Message-mode framing gives multi-request/response boundaries for free - no manual
        /// length-prefixing needed, unlike raw stdin/stdout.
        /// </summary>
        private static void RunKeepAliveLoop(NamedPipeServerStream pipeServer, IEncConverter converter, TimeSpan idleTimeout)
        {
            // self-terminating watchdog: if nothing arrives within idleTimeout - counting from process
            // start, so a launched-but-never-connected session still expires on schedule, and reset on
            // every message RECEIVED (not after Convert() finishes, so a slow conversion is never
            // mistaken for idle time) - this process exits itself. An orphaned child (parent crashed,
            // forgot to clean up, whatever) doesn't run forever depending on the parent's cooperation.
            // IEncConverter has no Dispose/cleanup member, so a hard kill needs no teardown step - and
            // per the comment on Main/HardExit above, a hard kill (bypassing ProcessExit) is required
            // here, not just tidy: a keep-alive session driving a Python-based converter would hit the
            // exact same Python.NET GIL-reacquisition deadlock on a plain Environment.Exit(0).
            using (var idleTimer = new Timer(_ => HardExit(0), null, idleTimeout, Timeout.InfiniteTimeSpan))
            {
                pipeServer.WaitForConnection();

                // one-time handshake, sent before any conversion request: reports back whatever the
                // real converter's own Initialize() (already run, in Run() above) adjusted via its ref
                // parameters - see EncConverterHostPipeResponse's Adjusted* doc comment for why a caller
                // needs this, not just the values it originally sent in the launch request.
                WriteMessage(pipeServer, new EncConverterHostPipeResponse
                {
                    Success = true,
                    ServerProcessId = Process.GetCurrentProcess().Id,
                    AdjustedLhsEncodingId = converter.LeftEncodingID,
                    AdjustedRhsEncodingId = converter.RightEncodingID,
                    AdjustedConversionType = converter.ConversionType,
                    AdjustedProcessTypeFlags = converter.ProcessType,
                });

                while (pipeServer.IsConnected)
                {
                    var requestBytes = ReadMessage(pipeServer);
                    if (requestBytes == null)
                        break;   // client disconnected

                    idleTimer.Change(idleTimeout, Timeout.InfiniteTimeSpan);

                    var pipeRequest = JsonConvert.DeserializeObject<EncConverterHostPipeRequest>(
                        Encoding.UTF8.GetString(requestBytes));

                    if (pipeRequest.Shutdown)
                    {
                        WriteMessage(pipeServer, new EncConverterHostPipeResponse
                        {
                            Success = true,
                            ServerProcessId = Process.GetCurrentProcess().Id
                        });
                        return;   // clean voluntary exit, distinct from the idle-timeout path
                    }

                    EncConverterHostPipeResponse response;
                    try
                    {
                        if (pipeRequest.DirectionForward.HasValue)
                            converter.DirectionForward = pipeRequest.DirectionForward.Value;
                        if (pipeRequest.NormalizeOutput.HasValue)
                            converter.NormalizeOutput = pipeRequest.NormalizeOutput.Value;

                        var text = converter.Convert(pipeRequest.InputText);
                        response = new EncConverterHostPipeResponse
                        {
                            Success = true,
                            Result = text,
                            ServerProcessId = Process.GetCurrentProcess().Id
                        };
                    }
                    catch (Exception ex)
                    {
                        response = new EncConverterHostPipeResponse
                        {
                            Success = false,
                            ErrorMessage = BuildExceptionMessage(ex),
                            ServerProcessId = Process.GetCurrentProcess().Id
                        };
                    }

                    WriteMessage(pipeServer, response);
                }
            }
        }

        private static byte[] ReadMessage(PipeStream pipe)
        {
            using (var ms = new MemoryStream())
            {
                var buffer = new byte[4096];
                do
                {
                    int n = pipe.Read(buffer, 0, buffer.Length);
                    if (n == 0)
                        return null;   // client end closed
                    ms.Write(buffer, 0, n);
                }
                while (!pipe.IsMessageComplete);
                return ms.ToArray();
            }
        }

        private static void WriteMessage(PipeStream pipe, EncConverterHostPipeResponse response)
        {
            var bytes = Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(response));
            pipe.Write(bytes, 0, bytes.Length);
            pipe.Flush();
            pipe.WaitForPipeDrain();
        }

        /// <summary>
        /// Mirrors EncConverters.InstantiateIEncConverter's own two-step resolution (src/EncCnvtrs/
        /// EncConverters.cs:2401-2480): Activator.CreateInstance(assemblyReference, progId) alone only
        /// resolves assemblies .NET's default load context already knows about (this exe's own
        /// compile-time references) - a sibling transducer DLL (e.g. IcuEC.dll) sitting right next to
        /// this exe is NOT found that way (confirmed: Assembly.Load("IcuEC") fails with "cannot find
        /// the file specified" even when IcuEC.dll is in the same directory - simple-name loads don't
        /// probe the app's own base directory for assemblies outside the app's own dependency
        /// manifest). So: try the simple approach first, and if that fails, fall back to an explicit
        /// Assembly.LoadFrom() against this exe's own directory, exactly like InstantiateIEncConverter's
        /// own fallback does (there, against an install-location registry key instead - here, our
        /// design assumes the host exe and every transducer DLL it might be asked to load live
        /// side-by-side in the same install directory, so no registry lookup is needed).
        /// </summary>
        private static IEncConverter InstantiateConverter(string assemblyReference, string progId)
        {
            try
            {
                var handle = Activator.CreateInstance(assemblyReference, progId);
                return (IEncConverter)handle.Unwrap();
            }
            catch (Exception)
            {
                var simpleName = new AssemblyName(assemblyReference).Name;
                var dllPath = Path.Combine(AppContext.BaseDirectory, simpleName + ".dll");
                var assembly = Assembly.LoadFrom(dllPath);
                var type = assembly.GetType(progId, throwOnError: true);
                return (IEncConverter)Activator.CreateInstance(type);
            }
        }

        private static void TryDeleteFile(string filespec)
        {
            try
            {
                if (File.Exists(filespec))
                    File.Delete(filespec);
            }
            catch
            {
                // best-effort cleanup only - a leaked temp file is a much smaller problem than
                // failing the whole request over it.
            }
        }

        /// <summary>walks the InnerException chain into one readable message, same style as
        /// WebBrowserEdgeInfo.LogExceptionMessage.</summary>
        private static string BuildExceptionMessage(Exception ex)
        {
            var msg = "Error occurred: " + ex.Message;
            while (ex.InnerException != null)
            {
                ex = ex.InnerException;
                msg += $"{Environment.NewLine}because: (InnerException): {ex.Message}";
            }
            return msg;
        }
    }
}
