using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;

using SilEncConverters40;

namespace TestEncCnvtrs
{
    /// <summary>
    /// A fabricated stand-in for TechHindiSiteEncConverter's real-world failure mode, used by
    /// TestSubprocessFallback's self-heal test (see Handover.md, "Generic subprocess fallback for
    /// host-process conflicts"): declares a risky native dependency and throws on every in-process
    /// conversion attempt, but succeeds when run inside EncConverterHostExe.exe -- the same "works in
    /// a fresh process, fails in this one" shape as the real WebView2Loader.dll collision, without
    /// needing an actual native DLL conflict to reproduce.
    /// Deliberately public (not nested in the test fixture): EncConverterHostExe.exe resolves this via
    /// Activator.CreateInstance(assemblyReference, progId) in a separate process, which requires a
    /// public type with a public parameterless constructor.
    /// </summary>
    public class AlwaysRiskyTestEncConverter : SilEncConverters40.EncConverter
    {
        public const string CstrProgId = "TestEncCnvtrs.AlwaysRiskyTestEncConverter";

        public AlwaysRiskyTestEncConverter() : base(CstrProgId, "Test.AlwaysRisky")
        {
        }

        public override IEnumerable<string> RiskyNativeDependencies => new[] { "SomeFakeRiskyDll.dll" };

        protected override string GetConfigTypeName => null;

        protected override unsafe void DoConvert(byte* lpInBuffer, int nInLen, byte* lpOutBuffer, ref int rnOutLen)
        {
            // "in-process" here really means "not running as the fallback subprocess" -- exactly
            // mirroring the real bug's shape (a host process with something already loaded vs. a fresh
            // process with nothing loaded) without needing an actual native DLL collision to trigger it.
            if (Process.GetCurrentProcess().ProcessName != "EncConverterHostExe")
                throw new ApplicationException("Simulated native-dependency conflict - only works via the subprocess fallback.");

            // trivial passthrough "conversion" (Unicode in, same Unicode out) for the subprocess case
            // that reaches here -- same ECNormalizeData helpers ExeEncConverter's own DoConvert uses
            // for this exact UTF-16 byte-buffer <-> string round trip.
            var baInput = new byte[nInLen];
            ECNormalizeData.ByteStarToByteArr(lpInBuffer, nInLen, baInput);
            var strInput = Encoding.Unicode.GetString(baInput);

            rnOutLen = strInput.Length * 2;
            rnOutLen = ECNormalizeData.StringToByteStar(strInput, lpOutBuffer, rnOutLen, false);
        }
    }
}
