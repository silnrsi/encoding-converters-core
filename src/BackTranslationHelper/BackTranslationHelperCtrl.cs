using ECInterfaces;
using SilEncConverters40;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
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
        public FindReplaceHelper TheFindReplaceProject;
        public DirectableEncConverter TheFindReplaceConverter;
        public BackTranslationHelperModel _model;
        public bool IsModified = false;
        protected bool _displayExistingTargetTranslation;

        /// <summary>
        /// keep track of some recently translated portions, so we can avoid calling to Bing again for the same input data
        /// (which happens if the user is switching back and forth to an existing verse editing things manually in Ptx)
        /// key it by the converter name (bkz there may be multiple of these running each w/ the same source portion, but
        /// different target languages)
        /// </summary>
        protected static Dictionary<string, Dictionary<string, string>> _mapOfRecentTranslations = new Dictionary<string, Dictionary<string, string>>();
        #endregion

        public BackTranslationHelperCtrl()
        {
            InitializeComponent();

            this.MouseWheel += new MouseEventHandler(this.UserControl_MouseWheel);
            this.textBoxTargetBackTranslation.MouseWheel += new MouseEventHandler(this.TargetBackTranslation_MouseWheel);
            hideCurrentTargetTextToolStripMenuItem.Checked = Properties.Settings.Default.HideCurrentTargetText;
        }

        protected override void OnParentChanged(EventArgs e)
        {
            base.OnParentChanged(e);

            if (Properties.Settings.Default.PinnedToTop)
            {
                var parent = (Control)this;
                while (((parent = parent.Parent) != null) && !(parent is Form))
                    ;

                if (parent is Form parentForm)
                {
					buttonPinToTop.Image = global::BackTranslationHelper.Properties.Resources.pindown;
					parentForm.TopMost = true;
				}
			}
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

        #region SubscribeableEvent handling
        // If the user forms want specific text change events, they can register for them with an event handler which
        //  we'll call if the event occurs
        public const string SubscribeableEventKeyTargetBackTranslationTextChanged = "TargetBackTranslationTextChanged";
        public delegate void EventHandler<EventArgs>(string value);
        protected static Dictionary<string, EventHandler<EventArgs>> SubscribeableEvents = new Dictionary<string, EventHandler<EventArgs>>();

        public void RegisterForNotification(string key, EventHandler<EventArgs> eventHandler)
        {
            SubscribeableEvents[key] = eventHandler;
        }

        private void TextBoxTargetBackTranslation_TextChanged(object sender, System.EventArgs e)
        {
            if (SubscribeableEvents.TryGetValue(SubscribeableEventKeyTargetBackTranslationTextChanged, out EventHandler<EventArgs> eventHandler))
            {
                eventHandler(textBoxTargetBackTranslation.Text);
            }
        }

		public void AddToSettingsMenu(System.Windows.Forms.ToolStripMenuItem menuItem)
		{
			settingsToolStripMenuItem.DropDownItems.Add(menuItem);
		}
        #endregion

        public void Initialize(bool displayExistingTargetTranslation)
        {
            _displayExistingTargetTranslation = displayExistingTargetTranslation;
            CheckInitializeFindReplaceHelper();
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
            toolStripTextBoxStatus.Size = CalculateStatusLineSize(toolStripTextBoxStatus, settingsToolStripMenuItem);
            var targetLanguageFont = GetTargetLanguageFontForProject(projectName);
            var targetLanguageRightToLeft = GetTargetLanguageRightToLeftForProject(projectName);
            if (_displayExistingTargetTranslation && !hideCurrentTargetTextToolStripMenuItem.Checked)
            {
                var rowStyle = tableLayoutPanel.RowStyles[nRowStyleOffset++];
                rowStyle.SizeType = SizeType.Percent;   // gives it real estate
                rowStyle.Height = 20F;
				textBoxTargetTextExisting.Font = targetLanguageFont;
				textBoxTargetTextExisting.RightToLeft = targetLanguageRightToLeft;
            }
            else
            {
				var rowStyle = tableLayoutPanel.RowStyles[nRowStyleOffset++];
				rowStyle.SizeType = SizeType.Absolute; // makes it disappear
				rowStyle.Height = 0;
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
            //  then we need to display even the one as an option, bkz the editable textbox will contain the
            //  existing translation, which if it's not correct, we want to be able to fill it from the converted
            //  value (which will be in the 1st targetoption box)
            var i = 0;
            if ((numOfTranslators == 1) && !_displayExistingTargetTranslation && !hideCurrentTargetTextToolStripMenuItem.Checked)
            {
                // hide them all
                for (; i < MaxPossibleTargetTranslations; i++)
                {
                    var textBox = textBoxesPossibleTargetTranslations[i];
                    var button = buttonsFillTargetOption[i];
					var rowStyle = tableLayoutPanel.RowStyles[nRowStyleOffset + i];
					rowStyle.SizeType = SizeType.Absolute;
					rowStyle.Height = 0;
				}
            }
            else
            {
                // set up mnemonics for the buttons to make it easier to trigger
                var mnemonicChar = 1;
                if (_displayExistingTargetTranslation)
                    buttonFillExistingTargetText.Text = $" &{mnemonicChar++}";  // so that Alt+1 will trigger the button (and space so the text won't show over the icon)

                for (; i < numOfTranslators; i++)
                {
                    var textBox = textBoxesPossibleTargetTranslations[i];
                    var button = buttonsFillTargetOption[i];
                    button.Text = $" &{mnemonicChar++}";
					textBox.Font = targetLanguageFont;
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
					var rowStyle = tableLayoutPanel.RowStyles[nRowStyleOffset + i];
					rowStyle.SizeType = SizeType.Absolute;
					rowStyle.Height = 0;
				}
            }

            buttonSubstitute.Visible = FindReplaceHelper.IsSpellFixerAvailable;
            tableLayoutPanel.ResumeLayout(false);
            tableLayoutPanel.PerformLayout();
            ResumeLayout(false);
            PerformLayout();
        }

        private Size CalculateStatusLineSize(ToolStripTextBox toolStripTextBoxStatus, ToolStripMenuItem settingsToolStripMenuItem)
        {
            var width = this.Width - settingsToolStripMenuItem.Width - 50;  // Padding;
            return new Size(width, toolStripTextBoxStatus.Height);
        }

        private string CheckInitializeFindReplaceHelper(bool initialize = false)
        {
            if (TheFindReplaceProject != null)
                return TheFindReplaceProject.SpellFixerEncConverterName;

            // check if we're using a FindReplace helpers
            if (Properties.Settings.Default.MapProjectNameToFindReplaceProject == null)
                Properties.Settings.Default.MapProjectNameToFindReplaceProject = new StringCollection();
            var mapProjectNameToFindReplaceProjects = SettingToDictionary(Properties.Settings.Default.MapProjectNameToFindReplaceProject);
            var projectName = BackTranslationHelperDataSource.ProjectName;

            string findReplaceConverterName = null;
            if (!mapProjectNameToFindReplaceProjects.TryGetValue(projectName, out List<string> lstFriendlyName))
            {
                if (!initialize)
                    return findReplaceConverterName;

                TheFindReplaceProject = TrySpellFixerProjectLogin();
                if (TheFindReplaceProject == null)
                    return findReplaceConverterName; // must have canceled

                findReplaceConverterName = TheFindReplaceProject.SpellFixerEncConverterName;
                lstFriendlyName = new List<string> { findReplaceConverterName };
                mapProjectNameToFindReplaceProjects[projectName] = lstFriendlyName;
                Properties.Settings.Default.MapProjectNameToFindReplaceProject = SettingFromDictionary(mapProjectNameToFindReplaceProjects);
                Properties.Settings.Default.Save();
            }

            findReplaceConverterName = lstFriendlyName.FirstOrDefault();
            TheFindReplaceProject = new FindReplaceHelper(findReplaceConverterName);
            TheFindReplaceConverter = (DirectableEncConverter.EncConverters.ContainsKey(findReplaceConverterName))
                                        ? new DirectableEncConverter(DirectableEncConverter.EncConverters[findReplaceConverterName])
                                        : null;
            return findReplaceConverterName;
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
                mapProjectNameToEcTranslators[projectName] = translatorNames;
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

        public List<TargetPossible> GetNewTargetTexts()
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

        public void SetNewTargetTexts(List<TargetPossible> newTargetTexts)
        {
            var textBoxesPossibleTargetTranslations = tableLayoutPanel.Controls.OfType<TextBox>().Where(l => l.Name.Contains("textBoxPossibleTargetTranslation")).ToList();
            System.Diagnostics.Debug.Assert(newTargetTexts.Count == TheTranslators.Count);

            for (var i = 0; i < TheTranslators.Count; i++)
            {
                var textBox = textBoxesPossibleTargetTranslations[i];
				textBox.Text = newTargetTexts[i].TargetData;
            }
        }

		#region Event handlers
        public void GetNewData(ref BackTranslationHelperModel model)
        {
            if (model == null)
            {
                _model = BackTranslationHelperDataSource.Model;
            }
            else if (_model?.SourceToTranslate != model.SourceToTranslate)
            {
                _model = model;
                IsModified = false; // start over assuming it isn't edited
            }

            for (var i = _model.TargetsPossible.Count; i < TheTranslators.Count; i++)
            {
                var theTranslator = TheTranslators[i];
                var translatedText = ConvertText(theTranslator, _model.SourceToTranslate);
                _model.TargetsPossible.Add(new TargetPossible { TargetData = translatedText, PossibleIndex = i, TranslatorName = theTranslator.Name });
            }

            model = _model;
        }

        private string ConvertText(IEncConverter theTranslator, string sourceData)
        {
            if (!_mapOfRecentTranslations.TryGetValue(theTranslator.Name, out Dictionary<string, string> mapRecentTranslations))
            {
                mapRecentTranslations = new Dictionary<string, string>();
                _mapOfRecentTranslations[theTranslator.Name] = mapRecentTranslations;
            }

            if (!mapRecentTranslations.TryGetValue(sourceData, out string targetData))
            {
                try
                {
                    targetData = theTranslator.Convert(sourceData);

					// only save it if we get it back successfully
					mapRecentTranslations[sourceData] = targetData;
				}
				catch (Exception ex)
                {
					var msg = ex.Message;
                    MessageBox.Show($"{msg}. Press F5 in the 'Target Translation' text box to try the conversion again.", EncConverters.cstrCaption);
					targetData = msg;	// but display this so the user sees it
				}
            }

            return targetData;
        }

        public void UpdateData(BackTranslationHelperModel model)
        {
            textBoxSourceData.Text = model.SourceData;

            // this may be a mistake, but if the verse editing has already begun and we're here (e.g. bkz a new rule was added)
            //  then don't start over from whatever was the target data before. Reset it to what's already been edited.
            var targetData = (IsModified)
                                ? textBoxTargetBackTranslation.Text
                                : model.TargetData;
            targetData = PreprocessTargetData(targetData);

            textBoxTargetBackTranslation.Text = targetData; // may be null, in which case, setting NewTargetTexts below will fill it with the 1st translation
            textBoxTargetBackTranslation.DeselectAll();     // it shouldn't be pre-selected (which it seems to do)
            
            // if we're keeping track of what was originally in the Target Project (i.e. Paratext usage), then put the current value in the label for
            //  the existing target field (just in case the user starts to edit or choose one of the other possibilities and then wants to revert it
            //  (it has a fill button also).
            textBoxTargetTextExisting.Text = model.TargetDataPreExisting;    // would be null if we're not using it

            // some clients (i.e. Word) only pass 1 translated target text (bkz it only knows about 1 EncConverter/Translator)
            //  if we have fewer than the number of possible target translations (i.e. we added 1 or more addl Translators),
            //  then convert the missing ones to add to this collection
            var numOfTranslators = TheTranslators.Count;
            for (var i = model.TargetsPossible.Count; i < numOfTranslators; i++)
            {
                var theTranslator = TheTranslators[i];
                var translatedText = ConvertText(theTranslator, model.SourceToTranslate);
                model.TargetsPossible.Add(new TargetPossible { TargetData = translatedText, PossibleIndex = i, TranslatorName = theTranslator.Name });
            }

            SetNewTargetTexts(model.TargetsPossible);

            // now check if the existing target was blank, then update the actual editable box w/ the first possibility
            if (String.IsNullOrEmpty(targetData))
            {
                var targetPossible = model.TargetsPossible.FirstOrDefault();
                textBoxTargetBackTranslation.Text = targetPossible?.TargetData;
            }
        }

        private string PreprocessTargetData(string targetDataOrig)
        {
            // if we have a FindReplaceHelper attached to this project, then use it before writing the result
            string difference = null, targetData = targetDataOrig;
            if (TheFindReplaceConverter != null)
            {
                // don't do this, bkz if the user is actively modifying search and replace rules, we don't want some previous
                //  value to override some newer fix
                // targetData = ConvertText(TheFindReplaceConverter.GetEncConverter, targetDataOrig);
                targetData = TheFindReplaceConverter.SafeConvert(targetDataOrig);

                if (targetData != targetDataOrig)
                {
                    difference = Difference(targetData, targetDataOrig);
                }
            }

            if (String.IsNullOrEmpty(difference))
            {
                SetStatusBox(String.Empty);
            }
            else
            {
                SetStatusBox($"substitutions made: {difference}");
            }

            return targetData;
        }

        private void SetStatusBox(string value)
        {
            toolStripTextBoxStatus.Text = value;
            toolStripTextBoxStatus.BackColor = String.IsNullOrEmpty(value)
                                                ? System.Drawing.SystemColors.Control
                                                : System.Drawing.SystemColors.ButtonShadow;
        }

        public static string Difference(string str1, string str2)
        {
            if (str1 == null)
            {
                return str2;
            }
            if (str2 == null)
            {
                return str1;
            }

            List<string> set1 = str1.Split(' ').Distinct().ToList();
            List<string> set2 = str2.Split(' ').Distinct().ToList();

            var diff = set2.Count() > set1.Count() ? set2.Except(set1).ToList() : set1.Except(set2).ToList();

            return $"'{string.Join("','", diff)}'";
        }

#endregion

#region Private helper methods

#endregion

        private void ButtonWriteTextToTarget_Click(object sender, System.EventArgs e)
        {
            BackTranslationHelperDataSource.ButtonPressed(ButtonPressed.WriteToTarget);
            BackTranslationHelperDataSource.Log($"changing target text from '{textBoxTargetTextExisting.Text}' to '{textBoxTargetBackTranslation.Text}'");
            if (!BackTranslationHelperDataSource.WriteToTarget(textBoxTargetBackTranslation.Text))
            {
                SetStatusBox($"We had to reload the text. Click the button again.");
                if (!BackTranslationHelperDataSource.WriteToTarget(textBoxTargetBackTranslation.Text))
                    return;
            }
            IsModified = false;
        }

        private void ButtonNextSection_Click(object sender, System.EventArgs e)
        {
            if (BackTranslationHelperDataSource == null)
            {
                // see if we can manually trigger the change of verse number in the host
                return;
            }

            var existingTargetText = textBoxTargetTextExisting.Text;
            var newTargetText = textBoxTargetBackTranslation.Text;

            if (!autoSaveToolStripMenuItem.Checked)
            {
                if (IsModified)
                {
                    var res = MessageBox.Show($"Do you want to save/write out the translated text before moving to the next portion?",
                                              BackTranslationHelperDataSource.ProjectName, MessageBoxButtons.YesNoCancel);
                    if (res == DialogResult.Cancel)
                    {
                        return;
                    }
                    if (res == DialogResult.No)
                    {
                        IsModified = false;
                        BackTranslationHelperDataSource.ButtonPressed(ButtonPressed.MoveToNext);
                        BackTranslationHelperDataSource.MoveToNext();
                        return;
                    }
                }
            }

			// removed the check for IsModified, bkz if the user clicks 'Next', it means write. If they meant to
			//	not write it, they'd click 'Skip'.
            BackTranslationHelperDataSource.Log($"change target text from '{existingTargetText}' to '{newTargetText}'");
            if (!BackTranslationHelperDataSource.WriteToTarget(newTargetText))
            {
                // assuming that before returning false, the caller updated the data,
                //  let's try that again and see what happens
                SetStatusBox($"We had to reload the text. Click the button again.");
                if (!BackTranslationHelperDataSource.WriteToTarget(newTargetText))
                    return;
            }

            IsModified = false;
            BackTranslationHelperDataSource.ButtonPressed(ButtonPressed.MoveToNext);
            BackTranslationHelperDataSource.MoveToNext();
        }

        private void ChangeEncConverterToolStripMenuItem_Click(object sender, EventArgs e)
        {
            var dlg = new TranslatorListForm(TheTranslators);
            if (dlg.ShowDialog() == DialogResult.OK)
            {
                var nameOfTranslatorBeingRemoved = dlg.SelectedDisplayName;
                TheTranslators.RemoveAll(t => t.Name == nameOfTranslatorBeingRemoved);
                _model.TargetsPossible.RemoveAll(tp => tp.TranslatorName == nameOfTranslatorBeingRemoved);
                var mapProjectNameToEcTranslators = SettingToDictionary(Properties.Settings.Default.MapProjectNameToEcTranslators);
                var projectName = BackTranslationHelperDataSource.ProjectName;
                if (mapProjectNameToEcTranslators.TryGetValue(projectName, out List<string> translatorNames))
                {
                    translatorNames.RemoveAll(t => t == nameOfTranslatorBeingRemoved);
                    Properties.Settings.Default.MapProjectNameToEcTranslators = SettingFromDictionary(mapProjectNameToEcTranslators);
                    Properties.Settings.Default.Save();
                }

                Reload();
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
				mapProjectNameToEcTranslators[projectName] = translatorNames;
            }

            if (!translatorNames.Any(n => n == theTranslator.Name))
            {
                translatorNames.Add(theTranslator.Name);
                Properties.Settings.Default.MapProjectNameToEcTranslators = SettingFromDictionary(mapProjectNameToEcTranslators);
                Properties.Settings.Default.Save();
            }

            Reload();
        }

        private void UpdateEditableTextBox(TextBox textBoxFrom)
        {
            if (IsModified)
            {
                var res = MessageBox.Show($"Do you want to overwrite the current changes in the 'Target Translation' box?",
                                    BackTranslationHelperDataSource.ProjectName, MessageBoxButtons.YesNo);
                if (res != DialogResult.Yes)
                {
                    return;
                }
            }

            // also put it in the model as the ‘TargetData’ (possibly overwriting the translated value) so
            //  it’ll show up there again, rather than being overwritten by something previous
            _model.TargetData =  textBoxTargetBackTranslation.Text = PreprocessTargetData(textBoxFrom.Text);
            IsModified = true;	// mark it as modified now, since we changed from the default (which was the existing target text)
        }

        private void ButtonFillExistingTargetText_Click(object sender, EventArgs e)
        {
            UpdateEditableTextBox(textBoxTargetTextExisting);
        }

        private void ButtonFillTargetTextOption1_Click(object sender, EventArgs e)
        {
            UpdateEditableTextBox(textBoxPossibleTargetTranslation1);
        }

        private void ButtonFillTargetTextOption2_Click(object sender, EventArgs e)
        {
            UpdateEditableTextBox(textBoxPossibleTargetTranslation2);
        }

        private void ButtonFillTargetTextOption3_Click(object sender, EventArgs e)
        {
            UpdateEditableTextBox(textBoxPossibleTargetTranslation3);
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
                    Reload();
                }
            }
        }

        private void Reload()
        {
            System.Diagnostics.Debug.Assert(_model != null);
            Initialize(_displayExistingTargetTranslation && !hideCurrentTargetTextToolStripMenuItem.Checked);
            UpdateData(_model);
        }

        private void HideCurrentTargetTextToolStripMenuItem_CheckStateChanged(object sender, EventArgs e)
        {
            var newCheckState = hideCurrentTargetTextToolStripMenuItem.Checked;
            if (newCheckState != Properties.Settings.Default.HideCurrentTargetText)
            {
                Properties.Settings.Default.HideCurrentTargetText = newCheckState;
                Properties.Settings.Default.Save();
                if (_model != null)
                {
                    Reload();
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
            if (IsModified)
            {
                var res = MessageBox.Show($"You have unsaved changes in the 'Target Translation' box. Click 'Yes' to lose them and continue (or click 'No' and then the 'Save Changes' or 'Next' buttons instead to keep them)",
                                          BackTranslationHelperDataSource.ProjectName,
                                          MessageBoxButtons.YesNo);
                if (res != DialogResult.Yes)
                {
                    return;
                }
            }

            IsModified = false; // so it will lose the changes and move on
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
                mapProjectNameToSourceFontOverride[projectName] = fontOverride;
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

				mapProjectNameToTargetFontOverride[projectName] = fontOverride;
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
            mapProjectNameToSourceRtLOverride[projectName] = value;
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
            mapProjectNameToTargetRtLOverride[projectName] = value;
            Properties.Settings.Default.MapProjectNameToTargetRtLOverride = SettingFromDictionary(mapProjectNameToTargetRtLOverride);
            Properties.Settings.Default.Save();
        }

        private void BroadCastKey(string keyToSend)
        {
            SendKeyToControl(textBoxSourceData, keyToSend);
            SendKeyToControl(textBoxTargetTextExisting, keyToSend);

            var textBoxesPossibleTargetTranslations = tableLayoutPanel.Controls.OfType<TextBox>().Where(l => l.Name.Contains("textBoxPossibleTargetTranslation")).ToList();
            for (var i = 0; i < TheTranslators.Count; i++)
            {
                var textBox = textBoxesPossibleTargetTranslations[i];
                SendKeyToControl(textBox, keyToSend);
            }
            textBoxTargetBackTranslation.Focus();    // finally, put the focus back in the editable box
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
                case Keys.F5:
                    if (_queryAboutF5Meaning)
                    {
                        var msg = "By pressing F5, did you mean to re-execute the translator (e.g. after making changes to it)?";
                        if (IsModified)
                            msg += " Note: existing changes will be overwritten if you click 'Yes'.";

                        var res = MessageBox.Show(msg, BackTranslationHelperDataSource.ProjectName, MessageBoxButtons.YesNoCancel);
                        if ((res != DialogResult.Yes) || (_model == null))
                            return;

                        _queryAboutF5Meaning = false;
                    }

                    IsModified = false; // so it can be changed

                    // I don't think I wanted to do this. This just means that if we originally processed the existing
                    //  text (in Ptx), then it would just shift to the other one, which isn't what we probably want
                    //  to do.
                    //  _model.TargetData = null;   // so it'll be reinitialized
                    _model.TargetsPossible.Clear();
                    Reload();
                    return;
            }

            if (!String.IsNullOrEmpty(keyToSend))
            {
                BroadCastKey(keyToSend);
                textBoxTargetBackTranslation.Focus();    // reset input focus back to the editable text box
            }
            else
            {
                // otherwise, if this was a bonafide edit, then we have to be careful not to clobber it if something causes us to update
                //  the data (e.g. adding a find-replace fix)
                IsModified = true;
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

        private void ButtonSubstitute_Click(object sender, EventArgs e)
        {
            if (TheFindReplaceProject == null)
            {
                if (String.IsNullOrEmpty(CheckInitializeFindReplaceHelper(true)))
                {
                    return;
                }
                System.Diagnostics.Debug.Assert(TheFindReplaceProject != null);
            }

            string findWhat;
            if ((findWhat = GetRequiredSelectedText()) == null)
                return;

            TheFindReplaceProject.AssignCorrectSpelling(findWhat);

            Reload();
        }

        private string GetRequiredSelectedText()
        {
            var findWhat = GetSelectedText();

            if (String.IsNullOrEmpty(findWhat))
            {
                MessageBox.Show("Select a word or phrase in the 'Target Translation' text box before clicking this button");
                return null;
            }

            return findWhat;
        }

        private string GetSelectedText()
        {
            var findWhat = textBoxTargetBackTranslation.SelectedText;
            if (String.IsNullOrEmpty(findWhat))
            {
                // if there is no selected text in the box, see if they want to use what's on the clipboard
                IDataObject iData = Clipboard.GetDataObject();

                // Determines whether the data is in a format you can use.
                if (iData.GetDataPresent(DataFormats.UnicodeText))
                {
                    findWhat = (string)iData.GetData(DataFormats.UnicodeText);
                }
            }

            return findWhat;
        }

        private void FindSubstitutionRuleMenuItem_Click(object sender, EventArgs e)
        {
            if (TheFindReplaceProject == null)
            {
                if (String.IsNullOrEmpty(CheckInitializeFindReplaceHelper()))
                {
                    return;
                }
                System.Diagnostics.Debug.Assert(TheFindReplaceProject != null);
            }

            string findWhat;
            if ((findWhat = GetRequiredSelectedText()) == null)
                return;

            TheFindReplaceProject.FindReplacementRule(findWhat);

            Reload();
        }

        private void EditSubtitutionsMenuItem_Click(object sender, EventArgs e)
        {
            if (TheFindReplaceProject == null)
            {
                if (String.IsNullOrEmpty(CheckInitializeFindReplaceHelper()))
                {
                    return;
                }
                // TheFindReplaceProject should be initialized if we're here
            }

            TheFindReplaceProject.EditSpellingFixes();

            Reload();
        }

        private void AssignNewSubstitutionProjectMenuItem_Click(object sender, EventArgs e)
        {
            // check if we're using a FindReplace helpers
            if (Properties.Settings.Default.MapProjectNameToFindReplaceProject != null)
            {
                var mapProjectNameToFindReplaceProjects = SettingToDictionary(Properties.Settings.Default.MapProjectNameToFindReplaceProject);
                var projectName = BackTranslationHelperDataSource.ProjectName;
                if (mapProjectNameToFindReplaceProjects.ContainsKey(projectName))
                {
                    mapProjectNameToFindReplaceProjects.Remove(projectName);
                    Properties.Settings.Default.MapProjectNameToFindReplaceProject = SettingFromDictionary(mapProjectNameToFindReplaceProjects);
                    Properties.Settings.Default.Save();
                }
            }

            TheFindReplaceProject = null;
            CheckInitializeFindReplaceHelper(true);
        }

        private FindReplaceHelper TrySpellFixerProjectLogin()
        {
            try
            {
                var aSF = FindReplaceHelper.GetFindReplaceHelper();
                DirectableEncConverter.EncConverters.Reinitialize();
                return aSF;
            }
            catch (Exception ex)
            {
                if (!ex.InnerException?.Message.Contains("No project selected") ?? true)
                    MessageBox.Show(ex.Message, EncConverters.cstrCaption);
            }
            return null;
        }

        private void ButtonPinToTop_Click(object sender, EventArgs e)
        {
            var newCheckState = !Properties.Settings.Default.PinnedToTop;
            Properties.Settings.Default.PinnedToTop = newCheckState;
            Properties.Settings.Default.Save();

            buttonPinToTop.Image = (newCheckState)
                                    ? global::BackTranslationHelper.Properties.Resources.pindown
                                    : global::BackTranslationHelper.Properties.Resources.pinup;

            // Now that I'm using a TableLayoutPanel in the Ptx plugin form, it may not be the parent.
            //  So go up the parent chain until we find a Form
            var parent = (Control)this;
            while (((parent = parent.Parent) != null) && !(parent is Form))
                ;

			if (parent is Form parentForm)
                parentForm.TopMost = newCheckState;
        }
	}
}
