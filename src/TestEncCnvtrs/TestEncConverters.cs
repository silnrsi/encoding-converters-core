// created by Steve McConnel, Feb 7, 2012.

using System;
using System.Collections;
using System.IO;
using System.Text;
using Microsoft.Win32;

using NUnit.Framework;

using ECInterfaces;
using SilEncConverters40;
using System.Reflection;

namespace TestEncCnvtrs
{
	/// ------------------------------------------------------------------------
	/// <summary>
	/// Set of tests to excercise the methods in the EncConverters class.
	/// </summary>
	/// ------------------------------------------------------------------------
	[TestFixture]
	public class TestEncConverters
	{
		EncConverters m_encConverters;
		bool m_fWroteRepoFile;
		bool m_fSetRegistryValue;
		string m_repoFile;

		/// --------------------------------------------------------------------
		/// <summary>
		/// Global initialization, called once before any test in this class
		/// ("fixture") is run.
		/// </summary>
		/// --------------------------------------------------------------------
		[TestFixtureSetUp]
		public void InitForClass()
		{
			if (Util.IsUnix)
			{
				// Make sure we get set up to be able to access Registry.LocalMachine.
				string machine_store = Environment.GetEnvironmentVariable("MONO_REGISTRY_PATH");
				if (machine_store == null)
				{
					Console.WriteLine("First, make sure that /var/lib/fieldworks exists and is writeable by everyone.");
					Console.WriteLine("eg, sudo mkdir -p /var/lib/fieldworks && sudo chmod +wt /var/lib/fieldworks");
					Console.WriteLine("Then add the following line to ~/.profile, logout, and login again.");
					Console.WriteLine("MONO_REGISTRY_PATH=/var/lib/fieldworks/registry; export MONO_REGISTRY_PATH");
					// doesn't work on Maverick, but try it anyway...
					Environment.SetEnvironmentVariable("MONO_REGISTRY_PATH", "/var/lib/fieldworks/registry");
				}
			}
			m_repoFile = null;
			RegistryKey key = Registry.CurrentUser.OpenSubKey(EncConverters.HKLM_PATH_TO_XML_FILE, true);
			if (key != null)
				m_repoFile = key.GetValue(EncConverters.strRegKeyForStorePath) as string;
			else
				key = Registry.CurrentUser.CreateSubKey(EncConverters.HKLM_PATH_TO_XML_FILE);
			if (String.IsNullOrEmpty(m_repoFile) && key != null)
			{
				m_repoFile = EncConverters.DefaultRepositoryPath;
				key.SetValue(EncConverters.strRegKeyForStorePath, m_repoFile);
				key.Flush();
				m_fSetRegistryValue = true;
			}
			if (key != null)
			{
				key.Close();
				key = null;
			}
			if (!String.IsNullOrEmpty(m_repoFile) &&
				File.Exists(m_repoFile) &&
				!File.Exists(m_repoFile + "-RESTOREME"))
			{
				File.Move(m_repoFile, m_repoFile + "-RESTOREME");
			}
			if (!File.Exists(m_repoFile))
				m_fWroteRepoFile = true;
			try
			{
				m_encConverters = new EncConverters();
			}
			catch (Exception e)
			{
				System.Diagnostics.Debug.WriteLine(e.Message);
				Console.WriteLine(e.Message);
			}
		}

		/// --------------------------------------------------------------------
		/// <summary>
		/// Initialization called before each test in this class ("fixture") is
		/// run.
		/// </summary>
		/// --------------------------------------------------------------------
		[SetUp]
		public void InitBeforeTest()
		{
			if (m_encConverters != null)
			{
				RemoveAnyAddedConverters();
				m_encConverters.Reinitialize();
			}
		}

		/// --------------------------------------------------------------------
		/// <summary>
		/// Cleanup called after each test in this class ("fixture") is run.
		/// </summary>
		/// --------------------------------------------------------------------
		[TearDown]
		public void CleanupAfterTest()
		{
			RemoveAnyAddedConverters();
		}

		/// <summary>
		/// Remove any added converters that are left behind due to a test
		/// failure or other crash.
		/// </summary>
		private void RemoveAnyAddedConverters()
		{
			if (m_encConverters.ContainsKey("UnitTesting-ISO-8859-1"))
				m_encConverters.Remove("UnitTesting-ISO-8859-1");
			if (m_encConverters.ContainsKey("UnitTesting-To-ISO-8859-1"))
				m_encConverters.Remove("UnitTesting-To-ISO-8859-1");
			if (m_encConverters.ContainsKey("UnitTesting-Latin-Hebrew"))
				m_encConverters.Remove ("UnitTesting-Latin-Hebrew");
			if (m_encConverters.ContainsKey("UnitTesting-Hebrew-Latin"))
				m_encConverters.Remove ("UnitTesting-Hebrew-Latin");
			if (m_encConverters.ContainsKey("UnitTesting-Consonants->C"))
				m_encConverters.Remove("UnitTesting-Consonants->C");
			if (m_encConverters.ContainsKey("UnitTesting-Vowels->V"))
				m_encConverters.Remove("UnitTesting-Vowels->V");
			if (m_encConverters.ContainsKey("UnitTesting-Senufo-To-Unicode"))
				m_encConverters.Remove("UnitTesting-Senufo-To-Unicode");
			if (m_encConverters.ContainsKey("UnitTesting-Unicode-To-Senufo"))
				m_encConverters.Remove("UnitTesting-Unicode-To-Senufo");
			if (m_encConverters.ContainsKey("UnitTesting-Ann-To-Unicode"))
				m_encConverters.Remove("UnitTesting-Ann-To-Unicode");
			if (m_encConverters.ContainsKey("UnitTesting-Unicode-To-Ann"))
				m_encConverters.Remove("UnitTesting-Unicode-To-Ann");
			if (m_encConverters.ContainsKey("UnitTesting-ReverseString"))
				m_encConverters.Remove("UnitTesting-ReverseString");
			if (m_encConverters.ContainsKey("UnitTesting-Python-1252-To-Unicode"))
				m_encConverters.Remove("UnitTesting-Python-1252-To-Unicode");
			if (m_encConverters.ContainsKey("UnitTesting-Python-Unicode-To-1252"))
				m_encConverters.Remove("UnitTesting-Python-Unicode-To-1252");
			if (m_encConverters.ContainsKey("UnitTesting-From-CP_1252"))
				m_encConverters.Remove("UnitTesting-From-CP_1252");
			if (m_encConverters.ContainsKey("UnitTesting-To-CP_1252"))
				m_encConverters.Remove("UnitTesting-To-CP_1252");
		}

		/// --------------------------------------------------------------------
		/// <summary>
		/// Global cleanup, called once after all tests in this class
		/// ("fixture") have been run.
		/// </summary>
		/// --------------------------------------------------------------------
		[TestFixtureTearDown]
		public void CleanupForClass()
		{
			m_encConverters = null;
			if (m_fWroteRepoFile)
			{
				File.Delete(m_repoFile);
				m_fWroteRepoFile = false;
			}
			if (!String.IsNullOrEmpty(m_repoFile) &&
				File.Exists(m_repoFile + "-RESTOREME"))
			{
				File.Move(m_repoFile + "-RESTOREME", m_repoFile);
			}
			if (m_fSetRegistryValue)
			{
				RegistryKey key = Registry.CurrentUser.OpenSubKey(EncConverters.HKLM_PATH_TO_XML_FILE, true);
				key.DeleteValue(EncConverters.strRegKeyForStorePath);
				key.Flush();
				key.Close();
				m_fSetRegistryValue = false;
			}
			m_repoFile = null;
		}

		[Test]
		public void TestBasics()
		{
			int count = m_encConverters.Count;
			Assert.LessOrEqual(0, count, "The number of converters shouldn't be negative!");
			Assert.IsNotNull(m_encConverters);
			string[] encodings = m_encConverters.Encodings;
			Assert.IsNotNull(encodings);
			string[] mappings = m_encConverters.Mappings;
			Assert.IsNotNull(mappings);
			string[] fonts = m_encConverters.Fonts;
			Assert.IsNotNull(fonts);
			int countKeys = m_encConverters.Keys.Count;
			int countValues = m_encConverters.Values.Count;
			Assert.AreEqual(countKeys, countValues, "Should have same number of keys and values!");
			Assert.LessOrEqual(0, countKeys, "The number of keys and values shouldn't be negative!");
			string[] types;
			string[] names;
			m_encConverters.GetImplementationDisplayNames(out types, out names);
			Assert.IsNotNull(types);
			Assert.IsNotNull(names);
			Assert.AreEqual(types.Length, names.Length);
			Assert.LessOrEqual(1, types.Length, "We must have at least one implementation!");
			var reg = m_encConverters.RepositoryFile;
			Assert.IsNotNull(reg);
		}

		[Test]
		public void TestIcuConvEncConverters()
		{
			int countOrig = m_encConverters.Count;
			m_encConverters.Add("UnitTesting-ISO-8859-1", "ISO-8859-1", ConvType.Legacy_to_from_Unicode,
				"LEGACY", "UNICODE", ProcessTypeFlags.ICUConverter);
			int countNew = m_encConverters.Count;
			Assert.AreEqual(countOrig+1, countNew, "Should have one new converter now");
			string[] encodings = m_encConverters.Encodings;
			Assert.LessOrEqual(2, encodings.Length, "Should have at least 2 encodings now.");
			string[] mappings = m_encConverters.Mappings;
			Assert.LessOrEqual(1, mappings.Length, "Should have at least 1 mapping now.");
			int countKeys = m_encConverters.Keys.Count;
			int countValues = m_encConverters.Values.Count;
			Assert.AreEqual(countKeys, countValues, "Should have same number of keys and values!");
			Assert.LessOrEqual(1, countKeys, "Should have at least one key now.");
			IEncConverter ec = m_encConverters["UnitTesting-ISO-8859-1"];
			Assert.IsNotNull(ec);
			// Note that converting from ISO-8859.1 should result in the same "string".
			const string first = "ABC\u00A1\u00B0\u00C0\u00D0";
			string output = ec.Convert(first);
			Assert.AreEqual(first, output, "Instantiated ICU.conv converter should work properly!");

			m_encConverters.Add("UnitTesting-To-ISO-8859-1", "ISO-8859-1", ConvType.Unicode_to_from_Legacy,
				"UNICODE", "LEGACY", ProcessTypeFlags.ICUConverter);
			countNew = m_encConverters.Count;
			Assert.AreEqual(countOrig+2, countNew, "Should have two new converters now.");
			mappings = m_encConverters.Mappings;
			Assert.LessOrEqual(2, mappings.Length);
			countKeys = m_encConverters.Keys.Count;
			countValues = m_encConverters.Values.Count;
			Assert.AreEqual(countKeys, countValues, "Should have same number of keys and values!");
			Assert.LessOrEqual(2, countKeys, "Should have at least one key now.");
			IEncConverter ecRev = m_encConverters["UnitTesting-To-ISO-8859-1"];
			Assert.IsNotNull(ecRev);
			// Note that converting to ISO-8859.1 should result in the same "string".
			const string second = "abc\u00AF\u00BF\u00CF\u00DF\u00EF\u00FF";
			string outputRev = ecRev.Convert(second);
			Assert.AreEqual(second, outputRev,
				"Second instantiated ICU.conv converter should work properly!");

			m_encConverters.Remove("UnitTesting-ISO-8859-1");
			m_encConverters.Remove("UnitTesting-To-ISO-8859-1");
			int countAfter = m_encConverters.Count;
			Assert.AreEqual(countOrig, countAfter, "Should have the original number of converters now.");
		}

		const string m_inputLatin =
			"ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz";

		// "ַבקדֶפגהִזכלמנֳפקרסטֻווכסיזַבקדֶפגהִזכלמנֳפקרסטֻווכסיז"
		const string m_transToHebrew =
			"\u05b7\u05d1\u05e7\u05d3\u05b6\u05e4\u05d2\u05d4" +
			"\u05b4\u05d6\u05db\u05dc\u05de\u05e0\u05b3\u05e4" +
			"\u05e7\u05e8\u05e1\u05d8\u05bb\u05d5\u05d5\u05db" +
			"\u05e1\u05d9\u05d6\u05b7\u05d1\u05e7\u05d3\u05b6" +
			"\u05e4\u05d2\u05d4\u05b4\u05d6\u05db\u05dc\u05de" +
			"\u05e0\u05b3\u05e4\u05e7\u05e8\u05e1\u05d8\u05bb" +
			"\u05d5\u05d5\u05db\u05e1\u05d9\u05d6";

		// "אבגדהוזחטיךכלםמןנסעףפץצקרשתװױײ"
		const string m_inputHebrew =
			"\u05D0\u05D1\u05D2\u05D3\u05D4\u05D5\u05D6\u05D7" +
			"\u05D8\u05D9\u05DA\u05DB\u05DC\u05DD\u05DE\u05DF" +
			"\u05E0\u05E1\u05E2\u05E3\u05E4\u05E5\u05E6\u05E7" +
			"\u05E8\u05E9\u05EA" +
			"\u05F0\u05F1\u05F2";

		// "ʼbgdhwzẖtykklmmnnsʻppẕẕqrşţwwwyyy"
		const string m_transToLatin =
			"\u02bcbgdhwz\u1e96tykklmmnns\u02bbpp\u1e95\u1e95qr\u015f\u0163wwwyyy";

		[Test]
		public void TestIcuTransliterators()
		{
			int countOrig = m_encConverters.Count;
			m_encConverters.Add("UnitTesting-Latin-Hebrew", "Latin-Hebrew", ConvType.Unicode_to_from_Unicode,
				"UNICODE", "UNICODE", ProcessTypeFlags.ICUTransliteration);
			int countNew = m_encConverters.Count;
			Assert.AreEqual(countOrig+1, countNew, "Should have one new converter (ICU transliterator) now");
			string[] encodings = m_encConverters.Encodings;
			Assert.LessOrEqual(1, encodings.Length, "Should have at least 1 encoding now.");
			string[] mappings = m_encConverters.Mappings;
			Assert.LessOrEqual(1, mappings.Length, "Should have at least 1 mapping now.");
			int countKeys = m_encConverters.Keys.Count;
			int countValues = m_encConverters.Values.Count;
			Assert.AreEqual(countKeys, countValues, "Should have same number of keys and values!");
			Assert.LessOrEqual(1, countKeys, "Should have at least one key now.");
			IEncConverter ec = m_encConverters["UnitTesting-Latin-Hebrew"];
			Assert.IsNotNull(ec);
			string output = ec.Convert(m_inputLatin);
			Assert.AreEqual(m_transToHebrew, output, "Instantiated ICU.trans converter should work properly!");

			m_encConverters.Add("UnitTesting-Hebrew-Latin", "Hebrew-Latin", ConvType.Unicode_to_from_Unicode,
				"UNICODE", "UNICODE", ProcessTypeFlags.ICUTransliteration);
			countNew = m_encConverters.Count;
			Assert.AreEqual(countOrig+2, countNew, "Should have two new converters (ICU transliterators) now");
			encodings = m_encConverters.Encodings;
			Assert.LessOrEqual(1, encodings.Length, "Should have at least 1 encoding now.");
			mappings = m_encConverters.Mappings;
			Assert.LessOrEqual(2, mappings.Length, "Should have at least 2 mappings now.");
			countKeys = m_encConverters.Keys.Count;
			countValues = m_encConverters.Values.Count;
			Assert.AreEqual(countKeys, countValues, "Should have same number of keys and values!");
			Assert.LessOrEqual(2, countKeys, "Should have at least two keys now.");
			IEncConverter ecRev = m_encConverters["UnitTesting-Hebrew-Latin"];
			Assert.IsNotNull(ecRev);
			string outputRev = ecRev.Convert(m_inputHebrew);
			Assert.AreEqual(m_transToLatin, outputRev, "Second instantiated ICU.trans converter should work properly!");

			m_encConverters.Remove("UnitTesting-Latin-Hebrew");
			m_encConverters.Remove("UnitTesting-Hebrew-Latin");
			int countAfter = m_encConverters.Count;
			Assert.AreEqual(countOrig, countAfter, "Should have the original number of converters now.");
		}

		[Test]
		public void TestIcuRegexConverters()
		{
			int countOrig = m_encConverters.Count;
			m_encConverters.Add("UnitTesting-Consonants->C", "[bcdfghjklmnpqrstvwxyz]->C /i", ConvType.Unicode_to_from_Unicode,
				"UNICODE", "UNICODE", ProcessTypeFlags.ICURegularExpression);
			int countNew = m_encConverters.Count;
			Assert.AreEqual(countOrig+1, countNew, "Should have one new converter (ICU regex) now");
			string[] encodings = m_encConverters.Encodings;
			Assert.LessOrEqual(1, encodings.Length, "Should have at least 1 encoding now.");
			string[] mappings = m_encConverters.Mappings;
			Assert.LessOrEqual(1, mappings.Length, "Should have at least 1 mapping now.");
			int countKeys = m_encConverters.Keys.Count;
			int countValues = m_encConverters.Values.Count;
			Assert.AreEqual(countKeys, countValues, "Should have same number of keys and values!");
			Assert.LessOrEqual(1, countKeys, "Should have at least one key now.");
			IEncConverter ec = m_encConverters["UnitTesting-Consonants->C"];
			Assert.IsNotNull(ec);
			string output = ec.Convert("This is a test.  This is only a test!");
			Assert.AreEqual("CCiC iC a CeCC.  CCiC iC oCCC a CeCC!", output,
				"Instantiated ICU.reg converter should work properly!");

			m_encConverters.Add("UnitTesting-Vowels->V", "[aeiou]->V /i", ConvType.Unicode_to_from_Unicode,
				"UNICODE", "UNICODE", ProcessTypeFlags.ICURegularExpression);
			countNew = m_encConverters.Count;
			Assert.AreEqual(countOrig+2, countNew, "Should have two new converters (ICU regex) now");
			encodings = m_encConverters.Encodings;
			Assert.LessOrEqual(1, encodings.Length, "Should have at least 1 encoding now.");
			mappings = m_encConverters.Mappings;
			Assert.LessOrEqual(2, mappings.Length, "Should have at least 2 mappings now.");
			countKeys = m_encConverters.Keys.Count;
			countValues = m_encConverters.Values.Count;
			Assert.AreEqual(countKeys, countValues, "Should have same number of keys and values!");
			Assert.LessOrEqual(2, countKeys, "Should have at least two keys now.");
			IEncConverter ecV = m_encConverters["UnitTesting-Vowels->V"];
			Assert.IsNotNull(ecV);
			string outputCV = ecV.Convert(output);
			Assert.AreEqual("CCVC VC V CVCC.  CCVC VC VCCC V CVCC!", outputCV,
				"Second instantiated ICU.reg converter should work properly!");

			m_encConverters.Remove("UnitTesting-Consonants->C");
			m_encConverters.Remove("UnitTesting-Vowels->V");
			int countAfter = m_encConverters.Count;
			Assert.AreEqual(countOrig, countAfter, "Should have the original number of converters now.");
		}

		byte[] m_bytesSenufo = new byte[] {
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
			};

			// Missing byte values in the mapping
			// ----------------------------------
			// \u0040 =  64 = '@'
			// \u0080 = 128
			// \u00C7 = 199
			// \u00FA = 250

		const string m_outputSenufo = " !\"#$%&'()*+,-./0123456789:;<=>?" +
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

		// Both 82 and E9 transform to 00E9, and the reverse will always be E9.
		// Both 93 and F4 transform to 00F4, and the reverse will always be F4.
		// Both 9B and 9D transform to 0254, and the reverse will always be 9B.
		// These may be bugs in the map, or maybe the original Senufo encoding
		// had some redundancies for some reason.

		byte[] m_bytesSenufoReversed = new byte[] {
			0x20,0x21,0x22,0x23,0x24,0x25,0x26,0x27,0x28,0x29,0x2A,0x2B,0x2C,0x2D,0x2E,0x2F,
			0x30,0x31,0x32,0x33,0x34,0x35,0x36,0x37,0x38,0x39,0x3A,0x3B,0x3C,0x3D,0x3E,0x3F,
			     0x41,0x42,0x43,0x44,0x45,0x46,0x47,0x48,0x49,0x4A,0x4B,0x4C,0x4D,0x4E,0x4F,
			0x50,0x51,0x52,0x53,0x54,0x55,0x56,0x57,0x58,0x59,0x5A,0x5B,0x5C,0x5D,0x5E,0x5F,
			0x60,0x61,0x62,0x63,0x64,0x65,0x66,0x67,0x68,0x69,0x6A,0x6B,0x6C,0x6D,0x6E,0x6F,
			0x70,0x71,0x72,0x73,0x74,0x75,0x76,0x77,0x78,0x79,0x7A,0x7B,0x7C,0x7D,0x7E,0x7F,
			     0x81,0xE9,0x83,0x84,0x85,0x86,0x87,0x88,0x89,0x8A,0x8B,0x8C,0x8D,0x8E,0x8F,
			0x90,0x91,0x92,0xF4,0x94,0x95,0x96,0x97,0x98,0x99,0x9A,0x9B,0x9C,0x9B,0x9E,0x9F,
			0xA0,0xA1,0xA2,0xA3,0xA4,0xA5,0xA6,0xA7,0xA8,0xA9,0xAA,0xAB,0xAC,0xAD,0xAE,0xAF,
			0xB0,0xB1,0xB2,0xB3,0xB4,0xB5,0xB6,0xB7,0xB8,0xB9,0xBA,0xBB,0xBC,0xBD,0xBE,0xBF,
			0xC0,0xC1,0xC2,0xC3,0xC4,0xC5,0xC6,     0xC8,0xC9,0xCA,0xCB,0xCC,0xCD,0xCE,0xCF,
			0xD0,0xD1,0xD2,0xD3,0xD4,0xD5,0xD6,0xD7,0xD8,0xD9,0xDA,0xDB,0xDC,0xDD,0xDE,0xDF,
			0xE0,0xE1,0xE2,0xE3,0xE4,0xE5,0xE6,0xE7,0xE8,0xE9,0xEA,0xEB,0xEC,0xED,0xEE,0xEF,
			0xF0,0xF1,0xF2,0xF3,0xF4,0xF5,0xF6,0xF7,0xF8,0xF9,     0xFB,0xFC,0xFD,0xFE,0xFF
			};

		[Test]
		public void TestTecKitConverters()
		{
			int countOrig = m_encConverters.Count;
			var dir = GetTestSourceFolder();
			string filename1 = Path.Combine(dir, "Senufo.tec");
			m_encConverters.Add("UnitTesting-Senufo-To-Unicode", filename1, ConvType.Legacy_to_from_Unicode,
				"LEGACY", "UNICODE", ProcessTypeFlags.UnicodeEncodingConversion);
			int countNew = m_encConverters.Count;
			Assert.AreEqual(countOrig+1, countNew, "Should have one new converter (TECkit) now");
			string[] encodings = m_encConverters.Encodings;
			Assert.LessOrEqual(2, encodings.Length, "Should have at least 2 encodings now.");
			string[] mappings = m_encConverters.Mappings;
			Assert.LessOrEqual(1, mappings.Length, "Should have at least 1 mapping now.");
			int countKeys = m_encConverters.Keys.Count;
			int countValues = m_encConverters.Values.Count;
			Assert.AreEqual(countKeys, countValues, "Should have same number of keys and values!");
			Assert.LessOrEqual(1, countKeys, "Should have at least one key now.");
			IEncConverter ec = m_encConverters["UnitTesting-Senufo-To-Unicode"];
			Assert.IsNotNull(ec, "Added converter UnitTesting-Senufo-To-Unicode should exist!");
			string input = TestUtil.GetPseudoStringFromBytes(m_bytesSenufo);
			string output = ec.Convert(input);
			Assert.AreEqual(m_outputSenufo, output, "Senufo.tec should convert data properly!");
			string outMissing = ec.Convert("@\u0080\u00C7\u00FA");
			Assert.AreEqual("\uFFFD\uFFFD\uFFFD\uFFFD", outMissing);

			m_encConverters.Add("UnitTesting-Unicode-To-Senufo", filename1, ConvType.Unicode_to_from_Legacy,
				"UNICODE", "LEGACY", ProcessTypeFlags.UnicodeEncodingConversion);
			countNew = m_encConverters.Count;
			Assert.AreEqual(countOrig+2, countNew, "Should have two new converters now.");
			mappings = m_encConverters.Mappings;
			Assert.LessOrEqual(2, mappings.Length, "Should have at least two mappings now.");
			countKeys = m_encConverters.Keys.Count;
			countValues = m_encConverters.Values.Count;
			Assert.AreEqual(countKeys, countValues, "Should have same number of keys and values!");
			Assert.LessOrEqual(2, countKeys, "Should have at least two keys now.");
			IEncConverter ecRev = m_encConverters["UnitTesting-Unicode-To-Senufo"];
			ecRev.DirectionForward = false;		// shouldn't it be able to set this automatically?
			Assert.IsNotNull(ecRev);
			string outputRaw = ecRev.Convert(m_outputSenufo);
			byte[] output2 = TestUtil.GetBytesFromPseudoString(outputRaw);
			Assert.AreEqual(m_bytesSenufoReversed, output2, "Reversed Senufo.tec should convert data properly!");

			m_encConverters.Remove("UnitTesting-Senufo-To-Unicode");
			m_encConverters.Remove("UnitTesting-Unicode-To-Senufo");
			int countAfter = m_encConverters.Count;
			Assert.AreEqual(countOrig, countAfter, "Should have the original number of converters now.");
		}

		private static string GetTestSourceFolder()
		{
			var codeBase = Assembly.GetExecutingAssembly().CodeBase;
			string filepath;
			var uri = new Uri(Assembly.GetExecutingAssembly().CodeBase);
			filepath = uri.GetComponents(UriComponents.Path, UriFormat.Unescaped);
			if (Util.IsUnix && codeBase.StartsWith("file:///") && !filepath.StartsWith("/"))
				filepath = "/" + filepath;
			var configDir = Path.GetDirectoryName(filepath);
			var outDir = Path.GetDirectoryName(configDir);
			var dir = Path.GetDirectoryName(outDir);
			if (dir != null)
				dir = Path.Combine(Path.Combine(dir, "src"), "TestEncCnvtrs");
			return dir;
		}

		[Test]
		public void TestTecKitMapConverters()
		{
			int countOrig = m_encConverters.Count;
			var dir = GetTestSourceFolder();
			string filename1 = Path.Combine(dir, "Senufo.map");
			m_encConverters.Add("UnitTesting-Senufo-To-Unicode", filename1, ConvType.Legacy_to_from_Unicode,
				"LEGACY", "UNICODE", ProcessTypeFlags.UnicodeEncodingConversion);
			int countNew = m_encConverters.Count;
			Assert.AreEqual(countOrig+1, countNew, "Should have one new converter (TECkit) now");
			string[] encodings = m_encConverters.Encodings;
			Assert.LessOrEqual(2, encodings.Length, "Should have at least 2 encodings now.");
			string[] mappings = m_encConverters.Mappings;
			Assert.LessOrEqual(1, mappings.Length, "Should have at least 1 mapping now.");
			int countKeys = m_encConverters.Keys.Count;
			int countValues = m_encConverters.Values.Count;
			Assert.AreEqual(countKeys, countValues, "Should have same number of keys and values!");
			Assert.LessOrEqual(1, countKeys, "Should have at least one key now.");
			IEncConverter ec = m_encConverters["UnitTesting-Senufo-To-Unicode"];
			Assert.IsNotNull(ec, "Added converter UnitTesting-Senufo-To-Unicode should exist!");
			string input = TestUtil.GetPseudoStringFromBytes(m_bytesSenufo);
			string output = ec.Convert(input);
			Assert.AreEqual(m_outputSenufo, output, "Senufo.tec should convert data properly!");
			string outMissing = ec.Convert("@\u0080\u00C7\u00FA");
			Assert.AreEqual("\uFFFD\uFFFD\uFFFD\uFFFD", outMissing);

			m_encConverters.Add("UnitTesting-Unicode-To-Senufo", filename1, ConvType.Unicode_to_from_Legacy,
				"UNICODE", "LEGACY", ProcessTypeFlags.UnicodeEncodingConversion);
			countNew = m_encConverters.Count;
			Assert.AreEqual(countOrig+2, countNew, "Should have two new converters now.");
			mappings = m_encConverters.Mappings;
			Assert.LessOrEqual(2, mappings.Length, "Should have at least two mappings now.");
			countKeys = m_encConverters.Keys.Count;
			countValues = m_encConverters.Values.Count;
			Assert.AreEqual(countKeys, countValues, "Should have same number of keys and values!");
			Assert.LessOrEqual(2, countKeys, "Should have at least two keys now.");
			IEncConverter ecRev = m_encConverters["UnitTesting-Unicode-To-Senufo"];
			ecRev.DirectionForward = false;		// shouldn't it be able to set this automatically?
			Assert.IsNotNull(ecRev);
			string outputRaw = ecRev.Convert(m_outputSenufo);
			byte[] output2 = TestUtil.GetBytesFromPseudoString(outputRaw);
			Assert.AreEqual(m_bytesSenufoReversed, output2, "Reversed Senufo.tec should convert data properly!");

			m_encConverters.Remove("UnitTesting-Senufo-To-Unicode");
			m_encConverters.Remove("UnitTesting-Unicode-To-Senufo");
			int countAfter = m_encConverters.Count;
			Assert.AreEqual(countOrig, countAfter, "Should have the original number of converters now.");
		}

		private readonly byte[] m_bytesAnn = new byte[] {0xE9, 0x4C, 0x83, 0xE7, 0xA2 };
		private const string m_utf16Ann = "\u0915\u093F\u0924\u093E\u092C";

		[Test]
		public void TestCcEncConverters()
		{
			int countOrig = m_encConverters.Count;
			var dir = GetTestSourceFolder();
			string filename1 = Path.Combine(dir, "ann2unicode.cct");
			m_encConverters.Add("UnitTesting-Ann-To-Unicode", filename1,
				ConvType.Legacy_to_from_Unicode,
				"LEGACY", "UNICODE", ProcessTypeFlags.UnicodeEncodingConversion);
			int countNew = m_encConverters.Count;
			Assert.AreEqual(countOrig + 1, countNew, "Should have one new converter (CC) now");
			string[] encodings = m_encConverters.Encodings;
			Assert.LessOrEqual(2, encodings.Length, "Should have at least 2 encodings now.");
			string[] mappings = m_encConverters.Mappings;
			Assert.LessOrEqual(1, mappings.Length, "Should have at least 1 mapping now.");
			int countKeys = m_encConverters.Keys.Count;
			int countValues = m_encConverters.Values.Count;
			Assert.AreEqual(countKeys, countValues, "Should have same number of keys and values!");
			Assert.LessOrEqual(1, countKeys, "Should have at least one key now.");
			IEncConverter ec = m_encConverters["UnitTesting-Ann-To-Unicode"];
			Assert.IsNotNull(ec, "Added converter UnitTesting-Ann-To-Unicode should exist!");
			string input = TestUtil.GetPseudoStringFromBytes(m_bytesAnn);
			string output = ec.Convert(input);
			Assert.AreEqual(m_utf16Ann, output, "ann2unicode.cct should convert data properly!");

			string filename2 = Path.Combine(dir, "unicode2ann.cct");
			m_encConverters.Add("UnitTesting-Unicode-To-Ann", filename2,
				ConvType.Unicode_to_from_Legacy,
				"UNICODE", "LEGACY", ProcessTypeFlags.UnicodeEncodingConversion);
			countNew = m_encConverters.Count;
			Assert.AreEqual(countOrig + 2, countNew, "Should have two new converters now.");
			mappings = m_encConverters.Mappings;
			Assert.LessOrEqual(2, mappings.Length, "Should have at least two mappings now.");
			countKeys = m_encConverters.Keys.Count;
			countValues = m_encConverters.Values.Count;
			Assert.AreEqual(countKeys, countValues, "Should have same number of keys and values!");
			Assert.LessOrEqual(2, countKeys, "Should have at least two keys now.");
			IEncConverter ecRev = m_encConverters["UnitTesting-Unicode-To-Ann"];
			Assert.IsNotNull(ecRev);
			string outputRaw = ecRev.Convert(m_utf16Ann);
			byte[] output2 = TestUtil.GetBytesFromPseudoString(outputRaw);
			Assert.AreEqual(m_bytesAnn, output2, "unicode2ann.cct should convert data properly!");

			m_encConverters.Remove("UnitTesting-Ann-To-Unicode");
			m_encConverters.Remove("UnitTesting-Unicode-To-Ann");
			int countAfter = m_encConverters.Count;
			Assert.AreEqual(countOrig, countAfter, "Should have the original number of converters now.");
		}

		[Test]
		public void TestPerlEncConverter()
		{
			int countOrig = m_encConverters.Count;
			m_encConverters.Add("UnitTesting-ReverseString", "$strOut = reverse($strIn);",
				ConvType.Unicode_to_from_Unicode,
				"UNICODE", "UNICODE", ProcessTypeFlags.PerlExpression);
			int countNew = m_encConverters.Count;
			Assert.AreEqual(countOrig + 1, countNew, "Should have one new converter (ICU regex) now");
			string[] encodings = m_encConverters.Encodings;
			Assert.LessOrEqual(1, encodings.Length, "Should have at least 1 encoding now.");
			string[] mappings = m_encConverters.Mappings;
			Assert.LessOrEqual(1, mappings.Length, "Should have at least 1 mapping now.");
			int countKeys = m_encConverters.Keys.Count;
			int countValues = m_encConverters.Values.Count;
			Assert.AreEqual(countKeys, countValues, "Should have same number of keys and values!");
			Assert.LessOrEqual(1, countKeys, "Should have at least one key now.");
			IEncConverter ec = m_encConverters["UnitTesting-ReverseString"];
			Assert.IsNotNull(ec);
			string output = ec.Convert("This is a test.  This is only a test!");
			Assert.AreEqual("!tset a ylno si sihT  .tset a si sihT", output,
				"Instantiated SIL.perl converter should work properly!");

			m_encConverters.Remove("UnitTesting-ReverseString");
			int countAfter = m_encConverters.Count;
			Assert.AreEqual(countOrig, countAfter, "Should have the original number of converters now.");
		}

		// 0x81, 0x8D, 0x8F, 0x90, and 0x9D are undefined according to the Python cp1252 codec.
		// And Python codecs appear to bail on undefined characters...
		byte[] m_1252bytes = new byte[] {
			 32,  33,  34,  35,  36,  37,  38,  39,  40,  41,  42,  43,  44,  45,  46,  47,
			 48,  49,  50,  51,  52,  53,  54,  55,  56,  57,  58,  59,  60,  61,  62,  63,
			 64,  65,  66,  67,  68,  69,  70,  71,  72,  73,  74,  75,  76,  77,  78,  79,
			 80,  81,  82,  83,  84,  85,  86,  87,  88,  89,  90,  91,  92,  93,  94,  95,
			 96,  97,  98,  99, 100, 101, 102, 103, 104, 105, 106, 107, 108, 109, 110, 111,
			112, 113, 114, 115, 116, 117, 118, 119, 120, 121, 122, 123, 124, 125, 126, 127,
			128,      130, 131, 132, 133, 134, 135, 136, 137, 138, 139, 140,      142,
			     145, 146, 147, 148, 149, 150, 151, 152, 153, 154, 155, 156,      158, 159,
			160, 161, 162, 163, 164, 165, 166, 167, 168, 169, 170, 171, 172, 173, 174, 175,
			176, 177, 178, 179, 180, 181, 182, 183, 184, 185, 186, 187, 188, 189, 190, 191,
			192, 193, 194, 195, 196, 197, 198, 199, 200, 201, 202, 203, 204, 205, 206, 207,
			208, 209, 210, 211, 212, 213, 214, 215, 216, 217, 218, 219, 220, 221, 222, 223,
			224, 225, 226, 227, 228, 229, 230, 231, 232, 233, 234, 235, 236, 237, 238, 239,
			240, 241, 242, 243, 244, 245, 246, 247, 248, 249, 250, 251, 252, 253, 254, 255
			};
		string m_1252Converted = " !\"#$%&'()*+,-./0123456789:;<=>?" +
				"@ABCDEFGHIJKLMNOPQRSTUVWXYZ[\\]^_`abcdefghijklmnopqrstuvwxyz{|}~\u007F" +
				"\u20ac\u201a\u0192\u201e\u2026\u2020\u2021\u02c6\u2030\u0160\u2039\u0152\u017d" +
				"\u2018\u2019\u201c\u201d\u2022\u2013\u2014\u02dc\u2122\u0161\u203a\u0153\u017e\u0178" +
				"\u00A0\u00A1\u00A2\u00A3\u00A4\u00A5\u00A6\u00A7\u00A8\u00A9\u00AA\u00AB\u00AC\u00AD\u00AE\u00AF" +
				"\u00B0\u00B1\u00B2\u00B3\u00B4\u00B5\u00B6\u00B7\u00B8\u00B9\u00BA\u00BB\u00BC\u00BD\u00BE\u00BF" +
				"\u00C0\u00C1\u00C2\u00C3\u00C4\u00C5\u00C6\u00C7\u00C8\u00C9\u00CA\u00CB\u00CC\u00CD\u00CE\u00CF" +
				"\u00D0\u00D1\u00D2\u00D3\u00D4\u00D5\u00D6\u00D7\u00D8\u00D9\u00DA\u00DB\u00DC\u00DD\u00DE\u00DF" +
				"\u00E0\u00E1\u00E2\u00E3\u00E4\u00E5\u00E6\u00E7\u00E8\u00E9\u00EA\u00EB\u00EC\u00ED\u00EE\u00EF" +
				"\u00F0\u00F1\u00F2\u00F3\u00F4\u00F5\u00F6\u00F7\u00F8\u00F9\u00FA\u00FB\u00FC\u00FD\u00FE\u00FF";

		[Test]
		public void TestPyScriptEncConverters()
		{
			int countOrig = m_encConverters.Count;
			var dir = GetTestSourceFolder();
			string filename1 = Path.Combine(dir, "From1252.py");
			m_encConverters.Add("UnitTesting-Python-1252-To-Unicode", filename1,
				ConvType.Legacy_to_from_Unicode,
				"LEGACY", "UNICODE", ProcessTypeFlags.PythonScript);
			int countNew = m_encConverters.Count;
			Assert.AreEqual(countOrig + 1, countNew, "Should have one new converter (CC) now");
			string[] encodings = m_encConverters.Encodings;
			Assert.LessOrEqual(2, encodings.Length, "Should have at least 2 encodings now.");
			string[] mappings = m_encConverters.Mappings;
			Assert.LessOrEqual(1, mappings.Length, "Should have at least 1 mapping now.");
			int countKeys = m_encConverters.Keys.Count;
			int countValues = m_encConverters.Values.Count;
			Assert.AreEqual(countKeys, countValues, "Should have same number of keys and values!");
			Assert.LessOrEqual(1, countKeys, "Should have at least one key now.");
			IEncConverter ec = m_encConverters["UnitTesting-Python-1252-To-Unicode"];
			Assert.IsNotNull(ec, "Added converter UnitTesting-1252-To-Unicode should exist!");
			string input = TestUtil.GetPseudoStringFromBytes(m_1252bytes);
			string output = ec.Convert(input);
			Assert.AreEqual(m_1252Converted, output, "From1252.py should convert data properly!");

			string filename2 = Path.Combine(dir, "To1252.py");
			m_encConverters.Add("UnitTesting-Python-Unicode-To-1252", filename2,
				ConvType.Unicode_to_from_Legacy,
				"UNICODE", "LEGACY", ProcessTypeFlags.PythonScript);
			countNew = m_encConverters.Count;
			Assert.AreEqual(countOrig + 2, countNew, "Should have two new converters now.");
			mappings = m_encConverters.Mappings;
			Assert.LessOrEqual(2, mappings.Length, "Should have at least two mappings now.");
			countKeys = m_encConverters.Keys.Count;
			countValues = m_encConverters.Values.Count;
			Assert.AreEqual(countKeys, countValues, "Should have same number of keys and values!");
			Assert.LessOrEqual(2, countKeys, "Should have at least two keys now.");
			IEncConverter ecRev = m_encConverters["UnitTesting-Python-Unicode-To-1252"];
			Assert.IsNotNull(ecRev, "Added converter UnitTesting-Python-Unicode-To-1252 should exist!");
 			string outputRaw = ecRev.Convert(m_1252Converted);
			byte[] output2 = TestUtil.GetBytesFromPseudoString(outputRaw);
			Assert.AreEqual(m_1252bytes, output2, "To1252.py should convert data properly!");

			m_encConverters.Remove("UnitTesting-Python-1252-To-Unicode");
			m_encConverters.Remove("UnitTesting-Python-Unicode-To-1252");
			int countAfter = m_encConverters.Count;
			Assert.AreEqual(countOrig, countAfter, "Should have the original number of converters now.");
		}

		[Test]
		public void TestCodePageConverters()
		{
			int countOrig = m_encConverters.Count;
			m_encConverters.Add("UnitTesting-From-CP_1252", "1252",
				ConvType.Legacy_to_from_Unicode,
				"LEGACY", "UNICODE", ProcessTypeFlags.CodePageConversion);
			int countNew = m_encConverters.Count;
			Assert.AreEqual(countOrig + 1, countNew, "Should have one new converter (CC) now");
			string[] encodings = m_encConverters.Encodings;
			Assert.LessOrEqual(2, encodings.Length, "Should have at least 2 encodings now.");
			string[] mappings = m_encConverters.Mappings;
			Assert.LessOrEqual(1, mappings.Length, "Should have at least 1 mapping now.");
			int countKeys = m_encConverters.Keys.Count;
			int countValues = m_encConverters.Values.Count;
			Assert.AreEqual(countKeys, countValues, "Should have same number of keys and values!");
			Assert.LessOrEqual(1, countKeys, "Should have at least one key now.");
			IEncConverter ec = m_encConverters["UnitTesting-From-CP_1252"];
			Assert.IsNotNull(ec, "Added converter UnitTesting-From-CP_1252 should exist!");
			string input = TestUtil.GetPseudoStringFromBytes(m_1252bytes);
			string output = ec.Convert(input);
			Assert.AreEqual(m_1252Converted, output, "CP_1252 should convert data to Unicode properly!");

			m_encConverters.Add("UnitTesting-To-CP_1252", "1252",
				ConvType.Unicode_to_from_Legacy,
				"UNICODE", "LEGACY", ProcessTypeFlags.CodePageConversion);
			countNew = m_encConverters.Count;
			Assert.AreEqual(countOrig + 2, countNew, "Should have two new converters now.");
			mappings = m_encConverters.Mappings;
			Assert.LessOrEqual(2, mappings.Length, "Should have at least two mappings now.");
			countKeys = m_encConverters.Keys.Count;
			countValues = m_encConverters.Values.Count;
			Assert.AreEqual(countKeys, countValues, "Should have same number of keys and values!");
			Assert.LessOrEqual(2, countKeys, "Should have at least two keys now.");
			IEncConverter ecRev = m_encConverters["UnitTesting-To-CP_1252"];
			Assert.IsNotNull(ecRev, "Added converter UnitTesting-To-CP_1252 should exist!");
 			string outputRaw = ecRev.Convert(m_1252Converted);
			byte[] output2 = TestUtil.GetBytesFromPseudoString(outputRaw);
			Assert.AreEqual(m_1252bytes, output2, "CP_1252 should convert data from Unicode properly!");

			m_encConverters.Remove("UnitTesting-From-CP_1252");
			m_encConverters.Remove("UnitTesting-To-CP_1252");
			int countAfter = m_encConverters.Count;
			Assert.AreEqual(countOrig, countAfter, "Should have the original number of converters now.");
		}
		
		[Test]
		public void TestListingCodePageConverters()
		{
			var converters = CpEncConverter.GetAvailableConverterSpecs();
			Assert.Less(10, converters.Count, "There should be at least ten CodePage converters available!");
		}
	}

	/// <summary>
	/// Utility methods useful for testing encoding converters.
	/// </summary>
	public static class TestUtil
	{
		/// <summary>
		/// Convert a byte array to a string by zero-padding all the bytes in the array.
		/// </summary>
		public static string GetPseudoStringFromBytes(byte[] bytes)
		{
			char[] rgch = new char[bytes.Length];
			for (int i = 0; i < bytes.Length; ++i)
				rgch[i] = (char)bytes[i];
			return new string(rgch);
		}

		/// <summary>
		/// Convert the string to a byte array by stripping the top byte from each character in
		/// the string.
		/// </summary>
		public static byte[] GetBytesFromPseudoString(string str)
		{
			byte[] bytes = new byte[str.Length];
			var mask = (str[0] & 0xFF00);
			for (int i = 0; i < str.Length; ++i)
			{
				bytes[i] = (byte)(str[i] & 0xFF);
				var newmask = (str[i] & 0xFF00);
				Assert.AreEqual(mask, newmask,
					String.Format("char[{0}] has a different upper byte ({1:x4}) than the first char in the string ({2:x4})", i, newmask, mask));
			}
			return bytes;
		}

		public static void WriteBytesForDebugging(byte[] bytes, string header)
		{
			if (!String.IsNullOrEmpty(header))
				Console.WriteLine(header);
			if (bytes != null)
			{
				Console.WriteLine("bytes.Length = {0}", bytes.Length);
				for (int i = 0; i < bytes.Length; ++i)
				{
					if (i > 0 && (i % 16) == 0)
						Console.WriteLine();
					Console.Write(" {0:x2}", bytes[i]);
				}
			}
			else
			{
				Console.WriteLine("bytes = null");
			}
			Console.WriteLine();
		}

		public static void WriteCharsForDebugging(string raw, string header)
		{
			if (!String.IsNullOrEmpty(header))
				Console.WriteLine(header);
			if (raw != null)
			{
				Console.WriteLine("raw.Length = {0}", raw.Length);
				for (int i = 0; i < raw.Length; ++i)
				{
					if (i > 0 && (i % 16) == 0)
						Console.WriteLine();
					Console.Write(" {0:x4}", (int)raw[i]);
				}
			}
			else
			{
				Console.WriteLine("raw = null");
			}
			Console.WriteLine();
		}
		
        public static bool IsUnix
        {
            get
			{
				return Environment.OSVersion.Platform == PlatformID.Unix ||
						Environment.OSVersion.Platform == PlatformID.MacOSX;	// MacOSX is built on top of BSD.
			}
        }

		public static bool IsWindows
        {
            get
			{
				return Environment.OSVersion.Platform == PlatformID.Win32S ||
					Environment.OSVersion.Platform == PlatformID.Win32Windows ||
					Environment.OSVersion.Platform == PlatformID.Win32NT ||
					Environment.OSVersion.Platform == PlatformID.WinCE;
			}
        }
	}
}
