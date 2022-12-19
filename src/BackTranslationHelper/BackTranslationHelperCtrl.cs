using ECInterfaces;
using SilEncConverters40;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Reflection;
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

			this.MouseWheel += new MouseEventHandler(this.UserControl_MouseWheel);
			this.textBoxTargetBackTranslation.MouseWheel += new MouseEventHandler(this.TargetBackTranslation_MouseWheel);

		}

		private void TargetBackTranslation_MouseWheel(object sender, MouseEventArgs e)
		{
			UserControl_MouseWheel(sender, e);
		}

		private void UserControl_MouseWheel(object sender, MouseEventArgs e)
		{
			var keyToSend = "{DOWN}";
			if (e.Delta > 0)
				keyToSend = "{UP}";
			BroadCastKey(keyToSend);
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

			var nRowStyleOffset = 1;    // the offset to the row we're dealing w/ below

			var projectName = BackTranslationHelperDataSource.ProjectName;
			textBoxSourceData.Font = GetSourceLanguageFontForProject(projectName);
			textBoxSourceData.RightToLeft = GetSourceLanguageRightToLeftForProject(projectName);
			var targetLanguageFont = GetTargetLanguageFontForProject(projectName);
			var targetLanguageRightToLeft = GetTargetLanguageRightToLeftForProject(projectName);
			if (displayExistingTargetTranslation)
			{
				var rowStyle = tableLayoutPanel.RowStyles[nRowStyleOffset++];
				rowStyle.SizeType = SizeType.Percent;   // gives it real estate
				rowStyle.Height = 20F;
				labelForExistingTargetData.Visible = !hideColumn1LabelsToolStripMenuItem.Checked;
				textBoxTargetTextExisting.Visible = buttonFillExistingTargetText.Visible = true;
				textBoxTargetTextExisting.Font = targetLanguageFont;
				textBoxTargetTextExisting.RightToLeft = targetLanguageRightToLeft;
			}
			else
			{
				tableLayoutPanel.RowStyles[nRowStyleOffset++].SizeType = SizeType.AutoSize; // makes it disappear
				textBoxTargetTextExisting.Visible = labelForExistingTargetData.Visible = buttonFillExistingTargetText.Visible = false;
			}

			textBoxTargetBackTranslation.Font = targetLanguageFont;
			textBoxTargetBackTranslation.RightToLeft = targetLanguageRightToLeft;

			// we're either showing the target translated suggestion in a textbox (if there's only 1 converter)
			//  or in readonly textboxes (so they can have scroll bars) above it to choose from (if there are more than one converter)
			var textBoxesPossibleTargetTranslations = tableLayoutPanel.Controls.OfType<TextBox>().Where(l => l.Name.Contains("textBoxPossibleTargetTranslation")).ToList();
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
					var textBox = textBoxesPossibleTargetTranslations[i];
					var button = buttonsFillTargetOption[i];
					textBox.Visible = button.Visible = false;
					tableLayoutPanel.RowStyles[nRowStyleOffset + i].SizeType = SizeType.AutoSize;
				}
				labelForTargetDataOptions.Visible = false;
			}
			else
			{
				labelForTargetDataOptions.Visible = !Properties.Settings.Default.HideLabels;

				// set up mnemonics for the buttons to make it easier to trigger
				var mnemonicChar = 1;
				if (displayExistingTargetTranslation)
					buttonFillExistingTargetText.Text = $" &{mnemonicChar++}";  // so that Alt+1 will trigger the button (and space so the text won't show over the icon)

				for (; i < numOfTranslators; i++)
				{
					var textBox = textBoxesPossibleTargetTranslations[i];
					var button = buttonsFillTargetOption[i];
					button.Text = $" &{mnemonicChar++}";
					textBox.Visible = button.Visible = true;
					textBox.Font = textBoxTargetTextExisting.Font;
					textBox.RightToLeft = targetLanguageRightToLeft;
					var rowStyle = tableLayoutPanel.RowStyles[nRowStyleOffset + i];
					rowStyle.SizeType = SizeType.Percent;   // gives it real estate
					rowStyle.Height = 20F;
					toolTip.SetToolTip(textBox, $"This is the translation from the {TheTranslators[i].Name} Translator");
				}

				for (; i < MaxPossibleTargetTranslations; i++)
				{
					var textBox = textBoxesPossibleTargetTranslations[i];
					var button = buttonsFillTargetOption[i];
					textBox.Visible = button.Visible = false;
					tableLayoutPanel.RowStyles[nRowStyleOffset + i].SizeType = SizeType.AutoSize;
				}
			}

			tableLayoutPanel.ResumeLayout(false);
			tableLayoutPanel.PerformLayout();
			ResumeLayout(false);
			PerformLayout();
		}

		private RightToLeft GetSourceLanguageRightToLeftForProject(string projectName)
		{
			var languageRightToLeft = BackTranslationHelperDataSource.SourceLanguageRightToLeft;
			if (Properties.Settings.Default.MapProjectNameToSourceRtLOverride != null)
			{
				var rtlOverrides = SettingToDictionary(Properties.Settings.Default.MapProjectNameToSourceRtLOverride);
				if (rtlOverrides.TryGetValue(projectName, out List<string> rtlOverride))
				{
					languageRightToLeft = rtlOverride[0] == "true";
				}
			}
			return languageRightToLeft ? RightToLeft.Yes : RightToLeft.No;
		}

		private RightToLeft GetTargetLanguageRightToLeftForProject(string projectName)
		{
			var languageRightToLeft = BackTranslationHelperDataSource.TargetLanguageRightToLeft;
			if (Properties.Settings.Default.MapProjectNameToTargetRtLOverride != null)
			{
				var rtlOverrides = SettingToDictionary(Properties.Settings.Default.MapProjectNameToTargetRtLOverride);
				if (rtlOverrides.TryGetValue(projectName, out List<string> rtlOverride))
				{
					languageRightToLeft = rtlOverride[0] == "true";
				}
			}
			return languageRightToLeft ? RightToLeft.Yes : RightToLeft.No;
		}

		private Font GetSourceLanguageFontForProject(string projectName)
		{
			// see if there's an override
			if (Properties.Settings.Default.MapProjectNameToSourceFontOverride != null)
			{
				var fontOverrides = SettingToDictionary(Properties.Settings.Default.MapProjectNameToSourceFontOverride);
				if (fontOverrides.TryGetValue(projectName, out List<string> fontOverride))
				{
					return new System.Drawing.Font(fontOverride[0], float.Parse(fontOverride[1]));
				}
			}
			return BackTranslationHelperDataSource.SourceLanguageFont;
		}

		private Font GetTargetLanguageFontForProject(string projectName)
		{
			// see if there's an override
			if (Properties.Settings.Default.MapProjectNameToTargetFontOverride != null)
			{
				var fontOverrides = SettingToDictionary(Properties.Settings.Default.MapProjectNameToTargetFontOverride);
				if (fontOverrides.TryGetValue(projectName, out List<string> fontOverride))
				{
					return new System.Drawing.Font(fontOverride[0], float.Parse(fontOverride[1]));
				}
			}
			return BackTranslationHelperDataSource.TargetLanguageFont;
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
				if (mapProjectNameToEcTranslators.ContainsKey(projectName))
					mapProjectNameToEcTranslators.Remove(projectName);
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
				var textBoxesPossibleTargetTranslations = tableLayoutPanel.Controls.OfType<TextBox>().Where(l => l.Name.Contains("textBoxPossibleTargetTranslation")).ToList();
				System.Diagnostics.Debug.Assert(value.Count == TheTranslators.Count);
				System.Diagnostics.Debug.Assert(textBoxesPossibleTargetTranslations.Where(l => l.Visible).Take(value.Count).ToList().All(l => l.Visible));

				for (var i = 0; i < TheTranslators.Count; i++)
				{
					var textBox = textBoxesPossibleTargetTranslations[i];
					if (textBox.Visible)
						textBox.Text = value[i].TargetData;
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
            textBoxSourceData.Text = model.SourceData;
			textBoxTargetBackTranslation.Text = model.TargetData;   // may be null, in which case, setting NewTargetTexts below will fill it with the 1st translation

			// if we're keeping track of what was originally in the Target Project (i.e. Paratext usage), then put the current value in the label for
			//	the existing target field (just in case the user starts to edit or choose one of the other possibilities and then wants to revert it
			//	(it has a fill button also).
			textBoxTargetTextExisting.Text = model.TargetDataPreExisting;	// would be null if we're not using it

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

        private void ButtonWriteTextToTarget_Click(object sender, System.EventArgs e)
        {
            BackTranslationHelperDataSource.ButtonPressed(ButtonPressed.WriteToTarget);
            BackTranslationHelperDataSource.Log($"change target text from '{textBoxTargetTextExisting.Text}' to '{textBoxTargetBackTranslation.Text}'");
            BackTranslationHelperDataSource.WriteToTarget(textBoxTargetBackTranslation.Text);
        }

        private void ButtonCopyToClipboard_Click(object sender, System.EventArgs e)
        {
            Clipboard.SetDataObject(textBoxTargetBackTranslation.Text);
            BackTranslationHelperDataSource.ButtonPressed(ButtonPressed.Copy);
        }

        private void ButtonNextSection_Click(object sender, System.EventArgs e)
        {
            if (BackTranslationHelperDataSource == null)
            {
                // see if we can manually trigger the change of verse number in the host
                return;
            }
            BackTranslationHelperDataSource.ButtonPressed(ButtonPressed.MoveToNext);
            var existingTargetText = textBoxTargetTextExisting.Text;
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

        private void ChangeEncConverterToolStripMenuItem_Click(object sender, EventArgs e)
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
				Initialize(!String.IsNullOrEmpty(_model.TargetDataPreExisting));
				UpdateData(_model);
			}
		}

        private void AddEncConverterToolStripMenuItem_Click(object sender, EventArgs e)
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
            Initialize(!String.IsNullOrEmpty(_model.TargetDataPreExisting));
            UpdateData(_model);
        }

        private void ButtonFillExistingTargetText_Click(object sender, EventArgs e)
        {
            textBoxTargetBackTranslation.Text = textBoxTargetTextExisting.Text;
        }

        private void ButtonFillTargetTextOption1_Click(object sender, EventArgs e)
        {
            textBoxTargetBackTranslation.Text = textBoxPossibleTargetTranslation1.Text;
        }

        private void ButtonFillTargetTextOption2_Click(object sender, EventArgs e)
        {
            textBoxTargetBackTranslation.Text = textBoxPossibleTargetTranslation2.Text;
        }

        private void ButtonFillTargetTextOption3_Click(object sender, EventArgs e)
        {
            textBoxTargetBackTranslation.Text = textBoxPossibleTargetTranslation3.Text;
        }

        private void TextBoxTargetBackTranslation_Enter(object sender, EventArgs e)
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

        private void HideColumn1LabelsToolStripMenuItem_CheckStateChanged(object sender, EventArgs e)
        {
            var newCheckState = hideColumn1LabelsToolStripMenuItem.Checked;
            if (newCheckState != Properties.Settings.Default.HideLabels)
            {
                Properties.Settings.Default.HideLabels = newCheckState;
                Properties.Settings.Default.Save();
                if (_model != null)
                {
                    Initialize(textBoxTargetTextExisting.Visible);
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

		private void ButtonClose_Click(object sender, EventArgs e)
		{
			BackTranslationHelperDataSource?.ButtonPressed(ButtonPressed.Close);
			BackTranslationHelperDataSource?.Cancel();
		}

		private void ButtonSkip_Click(object sender, EventArgs e)
		{
			BackTranslationHelperDataSource?.ButtonPressed(ButtonPressed.Skip);
			BackTranslationHelperDataSource?.MoveToNext();
		}

		private void SourceTextToolStripMenuItem_Click(object sender, EventArgs e)
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

				textBoxSourceData.Font = fontDialog.Font;
				mapProjectNameToSourceFontOverride.Add(projectName, fontOverride);
				Properties.Settings.Default.MapProjectNameToSourceFontOverride = SettingFromDictionary(mapProjectNameToSourceFontOverride);
				Properties.Settings.Default.Save();
			}
		}

		private void TargetTextToolStripMenuItem_Click(object sender, EventArgs e)
		{
			if (Properties.Settings.Default.MapProjectNameToTargetFontOverride == null)
				Properties.Settings.Default.MapProjectNameToTargetFontOverride = new StringCollection();

			var projectName = BackTranslationHelperDataSource.ProjectName;
			fontDialog.Font = GetTargetLanguageFontForProject(projectName);
			var mapProjectNameToTargetFontOverride = SettingToDictionary(Properties.Settings.Default.MapProjectNameToTargetFontOverride);
			if (mapProjectNameToTargetFontOverride.TryGetValue(projectName, out List<string> fontOverride))
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

				textBoxTargetTextExisting.Font =
					textBoxPossibleTargetTranslation1.Font =
					textBoxPossibleTargetTranslation2.Font =
					textBoxPossibleTargetTranslation3.Font =
					textBoxTargetBackTranslation.Font = fontDialog.Font;
				mapProjectNameToTargetFontOverride.Add(projectName, fontOverride);
				Properties.Settings.Default.MapProjectNameToTargetFontOverride = SettingFromDictionary(mapProjectNameToTargetFontOverride);
				Properties.Settings.Default.Save();
			}
		}

		private void SourceRtlOverrideToolStripMenuItem_CheckedChanged(object sender, EventArgs e)
		{
			if (_ignoreChange)
				return;

			if (Properties.Settings.Default.MapProjectNameToSourceRtLOverride == null)
				Properties.Settings.Default.MapProjectNameToSourceRtLOverride = new StringCollection();

			var projectName = BackTranslationHelperDataSource.ProjectName;
			var rtlOverride = sourceRightToLeftToolStripMenuItem.Checked ? RightToLeft.Yes : RightToLeft.No;

			textBoxSourceData.RightToLeft = rtlOverride;
			var mapProjectNameToSourceRtLOverride = SettingToDictionary(Properties.Settings.Default.MapProjectNameToSourceRtLOverride);
			var value = new List<string> { sourceRightToLeftToolStripMenuItem.Checked ? "true" : "false" };
			if (mapProjectNameToSourceRtLOverride.ContainsKey(projectName))
				mapProjectNameToSourceRtLOverride[projectName] = value;
			else
				mapProjectNameToSourceRtLOverride.Add(projectName, value);

			Properties.Settings.Default.MapProjectNameToSourceRtLOverride = SettingFromDictionary(mapProjectNameToSourceRtLOverride);
			Properties.Settings.Default.Save();
		}

		private void TargetRtlOverrideToolRightToLeftStripMenuItem_CheckedChanged(object sender, EventArgs e)
		{
			if (_ignoreChange)
				return;

			if (Properties.Settings.Default.MapProjectNameToTargetRtLOverride == null)
				Properties.Settings.Default.MapProjectNameToTargetRtLOverride = new StringCollection();

			var projectName = BackTranslationHelperDataSource.ProjectName;
			var rtlOverride = targetRightToLeftToolStripMenuItem.Checked ? RightToLeft.Yes : RightToLeft.No;

			textBoxTargetTextExisting.RightToLeft =
				textBoxPossibleTargetTranslation1.RightToLeft =
				textBoxPossibleTargetTranslation2.RightToLeft =
				textBoxPossibleTargetTranslation3.RightToLeft =
				textBoxTargetBackTranslation.RightToLeft = rtlOverride;
			var mapProjectNameToTargetRtLOverride = SettingToDictionary(Properties.Settings.Default.MapProjectNameToTargetRtLOverride);
			var value = new List<string> { targetRightToLeftToolStripMenuItem.Checked ? "true" : "false" };
			if (mapProjectNameToTargetRtLOverride.ContainsKey(projectName))
				mapProjectNameToTargetRtLOverride[projectName] = value;
			else
				mapProjectNameToTargetRtLOverride.Add(projectName, value);

			Properties.Settings.Default.MapProjectNameToTargetRtLOverride = SettingFromDictionary(mapProjectNameToTargetRtLOverride);
			Properties.Settings.Default.Save();
		}

		private void BroadCastKey(string keyToSend)
		{
			SendKeyToControl(textBoxSourceData, keyToSend);
			if (textBoxTargetTextExisting.Visible)
				SendKeyToControl(textBoxTargetTextExisting, keyToSend);

			var textBoxesPossibleTargetTranslations = tableLayoutPanel.Controls.OfType<TextBox>().Where(l => l.Name.Contains("textBoxPossibleTargetTranslation")).ToList();
			for (var i = 0; i < TheTranslators.Count; i++)
			{
				var textBox = textBoxesPossibleTargetTranslations[i];
				if (textBox.Visible)
				{
					SendKeyToControl(textBox, keyToSend);
				}
			}
			textBoxTargetBackTranslation.Focus();	// finally, put the focus back in the editable box
		}

		private void SendKeyToControl(Control control, string keyToSend)
		{
			control.Focus();
			SendKeys.SendWait(keyToSend);
		}

		private bool _queryAboutF5Meaning = true;
		private void TextBoxTargetBackTranslation_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
		{
			string keyToSend = null;
			switch (e.KeyData)
			{
				case Keys.Up:
					keyToSend = "{UP}";
					break;
				case Keys.Down:
					keyToSend = "{DOWN}";
					break;
				case Keys.Right:
					keyToSend = "{RIGHT}";
					break;
				case Keys.F5:
					if (_queryAboutF5Meaning)
					{
						var res = MessageBox.Show($"By pressing F5, did you mean to re-execute the translator (e.g. after making changes to it)?", BackTranslationHelperDataSource.ProjectName, MessageBoxButtons.YesNoCancel);
						if ((res != DialogResult.Yes) || (_model == null))
							return;

						_queryAboutF5Meaning = false;
					}

					_model.TargetData = null;   // so it'll be reinitialized
					_model.TargetsPossible.Clear();
					Initialize(!String.IsNullOrEmpty(_model.TargetDataPreExisting));
					UpdateData(_model);

					return;
			}

			if (!String.IsNullOrEmpty(keyToSend))
			{
				BroadCastKey(keyToSend);
				textBoxTargetBackTranslation.Focus();	// reset input focus back to the editable text box
			}
		}

		private static bool _ignoreChange;
		private void DisplayRighttoleftToolStripMenuItem_DropDownOpening(object sender, EventArgs e)
		{
			_ignoreChange = true;
			sourceRightToLeftToolStripMenuItem.Checked = textBoxSourceData.RightToLeft == RightToLeft.Yes;
			targetRightToLeftToolStripMenuItem.Checked = textBoxTargetBackTranslation.RightToLeft == RightToLeft.Yes;
			_ignoreChange = false;
		}
	}
}
