using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Runtime.InteropServices;   // for the class attributes
using System.Text;                      // for ASCIIEncoding
using System.Threading.Tasks;
using System.Windows.Forms;
using DeepL;
using ECInterfaces;                     // for ConvType

namespace SilEncConverters40.EcTranslators.DeepLTranslator
{
	/// <summary>
	/// Managed DeepL Translator EncConverter.
	/// </summary>
	[GuidAttribute("4EA28F93-EDAD-49A4-8FB7-3E22C5EAD07B")]
	// normally these subclasses are treated as the base class (i.e. the 
	//  client can use them orthogonally as IEncConverter interface pointers
	//  so normally these individual subclasses would be invisible), but if 
	//  we add 'ComVisible = false', then it doesn't get the registry 
	//  'HKEY_CLASSES_ROOT\SilEncConverters40.EcTranslators.DeepLTranslatorEncConverter' which is the basis of 
	//  how it is started (see EncConverters.AddEx).
	// [ComVisible(false)] 
	public class DeepLTranslatorEncConverter : TranslatorConverter
	{
		#region Const Definitions
		// by putting the DeepL key in a settings file, users can get their own free DeepL account, create their own 'Translator'
		//  resource, and enter their own key in the file (or the UI to have us set it in the file) and get their own 500K chars free
		//	(or pay for as much as they want)
		// see https://www.deepl.com/pro?cta=header-pro/

		private static Translator _deepLTranslator;
		public static Translator DeepLTranslator
		{
			get
			{
				if (_deepLTranslator == null)
				{
					var handler = new Http2CustomHandler();
					var options = new TranslatorOptions
					{
						MaximumNetworkRetries = 2,
						PerRetryConnectionTimeout = TimeSpan.FromSeconds(5),
						OverallConnectionTimeout = TimeSpan.FromSeconds(10),
						ClientFactory = () => new HttpClientAndDisposeFlag
						{
							HttpClient = new HttpClient(handler),
							DisposeClient = true,
						},
					};
					_deepLTranslator = new Translator(DeepLTranslatorSubscriptionKey, options);
				}
				return _deepLTranslator;
			}
		}

		public static string DeepLTranslatorSubscriptionKey
		{
			get
			{
				var overrideKey = Properties.Settings.Default.DeepLTranslatorKeyOverride;
				var key = (!String.IsNullOrEmpty(overrideKey))
							? overrideKey
							: Properties.Settings.Default.DeepLTranslatorKey;
				return EncryptionClass.Decrypt(key);
			}
			set
			{
				// the value is already encrypted by the time it gets here
				Properties.Settings.Default.DeepLTranslatorKeyOverride = value;
			}
		}

		public enum TransductionType
		{
			Unknown,
			Translate,
			DictionaryLookup
		};

		public const string CstrDisplayName = "DeepL Translator";
        internal const string strHtmlFilename  = "DeepL_Translator_Plug-in_About_box.htm";
		#endregion Const Definitions

		#region Member Variable Definitions

		protected TransductionType TransductionRequested;
		protected Formality FormalityLevel;
		protected string FromLanguage;
		protected string ToLanguage;

		#endregion Member Variable Definitions

		#region Initialization
		public DeepLTranslatorEncConverter() : base(typeof(DeepLTranslatorEncConverter).FullName,EncConverters.strTypeSILDeepLTranslator)
        {
			// this (might be?) needed to be able to use the DeepL Translator (https call) from Word. If you don't have it, you just get this error:
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

			if (!ParseConverterIdentifier(converterSpec, out TransductionRequested,
										  ref FromLanguage, out ToLanguage, out FormalityLevel))
			{
				throw new ApplicationException($"{CstrDisplayName} not properly configured! converterName: {converterName}");
			}

			if (conversionType == ConvType.Unknown)
				conversionType = ConvType.Unicode_to_Unicode;

			// I'm assuming that we'd never want to set up a different one to go the other direction
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
			processTypeFlags |= (int)ProcessTypeFlags.Translation;  // this is a Translation process type by definition

			Util.DebugWriteLine(this, $"END: {converterName}, {converterSpec}");
		}

		internal static bool ParseConverterIdentifier(string converterSpec,
			out TransductionType transductionRequested,
			ref string fromLanguage, out string toLanguage,
			out Formality formalityLevel)
		{
			transductionRequested = TransductionType.Unknown;
			formalityLevel = Formality.Default;
			toLanguage = null;

			string[] astrs = converterSpec.Split(new[] { ';' });

			// gotta at least have the transduction type requested and the toLanguage
			if (astrs.Length < 2)
				return false;

			if (!Enum.TryParse(astrs[0], out transductionRequested))
				return false;

			if (!String.IsNullOrEmpty(astrs[1]))
				fromLanguage = astrs[1];	// don't change it (from Auto-Detect) if it was saved as empty space

			toLanguage = astrs[2];
			if (astrs.Length == 4)
				if (!Enum.TryParse(astrs[3], out formalityLevel))
					return false;

			return true;
		}

#pragma warning disable CS3002 // Return type is not CLS-compliant
		public async static Task<(List<DeepL.Model.SourceLanguage> languagesSource,
						List<DeepL.Model.TargetLanguage> languagesTarget,
						List<DeepL.Model.GlossaryLanguagePair> glossaryLanguagePairs,
						string usageLeft)>
							GetCapabilities()
#pragma warning restore CS3002 // Return type is not CLS-compliant
		{
			try
			{
				// Source and target languages
				var resultSLs = await Task.Run(async delegate
				{
					return await DeepLTranslator.GetSourceLanguagesAsync();
				}).ConfigureAwait(false);

				var sourceLanguages = resultSLs.ToList();
				foreach (var lang in sourceLanguages)
				{
					System.Diagnostics.Debug.Write($"{lang.Name} ({lang.Code})"); // Example: "English (EN)"
				}

				var targetLanguages = DeepLTranslator.GetTargetLanguagesAsync().Result.ToList();
				foreach (var lang in targetLanguages)
				{
					if (lang.SupportsFormality)
					{
						System.Diagnostics.Debug.Write($"{lang.Name} ({lang.Code}) supports formality");
						// Example: "German (DE) supports formality"
					}
				}

				// Glossary languages
				var glossaryLanguages = DeepLTranslator.GetGlossaryLanguagesAsync().Result.ToList();
				foreach (var languagePair in glossaryLanguages)
				{
					System.Diagnostics.Debug.Write($"{languagePair.SourceLanguageCode} to {languagePair.TargetLanguageCode}");
					// Example: "EN to DE", "DE to EN"
				}

				string usageLeft = null;
				var usage = DeepLTranslator.GetUsageAsync().Result;
				if (usage.AnyLimitReached)
				{
					usageLeft = "Translation limit exceeded! You need your own API key to use it (or wait for the next month).";
				}
				else if (usage.Character != null)
				{
					usageLeft = $"Character usage: {usage.Character} for the month";
				}
				else
				{
					Console.WriteLine($"{usage}");
				}

				return (sourceLanguages, targetLanguages, glossaryLanguages, usageLeft);
			}
			catch (Exception ex)
			{
				var error = LogExceptionMessage($"{typeof(DeepLTranslatorEncConverter).Name}.GetCapabilities", ex);
				if (error.Contains("The server name or address could not be resolved"))
					error += String.Format("{0}{0}Unable to reach the {1} service. Are you connected to the internet?", Environment.NewLine, CstrDisplayName);
				MessageBox.Show(error, EncConverters.cstrCaption);
			}
			return (null, null, null, null);
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
								: CallDeepLTranslator(strInput).Result;

			StringToProperByteStar(strOutput, lpOutBuffer, ref rnOutLen);
		}

		private async Task<string> CallDeepLTranslator(string strInput)
		{
			try
			{
				var options = (FormalityLevel != Formality.Default) ? new TextTranslateOptions { Formality = FormalityLevel } : null;
				var translatedText = await Task.Run(async delegate
				{
					return await DeepLTranslator.TranslateTextAsync(strInput, FromLanguage, ToLanguage, options);
				}).ConfigureAwait(false);

				return translatedText.Text;
			}
			catch (Exception ex)
			{
				return LogExceptionMessage(GetType().Name, ex);
			}
		}

#endregion Abstract Base Class Overrides

#region Misc helpers

		protected override string GetConfigTypeName
		{
			get { return typeof(DeepLTranslatorEncConverterConfig).AssemblyQualifiedName; }
		}

#endregion Misc helpers
	}

	public class Http2CustomHandler : WinHttpHandler
	{
		protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, System.Threading.CancellationToken cancellationToken)
		{
			request.Version = new Version("2.0");
			return base.SendAsync(request, cancellationToken);
		}
	}
}
