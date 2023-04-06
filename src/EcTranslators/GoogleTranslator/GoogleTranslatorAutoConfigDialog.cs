#define DisableBilling

using System;
using System.Windows.Forms;
using ECInterfaces;                     // for IEncConverter
using static SilEncConverters40.EcTranslators.GoogleTranslator.GoogleTranslatorEncConverter;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using SilEncConverters40.EcTranslators.GoogleTranslator;
using Google.Cloud.Translation.V2;
using System.Net;
using Google.Apis.Auth.OAuth2;
using Newtonsoft.Json;

#if !DisableBilling
using System.Threading.Tasks;
using Google.Cloud.Billing.V1;
using Grpc.Core;
#endif

namespace SilEncConverters40.EcTranslators.GoogleTranslator
{
	public partial class GoogleTranslatorAutoConfigDialog : AutoConfigDialog
	{
		private const string SourceLanguageNameAutoDetect = "Auto-Detect";
		private const string TargetLanguageNameMustBeConfigure = "Select Target Language";

		protected List<Language> LanguagesSupported;

		public GoogleTranslatorAutoConfigDialog
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

			LanguagesSupported = GetCapabilities().GetAwaiter().GetResult();

			string fromLanguage = SourceLanguageNameAutoDetect, toLanguage = TargetLanguageNameMustBeConfigure;
			InitializeSourceAndTargetLanguages(initializeTargetLanguageAlso: true);
#if !DisableBilling
			labelJuiceLeft.Text = GetUsage().GetAwaiter().GetResult();
#else
			labelJuiceLeft.Text = String.Empty;
#endif
			// if we're editing converter, then set the Converter Spec and say it's unmodified
			if (m_bEditMode)
			{
				System.Diagnostics.Debug.Assert(!String.IsNullOrEmpty(ConverterIdentifier));

				ParseConverterIdentifier(ConverterIdentifier, ref fromLanguage, out toLanguage);

				InitializeComboBoxFromCode(comboBoxSourceLanguages, fromLanguage);
				InitializeComboBoxFromCode(comboBoxTargetLanguages, toLanguage);
				IsModified = false;
			}
			else
			{
				comboBoxSourceLanguages.SelectedItem = fromLanguage;
				comboBoxTargetLanguages.SelectedItem = toLanguage;
			}

			m_bInitialized = true;

			helpProvider.SetHelpString(comboBoxSourceLanguages, Properties.Resources.HelpForGoogleTranslatorSourceLanguagesComboBox);
			helpProvider.SetHelpString(comboBoxTargetLanguages, Properties.Resources.HelpForGoogleTranslatorTargetLanguagesComboBox);
			helpProvider.SetHelpString(buttonSetGoogleTranslateApiKey, Properties.Resources.HelpForGoogleTranslatorAddYourOwnApiKey);

			Util.DebugWriteLine(this, "END");
        }

#if !DisableBilling
		private static CloudBillingClient _billingClient;
		public static CloudBillingClient BillingClient
		{
			get
			{
				if (_billingClient == null)
				{
					var googleCreds = GoogleCredential.FromJson(GoogleTranslatorSubscriptionKey);
					var cloudBillingClient = new CloudBillingClientBuilder
					{
						GoogleCredential = googleCreds
					};

					_billingClient = cloudBillingClient.Build();
				}
				return _billingClient;
			}
		}

		public static async Task<string> GetUsage()
		{
			var request = new GetProjectBillingInfoRequest { Name = Properties.Settings.Default.GoogleCloudBillingProjectName };
			var projectInfo = await Task.Run(async delegate
			{
				return await BillingClient.GetProjectBillingInfoAsync(request);
			}).ConfigureAwait(false);

			return JsonConvert.SerializeObject(projectInfo);
		}
#endif

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

		public GoogleTranslatorAutoConfigDialog
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
			var selectedToLanguage = (string)comboBoxTargetLanguages.SelectedItem;
			if (TargetLanguageNameMustBeConfigure == selectedToLanguage)
			{
				MessageBox.Show(this, "The Target Language must be selected!", EncConverters.cstrCaption);
				return false;
			}

			var selectedFromLanguage = (string)comboBoxSourceLanguages.SelectedItem;
			if (selectedFromLanguage == SourceLanguageNameAutoDetect)
				selectedFromLanguage = null;

			// for TECkit, get the converter identifier from the Setup tab controls.
			ConverterIdentifier = String.Format("{0};{1}",
				ExtractCode(selectedFromLanguage),
				ExtractCode(selectedToLanguage));

            return base.OnApply();
        }

		// use $ at the end, for lgs like Chinese simplified, which have their own () value (we want the one at the end)
		private static Regex _regexExtractCode = new Regex(@"\(([-a-zA-Z]{2,}?)\)$");

		/// <summary>
		/// turn something like Hindi हिन्दी (hi) into just 'hi'
		/// </summary>
		/// <param name="languageName"></param>
		/// <returns></returns>
		public static string ExtractCode(string languageName)
		{
			if (String.IsNullOrEmpty(languageName))
				return String.Empty;

			var match = _regexExtractCode.Match(languageName);
			return (match.Success && match.Groups.Count == 2)
				? match.Groups[1].Value
				: languageName;
		}

		protected override string ProgID
        {
            get
            {
                return typeof(GoogleTranslatorEncConverter).FullName; 
            }
        }

        protected override string ImplType
        {
            get
            {
                return EncConverters.strTypeSILGoogleTranslator;
            }
        }

        protected override string DefaultFriendlyName
        {
            // as the default, make it the same as the table name (w/o extension)
            get
			{
				var selectedSourceLanguage = (string)comboBoxSourceLanguages.SelectedItem;
				if (selectedSourceLanguage == SourceLanguageNameAutoDetect)
					selectedSourceLanguage = "Any";	// keep it simple
				return $"Google Translate {selectedSourceLanguage} to {comboBoxTargetLanguages.SelectedItem}";
			}
        }

        private void textBoxFileSpec_TextChanged(object sender, EventArgs e)
        {
            if (m_bInitialized) // but only do this after we've already initialized (we might have set it during m_bEditMode)
                IsModified = (((TextBox)sender).Text.Length > 0);
        }

		private bool _sourceLanguagesInitialized = false;
		private bool _targetLanguagesInitialized = false;

		/// <summary>
		/// Initialize the source and possibly target language combo boxes with the translation languages possible.
		/// For TranslateWithTransliteration, we only need the source language, so the 'initializeTargetLanguageAlso' parameter should be false
		/// </summary>
		/// <param name="initializeTargetLanguageAlso">true to initialize the target language combo box also</param>
		private void InitializeSourceAndTargetLanguages(bool initializeTargetLanguageAlso)
		{
			if (_sourceLanguagesInitialized && (_targetLanguagesInitialized || !initializeTargetLanguageAlso))
				return;

			_sourceLanguagesInitialized = true;

			var translationLanguagesPossible = LanguagesSupported.Select(t => $"{t.Name} ({t.Code})").OrderBy(s => s).ToArray();
			comboBoxSourceLanguages.Items.Clear();
			comboBoxSourceLanguages.Items.Add(SourceLanguageNameAutoDetect);
			comboBoxSourceLanguages.Items.AddRange(translationLanguagesPossible);

			if (!_targetLanguagesInitialized && initializeTargetLanguageAlso)
			{
				_targetLanguagesInitialized = true;

				comboBoxTargetLanguages.Items.Clear();
				comboBoxTargetLanguages.Items.Add(TargetLanguageNameMustBeConfigure);
				comboBoxTargetLanguages.Items.AddRange(translationLanguagesPossible);
			}
		}

		private void comboBoxTargetLanguages_SelectedIndexChanged(object sender, EventArgs e)
		{
			IsModified = true;  // modified even if we don't update script languages

			// we don't initialize the script language(s) based on the target language changing unless we're in one of the transliterate modes
			string selectedItem = (string)comboBoxTargetLanguages.SelectedItem;
			if (TargetLanguageNameMustBeConfigure == selectedItem)
			{
				return;
			}

			var targetLanguage = LanguagesSupported.FirstOrDefault(l => l.Code == ExtractCode(selectedItem));
		}

		private void comboBoxSourceLanguages_SelectedIndexChanged(object sender, EventArgs e)
		{
			IsModified = true;	// modified even if we don't update script languages
		}

		private void ButtonSetGoogleTranslateApiKey_Click(object sender, EventArgs e)
		{
#if encryptingNewCredentials
			var googleTranslatorKeyHide = Properties.Settings.Default.GoogleTranslatorCredentials;
			var credentials = EncryptionClass.Encrypt(googleTranslatorKeyHide);
#endif
			// only send the key if it's already the override key (so we don't expose ours)
			var translatorCredentialsOverride = Properties.Settings.Default.GoogleTranslatorCredentialsOverride;
			if (!String.IsNullOrEmpty(translatorCredentialsOverride))
				translatorCredentialsOverride = EncryptionClass.Decrypt(translatorCredentialsOverride);

			using var dlg = new QueryForGoogleCredentials(translatorCredentialsOverride);
			if (dlg.ShowDialog() == DialogResult.Yes)
			{
				var translatorKey = EncryptionClass.Encrypt(dlg.TranslatorKey);
				GoogleTranslatorSubscriptionKey = translatorKey;
				Properties.Settings.Default.Save();
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

