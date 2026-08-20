// Exercises EncConverterHostExe.exe's optional keep-alive mode (see Handover.md, "Generic
// subprocess fallback for host-process conflicts") - staying resident and serving repeated
// Convert() calls against the SAME already-initialized converter instance over a named pipe,
// instead of the one-shot "spawn, convert once, exit" behavior TestEncConverterHost.cs
// exercises. Uses AdaptItEncConverter (already wired up with real fixture data in
// TestEncConverterHost.TestAdaptItEncConverter_ViaSubprocess) since it has genuine non-trivial
// init cost - loading/parsing an XML KB file - making it a reasonable stand-in for "heavy-init
// converter used many times in a loop".
//
// Every conversion in keep-alive mode - including the first - goes over the pipe; nothing is
// ever written to stdout in this mode (see Program.cs/Handover.md for why: an earlier attempt at
// signaling "first result ready, but still running" via closing Console.Out turned out not to
// work reliably on Windows/.NET - confirmed by this very test file hanging for exactly
// KeepAliveSeconds before "succeeding" the first time it was run).
//
// Deliberately has its own launch helper, distinct from TestEncConverterHost.RunEncConverterHostExe
// - that one reads stdout to EOF and calls WaitForExit(), neither of which applies here since the
// process is meant to keep running and never writes to stdout at all in this mode.

using System;
using System.Diagnostics;
using System.IO;
using System.IO.Pipes;
using System.Reflection;
using System.Text;

using NUnit.Framework;
using Newtonsoft.Json;

using ECInterfaces;
using SilEncConverters40.EncConverterHostExe;

namespace TestEncCnvtrs
{
	[TestFixture]
	public class TestEncConverterHostKeepAlive
	{
		#region Helpers

		private const string AdaptItInput = "यह, परीक्षा है?";
		private const string AdaptItExpectedOutput = "%3%this%she%he%, %2%examination%test% %2%PRES%is%?";

		private static string ExePath()
		{
			var exeDir = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
			var exePath = Path.Combine(exeDir, "EncConverterHostExe.exe");
			Assert.IsTrue(File.Exists(exePath), $"EncConverterHostExe.exe not found at '{exePath}' - build it first.");
			return exePath;
		}

		private static EncConverterHostRequest MakeAdaptItKeepAliveRequest(int keepAliveSeconds, string sessionPipeName)
		{
			var kbProjectName = "Hindi to English adaptations";
			var pathToAiKbFile = Path.Combine(
				Path.Combine(TestEncConverters.GetTestSourceFolder(), kbProjectName), $"{kbProjectName}.xml");

			return new EncConverterHostRequest
			{
				AssemblyReference = "AIGuesserEC",
				ProgId = "SilEncConverters40.AdaptItEncConverter",
				ConverterName = "AiKbConverter-Hindi",
				ConverterIdentifier = pathToAiKbFile,
				LhsEncodingId = "UNICODE",
				RhsEncodingId = "UNICODE",
				ConversionType = ConvType.Unicode_to_from_Unicode,
				ProcessTypeFlags = (int)ProcessTypeFlags.DontKnow,
				// InputText is ignored when KeepAliveSeconds > 0 - left null deliberately, to make
				// sure nothing relies on it by accident.
				KeepAliveSeconds = keepAliveSeconds,
				SessionPipeName = sessionPipeName,
			};
		}

		/// <summary>Launches the exe in keep-alive mode and returns the Process immediately - nothing
		/// is written to stdout in this mode (see Program.cs/Handover.md), so there's nothing to read
		/// before proceeding to connect a pipe client.</summary>
		private static Process LaunchKeepAliveSession(EncConverterHostRequest request)
		{
			var requestFilespec = Path.GetTempFileName();
			File.WriteAllText(requestFilespec, JsonConvert.SerializeObject(request));

			var psi = new ProcessStartInfo(ExePath(), $"\"{requestFilespec}\"")
			{
				UseShellExecute = false,
				CreateNoWindow = true,
				RedirectStandardOutput = true,
				RedirectStandardError = true,
				StandardOutputEncoding = Encoding.Unicode,
				StandardErrorEncoding = Encoding.Unicode,
			};

			return Process.Start(psi);
		}

		private static EncConverterHostPipeResponse ReceivePipeResponse(PipeStream pipe)
		{
			using (var ms = new MemoryStream())
			{
				var buffer = new byte[4096];
				do
				{
					int n = pipe.Read(buffer, 0, buffer.Length);
					Assert.Greater(n, 0, "pipe closed before a complete response was received");
					ms.Write(buffer, 0, n);
				}
				while (!pipe.IsMessageComplete);

				return JsonConvert.DeserializeObject<EncConverterHostPipeResponse>(
					Encoding.UTF8.GetString(ms.ToArray()));
			}
		}

		private static EncConverterHostPipeResponse SendPipeRequest(PipeStream pipe, EncConverterHostPipeRequest request)
		{
			var requestBytes = Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(request));
			pipe.Write(requestBytes, 0, requestBytes.Length);
			pipe.Flush();
			pipe.WaitForPipeDrain();

			return ReceivePipeResponse(pipe);
		}

		#endregion Helpers

		[Test]
		public void TestKeepAliveSession_ReusesSameProcessAcrossCalls()
		{
			var sessionPipeName = "EncConverterHostExe-Test-" + Guid.NewGuid();
			var request = MakeAdaptItKeepAliveRequest(keepAliveSeconds: 30, sessionPipeName);

			var process = LaunchKeepAliveSession(request);
			try
			{
				using (var client = new NamedPipeClientStream(".", sessionPipeName, PipeDirection.InOut))
				{
					// Connect() retries internally until the timeout, so no explicit wait for the
					// child to reach Initialize()/WaitForConnection() is needed here.
					client.Connect(10000);
					// NamedPipeClientStream does NOT inherit PipeTransmissionMode.Message from the
					// server automatically - this must be set explicitly, or IsMessageComplete below
					// won't mean what it's supposed to.
					client.ReadMode = PipeTransmissionMode.Message;

					// one-time handshake, sent by the server immediately after accepting the connection
					// and before any conversion request - see EncConverterHostPipeResponse's Adjusted*
					// doc comment. Must be consumed here or every response below is read one message
					// behind where the client thinks it is.
					var handshake = ReceivePipeResponse(client);
					Assert.IsTrue(handshake.Success, handshake.ErrorMessage);

					// three conversions against the same session, including what would have been
					// "the first" call in one-shot mode.
					var response1 = SendPipeRequest(client, new EncConverterHostPipeRequest { InputText = AdaptItInput });
					Assert.IsTrue(response1.Success, response1.ErrorMessage);
					Assert.AreEqual(AdaptItExpectedOutput, response1.Result);

					var response2 = SendPipeRequest(client, new EncConverterHostPipeRequest { InputText = AdaptItInput });
					Assert.IsTrue(response2.Success, response2.ErrorMessage);
					Assert.AreEqual(AdaptItExpectedOutput, response2.Result);

					var response3 = SendPipeRequest(client, new EncConverterHostPipeRequest { InputText = AdaptItInput });
					Assert.IsTrue(response3.Success, response3.ErrorMessage);
					Assert.AreEqual(AdaptItExpectedOutput, response3.Result);

					// the deterministic, non-timing-based proof of reuse: every response came from the
					// exact same OS process this test launched.
					Assert.AreEqual(process.Id, response1.ServerProcessId,
						"the pipe-served response should come from the same process the caller launched.");
					Assert.AreEqual(response1.ServerProcessId, response2.ServerProcessId,
						"every pipe-served response in one session should come from the same process.");
					Assert.AreEqual(response1.ServerProcessId, response3.ServerProcessId,
						"every pipe-served response in one session should come from the same process.");

					var shutdownResponse = SendPipeRequest(client, new EncConverterHostPipeRequest { Shutdown = true });
					Assert.IsTrue(shutdownResponse.Success);
				}

				Assert.IsTrue(process.WaitForExit(5000), "the process should exit promptly after a Shutdown request.");
				Assert.AreEqual(0, process.ExitCode);
			}
			finally
			{
				// backstop: a failed assertion above must never leak a live subprocess across test runs.
				if (!process.HasExited)
					process.Kill();
			}
		}

		[Test]
		public void TestKeepAliveSession_SelfTerminatesWhenIdle()
		{
			var sessionPipeName = "EncConverterHostExe-Test-" + Guid.NewGuid();
			// short on purpose - this test's whole point is proving the idle timer actually fires,
			// not exercising a realistic production timeout.
			var request = MakeAdaptItKeepAliveRequest(keepAliveSeconds: 2, sessionPipeName);

			var process = LaunchKeepAliveSession(request);
			try
			{
				// deliberately never connect a pipe client at all - prove the orphaned-child
				// self-termination property: a session nobody ever talks to still expires on schedule
				// (the idle timer starts counting from process start, not from first contact).
				Assert.IsTrue(process.WaitForExit(10000),
					"the process should self-terminate once idle longer than KeepAliveSeconds, without anything else killing it.");
				Assert.AreEqual(0, process.ExitCode, "self-termination via the idle timer should be a clean exit.");
			}
			finally
			{
				if (!process.HasExited)
					process.Kill();
			}
		}
	}
}
