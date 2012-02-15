// created by Steve McConnel, Feb 7, 2012.

using System;
using System.Collections;
using System.IO;
using Microsoft.Win32;

using NUnit.Framework;

using ECInterfaces;
using SilEncConverters40;

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
			if (m_encConverters["UnitTesting-ISO-8859-1"] != null)
				m_encConverters.Remove("UnitTesting-ISO-8859-1");
			if (m_encConverters["UnitTesting-To-ISO-8859-1"] != null)
				m_encConverters.Remove("UnitTesting-To-ISO-8859-1");
			if (m_encConverters["UnitTesting-Latin-Hebrew"] != null)
				m_encConverters.Remove ("UnitTesting-Latin-Hebrew");
			if (m_encConverters["UnitTesting-Hebrew-Latin"] != null)
				m_encConverters.Remove ("UnitTesting-Hebrew-Latin");
			if (m_encConverters["UnitTesting-Consonants->C"] != null)
				m_encConverters.Remove("UnitTesting-Consonants->C");
			if (m_encConverters["UnitTesting-Vowels->V"] != null)
				m_encConverters.Remove("UnitTesting-Vowels->V");
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
		public void TestDiscovery()
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
		public void AddIcuConvEncConverters()
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
			string output = ec.Convert("ABC\u00A1\u00B0\u00C0\u00D0");
			Assert.AreEqual("ABC\u00A1\u00B0\u00C0\u00D0", output, "Instantiated ICU.conv converter should work properly!");

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
			string outputRev = ecRev.Convert("abc\u00AF\u00BF\u00CF\u00DF\u00EF\u00FF");
			Assert.AreEqual("abc\u00AF\u00BF\u00CF\u00DF\u00EF\u00FF", outputRev,
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
		public void AddIcuTransliterators()
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
//			var xxx = outputRev.ToCharArray();
//			if (xxx != null)
//			{
//				Console.WriteLine("Hebrew-Latin output characters");
//				for (int i = 0; i < xxx.Length; ++i)
//				{
//					char ch = xxx[i];
//					Console.WriteLine("Latin[{0:d2}] = '{1}' = {2:d5} = \\u{2:x4}", i, ch, (int)ch);
//				}
//			}
			Assert.AreEqual(m_transToLatin, outputRev, "Second instantiated ICU.trans converter should work properly!");

			m_encConverters.Remove("UnitTesting-Latin-Hebrew");
			m_encConverters.Remove("UnitTesting-Hebrew-Latin");
			int countAfter = m_encConverters.Count;
			Assert.AreEqual(countOrig, countAfter, "Should have the original number of converters now.");
		}
		
		[Test]
		public void AddIcuRegexConverters()
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
	}
}

