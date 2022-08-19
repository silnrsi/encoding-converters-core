using ECInterfaces;
using SilEncConverters40;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Windows.Forms;

namespace BackTranslationHelper
{
    public partial class BackTranslationHelperCtrl : UserControl
    {
        public int MaxPossibleTargetTranslations = 3;  // to add more, you have to add new lines like the one starting at row 2

        #region Member variables
        // the form in which this UserControl is embedded will initialize these
        public IBackTranslationHelperDataSource BackTranslationHelperDataSource;
        public List<IEncConverter> TheTranslators = new List<IEncConverter>();

        public BackTranslationHelperModel _model;
        #endregion

        public BackTranslationHelperCtrl()
        {
            InitializeComponent();
        }

        public void Initialize(bool displayExistingTargetTranslation)
        {
            BackTranslationHelperDataSource.SetDataUpdateProc(UpdateData);

            // get the last used converter names from settings 
            InitializeTheTranslators();

            tableLayoutPanel.SuspendLayout();
            SuspendLayout();

            hideColumn1LabelsToolStripMenuItem.Checked = Properties.Settings.Default.HideLabels;
            InitializeLabelHiding();

            labelSourceData.Font = BackTranslationHelperDataSource.SourceLanguageFont;
            if (displayExistingTargetTranslation)
            {
                labelForExistingTargetData.Visible = !hideColumn1LabelsToolStripMenuItem.Checked;
                labelTargetTextExisting.Visible = buttonFillExistingTargetText.Visible = true;
                labelTargetTextExisting.Font = BackTranslationHelperDataSource.TargetLanguageFont;
            }
            else
                labelTargetTextExisting.Visible = labelForExistingTargetData.Visible = buttonFillExistingTargetText.Visible = false;

            textBoxTargetBackTranslation.Font = BackTranslationHelperDataSource.TargetLanguageFont;

            // we're either showing the target translated suggestion in a textbox (if there's only 1 converter)
            //  or in labels above it to choose from (if there are more than one converter)
            var labelsPossibleTargetTranslations = tableLayoutPanel.Controls.OfType<Label>().Where(l => l.Name.Contains("labelPossibleTargetTranslation")).ToList();
            var buttonsFillTargetOption = tableLayoutPanel.Controls.OfType<Button>().Where(b => b.Name.Contains("buttonFillTargetTextOption")).ToList();
            var numOfTranslators = TheTranslators.Count;

            // if there's only one, then we don't need to display the 'possible' translations to start with.
			// NB: but only if we're not displaying any pre-existing target translations. If we are (i.e. Paratext),
			//	then we need to display even the one as an option, bkz the editable textbox will contain the
			//	existing translation, which if it's not correct, we want to be able to fill it from the converted
			//	value (which will be in the 1st targetoption box)
            var i = 0;
            if ((numOfTranslators == 1) && !displayExistingTargetTranslation)
			{
                // hide them all
                for (; i < MaxPossibleTargetTranslations; i++)
                {
                    var label = labelsPossibleTargetTranslations[i];
                    var button = buttonsFillTargetOption[i];
                    label.Visible = button.Visible = false;
                }
                labelForTargetDataOptions.Visible = false;
            }
            else
            {
                labelForTargetDataOptions.Visible = !Properties.Settings.Default.HideLabels;

				// set up mnemonics for the buttons to make it easier to trigger
				var mnemonicChar = 1;
				if (displayExistingTargetTranslation)
					buttonFillExistingTargetText.Text = $" &{mnemonicChar++}";	// so that Alt+1 will trigger the button (and space so the text won't show over the icon)

                for (; i < numOfTranslators; i++)
                {
                    var label = labelsPossibleTargetTranslations[i];
                    var button = buttonsFillTargetOption[i];
					button.Text = $" &{mnemonicChar++}";
					label.Visible = button.Visible = true;
                    label.Font = labelTargetTextExisting.Font;
                    toolTip.SetToolTip(label, $"This is the translation from the {TheTranslators[i].Name} Translator");
                }

                for (; i < MaxPossibleTargetTranslations; i++)
                {
                    var label = labelsPossibleTargetTranslations[i];
                    var button = buttonsFillTargetOption[i];
                    label.Visible = button.Visible = false;
                }
            }

            tableLayoutPanel.ResumeLayout(false);
            tableLayoutPanel.PerformLayout();
            ResumeLayout(false);
            PerformLayout();
        }

        private void InitializeTheTranslators()
        {
            if (Properties.Settings.Default.MapProjectNameToEcTranslators == null)
                Properties.Settings.Default.MapProjectNameToEcTranslators = new StringCollection();
            var mapProjectNameToEcTranslators = SettingToDictionary(Properties.Settings.Default.MapProjectNameToEcTranslators);
            var projectName = BackTranslationHelperDataSource.ProjectName;
            if (mapProjectNameToEcTranslators.TryGetValue(projectName, out List<string> translatorNames))
            {
                foreach (var translatorName in translatorNames)
                    if (!TheTranslators.Any(t => t.Name == translatorName) && DirectableEncConverter.EncConverters.ContainsKey(translatorName))
                        TheTranslators.Add(DirectableEncConverter.EncConverters[translatorName]);
            }

            // see how many converters are configured (if none, then query for one)
            if (!TheTranslators.Any())
            {
                var aTranslator = QueryTranslator();
				if (aTranslator == null)
					return;

                var theTranslator = aTranslator.GetEncConverter;
                TheTranslators.Add(theTranslator);

                // save it in settings for this project, so we can load it automatically next time
                translatorNames = new List<string> { theTranslator.Name };
                mapProjectNameToEcTranslators.Add(projectName, translatorNames);
                Properties.Settings.Default.MapProjectNameToEcTranslators = SettingFromDictionary(mapProjectNameToEcTranslators);
                Properties.Settings.Default.Save();
            }
        }

        public DirectableEncConverter QueryTranslator()
        {
            DirectableEncConverter theEc = null;
            try
            {
                var aEc = DirectableEncConverter.EncConverters.AutoSelectWithTitle(ConvType.Unicode_to_Unicode,
                                                                                   "Select the Encoding Converter to do the Translation (e.g. a Bing or DeepL translator)");
                if (aEc != null)
                    theEc = new DirectableEncConverter(aEc);
            }
            catch (Exception)
            {
            }

            return theEc;
        }

        public List<TargetPossible> NewTargetTexts
        {
            get
            {
                // always choose the one in the text box
                var newTargetTexts = new List<TargetPossible> 
                { 
                    new TargetPossible 
                    { 
                        TargetData = textBoxTargetBackTranslation.Text,
                        PossibleIndex = 0,
                        TranslatorName = TheTranslators.FirstOrDefault()?.Name
                    } 
                };
                return newTargetTexts;
            }

            set
            {
				var labelsPossibleTargetTranslations = tableLayoutPanel.Controls.OfType<Label>().Where(l => l.Name.Contains("labelPossibleTargetTranslation")).ToList();
				System.Diagnostics.Debug.Assert(value.Count == TheTranslators.Count);
				System.Diagnostics.Debug.Assert(labelsPossibleTargetTranslations.Where(l => l.Visible).Take(value.Count).ToList().All(l => l.Visible));

				for (var i = 0; i < TheTranslators.Count; i++)
				{
					var label = labelsPossibleTargetTranslations[i];
					if (label.Visible)
						label.Text = value[i].TargetData;
				}

				// now update the actual editable box w/ the first possibility (if it isn't already filled,
				// which would have come from the original target from the target project)
				if (String.IsNullOrEmpty(textBoxTargetBackTranslation.Text))
				{
					var targetPossible = value.FirstOrDefault();
					textBoxTargetBackTranslation.Text = targetPossible?.TargetData;
				}
			}
        }

        #region Event handlers
        public void GetNewData(ref BackTranslationHelperModel model)
        {
            if (model == null)
                _model = BackTranslationHelperDataSource.Model;

            else if (_model?.SourceData != model.SourceData)
                _model = model;

            for (var i = _model.TargetsPossible.Count; i < TheTranslators.Count; i++)
            {
                var theTranslator = TheTranslators[i];
                var translatedText = theTranslator.Convert(_model.SourceData);
                _model.TargetsPossible.Add(new TargetPossible { TargetData = translatedText, PossibleIndex = i, TranslatorName = theTranslator.Name });
            }

            model = _model;
        }

        public void UpdateData(BackTranslationHelperModel model)
        {
            labelSourceData.Text = model.SourceData;
			textBoxTargetBackTranslation.Text = model.TargetData;   // may be null, in which case, setting NewTargetTexts below will fill it with the 1st translation

			// if we're keeping track of what was originally in the Target Project (i.e. Paratext usage), then put the current value in the label for
			//	the existing target field (just in case the user starts to edit or choose one of the other possibilities and then wants to revert it
			//	(it has a fill button also).
			if (_model.TargetDataPreExisting)
				labelTargetTextExisting.Text = model.TargetData;

            // some clients (i.e. Word) only pass 1 translated target text (bkz it only knows about 1 EncConverter/Translator)
            //  if we have fewer than the number of possible target translations (i.e. we added 1 or more addl Translators),
            //  then convert the missing ones to add to this collection
            var numOfTranslators = TheTranslators.Count;
            for (var i = model.TargetsPossible.Count; i < numOfTranslators; i++)
            {
                var theTranslator = TheTranslators[i];
                var translatedText = theTranslator.Convert(model.SourceData);
                model.TargetsPossible.Add(new TargetPossible { TargetData = translatedText, PossibleIndex = i, TranslatorName = theTranslator.Name });
            }

            NewTargetTexts = model.TargetsPossible;
        }

        #endregion

        #region Private helper methods

        #endregion

        private void buttonWriteTextToTarget_Click(object sender, System.EventArgs e)
        {
            BackTranslationHelperDataSource.ButtonPressed(ButtonPressed.WriteToTarget);
            BackTranslationHelperDataSource.Log($"change target text from '{labelTargetTextExisting.Text}' to '{textBoxTargetBackTranslation.Text}'");
            BackTranslationHelperDataSource.WriteToTarget(textBoxTargetBackTranslation.Text);
        }

        private void buttonCopyToClipboard_Click(object sender, System.EventArgs e)
        {
            Clipboard.SetDataObject(textBoxTargetBackTranslation.Text);
            BackTranslationHelperDataSource.ButtonPressed(ButtonPressed.Copy);
        }

        private void buttonNextSection_Click(object sender, System.EventArgs e)
        {
            if (BackTranslationHelperDataSource == null)
            {
                // see if we can manually trigger the change of verse number in the host
                return;
            }
            BackTranslationHelperDataSource.ButtonPressed(ButtonPressed.MoveToNext);
            var existingTargetText = labelTargetTextExisting.Text;
            var newTargetText = textBoxTargetBackTranslation.Text;
            var modified = existingTargetText != newTargetText;

            if (!autoSaveToolStripMenuItem.Checked)
            {
                if (modified)
                {
                    var res = MessageBox.Show($"Do you want to save/write out the translated text before moving to the next portion?", BackTranslationHelperDataSource.ProjectName, MessageBoxButtons.YesNoCancel);
                    if (res == DialogResult.Cancel)
                    {
                        BackTranslationHelperDataSource.Cancel();
                        return;
                    }
                    if (res == DialogResult.No)
                    {
                        BackTranslationHelperDataSource.MoveToNext();
                        return;
                    }
                }
            }

            if (modified)
            {
                BackTranslationHelperDataSource.Log($"change target text from '{existingTargetText}' to '{newTargetText}'");
                BackTranslationHelperDataSource.WriteToTarget(newTargetText);
            }

            BackTranslationHelperDataSource.MoveToNext();
        }

        private void changeEncConverterToolStripMenuItem_Click(object sender, EventArgs e)
        {
            var dlg = new TranslatorListForm(TheTranslators);
            if (dlg.ShowDialog() == DialogResult.OK)
            {
                TheTranslators.RemoveAll(t => t.Name == dlg.SelectedDisplayName);

                var mapProjectNameToEcTranslators = SettingToDictionary(Properties.Settings.Default.MapProjectNameToEcTranslators);
                var projectName = BackTranslationHelperDataSource.ProjectName;
                if (mapProjectNameToEcTranslators.TryGetValue(projectName, out List<string> translatorNames))
                {
                    translatorNames.RemoveAll(t => t == dlg.SelectedDisplayName);
                    Properties.Settings.Default.MapProjectNameToEcTranslators = SettingFromDictionary(mapProjectNameToEcTranslators);
                    Properties.Settings.Default.Save();
                }

				System.Diagnostics.Debug.Assert(_model != null);
				Initialize(_model.TargetDataPreExisting);
				UpdateData(_model);
			}
		}

        private void addEncConverterToolStripMenuItem_Click(object sender, EventArgs e)
        {
            var newTranslator = QueryTranslator();
			if (newTranslator == null)
				return;

			var theTranslator = newTranslator.GetEncConverter;
            var projectName = BackTranslationHelperDataSource.ProjectName;
            if (TheTranslators.Any(t => t.Name == theTranslator.Name))
            {
                MessageBox.Show($"You've already added the {theTranslator.Name} Translator/EncConverter", projectName);
                return;
            }

            TheTranslators.Add(theTranslator);
            var mapProjectNameToEcTranslators = SettingToDictionary(Properties.Settings.Default.MapProjectNameToEcTranslators);
            if (!mapProjectNameToEcTranslators.TryGetValue(projectName, out List<string> translatorNames))
            {
                translatorNames = new List<string>();
                mapProjectNameToEcTranslators.Add(projectName, translatorNames);
            }

            if (!translatorNames.Any(n => n == theTranslator.Name))
            {
                translatorNames.Add(theTranslator.Name);
                Properties.Settings.Default.MapProjectNameToEcTranslators = SettingFromDictionary(mapProjectNameToEcTranslators);
                Properties.Settings.Default.Save();
            }

            System.Diagnostics.Debug.Assert(_model != null);
            Initialize(_model.TargetDataPreExisting);
            UpdateData(_model);
        }

        private void buttonFillExistingTargetText_Click(object sender, EventArgs e)
        {
            textBoxTargetBackTranslation.Text = labelTargetTextExisting.Text;
        }

        private void buttonFillTargetTextOption1_Click(object sender, EventArgs e)
        {
            textBoxTargetBackTranslation.Text = labelPossibleTargetTranslation1.Text;
        }

        private void buttonFillTargetTextOption2_Click(object sender, EventArgs e)
        {
            textBoxTargetBackTranslation.Text = labelPossibleTargetTranslation2.Text;
        }

        private void buttonFillTargetTextOption3_Click(object sender, EventArgs e)
        {
            textBoxTargetBackTranslation.Text = labelPossibleTargetTranslation3.Text;
        }

        private void textBoxTargetBackTranslation_Enter(object sender, EventArgs e)
        {
            BackTranslationHelperDataSource?.ActivateKeyboard();
        }

        private void InitializeLabelHiding()
        {
            labelForSourceData.Visible =
                labelForExistingTargetData.Visible =
                labelForTargetDataOptions.Visible =
                labelForTargetTranslation.Visible = !Properties.Settings.Default.HideLabels;
        }

        private void hideColumn1LabelsToolStripMenuItem_CheckStateChanged(object sender, EventArgs e)
        {
            var newCheckState = hideColumn1LabelsToolStripMenuItem.Checked;
            if (newCheckState != Properties.Settings.Default.HideLabels)
            {
                Properties.Settings.Default.HideLabels = newCheckState;
                Properties.Settings.Default.Save();
                if (_model != null)
                {
                    Initialize(labelTargetTextExisting.Visible);
                    UpdateData(_model);
                }
            }
        }

        protected const char chNeverUsedChar = '\u0009';  // add to words we replace, so we don't process them again

        public static Dictionary<string, List<string>> SettingToDictionary(StringCollection data)
        {
            var map = new Dictionary<string, List<string>>();
            for (var i = 0; i < data.Count; i += 2)
            {
                map.Add(data[i], data[i + 1].Split(new[] { chNeverUsedChar }, StringSplitOptions.RemoveEmptyEntries).ToList());
            }

            return map;
        }

        public static StringCollection SettingFromDictionary(Dictionary<string, List<string>> map)
        {
            var lst = new StringCollection();
            foreach (KeyValuePair<string, List<string>> kvp in map)
            {
                lst.Add(kvp.Key);
                lst.Add(String.Join(chNeverUsedChar.ToString(), kvp.Value));
            }

            return lst;
        }

		private void buttonClose_Click(object sender, EventArgs e)
		{
			BackTranslationHelperDataSource?.ButtonPressed(ButtonPressed.Close);
			BackTranslationHelperDataSource?.Cancel();
		}

		private void buttonSkip_Click(object sender, EventArgs e)
		{
			BackTranslationHelperDataSource?.ButtonPressed(ButtonPressed.Skip);
			BackTranslationHelperDataSource?.MoveToNext();
		}

		private void sourceTextToolStripMenuItem_Click(object sender, EventArgs e)
		{
			if (Properties.Settings.Default.MapProjectNameToSourceFontOverride == null)
				Properties.Settings.Default.MapProjectNameToSourceFontOverride = new StringCollection();

			fontDialog.Font = BackTranslationHelperDataSource.SourceLanguageFont;
			var mapProjectNameToSourceFontOverride = SettingToDictionary(Properties.Settings.Default.MapProjectNameToSourceFontOverride);
			var projectName = BackTranslationHelperDataSource.ProjectName;
			if (mapProjectNameToSourceFontOverride.TryGetValue(projectName, out List<string> fontOverride))
			{
				fontDialog.Font = new System.Drawing.Font(fontOverride[0], float.Parse(fontOverride[1]));
			}

			if (fontDialog.ShowDialog() == DialogResult.OK)
			{
				fontOverride = new List<string>
				{
					fontDialog.Font.Name,
					fontDialog.Font.SizeInPoints.ToString()
				};

				labelSourceData.Font = fontDialog.Font;
				mapProjectNameToSourceFontOverride.Add(projectName, fontOverride);
				Properties.Settings.Default.MapProjectNameToSourceFontOverride = SettingFromDictionary(mapProjectNameToSourceFontOverride);
				Properties.Settings.Default.Save();
			}

		}
	}
}
