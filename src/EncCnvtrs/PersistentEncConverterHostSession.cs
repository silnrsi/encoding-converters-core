using System;
using System.Diagnostics;
using System.IO;
using System.IO.Pipes;
using System.Text;
using System.Threading.Tasks;

using Newtonsoft.Json;

using ECInterfaces;
using SilEncConverters40.EncConverterHostExe;

namespace SilEncConverters40
{
    /// <summary>
    /// Caller-side counterpart to EncConverterHostExe's keep-alive mode (see Handover.md, "Generic
    /// subprocess fallback for host-process conflicts", and src/EncConverterHostExe/Program.cs):
    /// launches the host exe once with KeepAliveSeconds &gt; 0, holds the named-pipe connection open,
    /// and drives repeated Convert() calls against the SAME already-initialized IEncConverter instance
    /// in the child process, instead of paying full re-initialization cost on every call the way a
    /// one-shot invocation would.
    /// One instance == one live child process == one converter instance in that child. Not thread-safe
    /// (mirrors the underlying pipe, which is single-client) -- callers needing concurrent conversions
    /// should use separate sessions.
    /// Used by <see cref="SubprocessEncConverter"/>, the IEncConverter proxy EncConverters.AddEx hands
    /// back in place of a converter whose RiskyNativeDependencies conflict with something already loaded
    /// in this host process.
    /// </summary>
    internal sealed class PersistentEncConverterHostSession : IDisposable
    {
        // generous default: a Word/Paratext session doing occasional conversions over a long editing
        // session shouldn't have its subprocess reaped between keystrokes, but an abandoned session
        // (e.g. the host app was closed without an orderly shutdown) shouldn't run forever either.
        private const int DefaultKeepAliveSeconds = 600;

        private readonly string _hostExePath;
        private readonly EncConverterHostRequest _launchRequest;

        private Process _process;
        private NamedPipeClientStream _pipe;

        // Populated from the one-time handshake response (see EncConverterHostPipeResponse's Adjusted*
        // doc comment) the first time a session is actually established. Null until then. Exposed so
        // SubprocessEncConverter can adopt whatever the real converter's own Initialize() adjusted
        // (e.g. TechHindiSiteEncConverter's uni-directional detection) instead of being stuck reporting
        // the caller's original, pre-adjustment values forever.
        public string AdjustedLhsEncodingId { get; private set; }
        public string AdjustedRhsEncodingId { get; private set; }
        public ConvType? AdjustedConversionType { get; private set; }
        public int? AdjustedProcessTypeFlags { get; private set; }

        public PersistentEncConverterHostSession(string hostExePath, string assemblyReference, string progId,
            string converterName, string converterIdentifier, string lhsEncodingId, string rhsEncodingId,
            ConvType conversionType, int processTypeFlags, int codePageInput, int codePageOutput,
            int keepAliveSeconds = DefaultKeepAliveSeconds)
        {
            if (String.IsNullOrEmpty(hostExePath) || !File.Exists(hostExePath))
                throw new FileNotFoundException("EncConverterHostExe.exe not found.", hostExePath);

            _hostExePath = hostExePath;
            _launchRequest = new EncConverterHostRequest
            {
                AssemblyReference = assemblyReference,
                ProgId = progId,
                ConverterName = converterName,
                ConverterIdentifier = converterIdentifier,
                LhsEncodingId = lhsEncodingId,
                RhsEncodingId = rhsEncodingId,
                ConversionType = conversionType,
                ProcessTypeFlags = processTypeFlags,
                CodePageInput = codePageInput,
                CodePageOutput = codePageOutput,
                KeepAliveSeconds = keepAliveSeconds <= 0 ? DefaultKeepAliveSeconds : keepAliveSeconds,
            };
        }

        /// <summary>
        /// Sends one conversion request to the (launched-on-demand) child, connecting/relaunching first
        /// if this is the first call or the previous child is no longer around (e.g. it hit its idle
        /// timeout since the last call -- a long gap between conversions in the same Word session is
        /// expected, not an error). A single transparent relaunch-and-retry covers that case; a second
        /// failure in a row is a real error and is allowed to propagate.
        /// </summary>
        public string Convert(string inputText, bool? directionForward, NormalizeFlags? normalizeOutput)
        {
            try
            {
                return SendPipeRequest(inputText, directionForward, normalizeOutput);
            }
            catch (Exception ex) when (ex is IOException || ex is ObjectDisposedException || ex is InvalidOperationException || ex is TimeoutException)
            {
                Util.DebugWriteLine(typeof(PersistentEncConverterHostSession).Name,
                    $"Pipe request failed ({ex.Message}) -- relaunching session and retrying once.");
                CloseSession();
                return SendPipeRequest(inputText, directionForward, normalizeOutput);
            }
        }

        private string SendPipeRequest(string inputText, bool? directionForward, NormalizeFlags? normalizeOutput)
        {
            EnsureSessionStarted();

            var request = new EncConverterHostPipeRequest
            {
                InputText = inputText,
                DirectionForward = directionForward,
                NormalizeOutput = normalizeOutput,
            };
            var response = SendAndReceive(request);

            if (!response.Success)
                throw new ApplicationException($"EncConverterHostExe conversion failed: {response.ErrorMessage}");

            return response.Result;
        }

        private void EnsureSessionStarted()
        {
            if (_process != null && !_process.HasExited && _pipe != null && _pipe.IsConnected)
                return;

            CloseSession();

            var sessionPipeName = "EncConverterHostSession-" + Guid.NewGuid();
            _launchRequest.SessionPipeName = sessionPipeName;

            var requestFilespec = Path.GetTempFileName();
            File.WriteAllText(requestFilespec, JsonConvert.SerializeObject(_launchRequest));

            _process = Process.Start(new ProcessStartInfo(_hostExePath, $"\"{requestFilespec}\"")
            {
                UseShellExecute = false,
                CreateNoWindow = true,
                RedirectStandardOutput = true,
                RedirectStandardError = true,
            });

            // ties the child's lifetime to this process's own (see HostProcessJobObject's class doc for
            // why this needs to be a kernel-level mechanism, not a managed Dispose()/finalizer) -- so an
            // uncleanly-killed host process (crash, forced kill) takes this child down immediately rather
            // than leaving it orphaned for up to its own idle timeout.
            HostProcessJobObject.AssignToKillOnCloseJob(_process);

            _pipe = new NamedPipeClientStream(".", sessionPipeName, PipeDirection.InOut);
            _pipe.Connect(10000);
            // NamedPipeClientStream does not inherit PipeTransmissionMode.Message from the server
            // automatically -- must be set explicitly or IsMessageComplete-based framing below is wrong.
            _pipe.ReadMode = PipeTransmissionMode.Message;

            // consume the one-time handshake Program.RunKeepAliveLoop sends immediately after accepting
            // this connection, before any conversion request -- see EncConverterHostPipeResponse's
            // Adjusted* doc comment for why this exists.
            var handshake = ReceiveOneMessage();
            if (!handshake.Success)
                throw new ApplicationException($"EncConverterHostExe failed to initialize: {handshake.ErrorMessage}");

            AdjustedLhsEncodingId = handshake.AdjustedLhsEncodingId;
            AdjustedRhsEncodingId = handshake.AdjustedRhsEncodingId;
            AdjustedConversionType = handshake.AdjustedConversionType;
            AdjustedProcessTypeFlags = handshake.AdjustedProcessTypeFlags;
        }

        // Generous enough for a legitimate (if slow) conversion, but bounded so a child that never
        // responds -- whether it's genuinely hung deep inside a native call (see
        // WebBrowserEdge.WebView2InitializationTimeoutSeconds's own doc comment: a live dotnet-dump
        // capture proved a native COM call can simply never complete, with nothing to observe, ever)
        // or has died in some way that doesn't cleanly close the pipe -- can't hang the CALLER forever.
        // Confirmed necessary, not theoretical: reproduced a run where this exact pipe read blocked far
        // past WebBrowserEdge's own 20s internal timeout with no observable activity in the child at
        // all. This is what actually delivers the isolation guarantee the whole subprocess design
        // exists for -- without it, a parent (Word, a test host, anything) blocked in a plain
        // synchronous, unbounded Read() is just as stuck as if it had attempted the risky operation
        // in-process itself; running it in a subprocess only helps if the CALLER can also give up on
        // it.
        private const int ReceiveTimeoutSeconds = 30;

        private EncConverterHostPipeResponse SendAndReceive(EncConverterHostPipeRequest request)
        {
            var requestBytes = Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(request));
            _pipe.Write(requestBytes, 0, requestBytes.Length);
            _pipe.Flush();
            _pipe.WaitForPipeDrain();

            return ReceiveOneMessage();
        }

        /// <summary>Reads exactly one Message-mode pipe message, bounded by ReceiveTimeoutSeconds. Used
        /// both for a normal request/response round-trip (see SendAndReceive) and for the one-time
        /// handshake (see EnsureSessionStarted), which arrives unprompted -- nothing is written to the
        /// pipe before that particular read.</summary>
        private EncConverterHostPipeResponse ReceiveOneMessage()
        {
            using (var ms = new MemoryStream())
            {
                var buffer = new byte[4096];
                do
                {
                    var readTask = _pipe.ReadAsync(buffer, 0, buffer.Length);
                    if (!readTask.Wait(TimeSpan.FromSeconds(ReceiveTimeoutSeconds)))
                    {
                        // don't leave the child running for whatever's left of its own (much longer)
                        // idle timeout -- reclaim it now, since we're giving up on it ourselves.
                        // HostProcessJobObject's kill-on-close job only handles an uncleanly-killed
                        // *parent*; it's not a substitute for this.
                        KillProcessBestEffort();
                        throw new TimeoutException(
                            $"EncConverterHostExe did not respond within {ReceiveTimeoutSeconds}s.");
                    }

                    int n = readTask.Result;
                    if (n == 0)
                        throw new IOException("EncConverterHostExe pipe closed before a complete response was received.");
                    ms.Write(buffer, 0, n);
                }
                while (!_pipe.IsMessageComplete);

                return JsonConvert.DeserializeObject<EncConverterHostPipeResponse>(Encoding.UTF8.GetString(ms.ToArray()));
            }
        }

        private void KillProcessBestEffort()
        {
            try
            {
                if (_process != null && !_process.HasExited)
                    _process.Kill();
            }
            catch
            {
                // best-effort only -- if it's already gone or unresponsive to Kill() for some reason,
                // there's nothing more to do here; CloseSession()'s own cleanup runs regardless.
            }
        }

        private void CloseSession()
        {
            try
            {
                if (_pipe != null && _pipe.IsConnected)
                    SendAndReceive(new EncConverterHostPipeRequest { Shutdown = true });
            }
            catch
            {
                // best effort -- if the child's already gone or unresponsive, the Kill() below (via
                // Dispose) or its own idle timer will clean it up regardless.
            }
            finally
            {
                _pipe?.Dispose();
                _pipe = null;

                if (_process != null)
                {
                    try
                    {
                        if (!_process.WaitForExit(2000))
                            _process.Kill();
                    }
                    catch
                    {
                    }
                    _process.Dispose();
                    _process = null;
                }
            }
        }

        public void Dispose()
        {
            CloseSession();
        }
    }
}
