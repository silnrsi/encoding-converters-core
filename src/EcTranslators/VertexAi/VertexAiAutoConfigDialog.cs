// #define encryptingNewCredentials

using System;
using System.Windows.Forms;
using ECInterfaces;                     // for IEncConverter
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using static SilEncConverters40.EcTranslators.VertexAi.VertexAiEncConverter;
using System.Net;
using SilEncConverters40.EcTranslators.Properties;
using SilEncConverters40.EcTranslators.GoogleTranslator;

namespace SilEncConverters40.EcTranslators.VertexAi
{
    public partial class VertexAiAutoConfigDialog : AutoConfigDialog
    {
        private const string SourceLanguageNameMustBeConfigured = "<Type in the language you want to translate from>";
        private const string TargetLanguageNameMustBeConfigured = "<Type in the language you want to translate to>";
        private readonly string ResourceNeededWarning = $"The {VertexAiEncConverter.strDisplayName} requires an Google Cloud Vertex AI resource.";

        public VertexAiAutoConfigDialog
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
										 out string projectId, out string locationId, out string publisher, out string modelId,
										 out string addlInstructions, out string systemPrompt);

				textBoxProjectId.Text = projectId;
				textBoxLocationId.Text = locationId;
				textBoxPublisher.Text = publisher;
				textBoxModelId.Text = modelId;
                textBoxCompleteSystemPrompt.Text = systemPrompt;
                if (!String.IsNullOrEmpty(addlInstructions))
                    comboBoxSystemPromptAdditions.Text = addlInstructions;
                IsModified = false;
            }
            else
            {
                textBoxCompleteSystemPrompt.Text = ResourceNeededWarning;
				textBoxProjectId.Text = Settings.Default.GoogleCloudVertexAiProjectId;
				textBoxLocationId.Text = Settings.Default.GoogleCloudVertexAiLocationId;
				textBoxPublisher.Text = Settings.Default.GoogleCloudVertexAiPublisher;
				textBoxModelId.Text = Settings.Default.GoogleCloudVertexAiModelId;
            }

            textBoxSourceLanguage.Text = fromLanguage;
            textBoxTargetLanguage.Text = toLanguage;
                LoadComboBoxFromSettings(comboBoxSystemPromptAdditions, Settings.Default.AzureOpenAiSystemPromptAdditions);

            m_bInitialized = true;

			helpProvider.SetHelpString(textBoxProjectId, Resources.HelpForVertexAiProjectIdTextBox);
			helpProvider.SetHelpString(textBoxLocationId, Resources.HelpForVertexAiLocationIdTextBox);
			helpProvider.SetHelpString(textBoxPublisher, Resources.HelpForVertexAiPublisherTextBox);
			helpProvider.SetHelpString(textBoxModelId, Resources.HelpForVertexAiModelIdTextBox);
            helpProvider.SetHelpString(textBoxSourceLanguage, Properties.Resources.HelpForVertexAiSourceLanguageTextBox);
            helpProvider.SetHelpString(textBoxTargetLanguage, Properties.Resources.HelpForVertexAiTargetLanguageTextBox);
            helpProvider.SetHelpString(textBoxCompleteSystemPrompt, Properties.Resources.HelpForVertexAiSystemPromptTextBox);
            helpProvider.SetHelpString(comboBoxSystemPromptAdditions, Properties.Resources.HelpForVertexAiSystemPromptAdditionsComboBox);
            helpProvider.SetHelpString(buttonSetVertexAiApiKey, Properties.Resources.HelpForVertexAiAddYourOwnApiKey);

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

        public VertexAiAutoConfigDialog
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

            var additionToSystemPrompt = comboBoxSystemPromptAdditions.Text?.Trim().Replace(";", null);
            if (!String.IsNullOrEmpty(additionToSystemPrompt) && !Settings.Default.AzureOpenAiSystemPromptAdditions.Contains(additionToSystemPrompt))
            {
                Settings.Default.AzureOpenAiSystemPromptAdditions.Insert(0, additionToSystemPrompt);
                Settings.Default.Save();
            }

			var projectId = textBoxProjectId.Text;
			var locationId = textBoxLocationId.Text;
			var publisher = textBoxPublisher.Text;
			var modelId = textBoxModelId.Text;

			if (!IsValidUserInput(null, ref projectId))
			{
				MessageBox.Show(this, $"The Vertex AI resource Project Id must be entered and cannot contain a ';' character! See {Resources.GoogleCloudVertexCredentialsDialogInstructionUrl} for help on creating one.", EncConverters.cstrCaption);
				return false;
			}
			Settings.Default.GoogleCloudVertexAiProjectId = projectId;

			if (!IsValidUserInput(null, ref locationId))
			{
				MessageBox.Show(this, $"The Vertex AI resource Location Id (e.g. us-central1) must be entered and cannot contain a ';' character! See {Resources.GoogleCloudVertexCredentialsDialogInstructionUrl} for help on creating one.", EncConverters.cstrCaption);
				return false;
			}
			Settings.Default.GoogleCloudVertexAiLocationId = locationId;

			if (!IsValidUserInput(null, ref publisher))
			{
				MessageBox.Show(this, $"The Vertex AI resource Publisher (e.g. google) must be entered and cannot contain a ';' character! See {Resources.GoogleCloudVertexCredentialsDialogInstructionUrl} for help on creating one.", EncConverters.cstrCaption);
				return false;
			}
			Settings.Default.GoogleCloudVertexAiPublisher = publisher;

			if (!IsValidUserInput(null, ref modelId))
			{
				MessageBox.Show(this, $"The Vertex AI resource Model Id (e.g. chat-bison, chat-bison-32k, or gemini-pro) must be entered and cannot contain a ';' character! See {Resources.GoogleCloudVertexCredentialsDialogInstructionUrl} for help on creating one.", EncConverters.cstrCaption);
				return false;
			}
			Settings.Default.GoogleCloudVertexAiModelId = modelId;
			Settings.Default.Save();

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
            ConverterIdentifier = String.Format("{0};{1};{2};{3};{4};{5};{6}",
                selectedFromLanguage,
                selectedToLanguage,
				projectId,
				locationId,
				publisher,
				modelId,
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
                return typeof(VertexAiEncConverter).FullName; 
            }
        }

        protected override string ImplType
        {
            get
            {
                return EncConverters.strTypeSILVertexAiTranslator;
            }
        }

        protected override string DefaultFriendlyName
        {
            // as the default, make it the same as the table name (w/o extension)
            get
            {
                var additionToSystemPrompt = comboBoxSystemPromptAdditions.Text?.Trim().Replace(";", null);
                if (additionToSystemPrompt.StartsWith(ReplacementSystemPrompt))
                    return $"Vertex AI {SubstituteSystemPrompt(additionToSystemPrompt)}";

                var selectedFromLanguage = textBoxSourceLanguage.Text.Trim();
                if (selectedFromLanguage == SourceLanguageNameMustBeConfigured)
                    selectedFromLanguage = null;
                var selectedToLanguage = textBoxTargetLanguage.Text.Trim();
                if (selectedToLanguage == TargetLanguageNameMustBeConfigured)
                    selectedToLanguage = null;
                return $"Vertex AI Translate {selectedFromLanguage} to {selectedToLanguage}";
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
            var systemPrompt = VertexAiEncConverter.GetSystemPrompt(textBoxSourceLanguage.Text?.Trim(), textBoxTargetLanguage.Text?.Trim(),
                                               comboBoxSystemPromptAdditions.Text?.Trim());
            textBoxCompleteSystemPrompt.Text = systemPrompt;
        }

        private void ButtonSetVertexAiKey_Click(object sender, EventArgs e)
        {
#if encryptingNewCredentials
			var googleTranslatorKeyHide = Properties.Settings.Default.GoogleTranslatorCredentials;
			var credentials = EncryptionClass.Encrypt(googleTranslatorKeyHide);
#endif
			// only send the key if it's already the override key (so we don't expose ours)
			var vertexAiCredentialsOverride = Properties.Settings.Default.GoogleCloudVertexAiCredentialsOverride;
			if (!String.IsNullOrEmpty(vertexAiCredentialsOverride))
				vertexAiCredentialsOverride = EncryptionClass.Decrypt(vertexAiCredentialsOverride);

			using var dlg = new QueryForGoogleCredentials(vertexAiCredentialsOverride);
			if (dlg.ShowDialog() == DialogResult.OK)
			{
				GoogleCloudVertexAiSubscriptionKey = dlg.TranslatorKey;
				Properties.Settings.Default.Save();
			}
		}

		private void TextBoxVertexResourceParameters_TextChanged(object sender, EventArgs e)
		{
			IsModified = true;
		}
	}
}

