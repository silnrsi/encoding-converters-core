// Exercises EncConverterHostExe.exe (see Handover.md, "Generic subprocess fallback for
// host-process conflicts") - the generic, engine-agnostic subprocess host that any
// IEncConverter implementation can eventually be routed through if a native dependency
// conflict is detected. None of that detection/routing exists yet, so these tests drive the
// host exe directly, pretending some caller decided a subprocess was necessary - one test
// per transducer type, to prove the pathway genuinely generalizes across every kind of engine
// this repo ships (COM-visible core converters, native P/Invoke wrappers, an embedded
// scripting engine, and cloud translators), not just the WebView2-based one that motivated it.
//
// Every test follows the same shape: build an EncConverterHostRequest (the same tuple
// IEncConverter.Initialize()/EncConverters.InstantiateIEncConverter() already require - no
// EncConverters dispatcher/repository involved), serialize it to a temp file, run
// EncConverterHostExe.exe against it, and check stdout/exit code - never touching the
// converter type in-process ourselves.

using System;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using System.Text;

using NUnit.Framework;
using Newtonsoft.Json;

using ECInterfaces;
using SilEncConverters40;
using SilEncConverters40.EncConverterHostExe;

namespace TestEncCnvtrs
{
	[TestFixture]
	public class TestEncConverterHost
	{
		#region Helpers

		private static (int ExitCode, string StdOut, string StdErr) RunEncConverterHostExe(EncConverterHostRequest request)
		{
			var exeDir = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
			var exePath = Path.Combine(exeDir, "EncConverterHostExe.exe");
			Assert.IsTrue(File.Exists(exePath), $"EncConverterHostExe.exe not found at '{exePath}' - build it first.");

			// EncConverterHostExe deletes this itself once it's read it - see Program.cs.
			var requestFilespec = Path.GetTempFileName();
			File.WriteAllText(requestFilespec, JsonConvert.SerializeObject(request));

			var psi = new ProcessStartInfo(exePath, $"\"{requestFilespec}\"")
			{
				UseShellExecute = false,
				CreateNoWindow = true,
				RedirectStandardOutput = true,
				RedirectStandardError = true,
				StandardOutputEncoding = Encoding.Unicode,
				StandardErrorEncoding = Encoding.Unicode,
			};

			using (var process = Process.Start(psi))
			{
				var stdOut = process.StandardOutput.ReadToEnd();
				var stdErr = process.StandardError.ReadToEnd();
				process.WaitForExit();
				return (process.ExitCode, stdOut, stdErr);
			}
		}

		private static string ResolvePythonDllPath()
		{
			// same two candidate locations TestPy3ScriptEncConverters.cs already checks
			const string pathPython64 = @"C:\Users\pete_\AppData\Local\Programs\Python\Python39\Python39.dll";
			const string pathPython86 = @"C:\Python37-32\Python37.dll";

			if (Environment.Is64BitProcess && File.Exists(pathPython64))
				return pathPython64;
			if (!Environment.Is64BitProcess && File.Exists(pathPython86))
				return pathPython86;

			Assert.Fail($"Neither '{pathPython64}' nor '{pathPython86}' was found - install Python 3 to run this test (see TestPy3ScriptEncConverters.cs).");
			return null;
		}

		#endregion Helpers

		[Test]
		public void TestBingTranslatorEncConverter_ViaSubprocess()
		{
			// converterSpec format ("Translate;<from>;<to>") and sample data straight from
			// TestEcTranslators.cs's own [TestCase]s.
			var request = new EncConverterHostRequest
			{
				AssemblyReference = "EcTranslators",
				ProgId = "SilEncConverters40.EcTranslators.BingTranslator.BingTranslatorEncConverter",
				ConverterName = "Bing-Hindi-To-English",
				ConverterIdentifier = "Translate;hi;en",
				LhsEncodingId = "UNICODE",
				RhsEncodingId = "UNICODE",
				ConversionType = ConvType.Unicode_to_Unicode,
				ProcessTypeFlags = (int)ProcessTypeFlags.Translation,
				InputText = "यीशु ने यह भी कहा,",
			};

			var (exitCode, stdOut, stdErr) = RunEncConverterHostExe(request);

			// relies on the built-in shared subscription key, same as TestEcTranslators.cs - may
			// fail if that resource no longer has any remaining juice (see that file's own comment).
			Assert.AreEqual(0, exitCode, $"EncConverterHostExe failed: {stdErr}");
			Assert.AreEqual("Jesus also said,", stdOut,
				"BingTranslatorEncConverter should translate Hindi to English properly via the subprocess host.");
		}

		[Test]
		public void TestDeepLTranslatorEncConverter_ViaSubprocess()
		{
			var request = new EncConverterHostRequest
			{
				AssemblyReference = "EcTranslators",
				ProgId = "SilEncConverters40.EcTranslators.DeepLTranslator.DeepLTranslatorEncConverter",
				ConverterName = "DeepL-English-To-French",
				ConverterIdentifier = "Translate;en;fr",
				LhsEncodingId = "UNICODE",
				RhsEncodingId = "UNICODE",
				ConversionType = ConvType.Unicode_to_Unicode,
				ProcessTypeFlags = (int)ProcessTypeFlags.Translation,
				InputText = "Hello, world!",
			};

			var (exitCode, stdOut, stdErr) = RunEncConverterHostExe(request);

			// relies on the built-in shared subscription key, same as TestEcTranslators.cs - may
			// fail if that resource no longer has any remaining juice.
			Assert.AreEqual(0, exitCode, $"EncConverterHostExe failed: {stdErr}");
			Assert.AreEqual("Bonjour, tout le monde !", stdOut,
				"DeepLTranslatorEncConverter should translate English to French properly via the subprocess host.");
		}

		[Test]
		public void TestGoogleTranslatorEncConverter_ViaSubprocess()
		{
			var request = new EncConverterHostRequest
			{
				AssemblyReference = "EcTranslators",
				ProgId = "SilEncConverters40.EcTranslators.GoogleTranslator.GoogleTranslatorEncConverter",
				ConverterName = "Google-English-To-French",
				ConverterIdentifier = "en;fr",
				LhsEncodingId = "UNICODE",
				RhsEncodingId = "UNICODE",
				ConversionType = ConvType.Unicode_to_Unicode,
				ProcessTypeFlags = (int)ProcessTypeFlags.Translation,
				InputText = "Hello, world!",
			};

			var (exitCode, stdOut, stdErr) = RunEncConverterHostExe(request);

			// deliberately doesn't set GOOGLE_APPLICATION_CREDENTIALS (unlike TestEcTranslators.cs,
			// which points it at a path only present on the original dev's machine) - relies on the
			// same built-in encrypted fallback key GoogleTranslatorEncConverter falls back to when
			// nothing else is configured.
			Assert.AreEqual(0, exitCode, $"EncConverterHostExe failed: {stdErr}");
			Assert.AreEqual("Bonjour le monde!", stdOut,
				"GoogleTranslatorEncConverter should translate English to French properly via the subprocess host.");
		}

		[Test]
		public void TestPy3ScriptEncConverter_ViaSubprocess()
		{
			var scriptPath = Path.Combine(TestEncConverters.GetMapsTablesFolder(), "PythonExamples", "ReverseString.py");
			var pythonDllPath = ResolvePythonDllPath();

			var request = new EncConverterHostRequest
			{
				AssemblyReference = "PyScriptEC",
				ProgId = "SilEncConverters40.Py3ScriptEncConverter",
				ConverterName = "ReverseString",
				ConverterIdentifier = $"{scriptPath};{pythonDllPath}",
				LhsEncodingId = "UNICODE",
				RhsEncodingId = "UNICODE",
				ConversionType = ConvType.Unicode_to_from_Unicode,
				ProcessTypeFlags = (int)ProcessTypeFlags.PythonScript,
				InputText = "abcde",
			};

			var (exitCode, stdOut, stdErr) = RunEncConverterHostExe(request);

			Assert.AreEqual(0, exitCode, $"EncConverterHostExe failed: {stdErr}");
			Assert.AreEqual("edcba", stdOut,
				"Py3ScriptEncConverter (ReverseString.py) should convert data properly via the subprocess host.");
		}

		[Test]
		public void TestIcuTranslitEncConverter_ViaSubprocess()
		{
			// same converter/spec/direction TestIcuTranslit.cs already exercises in-process, and the
			// same input/output already verified end-to-end against this exact exe by hand.
			var request = new EncConverterHostRequest
			{
				AssemblyReference = "IcuEC",
				ProgId = "SilEncConverters40.IcuTranslitEncConverter",
				ConverterName = "Latin-Greek",
				ConverterIdentifier = "Latin-Greek",
				LhsEncodingId = "UNICODE",
				RhsEncodingId = "UNICODE",
				ConversionType = ConvType.Unicode_to_from_Unicode,
				ProcessTypeFlags = (int)ProcessTypeFlags.ICUTransliteration,
				InputText = "abg",
			};

			var (exitCode, stdOut, stdErr) = RunEncConverterHostExe(request);

			Assert.AreEqual(0, exitCode, $"EncConverterHostExe failed: {stdErr}");
			Assert.AreEqual("ἀβγ", stdOut,
				"IcuTranslitEncConverter (Latin-Greek) should convert data properly via the subprocess host.");
		}

		[Test]
		public void TestNetRegexEncConverter_ViaSubprocess()
		{
			// same converterSpec/input/output as TestNetRegex.cs's TestRegexToVandC.
			var request = new EncConverterHostRequest
			{
				AssemblyReference = "SilEncConverters40",
				ProgId = "SilEncConverters40.NetRegexEncConverter",
				ConverterName = "Vowels->V",
				ConverterIdentifier = "{[aeiou]}->{V};1",
				LhsEncodingId = "UNICODE",
				RhsEncodingId = "UNICODE",
				ConversionType = ConvType.Unicode_to_from_Unicode,
				ProcessTypeFlags = (int)ProcessTypeFlags.ICURegularExpression,
				InputText = "abcdEfGhijklMnopqrstUwxyz",
			};

			var (exitCode, stdOut, stdErr) = RunEncConverterHostExe(request);

			Assert.AreEqual(0, exitCode, $"EncConverterHostExe failed: {stdErr}");
			Assert.AreEqual("VbcdVfGhVjklMnVpqrstVwxyz", stdOut,
				"NetRegexEncConverter should convert data properly via the subprocess host.");
		}

		[Test]
		public void TestIcuBreakIteratorEncConverter_ViaSubprocess()
		{
			// same converterSpec (its own DefaultSeparator, a single space) and Thai sample data as
			// TestEncConverters.cs's TestIcuBreakIteratorConverter.
			var request = new EncConverterHostRequest
			{
				AssemblyReference = "IcuEC",
				ProgId = "SilEncConverters40.IcuBreakIteratorEncConverter",
				ConverterName = "ICU Boundary Analysis/Break Iterator",
				ConverterIdentifier = " ",
				LhsEncodingId = "UNICODE",
				RhsEncodingId = "UNICODE",
				ConversionType = ConvType.Unicode_to_from_Unicode,
				ProcessTypeFlags = (int)ProcessTypeFlags.DontKnow,
				InputText = "พักหลังๆนี่เวลาแก๊นจะตัดสินใจซื้ออะไรซักอย่างที่มันมีราคา จะคิดแล้วคิดอีก อย่างน้อยก็ทิ้งเวลาไว้ตั้งแต่",
			};

			var (exitCode, stdOut, stdErr) = RunEncConverterHostExe(request);

			Assert.AreEqual(0, exitCode, $"EncConverterHostExe failed: {stdErr}");
			Assert.AreEqual(
				"พัก หลังๆ นี่ เวลา แก๊น จะ ตัดสิน ใจ ซื้อ อะไร ซัก อย่าง ที่ มัน มี ราคา จะ คิด แล้ว คิด อีก อย่าง น้อย ก็ ทิ้ง เวลา ไว้ ตั้งแต่",
				stdOut,
				"IcuBreakIteratorEncConverter should insert word breaks properly via the subprocess host.");
		}

		[Test]
		public void TestAdaptItEncConverter_ViaSubprocess()
		{
			// same AdaptIt KB project and sample data as TestEncConverters.cs's
			// [TestCase("UnitTesting-AiKbConverter-Hindi", "Hindi to English adaptations", ...)].
			var kbProjectName = "Hindi to English adaptations";
			var pathToAiKbFile = Path.Combine(
				Path.Combine(TestEncConverters.GetTestSourceFolder(), kbProjectName), $"{kbProjectName}.xml");

			var request = new EncConverterHostRequest
			{
				AssemblyReference = "AIGuesserEC",
				ProgId = "SilEncConverters40.AdaptItEncConverter",
				ConverterName = "AiKbConverter-Hindi",
				ConverterIdentifier = pathToAiKbFile,
				LhsEncodingId = "UNICODE",
				RhsEncodingId = "UNICODE",
				ConversionType = ConvType.Unicode_to_from_Unicode,
				ProcessTypeFlags = (int)ProcessTypeFlags.DontKnow,
				InputText = "यह, परीक्षा है?",
			};

			var (exitCode, stdOut, stdErr) = RunEncConverterHostExe(request);

			Assert.AreEqual(0, exitCode, $"EncConverterHostExe failed: {stdErr}");
			Assert.AreEqual("%3%this%she%he%, %2%examination%test% %2%PRES%is%?", stdOut,
				"AdaptItEncConverter should look up KB adaptations properly via the subprocess host.");
		}

		[Test]
		public void TestTecEncConverter_TecFlavor_ViaSubprocess()
		{
			RunSenufoTecTest("Senufo.tec");
		}

		[Test]
		public void TestTecEncConverter_MapFlavor_ViaSubprocess()
		{
			// same TecEncConverter class/ProgID either way - Initialize() itself switches behavior
			// based on the ConverterIdentifier's file extension (see Handover.md/TecEncConverter.cs).
			RunSenufoTecTest("Senufo.map");
		}

		private static void RunSenufoTecTest(string tableFilename)
		{
			var pathToTable = Path.Combine(TestEncConverters.GetTestSourceFolder(), tableFilename);

			var request = new EncConverterHostRequest
			{
				AssemblyReference = "SilEncConverters40",
				ProgId = "SilEncConverters40.TecEncConverter",
				ConverterName = $"Senufo-To-Unicode ({tableFilename})",
				ConverterIdentifier = pathToTable,
				LhsEncodingId = "LEGACY",
				RhsEncodingId = "UNICODE",
				ConversionType = ConvType.Legacy_to_from_Unicode,
				ProcessTypeFlags = (int)ProcessTypeFlags.UnicodeEncodingConversion,
				CodePageInput = EncConverters.cnIso8859_1CodePage,
				// same byte array (with its intentional gaps) as TestEncConverters.cs's m_bytesSenufo
				InputText = TestUtil.GetPseudoStringFromBytes(new byte[] {
					 32,  33,  34,  35,  36,  37,  38,  39,  40,  41,  42,  43,  44,  45,  46,  47,
					 48,  49,  50,  51,  52,  53,  54,  55,  56,  57,  58,  59,  60,  61,  62,  63,
					      65,  66,  67,  68,  69,  70,  71,  72,  73,  74,  75,  76,  77,  78,  79,
					 80,  81,  82,  83,  84,  85,  86,  87,  88,  89,  90,  91,  92,  93,  94,  95,
					 96,  97,  98,  99, 100, 101, 102, 103, 104, 105, 106, 107, 108, 109, 110, 111,
					112, 113, 114, 115, 116, 117, 118, 119, 120, 121, 122, 123, 124, 125, 126, 127,
					     129, 130, 131, 132, 133, 134, 135, 136, 137, 138, 139, 140, 141, 142, 143,
					144, 145, 146, 147, 148, 149, 150, 151, 152, 153, 154, 155, 156, 157, 158, 159,
					160, 161, 162, 163, 164, 165, 166, 167, 168, 169, 170, 171, 172, 173, 174, 175,
					176, 177, 178, 179, 180, 181, 182, 183, 184, 185, 186, 187, 188, 189, 190, 191,
					192, 193, 194, 195, 196, 197, 198,      200, 201, 202, 203, 204, 205, 206, 207,
					208, 209, 210, 211, 212, 213, 214, 215, 216, 217, 218, 219, 220, 221, 222, 223,
					224, 225, 226, 227, 228, 229, 230, 231, 232, 233, 234, 235, 236, 237, 238, 239,
					240, 241, 242, 243, 244, 245, 246, 247, 248, 249,      251, 252, 253, 254, 255
				}),
			};

			var (exitCode, stdOut, stdErr) = RunEncConverterHostExe(request);

			const string expectedOutput = " !\"#$%&'()*+,-./0123456789:;<=>?" +
				"ABCDEFGHIJKLMNOPQRSTUVWXYZ[\\]^_`abcdefghijklmnopqrstuvwxyz{|}~\u007F" +
				"\u01D4\u00E9\u00E2\u01CE\u00E0\u0101\u00E7" +
				"\u00EA\u00EB\u00E8\u00EF\u00EE\u00EC\u011B\u012B" +
				"\u0269\uF173\u025b\u0301\u025B\u0300\u00F4\u014D\u00F2\u00FB\u00F9" +
				"\u016B\u0254\u0301\u0254\u0300\u0254\u00AB\u0254\u014A\u0254\u0302" +
				"\u00A0\u00ED\u00F3\u00FA\u0272\u019D\u025B\u030C\u0254\u030C" +
				"\u0294\u2018\uF173\u0113\u0069\u030C\u006F\u030C\u2039\u203A" +
				"\u02C6\uF171\u00B2\u00B3\u00B4\u0061\uF173\u00B6\u00B7" +
				"\u025B\uF173\u00C1\u201C\u00BB\u00BC\u0069\uF173\u006F\uF173\u2022" +
				"\u200C\u200D\u0065\uF173\u00E1\u0186\u00C0\u0254\uF173" +
				"\u00C8\u00C9\u0069\uF171\u0269\u0301\u0269\u0300\u00CD\u0269\uF171\u00CF" +
				"\u0076\u0300\u2014\u0075\uF171\u0075\uF173\u006F\uF171\u0061\uF171\u0254\uF171\u201D" +
				"\u0065\uF171\u00D9\u2019\u00DB\u00DC\u222B\uF216\u00DF" +
				"\u0269\u0302\u006D\u030C\u006D\u0304\u025B\u0304\u006E\u0304\u1E3F\u006D\u0300\u007A\u0300" +
				"\u0272\u0304\u00E9\u025B\u0302\u014B\u0301\u014B\u0300\u014B\u0304\u025B\u0272\u0301" +
				"\u025B\uF171\u0144\u0272\u0300\u02CA\u00F4\u0269\u00A8\u00AF" +
				"\u01F9\u1E81\u02C7\u014B\u006E\u0302\u0148\u0190";

			Assert.AreEqual(0, exitCode, $"EncConverterHostExe failed: {stdErr}");
			Assert.AreEqual(expectedOutput, stdOut,
				$"TecEncConverter ({tableFilename}) should convert Senufo data properly via the subprocess host.");
		}

		[Test]
		public void TestCcEncConverter_ViaSubprocess()
		{
			// CC has a file length limit, so copy the table to a short temp path first, same as
			// TestEncConverters.cs's GetCcTablePaths.
			var sourcePath = Path.Combine(TestEncConverters.GetTestSourceFolder(), "ann2unicode.cct");
			var tempTablePath = Path.Combine(Path.GetTempPath(), "ann2unicode.cct");
			File.Copy(sourcePath, tempTablePath, true);

			var request = new EncConverterHostRequest
			{
				AssemblyReference = "CcEC",
				ProgId = "SilEncConverters40.CcEncConverter",
				ConverterName = "Ann-To-Unicode",
				ConverterIdentifier = tempTablePath,
				LhsEncodingId = "LEGACY",
				RhsEncodingId = "UNICODE",
				ConversionType = ConvType.Legacy_to_from_Unicode,
				ProcessTypeFlags = (int)ProcessTypeFlags.UnicodeEncodingConversion,
				CodePageInput = EncConverters.cnIso8859_1CodePage,
				InputText = TestUtil.GetPseudoStringFromBytes(new byte[] { 0xE9, 0x4C, 0x83, 0xE7, 0xA2 }),
			};

			var (exitCode, stdOut, stdErr) = RunEncConverterHostExe(request);

			Assert.AreEqual(0, exitCode, $"EncConverterHostExe failed: {stdErr}");
			Assert.AreEqual("\u0915\u093F\u0924\u093E\u092C", stdOut,
				"CcEncConverter (ann2unicode.cct) should convert data properly via the subprocess host.");
		}
	}
}
