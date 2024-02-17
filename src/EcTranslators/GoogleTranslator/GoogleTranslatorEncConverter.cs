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
using Google.Apis.Auth.OAuth2;
using Google.Cloud.Translation.V2;
using DeepL.Model;
using Microsoft.Extensions.Options;
using System.Windows.Forms;

namespace SilEncConverters40.EcTranslators.GoogleTranslator
{
	/// <summary>
	/// Managed Google Translate EncConverter.
	/// </summary>
#if X64
	[GuidAttribute("A2C80989-EBBD-4257-9006-31FADC88646F")]
#else
	[GuidAttribute("A8C89778-D507-4DF5-8AB5-220D40177636")]
#endif
	// normally these subclasses are treated as the base class (i.e. the 
	//  client can use them orthogonally as IEncConverter interface pointers
	//  so normally these individual subclasses would be invisible), but if 
	//  we add 'ComVisible = false', then it doesn't get the registry 
	//  'HKEY_CLASSES_ROOT\SilEncConverters40.EcTranslators.GoogleTranslatorEncConverter' which is the basis of 
	//  how it is started (see EncConverters.AddEx).
	// [ComVisible(false)] 
	public class GoogleTranslatorEncConverter : TranslatorConverter
	{
		#region Const Definitions
		// by putting the google translate credentials in a settings file, users can get their own credentials to use, 
		//  and enter it thru the UI if our key runs out of free stuff
		// see https://console.cloud.google.com/apis/credentials
		private static TranslationClient _translationClient;
		public static TranslationClient TranslateClient
		{
			get
			{
				if (_translationClient == null)
				{
					var googleCreds = GoogleCredential.FromJson(GoogleTranslatorSubscriptionKey);
					_translationClient = TranslationClient.Create(googleCreds);
				}
				return _translationClient;
			}
		}

		public static string GoogleTranslatorSubscriptionKey
		{
			get
			{
				var overrideKey = Properties.Settings.Default.GoogleTranslatorCredentialsOverride;
				var key = (!String.IsNullOrEmpty(overrideKey))
							? overrideKey
							: Properties.Settings.Default.GoogleTranslatorCredentials;
				return EncryptionClass.Decrypt(key);
			}
			set
			{
				// the value is already encrypted by the time it gets here
				var credentials = String.IsNullOrEmpty(value) ? value : EncryptionClass.Encrypt(value);
				Properties.Settings.Default.GoogleTranslatorCredentialsOverride = credentials;
			}
		}

		public const string CstrDisplayName = "Google Translate";
        internal const string strHtmlFilename  = "Google_Translate_Plug-in_About_box.htm";
		#endregion Const Definitions

		#region Member Variable Definitions

		protected string fromLanguage;
		protected string toLanguage;

		#endregion Member Variable Definitions

		#region Initialization
		public GoogleTranslatorEncConverter() : base(typeof(GoogleTranslatorEncConverter).FullName,EncConverters.strTypeSILGoogleTranslator)
        {
			// this is needed to be able to use the Google Translator (https call) from Word. If you don't have it, you just get this error:
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

			if (!ParseConverterIdentifier(converterSpec, ref fromLanguage, out toLanguage))
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
			ref string fromLanguage, out string toLanguage)
		{
			toLanguage = null;

			string[] astrs = converterSpec.Split(new[] { ';' });

			// gotta at least have the toLanguage
			if (astrs.Length < 1)
				return false;

			if (!String.IsNullOrEmpty(astrs[0]))
				fromLanguage = astrs[0];	// don't change it (from Auto-Detect) if it was saved as empty space

			toLanguage = astrs[1];
			return true;
		}

#pragma warning disable CS3002 // Return type is not CLS-compliant
		public async static Task<List<Google.Cloud.Translation.V2.Language>> GetCapabilities()
#pragma warning restore CS3002 // Return type is not CLS-compliant
		{
			try
			{
				var resultLanguagesSupported = await Task.Run(async delegate
				{
					return (await TranslateClient.ListLanguagesAsync(LanguageCodes.English)).ToList();
				}).ConfigureAwait(false);

				return resultLanguagesSupported;
			}
			catch (Exception ex)
			{
				var error = LogExceptionMessage($"{typeof(GoogleTranslatorEncConverter).Name}.GetCapabilities", ex);
				if (error.Contains("The remote name could not be resolved"))
					error += String.Format("{0}{0}Unable to reach the {1} service. Are you connected to the internet?", Environment.NewLine, CstrDisplayName);
				MessageBox.Show(error, EncConverters.cstrCaption);
			}
			return null;
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
								: CallGoogleTranslator(strInput).Result;

			StringToProperByteStar(strOutput, lpOutBuffer, ref rnOutLen);
		}

		private async Task<string> CallGoogleTranslator(string strInput)
		{
			try
			{
				var result = await Task.Run(async delegate
				{
					return await TranslateClient.TranslateTextAsync(strInput, toLanguage, fromLanguage);
				}).ConfigureAwait(false);

				return result.TranslatedText;
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
			get { return typeof(GoogleTranslatorEncConverterConfig).AssemblyQualifiedName; }
		}

		#endregion Misc helpers
	}
}
