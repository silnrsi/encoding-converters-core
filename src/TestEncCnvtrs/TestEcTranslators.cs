using System;
using System.IO;
using Microsoft.Win32;

using NUnit.Framework;

using ECInterfaces;
using SilEncConverters40;
using System.Linq;
using SilEncConverters40.EcTranslators.BingTranslator;
using SilEncConverters40.EcTranslators.DeepLTranslator;
using SilEncConverters40.EcTranslators.GoogleTranslator;
using SilEncConverters40.EcTranslators.AzureOpenAI;
using SilEncConverters40.EcTranslators.NllbTranslator;
using SilEncConverters40.EcTranslators.VertexAi;
using System.Threading.Tasks;
using SilEncConverters40.EcTranslators;

namespace TestEncCnvtrs
{
    [TestFixture]
    public class TestEcTranslators
    {
        private const string AzureOpenAiKey = "16ZNuS...";
        private const string AzureOpenAiDeploymentName = "model-router-2";
        private const string AzureOpenAiEndpoint = "https://ai-beaton9343ai639356232696.cognitiveservices.azure.com/openai/v1/";

        private const string VertexAiCredentials = @"H:\bright-coyote-381812-bc584cec007f.json";

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

        [Test]
        [TestCase(typeof(BingTranslatorEncConverter), "AzureTranslatorSubscriptionKey")]
        [TestCase(typeof(DeepLTranslatorEncConverter), "DeepLTranslatorSubscriptionKey")]
        [TestCase(typeof(GoogleTranslatorEncConverter), "GoogleTranslatorSubscriptionKey")]
        public void TestTranslatorOverrideKeyExists(Type typeEncConverter, string fieldName)
        {
            var theEncConverters = DirectableEncConverter.EncConverters;
            TranslatorConverter theTranslator = (TranslatorConverter)theEncConverters.InstantiateIEncConverter(typeEncConverter.FullName, null);

            // this is False if you don't have the GOOGLE_APPLICATION_CREDENTIALS env var defined or true otherwise
            var isEnvVarDefined = (typeEncConverter == typeof(GoogleTranslatorEncConverter)) &&
                                    !String.IsNullOrEmpty(Environment.GetEnvironmentVariable(GoogleTranslatorEncConverter.EnvVarNameCredentials));
            if (isEnvVarDefined)
                Assert.True(theTranslator.HasUserOverriddenCredentials);
            else
                Assert.IsFalse(theTranslator.HasUserOverriddenCredentials);

            var fieldInfo = typeEncConverter.GetMethod($"set_{fieldName}");
            Assert.NotNull(fieldInfo);

            const string Credentials = "asdg";
            fieldInfo.Invoke(theTranslator, new[] { Credentials });
            Assert.IsTrue(theTranslator.HasUserOverriddenCredentials);
        }

        // NLLB always has an 'override' key, bkz it's run locally
        [Test]
        public void TestNllbTranslatorOverrideKeyExists()
        {
            var theEncConverters = DirectableEncConverter.EncConverters;
            TranslatorConverter theTranslator = (TranslatorConverter)theEncConverters.InstantiateIEncConverter(typeof(NllbTranslatorEncConverter).FullName, null);
            Assert.IsTrue(theTranslator.HasUserOverriddenCredentials);
        }

        private const string VertexAiConverterFriendlyName = "VertexAiTranslator";

        // here is where you can find current model IDs for Vertex AI: https://docs.cloud.google.com/vertex-ai/generative-ai/docs/learn/model-versions#latest-stable
        [Test]
//      [TestCase("Hindi;English;bright-coyote-381812;us-central1;google;chat-bison-32k;Translate from Hindi into English.", "यीशु ने यह भी कहा,", "Jesus also said,")]
//      [TestCase(";;bright-coyote-381812;us-central1;google;chat-bison-32k;UseSystemPrompt: Translate from Hindi into English.", "परमेश्वर भेदभाव नहीं करता।", "God is impartial.")]
//      [TestCase("Hindi;English;bright-coyote-381812;us-central1;google;chat-bison-32k;with a \"free translation\" style aimed at high school students", @"यीशु ने यह भी कहा,
//परमे‍‍श्वर मेरा पिता है।", @"Jesus also said,
//God is my Father.")]
//      // For the NMT model, the model ID is general/nmt
//      [TestCase("Hindi;English;bright-coyote-381812;us-central1;google;chat-bison;Translate from Hindi into English.", "यीशु ने यह भी कहा,", "Jesus also said,")]
        // try gemini pro
        [TestCase("Hindi;English;bright-coyote-381812;us-central1;google;gemini-2.5-pro;Translate from Hindi into English.", "यीशु ने यह भी कहा,", "Jesus also said,")]
        //[TestCase("Hindi;English;bright-coyote-381812;us-central1;google;gemini-2.5-pro;Translate from Hindi into English.", "परंतु वह चोगे को छोड़कर वहाँ से भाग गया। ",
		// 	"But he left his cloak and fled from there.")]
        [TestCase("Hindi;English;bright-coyote-381812;us-central1;google;gemini-2.5-pro;Translate from Hindi into English.", "यीशु ने कहा,", "Jesus said,")]
        // multiple lines
        [TestCase("Hindi;English;bright-coyote-381812;us-central1;google;gemini-2.5-pro;Translate from Hindi into English.", @"यीशु ने कहा,
परमे‍‍श्वर मेरा पिता है।", @"Jesus said,
God is my father.")]
        [TestCase("Hindi;English;bright-coyote-381812;us-central1;google;gemini-2.5-pro;Translate from Hindi into English.", "यीशु ने कहा,", "Jesus said,")]
        // multiple lines
        [TestCase("Hindi;English;bright-coyote-381812;us-central1;google;gemini-2.5-pro;Translate from Hindi into English.", @"यीशु ने कहा,
परमे‍‍श्वर मेरा पिता है।", @"Jesus said,
God is my father.")]
        public void TestVertexAiConverter(string converterSpec, string testInput, string testOutput)
        {
            Environment.SetEnvironmentVariable("GOOGLE_APPLICATION_CREDENTIALS", VertexAiCredentials);    // see C:\Users\pete_\source\repos\encoding-converters-core\src\EcTranslators\VertexAi\VertexAiExe\Program.cs

            m_encConverters.AddConversionMap(VertexAiConverterFriendlyName, converterSpec, ConvType.Unicode_to_Unicode,
                                             VertexAiEncConverter.ImplTypeSilVertexAi, "UNICODE", "UNICODE", ProcessTypeFlags.Translation);

            var theEc = m_encConverters[VertexAiConverterFriendlyName];

            // do a forward conversion
            var strOutput = theEc.Convert(testInput);
            Assert.AreEqual(testOutput, strOutput);
        }

        private const string PromptAiConverterFriendlyName = "PromptAiTranslator";  // either vertex or azure open ai, but system prompt-based transducer

        // this test is likely to fail, bkz we're at the mercy of the translation resource, which is constantly evolving
        //    So don't worry too much if it does... these are here bkz at one point they did work as expected.
        /* these don't reliably work
        [Test]
        [TestCase("Hindi;English;bright-coyote-381812;us-central1;google;chat-bison-32k;with a \"free translation\" style aimed at high school students",
            VertexAiEncConverter.ImplTypeSilVertexAi,
            @"सामर्थ अनुसार दान",                                  // phrase to translate
            @"ability",                                        // should contain... (at the mercy of the resource, though, so it might fail)
            @"Donate according to your capability",            // give this as an example input
            @"परंतु अपनी सामर्थ अनुसार ही दो",                        // ... and output (recommending 'capability' instead
            @"capability")]                                    // now see if it has learned from our example
        [TestCase("Hindi;English;with a \"free translation\" style aimed at high school students",
            AzureOpenAiEncConverter.ImplTypeSilAzureOpenAi,
            @"सामर्थ अनुसार दान",                                 // phrase to translate
            @"ability",                                       // should contain... (at the mercy of the resource, though, so it might fail)
            @"Donate according to your capability",           // give this as an example input
            @"परंतु अपनी सामर्थ अनुसार ही दो",                       // ... and output (recommending 'capability' instead
            @"capability")]                                   // now see if it has learned from our example (doesn't always work)
        [TestCase("Hindi;English;bright-coyote-381812;us-central1;google;chat-bison-32k;with a \"literal translation\" style",
            VertexAiEncConverter.ImplTypeSilVertexAi,
            @"यीशु ने कहा,",                                        // phrase to translate ("Jesus said")
            @"said",                                            // should contain... (at the mercy of the resource, though, so it might fail)
            @"Jesus spoke,",                                    // give this as an example translation for earlier output
            @"राम ने कहा,",                                        // ... and see if new subject can get the same verb (spoke)
            @"spoke")]                                            // now see if it has learned from our example
        [TestCase("Hindi;English;bright-coyote-381812;us-central1;google;gemini-1.5-flash;with a \"literal translation\" style",
            VertexAiEncConverter.ImplTypeSilVertexAi,
            @"यीशु ने कहा,",                                       // phrase to translate ("Jesus said")
            @"said",                                            // should contain... (at the mercy of the resource, though, so it might fail)
            @"Jesus spoke,",                                    // give this as an example translation for earlier output
            @"यीशु ने कहा,",                                        // ... and see if new subject can get the same verb (spoke)
            @"spoke")]                                            // now see if it has learned from our example
        public void TestPromptAiConverter_With_Examples(string converterSpec, string implName, string testInput1, string testOutput1Contains,
                                                        string updatedOutput1, string testInput2, string testOutput2Contains)
        {
            // normally, these values get to the program that calls the AzureOpenAI endpoint via command line parameters that
            //    come from the exe's settings *.config file... but for tests, there isn't one, so, we can also send as env vars:
            // BUT if you don't have a resource (and they're not easily available yet or free), then this test will fail anyway...
            Environment.SetEnvironmentVariable(AzureOpenAiEncConverter.EnvVarNameEndPoint, AzureOpenAiEndpoint);
            Environment.SetEnvironmentVariable(AzureOpenAiEncConverter.EnvVarNameDeploymentName, AzureOpenAiDeploymentName);
            Environment.SetEnvironmentVariable(AzureOpenAiEncConverter.EnvVarNameKey, AzureOpenAiKey);    // change the key to the real value or this test will fail
            Environment.SetEnvironmentVariable("GOOGLE_APPLICATION_CREDENTIALS", VertexAiCredentials);    // see C:\Users\pete_\source\repos\encoding-converters-core\src\EcTranslators\VertexAi\VertexAiExe\Program.cs

            m_encConverters.AddConversionMap(VertexAiConverterFriendlyName, converterSpec, ConvType.Unicode_to_Unicode,
                                             implName, "UNICODE", "UNICODE", ProcessTypeFlags.Translation);

            var theEc = m_encConverters[VertexAiConverterFriendlyName];

            // do a forward conversion
            var strOutput = theEc.Convert(testInput1);
            Assert.IsTrue(strOutput.Contains(testOutput1Contains));     // e.g. सामर्थ normally gets translated as 'ability' 

            // to train it with examples, we have to call the AddExample method (but since we only have the interface pointer, use reflection)
            var type = theEc.GetType();
            var methodInfo = type.GetMethod("AddExample");              // train it on translating it as 'capability' (though, that's not more natural)
            var parameters = new object[] { testInput1, updatedOutput1 };
            methodInfo.Invoke(theEc, parameters);                        // this will release the VertexAiExe, since that's the only way the args (including the example sentences) are sent

            strOutput = theEc.Convert(testInput2);
            Assert.IsTrue(strOutput.Contains(testOutput2Contains));     // see if it learned to use 'capability' rather than 'ability'
        }
        */

        private const string NllbConverterFriendlyName = "NllbTranslator";

        /// <summary>
        /// To run this test, you need to go thru the instructions in have the $(SolutionDir)redist\Help\NLLB_Translate_Plug-in_About_box.htm
        /// file to create a docker container for the NLLB 600m model and run it on your local machine.
        /// Two additional notes:
        /// 1) have no api key in the NLLB Model Config dialog (or set up an env var with it--see TestAzureOpenAiConverter for an eg)
        /// 2) Change the @"C:\NLLB\test" path to wherever you chose as the Docker Project folder.
        /// </summary>
        /// <param name="converterSpec">ConverterIdentifier of the NLLB converter to be used to convert the text</param>
        /// <param name="testInput">The text to be translated</param>
        /// <param name="testOutput">The expected translation</param>
        [Test]
        [TestCase(@"C:\NLLB\test;hin_Deva;eng_Latn",
            @"फिर एक स्‍वर्गदूत ने मुझसे कहा, ""इस बात को लिख ले: वे धन्य हैं, जिनको मेमने के विवाह के भोज का निमन्‍त्रण है।
यह परमेश्वर के सच्‍चे बोल हैं।",
            @"Then the angel said to me, ""Write: Blessed are those who are invited to the wedding banquet of the Lamb"".
These are the true words of God.")]
        [TestCase(@"C:\NLLB\nllb-600m;hin_Deva;eng_Latn;http://localhost:8000;N3RK/o+wYtvEFMVGtYsmROIyLr/+RWh1",
            "वे जानते हैं कि परमेश्वर का अस्तित्व है और यह सब कुछ उनके लिए ही बनाया है। परंतु फिर भी न तो वे परमेश्वर का कोई सम्‍मान, और न ही तो उसका धन्यवाद करते हैं। इसलिए उनकी आँखें में पर्दा पड़ गया है, और परमेश्वर के विषय में उनका जो विचार है, वह गलत हो चुका है।",
            "They know that God exists and that he created everything for them. Yet they neither glorified God nor gave him thanks. Therefore their eyes are blinded, and their thoughts are deceitful toward God.")]
        [TestCase(@"C:\NLLB\nllb-600m;hin_Deva;eng_Latn;http://localhost:8000;N3RK/o+wYtvEFMVGtYsmROIyLr/+RWh1",
            "(केवल ये शहीद और न्याय करने वाले लोग हजार वर्षों वाले उस युग के आरंभ में पुनर्जीवित हो जाएँगे। इस बार जीवित होने को “पहला जीवित होना” कहते हैं। बाकि जो मरे हुए हैं, परमेश्वर उन सबको तब तक पुनर्जीवित नहीं करेगा, जब तक उस हजार वर्षों वाले युग का अंत नहीं होगा।)",
            "(Only these martyrs and judges will be resurrected at the beginning of that millennial age. This time being alive is called being alive first. The rest of the dead God did not bring back to life until the thousand years were over. )")]
        [TestCase(@"C:\NLLB\nllb-600m;hin_Deva;eng_Latn;http://localhost:8000;N3RK/o+wYtvEFMVGtYsmROIyLr/+RWh1",
            "केवल ये शहीद और न्याय करने वाले लोग हजार वर्षों वाले उस युग के आरंभ में पुनर्जीवित हो जाएँगे। इस बार जीवित होने को “पहला जीवित होना” कहते हैं। बाकि जो मरे हुए हैं, परमेश्वर उन सबको तब तक पुनर्जीवित नहीं करेगा, जब तक उस हजार वर्षों वाले युग का अंत नहीं होगा,",
            "Only these martyrs and judges will be resurrected at the beginning of that millennial age. This time being alive is called being alive first. The rest of the dead God did not bring back to life until the thousand years were over.")]
        [TestCase(@"C:\NLLB\nllb-600m;hin_Deva;eng_Latn;http://localhost:8000;N3RK/o+wYtvEFMVGtYsmROIyLr/+RWh1",
            "बाकि जो मरे हुए हैं, परमेश्वर उन सबको तब तक पुनर्जीवित नहीं करेगा, जब तक उस हजार वर्षों वाले युग का अंत नहीं होगा,",
            "The rest of the dead God did not bring back to life until the thousand years were over.")]    // yes, NLLB changes the final ',' to a '.'
        public void TestNllbConverter(string converterSpec, string testInput, string testOutput)
        {
            m_encConverters.AddConversionMap(NllbConverterFriendlyName, converterSpec, ConvType.Unicode_to_Unicode,
                                             EncConverters.strTypeSILNllbTranslator, "UNICODE", "UNICODE", ProcessTypeFlags.Translation);

            var theEc = m_encConverters[NllbConverterFriendlyName];

            // do a forward conversion
            var strOutput = theEc.Convert(testInput);
            Assert.AreEqual(testOutput, strOutput);
        }

        // a Kangri (xnr) sentence that the TranslateGemma xnr->dgo model (greedily decoded) gets stuck on, repeating
        //  "बुरियें आत्‍में दी" until it hits its maximum output length
        private const string XnrSentenceThatLoops = "परमेसर बड्डे बाल़े दे हके जो ग्रैह़ण नी करदे ह़न, अनिकि कुदरती जन्‍मैं दिआ बजुर्गाइआ जो ग्रैह़ण नी करदे, अपर नौंऐं जन्‍मैं दिआ बजुर्गाइआ जो ग्रैह़ण करदे ह़न।";

        [Test]
        [TestCase("One. Two! Three", new[] { "One. ", "Two! ", "Three" })]
        [TestCase("No final punctuation\r\nNext line. Last one.", new[] { "No final punctuation\r\n", "Next line. ", "Last one." })]
        [TestCase("Line one.\nLine two.\n", new[] { "Line one.\n", "Line two.\n" })]
        [TestCase("वे जानते हैं। परंतु फिर भी", new[] { "वे जानते हैं। ", "परंतु फिर भी" })]
        public void TestNllbSplitIntoSentences(string input, string[] expected)
        {
            var theEc = new NllbTranslatorEncConverter();
            var sentences = theEc.SplitIntoSentences(input);
            Assert.AreEqual(expected, sentences.ToArray());
            Assert.AreEqual(input, String.Join(String.Empty, sentences), "the pieces should add up to the whole input");
        }

        [Test]
        [TestCase(XnrSentenceThatLoops,
            "परमेसर बड्डे बाल़े दे हके जो ग्रैह़ण नी करदे ह़न, अनिकि कुदरती जन्‍मैं दिआ बजुर्गाइआ जो ग्रैह़ण नी करदे,",
            "अपर नौंऐं जन्‍मैं दिआ बजुर्गाइआ जो ग्रैह़ण करदे ह़न।")]
        [TestCase("First sentence here. And a second, longer one.\r\n", "First sentence here.", "And a second, longer one.\r\n")]
        [TestCase("one two three four five six seven", "one two three", "four five six seven")]
        [TestCase("one two three four five", null, null)]  // too few words to split without punctuation
        public void TestNllbTrySplitForRetry(string input, string expectedLeft, string expectedRight)
        {
            var theEc = new NllbTranslatorEncConverter();
            var isSplit = theEc.TrySplitForRetry(input, out var left, out var right);
            Assert.AreEqual(expectedLeft != null, isSplit);
            Assert.AreEqual(expectedLeft, left);
            Assert.AreEqual(expectedRight, right);
        }

        [Test]
        [TestCase("परमेसर बड्डे आले दे ह़क्‍क गी ग्रैह़न नेई करदे ह़न, यानिकि कुदरती जन्‍म दी बुरियें आत्‍में दी बुरियें आत्‍में दी बुरियें आत्‍में दी बुरियें आत्‍में दी बुरियें आत्‍में दी बुरियें",
            true, "परमेसर बड्डे आले दे ह़क्‍क गी ग्रैह़न नेई करदे ह़न, यानिकि कुदरती जन्‍म दी बुरियें आत्‍में")]
        [TestCase("the the the the the the the the", true, "the the")]
        [TestCase("पवित्र, पवित्र, पवित्र प्रभु परमेश्वर", false, null)]     // "holy, holy, holy" is legit
        [TestCase("Then the angel said to me, Write: Blessed are those who are invited.", false, null)]
        public void TestNllbHasRepetitionLoop(string translation, bool expected, string expectedPartial)
        {
            Assert.AreEqual(expected, NllbTranslatorEncConverter.HasRepetitionLoop(translation, out var partial));
            if (expected)
                Assert.AreEqual(expectedPartial, partial);
        }

        /// <summary>
        /// Needs the TranslateGemma xnr<->dgo docker container (C:\vscode\FineTuneTranslateGemma\docker) running on port
        /// 8010. With its degenerate-output guard, the server recovers by itself; with an older server that just returns
        /// the looping output (or times out), the converter's own split-and-retry should recover.
        /// </summary>
        [Test]
        [TestCase(@"C:\vscode\FineTuneTranslateGemma;xnr;dgo;http://localhost:8010", XnrSentenceThatLoops)]
        public void TestNllbConverterRecoversFromModelLoop(string converterSpec, string testInput)
        {
            if (!NllbTranslatorEncConverter.IsHttpServerListeningAsync("http://localhost:8010").Result)
                Assert.Ignore("TranslateGemma docker container isn't running on port 8010");

            m_encConverters.AddConversionMap(NllbConverterFriendlyName, converterSpec, ConvType.Unicode_to_Unicode,
                                             EncConverters.strTypeSILNllbTranslator, "UNICODE", "UNICODE", ProcessTypeFlags.Translation);

            var theEc = m_encConverters[NllbConverterFriendlyName];
            var strOutput = theEc.Convert(testInput);
            Console.WriteLine(strOutput);
            Assert.IsFalse(strOutput.Contains("Error occurred"), strOutput);
            Assert.IsFalse(NllbTranslatorEncConverter.HasRepetitionLoop(strOutput, out _), strOutput);
            Assert.IsTrue(strOutput.StartsWith("परमेसर"), strOutput);
        }

        /// <summary>
        /// Simulates a server whose model takes longer than the (user) NllbRequestTimeoutSeconds setting: the converter
        /// should wait for the server to finish with the request it gave up on, and then retry in shorter pieces. Slow
        /// (~2 mins), and needs the TranslateGemma xnr<->dgo container running *without* its degenerate-output guard
        /// (i.e. like a server that doesn't have one), e.g. with these added to its docker run command:
        ///   -e LOOP_REPEATS=100000 -e OUTPUT_LENGTH_RATIO=1000 -e RETRY_REPETITION_PENALTY=1.0 -e MAX_SPLIT_DEPTH=0
        /// </summary>
        [Test]
        [Explicit("slow, and needs a specially configured TranslateGemma container (see comments)")]
        [TestCase(@"C:\vscode\FineTuneTranslateGemma;xnr;dgo;http://localhost:8010", XnrSentenceThatLoops)]
        public void TestNllbConverterRecoversFromTimeout(string converterSpec, string testInput)
        {
            if (!NllbTranslatorEncConverter.IsHttpServerListeningAsync("http://localhost:8010").Result)
                Assert.Ignore("TranslateGemma docker container isn't running on port 8010");

            // the Settings class is internal to EcTranslators
            var settings = (System.Configuration.ApplicationSettingsBase)typeof(NllbTranslatorEncConverter).Assembly
                                .GetType("SilEncConverters40.EcTranslators.Properties.Settings")
                                .GetProperty("Default").GetValue(null);
            var originalTimeout = settings["NllbRequestTimeoutSeconds"];
            settings["NllbRequestTimeoutSeconds"] = 15;
            try
            {
                const string friendlyName = NllbConverterFriendlyName + "WithShortTimeout";
                m_encConverters.AddConversionMap(friendlyName, converterSpec, ConvType.Unicode_to_Unicode,
                                                 EncConverters.strTypeSILNllbTranslator, "UNICODE", "UNICODE", ProcessTypeFlags.Translation);

                var theEc = m_encConverters[friendlyName];
                var strOutput = theEc.Convert(testInput);
                Console.WriteLine(strOutput);
                Assert.IsFalse(strOutput.Contains("Error occurred"), strOutput);
                Assert.IsFalse(NllbTranslatorEncConverter.HasRepetitionLoop(strOutput, out _), strOutput);
            }
            finally
            {
                settings["NllbRequestTimeoutSeconds"] = originalTimeout;
            }
        }

        private const string AzureOpenAIConverterFriendlyName = "ChatGptTranslator";

        [Test]
        [TestCase("Hindi;English;Translate from Hindi into English.", "यीशु ने यह भी कहा,", "Jesus also said,")]
        [TestCase(";;UseSystemPrompt: Translate from Hindi into English.", "परमेश्वर भेदभाव नहीं करता।", "God does not discriminate.")]
        [TestCase("Hindi;English;with a \"free translation\" style aimed at high school students", @"यीशु ने यह भी कहा,
परमे‍‍श्वर मेरा पिता है।", @"Jesus also said,
God is my father.")]
        public void TestAzureOpenAiConverter(string converterSpec, string testInput, string testOutput)
        {
            // normally, these values get to the program that calls the AzureOpenAI endpoint via command line parameters that
            //    come from the exe's settings *.config file... but for tests, there isn't one, so, we can also send as env vars:
            // BUT if you don't have a resource (and they're not easily available yet or free), then this test will fail anyway...
            Environment.SetEnvironmentVariable(AzureOpenAiEncConverter.EnvVarNameEndPoint, AzureOpenAiEndpoint);
            Environment.SetEnvironmentVariable(AzureOpenAiEncConverter.EnvVarNameDeploymentName, AzureOpenAiDeploymentName);
            Environment.SetEnvironmentVariable(AzureOpenAiEncConverter.EnvVarNameKey, AzureOpenAiKey);    // change the key to the real value or this test will fail

            m_encConverters.AddConversionMap(AzureOpenAIConverterFriendlyName, converterSpec, ConvType.Unicode_to_Unicode,
                                             AzureOpenAiEncConverter.ImplTypeSilAzureOpenAi, "UNICODE", "UNICODE", ProcessTypeFlags.Translation);

            var theEc = m_encConverters[AzureOpenAIConverterFriendlyName];

            // do a forward conversion
            var strOutput = theEc.Convert(testInput);
            Assert.AreEqual(testOutput.ToUpper(), strOutput.ToUpper());
        }

        private const string OllamaEndpoint = "http://localhost:11434/v1";      // Ollama's OpenAI-compatible endpoint (default port 11434)
        private const string OllamaModelName = "gemma4";						// change to whatever model you've pulled locally (e.g. `ollama pull llama3.2`)
        private const string OllamaKey = "ollama";                              // Ollama doesn't validate the API key, but the OpenAI client library requires some non-empty value

        /// <summary>
        /// To run this test:
        /// 1) install Ollama (https://ollama.com/download) and make sure `ollama serve` is running locally (it listens on
        ///    port 11434 by default and exposes an OpenAI-compatible API at http://localhost:11434/v1).
        /// 2) pull a model, e.g. `ollama pull llama3.1` (and update OllamaModelName above if you pull a different one).
        /// This exercises the same AzureOpenAiEncConverter/AzureOpenAiExe code path as TestAzureOpenAiConverter above, but
        ///    pointed at a generic OpenAI-compatible endpoint rather than a real Azure OpenAI resource -- unlike Azure,
        ///    Ollama's endpoint is already in the form the OpenAI client library expects, so (unlike when setting this up
        ///    via the "Azure OpenAI?" checkbox on QueryForAzureKeyDeploymentNameAndEndpoint) no "openai/v1/" ending is needed.
        /// </summary>
        [Test]
        [TestCase("Hindi;English;Translate from Hindi into English.", "यीशु ने यह भी कहा,", "Jesus also said")]
        public void TestAzureOpenAiConverterViaOllama(string converterSpec, string testInput, string testOutputContains)
        {
            Environment.SetEnvironmentVariable(AzureOpenAiEncConverter.EnvVarNameEndPoint, OllamaEndpoint);
            Environment.SetEnvironmentVariable(AzureOpenAiEncConverter.EnvVarNameDeploymentName, OllamaModelName);
            Environment.SetEnvironmentVariable(AzureOpenAiEncConverter.EnvVarNameKey, OllamaKey);

            m_encConverters.AddConversionMap(AzureOpenAIConverterFriendlyName, converterSpec, ConvType.Unicode_to_Unicode,
                                             AzureOpenAiEncConverter.ImplTypeSilAzureOpenAi, "UNICODE", "UNICODE", ProcessTypeFlags.Translation);

            var theEc = m_encConverters[AzureOpenAIConverterFriendlyName];

            // do a forward conversion (local models are less predictable than Azure/OpenAI's hosted ones, so just check
            //    that the expected word/phrase shows up in the translation, rather than requiring an exact match)
            var strOutput = theEc.Convert(testInput);
            Assert.IsTrue(strOutput.ToUpper().Contains(testOutputContains.ToUpper()));
        }

		private const string OpenAiEndpoint = "https://api.openai.com/v1";
		private const string OpenAiModelName = "gpt-5.4-mini";
		private const string OpenAiKey = "sk-svcacct-...";

		/// <summary>
		/// To run this test you need an OpenAI API account and an API key (see https://platform.openai.com/account/api-keys).
		/// </summary>
		[Test]
		[TestCase("Hindi;English;Translate from Hindi into English.", "यीशु ने यह भी कहा,", "Jesus also said")]
		public void TestAzureOpenAiConverterViaOpenAi(string converterSpec, string testInput, string testOutputContains)
		{
			Environment.SetEnvironmentVariable(AzureOpenAiEncConverter.EnvVarNameEndPoint, OpenAiEndpoint);
			Environment.SetEnvironmentVariable(AzureOpenAiEncConverter.EnvVarNameDeploymentName, OpenAiModelName);
			Environment.SetEnvironmentVariable(AzureOpenAiEncConverter.EnvVarNameKey, OpenAiKey);
			m_encConverters.AddConversionMap(AzureOpenAIConverterFriendlyName, converterSpec, ConvType.Unicode_to_Unicode,
											 AzureOpenAiEncConverter.ImplTypeSilAzureOpenAi, "UNICODE", "UNICODE", ProcessTypeFlags.Translation);

			var theEc = m_encConverters[AzureOpenAIConverterFriendlyName];

			// do a forward conversion (local models are less predictable than Azure/OpenAI's hosted ones, so just check
			//    that the expected word/phrase shows up in the translation, rather than requiring an exact match)
			var strOutput = theEc.Convert(testInput);
			Assert.IsTrue(strOutput.ToUpper().Contains(testOutputContains.ToUpper()));
		}

		private const string BingTranslatorConverterFriendlyName = "BingTranslator";

        // these tests may fail if the Bing Translator resource no longer has any remaining juice... OR (more likely)
        //    if they change the translation
        [Test]
        [TestCase(ProcessTypeFlags.Translation, "Translate;hi;en", "", "")]
        [TestCase(ProcessTypeFlags.Translation, "Translate;hi;en", "यीशु ने यह भी कहा,", "Jesus also said,")]
        // multi-line translations
        [TestCase(ProcessTypeFlags.Translation, "Translate;hi;en", @"यीशु ने कहा,
परमे‍‍श्वर मेरा पिता है।", @"Jesus said, 
God is my Father.")]
        [TestCase(ProcessTypeFlags.Translation, "Translate;;en", "यीशु ने यह भी कहा,", "Jesus also said,")]
        [TestCase(ProcessTypeFlags.Translation, "Translate;en;zh-Hans", "Get to know the beautiful country of Israel.", "了解美丽的以色列国家。")]
        [TestCase(ProcessTypeFlags.Translation | ProcessTypeFlags.Transliteration, "TranslateWithTransliterate;en;ar;;Latn", "God", "Ya ilahi")]
        [TestCase(ProcessTypeFlags.Translation | ProcessTypeFlags.Transliteration, "TranslateWithTransliterate;;ar;;Latn", "God", "Ya ilahi")]
        // [TestCase("TranslateWithTransliterate;;en;;Deva", "नहीं", "not")]    // can't transliterate from an language=en result
        // [TestCase("TranslateWithTransliterate;en;ar;;Arab", "God", "الله")] // you *can* do this, but there is no transliteration part of it (since the result is already in Arab script)
        [TestCase(ProcessTypeFlags.Transliteration, "Transliterate;hi;;Deva;Latn", "संसार", "sansar")]
        // doesn't like short runs of text [TestCase(ProcessTypeFlags.Transliteration, "Transliterate;hi;;Latn;Deva", "sansar", "संसार")]
        [TestCase(ProcessTypeFlags.Transliteration, "Transliterate;ar;;Arab;Latn", "الله", "Allah")]
        // doesn't like short runs of text [TestCase(ProcessTypeFlags.Transliteration, "Transliterate;ar;;Latn;Arab", "alleh", "الله")]
        // [TestCase("Transliterate;ar;;;Latn", "الله", "alleh")]        // this doesn't work, because with Transliterate, you must specify the fromScript
        [TestCase(ProcessTypeFlags.Translation, "DictionaryLookup;en;hi", "with", "साथ")]
        [TestCase(ProcessTypeFlags.Translation, "DictionaryLookup;hi;en", "से", "%2%from%than%")]    // when multiple results, return the ample disambiguation syntax (cf. AdaptIt lookup converter)
        [TestCase(ProcessTypeFlags.Translation, "DictionaryLookup;en;hi", "schmaboogle", "")]        // when there are no translations, it returns empty string
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

        // these tests may fail if the DeepL Translator resource no longer has any remaining juice... OR (more likely)
        //    if they change the translation
        [Test]
        [TestCase(ProcessTypeFlags.Translation, "Translate;en;fr", "Hello, world!", "Bonjour, tout le monde !")]
        [TestCase(ProcessTypeFlags.Translation, "Translate;en;zh", "How are you?", "你好吗？")]
        [TestCase(ProcessTypeFlags.Translation, "Translate;en;de;Less", "How are you?", "Wie geht’s dir?")]
        [TestCase(ProcessTypeFlags.Translation, "Translate;en;de;More", "How are you?", "Wie geht es Ihnen?")]
        // multi-line translations
        [TestCase(ProcessTypeFlags.Translation, "Translate;en;fr", @"Jesus said,
God is my father.", @"Jésus a dit :
Dieu est mon père.")]
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

        private const string GoogleTranslatorConverterFriendlyName = "GoogleTranslator";

        // these tests may fail if Google Translate resource no longer has any remaining juice... OR (more likely)
        //    if they change the translation
        [Test]
        [TestCase("en;fr", "Hello, world!", "Bonjour le monde!")]
        [TestCase("en;zh", "How are you?", "你好吗？")]
        [TestCase("en;de", "How are you?", "Wie geht es dir?")]
        // multi-line translations
        [TestCase("hi;en", @"यीशु ने कहा,
परमे‍‍श्वर मेरा पिता है।", @"Jesus said,
God is my father.")]
        public void TestGoogleConverterWithEnvVariable(string converterSpec, string testInput, string testOutput)
        {
            Environment.SetEnvironmentVariable("GOOGLE_APPLICATION_CREDENTIALS", VertexAiCredentials);    // see C:\Users\pete_\source\repos\encoding-converters-core\src\EcTranslators\VertexAi\VertexAiExe\Program.cs

            m_encConverters.AddConversionMap(GoogleTranslatorConverterFriendlyName, converterSpec, ConvType.Unicode_to_Unicode,
                                             EncConverters.strTypeSILGoogleTranslator, "UNICODE", "UNICODE",
                                             ProcessTypeFlags.Translation);

            var theEc = m_encConverters[GoogleTranslatorConverterFriendlyName];

            // do a forward conversion
            var strOutput = theEc.Convert(testInput);
            Assert.AreEqual(testOutput, strOutput);
            Assert.True(((TranslatorConverter)theEc).HasUserOverriddenCredentials);
        }

        // these tests may fail if Google Translate resource no longer has any remaining juice... OR (more likely)
        //    if they change the translation
        [Test]
        [TestCase("en;fr", "Hello, world!", "Bonjour le monde!")]
        [TestCase("en;zh", "How are you?", "你好吗？")]
        [TestCase("en;de", "How are you?", "Wie geht es dir?")]
        // multi-line translations
        [TestCase("hi;en", @"यीशु ने कहा,
परमे‍‍श्वर मेरा पिता है।", @"Jesus said,
God is my father.")]
        public void TestGoogleConverter(string converterSpec, string testInput, string testOutput)
        {
            m_encConverters.AddConversionMap(GoogleTranslatorConverterFriendlyName, converterSpec, ConvType.Unicode_to_Unicode,
                                             EncConverters.strTypeSILGoogleTranslator, "UNICODE", "UNICODE",
                                             ProcessTypeFlags.Translation);

            var theEc = m_encConverters[GoogleTranslatorConverterFriendlyName];

            // do a forward conversion
            var strOutput = theEc.Convert(testInput);
            Assert.AreEqual(testOutput, strOutput);

            // this is False if you don't have the GOOGLE_APPLICATION_CREDENTIALS env var defined or true otherwise
            var isEnvVarDefined = !String.IsNullOrEmpty(Environment.GetEnvironmentVariable(GoogleTranslatorEncConverter.EnvVarNameCredentials));
            if (isEnvVarDefined)
                Assert.True(((TranslatorConverter)theEc).HasUserOverriddenCredentials);
            else
                Assert.False(((TranslatorConverter)theEc).HasUserOverriddenCredentials);
        }

        [Test]
        public async Task TestGoogleTranslateGetCapabilities()
        {
            var res = await GoogleTranslatorEncConverter.GetCapabilities();
            Assert.IsNotNull(res);
            var languagesSupported = res.Select(l => l.Code).ToList();
            Assert.Contains("en", languagesSupported);
            Assert.Contains("fr", languagesSupported);
            Assert.Contains("de", languagesSupported);
            Assert.Contains("doi", languagesSupported);
        }

#if Disable
        [Test]
        public async Task TestGoogleCloudBillingAccountQuery()
        {
            var res = await GoogleTranslatorAutoConfigDialog.GetUsage();
            Assert.IsNotNull(res);
        }
#endif
    }
}
