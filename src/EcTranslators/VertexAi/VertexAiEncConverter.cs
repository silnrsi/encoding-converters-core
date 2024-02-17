// #define encryptingNewCredentials

using System;
using System.IO;
using System.Runtime.InteropServices;
using System.Diagnostics;
using System.Text;
using System.Text.RegularExpressions;
using Microsoft.Win32;                  // for RegistryKey
using ECInterfaces;                     // for IEncConverter
using SilEncConverters40.EcTranslators.BingTranslator;
using System.Windows.Forms;
using System.Reflection;
using Google.Apis.Discovery;

namespace SilEncConverters40.EcTranslators.VertexAi
{
	/// <summary>
	/// Access to chat-bison or eventually gemini-pro via Google Cloud's Vertext AI
	/// </summary>
#if X64
	[GuidAttribute("06907915-F41E-48BA-BF47-49F1E3CE584D")]
#else
	[GuidAttribute("8B8FC565-9E65-44F7-B2C4-A2C11E54AD86")]
#endif
	public class VertexAiEncConverter : PromptExeTranslator
	{
		#region Member Variable Definitions
		protected string VertexAiSystemPrompt;

		public const string strDisplayName = "Vertex AI Translator";
		const string SystemPromptFormat = "You will be given 1 or more lines of text in {0} which you are to translate into {1}{2} and return only the translated lines.";
		public static readonly string strHtmlFilename = "Vertex_AI_Translate_Plug-in_About_box.htm";
		public const string strExeDefPath = "VertexAiExe";
		public const string ImplTypeSilVertexAi = "SIL.VertexAi";
		public const string ReplacementSystemPrompt = "UseSystemPrompt: ";      // to allow the user to specify the entire system prompt (rather than just the additional bit beyond 'SystemPromptFormat' above

		public const string EnvVarNameProjectId = "EncConverters_VertexAiProjectId";    // e.g. bright-coyote-381812
		public const string EnvVarNameLocationId = "EncConverters_VertexAiLocationId";  // e.g. us-central1
		public const string EnvVarNamePublisher = "EncConverters_VertexAiPublisher";    // e.g. google
		public const string EnvVarNameModelId = "EncConverters_VertexAiModelId";        // e.g. chat-bison-32k, chat-bison, gemini-pro, etc.

		#endregion Member Variable Definitions

		#region Initialization
		// by putting the google cloud vertex AI credentials in a settings file, users can get their own credentials to use, 
		//  and enter it thru the UI if our key runs out of free stuff
		// see https://console.cloud.google.com/apis/credentials
		public static string GoogleCloudVertexAiSubscriptionKey
		{
			get
			{
				var overrideKey = Properties.Settings.Default.GoogleCloudVertexAiCredentialsOverride;
				var key = (!String.IsNullOrEmpty(overrideKey))
							? overrideKey
							: Properties.Settings.Default.GoogleCloudVertexAiCredentials;

#if encryptingNewCredentials
                var translatorKey = EncryptionClass.Encrypt(key);
#endif
				return String.IsNullOrEmpty(key) ? key : EncryptionClass.Decrypt(key);  // it may be null if the user already has the proper env var
			}
			set
			{
				// the value is already encrypted by the time it gets here
				var credentials = String.IsNullOrEmpty(value) ? value : EncryptionClass.Encrypt(value);
				Properties.Settings.Default.GoogleCloudVertexAiCredentialsOverride = credentials;
			}
		}

		/// <summary>
		/// The class constructor. </summary>
		public VertexAiEncConverter()
			: base
			(
				typeof(VertexAiEncConverter).FullName,
				ImplTypeSilVertexAi
			)
		{
		}

		public override void Initialize(
			string converterName,
			string converterSpec,
			ref string lhsEncodingID,
			ref string rhsEncodingID,
			ref ConvType conversionType,
			ref Int32 processTypeFlags,
			Int32 codePageInput,
			Int32 codePageOutput,
			bool bAdding)
		{
			// let the base class have first stab at it
			base.Initialize(converterName, converterSpec, ref lhsEncodingID, ref rhsEncodingID,
				ref conversionType, ref processTypeFlags, codePageInput, codePageOutput, bAdding);

			ParseConverterIdentifier(converterSpec, out string fromLanguage, out string toLanguage,
									 out string projectId, out string locationId, out string publisher, out string modelId,
									 out string addlInstructions, out string systemPrompt);

			VertexAiSystemPrompt = systemPrompt;

			// this is the only one we support from now on (if the user really wants to do legacy to unicode, they have to deal with the legacy as coming in utf-8 format
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
		}

		internal static bool ParseConverterIdentifier(string converterSpec,
			out string fromLanguage, out string toLanguage,
			out string projectId, out string locationId, out string publisher, out string modelId,
			out string addlInstructions, out string systemPrompt)
		{
			var strs = converterSpec.Split(new[] { ';' });
			if (strs.Length < 6)
				throw new ApplicationException($"{strDisplayName} not properly configured! converterSpec: {converterSpec} must have at least a source and target language, followed by GoogleCloud Vertex AI settings: 'projectId', 'locationId' (e.g. us-central1), 'publisher' (e.g. google), and modelId (eg. chat-bison, chat-bison-32k, gemini-pro)");

			fromLanguage = strs[0];
			toLanguage = strs[1];
			projectId = strs[2];
			locationId = strs[3];
			publisher = strs[4];
			modelId = strs[5];

			addlInstructions = (strs.Length == 7)
										? strs[6]
										: String.Empty;

			systemPrompt = GetSystemPrompt(fromLanguage, toLanguage, addlInstructions);

			return true;
		}

		public static string GetSystemPrompt(string fromLanguage, string toLanguage, string addlInstructions)
		{
			return (addlInstructions.StartsWith(ReplacementSystemPrompt))
								? SubstituteSystemPrompt(addlInstructions)
								: FormatSystemPrompt(fromLanguage, toLanguage, addlInstructions);
		}

		public static string SubstituteSystemPrompt(string addlInstructions)
		{
			return addlInstructions.Substring(ReplacementSystemPrompt.Length).Replace(";", null);
		}

		public static string FormatSystemPrompt(string fromLanguage, string toLanguage, string addlInstructions)
		{
			// can't have double quotes in it, bkz we pass it as a command line parameter surrounded by double quotes
			return String.Format(SystemPromptFormat, fromLanguage, toLanguage, $" {addlInstructions.Replace(";", null)}");
		}

		#endregion Initialization

		public override string ExeName
		{
			get
			{
				// the requirement is that this DLL (i.e. EcTranslators.dll) is in the same folder as the VertexAiExe.exe
				//    console app (bkz this EncConverter could be being launched by Paratext, which could even be using a different
				//    version of the core SilEncConverters40.dll--in its install folder), and which wouldn't have this
				//    EcTranslators DLL, OR by Word, which knows nothing about either and launches them via COM).
				// So to get the path where the VertexAiExe is located, it should be the same location as this DLL
				//    (e.g. the SILConverters install dir)
				var pathToDll = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
				return Path.Combine(pathToDll, $"{strExeDefPath}.exe");
			}
		}

		public override string Arguments
		{
			get
			{
				ParseConverterIdentifier(ConverterIdentifier, out string fromLanguage, out string toLanguage,
										 out string projectId, out string locationId, out string publisher, out string modelId,
										 out string addlInstructions, out string systemPrompt);

				var args = new VertexAiPromptExeTranslatorCommandLineArgs
				{
					ProjectId = projectId,
					LocationId = locationId,
					Publisher = publisher,
					ModelId = modelId,
					SystemPrompt = VertexAiSystemPrompt,
					Credentials = GoogleCloudVertexAiSubscriptionKey,
					ExamplesInputString = ExamplesInputString,
					ExamplesOutputString = ExamplesOutputString,
				};

				// The system prompt can't have double quotes in it, bkz those are used for separating the 4 command line parameters,
				//    So convert them to single quotes, which should also work
				// return $"\"{projectId}\" \"{locationId}\" \"{publisher}\" \"{modelId}\" \"{VertexAiSystemPrompt.Replace("\"", "'")}\" \"{GoogleCloudVertexAiSubscriptionKey}\"";
				var tempFilespec = args.SaveToTempFile();
				var arguments = $"\"{tempFilespec}\"";
				return arguments;
			}
		}

		private static bool IsValidParameter(string envVarName, ref string parameter)
		{
			return !string.IsNullOrEmpty(parameter) ||
					!string.IsNullOrEmpty((parameter = Environment.GetEnvironmentVariable(envVarName)));
		}

		#region Misc helpers

		protected override EncodingForm DefaultUnicodeEncForm(bool bForward, bool bLHS)
		{
			// if it's unspecified, then we want UTF-16
			return EncodingForm.UTF16;
		}

		#endregion Misc helpers

		#region Abstract Base Class Overrides
		protected override string GetConfigTypeName
		{
			get { return typeof(VertexAiEncConverterConfig).AssemblyQualifiedName; }
		}

		protected override Encoding StandardOutputEncoding
		{
			get
			{
				return Encoding.Unicode;
			}
		}

		protected override Encoding StandardInputEncoding
		{
			get
			{
				return Encoding.Unicode;
			}
		}

		#endregion Abstract Base Class Overrides
	}
}

