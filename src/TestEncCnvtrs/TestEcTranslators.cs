using System;
using System.IO;
using Microsoft.Win32;

using NUnit.Framework;

using ECInterfaces;
using SilEncConverters40;
using System.Reflection;
using System.Threading;
using System.Linq;
using SilEncConverters40.EcTranslators.BingTranslator;
using SilEncConverters40.EcTranslators.DeepLTranslator;
using System.Threading.Tasks;

namespace TestEncCnvtrs
{
	[TestFixture]
	public class TestEcTranslators
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
		[OneTimeSetUp]
		public void InitForClass()
		{
			Util.DebugWriteLine(this, "BEGIN");
			if (Util.IsUnix)
			{
				// Make sure we get set up to be able to access Registry.LocalMachine.
				string machine_store = Environment.GetEnvironmentVariable("MONO_REGISTRY_PATH");
				if (machine_store == null)
				{
					Console.WriteLine("First, make sure that " + Util.CommonAppDataPath() +
									  " exists and is writeable by everyone.");
					Console.WriteLine("eg, sudo mkdir -p " + Util.CommonAppDataPath() +
									  " && sudo chmod +wt " + Util.CommonAppDataPath());
					Console.WriteLine("Then add the following line to ~/.profile, logout, and login again.");
					Console.WriteLine("MONO_REGISTRY_PATH=" + Util.CommonAppDataPath() +
									  "/registry; export MONO_REGISTRY_PATH");
					// doesn't work on Maverick, but try it anyway...
					Environment.SetEnvironmentVariable("MONO_REGISTRY_PATH", Util.CommonAppDataPath() + "/registry");
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
				Util.DebugWriteLine(this, e.Message);
			}
			Util.DebugWriteLine(this, "END");
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
			if (m_encConverters != null)
			{
				RemoveAnyAddedConverters();
			}
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

			if (m_encConverters.ContainsKey("UnitTesting-From-CP_1252"))
				m_encConverters.Remove("UnitTesting-From-CP_1252");
			if (m_encConverters.ContainsKey("UnitTesting-To-CP_1252"))
				m_encConverters.Remove("UnitTesting-To-CP_1252");

			if (m_encConverters.ContainsKey("UnitTesting-ThaiWordBreaker"))
				m_encConverters.Remove("UnitTesting-ThaiWordBreaker");
			if (m_encConverters.ContainsKey("UnitTesting-AiKbConverter"))
				m_encConverters.Remove("UnitTesting-AiKbConverter");

			if (m_encConverters.ContainsKey(TestEncConverters.TechHindiSiteConverterFriendlyName))
				m_encConverters.Remove(TestEncConverters.TechHindiSiteConverterFriendlyName);
		}

		/// --------------------------------------------------------------------
		/// <summary>
		/// Global cleanup, called once after all tests in this class
		/// ("fixture") have been run.
		/// </summary>
		/// --------------------------------------------------------------------
		[OneTimeTearDown]
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
				if (File.Exists(m_repoFile))
				{
					File.Delete(m_repoFile); // to overwrite file
				}
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

		/*
		[Apartment(ApartmentState.STA)]
		[Test]
		[TestCase("tta_input_ta;tta_output_ta;Delay;;GeckoFx")]
		public void TestBingTranslatorAsHtmlSite(string converterSpecSuffix)
		{
			BingTranslatorAsHtmlSite(converterSpecSuffix);
		}

		private const string BingTranslatorSiteConverterFriendlyName = "BingHindi>English";

		public void BingTranslatorAsHtmlSite(string converterSpecSuffix)
        {
			var dir = @"C:\btmp\SILConverter test files";	// GetTestSourceFolder();
            var pathToTechHindiSiteFile = Path.Combine(dir, "BingTranslatorHindiToEnglish.html");
            var converterSpec = $"{pathToTechHindiSiteFile};{converterSpecSuffix}";
			m_encConverters.AddConversionMap(BingTranslatorSiteConverterFriendlyName, converterSpec, ConvType.Unicode_to_Unicode,
											 EncConverters.strTypeSILtechHindiSite, "UNICODE", "UNICODE",
											 ProcessTypeFlags.Translation);

            var theEc = m_encConverters[BingTranslatorSiteConverterFriendlyName];

			// do a forward conversion
            var strOutput = theEc.Convert("पुराने युग के संत-महात्‍माओं का जैसा पहनावा होता था, वैसे युहन्‍ना के कपड़े भी ऊँटों के बालों के बने हुए होते थे।");
            Assert.AreEqual("Just like the saint-mahatmas of the ancient era used to dress, yuhanna's clothes were also made of camel hair.", strOutput);
        }
		*/

		private const string BingTranslatorConverterFriendlyName = "BingTranslator";

		// these tests may fail if the Bing Translator resource no longer has any remaining juice...
		[Test]
		[TestCase(ProcessTypeFlags.Translation, "Translate;hi;en", "", "")]
		[TestCase(ProcessTypeFlags.Translation, "Translate;hi;en", "यीशु ने यह भी कहा,", "Jesus also said,")]
		[TestCase(ProcessTypeFlags.Translation, "Translate;;en", "यीशु ने यह भी कहा,", "Jesus also said,")]
		[TestCase(ProcessTypeFlags.Translation, "Translate;en;zh-Hans", "This Israel Field Guide has been developed to help you get to know the beautiful country of Israel and also to encourage you to learn and experience the incredible Word of God and the truth of the events and doctrines that it presents.", "本以色列实地指南旨在帮助您了解美丽的以色列国，并鼓励您学习和体验上帝令人难以置信的话语以及它所呈现的事件和教义的真理。")]
		[TestCase(ProcessTypeFlags.Translation | ProcessTypeFlags.Transliteration, "TranslateWithTransliterate;en;ar;;Latn", "God", "alleh")]
		[TestCase(ProcessTypeFlags.Translation | ProcessTypeFlags.Transliteration, "TranslateWithTransliterate;;ar;;Latn", "God", "alleh")]
		// [TestCase("TranslateWithTransliterate;;en;;Deva", "नहीं", "not")]	// can't transliterate from an language=en result
		// [TestCase("TranslateWithTransliterate;en;ar;;Arab", "God", "الله")] // you *can* do this, but there is no transliteration part of it (since the result is already in Arab script)
		[TestCase(ProcessTypeFlags.Transliteration, "Transliterate;hi;;Deva;Latn", "संसार", "sansar")]
		[TestCase(ProcessTypeFlags.Transliteration, "Transliterate;hi;;Latn;Deva", "sansar", "संसार")]
		[TestCase(ProcessTypeFlags.Transliteration, "Transliterate;ar;;Arab;Latn", "الله", "alleh")]
		[TestCase(ProcessTypeFlags.Transliteration, "Transliterate;ar;;Latn;Arab", "alleh", "الله")]
		// [TestCase("Transliterate;ar;;;Latn", "الله", "alleh")]		// this doesn't work, because with Transliterate, you must specify the fromScript
		[TestCase(ProcessTypeFlags.Translation, "DictionaryLookup;en;hi", "with", "साथ")]
		[TestCase(ProcessTypeFlags.Translation, "DictionaryLookup;hi;en", "से", "%2%from%than%")]	// when multiple results, return the ample disambiguation syntax (cf. AdaptIt lookup converter)
		[TestCase(ProcessTypeFlags.Translation, "DictionaryLookup;en;hi", "schmaboogle", "")]		// when there are no translations, it returns empty string
		public void TestBingTransliteratorConverter(ProcessTypeFlags processType, string converterSpec, string testInput, string testOutput)
		{
			m_encConverters.AddConversionMap(BingTranslatorConverterFriendlyName, converterSpec, ConvType.Unicode_to_Unicode,
											 EncConverters.strTypeSILBingTranslator, "UNICODE", "UNICODE",
											 processType);

			var theEc = m_encConverters[BingTranslatorConverterFriendlyName];

			// do a forward conversion
			var strOutput = theEc.Convert(testInput);
			Assert.AreEqual(testOutput, strOutput);
		}

		[Test]
		public void TestBingTranslatorGetCapabilities()
        {
			var res = BingTranslatorEncConverter.GetCapabilities();
			Assert.Contains("Arabic العربية (ar)", res.translations.Select(t => t.ToString()).ToList());
			Assert.Contains("Hindi हिन्दी (hi) => (Latin लैटिन (Latn)) OR (Devanagari देवनागरी (Deva))", res.transliterations.Select(t => t.ToString()).ToList());
			Assert.Contains("Bulgarian Български (bg) => English (en)", res.dictionaryOptions.Select(t => t.ToString()).ToList());
		}

		[Test]
		[TestCase("Hindi हिन्दी (hi)", "hi")]
		[TestCase("French (Canada) Français (Canada) (fr-CA)", "fr-CA")]
		[TestCase("Arabic العربية (ar)", "ar")]
		[TestCase("Chinese Simplified 中文 (简体) (zh-Hans)", "zh-Hans")]
		[TestCase("Fijian Na Vosa Vakaviti (fj)", "fj")]
		[TestCase("Hmong Daw (mww)", "mww")]
		[TestCase("Inuktitut (Latin) (iu-Latn)", "iu-Latn")]
		[TestCase("Klingon (pIqaD) (tlh-Piqd)", "tlh-Piqd")]
		[TestCase("Kurdish (Northern) Kurdî (Bakur) (kmr)", "kmr")]
		[TestCase("Mongolian (Traditional) ᠮᠣᠩᠭᠣᠯ ᠬᠡᠯᠡ (mn-Mong)", "mn-Mong")]
		[TestCase("Serbian (Cyrillic) Српски (ћирилица) (sr-Cyrl)", "sr-Cyrl")]
		public void TestExtractingLangCode(string menuItem, string languageCodeExpected)
        {
			var languageCodeActual = BingTranslatorAutoConfigDialog.ExtractCode(menuItem);
			Assert.AreEqual(languageCodeExpected, languageCodeActual);
		}

		private const string DeepLTranslatorConverterFriendlyName = "DeepLTranslator";

		// these tests may fail if the DeepL Translator resource no longer has any remaining juice...
		[Test]
		[TestCase(ProcessTypeFlags.Translation, "Translate;en;fr", "Hello, world!", "Bonjour, le monde !")]
		[TestCase(ProcessTypeFlags.Translation, "Translate;en;zh", "This Israel Field Guide has been developed to help you get to know the beautiful country of Israel.", "这本《以色列实地指南》是为了帮助你了解以色列这个美丽的国家而编写的。")]
		[TestCase(ProcessTypeFlags.Translation, "Translate;en;de;Less", "How are you?", "Wie geht es dir?")]
		[TestCase(ProcessTypeFlags.Translation, "Translate;en;de;More", "How are you?", "Wie geht es Ihnen?")]
		public void TestDeepLConverter(ProcessTypeFlags processType, string converterSpec, string testInput, string testOutput)
		{
			m_encConverters.AddConversionMap(DeepLTranslatorConverterFriendlyName, converterSpec, ConvType.Unicode_to_Unicode,
											 EncConverters.strTypeSILDeepLTranslator, "UNICODE", "UNICODE",
											 processType);

			var theEc = m_encConverters[DeepLTranslatorConverterFriendlyName];

			// do a forward conversion
			var strOutput = theEc.Convert(testInput);
			Assert.AreEqual(testOutput, strOutput);
		}

		[Test]
		public async Task TestDeeplTranslatorGetCapabilities()
		{
            var res = await DeepLTranslatorEncConverter.GetCapabilities();
            Assert.IsNotNull(res);
			Assert.Contains("en", res.languagesSource.Select(l => l.Code).ToList());
			Assert.Contains("fr", res.languagesTarget.Select(l => l.Code).ToList());
			Assert.Contains("fr", res.glossaryLanguagePairs.Select(l => l.SourceLanguageCode).ToList());
			Assert.Contains("de", res.glossaryLanguagePairs.Select(l => l.TargetLanguageCode).ToList());
			Assert.NotNull(res.usageLeft);
		}
	}
}
