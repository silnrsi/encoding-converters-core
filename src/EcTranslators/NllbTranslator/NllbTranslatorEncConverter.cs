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

namespace SilEncConverters40.EcTranslators.NllbTranslator
{
	/// <summary>
	/// Managed Nllb Translate EncConverter.
	/// </summary>
#if X64
	[GuidAttribute("DAFFF949-BA9C-4A28-B7A3-13D205D0B838")]
#else
	[GuidAttribute("DAFFF949-BA9C-4A28-B7A3-13D205D0B838")]
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

		#endregion Const Definitions

		#region Member Variable Definitions


		// borrowing from deepL's approach
		private static Translator _nllbTranslator;
		public static Translator NllbTranslator
		{
			get
			{
				if (_nllbTranslator == null)
				{
					var handler = new DeepLTranslator.Http2CustomHandler();
					var options = new DeepL.TranslatorOptions
					{
						MaximumNetworkRetries = 2,
						PerRetryConnectionTimeout = TimeSpan.FromSeconds(5),
						OverallConnectionTimeout = TimeSpan.FromSeconds(10),
						ClientFactory = () => new DeepL.HttpClientAndDisposeFlag
						{
							HttpClient = new HttpClient(handler),
							DisposeClient = true,
						},
					};
					_nllbTranslator = new Translator(NllbTranslatorApiKey, options);
				}
				return _nllbTranslator;
			}
		}

		public static string NllbTranslatorApiKey
		{
			get
			{
				var overrideKey = Properties.Settings.Default.NllbTranslatorKeyOverride;
				var key = (!String.IsNullOrEmpty(overrideKey))
							? overrideKey
							: Properties.Settings.Default.NllbTranslatorKey;
				// var translatorKey = EncryptionClass.Encrypt(key);
				return EncryptionClass.Decrypt(key);
			}
			set
			{
				var translatorKey = EncryptionClass.Encrypt(value);
				Properties.Settings.Default.NllbTranslatorKeyOverride = translatorKey;
			}
		}

		protected string fromLanguage;
		protected string toLanguage;

		#endregion Member Variable Definitions

		#region Initialization
		public NllbTranslatorEncConverter() : base(typeof(NllbTranslatorEncConverter).FullName,EncConverters.strTypeSILNllbTranslator)
        {
			// this is needed to be able to use the NLLB Translator (https call) from Word. If you don't have it, you just get this error:
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

			if (!ParseConverterIdentifier(converterSpec, out fromLanguage, out toLanguage))
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
			processTypeFlags |= (int)ProcessTypeFlags.Translation;

			Util.DebugWriteLine(this, "END");
		}

		internal static bool ParseConverterIdentifier(string converterSpec,
			out string fromLanguage, out string toLanguage)
		{
			toLanguage = null;

			string[] astrs = converterSpec.Split(new[] { ';' });

			if (astrs.Length < 2)
				throw new ApplicationException($"{CstrDisplayName} not properly configured! converterSpec: {converterSpec} must have at least a source and target language (eg. hin_Deva;eng_Latn)");

			fromLanguage = astrs[0];
			toLanguage = astrs[1];
			return true;
		}

#pragma warning disable CS3002 // Return type is not CLS-compliant
		public async static Task<Dictionary<string, string>> GetCapabilities()
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
				var error = LogExceptionMessage($"{typeof(NllbTranslatorEncConverter).Name}.GetCapabilities", ex);
				if (error.Contains("Unable to connect to the remote server"))
					error += String.Format("{0}{0}Unable to reach the {1} service. Have you turned on the NLLB Docker container?", Environment.NewLine, CstrDisplayName);
				MessageBox.Show(error, EncConverters.cstrCaption);
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

			var strOutput = String.IsNullOrEmpty(strInput)
								? strInput
								: CallNllbTranslator(strInput).Result;

			StringToProperByteStar(strOutput, lpOutBuffer, ref rnOutLen);
		}

		private async Task<string> CallNllbTranslator(string strInput)
		{
			try
			{
				var translatedText = await Task.Run(async delegate
				{
					return await NllbTranslator.TranslateTextAsync(strInput, fromLanguage, toLanguage);
				}).ConfigureAwait(false);

				var result = HarvestResult(translatedText);
				return result;
			}
			catch (Exception ex)
			{
				return LogExceptionMessage(GetType().Name, ex);
			}
		}

		private string HarvestResult(string jsonResult)
		{
			var jsonArray = JArray.Parse(jsonResult);
			var output = jsonArray.Select(obj => (string)obj["translatedText"])?.ToList();
			return String.Join(Environment.NewLine, output);
		}

		#endregion Abstract Base Class Overrides

		#region Misc helpers

		protected override string GetConfigTypeName
		{
			get { return typeof(NllbTranslatorEncConverterConfig).AssemblyQualifiedName; }
		}

		#endregion Misc helpers
	}
}
