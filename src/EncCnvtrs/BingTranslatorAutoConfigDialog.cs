using System;
using System.Windows.Forms;
using System.IO;
using ECInterfaces;                     // for IEncConverter
using static SilEncConverters40.BingTranslatorEncConverter;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace SilEncConverters40
{
	//[CLSCompliantAttribute(false)]  // because of GeckoWebBrowser
	public partial class BingTranslatorAutoConfigDialog : AutoConfigDialog
	{
		private const string SourceLanguageNameAutoDetect = "Auto-Detect";
		private const string TargetLanguageNameMustBeConfigure = "Select Target Language";
		private const string TargetScriptNameMustBeConfigure = "Select Target Script";

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
										 out fromLanguage, out toLanguage,
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

				IsModified = false;
			}
			else
			{
				radioButtonTranslate.Checked = true;	// to trigger the loading of the combo boxes
			}

			comboBoxSourceLanguages.SelectedItem = fromLanguage;
			comboBoxTargetLanguages.SelectedItem = toLanguage;

			m_bInitialized = true;

            Util.DebugWriteLine(this, "END");
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
				MessageBox.Show(this, "The Target Language must be selected!", EncConverters.cstrCaption);
				return false;
			}

			var selectedFromLanguage = (string)comboBoxSourceLanguages.SelectedItem;
			if (selectedFromLanguage == SourceLanguageNameAutoDetect)
				selectedFromLanguage = null;

			// for TECkit, get the converter identifier from the Setup tab controls.
			ConverterIdentifier = String.Format("{0};{1};{2};{3};{4}",
				transductionSelected.ToString(),
				ExtractCode(selectedFromLanguage),
				ExtractCode(selectedToLanguage),
				ExtractCode((string)comboBoxSourceScripts.SelectedItem),
				ExtractCode((string)comboBoxTargetScripts.SelectedItem));

            return base.OnApply();
        }

		private Regex _regexExtractCode = new Regex(@"\(([a-zA-Z]{2,})\)");

		/// <summary>
		/// turn something like Hindi|हिन्दी (hi) into just 'hi'
		/// </summary>
		/// <param name="languageName"></param>
		/// <returns></returns>
		private string ExtractCode(string languageName)
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
				string defaultName;
				switch (transductionSelected)
				{
					case TransductionType.Translate:
						defaultName = $"{comboBoxSourceLanguages.SelectedItem} to {comboBoxTargetLanguages.SelectedItem}";
						break;
					case TransductionType.TranslateWithTransliterate:
						defaultName = $"{comboBoxSourceLanguages.SelectedItem} to {comboBoxTargetLanguages.SelectedItem} in {comboBoxTargetScripts.SelectedItem}";
						break;
					case TransductionType.Transliterate:
						defaultName = $"{comboBoxSourceLanguages.SelectedItem} in {comboBoxSourceScripts.SelectedItem} to {comboBoxTargetScripts.SelectedItem}";
						break;
					case TransductionType.DictionaryLookup:
						defaultName = $"Lookup of {comboBoxSourceLanguages.SelectedItem} to {comboBoxTargetLanguages.SelectedItem}";
						break;
					default:
						defaultName = ConverterIdentifier;
						break;
				};
				return defaultName;
			}
        }

        private void textBoxFileSpec_TextChanged(object sender, EventArgs e)
        {
            if (m_bInitialized) // but only do this after we've already initialized (we might have set it during m_bEditMode)
                IsModified = (((TextBox)sender).Text.Length > 0);
        }

		private bool _sourceLanguagesInitialized = false;
		private bool _targetLanguagesInitialized = false;
		private bool _sourceScriptLanguagesInitialized = false;
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
				comboBoxTargetLanguages.Items.Clear();
				comboBoxTargetLanguages.Items.Add(TargetLanguageNameMustBeConfigure);
				comboBoxTargetLanguages.Items.AddRange(translationLanguagesPossible);
			}
		}

		private void InitializeSourceScriptLanguages()
		{
			if (_sourceScriptLanguagesInitialized)
				return;

			// the scripts we can transliterate *from* are dependent on the source language.
			var selectedSourceLanguage = (string)comboBoxSourceLanguages.SelectedItem;
			if (selectedSourceLanguage == SourceLanguageNameAutoDetect)
			{
				// if the source language name hasn't been configured yet, the we don't want to allow
				//	selection (or loading) of the comboboxes yet
				comboBoxSourceScripts.Enabled = false;
				return;
			}
			else
				comboBoxSourceScripts.Enabled = true;

			_sourceScriptLanguagesInitialized = true;

			var possibleSourceScripts = transliterationsPossible.FirstOrDefault(t => t.ToString().Contains(selectedSourceLanguage))?
																.ScriptsSupported
																.Select(s => s.ToString())
																.ToArray();

			comboBoxSourceScripts.Items.Clear();
			comboBoxSourceScripts.Items.AddRange(possibleSourceScripts);
			comboBoxSourceScripts.SelectedIndex = 0;
		}

		private void InitializeTargetScriptLanguages()
		{
			if (_targetScriptLanguagesInitialized)
				return;

			// the scripts we can transliterate *from* are dependent on the target language and the source script
			//  (or just the target language if doing TranslateWithTransliterate)
			var selectedTargetLanguage = (string)comboBoxTargetLanguages.SelectedItem;
			if (selectedTargetLanguage == TargetLanguageNameMustBeConfigure)
			{
				// if the target language name hasn't been configured yet, the we don't want to allow
				//	selection (or even loading) of the script combo boxes yet
				comboBoxTargetScripts.Enabled = false;
				return;
			}

			comboBoxTargetScripts.Enabled = true;

			// the scripts we can transliterate *to* are dependent on the source script also (for plain Transliterate)
			string[] possibleTargetScripts;
			switch (transductionSelected)
			{
				case TransductionType.Transliterate:
					var selectedSourceScriptLanguage = (string)comboBoxSourceScripts.SelectedItem;
					comboBoxTargetScripts.Enabled = true;
					possibleTargetScripts = transliterationsPossible.FirstOrDefault(t => t.ToString().Contains(selectedTargetLanguage))?
																	.ScriptsSupported
																	.FirstOrDefault(s => s.ToString().Contains(selectedSourceScriptLanguage))?
																	.ToScripts
																	.Select(s => s.ToString())
																	.ToArray();
					break;
				case TransductionType.TranslateWithTransliterate:
					possibleTargetScripts = transliterationsPossible.FirstOrDefault(t => t.ToString().Contains(selectedTargetLanguage))?
																	.ScriptsSupported
																	.SelectMany(s => s.ToScripts.Select(ts => ts.ToString()))
																	.ToArray();

					// TODO: probably want to remove the 1st entry (no need to do transliteration to self script)
					break;
				default:
					System.Diagnostics.Debug.Fail($"Not expecting a transductionSelected of {transductionSelected} here!");
					return;
			}

			_targetScriptLanguagesInitialized = true;

			comboBoxTargetScripts.Items.Clear();
			comboBoxTargetScripts.Items.AddRange(possibleTargetScripts);
			comboBoxTargetScripts.SelectedIndex = possibleTargetScripts.Length - 1;
		}

		private void radioButtonTranslate_CheckedChanged(object sender, EventArgs e)
		{
			if (!(sender as RadioButton).Checked)
				return; // means it was unchecked

			comboBoxTargetLanguages.Visible = labelTargetLanguage.Visible = true;

			InitializeSourceAndTargetLanguages(initializeTargetLanguageAlso: true);

			comboBoxSourceScripts.Visible = comboBoxTargetScripts.Visible =
				labelSourceScript.Visible = labelTargetScript.Visible = false;

			comboBoxSourceScripts.SelectedItem = comboBoxTargetScripts.SelectedItem = null;

			transductionSelected = TransductionType.Translate;
			IsModified = true;
		}

		private void radioButtonTranslateWithTransliteration_CheckedChanged(object sender, EventArgs e)
		{
			if (!(sender as RadioButton).Checked)
				return; // means it was unchecked

			comboBoxTargetLanguages.Visible = labelTargetLanguage.Visible = true;

			InitializeSourceAndTargetLanguages(initializeTargetLanguageAlso: true);

			comboBoxTargetScripts.Visible = labelTargetScript.Visible = true;

			InitializeTargetScriptLanguages();

			comboBoxSourceScripts.Visible = labelSourceScript.Visible = false;
			comboBoxSourceScripts.SelectedItem = null;

			transductionSelected = TransductionType.TranslateWithTransliterate;
			IsModified = true;
		}

		private void radioButtonTransliterate_CheckedChanged(object sender, EventArgs e)
		{
			if (!(sender as RadioButton).Checked)
				return; // means it was unchecked

			// for transliterate, we don't care about the target language
			comboBoxTargetLanguages.Visible = labelTargetLanguage.Visible = false;

			InitializeSourceAndTargetLanguages(initializeTargetLanguageAlso: false);

			comboBoxTargetScripts.Visible = comboBoxSourceScripts.Visible = 
				labelTargetScript.Visible = labelSourceScript.Visible = true;
			comboBoxSourceScripts.Enabled = false;	// not editable

			InitializeSourceScriptLanguages();
			InitializeTargetScriptLanguages();

			transductionSelected = TransductionType.Transliterate;
			IsModified = true;
		}

		private void radioButtonDictionaryLookup_CheckedChanged(object sender, EventArgs e)
		{
			if (!(sender as RadioButton).Checked)
				return; // means it was unchecked

			InitializeSourceAndTargetLanguages(initializeTargetLanguageAlso: true);

			comboBoxSourceScripts.Visible = comboBoxTargetScripts.Visible =
				labelSourceScript.Visible = labelTargetScript.Visible = false;

			comboBoxSourceScripts.SelectedItem = comboBoxTargetScripts.SelectedItem = null;

			transductionSelected = TransductionType.DictionaryLookup;
			IsModified = true;
		}

		private void comboBoxTargetLanguages_SelectedIndexChanged(object sender, EventArgs e)
		{
			// we don't initialize the script language(s) unless a target language is selected and we're in one of the transliterate modes
			string selectedItem = (string)comboBoxTargetLanguages.SelectedItem;
			if ((TargetLanguageNameMustBeConfigure == selectedItem) ||
				(transductionSelected == TransductionType.Translate) ||
				(transductionSelected == TransductionType.DictionaryLookup))
			{
				return;
			}

			// reinitialize these so they get updated if/whenever the target language changes
			_sourceScriptLanguagesInitialized = _targetScriptLanguagesInitialized = false;
			if (transductionSelected == TransductionType.Transliterate)
			{
				InitializeSourceScriptLanguages();
			}
			else
			{
				InitializeTargetScriptLanguages();
			}

			IsModified = true;
		}

		private void comboBoxSourceLanguages_SelectedIndexChanged(object sender, EventArgs e)
		{
			IsModified = true;
		}

		private void comboBoxSourceScripts_SelectedIndexChanged(object sender, EventArgs e)
		{
			_targetScriptLanguagesInitialized = false;
			InitializeTargetScriptLanguages();
			IsModified = true;
		}

		private void comboBoxTargetScripts_SelectedIndexChanged(object sender, EventArgs e)
		{
			IsModified = true;
		}
	}
}

