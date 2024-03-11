//
// PerlExpressionEncConverter.cs
//
// Created by Jim Kornelsen on Nov 21 2011
//
// 28-Nov-11 JDK  Wrap Perl expression and write in temp file, rather than requiring input file.
// 09-Jan-12 JDK  eInFormEngine should be UTF8, but eOutFormEngine UTF16 because it is a C# string.
//
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

namespace SilEncConverters40.EcTranslators.AzureOpenAI
{
    /// <summary>
    /// Access to ChatGPT 3.5 via Azure's OpenAI
    /// </summary>
#if X64
    [GuidAttribute("194C96AC-60D9-4456-8B57-A522B5CB1ED2")]
#else
    [GuidAttribute("74EC9437-A9F1-44B3-BA70-5A8055E210D9")]
#endif
    public class AzureOpenAiEncConverter : PromptExeTranslator
    {
        #region Member Variable Definitions
        protected string AiSystemInstructions;

        public const string strDisplayName = "Azure OpenAI Translator";
        const string SystemPromptFormat = "You will be given 1 or more lines of text in {0} which you are to translate into {1}{2} and return only the translated lines.";
        public static readonly string strHtmlFilename = "Azure_OpenAI_Translate_Plug-in_About_box.htm";
        public const string strExeDefPath = "AzureOpenAiExe";
        public const string ImplTypeSilAzureOpenAi = "SIL.AzureOpenAI";
        public const string ReplacementSystemPrompt = "UseSystemPrompt: ";

        public const string EnvVarNameDeploymentName = "EncConverters_AzureOpenAiDeploymentName";
        public const string EnvVarNameEndPoint = "EncConverters_AzureOpenAiEndpoint";
        public const string EnvVarNameKey = "EncConverters_AzureOpenAiKey";

        #endregion Member Variable Definitions

        #region Initialization
        // by putting the azure key in a settings file, users can (someday) get their own azure OpenAI resource 
        //  and enter their own key in the settings file(s) (or the UI to have us set it in the file) to have
        //    access to it
        public static string AzureOpenAiEndpoint
        {
            get
            {
                var endpoint = Properties.Settings.Default.AzureOpenAiEndpoint;
                return !String.IsNullOrEmpty(endpoint)
                        ? endpoint
                        : Environment.GetEnvironmentVariable(AzureOpenAiEncConverter.EnvVarNameEndPoint);
            }
            set
            {
                Properties.Settings.Default.AzureOpenAiEndpoint = value;
            }
        }

        public static string AzureOpenAiDeploymentName
        {
            get
            {
                var deploymentName = Properties.Settings.Default.AzureOpenAiDeploymentName;
                return !String.IsNullOrEmpty(deploymentName)
                        ? deploymentName
                        : Environment.GetEnvironmentVariable(AzureOpenAiEncConverter.EnvVarNameDeploymentName);
            }
            set
            {
                // the value is already encrypted by the time it gets here
                Properties.Settings.Default.AzureOpenAiDeploymentName = value;
            }
        }

        public static string AzureOpenAiKeyOverride
        {
            get
            {
                var overrideKey = Properties.Settings.Default.AzureOpenAiKeyOverride;

#if encryptingNewCredentials
                var translatorKey = EncryptionClass.Encrypt(overrideKey);
#endif

                // decrypt it if we're storing it
                overrideKey = String.IsNullOrEmpty(overrideKey) ? overrideKey : EncryptionClass.Decrypt(overrideKey);

                return !String.IsNullOrEmpty(overrideKey)
                        ? overrideKey
                        : Environment.GetEnvironmentVariable(EnvVarNameKey);
            }
            set
            {
                // the value is already encrypted by the time it gets here
                var key = String.IsNullOrEmpty(value) ? value : EncryptionClass.Encrypt(value);
                Properties.Settings.Default.AzureOpenAiKeyOverride = key;
            }
        }

        /// <summary>
        /// The class constructor. </summary>
        public AzureOpenAiEncConverter()
            : base
            (
                typeof(AzureOpenAiEncConverter).FullName,
                ImplTypeSilAzureOpenAi
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
                ref conversionType, ref processTypeFlags, codePageInput, codePageOutput, bAdding );

            ParseConverterIdentifier(converterSpec, out string fromLanguage, out string toLanguage,
                                     out string addlInstructions, out string systemPrompt);

            AiSystemInstructions = systemPrompt;

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
            out string addlInstructions, out string systemPrompt)
        {
            var strs = converterSpec.Split(new[] { ';' });
            if (strs.Length < 2)
                throw new ApplicationException($"{strDisplayName} not properly configured! converterSpec: {converterSpec} must have at least a source and target language (eg. Hindi;English)");

            fromLanguage = strs[0];
            toLanguage = strs[1];
            addlInstructions = (strs.Length == 3)
                                        ? strs[2]
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
                // the requirement is that this DLL (i.e. EcTranslators.dll) is in the same folder as the AzureOpenAiExe.exe
                //    console app (bkz this EncConverter could be being launched by Paratext, which could even be using a different
                //    version of the core SilEncConverters40.dll--in its install folder), and which wouldn't have this
                //    EcTranslators DLL, OR by Word, which knows nothing about either and launches them via COM).
                // So to get the path where the AzureOpenAiExe is located, it should be the same location as this DLL
                //    (e.g. the SILConverters install dir)
                var pathToDll = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
                return Path.Combine(pathToDll, $"{strExeDefPath}.exe");
            }
        }

        public override string Arguments
        {
            get
            {
                var azureOpenAiKey = AzureOpenAiKeyOverride;
                var azureOpenAiDeploymentName = AzureOpenAiDeploymentName;
                var azureOpenAiEndpoint = AzureOpenAiEndpoint;
                if (!IsValidParameter(EnvVarNameKey, ref azureOpenAiKey) ||
                    !IsValidParameter(EnvVarNameDeploymentName, ref azureOpenAiDeploymentName) ||
                    !IsValidParameter(EnvVarNameEndPoint, ref azureOpenAiEndpoint))
                {
                    MessageBox.Show(String.Format("You need to edit this converter instance and enter the key to your Azure Open AI resource in the Setup tab.{0}You may need to do this for each Client app, since they store their Settings separately.{0}You can also use environment variables to set these values:{0}{1}{0}{2}{0}{3}",
                                                  Environment.NewLine, EnvVarNameKey, EnvVarNameDeploymentName, EnvVarNameEndPoint),
                                    EncConverters.cstrCaption);
                }

                var args = new AzureOpenAiPromptExeTranslatorCommandLineArgs
                {
                    DeploymentId = azureOpenAiDeploymentName,
                    EndpointId = azureOpenAiEndpoint,
                    SystemPrompt = AiSystemInstructions,
                    Credentials = azureOpenAiKey,
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

        protected override EncodingForm  DefaultUnicodeEncForm(bool bForward, bool bLHS)
        {
            // if it's unspecified, then we want UTF-16
            return EncodingForm.UTF16;
        }

        #endregion Misc helpers

        #region Abstract Base Class Overrides
        protected override string   GetConfigTypeName
        {
            get { return typeof(AzureOpenAiEncConverterConfig).AssemblyQualifiedName; }
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
