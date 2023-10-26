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

namespace SilEncConverters40.EcTranslators.BingTranslator
{
	/// <summary>
	/// Managed Bing Translator EncConverter.
	/// </summary>
	[GuidAttribute("73742D80-5508-4500-9FAA-AF82E4756C35")]
	// normally these subclasses are treated as the base class (i.e. the 
	//  client can use them orthogonally as IEncConverter interface pointers
	//  so normally these individual subclasses would be invisible), but if 
	//  we add 'ComVisible = false', then it doesn't get the registry 
	//  'HKEY_CLASSES_ROOT\SilEncConverters40.EcTranslators.BingTranslatorEncConverter' which is the basis of 
	//  how it is started (see EncConverters.AddEx).
	// [ComVisible(false)] 
	public class BingTranslatorEncConverter : TranslatorConverter
	{
		#region Const Definitions
		// by putting the azure key in a settings file, users can get their own free azure account, create their own 'Translator'
		//  resource, and enter their own key in the file (or the UI to have us set it in the file) and get their own 2E6 chars free
		//	(or pay for as much as they want)
		// see https://docs.microsoft.com/en-us/azure/cognitive-services/translator/quickstart-translator
		public static string AzureTranslatorSubscriptionKey
		{
			get
			{
				var overrideKey = Properties.Settings.Default.AzureTranslatorKeyOverride;
				var key = (!String.IsNullOrEmpty(overrideKey))
							? overrideKey
							: Properties.Settings.Default.AzureTranslatorKey;
				return EncryptionClass.Decrypt(key);
			}
			set
			{
				// the value is already encrypted by the time it gets here
				Properties.Settings.Default.AzureTranslatorKeyOverride = value;
			}
		}

		private static readonly string endpointTranslator = Properties.Settings.Default.AzureTranslatorTextTranslationEndpoint;
		private static readonly string endpointCapabilities = $"{endpointTranslator}languages?api-version=3.0";

		// Add your location, also known as region. The default is global.
		// This is required if using a Cognitive Services resource.
		public static string AzureTranslatorLocation = Properties.Settings.Default.AzureTranslatorRegion;

		public enum TransductionType
		{
			Unknown,
			Translate,
			Transliterate,
			TranslateWithTransliterate,
			DictionaryLookup
		};

        public const string CstrDisplayName = "Bing Translator";
        internal const string strHtmlFilename  = "Bing_Translator_Plug-in_About_box.htm";
		#endregion Const Definitions

		#region Member Variable Definitions

		protected TransductionType transductionRequested;
		protected string fromLanguage;
		protected string toLanguage;
		protected string toScript;
		protected string fromScript;

		#endregion Member Variable Definitions

		#region Initialization
		public BingTranslatorEncConverter() : base(typeof(BingTranslatorEncConverter).FullName,EncConverters.strTypeSILBingTranslator)
        {
			// this is needed to be able to use the Azure Translator (https call) from Word. If you don't have it, you just get this error:
			//	Unable to read data from the transport connection: An existing connection was forcibly closed by the remote host
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

			if (!ParseConverterIdentifier(converterSpec, out transductionRequested,
										  ref fromLanguage, out toLanguage, out fromScript, out toScript))
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
			//	over usage -- e.g. Paratext should be blocking these EncConverter types as the 'Transliteration'
			//	type project EncConverter (bkz it'll try to "transliterate" the entire corpus -- probably not
			//	what's wanted). Also ClipboardEncConverter also doesn't process these for a preview (so the
			//	system tray popup doesn't take forever to display.
			processTypeFlags |= (int)ProcessTypeFlags.Translation;	// this is a Translation process type by definition

			Util.DebugWriteLine(this, "END");
		}

		internal static bool ParseConverterIdentifier(string converterSpec,
			out TransductionType transductionRequested,
			ref string fromLanguage, out string toLanguage,
			out string fromScript, out string toScript)
		{
			transductionRequested = TransductionType.Unknown;
			toLanguage = fromScript = toScript = null;

			string[] astrs = converterSpec.Split(new[] { ';' });

			// gotta at least have the transduction type requested and the toLanguage
			if (astrs.Length < 3)
				return false;

			if (!Enum.TryParse(astrs[0], out transductionRequested))
				return false;

			if (!String.IsNullOrEmpty(astrs[1]))
				fromLanguage = astrs[1];	// don't change it (from Auto-Detect) if it was saved as empty space

			toLanguage = astrs[2];
			fromScript = (astrs.Length >= 4) ? astrs[3] : null;
			toScript = (astrs.Length >= 5) ? astrs[4] : null;
			return true;
		}

		public static async Task<string> GetTranslationCapabilities()
		{
			try
			{
				string jsonResponse = null;
				using (HttpClient client = new HttpClient())
				{
					var response = await client.GetAsync(endpointCapabilities).ConfigureAwait(false);
					jsonResponse = await response.Content.ReadAsStringAsync().ConfigureAwait(false);
					if (!response.IsSuccessStatusCode)
					{
						throw new ApplicationException($"You must have internet in order to use the {CstrDisplayName}. Are you online? (response: {jsonResponse})");
					}
				}
				return jsonResponse;
			}
			catch (Exception ex)
			{
				return LogExceptionMessage("GetTranslationCapabilities", ex);
			}
		}

		public static (List<TranslationLanguage> translations, List<TransliterationLanguage> transliterations, List<DictionaryLanguage> dictionaryOptions) GetCapabilities()
		{
			var jsonCapabilities = GetTranslationCapabilities().Result;
			JObject capabilities = JObject.Parse(jsonCapabilities);
			var translationTokens = capabilities["translation"].Children();
			var translations = TranslationLanguage.LoadFromJTokens(translationTokens);

			var transliterationsTokens = capabilities["transliteration"].Children();
			var transliterations = TransliterationLanguage.LoadFromJTokens(transliterationsTokens);

			var dictionaryTokens = capabilities["dictionary"].Children();
			var dictionaryOptions = DictionaryLanguage.LoadFromJTokens(dictionaryTokens);

			return (translations, transliterations, dictionaryOptions);
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
			CheckOverusage();

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

			var strOutput = String.IsNullOrEmpty(strInput)
								? strInput
								: CallBingTranslator(strInput).Result;

			StringToProperByteStar(strOutput, lpOutBuffer, ref rnOutLen);
		}

		private async Task<string> CallBingTranslator(string strInput)
		{
			try
			{
				// Input and output languages are defined as parameters.
				var routeSuffix = route;
				var body = new object[] { new { Text = CleanDictionaryLookupInputString(strInput) } };
				var requestBody = JsonConvert.SerializeObject(body);

				using (var client = new HttpClient())
				using (var request = new HttpRequestMessage())
				{
					// Build the request.
					request.Method = HttpMethod.Post;
					request.RequestUri = new Uri(endpointTranslator + routeSuffix);
					request.Content = new StringContent(requestBody, Encoding.UTF8, "application/json");
					request.Headers.Add("Ocp-Apim-Subscription-Key", AzureTranslatorSubscriptionKey);
					request.Headers.Add("Ocp-Apim-Subscription-Region", AzureTranslatorLocation);

					// Send the request and get response.
					var response = await client.SendAsync(request).ConfigureAwait(false);

					// Read response as a string.
					var result = await response.Content.ReadAsStringAsync();
					if (!response.IsSuccessStatusCode)
						throw new ApplicationException(result);

					result = HarvestResult(result);
					return result;
				}
			}
			catch (Exception ex)
			{
				return LogExceptionMessage(GetType().Name, ex);
			}
		}

		private string CleanDictionaryLookupInputString(string strInput)
		{
			// for dictionary lookup, make sure there's only a single word (or the lookup will return no translations)
			if (transductionRequested == TransductionType.DictionaryLookup)
			{
				strInput = strInput?.Trim();
				var nIndex = strInput?.IndexOf(' ');
				if (nIndex > 0)
					strInput = strInput.Substring(0, (int)nIndex);
			}

			return strInput;
		}

		private string HarvestResult(string jsonResult)
		{
			JArray json = JArray.Parse(jsonResult);
			string path = null, output = null;
			switch (transductionRequested)
			{
				case TransductionType.Translate:
					output = ExtractTranslation(json);
					break;
				case TransductionType.TranslateWithTransliterate:
					path = $"$..translations[?(@.to == '{toLanguage}')].transliteration.text";
					output = json.SelectToken(path)?.ToString();

					// not all languages support Transliteration. If this one doesn't, then see if simple 'Translation' works
					if (String.IsNullOrEmpty(output))
						output = ExtractTranslation(json);
					break;
				case TransductionType.Transliterate:
					path = $"[?(@.script == '{toScript}')].text";
					output = json.SelectToken(path)?.ToString();
					break;
				case TransductionType.DictionaryLookup:
					path = "$..translations[?(@displayTarget != null)].displayTarget";
					var translations = json.SelectTokens(path)?.ToList();
					output = (translations.Count == 0)
								? String.Empty
								: (translations.Count == 1)
									? translations.First().ToString()
									: translations.Aggregate($"%{translations.Count}%", (c, n) => { return c + $"{n}%"; });
					break;
			};
			return output ?? String.Empty;
		}

		private string ExtractTranslation(JArray json)
		{
			var path = $"$..translations[?(@.to == '{toLanguage}')].text";
			var output = json.SelectToken(path)?.ToString();
			return output;
		}

		#endregion Abstract Base Class Overrides

		#region Misc helpers

		protected string route
		{
			get
			{
				var fromIndicated = (!String.IsNullOrEmpty(fromLanguage)) ? $"&from={fromLanguage}" : String.Empty;
				var toIndicated = (!String.IsNullOrEmpty(toLanguage)) ? $"&to={toLanguage}" : String.Empty;
				switch (transductionRequested)
				{
					case TransductionType.Translate:
						return $"translate?api-version=3.0{fromIndicated}{toIndicated}";
					case TransductionType.Transliterate:
						// transliterate has a special form for the from language
						fromIndicated = (!String.IsNullOrEmpty(fromLanguage)) ? $"&language={fromLanguage}" : String.Empty;
						return $"transliterate?api-version=3.0{fromIndicated}&fromScript={fromScript}&toScript={toScript}";
					case TransductionType.TranslateWithTransliterate:
						return $"translate?api-version=3.0{fromIndicated}{toIndicated}&toScript={toScript}";
					case TransductionType.DictionaryLookup:
						return $"dictionary/lookup?api-version=3.0{fromIndicated}{toIndicated}";
					default:
						throw new ApplicationException($"Unknown TransductionRequested for BingTranslatorEncConverter: '{transductionRequested}'");
				};
			}
		}

		protected override string GetConfigTypeName
		{
			get { return typeof(BingTranslatorEncConverterConfig).AssemblyQualifiedName; }
		}

		#endregion Misc helpers
	}
}
