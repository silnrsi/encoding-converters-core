using System;
using System.Windows.Forms;
using ECInterfaces;                     // for IEncConverter
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using static SilEncConverters40.EcTranslators.BingTranslator.BingTranslatorEncConverter;

namespace SilEncConverters40.EcTranslators.BingTranslator
{
	public partial class BingTranslatorAutoConfigDialog : AutoConfigDialog
	{
		private const string SourceLanguageNameAutoDetect = "Auto-Detect";
		private const string TargetLanguageNameMustBeConfigure = "Select Target Language";

		protected TransductionType transductionSelected;
		protected List<TranslationLanguage> translationsPossible;
		protected List<TransliterationLanguage> transliterationsPossible;
		protected List<DictionaryLanguage> dictionaryLookupsPossible;

		public BingTranslatorAutoConfigDialog
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

			var result = GetCapabilities();
			translationsPossible = result.translations;
			transliterationsPossible = result.transliterations;
			dictionaryLookupsPossible = result.dictionaryOptions;

			transductionSelected = TransductionType.Translate;	// by default
			string fromLanguage = SourceLanguageNameAutoDetect, toLanguage = TargetLanguageNameMustBeConfigure, toScript, fromScript;

			// if we're editing converter, then set the Converter Spec and say it's unmodified
			if (m_bEditMode)
			{
				System.Diagnostics.Debug.Assert(!String.IsNullOrEmpty(ConverterIdentifier));

				ParseConverterIdentifier(ConverterIdentifier, out transductionSelected,
										 ref fromLanguage, out toLanguage,
										 out fromScript, out toScript);

				switch (transductionSelected)
				{
					case TransductionType.TranslateWithTransliterate:
						{
							radioButtonTranslateWithTransliteration.Checked = true;
							break;
						};

					case TransductionType.Transliterate:
						{
							radioButtonTransliterate.Checked = true;
							break;
						};

					case TransductionType.DictionaryLookup:
						{
							radioButtonDictionaryLookup.Checked = true;
							break;
						};

					case TransductionType.Translate:
					default:
						{
							radioButtonTranslate.Checked = true;
							break;
						};
				};

				InitializeComboBoxFromCode(comboBoxSourceLanguages, fromLanguage);
				InitializeComboBoxFromCode(comboBoxTargetLanguages, toLanguage);
				InitializeComboBoxFromCode(comboBoxTargetScripts, toScript);
				InitializeSourceScriptBasedOnComboBoxValuesAndTransductionType();
				IsModified = false;
			}
			else
			{
				radioButtonTranslate.Checked = true;    // to trigger the loading of the combo boxes

				comboBoxSourceLanguages.SelectedItem = fromLanguage;
				comboBoxTargetLanguages.SelectedItem = toLanguage;
			}

			m_bInitialized = true;

			helpProvider.SetHelpString(radioButtonTranslate, Properties.Resources.HelpForBingTranslatorRadioButtonTranslate);
			helpProvider.SetHelpString(radioButtonTranslateWithTransliteration, Properties.Resources.HelpForBingTranslatorRadioButtonTranslateWithTransliteration);
			helpProvider.SetHelpString(radioButtonTransliterate, Properties.Resources.HelpForBingTranslatorRadioButtonTransliterate);
			helpProvider.SetHelpString(radioButtonDictionaryLookup, Properties.Resources.HelpForBingTranslatorRadioButtonDictionaryLookup);
			helpProvider.SetHelpString(comboBoxSourceLanguages, Properties.Resources.HelpForBingTranslatorSourceLanguagesComboBox);
			helpProvider.SetHelpString(comboBoxTargetLanguages, Properties.Resources.HelpForBingTranslatorTargetLanguagesComboBox);
			helpProvider.SetHelpString(textBoxSourceScript, Properties.Resources.HelpForBingTranslatorSourceScriptTextBox);
			helpProvider.SetHelpString(comboBoxTargetScripts, Properties.Resources.HelpForBingTranslatorTargetScriptsComboBox);
			helpProvider.SetHelpString(buttonSetBingTranslateApiKey, Properties.Resources.HelpForBingTranslatorAddYourOwnApiKey);

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

		public BingTranslatorAutoConfigDialog
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
				if (transductionSelected == TransductionType.Transliterate)
				{
					selectedToLanguage = null;
				}
				else
				{
					MessageBox.Show(this, "The Target Language must be selected!", EncConverters.cstrCaption);
					return false;
				}
			}

			var selectedFromLanguage = (string)comboBoxSourceLanguages.SelectedItem;
			if (selectedFromLanguage == SourceLanguageNameAutoDetect)
				selectedFromLanguage = null;

			// for TECkit, get the converter identifier from the Setup tab controls.
			ConverterIdentifier = String.Format("{0};{1};{2};{3};{4}",
				transductionSelected.ToString(),
				ExtractCode(selectedFromLanguage),
				ExtractCode(selectedToLanguage),
				ExtractCode(textBoxSourceScript.Text),
				ExtractCode((string)comboBoxTargetScripts.SelectedItem));

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
                return typeof(BingTranslatorEncConverter).FullName; 
            }
        }

        protected override string ImplType
        {
            get
            {
                return EncConverters.strTypeSILBingTranslator;
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
						return $"Bing Translate {selectedSourceLanguage} to {comboBoxTargetLanguages.SelectedItem}";
					case TransductionType.TranslateWithTransliterate:
						return $"Bing Translate {selectedSourceLanguage} to {comboBoxTargetLanguages.SelectedItem} + Transliterate to {comboBoxTargetScripts.SelectedItem} script";
					case TransductionType.Transliterate:
						return $"Bing Transliterate {selectedSourceLanguage} from {textBoxSourceScript.Text} script to {comboBoxTargetScripts.SelectedItem} script";
					case TransductionType.DictionaryLookup:
						return $"Bing Dictionary Lookup of {selectedSourceLanguage} words in {comboBoxTargetLanguages.SelectedItem}";
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
		private bool _targetScriptLanguagesInitialized = false;

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

			var translationLanguagesPossible = translationsPossible.Select(t => t.ToString()).OrderBy(s => s).ToArray();
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

		/// <summary>
		/// This method will search the transliteration list using either the source language (for Transliterate)
		/// or target language name (for TranslateWithTransliterate)
		/// </summary>
		/// <param name="relevantSelectedItem">the selected language name to search for the 0th script name from</param>
		private void InitializeSourceScript(string relevantSelectedItem, string currentTargetScript)
		{
			textBoxSourceScript.Text = ((relevantSelectedItem == null) || (relevantSelectedItem == SourceLanguageNameAutoDetect) ||
										(relevantSelectedItem == TargetLanguageNameMustBeConfigure))
										? null
										: transliterationsPossible.FirstOrDefault(t => t.ToString().Contains(relevantSelectedItem))?
																  .ScriptsSupported
																  .FirstOrDefault(s => String.IsNullOrEmpty(currentTargetScript) ||
																					   !s.ToString().Contains(currentTargetScript))?
																  .ToString();
		}

		private void InitializeTargetScriptLanguages()
		{
			if (_targetScriptLanguagesInitialized)
				return;

			// remove any previous contents
			comboBoxTargetScripts.Items.Clear();

			// the scripts we can transliterate *to* are dependent on the transduction type...
			string[] possibleTargetScripts;
			switch (transductionSelected)
			{
				case TransductionType.Transliterate:
					// for the Transliterate case, it's dependent on the source language
					var sourceLanguage = (string)comboBoxSourceLanguages.SelectedItem;
					if ((sourceLanguage == null) || (sourceLanguage == SourceLanguageNameAutoDetect))
					{
						// if the source language name hasn't been configured yet, the we don't want to allow
						//	selection (or even loading) of the target script combo box yet
						comboBoxTargetScripts.Enabled = false;
						return;
					}

					// for Transliterate, the target script options are all of the toScripts in the source language's Translateration
					possibleTargetScripts = transliterationsPossible.FirstOrDefault(t => t.ToString().Contains(sourceLanguage))?
																	.ScriptsSupported
																	.SelectMany(s => s.ToScripts.Select(ts => ts.ToString()))
																	.ToArray();
					break;

				case TransductionType.TranslateWithTransliterate:
					// for the TranslateWithTransliterate case, it's dependent on the target language
					var targetLanguage = (string)comboBoxTargetLanguages.SelectedItem;
					if ((targetLanguage == null) || (targetLanguage == TargetLanguageNameMustBeConfigure))
					{
						// if the target language name hasn't been configured yet, the we don't want to allow
						//	selection (or even loading) of the target script combo box yet
						comboBoxTargetScripts.Enabled = false;
						return;
					}

					// for TranslateWithTransliterate, the only option for target script is the toScripts of the 1st script
					possibleTargetScripts = transliterationsPossible.FirstOrDefault(t => t.ToString().Contains(targetLanguage))?
																	.ScriptsSupported
																	.FirstOrDefault()?
																	.ToScripts.Select(ts => ts.ToString())
																	.ToArray();
					break;

				default:
					System.Diagnostics.Debug.Fail($"Not expecting a transductionSelected of {transductionSelected} here!");
					return;
			}

			// enable it and load it with all but the 1st of the supported scripts for the source language (the 1st one
			//	being the native one for the language, so not a valid target script (there'd be nothing to do)
			// not every language supports transliteration
			if (possibleTargetScripts == null)
				return;

			comboBoxTargetScripts.Items.AddRange(possibleTargetScripts);
			comboBoxTargetScripts.SelectedIndex = 0;
			comboBoxTargetScripts.Enabled = _targetScriptLanguagesInitialized = true;
		}

		private void radioButtonTranslate_CheckedChanged(object sender, EventArgs e)
		{
			if (!(sender as RadioButton).Checked)
				return; // means it was unchecked

			transductionSelected = TransductionType.Translate;
			ProcessType = (int)ProcessTypeFlags.Translation;

			comboBoxTargetLanguages.Visible = labelTargetLanguage.Visible = true;

			InitializeSourceAndTargetLanguages(initializeTargetLanguageAlso: true);

			_targetScriptLanguagesInitialized = comboBoxTargetScripts.Visible = labelTargetScript.Visible =
				labelSourceScript.Visible = textBoxSourceScript.Visible = false;

			comboBoxTargetScripts.SelectedItem = textBoxSourceScript.Text = null;

			IsModified = true;
		}

		private void radioButtonTranslateWithTransliteration_CheckedChanged(object sender, EventArgs e)
		{
			if (!(sender as RadioButton).Checked)
				return; // means it was unchecked

			transductionSelected = TransductionType.TranslateWithTransliterate;
			ProcessType = (int)(ProcessTypeFlags.Translation | ProcessTypeFlags.Transliteration);

			comboBoxTargetLanguages.Visible = labelTargetLanguage.Visible = true;

			InitializeSourceAndTargetLanguages(initializeTargetLanguageAlso: true);

			InitializeSourceScript(comboBoxTargetLanguages.SelectedItem as string, null);

			comboBoxTargetScripts.Visible = labelTargetScript.Visible =
				labelSourceScript.Visible = textBoxSourceScript.Visible = true;

			_targetScriptLanguagesInitialized = false;	// so it's recalculated

			InitializeTargetScriptLanguages();

			IsModified = true;
		}

		private void radioButtonTransliterate_CheckedChanged(object sender, EventArgs e)
		{
			if (!(sender as RadioButton).Checked)
				return; // means it was unchecked

			transductionSelected = TransductionType.Transliterate;
			ProcessType = (int)ProcessTypeFlags.Transliteration;

			// for transliterate, we don't care about the target language
			_targetScriptLanguagesInitialized = comboBoxTargetLanguages.Visible = labelTargetLanguage.Visible = false;
			comboBoxTargetLanguages.SelectedItem = TargetLanguageNameMustBeConfigure;

			InitializeSourceAndTargetLanguages(initializeTargetLanguageAlso: false);

			comboBoxTargetScripts.Visible = labelTargetScript.Visible =
				labelSourceScript.Visible = textBoxSourceScript.Visible = true;

			InitializeSourceScript(comboBoxSourceLanguages.SelectedItem as string, null);
			InitializeTargetScriptLanguages();

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

			_targetScriptLanguagesInitialized = comboBoxTargetScripts.Visible = labelTargetScript.Visible =
				labelSourceScript.Visible = textBoxSourceScript.Visible = false;

			comboBoxTargetScripts.SelectedItem = textBoxSourceScript.Text = null;

			IsModified = true;
		}

		private void comboBoxTargetLanguages_SelectedIndexChanged(object sender, EventArgs e)
		{
			IsModified = true;  // modified even if we don't update script languages

			// we don't initialize the script language(s) based on the target language changing unless we're in one of the transliterate modes
			string selectedItem = (string)comboBoxTargetLanguages.SelectedItem;
			if ((TargetLanguageNameMustBeConfigure == selectedItem) ||
				(transductionSelected == TransductionType.Translate) ||
				(transductionSelected == TransductionType.DictionaryLookup))
			{
				return;
			}

			// reinitialize these so they get updated if/whenever the target language changes
			InitializeSourceScriptBasedOnComboBoxValuesAndTransductionType();

			_targetScriptLanguagesInitialized = false;
			InitializeTargetScriptLanguages();
		}

		private void InitializeSourceScriptBasedOnComboBoxValuesAndTransductionType()
		{
			string relevantSelectedItem, possibleTargetScript = null;
			if (transductionSelected == TransductionType.Transliterate)
			{
				relevantSelectedItem = (string)comboBoxSourceLanguages.SelectedItem;
				possibleTargetScript = (string)comboBoxTargetScripts.SelectedItem;
			}
			else if (transductionSelected == TransductionType.TranslateWithTransliterate)
				relevantSelectedItem = (string)comboBoxTargetLanguages.SelectedItem;
			else
				return;	// but only with a transliterate flavor

			InitializeSourceScript(relevantSelectedItem, possibleTargetScript);
		}

		private void comboBoxSourceLanguages_SelectedIndexChanged(object sender, EventArgs e)
		{
			IsModified = true;	// modified even if we don't update script languages

			// we don't initialize the script language(s) based on the source language changing unless we're in the Transliterate mode
			string selectedItem = (string)comboBoxSourceLanguages.SelectedItem;
			if ((SourceLanguageNameAutoDetect == selectedItem) ||
				(transductionSelected != TransductionType.Transliterate))
			{
				return;
			}

			// reinitialize these so they get updated if/whenever the target language changes
			_targetScriptLanguagesInitialized = false;

			InitializeSourceScript(comboBoxSourceLanguages.SelectedItem as string, comboBoxTargetScripts.SelectedItem as string);

			InitializeTargetScriptLanguages();
		}

		private void comboBoxTargetScripts_SelectedIndexChanged(object sender, EventArgs e)
		{
			if (transductionSelected == TransductionType.Transliterate)
			{
				// if we're doing transliterate, then if the target script is changed, it means we need to change the
				//	source script also. As of now, now translation
				InitializeSourceScript(comboBoxSourceLanguages.SelectedItem as string, comboBoxTargetScripts.SelectedItem as string);
			}

			IsModified = true;
		}

		private void buttonSetBingTranslateApiKey_Click(object sender, EventArgs e)
		{
			// only send the key if it's already the override key (so we don't expose ours)
			var dlg = new QueryForAzureKeyAndLocation(Properties.Settings.Default.AzureTranslatorKeyOverride, Properties.Settings.Default.AzureTranslatorRegion);
			if (dlg.ShowDialog() == DialogResult.Yes)
			{
				Properties.Settings.Default.AzureTranslatorKey = AzureTranslatorSubscriptionKey = dlg.AzureTranslatorKey;
				Properties.Settings.Default.AzureTranslatorRegion = AzureTranslatorLocation = dlg.AzureTranslatorLocation;
				Properties.Settings.Default.Save();
			}
		}
	}
}

