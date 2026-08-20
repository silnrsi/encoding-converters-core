// Unit tests for the pieces of the generic subprocess fallback (see Handover.md, "Generic
// subprocess fallback for host-process conflicts") that TestEncConverterHost.cs/
// TestEncConverterHostKeepAlive.cs don't already cover: the proactive detection utility
// (NativeModuleConflictDetector), the caller-side IEncConverter proxy (SubprocessEncConverter) that
// EncConverters.AddEx hands back instead of initializing a risky converter in-process, and the
// reactive net's registry cache (EncConverters.IsKnownToNeedSubprocessFallback/
// RememberNeedsSubprocessFallback). All are `internal` to SilEncConverters40 -- see its
// AssemblyInfo.cs InternalsVisibleTo, which is why this project is now signed with the same key.
//
// AddEx's own wiring (the proactive scan/cache check plus the reactive catch-and-remember net) is
// deliberately NOT re-exercised end-to-end here: doing so would require driving the full
// EncConverters XML-repository round trip with a fabricated always-risky converter type, which adds
// a lot of scaffolding for a code path that's a thin, easily-reviewed composition of the pieces
// tested directly below.

using System;
using System.Diagnostics;
using System.IO;
using System.Reflection;

using Microsoft.Win32;
using NUnit.Framework;

using ECInterfaces;
using SilEncConverters40;

namespace TestEncCnvtrs
{
	[TestFixture]
	public class TestSubprocessFallback
	{
		#region NativeModuleConflictDetector

		[Test]
		public void IsLoadedFromElsewhere_ModuleNotLoadedAtAll_ReturnsFalse()
		{
			// nothing resident to lose the DLL-search-order race against -- safe by definition.
			Assert.IsFalse(NativeModuleConflictDetector.IsLoadedFromElsewhere(
				"ThisDllDefinitelyDoesNotExist_" + Guid.NewGuid() + ".dll", @"C:\SomeDirectory"));
		}

		[Test]
		public void IsLoadedFromElsewhere_LoadedFromExpectedDirectory_ReturnsFalse()
		{
			// kernel32.dll is always loaded, from a directory we can independently confirm
			// (Environment.SystemDirectory) -- a deterministic stand-in for "a previous instantiation
			// in this process already loaded the correct copy of a risky dependency".
			Assert.IsFalse(NativeModuleConflictDetector.IsLoadedFromElsewhere("kernel32.dll", Environment.SystemDirectory));
		}

		[Test]
		public void IsLoadedFromElsewhere_LoadedFromDifferentDirectory_ReturnsTrue()
		{
			// same always-loaded module, but claim "our own directory" is somewhere it manifestly
			// isn't -- the foreign-copy-already-resident case this detector exists to catch (e.g.
			// Word's own WebView2Loader.dll instead of ours).
			var notOwnDirectory = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
			Assert.AreNotEqual(Environment.SystemDirectory, notOwnDirectory, "test setup sanity check");
			Assert.IsTrue(NativeModuleConflictDetector.IsLoadedFromElsewhere("kernel32.dll", notOwnDirectory));
		}

		#endregion NativeModuleConflictDetector

		#region SubprocessEncConverter

		private static string ExePath()
		{
			var exeDir = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
			var exePath = Path.Combine(exeDir, "EncConverterHostExe.exe");
			Assert.IsTrue(File.Exists(exePath), $"EncConverterHostExe.exe not found at '{exePath}' - build it first.");
			return exePath;
		}

		[Test]
		public void Convert_RoutesThroughSubprocessTransparently()
		{
			// same converter/spec/direction/expected-output TestEncConverterHost's own
			// TestIcuTranslitEncConverter_ViaSubprocess already verified end-to-end against this exact
			// exe -- this test's job is only to prove SubprocessEncConverter's IEncConverter surface
			// (Initialize/Convert) forwards correctly, not to re-prove the wire protocol itself.
			using (var proxy = new SubprocessEncConverter(ExePath(), "IcuEC", "SilEncConverters40.IcuTranslitEncConverter"))
			{
				string lhsEncodingId = "UNICODE";
				string rhsEncodingId = "UNICODE";
				var conversionType = ConvType.Unicode_to_from_Unicode;
				int processTypeFlags = (int)ProcessTypeFlags.ICUTransliteration;

				proxy.Initialize("Latin-Greek", "Latin-Greek", ref lhsEncodingId, ref rhsEncodingId,
					ref conversionType, ref processTypeFlags, 0, 0, bAdding: true);

				Assert.AreEqual("ἀβγ", proxy.Convert("abg"),
					"SubprocessEncConverter should forward Convert() through the child process and return its real result.");

				// a second call on the same proxy instance should keep working (i.e. the underlying
				// PersistentEncConverterHostSession's launch-on-demand/reconnect logic isn't a
				// one-shot-only affair) -- reuse itself (same server process) is already proven at the
				// wire-protocol level by TestEncConverterHostKeepAlive.cs.
				Assert.AreEqual("ἀβγ", proxy.Convert("abg"));
			}
		}

		[Test]
		public void Convert_BadProgId_ThrowsRatherThanHangingOrSucceeding()
		{
			// a bad ProgID fails inside the child during InstantiateConverter, before it ever calls
			// pipeServer.WaitForConnection() (see Program.cs) -- so the child exits immediately with a
			// stderr message, and the parent finds out either via NamedPipeClientStream.Connect()
			// timing out (~10s -- PersistentEncConverterHostSession's connect timeout, the common case)
			// or an IOException from a Write() racing the child's exit (occasionally observed after
			// Program.cs switched to HardExit/TerminateProcess for a hard, immediate kill -- the OS can
			// tear down the named pipe object at any point mid-connection-attempt, so the client
			// sometimes sees "briefly connectable, then broken" instead of "never available"). Either
			// way this still fails loudly and deterministically, which is the property this test
			// actually cares about (never hangs forever, never silently "succeeds") -- not which
			// specific exception type reports it.
			using (var proxy = new SubprocessEncConverter(ExePath(), "IcuEC", "SilEncConverters40.NoSuchConverterType"))
			{
				string lhsEncodingId = "UNICODE";
				string rhsEncodingId = "UNICODE";
				var conversionType = ConvType.Unicode_to_from_Unicode;
				int processTypeFlags = 0;

				proxy.Initialize("bad", "bad", ref lhsEncodingId, ref rhsEncodingId,
					ref conversionType, ref processTypeFlags, 0, 0, bAdding: true);

				var ex = Assert.Catch(() => proxy.Convert("abg"));
				Assert.IsTrue(ex is TimeoutException || ex is IOException,
					$"Expected TimeoutException or IOException, but got {ex.GetType()}: {ex.Message}");
			}
		}

		#endregion SubprocessEncConverter

		#region Reactive-net registry cache

		[Test]
		public void RememberAndIsKnown_RoundTrip()
		{
			// a unique host/progId pair per run so this can never collide with a real cache entry (or a
			// leftover from a previous failed run) and false-positive.
			var hostProcessName = "TestHost_" + Guid.NewGuid().ToString("N");
			var progId = "Test.Fake.ProgId." + Guid.NewGuid().ToString("N");

			try
			{
				Assert.IsFalse(EncConverters.IsKnownToNeedSubprocessFallback(hostProcessName, progId),
					"a combination that was never remembered should not read back as known.");

				EncConverters.RememberNeedsSubprocessFallback(hostProcessName, progId);

				Assert.IsTrue(EncConverters.IsKnownToNeedSubprocessFallback(hostProcessName, progId),
					"a combination that was just remembered should read back as known -- via HKLM if this "
					+ "process has write access to it, or via the HKCU fallback (see RememberNeedsSubprocessFallback) "
					+ "if not; either way the round trip should succeed without the caller needing to know which.");
			}
			finally
			{
				// best-effort cleanup under both hives -- whichever one the write actually landed in.
				DeleteTestSubKey(Registry.LocalMachine, hostProcessName);
				DeleteTestSubKey(Registry.CurrentUser, hostProcessName);
			}
		}

		private static void DeleteTestSubKey(RegistryKey hive, string hostProcessName)
		{
			try
			{
				hive.DeleteSubKeyTree($@"SOFTWARE\SIL\SilEncConverters40\SubprocessFallback\{hostProcessName}", throwOnMissingSubKey: false);
			}
			catch
			{
				// best-effort cleanup only -- an orphaned test key under a throwaway GUID name is
				// harmless noise, not a correctness problem.
			}
		}

		#endregion Reactive-net registry cache

		#region Same-call self-heal (EncConverter.Convert/ConvertEx + lazily-loaded resources)

		[Test]
		public void Convert_SelfHealsOnFirstFailure_WhenRiskyConverterThrowsInProcess()
		{
			// AlwaysRiskyTestEncConverter stands in for TechHindiSiteEncConverter's real-world failure
			// mode (see its own doc comment): it declares a risky dependency and throws on every
			// in-process attempt, succeeding only when run as EncConverterHostExe.exe. This proves
			// EncConverter.Convert() itself -- not just SubprocessEncConverter/AddEx in isolation --
			// catches that failure and transparently retries via the subprocess fallback, within the
			// SAME call, with no visible error to the caller at all.
			var exeDir = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
			Assert.IsTrue(File.Exists(Path.Combine(exeDir, "EncConverterHostExe.exe")),
				"EncConverterHostExe.exe not found - build it first.");

			// HKCU, not HKLM: creating a brand-new key under HKLM\SOFTWARE\SIL requires elevation this
			// test shouldn't need, and EncConverters.GetRegistryValue (which FindEncConverterHostExePath
			// uses) already checks HKCU before HKLM, so this is picked up identically either way.
			var installDirKey = Registry.CurrentUser.CreateSubKey(@"SOFTWARE\SIL\SILConverters");
			var previousInstallDir = installDirKey.GetValue("InstallDir") as string;
			try
			{
				// points EncConverters.FindEncConverterHostExePath (which AlwaysRiskyTestEncConverter's
				// base-class Convert() consults internally once it needs to fall back) at this test
				// run's own output folder, where EncConverterHostExe.exe already sits -- exactly what
				// the real installer's InstallDir value points at in production.
				installDirKey.SetValue("InstallDir", exeDir);

				var converter = new AlwaysRiskyTestEncConverter();
				string lhs = "UNICODE", rhs = "UNICODE";
				var conversionType = ConvType.Unicode_to_from_Unicode;
				int processTypeFlags = 0;
				converter.Initialize("SelfHealTest", "unused", ref lhs, ref rhs, ref conversionType,
					ref processTypeFlags, 0, 0, bAdding: true);

				// the in-process attempt is designed to throw -- Convert() should catch that, engage
				// the subprocess fallback, and retry transparently, returning the real result from a
				// single call with no exception ever reaching this test.
				Assert.AreEqual("abc", converter.Convert("abc"),
					"Convert() should have caught the in-process failure and self-healed via the subprocess fallback.");

				// a second call on the same instance should go straight to the now-cached proxy,
				// without re-attempting (and re-throwing from) the doomed in-process path again.
				Assert.AreEqual("xyz", converter.Convert("xyz"));
			}
			finally
			{
				try
				{
					if (previousInstallDir != null)
						installDirKey.SetValue("InstallDir", previousInstallDir);
					else
						installDirKey.DeleteValue("InstallDir", throwOnMissingValue: false);
				}
				catch
				{
					// best-effort cleanup only.
				}

				DeleteTestSubKey(Registry.LocalMachine, Process.GetCurrentProcess().ProcessName);
				DeleteTestSubKey(Registry.CurrentUser, Process.GetCurrentProcess().ProcessName);
			}
		}

		#endregion Same-call self-heal
	}
}
