// #define encryptingNewCredentials

using System;
using System.Windows.Forms;
using ECInterfaces;                     // for IEncConverter
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using static SilEncConverters40.EcTranslators.AzureOpenAI.AzureOpenAiEncConverter;
using System.Net;
using SilEncConverters40.EcTranslators.Properties;

namespace SilEncConverters40.EcTranslators.AzureOpenAI
{
    public partial class AzureOpenAiAutoConfigDialog : AutoConfigDialog
    {
        private const string SourceLanguageNameMustBeConfigured = "<Type in the language you want to translate from>";
        private const string TargetLanguageNameMustBeConfigured = "<Type in the language you want to translate to>";
        private readonly string ResourceNeededWarning = $"The {AzureOpenAiEncConverter.strDisplayName} requires an Azure OpenAI resource.";

        public AzureOpenAiAutoConfigDialog
            (
            IEncConverters aECs,
            string strDisplayName,
            string strFriendlyName,
            string strConverterIdentifier,
            ConvType eConversionType,
            string strLhsEncodingId,
            string strRhsEncodingId,
            int lProcessTypeFlags,
            bool bIsInRepository
            )
        {
            Util.DebugWriteLine(this, "(1) BEGIN");
            InitializeComponent();
            Util.DebugWriteLine(this, "initialized component");
            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;

            base.Initialize
            (
            aECs,
            strHtmlFilename,
            strDisplayName,
            strFriendlyName,
            strConverterIdentifier,
            eConversionType,
            strLhsEncodingId,
            strRhsEncodingId,
            lProcessTypeFlags,
            bIsInRepository
            );
            Util.DebugWriteLine(this, "called base.Initalize");

            string fromLanguage = SourceLanguageNameMustBeConfigured, toLanguage = TargetLanguageNameMustBeConfigured;

            // if we're editing converter, then set the Converter Spec and say it's unmodified
            if (m_bEditMode)
            {
                System.Diagnostics.Debug.Assert(!String.IsNullOrEmpty(ConverterIdentifier));

                ParseConverterIdentifier(ConverterIdentifier, out fromLanguage, out toLanguage,
                                         out string addlInstructions, out string systemPrompt);

                textBoxCompleteSystemPrompt.Text = systemPrompt;
                if (!String.IsNullOrEmpty(addlInstructions))
                    comboBoxSystemPromptAdditions.Text = addlInstructions;
                IsModified = false;
            }
            else
            {
                textBoxCompleteSystemPrompt.Text = ResourceNeededWarning;
            }

            textBoxSourceLanguage.Text = fromLanguage;
            textBoxTargetLanguage.Text = toLanguage;
                LoadComboBoxFromSettings(comboBoxSystemPromptAdditions, Settings.Default.AzureOpenAiSystemPromptAdditions);

            m_bInitialized = true;

            helpProvider.SetHelpString(textBoxSourceLanguage, Properties.Resources.HelpForAzureOpenAiSourceLanguageTextBox);
            helpProvider.SetHelpString(textBoxTargetLanguage, Properties.Resources.HelpForAzureOpenAiTargetLanguageTextBox);
            helpProvider.SetHelpString(textBoxCompleteSystemPrompt, Properties.Resources.HelpForAzureOpenAiSystemPromptTextBox);
            helpProvider.SetHelpString(comboBoxSystemPromptAdditions, Properties.Resources.HelpForAzureOpenAiSystemPromptAdditionsComboBox);
            helpProvider.SetHelpString(buttonSetAzureOpenAiApiKey, Properties.Resources.HelpForAzureOpenAiAddYourOwnApiKey);

            Util.DebugWriteLine(this, "END");
        }

        private void InitializeComboBoxFromCode(ComboBox comboBox, string code)
        {
            if (String.IsNullOrEmpty(code))
                return;

            string value = $"({code})";
            var item = comboBox.Items.Cast<string>().FirstOrDefault(i => i.Contains(value));

            if ((item == null) && (comboBox.Items.Count > 0))
            {
                item = (string)comboBox.Items[0];
            }

            if (item != null)
                comboBox.SelectedItem = item;
        }

        public AzureOpenAiAutoConfigDialog
            (
            IEncConverters aECs,
            string strFriendlyName,
            string strConverterIdentifier,
            ConvType eConversionType,
            string strTestData
            )
        {
            Util.DebugWriteLine(this, "(2) BEGIN");
            InitializeComponent();

            base.Initialize
            (
            aECs,
            strFriendlyName,
            strConverterIdentifier,
            eConversionType,
            strTestData
            );
            Util.DebugWriteLine(this, "END");
        }

        // this method is called either when the user clicks the "Apply" or "OK" buttons *OR* if she
        //  tries to switch to the Test or Advanced tab. This is the dialog's one opportunity
        //  to make sure that the user has correctly configured a legitimate converter.
        protected override bool OnApply()
        {
            if (!IsModified)
                return true;

            var azureOpenAiResourceKey = AzureOpenAiKeyOverride;
            var azureOpenAiEndpoint = AzureOpenAiEndpoint;
            var azureOpenAiDeploymentName = AzureOpenAiDeploymentName;

            if (String.IsNullOrEmpty(azureOpenAiResourceKey) || String.IsNullOrEmpty(azureOpenAiEndpoint) || String.IsNullOrEmpty(azureOpenAiDeploymentName))
            {
                MessageBox.Show(this, $"Click the '{buttonSetAzureOpenAiApiKey.Text}' button to enter your Azure Open AI Resource information", EncConverters.cstrCaption);
                return false;
            }

            var additionToSystemPrompt = comboBoxSystemPromptAdditions.Text?.Trim().Replace(";", null);
            if (!String.IsNullOrEmpty(additionToSystemPrompt) && !Settings.Default.AzureOpenAiSystemPromptAdditions.Contains(additionToSystemPrompt))
            {
                Settings.Default.AzureOpenAiSystemPromptAdditions.Insert(0, additionToSystemPrompt);
                Settings.Default.Save();
                LoadComboBoxFromSettings(comboBoxSystemPromptAdditions, Settings.Default.AzureOpenAiSystemPromptAdditions);
            }

            var selectedFromLanguage = textBoxSourceLanguage.Text;
            var selectedToLanguage = textBoxTargetLanguage.Text;
            if (!additionToSystemPrompt.StartsWith(ReplacementSystemPrompt))
            {
                if (!IsValidUserInput(SourceLanguageNameMustBeConfigured, ref selectedFromLanguage))
                {
                    MessageBox.Show(this, "A Source language name (e.g. Hindi) must be entered and cannot contain a ';' character!", EncConverters.cstrCaption);
                    return false;
                }

                if (!IsValidUserInput(TargetLanguageNameMustBeConfigured, ref selectedToLanguage))
                {
                    MessageBox.Show(this, "A Target Language (e.g. English) must be entered and cannot contain a ';' character!", EncConverters.cstrCaption);
                    return false;
                }
            }
            else
            {
                selectedFromLanguage = null;
                selectedToLanguage = null;
            }

            // for Azure Open AI, get the converter identifier from the Setup tab controls.
            //  e.g. "Hindi;English;with a 'free translation' style aimed at high school students"
            ConverterIdentifier = String.Format("{0};{1};{2}",
                selectedFromLanguage,
                selectedToLanguage,
                additionToSystemPrompt);

            return base.OnApply();

            static bool IsValidUserInput(string defaultValue, ref string userInput)
            {
                userInput = userInput?.Trim();
                return (defaultValue != userInput) && !String.IsNullOrEmpty(userInput) && !userInput.Contains(";");
            }
        }

        protected override string ProgID
        {
            get
            {
                return typeof(AzureOpenAiEncConverter).FullName; 
            }
        }

        protected override string ImplType
        {
            get
            {
                return EncConverters.strTypeSILAzureOpenAiTranslator;
            }
        }

        protected override string DefaultFriendlyName
        {
            // as the default, make it the same as the table name (w/o extension)
            get
            {
                var additionToSystemPrompt = comboBoxSystemPromptAdditions.Text?.Trim().Replace(";", null);
                if (additionToSystemPrompt.StartsWith(ReplacementSystemPrompt))
                    return $"Azure Open AI {SubstituteSystemPrompt(additionToSystemPrompt)}";

                var selectedFromLanguage = textBoxSourceLanguage.Text.Trim();
                if (selectedFromLanguage == SourceLanguageNameMustBeConfigured)
                    selectedFromLanguage = null;
                var selectedToLanguage = textBoxTargetLanguage.Text.Trim();
                if (selectedToLanguage == TargetLanguageNameMustBeConfigured)
                    selectedToLanguage = null;
                return $"Azure Open AI Translate {selectedFromLanguage} to {selectedToLanguage}";
            }
        }

        private void textBox_TextChanged(object sender, EventArgs e)
        {
            if (m_bInitialized) // but only do this after we've already initialized (we might have set it during m_bEditMode)
                IsModified = true;
            UpdateSystemPrompt();
        }

        private void comboBoxSystemPromptAdditions_TextChanged(object sender, EventArgs e)
        {
            if (m_bInitialized)
                IsModified = true;
            UpdateSystemPrompt();
        }

        private void UpdateSystemPrompt()
        {
            var systemPrompt = AzureOpenAiEncConverter.GetSystemPrompt(textBoxSourceLanguage.Text?.Trim(), textBoxTargetLanguage.Text?.Trim(),
                                               comboBoxSystemPromptAdditions.Text?.Trim());
            textBoxCompleteSystemPrompt.Text = systemPrompt;
        }

        private void ButtonSetAzureOpenAiKey_Click(object sender, EventArgs e)
        {
#if encryptingNewCredentials
            var azureOpenAiKeyHide = "5bc...";
            var clientId = EncryptionClass.Encrypt(azureOpenAiKeyHide);
#endif

            using var dlg = new QueryForAzureKeyDeploymentNameAndEndpoint(AzureOpenAiKeyOverride, AzureOpenAiDeploymentName,
                                                                          AzureOpenAiEndpoint);
            if (dlg.ShowDialog() == DialogResult.OK)
            {
                AzureOpenAiKeyOverride = dlg.AzureOpenAiKeyOverride;
                AzureOpenAiDeploymentName = dlg.AzureOpenAiDeploymentName;
                AzureOpenAiEndpoint = dlg.AzureOpenAiEndpoint;
                Properties.Settings.Default.Save();
            }
        }
    }
}

