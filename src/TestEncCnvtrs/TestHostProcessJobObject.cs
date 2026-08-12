// Exercises HostProcessJobObject (see Handover.md, "Generic subprocess fallback for host-process
// conflicts") -- the Windows Job Object that ties an EncConverterHostExe.exe child's lifetime to
// this process's own, so an uncleanly-killed host (a crash, or being forcibly terminated) takes the
// child down immediately rather than leaving it orphaned for up to its own idle timeout.
//
// [Explicit] because this test's whole point is to have *this test process itself* hard-killed from
// the outside mid-run (simulating the host-crash scenario no amount of Dispose()/finally/finalizer
// code can react to) -- it's not something a normal test run should ever do to itself. Driven
// manually via:
//   dotnet test TestEncCnvtrs.dll --filter "FullyQualifiedName~LaunchSessionAndBlock_ForExternalCrashKillVerification"
// while an external script (see Handover.md for the exact steps used to verify this) watches for the
// EncConverterHostExe.exe child to appear, force-kills *this* test process's own host (not a graceful
// stop), and confirms the child disappears promptly rather than surviving for its full idle timeout.

using System;
using System.IO;
using System.Reflection;
using System.Threading;

using NUnit.Framework;

using ECInterfaces;
using SilEncConverters40;

namespace TestEncCnvtrs
{
	[TestFixture]
	public class TestHostProcessJobObject
	{
		[Test]
		[Explicit("Manually driven: this process gets hard-killed externally mid-run - see Handover.md, "
			+ "\"Generic subprocess fallback for host-process conflicts\", for the exact verification steps.")]
		public void LaunchSessionAndBlock_ForExternalCrashKillVerification()
		{
			var exeDir = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
			var exePath = Path.Combine(exeDir, "EncConverterHostExe.exe");
			Assert.IsTrue(File.Exists(exePath), $"EncConverterHostExe.exe not found at '{exePath}' - build it first.");

			using (var proxy = new SubprocessEncConverter(exePath, "IcuEC", "SilEncConverters40.IcuTranslitEncConverter"))
			{
				string lhsEncodingId = "UNICODE";
				string rhsEncodingId = "UNICODE";
				var conversionType = ConvType.Unicode_to_from_Unicode;
				int processTypeFlags = (int)ProcessTypeFlags.ICUTransliteration;

				proxy.Initialize("Latin-Greek", "Latin-Greek", ref lhsEncodingId, ref rhsEncodingId,
					ref conversionType, ref processTypeFlags, 0, 0, bAdding: true);

				// prove the child is actually up (and thus actually assigned to the job) before blocking.
				Assert.AreEqual("ἀβγ", proxy.Convert("abg"));

				// long enough for an external script to find this process, hard-kill it, and observe the
				// child's fate - nothing about this sleep duration matters to the test itself, since it's
				// never expected to complete normally in this manually-driven scenario.
				Thread.Sleep(TimeSpan.FromMinutes(10));
			}
		}
	}
}
