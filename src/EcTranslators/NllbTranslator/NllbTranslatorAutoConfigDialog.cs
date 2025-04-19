#define DisableBilling

using System;
using System.Windows.Forms;
using ECInterfaces;                     // for IEncConverter
using static SilEncConverters40.EcTranslators.NllbTranslator.NllbTranslatorEncConverter;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.IO;
using static System.Environment;

namespace SilEncConverters40.EcTranslators.NllbTranslator
{
    public partial class NllbTranslatorAutoConfigDialog : AutoConfigDialog
    {
        private readonly ComboBoxItem SourceLanguageNameMustBeConfigured = new ComboBoxItem { Display = "Select Source Language" };
        private readonly ComboBoxItem TargetLanguageNameMustBeConfigured = new ComboBoxItem { Display = "Select Target Language" };
        private string ModelNameSuffix = String.Empty;	// so we can add it to the friendly name -- but only works if the user edits (which they should do, but...)

        public NllbTranslatorAutoConfigDialog
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

            // if we're editing converter, then set the Converter Spec and say it's unmodified
            if (m_bEditMode)
            {
                System.Diagnostics.Debug.Assert(!String.IsNullOrEmpty(ConverterIdentifier));

                ParseConverterIdentifier(ConverterIdentifier, out string pathToDockerProject, out string fromLanguage, out string toLanguage,
                                         out string apiKey, out string endpoint);

                var languagesSupported = GetLanguagesSupportedAndInitializeComboBoxes(true, apiKey, endpoint);
                if (languagesSupported == null)
                    return;

                DockerProjectFolderPath = pathToDockerProject;
                var selectedItem = comboBoxSourceLanguages.Items.Cast<ComboBoxItem>().FirstOrDefault(l => l.Code == fromLanguage);
                comboBoxSourceLanguages.SelectedItem = selectedItem;
                comboBoxTargetLanguages.SelectedItem = languagesSupported.FirstOrDefault(l => l.Code == toLanguage);
                IsModified = false;
            }
            else
            {
                // if we've done one before... see if it still works
                DockerProjectFolderPath = Properties.Settings.Default.NllbTranslatorPathToDockerProject;

                var apiKey = NllbTranslatorApiKey;
                var endpoint = NllbTranslatorEndpoint;
                if (!String.IsNullOrEmpty(apiKey) && !String.IsNullOrEmpty(endpoint)
                    && IsHttpServerListeningAsync(endpoint).Result)
                {
                    GetLanguagesSupportedAndInitializeComboBoxes(false, apiKey, endpoint);
                }
                else
                    buttonConfigureNllbModel.Enabled = !String.IsNullOrEmpty(DockerProjectFolderPath);    // until the path is chosen

                comboBoxSourceLanguages.SelectedItem = SourceLanguageNameMustBeConfigured;
                comboBoxTargetLanguages.SelectedItem = TargetLanguageNameMustBeConfigured;
            }

            m_bInitialized = true;

            helpProvider.SetHelpString(comboBoxSourceLanguages, Properties.Resources.HelpForNllbTranslatorSourceLanguagesComboBox);
            helpProvider.SetHelpString(comboBoxTargetLanguages, Properties.Resources.HelpForNllbTranslatorTargetLanguagesComboBox);
            helpProvider.SetHelpString(buttonConfigureNllbModel, Properties.Resources.HelpForNllbTranslatorAddYourOwnApiKey);

            Util.DebugWriteLine(this, "END");
        }

        private List<ComboBoxItem> GetLanguagesSupportedAndInitializeComboBoxes(bool showError, string apiKey, string endpoint)
        {
            // for our purposes here, we only need the Model configuration (so we can hit the endpoint for languages supported);
            //  not the specific languages we want to convert To/From. So we don't want to use 'OnApply' here, bkz it will fails
            //  so just create a temporary one and set the key/endpoint and use it to get the languages supported.
            var theNllbEncConverter = new NllbTranslatorEncConverter
            {
                ApiKey = apiKey,
                Endpoint = endpoint
            };
            var langMap = theNllbEncConverter.GetCapabilities(showError).GetAwaiter().GetResult();
            if (langMap == null)
                return null;

            var languagesSupported = langMap.Select(kvp => new ComboBoxItem { Code = kvp.Key, Display = kvp.Value })
                                            .OrderBy(c => c.Display)
                                            .ToList();
            InitializeSourceAndTargetLanguages(languagesSupported);
            return languagesSupported;
        }

        public NllbTranslatorAutoConfigDialog
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
            var dockerProjectFolder = DockerProjectFolderPath;
            if (String.IsNullOrEmpty(dockerProjectFolder))
            {
                MessageBox.Show(this, "The Path to the Docker Project Folder must be entered!", EncConverters.cstrCaption);
                return false;
            }

            var selectedToLanguage = (ComboBoxItem)comboBoxTargetLanguages.SelectedItem;
            if ((selectedToLanguage == null) || (TargetLanguageNameMustBeConfigured == selectedToLanguage))
            {
                MessageBox.Show(this, "The Target Language must be selected!", EncConverters.cstrCaption);
                return false;
            }

            var selectedFromLanguage = (ComboBoxItem)comboBoxSourceLanguages.SelectedItem;
            if ((selectedFromLanguage == null) || (SourceLanguageNameMustBeConfigured == selectedFromLanguage))
            {
                MessageBox.Show(this, "The Source Language must be selected!", EncConverters.cstrCaption);
                return false;
            }

            // for this converter, use the source and target language codes (e.g. hin_Deva) as the converter identifier
            // UPDATE: also include the path to the project and the API key (encrypted) and the Endpoint, since it's
            //  possible to have multiple models running. The latter two can be blank, though to just revert to the
            //  defaults (i.e. your-api-key-here and http://localhost:8000, respectively)
            // P.S. no need to validate them, bkz if they don't exist, then we wouldn't have the selectedLgs either
            ConverterIdentifier = String.Format("{0};{1};{2};{3};{4}",
                dockerProjectFolder,
                selectedFromLanguage.Code,
                selectedToLanguage.Code,
                NllbTranslatorEndpoint,
                EncryptionClass.Encrypt(NllbTranslatorApiKey));

            return base.OnApply();
        }

        protected override string ProgID
        {
            get
            {
                return typeof(NllbTranslatorEncConverter).FullName; 
            }
        }

        protected override string ImplType
        {
            get
            {
                return EncConverters.strTypeSILNllbTranslator;
            }
        }

        protected override string DefaultFriendlyName
        {
            // as the default, make it the same as the table name (w/o extension)
            get
            {
                var selectedSourceLanguage = (ComboBoxItem)comboBoxSourceLanguages.SelectedItem;
                var selectedTargetLanguage = (ComboBoxItem)comboBoxTargetLanguages.SelectedItem;
                return $"NLLB{ModelNameSuffix} Translate {selectedSourceLanguage} to {selectedTargetLanguage}";
            }

        }

        /// <summary>
        /// Initialize the source and possibly target language combo boxes with the translation languages possible.
        /// For TranslateWithTransliteration, we only need the source language, so the 'initializeTargetLanguageAlso' parameter should be false
        /// </summary>
        /// <param name="initializeTargetLanguageAlso">true to initialize the target language combo box also</param>
        private void InitializeSourceAndTargetLanguages(List<ComboBoxItem> languagesSupported)
        {
            var items = languagesSupported.ToArray();
            comboBoxSourceLanguages.Items.Clear();
            comboBoxSourceLanguages.Items.Add(SourceLanguageNameMustBeConfigured);
            comboBoxSourceLanguages.Items.AddRange(items);

            comboBoxTargetLanguages.Items.Clear();
            comboBoxTargetLanguages.Items.Add(TargetLanguageNameMustBeConfigured);
            comboBoxTargetLanguages.Items.AddRange(items);
        }

        public class ComboBoxItem
        {
            public string Display { get; set; }
            public string Code { get; set; }
            public override string ToString()
            {
                return Display;
            }

            public override bool Equals(object obj)
            {
                if (obj == null)
                    return false;
                var objAsComboBoxItem = obj as ComboBoxItem;
                return (objAsComboBoxItem?.Display == Display) || (objAsComboBoxItem?.Code == Code);
            }

            public override int GetHashCode()
            {
                int hashCode = 1075847657;
                hashCode = hashCode * -1521134295 + EqualityComparer<string>.Default.GetHashCode(Code);
                hashCode = hashCode * -1521134295 + EqualityComparer<string>.Default.GetHashCode(Display);
                return hashCode;
            }
        }

        private void ButtonSetNllbTranslateApiKey_Click(object sender, EventArgs e)
        {
            var dockerProjectFolderPath = DockerProjectFolderPath;
            if (string.IsNullOrEmpty(dockerProjectFolderPath))
            {
                MessageBox.Show($"You must browse for/enter the path to where the Docker Project is located or should be created.", EncConverters.cstrCaption);
                return;
            }

            var apiKey = NllbTranslatorApiKey;
            var endpoint = NllbTranslatorEndpoint;
            if (m_aEC != null)
            {
                var theTranslator = (NllbTranslatorEncConverter)m_aEC;
                apiKey = theTranslator.ApiKey;
                endpoint = theTranslator.Endpoint;
            }


            using var dlg = new QueryForEndpointAndApiKey(dockerProjectFolderPath, apiKey, endpoint);
            if (dlg.ShowDialog() == DialogResult.OK)
            {
                // if the user configures a model, then save the API Key and Endpoint for any new converters they create
                // the path was set earlier, but save it here (since this means the user at least intended to do something,
                // whether they build the model (successfully) or not)
                Properties.Settings.Default.NllbTranslatorPathToDockerProject = dockerProjectFolderPath;
                NllbTranslatorApiKey = dlg.TranslatorApiKey;
                endpoint = dlg.Endpoint;
                NllbTranslatorEndpoint = (endpoint == Properties.Settings.Default.NllbTranslatorEndpoint) ? null : endpoint;
                Properties.Settings.Default.Save();

                m_aEC = null;    // reset the associated EncConverter instance so it'll get rebuilt w/ the new parameters
                ModelNameSuffix = dlg.ModelNameSuffix;	// so we can add it to the DefaultFriendlyName

                // in case something changed, reinitialize the combo boxes
                GetLanguagesSupportedAndInitializeComboBoxes(m_bInitialized, dlg.TranslatorApiKey, dlg.Endpoint);
            }
        }

        private string DockerProjectFolderPath
        {
            get { return textBoxDockerProjectFolder.Text?.Trim(); }
            set { textBoxDockerProjectFolder.Text = value;  }
        }

        private void buttonBrowse_Click(object sender, System.EventArgs e)
        {
            folderBrowserDialog.SelectedPath = Path.Combine(Path.Combine(Environment.GetFolderPath(SpecialFolder.CommonApplicationData), "SIL"), "NLLB Docker Folder" + Path.PathSeparator);
            if (folderBrowserDialog.ShowDialog() == DialogResult.OK)
            {
                DockerProjectFolderPath = folderBrowserDialog.SelectedPath;
                buttonConfigureNllbModel.Enabled = true;
            }
        }

        private void ComboBoxSourceLanguages_SelectedIndexChanged_1(object sender, EventArgs e)
        {
            IsModified = true;
        }

        private void ComboBoxTargetLanguages_SelectedIndexChanged_1(object sender, EventArgs e)
        {
            IsModified = true;
        }
    }
}

