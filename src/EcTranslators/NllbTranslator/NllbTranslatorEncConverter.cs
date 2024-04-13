// #define encryptingNewCredentials

using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Runtime.InteropServices;   // for the class attributes
using System.Text;                      // for ASCIIEncoding
using System.Threading.Tasks;
using ECInterfaces;                     // for ConvType
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using static Nllb.ITranslator;
using System.Windows.Forms;
using SilEncConverters40.EcTranslators.Properties;
using System.Collections;
using System.Text.RegularExpressions;

namespace SilEncConverters40.EcTranslators.NllbTranslator
{
    /// <summary>
    /// Managed Nllb Translate EncConverter.
    /// </summary>
#if X64
    [GuidAttribute("DAFFF949-BA9C-4A28-B7A3-13D205D0B838")]
#else
    [GuidAttribute("0CE67479-9D4D-4DC5-B89F-A5384B72DFBD")]
#endif
    // normally these subclasses are treated as the base class (i.e. the 
    //  client can use them orthogonally as IEncConverter interface pointers
    //  so normally these individual subclasses would be invisible), but if 
    //  we add 'ComVisible = false', then it doesn't get the registry 
    //  'HKEY_CLASSES_ROOT\SilEncConverters40.EcTranslators.NllbTranslatorEncConverter' which is the basis of 
    //  how it is started (see EncConverters.AddEx).
    // [ComVisible(false)] 
    public class NllbTranslatorEncConverter : TranslatorConverter
    {
        #region Const Definitions

        public const string CstrDisplayName = "NLLB Translator";
        internal const string strHtmlFilename = "NLLB_Translate_Plug-in_About_box.htm";

        public const string NllbAuthenticationPrefix = "SIL-NLLB-Auth-Key ";

        public const string EnvVarNameEndPoint = "EncConverters_NllbEndpoint";
        public const string EnvVarNameKey = "EncConverters_NllbApiKey";

        public const string SplitSentencesPrefix = @"\SplitSentences ";   // send for conversion: \SplitSentences ON and \SplitSentences OFF

        #endregion Const Definitions

        #region Member Variable Definitions

        public string FromLanguage;
        public string ToLanguage;
        public string PathToDockerProject;
        public string ApiKey;        // always clear text
        public string Endpoint;

        public Regex SentenceSplitter = new Regex(Properties.Settings.Default.NllbSentenceFinalPunctuationRegex);
        public bool IsSplitSentences = Properties.Settings.Default.NllbProcessSentenceBySentence;

        public int RepeatsOfLastInputString { get; set; } = 0;
        public string LastInputString { get; set; }
        public int MaxTokensPerSentence { get; set; }

        private static bool HasValidEnvironmentVariable(string envVarName, out string parameter)
        {
            return !string.IsNullOrEmpty((parameter = Environment.GetEnvironmentVariable(envVarName)));
        }

        // borrowing from deepL's approach
        internal Translator _nllbTranslator;
        public Translator NllbTranslator
        {
            get
            {
                if (_nllbTranslator == null)
                {
                    var handler = new DeepLTranslator.Http2CustomHandler();
                    var serverUrl = Endpoint ?? NllbTranslatorEndpoint;

                    var options = new DeepL.TranslatorOptions
                    {
                        ServerUrl = serverUrl,
                        MaximumNetworkRetries = 2,
                        PerRetryConnectionTimeout = TimeSpan.FromSeconds(5),
                        OverallConnectionTimeout = TimeSpan.FromSeconds(10),
                        ClientFactory = () => new DeepL.HttpClientAndDisposeFlag
                        {
                            HttpClient = new HttpClient(handler),
                            DisposeClient = true,
                        },
                    };
                    var apiKey = ApiKey ?? NllbTranslatorApiKey;
                    if (!String.IsNullOrEmpty(apiKey))
                        apiKey = NllbAuthenticationPrefix + apiKey;
                    _nllbTranslator = new Translator(apiKey, options);
                }
                return _nllbTranslator;
            }
        }

        public static string NllbTranslatorApiKey
        {
            get
            {
                // since the user won't be able to encrypt the key, it'll be in clear text as an environment variable
                if (HasValidEnvironmentVariable(EnvVarNameKey, out string overrideKey))
                    return overrideKey;

                var key = Properties.Settings.Default.NllbTranslatorKeyOverride;

#if encryptingNewCredentials
                var translatorKey = EncryptionClass.Encrypt(key);
#endif
                return String.IsNullOrEmpty(key) ? String.Empty : EncryptionClass.Decrypt(key);
            }
            set
            {
                var trimmedValue = value?.Trim();
                var translatorKey = !String.IsNullOrEmpty(trimmedValue)
                                        ? EncryptionClass.Encrypt(trimmedValue)
                                        : null;
                Properties.Settings.Default.NllbTranslatorKeyOverride = translatorKey;
            }
        }

        public static string NllbTranslatorEndpoint
        {
            get
            {
                var endpoint = HasValidEnvironmentVariable(EnvVarNameEndPoint, out string overrideEndpoint)
                                ? overrideEndpoint
                                : !String.IsNullOrEmpty((overrideEndpoint = Properties.Settings.Default.NllbTranslatorEndpointOverride))
                                    ? overrideEndpoint
                                    : Properties.Settings.Default.NllbTranslatorEndpoint;
                return endpoint;
            }
            set
            {
                Properties.Settings.Default.NllbTranslatorEndpointOverride = String.IsNullOrEmpty(value) ? null : value;
            }
        }

#endregion Member Variable Definitions

        #region Initialization
        public NllbTranslatorEncConverter() : base(typeof(NllbTranslatorEncConverter).FullName,EncConverters.strTypeSILNllbTranslator)
        {
            // this is needed to be able to use the NLLB Translator (https call) from Word. If you don't have it, you just get this error:
            //  Unable to read data from the transport connection: An existing connection was forcibly closed by the remote host
            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;
        }

        public override void Initialize(string converterName, string converterSpec,
            ref string lhsEncodingID, ref string rhsEncodingID, ref ConvType conversionType,
            ref Int32 processTypeFlags, Int32 codePageInput, Int32 codePageOutput, bool bAdding)
        {
            Util.DebugWriteLine(this, $"BEGIN: {converterName}, {converterSpec}");

            // let the base class have first stab at it
            base.Initialize(converterName, converterSpec, ref lhsEncodingID, ref rhsEncodingID, 
                ref conversionType, ref processTypeFlags, codePageInput, codePageOutput, bAdding );

            if (!ParseConverterIdentifier(converterSpec, out string pathToDockerProject, out FromLanguage, out ToLanguage,
                                         out ApiKey, out Endpoint))
            {
                throw new ApplicationException($"{CstrDisplayName} not properly configured! converterName: {converterName}");
            }

            if (conversionType == ConvType.Unknown)
                conversionType = ConvType.Unicode_to_Unicode;

            // I'm assuming that we'd have to/want to set up a different one to go the other direction
            m_eConversionType = conversionType = MakeUniDirectional(conversionType);

            if (String.IsNullOrEmpty(lhsEncodingID))
                lhsEncodingID = m_strLhsEncodingID = EncConverters.strDefUnicodeEncoding;
            if (String.IsNullOrEmpty(rhsEncodingID))
                rhsEncodingID = m_strRhsEncodingID = EncConverters.strDefUnicodeEncoding;

            // this is a Translation process type by definition. This is used by various programs to prevent
            //  over usage -- e.g. Paratext should be blocking these EncConverter types as the 'Transliteration'
            //  type project EncConverter (bkz it'll try to "transliterate" the entire corpus -- probably not
            //  what's wanted). Also ClipboardEncConverter also doesn't process these for a preview (so the
            //  system tray popup doesn't take forever to display.
            processTypeFlags |= (int)ProcessTypeFlags.Translation;

            Util.DebugWriteLine(this, "END");
        }

        internal static bool ParseConverterIdentifier(string converterSpec, out string pathToDockerProject,
            out string fromLanguage, out string toLanguage, out string apiKey, out string endpoint)
        {
            toLanguage = null;

            string[] astrs = converterSpec.Split(new[] { ';' });

            if (astrs.Length < 3)
                throw new ApplicationException($"{CstrDisplayName} not properly configured! converterSpec: {converterSpec} must have the path to the Docker project and the source and target languages (eg. D:\\Docker\\NLLB;hin_Deva;eng_Latn)");

            pathToDockerProject = astrs[0];
            fromLanguage = astrs[1];
            toLanguage = astrs[2];

            endpoint = (astrs.Length >= 4) ? astrs[3] : NllbTranslatorEndpoint;
            apiKey = (astrs.Length >= 5) ? EncryptionClass.Decrypt(astrs[4]) : NllbTranslatorApiKey;

            return true;
        }

#pragma warning disable CS3002 // Return type is not CLS-compliant
        public async Task<Dictionary<string, string>> GetCapabilities(bool showError)
#pragma warning restore CS3002 // Return type is not CLS-compliant
        {
            try
            {
                var resultLanguagesSupported = await Task.Run(async delegate
                {
                    return (await NllbTranslator.GetSupportedLanguagesAsync()).ToList();
                }).ConfigureAwait(false);

                var json = LoadEmbeddedResourceFileAsStringExecutingAssembly("NllbHumanReadableLgNames.json");
                var languageCodeMap = JsonConvert.DeserializeObject<LanguageInfo[]>(json).ToDictionary(l => l.Code, l => l.Name);

                resultLanguagesSupported.Except(languageCodeMap.Select(l => l.Key))
                                        .ToList()
                                        .ForEach(s => languageCodeMap.Add(s, s));
                return languageCodeMap;
            }
            catch (Exception ex)
            {
                var error = GetErrorMsg(ex);
                if (showError)
                    MessageBox.Show(error, EncConverters.cstrCaption);
                else
                    System.Diagnostics.Debug.WriteLine(error);
            }
            return null;
        }

        public class LanguageInfo
        {
            [JsonProperty("Code")]
            public string Code { get; set; }

            [JsonProperty("Name")]
            public string Name { get; set; }
        }
        #endregion Initialization

        #region Abstract Base Class Overrides

        [CLSCompliant(false)]
        protected override unsafe void DoConvert
            (
            byte*       lpInBuffer,
            int         nInLen,
            byte*       lpOutBuffer,
            ref int     rnOutLen
            )
        {
            // we need to put it *back* into a string for the lookup
            // [aside: I should probably override base.InternalConvertEx so I can avoid having the base 
            //  class version turn the input string into a byte* for this call just so we can turn around 
            //  and put it *back* into a string for our processing... but I like working with a known 
            //  quantity and no other EncConverter does it that way. Besides, I'm afraid I'll break smtg ;-]
            byte[] baIn = new byte[nInLen];
            ECNormalizeData.ByteStarToByteArr(lpInBuffer, nInLen, baIn);

            char[] caIn = Encoding.Unicode.GetChars(baIn);

            // here's our input string
            var strInput = new string(caIn);

            var strOutput = DoConvert(strInput);

            StringToProperByteStar(strOutput, lpOutBuffer, ref rnOutLen);
            return;
        }

        protected string DoConvert(string strInput)
        {
            // if we're not already splitting sentences, see if we're getting the same string over and over again
            if ((LastInputString == strInput) && (RepeatsOfLastInputString++ > 0))
            {
                // ... on the 3rd time of getting the same string, start the splitting of sentences and processing them separately
                //    to see if that helps. Here's an example of one that the facebook-1.3G model seems to lose its way with:
                //    (केवल ये शहीद और न्याय करने वाले लोग हजार वर्षों वाले उस युग के आरंभ में पुनर्जीवित हो जाएँगे। इस बार जीवित होने को “पहला जीवित होना” कहते हैं। बाकि जो मरे हुए हैं, परमेश्वर उन सबको तब तक पुनर्जीवित नहीं करेगा, जब तक उस हजार वर्षों वाले युग का अंत नहीं होगा।)
                //  litBt: (Only these martyrs and judge-doing ones will become alive again in the beginning of that thousand year era. This time of becoming alive is called the “first resurrection.” The remaining (ones) who have died, God will not make them alive again until the end of that thousand years era happens.)
                // 1.3G model (before splitting sentences):
                //  (The first resurrection is the first.) The rest of the dead will not be raised until the thousand years are ended.
                // 1.3G model (after splitting sentences):
                //  (Only these martyrs and judges will be resurrected at the beginning of the millennial age. This resurrection is called the first resurrection.) (The rest of the dead will not be raised until the thousand years are over.)
                IsSplitSentences = true;

                // start w/ 1 less than the current, so we'll likely do a split right away
                MaxTokensPerSentence = WordCount(strInput) - RepeatsOfLastInputString;

                // if the user keeps doing this until it goes negative, then just start over
                if (MaxTokensPerSentence < 0)
                {
                    SetSplitSentences(false);   // start over
                }
                System.Diagnostics.Debug.WriteLine($"NllbEncConverter: RepeatsOfLastInputString: {RepeatsOfLastInputString}, convert the string, \"{SplitSentencesPrefix} ON\" (or OFF), to turn on (or off) splitting sentences");
            }
            else
            {
                LastInputString = strInput;
                if (strInput.StartsWith(SplitSentencesPrefix))
                {
                    SetSplitSentences(strInput.Substring(Math.Min(strInput.Length, SplitSentencesPrefix.Length))?.StartsWith("ON") ?? false);
                    return strInput;
                }
            }

            var toAdd = default((int, string));
            var sentences = new List<(int WordCount, string Sentence)>();
            if (IsSplitSentences)
            {
                SentenceSplitter.Replace(strInput, LimitSentenceLength);
                if (sentences.Sum(s => s.Sentence.Length) < strInput.Length)
                {
                    var accumulatedSentences = String.Join(String.Empty, sentences.Select(s => s.Sentence));
                    System.Diagnostics.Debug.Assert(strInput.Contains(accumulatedSentences));
                    var leftOver = strInput.Substring(accumulatedSentences.Length); // add the remaining bit that didn't end in a sentence final puntuation
                    var lastSentence = sentences.LastOrDefault();
                    if (lastSentence != default)
                    {
                        var addlInput = lastSentence.Sentence + leftOver;
                        if (WordCount(addlInput) <= MaxTokensPerSentence)
                        {
                            sentences.Remove(lastSentence);      // remove it here, so it can be added back combined later
                            leftOver = addlInput;
                        }
                    }
                    toAdd = (WordCount(leftOver), leftOver);
                }
            }
            else
                toAdd = (WordCount(strInput), strInput);

            if (toAdd != default)
                sentences.Add(toAdd);

            var strOutput = String.Empty;
            foreach (var sentence in sentences.Select(s => s.Sentence))
            {
                var output = String.IsNullOrEmpty(sentence.Trim())
                                    ? sentence
                                    : CallNllbTranslator(sentence).Result;

                // make sure the space isn't lost between the sentences
                if ((strOutput.LastOrDefault() != default) && (output?.First() != ' '))
                    strOutput += ' ';

                strOutput += output;
            }

            return strOutput;


            string LimitSentenceLength(Match match)
            {
                var sentence = match.ToString();
                var wordCount = WordCount(sentence);
                var lastSentence = sentences.LastOrDefault();

                // if there is no last sentence or if it would go beyond the limit to add this one to its Sentence...
                int combinedWordCount = wordCount;
                if ((lastSentence == default((int, string))) || ((combinedWordCount = (wordCount + lastSentence.WordCount)) > MaxTokensPerSentence))
                {
                    sentences.Add((wordCount, sentence));   // ... make this new one the new last sentence
                }
                else
                {
                    // otherwise, remove the current last sentence, and add this combined one back in
                    sentences.Remove(lastSentence);
                    var combinedSentence = lastSentence.Sentence + sentence;
                    sentences.Add((combinedWordCount, combinedSentence));
                }
                return sentence;    // always return the matched item, so we can see if we got the whole thing
            }

            void SetSplitSentences(bool isSplitSentences)    // expecting this to be either 'ON' or 'OFF'
            {
                IsSplitSentences = isSplitSentences;
                RepeatsOfLastInputString = isSplitSentences ? 1 : 0;
                System.Diagnostics.Debug.WriteLine($"NllbEncConverter: Sentence Splitting is {isSplitSentences}. Convert the same string again and it'll start up again w/ 1 less than the number of tokens in the next string sent for conversion.");
            }
        }

        private static int WordCount(string sentence)
        {
            return sentence.Split(new[] { ' ' }).Length;
        }

        private async Task<string> CallNllbTranslator(string strInput)
        {
            try
            {
                var translatedText = await Task.Run(async delegate
                {
                    return await NllbTranslator.TranslateTextAsync(strInput, FromLanguage, ToLanguage);
                }).ConfigureAwait(false);

                var result = HarvestResult(translatedText);
                return result;
            }
            catch (Exception ex)
            {
                var error = GetErrorMsg(ex);
                return error;
            }
        }

        private static string GetErrorMsg(Exception ex)
        {
            var error = LogExceptionMessage(CstrDisplayName, ex);
            if (error.Contains("Unauthorized"))
                error = String.Format("You need to edit this converter instance and in the Setup tab, enter the api key you set up in your NLLB Docker instance. {0}(if you didn't change the default key, then just delete the current key to revert back to the default key).{0}You may need to do this for each client app, since they store their Settings separately.{0}You can also use environment variables to set these values:{0}{1}{0}{2}{0}{0}{3}",
                                      Environment.NewLine, EnvVarNameKey, EnvVarNameEndPoint, error);
            if (error.Contains("Unable to connect to the remote server") || error.Contains("A connection with the server could not be established"))
                error = String.Format("Unable to reach the {1} service. Have you turned on the NLLB Docker container?{0}{0}{2}", Environment.NewLine, CstrDisplayName, error);
            return error;
        }

        private string HarvestResult(string jsonResult)
        {
            var jsonArray = JArray.Parse(jsonResult);
            var output = jsonArray.Select(obj => (string)obj["translatedText"])?.ToList();
            return String.Join(Environment.NewLine, output);
        }

		public override bool HasUserOverriddenCredentials => true;

		#endregion Abstract Base Class Overrides

		#region Misc helpers

		protected override string GetConfigTypeName
        {
            get { return typeof(NllbTranslatorEncConverterConfig).AssemblyQualifiedName; }
        }

        #endregion Misc helpers
    }
}
