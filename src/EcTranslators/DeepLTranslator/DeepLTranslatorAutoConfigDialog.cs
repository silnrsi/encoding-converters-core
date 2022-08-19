using System;
using System.Windows.Forms;
using ECInterfaces;                     // for IEncConverter
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using static SilEncConverters40.EcTranslators.DeepLTranslator.DeepLTranslatorEncConverter;
using System.Net;

namespace SilEncConverters40.EcTranslators.DeepLTranslator
{
	public partial class DeepLTranslatorAutoConfigDialog : AutoConfigDialog
	{
		private const string SourceLanguageNameAutoDetect = "Auto-Detect";
		private const string TargetLanguageNameMustBeConfigure = "Select Target Language";

		protected TransductionType transductionSelected;
		protected DeepL.Formality formalityLevel;


		protected List<DeepL.Model.SourceLanguage> languagesSource;
		protected List<DeepL.Model.TargetLanguage> languagesTarget;
		protected List<DeepL.Model.GlossaryLanguagePair> glossaryLanguagePairs;

		public DeepLTranslatorAutoConfigDialog
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

			var capabilities = GetCapabilities().GetAwaiter().GetResult();
			languagesSource = capabilities.languagesSource;
			languagesTarget = capabilities.languagesTarget;
			glossaryLanguagePairs = capabilities.glossaryLanguagePairs;

			transductionSelected = TransductionType.Translate;  // by default
			formalityLevel = DeepL.Formality.Default;
			string fromLanguage = SourceLanguageNameAutoDetect, toLanguage = TargetLanguageNameMustBeConfigure;
			InitializeSourceAndTargetLanguages(initializeTargetLanguageAlso: true);
			labelJuiceLeft.Text = capabilities.usageLeft;

			// if we're editing converter, then set the Converter Spec and say it's unmodified
			if (m_bEditMode)
			{
				System.Diagnostics.Debug.Assert(!String.IsNullOrEmpty(ConverterIdentifier));

				ParseConverterIdentifier(ConverterIdentifier, out transductionSelected,
										 ref fromLanguage, out toLanguage,
										 out formalityLevel);

				InitializeComboBoxFromCode(comboBoxSourceLanguages, fromLanguage);
				InitializeComboBoxFromCode(comboBoxTargetLanguages, toLanguage);

				var targetLanguage = languagesTarget.FirstOrDefault(l => l.Code == toLanguage);
				radioButtonFormalityLess.Enabled = radioButtonFormalityMore.Enabled = targetLanguage.SupportsFormality;

				IsModified = false;
			}
			else
			{
				comboBoxSourceLanguages.SelectedItem = fromLanguage;
				comboBoxTargetLanguages.SelectedItem = toLanguage;
			}

			m_bInitialized = true;

			helpProvider.SetHelpString(comboBoxSourceLanguages, Properties.Resources.HelpForDeepLTranslatorSourceLanguagesComboBox);
			helpProvider.SetHelpString(comboBoxTargetLanguages, Properties.Resources.HelpForDeepLTranslatorTargetLanguagesComboBox);
			helpProvider.SetHelpString(groupBoxFormalityLevel, Properties.Resources.HelpForDeepLTranslatorFormalityLevel);
			helpProvider.SetHelpString(buttonSetDeepLTranslateApiKey, Properties.Resources.HelpForDeepLTranslatorAddYourOwnApiKey);

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

		public DeepLTranslatorAutoConfigDialog
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

			var formalityLevel = (_targetLanguageFormalitySupport)
				? (radioButtonFormalityLess.Checked)
					? DeepL.Formality.Less
					: DeepL.Formality.More
				: DeepL.Formality.Default;

			// for this converter, the converter identifier is build from these values as follows:
			ConverterIdentifier = String.Format("{0};{1};{2};{3}",
				transductionSelected.ToString(),
				ExtractCode(selectedFromLanguage),
				ExtractCode(selectedToLanguage),
				formalityLevel);

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
                return typeof(DeepLTranslatorEncConverter).FullName; 
            }
        }

        protected override string ImplType
        {
            get
            {
                return EncConverters.strTypeSILDeepLTranslator;
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
				switch (transductionSelected)
				{
					case TransductionType.Translate:
						return $"DeepL Translate {selectedSourceLanguage} to {comboBoxTargetLanguages.SelectedItem}";
					case TransductionType.DictionaryLookup:
						return $"DeepL Dictionary Lookup of {selectedSourceLanguage} words in {comboBoxTargetLanguages.SelectedItem}";
					default:
						return ConverterIdentifier;
				};
			}
        }

        private void textBoxFileSpec_TextChanged(object sender, EventArgs e)
        {
            if (m_bInitialized) // but only do this after we've already initialized (we might have set it during m_bEditMode)
                IsModified = (((TextBox)sender).Text.Length > 0);
        }

		private bool _sourceLanguagesInitialized = false;
		private bool _targetLanguagesInitialized = false;
		private bool _targetLanguageFormalitySupport = false;


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

			var sourceLanguagesPossible = languagesSource.Select(t => $"{t.Name} ({t.Code})").OrderBy(s => s).ToArray();
			comboBoxSourceLanguages.Items.Clear();
			comboBoxSourceLanguages.Items.Add(SourceLanguageNameAutoDetect);
			comboBoxSourceLanguages.Items.AddRange(sourceLanguagesPossible);

			if (!_targetLanguagesInitialized && initializeTargetLanguageAlso)
			{
				_targetLanguagesInitialized = true;

				var targetLanguagesPossible = languagesTarget.Select(t => $"{t.Name} ({t.Code})").OrderBy(s => s).ToArray();
				comboBoxTargetLanguages.Items.Clear();
				comboBoxTargetLanguages.Items.Add(TargetLanguageNameMustBeConfigure);
				comboBoxTargetLanguages.Items.AddRange(targetLanguagesPossible);
			}
		}

		private void radioButtonTranslate_CheckedChanged(object sender, EventArgs e)
		{
			if (!(sender as RadioButton).Checked)
				return; // means it was unchecked

			transductionSelected = TransductionType.Translate;
			ProcessType = (int)ProcessTypeFlags.Translation;

			comboBoxTargetLanguages.Visible = labelTargetLanguage.Visible = true;

			InitializeSourceAndTargetLanguages(initializeTargetLanguageAlso: true);

			IsModified = true;
		}

		private void radioButtonDictionaryLookup_CheckedChanged(object sender, EventArgs e)
		{
			if (!(sender as RadioButton).Checked)
				return; // means it was unchecked

			transductionSelected = TransductionType.DictionaryLookup;
			ProcessType = (int)ProcessTypeFlags.Translation;

			comboBoxTargetLanguages.Visible = labelTargetLanguage.Visible = true;

			InitializeSourceAndTargetLanguages(initializeTargetLanguageAlso: true);

			IsModified = true;
		}

		private void comboBoxTargetLanguages_SelectedIndexChanged(object sender, EventArgs e)
		{
			IsModified = true;

			// once the target language is chosen, we need to see if it supports formality or not
			var selectedItem = (string)comboBoxTargetLanguages.SelectedItem;
			if (TargetLanguageNameMustBeConfigure == selectedItem)
			{
				return;
			}

			var targetLanguage = languagesTarget.FirstOrDefault(l => l.Code == ExtractCode(selectedItem));
			_targetLanguageFormalitySupport = targetLanguage.SupportsFormality;

			radioButtonFormalityLess.Checked = radioButtonFormalityLess.Enabled = radioButtonFormalityMore.Enabled = _targetLanguageFormalitySupport;
		}

		private void comboBoxSourceLanguages_SelectedIndexChanged(object sender, EventArgs e)
		{
			IsModified = true;
		}

		private void buttonSetDeepLTranslateApiKey_Click(object sender, EventArgs e)
		{
			// only send the key if it's already the override key (so we don't expose ours)
			var dlg = new QueryForDeepLKey(Properties.Settings.Default.DeepLTranslatorKeyOverride);
			if (dlg.ShowDialog() == DialogResult.Yes)
			{
				Properties.Settings.Default.DeepLTranslatorKeyOverride = DeepLTranslatorSubscriptionKey = dlg.TranslatorKey;
				Properties.Settings.Default.Save();
			}
		}

		private void radioButtonFormality_CheckedChanged(object sender, EventArgs e)
		{
			IsModified = true;
		}
	}
}

