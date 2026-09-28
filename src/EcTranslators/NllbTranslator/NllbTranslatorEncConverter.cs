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
using System.Text.RegularExpressions;
using System.Net.Sockets;
using System.IO;

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
        public static readonly Regex RegexLocalModelPath = new Regex(@"LOCAL_MODEL_PATH = '(.*?)'");

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
        public string PathToLocalModel;

        public Regex SentenceSplitter = new Regex(Properties.Settings.Default.NllbSentenceFinalPunctuationRegex);
        public Regex ClauseSplitter = new Regex(Properties.Settings.Default.NllbClausePunctuationRegex);
        public bool IsSplitSentences = Properties.Settings.Default.NllbProcessSentenceBySentence;
        private Regex _hasParagraphTerminators = new Regex(@"(\r\n|\r|\n)$");
        private static readonly Regex _lineBreaks = new Regex(@"(?<=\n)|(?<=\r)(?!\n)");
        private static readonly Regex _whitespace = new Regex(@"\s+");

        public int RepeatsOfLastInputString { get; set; } = 0;
        public string LastInputString { get; set; }

        /// <summary>
        /// When splitting sentences, consecutive sentences (within a paragraph) are sent together as long as their
        /// combined word count doesn't exceed this. 0 (the default) means each sentence is sent separately.
        /// </summary>
        public int MaxTokensPerSentence { get; set; }

        /// <summary>
        /// If a translation times out or the model gets stuck repeating itself, the text is retried in two halves
        /// (split at sentence punctuation, then clause punctuation, then a word boundary), recursively to this depth.
        /// </summary>
        public const int MaxRetrySplitDepth = 3;
        public const int MinWordsToSplitAtWordBoundary = 6;

        public static TimeSpan RequestTimeout => TimeSpan.FromSeconds(Math.Max(5, Properties.Settings.Default.NllbRequestTimeoutSeconds));
        public static bool RetryShorterOnFailure => Properties.Settings.Default.NllbRetryShorterOnFailure;

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
                    // Since we supply our own ClientFactory, DeepL's MaximumNetworkRetries, PerRetryConnectionTimeout,
                    //  and OverallConnectionTimeout options are ignored (no retries happen), so these are the timeouts
                    //  that apply. The server only sends the response headers once the model has finished generating,
                    //  so ReceiveHeadersTimeout (WinHttpHandler's default is 30 secs) is effectively the translation
                    //  time limit.
                    var timeout = RequestTimeout;
                    var handler = new DeepLTranslator.Http2CustomHandler
                    {
                        SendTimeout = timeout,
                        ReceiveHeadersTimeout = timeout,
                        ReceiveDataTimeout = timeout,
                    };
                    var serverUrl = Endpoint ?? NllbTranslatorEndpoint;

                    var options = new DeepL.TranslatorOptions
                    {
                        ServerUrl = serverUrl,
                        ClientFactory = () => new DeepL.HttpClientAndDisposeFlag
                        {
                            HttpClient = new HttpClient(handler) { Timeout = timeout },
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
                                         out ApiKey, out Endpoint, out PathToLocalModel))
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

        public static void SearchForSetting(Regex regex, string settingsFileContents, ref string apiKey)
        {
            var match = regex.Match(settingsFileContents);
            if (match.Success)
            {
                apiKey = match.Groups[1].Value;
            }
        }

        internal static bool ParseConverterIdentifier(string converterSpec, out string pathToDockerProject,
            out string fromLanguage, out string toLanguage, out string apiKey, out string endpoint, out string pathToLocalModel)
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

            // check if the settings.py file has a value for LOCAL_MODEL_PATH, and if so, then that's the path to the local model.
            pathToLocalModel = null;
            var pathToSettingsPy = Path.Combine(pathToDockerProject, "settings.py");
            if (File.Exists(pathToSettingsPy))
            {
                SearchForSetting(RegexLocalModelPath, File.ReadAllText(pathToSettingsPy), ref pathToLocalModel);
            }
            return true;
        }

        public static bool LocalModelFoundExists(string text)
        {
            return !String.IsNullOrEmpty(text) && Directory.Exists(text);
        }

        public static void FindLanguageNames(string srcLgCode, string trgLgCode, out string srcLgName, out string trgLgName)
        {
            var langCodeToName = LoadLanguageDictionary(Properties.Resources.LangCodeToNameMap);
            srcLgName = langCodeToName.ContainsKey(srcLgCode) ? langCodeToName[srcLgCode] : srcLgCode;
            trgLgName = langCodeToName.ContainsKey(trgLgCode) ? langCodeToName[trgLgCode] : trgLgCode;
        }

        private static Dictionary<string, string> LoadLanguageDictionary(string resourceText)
        {
            var dict = new Dictionary<string, string>();

            foreach (var line in resourceText.Split(
                new[] { "\r\n", "\n" },
                StringSplitOptions.RemoveEmptyEntries).Skip(1))
            {
                var parts = line.Split('\t');

                if (parts.Length < 2)
                    continue;

                var key = parts[0].Trim();
                var value = parts[1].Trim();

                // file has duplicate keys, but we only want to add the first occurrence to the dictionary, so check before adding
                if (!dict.ContainsKey(key))
                    dict.Add(key, value);
            }

            return dict;
        }

        public static async Task<bool> IsHttpServerListeningAsync(string endpoint, int timeoutMs = 500)
        {
            var uri = new Uri(endpoint);

            var host = uri.Host;   // "localhost"
            var port = uri.Port;   // 8000

            var isEndpointLive = await Task.Run(async delegate
            {
                using var client = new TcpClient();
                var connectTask = client.ConnectAsync(host, port);
                var timeoutTask = Task.Delay(timeoutMs);
                var completedTask = await Task.WhenAny(connectTask, timeoutTask);
                return connectTask.IsCompleted && client.Connected;

            }).ConfigureAwait(false);

            return isEndpointLive;
        }

#pragma warning disable CS3002 // Return type is not CLS-compliant
        public async Task<Dictionary<string, string>> GetCapabilities(bool showError)
#pragma warning restore CS3002 // Return type is not CLS-compliant
        {
            try
            {
                var resultLanguagesSupported = await Task.Run(async delegate
                {
                    var isEndpointLive = await IsHttpServerListeningAsync(Endpoint);
                    return (!isEndpointLive)
                            ? new List<string> { "Unable to connect to the NLLB server." }
                            : (await NllbTranslator.GetSupportedLanguagesAsync()).ToList();
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
            if (strInput.StartsWith(SplitSentencesPrefix))
            {
                SetSplitSentences(strInput.Substring(Math.Min(strInput.Length, SplitSentencesPrefix.Length)).StartsWith("ON"));
                return strInput;
            }

            // If we get the same string for the 3rd time in a row (e.g. because the user didn't like the translation and
            //  is trying again), send it clause by clause (i.e. also split at commas, etc.) to see if that helps.
            //  Splitting helps a model that has lost its way on a long input. E.g. here's one the facebook-1.3G model
            //  lost its way with when it got the whole thing at once:
            //    (केवल ये शहीद और न्याय करने वाले लोग हजार वर्षों वाले उस युग के आरंभ में पुनर्जीवित हो जाएँगे। इस बार जीवित होने को “पहला जीवित होना” कहते हैं। बाकि जो मरे हुए हैं, परमेश्वर उन सबको तब तक पुनर्जीवित नहीं करेगा, जब तक उस हजार वर्षों वाले युग का अंत नहीं होगा।)
            //  litBt: (Only these martyrs and judge-doing ones will become alive again in the beginning of that thousand year era. This time of becoming alive is called the “first resurrection.” The remaining (ones) who have died, God will not make them alive again until the end of that thousand years era happens.)
            //  1.3G model (whole thing at once):
            //   (The first resurrection is the first.) The rest of the dead will not be raised until the thousand years are ended.
            //  1.3G model (sentence by sentence):
            //   (Only these martyrs and judges will be resurrected at the beginning of the millennial age. This resurrection is called the first resurrection.) (The rest of the dead will not be raised until the thousand years are over.)
            if (LastInputString == strInput)
            {
                RepeatsOfLastInputString++;
            }
            else
            {
                LastInputString = strInput;
                RepeatsOfLastInputString = 0;
            }

            var isSplitClauses = RepeatsOfLastInputString >= 2;
            System.Diagnostics.Debug.WriteLineIf(isSplitClauses, $"NllbEncConverter: same input {RepeatsOfLastInputString + 1} times in a row, so translating it clause by clause. Convert \"{SplitSentencesPrefix}ON\" (or OFF) to turn on (or off) splitting sentences");

            var segments = IsSplitSentences
                            ? SplitIntoSentences(strInput)
                            : new List<string> { strInput };
            if (isSplitClauses)
                segments = segments.SelectMany(s => SplitAfterEvery(ClauseSplitter, s)).ToList();

            var strOutput = String.Empty;
            foreach (var segment in segments)
            {
                var output = String.IsNullOrEmpty(segment.Trim())
                                    ? segment
                                    : TranslateWithFallback(segment);

                // make sure the space isn't lost between the sentences
                if ((strOutput.LastOrDefault() != default) && !_hasParagraphTerminators.IsMatch(strOutput) && (output?.FirstOrDefault() != ' '))
                    strOutput += ' ';

                strOutput += output;
            }

            return strOutput;
        }

        private void SetSplitSentences(bool isSplitSentences)    // from converting "\SplitSentences ON" (or OFF)
        {
            IsSplitSentences = isSplitSentences;
            LastInputString = null;
            RepeatsOfLastInputString = 0;
            System.Diagnostics.Debug.WriteLine($"NllbEncConverter: Sentence Splitting is {isSplitSentences}. (Converting the same string 3 times in a row translates it clause by clause.)");
        }

        /// <summary>
        /// Splits text into sentences, each keeping its trailing punctuation, whitespace, and line break, so the pieces
        /// always add up to the whole input. Consecutive sentences in the same paragraph are combined as long as they
        /// total no more than MaxTokensPerSentence words (by default, 0, so they aren't combined).
        /// </summary>
        public List<string> SplitIntoSentences(string text)
        {
            var sentences = new List<(int WordCount, string Sentence)>();

            // do lines separately, so a line without sentence final punctuation doesn't get lost or run into the next one
            foreach (var line in _lineBreaks.Split(text).Where(l => l.Length > 0))
            {
                var firstSentenceOfLine = sentences.Count;
                foreach (var sentence in SplitAfterEvery(SentenceSplitter, line))
                {
                    var wordCount = WordCount(sentence);
                    var last = sentences.Count - 1;
                    if ((last >= firstSentenceOfLine) && (sentences[last].WordCount + wordCount <= MaxTokensPerSentence))
                        sentences[last] = (sentences[last].WordCount + wordCount, sentences[last].Sentence + sentence);
                    else
                        sentences.Add((wordCount, sentence));
                }
            }

            return sentences.Select(s => s.Sentence).ToList();
        }

        /// <summary>Splits text after each match of the regex (the pieces always add up to the whole text).</summary>
        private static IEnumerable<string> SplitAfterEvery(Regex boundary, string text)
        {
            var start = 0;
            foreach (Match match in boundary.Matches(text))
            {
                var end = match.Index + match.Length;
                if ((end <= start) || (end >= text.Length))
                    continue;
                yield return text.Substring(start, end - start);
                start = end;
            }

            if (start < text.Length)
                yield return text.Substring(start);
        }

        /// <summary>
        /// Splits text in two, as near the middle as possible, to retry a translation that failed: at sentence final
        /// punctuation if there's any, otherwise at clause punctuation (e.g. commas), otherwise (if it has enough words)
        /// at the space nearest the middle. That last one doesn't need to know anything about the language, but can
        /// split a phrase, so it's the last resort.
        /// </summary>
        public bool TrySplitForRetry(string text, out string left, out string right)
        {
            return TrySplitNearMiddle(text, SentenceSplitter, out left, out right)
                || TrySplitNearMiddle(text, ClauseSplitter, out left, out right)
                || ((WordCount(text.Trim()) >= MinWordsToSplitAtWordBoundary)
                    && TrySplitNearMiddle(text, _whitespace, out left, out right));
        }

        private static bool TrySplitNearMiddle(string text, Regex boundary, out string left, out string right)
        {
            left = right = null;
            var length = text.TrimEnd().Length;   // ignoring any trailing paragraph terminator
            var middle = length / 2.0;
            var best = -1;
            foreach (Match match in boundary.Matches(text.Substring(0, length)))
            {
                var end = match.Index + match.Length;
                if ((end >= length) || String.IsNullOrWhiteSpace(text.Substring(0, end)))
                    continue;
                if ((best < 0) || (Math.Abs(end - middle) < Math.Abs(best - middle)))
                    best = end;
            }

            if (best < 0)
                return false;

            left = text.Substring(0, best).TrimEnd();
            right = text.Substring(best);  // keeps the trailing paragraph terminator (if any)
            return true;
        }

        private static int WordCount(string sentence)
        {
            return sentence.Split(new[] { ' ' }).Length;
        }

        /// <summary>
        /// Translates the text, and if that times out or the model gets stuck repeating itself, retries it in two
        /// halves (each of which can be split again, up to MaxRetrySplitDepth times). Not every translation server
        /// catches (or recovers from) its model getting stuck, so this works with any of them.
        /// </summary>
        private string TranslateWithFallback(string text, int depth = 0)
        {
            var result = CallNllbTranslator(text).Result;
            if (result.Failure == TranslationFailure.None)
                return result.Output;

            // no point retrying other failures (e.g. unauthorized or can't connect), nor splitting what the server
            //  already tried splitting
            if (RetryShorterOnFailure && (result.Failure != TranslationFailure.Other) && !result.ServerAlreadySplit
                && (depth < MaxRetrySplitDepth) && TrySplitForRetry(text, out var left, out var right))
            {
                System.Diagnostics.Debug.WriteLine($"NllbEncConverter: {result.Failure} translating \"{text}\", so retrying it as \"{left}\" + \"{right}\"");

                // if we gave up waiting, the server is probably still busy with it, and our retry would have to wait
                //  for it to finish anyway (counting against its timeout)
                if (result.Failure == TranslationFailure.Timeout)
                    WaitForServerToFinish();

                var leftOutput = TranslateWithFallback(left, depth + 1);
                var rightOutput = TranslateWithFallback(right, depth + 1);
                return leftOutput.TrimEnd(' ') + ' ' + rightOutput.TrimStart(' ');
            }

            return result.Output;
        }

        private enum TranslationFailure
        {
            None,
            Timeout,
            Degenerate,     // the model got stuck (e.g. repeating a phrase over and over)
            Other,
        }

        private struct TranslationResult
        {
            public string Output;               // the translation, or if it failed, the error message
            public TranslationFailure Failure;
            public bool ServerAlreadySplit;     // the server already retried it in pieces (so we needn't)
        }

        private async Task<TranslationResult> CallNllbTranslator(string strInput)
        {
            // make sure the paragraph terminator (if any) isn't lost -- even if it's an error message
            var match = _hasParagraphTerminators.Match(strInput);
            var paragraphTerminator = match.Success ? match.Value : String.Empty;

            try
            {
                var translatedText = await Task.Run(async delegate
                {
                    return await NllbTranslator.TranslateTextAsync(strInput, FromLanguage, ToLanguage);
                }).ConfigureAwait(false);

                var result = HarvestResult(translatedText);

                // a server that doesn't watch for its model getting stuck in a loop just returns whatever it produced
                //  by the time it hit its maximum output length, so check for that here
                if (HasRepetitionLoop(result, out var partial))
                {
                    var error = GetErrorMsg(new ApplicationException($"The translation model got stuck repeating itself on this text. Try translating a shorter piece of it. Partial translation: {partial}"));
                    return new TranslationResult { Output = error + paragraphTerminator, Failure = TranslationFailure.Degenerate };
                }

                return new TranslationResult { Output = result + paragraphTerminator };
            }
            catch (Exception ex)
            {
                if (TryParseDegenerateOutputError(ex, out var message, out var serverAlreadySplit))
                {
                    return new TranslationResult
                    {
                        Output = GetErrorMsg(new ApplicationException(message)) + paragraphTerminator,
                        Failure = TranslationFailure.Degenerate,
                        ServerAlreadySplit = serverAlreadySplit,
                    };
                }

                return new TranslationResult
                {
                    Output = GetErrorMsg(ex) + paragraphTerminator,
                    Failure = IsTimeout(ex) ? TranslationFailure.Timeout : TranslationFailure.Other,
                };
            }
        }

        /// <summary>
        /// Checks for the tell-tale sign of a model that has gotten stuck: some run of 2-10 words repeated 4 or more
        /// times in a row (e.g. "बुरियें आत्‍में दी बुरियें आत्‍में दी बुरियें आत्‍में दी ..."). If found, 'partial' is the text
        /// up to and including the first copy of the repeated words.
        /// </summary>
        public static bool HasRepetitionLoop(string text, out string partial, int maxUnitWords = 10, int minRepeats = 4)
        {
            partial = text;
            var words = text.Split((char[])null, StringSplitOptions.RemoveEmptyEntries);
            for (var start = 0; start < words.Length; start++)
            {
                // (a single word repeated 8+ times is also a 2-word unit repeated 4+ times)
                for (var n = 2; (n <= maxUnitWords) && (start + n * minRepeats <= words.Length); n++)
                {
                    var repeats = 1;
                    while ((start + (repeats + 1) * n <= words.Length) && IsSameWords(words, start, start + repeats * n, n))
                        repeats++;

                    if (repeats >= minRepeats)
                    {
                        partial = String.Join(" ", words.Take(start + n));
                        return true;
                    }
                }
            }
            return false;
        }

        private static bool IsSameWords(string[] words, int first, int second, int count)
        {
            for (var i = 0; i < count; i++)
            {
                if (words[first + i] != words[second + i])
                    return false;
            }
            return true;
        }

        /// <summary>
        /// The TranslateGemma docker server replies with a 422 {"code": "degenerate_output", ...} if its model gets
        /// stuck on the text (and it couldn't recover by retrying it in pieces itself).
        /// </summary>
        private static bool TryParseDegenerateOutputError(Exception ex, out string message, out bool serverAlreadySplit)
        {
            message = null;
            serverAlreadySplit = false;
            for (; ex != null; ex = ex.InnerException)
            {
                if (!ex.Message.Contains("degenerate_output"))
                    continue;

                try
                {
                    var json = JObject.Parse(ex.Message);
                    if ((string)json["code"] != "degenerate_output")
                        continue;

                    serverAlreadySplit = (bool?)json["splitAttempted"] ?? false;
                    var partial = (string)json["partialTranslation"];
                    message = (string)json["error"]
                            + (String.IsNullOrEmpty(partial) ? String.Empty : $" Partial translation: {partial}");
                    return true;
                }
                catch (JsonException)
                {
                }
            }
            return false;
        }

        private static bool IsTimeout(Exception ex)
        {
            for (; ex != null; ex = ex.InnerException)
            {
                if ((ex is TimeoutException) || (ex is TaskCanceledException)
                    || ((ex is System.ComponentModel.Win32Exception win32Ex) && (win32Ex.NativeErrorCode == 12002))  // ERROR_WINHTTP_TIMEOUT
                    || ex.Message.Contains("timed out"))
                    return true;

                if ((ex is AggregateException aggregateEx) && aggregateEx.InnerExceptions.Any(IsTimeout))
                    return true;
            }
            return false;
        }

        /// <summary>
        /// The longest we'll wait for the server to finish with a request we gave up on, before sending a retry anyway.
        /// (With the TranslateGemma server, a runaway generation of 1024 tokens takes ~90 secs on an 8GB GPU)
        /// </summary>
        public static readonly TimeSpan MaxWaitForBusyServer = TimeSpan.FromMinutes(5);

        /// <summary>
        /// After we time out, the server is likely still busy with the request we gave up on, and a retry would just
        /// wait behind it (and count that time against its own timeout -- so it'd likely time out too). The TranslateGemma
        /// server's /healthz says "busy": true while it's generating, or (being single-threaded) doesn't answer at all
        /// until it's done, so wait for it to answer "not busy". For other servers (i.e. with no such endpoint), a
        /// 404 (etc.) that only comes back once they're free works too.
        /// </summary>
        private void WaitForServerToFinish()
        {
            var pollTimeout = RequestTimeout;
            var deadline = DateTime.UtcNow + ((pollTimeout > MaxWaitForBusyServer) ? pollTimeout : MaxWaitForBusyServer);
            var healthUrl = new Uri(new Uri(Endpoint ?? NllbTranslatorEndpoint), "/healthz");
            Task.Run(async delegate
            {
                using var client = new HttpClient { Timeout = pollTimeout };
                while (DateTime.UtcNow < deadline)
                {
                    try
                    {
                        using var response = await client.GetAsync(healthUrl).ConfigureAwait(false);
                        if (!response.IsSuccessStatusCode)
                            return;     // not a server that tells us, but at least it's answering again

                        var json = JObject.Parse(await response.Content.ReadAsStringAsync().ConfigureAwait(false));
                        if (!((bool?)json["busy"] ?? false))
                            return;
                    }
                    catch (TaskCanceledException)
                    {
                        // timed out (i.e. it's still too busy to answer), so keep waiting
                        System.Diagnostics.Debug.WriteLine($"NllbEncConverter: still waiting for the server to finish with the request that timed out");
                        continue;
                    }
                    catch (Exception ex)
                    {
                        System.Diagnostics.Debug.WriteLine($"NllbEncConverter: couldn't check if the server is still busy: {ex.Message}");
                        return;
                    }

                    await Task.Delay(TimeSpan.FromSeconds(1)).ConfigureAwait(false);
                }
            }).Wait();
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
