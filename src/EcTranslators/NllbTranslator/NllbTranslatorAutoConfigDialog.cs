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
        private const int RowIndexDockerProjectFolder = 1;
        private const float RowHeightDockerProjectFolder = 50F;
        private const string ButtonLabelConfigureLocalModel = "Configure NLLB Model";
        private const string ButtonLabelConfigureRemoteModel = "Configure &Connection to NLLB Server";

        private readonly ComboBoxItem SourceLanguageNameMustBeConfigured = new ComboBoxItem { Display = "Select Source Language" };
        private readonly ComboBoxItem TargetLanguageNameMustBeConfigured = new ComboBoxItem { Display = "Select Target Language" };
        private string ModelNameSuffix = String.Empty;    // so we can add it to the friendly name -- but only works if the user edits (which they should do, but...)

        // the endpoint and api key for this converter (initially either the values for the converter being edited or the
        //  defaults from the last one configured)
        private string _endpoint;
        private string _apiKey;

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
                                         out string apiKey, out string endpoint, out string localModelPath);

                _apiKey = apiKey;
                _endpoint = endpoint;

                // if there's no Docker project folder, then the model is hosted on another machine
                radioButtonHostRemote.Checked = String.IsNullOrEmpty(pathToDockerProject);
                DockerProjectFolderPath = pathToDockerProject;
                UpdateHostingModeUi();
                IsModified = false;

                InitializeLanguageComboBoxes(true, apiKey, endpoint, fromLanguage, toLanguage);
            }
            else
            {
                // if we've done one before... see if it still works
                DockerProjectFolderPath = Properties.Settings.Default.NllbTranslatorPathToDockerProject;
                UpdateHostingModeUi();

                _apiKey = NllbTranslatorApiKey;
                _endpoint = NllbTranslatorEndpoint;
                if (!String.IsNullOrEmpty(_apiKey) && IsEndpointListening(_endpoint))
                {
                    InitializeLanguageComboBoxes(false, _apiKey, _endpoint, null, null);
                }
            }

            m_bInitialized = true;

            helpProvider.SetHelpString(radioButtonHostLocally, "Select this option if the NLLB model's Docker container is to be built and run on this computer");
            helpProvider.SetHelpString(radioButtonHostRemote, "Select this option if the NLLB model's Docker container is running on another computer on your network (e.g. http://192.168.1.20:8000)");
            helpProvider.SetHelpString(comboBoxSourceLanguages, Properties.Resources.HelpForNllbTranslatorSourceLanguagesComboBox);
            helpProvider.SetHelpString(comboBoxTargetLanguages, Properties.Resources.HelpForNllbTranslatorTargetLanguagesComboBox);
            helpProvider.SetHelpString(buttonConfigureNllbModel, Properties.Resources.HelpForNllbTranslatorAddYourOwnApiKey);

            Util.DebugWriteLine(this, "END");
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

        private bool IsRemoteHost => radioButtonHostRemote.Checked;

        private static bool IsEndpointListening(string endpoint)
        {
            try
            {
                return !String.IsNullOrEmpty(endpoint) && IsHttpServerListeningAsync(endpoint).Result;
            }
            catch (Exception ex)
            {
                // e.g. an invalid URI
                System.Diagnostics.Debug.WriteLine(ex.Message);
                return false;
            }
        }

        /// <summary>
        /// Initialize the Source and Target language combo boxes from the languages the server says it supports. Each is
        /// only enabled if the server returns at least one language for it. If there's only one, it's pre-selected. If
        /// the server can't be reached, then any language code we already know about (e.g. from the converter being
        /// edited or from a local model's configuration) is shown, but the combo box is left disabled.
        /// </summary>
        private void InitializeLanguageComboBoxes(bool showError, string apiKey, string endpoint, string fromLanguage, string toLanguage)
        {
            // for our purposes here, we only need the Model configuration (so we can hit the endpoint for languages supported);
            //  not the specific languages we want to convert To/From. So we don't want to use 'OnApply' here, bkz it will fails
            //  so just create a temporary one and set the key/endpoint and use it to get the languages supported.
            SupportedLanguages languagesSupported = null;
            if (!String.IsNullOrEmpty(endpoint))
            {
                var theNllbEncConverter = new NllbTranslatorEncConverter
                {
                    ApiKey = apiKey,
                    Endpoint = endpoint
                };
                languagesSupported = theNllbEncConverter.GetCapabilities(showError).GetAwaiter().GetResult();
            }

            if (languagesSupported != null)
            {
                InitializeLanguageComboBox(comboBoxSourceLanguages, SourceLanguageNameMustBeConfigured, languagesSupported.Sources, fromLanguage);
                InitializeLanguageComboBox(comboBoxTargetLanguages, TargetLanguageNameMustBeConfigured, languagesSupported.Targets, toLanguage);
            }
            else
            {
                InitializeLanguageComboBoxWithoutServer(comboBoxSourceLanguages, fromLanguage);
                InitializeLanguageComboBoxWithoutServer(comboBoxTargetLanguages, toLanguage);
            }
        }

        private static void InitializeLanguageComboBox(ComboBox comboBox, ComboBoxItem placeholder, List<LanguageInfo> languages, string selectedCode)
        {
            var items = languages.Select(l => new ComboBoxItem { Code = l.Code, Display = l.Name }).ToArray();

            comboBox.Items.Clear();
            switch (items.Length)
            {
                case 0:
                    // the server doesn't need to be told (e.g. a model that only does one pair)
                    comboBox.Enabled = false;
                    return;

                case 1:
                    comboBox.Items.Add(items[0]);
                    comboBox.SelectedIndex = 0;
                    break;

                default:
                    comboBox.Items.Add(placeholder);
                    comboBox.Items.AddRange(items);
                    comboBox.SelectedItem = (object)items.FirstOrDefault(i => i.Code == selectedCode) ?? placeholder;
                    break;
            }
            comboBox.Enabled = true;
        }

        private static void InitializeLanguageComboBoxWithoutServer(ComboBox comboBox, string knownCode)
        {
            comboBox.Items.Clear();
            comboBox.Enabled = false;
            if (String.IsNullOrEmpty(knownCode))
                return;

            var item = new ComboBoxItem { Code = knownCode, Display = GetLanguageName(knownCode) };
            comboBox.Items.Add(item);
            comboBox.SelectedItem = item;
        }

        private void ResetLanguageComboBoxes()
        {
            InitializeLanguageComboBoxWithoutServer(comboBoxSourceLanguages, null);
            InitializeLanguageComboBoxWithoutServer(comboBoxTargetLanguages, null);
        }

        /// <summary>
        /// Get the language code selected in the combo box. If the combo box is enabled (i.e. the server gave us options
        /// to choose from), then something must have been selected. If it's disabled, then use whatever (if anything) it
        /// was pre-filled with.
        /// </summary>
        private static bool TryGetSelectedLanguageCode(ComboBox comboBox, ComboBoxItem placeholder, out string code)
        {
            var selectedItem = comboBox.SelectedItem as ComboBoxItem;
            if (comboBox.Enabled && ((selectedItem == null) || (selectedItem == placeholder)))
            {
                code = null;
                return false;
            }

            code = selectedItem?.Code ?? String.Empty;
            return true;
        }

        // this method is called either when the user clicks the "Apply" or "OK" buttons *OR* if she
        //  tries to switch to the Test or Advanced tab. This is the dialog's one opportunity
        //  to make sure that the user has correctly configured a legitimate converter.
        protected override bool OnApply()
        {
            var isRemoteHost = IsRemoteHost;
            var dockerProjectFolder = isRemoteHost ? String.Empty : DockerProjectFolderPath;
            if (!isRemoteHost && String.IsNullOrEmpty(dockerProjectFolder))
            {
                MessageBox.Show(this, "The Path to the Docker Project Folder must be entered!", EncConverters.cstrCaption);
                return false;
            }

            if (isRemoteHost && String.IsNullOrEmpty(_endpoint))
            {
                MessageBox.Show(this, "Click the 'Configure Connection to NLLB Server' button to enter the address of the machine hosting the model!", EncConverters.cstrCaption);
                return false;
            }

            if (!TryGetSelectedLanguageCode(comboBoxTargetLanguages, TargetLanguageNameMustBeConfigured, out string toLanguageCode))
            {
                MessageBox.Show(this, "The Target Language must be selected!", EncConverters.cstrCaption);
                return false;
            }

            if (!TryGetSelectedLanguageCode(comboBoxSourceLanguages, SourceLanguageNameMustBeConfigured, out string fromLanguageCode))
            {
                MessageBox.Show(this, "The Source Language must be selected!", EncConverters.cstrCaption);
                return false;
            }

            // for this converter, use the source and target language codes (e.g. hin_Deva) as the converter identifier
            // UPDATE: also include the path to the project and the API key (encrypted) and the Endpoint, since it's
            //  possible to have multiple models running. The latter two can be blank, though to just revert to the
            //  defaults (i.e. '' and http://localhost:8000, respectively)
            // UPDATE2: the path to the project is blank if the model is hosted on another machine, and the language codes
            //  can be blank if the model doesn't need them (e.g. it only does one pair)
            ConverterIdentifier = String.Format("{0};{1};{2};{3};{4}",
                dockerProjectFolder,
                fromLanguageCode,
                toLanguageCode,
                _endpoint,
                EncryptionClass.Encrypt(_apiKey ?? String.Empty));

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
                var selectedSourceLanguage = comboBoxSourceLanguages.SelectedItem as ComboBoxItem;
                var selectedTargetLanguage = comboBoxTargetLanguages.SelectedItem as ComboBoxItem;
                if (!String.IsNullOrEmpty(selectedSourceLanguage?.Code) && !String.IsNullOrEmpty(selectedTargetLanguage?.Code))
                    return $"NLLB{ModelNameSuffix} Translate {selectedSourceLanguage} to {selectedTargetLanguage}";

                // the model doesn't need the languages, so just identify it by where it's hosted
                string host;
                try
                {
                    host = new Uri(_endpoint).Authority;
                }
                catch
                {
                    host = _endpoint;
                }
                return $"NLLB{ModelNameSuffix} Translate ({host})";
            }
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
            var isRemoteHost = IsRemoteHost;
            var dockerProjectFolderPath = DockerProjectFolderPath;
            if (!isRemoteHost && string.IsNullOrEmpty(dockerProjectFolderPath))
            {
                MessageBox.Show($"You must browse for/enter the path to where the Docker Project is located or should be created.", EncConverters.cstrCaption);
                return;
            }

            using var dlg = new QueryForEndpointAndApiKey(isRemoteHost ? null : dockerProjectFolderPath, _apiKey, _endpoint, isRemoteHost);
            if (dlg.ShowDialog() == DialogResult.OK)
            {
                // if the user configures a model, then save the API Key and Endpoint for any new converters they create
                // the path was set earlier, but save it here (since this means the user at least intended to do something,
                // whether they build the model (successfully) or not)
                if (!isRemoteHost)
                    Properties.Settings.Default.NllbTranslatorPathToDockerProject = dockerProjectFolderPath;

                _apiKey = dlg.TranslatorApiKey?.Trim();
                var endpoint = dlg.Endpoint?.Trim();
                _endpoint = String.IsNullOrEmpty(endpoint) ? Properties.Settings.Default.NllbTranslatorEndpoint : endpoint;

                NllbTranslatorApiKey = _apiKey;
                NllbTranslatorEndpoint = (_endpoint == Properties.Settings.Default.NllbTranslatorEndpoint) ? null : _endpoint;
                Properties.Settings.Default.Save();

                m_aEC = null;    // reset the associated EncConverter instance so it'll get rebuilt w/ the new parameters
                ModelNameSuffix = dlg.ModelNameSuffix;    // so we can add it to the DefaultFriendlyName
                IsModified = true;

                // in case something changed, reinitialize the combo boxes (the From/To language names are only known
                //  for a local model, in case the server doesn't answer)
                InitializeLanguageComboBoxes(m_bInitialized, _apiKey, _endpoint, dlg.FromLanguageName, dlg.ToLanguageName);
            }
        }

        /// <summary>
        /// Show/hide the controls that only apply to building and hosting the Docker container on this machine
        /// </summary>
        private void UpdateHostingModeUi()
        {
            var isLocalHost = !IsRemoteHost;
            labelFolderPath.Visible = textBoxDockerProjectFolder.Visible = buttonBrowse.Visible = isLocalHost;
            tableLayoutPanel1.RowStyles[RowIndexDockerProjectFolder].Height = isLocalHost ? RowHeightDockerProjectFolder : 0F;
            buttonConfigureNllbModel.Text = isLocalHost ? ButtonLabelConfigureLocalModel : ButtonLabelConfigureRemoteModel;
            buttonConfigureNllbModel.Enabled = !isLocalHost || !String.IsNullOrEmpty(DockerProjectFolderPath);
        }

        private void radioButtonHosting_CheckedChanged(object sender, EventArgs e)
        {
            // this gets called for both the one being unchecked and the one being checked; we only need the latter
            if (!((RadioButton)sender).Checked)
                return;

            UpdateHostingModeUi();
            if (!m_bInitialized)
                return;

            // whatever languages we had may not apply to the other hosting model, so start over with them
            ResetLanguageComboBoxes();
            IsModified = true;
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

        private void textBoxDockerProjectFolder_TextChanged(object sender, EventArgs e)
            {
            if (!String.IsNullOrEmpty(textBoxDockerProjectFolder.Text))
            {
                IsModified = true;
                buttonConfigureNllbModel.Enabled = true;
            }
        }
    }
}

