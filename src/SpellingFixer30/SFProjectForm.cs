using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using System.Collections;
using System.IO;
using System.Globalization;
using ECInterfaces;
using SilEncConverters40;
using SpellingFixer30.Properties;

namespace SpellingFixer30
{
    internal partial class SFProjectForm : Form
    {
        protected CscProject m_project = null;
        protected Hashtable m_mapScriptParamFiles = new Hashtable();
		protected SortedList<string, CultureInfo> m_cis = new SortedList<string, CultureInfo>();
        protected bool m_bNewProject = false;

        public SFProjectForm(CscProject project)
        {
            InitializeComponent();
            m_project = project;
            m_bSaveVSS = false;

			var _implementationClass = "SpellingFixer30.CscBaseProject";
			if (Directory.Exists(CscProject.VernacularScriptSystemsDirectory))
			{
                string strVssDir = CscProject.VernacularScriptSystemsDirectory;
                saveFileDialogVssFile.InitialDirectory = strVssDir;

                var di = new DirectoryInfo(strVssDir);
				foreach (FileInfo fiVSSs in di.GetFiles(String.Format("*.{0}", CscProject.VernacularScriptSystemFileExt)))
				{
					string strName = fiVSSs.Name;
					this.comboBoxVernScriptType.Items.Add(strName.Substring(0, strName.Length - fiVSSs.Extension.Length));

					if (strName == Settings.Default.LastVernScriptType)
					{
						var sfssp = VernScriptSystem.LoadVsvXml(fiVSSs.FullName);
						_implementationClass = sfssp.Properties.ProgId ?? _implementationClass;
					}
				}
			}

			// plus any project-specific VSS files in the project's own folder (which take precedence over a global one
			//  by the same name; see CscProject.CreateVernacularScriptSystemFileSpec)
			if (!String.IsNullOrEmpty(m_project?.Name))
			{
				foreach (var name in CscProject.GetProjectVernacularScriptSystemNames(m_project.Name))
					if (!comboBoxVernScriptType.Items.Contains(name))
						comboBoxVernScriptType.Items.Add(name);
			}

			CultureInfo[] cis = CultureInfo.GetCultures(CultureTypes.InstalledWin32Cultures);
			foreach (var ci in cis)
			{
			    if (m_cis.ContainsKey(ci.DisplayName))
			        continue;
				this.comboBoxLocale.Items.Add(ci.DisplayName);
				m_cis.Add(ci.DisplayName, ci);
			}
			this.comboBoxLocale.SelectedIndex = 0;

            m_bNewProject = (m_project == null);
            if (m_bNewProject)
            {
                // if we aren't editing, then set all check boxes to true.
                if (m_project == null)
                {
					// start w/ Indic
					Type typeProject = Type.GetType(_implementationClass);
                    m_project = (CscProject)Activator.CreateInstance(typeProject);
                }

                Properties.Settings.Default.Reload();
                if (Properties.Settings.Default.LastVernScriptType != null)
                    comboBoxVernScriptType.SelectedItem = Properties.Settings.Default.LastVernScriptType;
                else
                    comboBoxVernScriptType.SelectedIndex = 0;

                if (m_project != null)
                {
                    m_project.Font = fontDialogDisplay.Font;
                    m_project.TransliterationFont = new Font("Doulos SIL", 12);

                    labelTransliterator.Text = CscProject.DefaultTransliteratorEncConverterName;
                    labelVFont.Text = m_project.Font.Name;
                    labelTransliterationFont.Text = m_project.TransliterationFont.Name;
                }
            }
            else
            {
                PopulateControlsFromProject();
            }

            // the 'synchronize with Paratext' checkbox reflects whether the project folder is (already) a junction
            //  to a Paratext project's 'shared' folder. For a new project, we need a name before we can do that.
            string linkedParatextProjectName = null;
            if (!m_bNewProject)
            {
                try
                {
                    linkedParatextProjectName = ParatextProjectLink.GetLinkedParatextProjectName(m_project.Name);
                }
                catch { }
            }
            SetSyncWithParatextCheckBox(linkedParatextProjectName);
            checkBoxSyncWithParatext.Enabled = !m_bNewProject;
            textBoxProjectName.TextChanged += (s, e) =>
                checkBoxSyncWithParatext.Enabled = !String.IsNullOrEmpty(textBoxProjectName.Text.Trim());

            // help stuff
            helpProvider.SetHelpString(this.comboBoxVernScriptType, Properties.Resources.comboBoxVernScriptTypeHelp);
            helpProvider.SetHelpString(this.checkBoxCompareDoubleChars, Properties.Resources.checkBoxCompareDoubleCharsHelp);
            helpProvider.SetHelpString(this.checkBoxIgnoreCase, Properties.Resources.checkBoxIgnoreCaseHelp);
        }

        /// <summary>
        /// Move values from the (existing) project object to the controls
        /// </summary>
        private void PopulateControlsFromProject()
        {
            this.textBoxProjectName.Text = m_project.Name;
            this.textBoxProjectName.ReadOnly = true;	// don't all this to be edited in edit mode
            this.comboBoxVernScriptType.SelectedItem = m_project.VernacularScriptSystemName;

            if (m_project.Locale != null)
                this.comboBoxLocale.SelectedItem = m_project.Locale.DisplayName;

            if (m_project.Font != null)
            {
                SetFontTooltip(this.buttonFont, m_project.Font);
                this.fontDialogDisplay.Font = m_project.Font;
                labelVFont.Text = m_project.Font.Name;
                this.buttonFont.Enabled = true;
            }

            if (m_project.Transliterator != null)
            {
                IEncConverter aEC = m_project.Transliterator.GetEncConverter;
                System.Diagnostics.Debug.Assert(aEC != null);
                toolTip.SetToolTip(buttonChooseTransliterator, aEC.ToString());
                labelTransliterator.Text = aEC.Name;

                SetFontTooltip(buttonTransliteratorFont, m_project.TransliterationFont);
                labelTransliterationFont.Text = m_project.TransliterationFont.Name;
            }
        }

        private const string SyncWithParatextLabel = "&Synchronize Project data with a Paratext Project";

        private void SetSyncWithParatextCheckBox(string paratextProjectName)
        {
            var isLinked = !String.IsNullOrEmpty(paratextProjectName);
            checkBoxSyncWithParatext.Checked = isLinked;
            checkBoxSyncWithParatext.Text = isLinked
                                                ? $"{SyncWithParatextLabel} (currently: '{paratextProjectName}')"
                                                : SyncWithParatextLabel;
        }

        /// <summary>
        /// The junction is created (or removed) as soon as the user (un)checks the box, so that by the time they
        /// click OK, the project folder already points to wherever the data is supposed to go, and CscProject
        /// doesn't have to know the difference. (AutoCheck is off, so the box only changes if this succeeds.)
        /// </summary>
        private void checkBoxSyncWithParatext_Click(object sender, EventArgs e)
        {
            var projectName = textBoxProjectName.Text.Trim();
            if (String.IsNullOrEmpty(projectName))
            {
                MessageBox.Show("You must enter a Project/Language name first", CscProject.ApplicationCaption);
                return;
            }

            if (!checkBoxSyncWithParatext.Checked)
            {
                var paratextProjectName = ParatextProjectLink.QueryParatextProjectName(projectName);
                if (paratextProjectName == null)
                    return;

                Cursor = Cursors.WaitCursor;
                ParatextLinkResult result;
                try
                {
                    result = ParatextProjectLink.LinkToParatext(projectName, paratextProjectName);
                }
                finally
                {
                    Cursor = Cursors.Default;
                }

                if (result == ParatextLinkResult.NotLinked)
                    return;

                // the project folder now exists (as a junction), so the name can't be changed anymore
                textBoxProjectName.Text = projectName;
                textBoxProjectName.ReadOnly = true;
                m_project.Name = projectName;
                m_project.ParatextProjectName = paratextProjectName;

                if (result == ParatextLinkResult.LinkedUsingSharedData)
                    ReloadProjectFromSharedData(paratextProjectName);

                UpdateParatextProjectNameInProjectFile(paratextProjectName);
                SetSyncWithParatextCheckBox(paratextProjectName);
            }
            else
            {
                if (MessageBox.Show($"Do you want to stop synchronizing the '{projectName}' project with the Paratext project? " +
                                    "A copy of the data will be kept on this computer (the data in the Paratext project will be left as is for any other users).",
                                    CscProject.ApplicationCaption, MessageBoxButtons.YesNo, MessageBoxIcon.Question) != DialogResult.Yes)
                    return;

                if (!ParatextProjectLink.UnlinkFromParatext(projectName))
                    return;

                m_project.ParatextProjectName = null;
                UpdateParatextProjectNameInProjectFile(null);
                SetSyncWithParatextCheckBox(null);
            }
        }

        /// <summary>
        /// The user chose to use the version of the project that was already in the Paratext project's 'shared'
        /// folder, so load its settings into our project object (rather than overwriting them when OK is clicked)
        /// </summary>
        private void ReloadProjectFromSharedData(string paratextProjectName)
        {
            var projectFilePath = GetProjectFilePath();
            if (!File.Exists(projectFilePath))
                return; // only data files (no project file), so the settings in this dialog will be what's saved

            try
            {
                CscProjectStore.TransferToCscProject(projectFilePath, m_project);
                m_project.ParatextProjectName = paratextProjectName;
                m_project.Init();
                m_bNewProject = false;

                PopulateControlsFromProject();
                VernScriptTypeItemSelected();   // reloads the distinction rules for the (possibly different) project

                MessageBox.Show($"The '{m_project.Name}' project's settings have been loaded from the Paratext project '{paratextProjectName}'.",
                                CscProject.ApplicationCaption);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"The project is now synchronized with the Paratext project '{paratextProjectName}', but its settings couldn't be loaded: {ex.Message}",
                                CscProject.ApplicationCaption);
            }
        }

        /// <summary>
        /// Records the link in the project file right away (if there is one), so it's there even if the user
        /// cancels this dialog (since the junction has already been created/removed)
        /// </summary>
        private void UpdateParatextProjectNameInProjectFile(string paratextProjectName)
        {
            try
            {
                CscProjectStore.UpdateParatextProjectName(GetProjectFilePath(), paratextProjectName);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Unable to update the project file: {ex.Message}", CscProject.ApplicationCaption);
            }
        }

        private string GetProjectFilePath()
        {
            return Path.Combine(ParatextProjectLink.GetLocalProjectFolder(m_project.Name),
                                $"{m_project.Name}.{CscProject.ProjectFileExt}");
        }

        public CscProject CscProject
        {
            get
			{
				System.Diagnostics.Debug.Assert(m_project != null);
				return m_project; 
			}
        }

        private void SetFontTooltip(Control ctrl, Font font)
        {
            toolTip.SetToolTip(ctrl, String.Format("{0}, {1}", font.Name, font.Size));
        }

        private void buttonFont_Click(object sender, EventArgs e)
        {
            fontDialogDisplay.Font = CscProject.Font;
            if (fontDialogDisplay.ShowDialog() == DialogResult.OK)
            {
                CscProject.Font = fontDialogDisplay.Font;
                SetFontTooltip(this.buttonFont, fontDialogDisplay.Font);
                labelVFont.Text = fontDialogDisplay.Font.Name;
            }
        }

        private void buttonTransliteratorFont_Click(object sender, EventArgs e)
        {
            fontDialogDisplay.Font = CscProject.TransliterationFont;
            if (fontDialogDisplay.ShowDialog() == DialogResult.OK)
            {
                CscProject.TransliterationFont = fontDialogDisplay.Font;
                SetFontTooltip(buttonTransliteratorFont, fontDialogDisplay.Font);
                labelTransliterationFont.Text = fontDialogDisplay.Font.Name;
                DistinctionRuleCheckBox.TransliteratorFont = CscProject.TransliterationFont;
            }
        }

        bool m_bSaveVSS = false;
        bool m_bSaveVssInProjectFolder = false; // set when the analysis makes changes, to suggest a project-specific copy

        private void buttonOK_Click(object sender, EventArgs e)
        {
            if (String.IsNullOrEmpty(this.textBoxProjectName.Text))
            {
                MessageBox.Show("You must enter a Project/Language name first or click 'Cancel'");
            }
            else if (comboBoxVernScriptType.SelectedIndex < 0)
            {
                MessageBox.Show("You must choose a Vernacular Script System first");
            }
            else
            {
                System.Diagnostics.Debug.Assert(m_project != null);
                m_project.Name = this.textBoxProjectName.Text;
                // (if the user chose to synchronize with Paratext, this folder is already a junction to the
                //  Paratext project's 'shared' folder)
                string strProjectFolder = ParatextProjectLink.GetLocalProjectFolder(m_project.Name);
                CscProject.AddProjectKey(strProjectFolder, m_project.Name);

                if (this.comboBoxLocale.SelectedIndex != -1)
                    m_project.Locale = m_cis[(string)comboBoxLocale.SelectedItem];

                m_project.CompareDoubleChars = this.checkBoxCompareDoubleChars.Checked;
                m_project.CompareCase = this.checkBoxIgnoreCase.Checked;

                m_project.ResetAmbiguityRules();

                foreach (DistinctionRuleCheckBox aRule in m_alstDistinctionRuleCheckBoxes)
                {
                    if (aRule.Checked == true)
                    {
                        m_project.AddAmbiguityRuleName(aRule.Text);
                        m_bSaveVSS |= aRule.IsModified;
                    }
                }

                // see if the user edited the VSS file.
                string strVssName = (string)this.comboBoxVernScriptType.SelectedItem;
                if (m_bSaveVSS)
                    UpdateVssFile(m_alstDistinctionRuleCheckBoxes, ref strVssName);

                m_project.VernacularScriptSystemName = strVssName;

                // m_project.Init();   // initialize the rest (after having set up the rules)
                CscProject.SaveProject(m_project);
                DialogResult = DialogResult.OK;
                this.Close();
            }
        }

        protected void UpdateVssFile(List<DistinctionRuleCheckBox> alstDistinctionRuleCheckBoxes, ref string strVssName)
        {
            if (MessageBox.Show(String.Format("You've made changes to the ambiguities rules or transliterator for the {0} Vernacular Script Type file. Do you want to save the changes?", strVssName), CscProject.ApplicationCaption, MessageBoxButtons.YesNoCancel) != DialogResult.Yes)
                return;

            VernScriptSystem sfssp = DistinctionRuleCheckBox.VernScriptSystemFile;
            string strFolder = saveFileDialogVssFile.InitialDirectory;

            // a project-specific VSS goes in the project's own folder (so it goes wherever the project's data goes, e.g. to
            //  other users thru a Paratext project's 'shared' folder)
            string projectFolder = ParatextProjectLink.GetLocalProjectFolder(textBoxProjectName.Text.Trim());
            if (m_bSaveVssInProjectFolder)
            {
                if (!Directory.Exists(projectFolder))
                    Directory.CreateDirectory(projectFolder);
                saveFileDialogVssFile.InitialDirectory = projectFolder;
            }

            if (saveFileDialogVssFile.ShowDialog() == DialogResult.OK)
            {
                var savedFolder = Path.GetDirectoryName(saveFileDialogVssFile.FileName);
                if (String.Equals(Path.GetFullPath(savedFolder).TrimEnd('\\'), Path.GetFullPath(projectFolder).TrimEnd('\\'), StringComparison.OrdinalIgnoreCase))
                {
                    // (it's project-specific: found via the project folder, so no need to change the VSS folder)
                }
                else if (savedFolder != strFolder)
                {
                    // this means that the user has saved it elsewhere!?
                    DialogResult res = MessageBox.Show("If you save this file in a different folder, you won't be able to see the other files as well (unless you move them into your new folder as well). Do you want to change the folder where the Vernacular Script System files reside?", CscProject.ApplicationCaption, MessageBoxButtons.YesNoCancel);
                    if (res == DialogResult.Yes)
                        CscProject.VernacularScriptSystemsDirectory = Path.GetDirectoryName(saveFileDialogVssFile.FileName);
                    else
                        return;
                }

                IEncConverter aEC = m_project.Transliterator.GetEncConverter;
                if (aEC != null)
                {
                    if (sfssp.Properties != null)
                    {
                        var properties = sfssp.Properties;
                        properties.TransliteratorEncConverterName = aEC.Name;
                        properties.TransliteratorEncConverterDirectionForward = aEC.DirectionForward;
                        properties.TransliteratorEncConverterNormalize = aEC.NormalizeOutput.ToString();
                    }
                }

                VernScriptSystem.SaveVsvXml(saveFileDialogVssFile.FileName, sfssp);
                strVssName = Path.GetFileNameWithoutExtension(saveFileDialogVssFile.FileName);
            }
        }

        private void buttonCancel_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.Cancel;
            this.Close();
        }

        protected override void OnFormClosing(FormClosingEventArgs e)
        {
            base.OnFormClosing(e);

            // if this was a new project that the user linked to a Paratext project, but then cancelled before a
            //  project file was ever saved, don't leave an empty (junction) folder behind, or it'd look like a
            //  (broken) project
            if (!e.Cancel && (DialogResult != DialogResult.OK) && m_bNewProject &&
                !String.IsNullOrEmpty(m_project?.Name) && ParatextProjectLink.IsLinked(m_project.Name) &&
                !File.Exists(GetProjectFilePath()))
            {
                try
                {
                    var localFolder = ParatextProjectLink.GetLocalProjectFolder(m_project.Name);
                    var sharedFolder = ParatextProjectLink.GetJunctionTarget(localFolder);
                    ParatextProjectLink.RemoveJunction(localFolder);
                    if ((sharedFolder != null) && Directory.Exists(sharedFolder) && !Directory.EnumerateFileSystemEntries(sharedFolder).Any())
                        Directory.Delete(sharedFolder);
                }
                catch { }
            }
        }

        private void tabControl_Enter(object sender, EventArgs e)
        {
            if (this.comboBoxVernScriptType.SelectedIndex == -1)
            {
                this.tabControl.SelectedTab = this.tabPageLanguage;
                new ApplicationException("You must first select a Vernacular Script Type");
            }
        }

        internal List<DistinctionRuleCheckBox> m_alstDistinctionRuleCheckBoxes = new List<DistinctionRuleCheckBox>();
        // protected TableLayoutPanel m_panelForDistinctionRows = null;

		private void comboBoxVernScriptType_SelectedIndexChanged(object sender, EventArgs e)
        {
            VernScriptTypeItemSelected();
        }

        protected void VernScriptTypeItemSelected()
        {
			if (comboBoxVernScriptType.SelectedIndex != -1)
			{
				// save the last item selected
				Properties.Settings.Default.LastVernScriptType = (string)comboBoxVernScriptType.SelectedItem;
				Properties.Settings.Default.Save();

                VernScriptSystem sfssp = null;
                if (m_project != null)
                {
                    var strVernScriptFilename = CscProject.CreateVernacularScriptSystemFileSpec(comboBoxVernScriptType.SelectedItem?.ToString(),
                                                                                                m_bNewProject ? null : m_project.Name);
                    sfssp = VernScriptSystem.LoadVsvXml(strVernScriptFilename);
                }

                // if we aren't editing, then set all check boxes to true.
                else if (m_project == null)
                {
                    var strProgId = sfssp.Properties.ProgId ?? "SpellingFixer30.CscBaseProject";
                    Type typeProject = Type.GetType(strProgId);
                    m_project = (CscProject)Activator.CreateInstance(typeProject);
                    sfssp = m_project.Init();
                }

				textBoxAdditionalPunctuation.Text = m_project.ExtraPunctuation;
				string punctuationToolTip = $"Enter any additional punctuation or whitespace characters needed for this language, separated by spaces (these are used for boundary condition checking). No need to include any of these, which are already included: {SpellingFixer.GetDefaultPunctuation}";
				toolTip.SetToolTip(labelAddlPunct, punctuationToolTip);
				toolTip.SetToolTip(textBoxAdditionalPunctuation, punctuationToolTip);

				// now we can allow the user to set font.
				buttonFont.Enabled = true;

                if (sfssp.Distinctions.DistinctionList.Count > 0)
                {
                    var aDistsRow = sfssp.Distinctions;
                    this.checkBoxCompareDoubleChars.Checked = aDistsRow.CompareDoubleCharacters;
                    this.checkBoxIgnoreCase.Checked = aDistsRow.IgnoreCase;

                    var aRows = sfssp.Distinctions.DistinctionList;
                    
                    // clear out earlier table if present
                    flowLayoutPanel.Controls.Clear();

                    if (m_alstDistinctionRuleCheckBoxes.Count > 0)
                        m_alstDistinctionRuleCheckBoxes.Clear();

                    DistinctionRuleCheckBox.VernScriptSystemFile = sfssp;
                    DistinctionRuleCheckBox.ToolTip = toolTip;
                    DistinctionRuleCheckBox.TransliteratorEncConverter = CscProject.GetTransliterator(sfssp);
                    DistinctionRuleCheckBox.TransliteratorFont = CscProject.TransliterationFont; 

                    foreach (var aRow in aRows)
                    {
                        AddCheckBoxToTab(aRow);
                    }
                }
			}

		}

        protected DistinctionRuleCheckBox AddCheckBoxToTab(Distinction aRow, int index = -1)
        {
            // add a new row to the table layout thing for this new distinction rule
            // int nIndex = m_panelForDistinctionRows.RowStyles.Add(new RowStyle());
            DistinctionRuleCheckBox aDistinctionRule = new DistinctionRuleCheckBox(aRow,
                (m_bNewProject || m_project.ContainsAmbiguityRule(aRow.Name)));
            UpdateToolTip(toolTip, aRow, aDistinctionRule);
            helpProvider.SetHelpString(aDistinctionRule, Properties.Resources.DistinctionRuleCheckBoxHelp);
            // m_panelForDistinctionRows.Controls.Add(aDistinctionRule, 0, nIndex);
            flowLayoutPanel.Controls.Add(aDistinctionRule);
            if ((index >= 0) && (index < m_alstDistinctionRuleCheckBoxes.Count))
            {
                m_alstDistinctionRuleCheckBoxes.Insert(index, aDistinctionRule);
                flowLayoutPanel.Controls.SetChildIndex(aDistinctionRule, index);
            }
            else
                m_alstDistinctionRuleCheckBoxes.Add(aDistinctionRule);
            return aDistinctionRule;
        }

        private void buttonAddNew_Click(object sender, EventArgs e)
        {
            QueryDistinctionName dlg = new QueryDistinctionName();
            if (dlg.ShowDialog() == DialogResult.OK)
            {
                VernScriptSystem sfssp = DistinctionRuleCheckBox.VernScriptSystemFile;
                System.Diagnostics.Debug.Assert(sfssp.Distinctions != null);
                var aRow = new Distinction { Name = dlg.DistinctionName };
                sfssp.Distinctions.DistinctionList.Add(aRow);
                DistinctionRuleCheckBox aDistinctionRule = AddCheckBoxToTab(aRow);
                aDistinctionRule.EditDistinctionRule();
                aDistinctionRule.Checked = true;
            }
        }

        internal static void UpdateToolTip(ToolTip toolTip, Distinction aDistRow, DistinctionRuleCheckBox aDistinctionRuleCB)
        {
            string strToolTip = aDistinctionRuleCB.ToString();
            if (!String.IsNullOrEmpty(aDistRow.Description))
                strToolTip = aDistRow.Description + strToolTip;

            toolTip.SetToolTip(aDistinctionRuleCB, strToolTip);
        }

        /// <summary>
        /// Set to open the dialog on the Inconsistency Settings tab and start analyzing them right away
        /// </summary>
        public bool StartWithRuleAnalysis { get; set; }

        protected override void OnShown(EventArgs e)
        {
            base.OnShown(e);
            if (StartWithRuleAnalysis && !m_bNewProject)
            {
                tabControl.SelectedTab = tabPageDistinctionRules;
                BeginInvoke(new Action(() => buttonAnalyze.PerformClick()));
            }
        }

        private void buttonAnalyze_Click(object sender, EventArgs e)
        {
            if (m_bNewProject || (m_project.m_mapS2SBad2GoodWords == null))
            {
                MessageBox.Show("The analysis compares the Inconsistency Settings with the spelling corrections you've made, so it can only be done once the project has some data.",
                                CscProject.ApplicationCaption);
                return;
            }

            CscRuleAnalyzer analyzer;
            try
            {
                // analyze the settings as they are in this dialog (which may have been changed, but not saved yet)
                var enabled = m_alstDistinctionRuleCheckBoxes.Where(cb => cb.Checked).Select(cb => cb.Name).ToList();
                analyzer = CscRuleAnalyzer.Create(m_project, DistinctionRuleCheckBox.VernScriptSystemFile, enabled,
                                                  checkBoxCompareDoubleChars.Checked, checkBoxIgnoreCase.Checked,
                                                  DistinctionRuleCheckBox.TransliteratorEncConverter);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Unable to analyze the Inconsistency Settings: {ex.Message}", CscProject.ApplicationCaption);
                return;
            }

            using (var dlg = new RuleAnalysisForm(analyzer, m_project.Font ?? Font, canApply: true, dismissedKeys: m_project.DismissedRuleSuggestions))
            {
                var res = dlg.ShowDialog(this);
                if (dlg.AnalysisCompleted)
                {
                    m_project.RecordRuleAnalysis();

                    // (even if they cancel, remember the ones they asked not to be shown again)
                    var dismissed = dlg.DismissedKeys;
                    if (!dismissed.SetEquals(m_project.DismissedRuleSuggestions ?? new HashSet<string>()))
                        m_project.RecordDismissedRuleSuggestions(dismissed);
                }
                if (res == DialogResult.OK)
                    ApplyRuleAnalysisSuggestions(dlg.CheckedSuggestions);
            }
        }

        /// <summary>
        /// Applies the suggestions the user accepted: setting changes are made to the (in memory) Vernacular Script
        /// System and the check boxes (so they're saved, or not, by OK/Cancel as usual); word list clean-ups are saved now.
        /// </summary>
        private void ApplyRuleAnalysisSuggestions(List<RuleSuggestion> suggestions)
        {
            var vss = DistinctionRuleCheckBox.VernScriptSystemFile;
            var changes = new List<string>();
            int numWordListChanges = 0;

            foreach (var suggestion in suggestions)
            {
                switch (suggestion.Kind)
                {
                    case SuggestionKind.TurnOffRule:
                    case SuggestionKind.TurnOnRule:
                        var checkBox = m_alstDistinctionRuleCheckBoxes.FirstOrDefault(cb => cb.Name == suggestion.DistinctionName);
                        if (checkBox != null)
                        {
                            checkBox.Checked = (suggestion.Kind == SuggestionKind.TurnOnRule);
                            changes.Add(suggestion.Title);
                        }
                        break;

                    case SuggestionKind.MoveRule:
                        var toMove = vss.Distinctions.DistinctionList.FirstOrDefault(d => d.Name == suggestion.DistinctionName);
                        var checkBoxToMove = m_alstDistinctionRuleCheckBoxes.FirstOrDefault(cb => cb.Name == suggestion.DistinctionName);
                        if ((toMove != null) && (checkBoxToMove != null))
                        {
                            vss.Distinctions.DistinctionList.Remove(toMove);
                            vss.Distinctions.DistinctionList.Insert(0, toMove);
                            m_alstDistinctionRuleCheckBoxes.Remove(checkBoxToMove);
                            m_alstDistinctionRuleCheckBoxes.Insert(0, checkBoxToMove);
                            flowLayoutPanel.Controls.SetChildIndex(checkBoxToMove, 0);
                            checkBoxToMove.Checked = true;
                            m_bSaveVSS = true;
                            changes.Add(suggestion.Title);
                        }
                        break;

                    case SuggestionKind.AddRule:
                        var distinction = suggestion.NewDistinction;
                        var baseName = distinction.Name;
                        for (int i = 2; vss.Distinctions.DistinctionList.Any(d => d.Name == distinction.Name); i++)
                            distinction.Name = $"{baseName} ({i})";

                        // new settings go first, since that's how they were evaluated (so they apply before the
                        //  existing settings change the characters involved)
                        vss.Distinctions.DistinctionList.Insert(0, distinction);
                        var newCheckBox = AddCheckBoxToTab(distinction, 0);
                        newCheckBox.Checked = true;
                        m_bSaveVSS = true;
                        changes.Add($"Add '{distinction.Name}'");
                        break;

                    case SuggestionKind.PunctuationCharacter:
                        vss.ExtraPunctuation ??= new ExtraPunctuation { IgnorePunctuations = new List<IgnorePunctuation>() };
                        vss.ExtraPunctuation.IgnorePunctuations ??= new List<IgnorePunctuation>();
                        vss.ExtraPunctuation.IgnorePunctuations.Add(new IgnorePunctuation { Description = "added by the analysis", Value = suggestion.Character });
                        textBoxAdditionalPunctuation.Text = String.Join(" ", new[] { textBoxAdditionalPunctuation.Text, $"'{suggestion.Character}'" }.Where(s => !String.IsNullOrEmpty(s)));
                        m_bSaveVSS = true;
                        changes.Add(suggestion.Title);
                        break;

                    case SuggestionKind.IgnoreCharacter:
                        vss.IgnoreCharactersAfterTransliteration ??= new IgnoreCharactersAfter { Characters = new List<IgnoreCharacter>() };
                        vss.IgnoreCharactersAfterTransliteration.Characters ??= new List<IgnoreCharacter>();
                        vss.IgnoreCharactersAfterTransliteration.Characters.Add(new IgnoreCharacter { Description = $"U+{(int)suggestion.Character[0]:X4} (added by the analysis)", Value = suggestion.Character });
                        m_bSaveVSS = true;
                        changes.Add(suggestion.Title);
                        break;

                    case SuggestionKind.PreprocessRegex:
                        vss.PreprocessDataViaRegex ??= new RegexChanges { RegexChangeList = new List<RegexChange>() };
                        vss.PreprocessDataViaRegex.RegexChangeList ??= new List<RegexChange>();
                        vss.PreprocessDataViaRegex.RegexChangeList.Add(suggestion.Regex);
                        m_bSaveVSS = true;
                        changes.Add(suggestion.Title);
                        break;

                    case SuggestionKind.CleanUpWordLists:
                    case SuggestionKind.RemoveWords:
                        try
                        {
                            numWordListChanges += m_project.ApplyWordListFixes(suggestion.WordListFixes);
                        }
                        catch (Exception ex)
                        {
                            MessageBox.Show($"Unable to clean up the word lists: {ex.Message}", CscProject.ApplicationCaption);
                        }
                        break;
                }
            }

            if (!changes.Any() && (numWordListChanges == 0))
                return;

            var message = new System.Text.StringBuilder();
            if (numWordListChanges > 0)
                message.AppendLine($"{numWordListChanges} word list entries have been cleaned up (and saved).").AppendLine();

            if (changes.Any())
            {
                message.AppendLine("These changes have been made to the Inconsistency Settings:");
                foreach (var change in changes)
                    message.AppendLine("    " + change);
                message.AppendLine();
                message.Append("They'll take effect when you click OK.");

                if (m_bSaveVSS)
                {
                    // suggest saving a project-specific copy, so other projects using the same VSS aren't affected
                    var vssName = (string)comboBoxVernScriptType.SelectedItem;
                    var projectName = textBoxProjectName.Text.Trim();
                    var suggestedName = ((vssName ?? String.Empty).IndexOf(projectName, StringComparison.OrdinalIgnoreCase) >= 0)
                                            ? vssName
                                            : $"{vssName} ({projectName})";
                    saveFileDialogVssFile.FileName = $"{suggestedName}.{CscProject.VernacularScriptSystemFileExt}";
                    m_bSaveVssInProjectFolder = true;
                    message.Append($" You'll then be asked to save the Vernacular Script System file: we suggest calling it '{suggestedName}' " +
                                   $"and saving it in the project's own folder, so that any other projects that use '{vssName}' aren't affected, " +
                                   "and so it goes wherever the project's data goes (e.g. to other users, if the project is synchronized with a Paratext project).");
                }
            }

            MessageBox.Show(message.ToString(), CscProject.ApplicationCaption);
        }

        private void buttonSetAll_Click(object sender, EventArgs e)
        {
            foreach (DistinctionRuleCheckBox aCb in m_alstDistinctionRuleCheckBoxes)
                aCb.Checked = true;
        }

        private void buttonClearAll_Click(object sender, EventArgs e)
        {
            foreach (DistinctionRuleCheckBox aCb in m_alstDistinctionRuleCheckBoxes)
                aCb.Checked = false;
        }

        private void buttonChooseTransliterator_Click(object sender, EventArgs e)
        {
            CscProject.InsureAnyToLatin();
            IEncConverter aIEC = DirectableEncConverter.EncConverters.AutoSelect(ConvType.Unicode_to_Unicode);
            if (aIEC != null)
            {
                DirectableEncConverter aEC = new DirectableEncConverter(aIEC);
                m_project.Transliterator = aEC;
                m_bSaveVSS = true;
                toolTip.SetToolTip(buttonChooseTransliterator, aIEC.ToString());
                labelTransliterator.Text = aIEC.Name;
                DistinctionRuleCheckBox.TransliteratorEncConverter = aEC;
            }
        }
    }

    internal class DistinctionRuleCheckBox : CheckBox
    {
        public static ToolTip ToolTip = null;
        protected static VernScriptSystem m_sfssp = null;
        protected static DirectableEncConverter m_aECTransliteration = null;
        protected static Font m_fontTransliteration = null;

        protected Distinction m_aDistinctionRow = null;
        protected bool m_bModified = false;

        public DistinctionRuleCheckBox(Distinction aDistinctionRow, bool bChecked)
        {
            m_aDistinctionRow = aDistinctionRow;
            this.Text = m_aDistinctionRow.Name;
            this.Checked = bChecked;
        }

        protected override void OnMouseUp(MouseEventArgs mevent)
        {
            base.OnMouseUp(mevent);
            if (mevent.Button == MouseButtons.Right)
            {
                EditDistinctionRule();
            }
        }

        public void EditDistinctionRule()
        {
            System.Diagnostics.Debug.Assert(m_sfssp != null);
            DistinctionTreeEditor dlg = new DistinctionTreeEditor(m_sfssp, ref m_aDistinctionRow, TransliteratorEncConverter, TransliteratorFont);
            m_bModified = (dlg.ShowDialog() == DialogResult.OK);

            if (IsModified)
            {
                System.Diagnostics.Debug.Assert(ToolTip != null);
                SFProjectForm.UpdateToolTip(ToolTip, m_aDistinctionRow, this);
            }
        }

        public List<NormalizeRule> NormalizeRules
        {
            get { return m_aDistinctionRow.NormalizeRules; }
        }

        public Distinction DistinctionRow
        {
            get { return m_aDistinctionRow; }
        }

        public static DirectableEncConverter TransliteratorEncConverter
        {
            get { return m_aECTransliteration; }
            set { m_aECTransliteration = value; }
        }

        public static Font TransliteratorFont
        {
            get { return m_fontTransliteration; }
            set { m_fontTransliteration = value; }
        }

        public static VernScriptSystem VernScriptSystemFile
        {
            get { return m_sfssp; }
            set { m_sfssp = value; }
        }

        public bool IsModified
        {
            get { return m_bModified; }
        }

        public new string Name
        {
            get { return m_aDistinctionRow.Name; }
            set { m_aDistinctionRow.Name = value; }
        }

        public string Description
        {
            get { return m_aDistinctionRow.Description; }
            set { m_aDistinctionRow.Description = value; }
        }

        public override string ToString()
        {
            // (this used to chop the last 2 characters off the list of forms, e.g. showing "[ ] -> k" for "[kh] -> k")
            string strTip = null;
            foreach (NormalizeRule aNRRow in NormalizeRules ?? new List<NormalizeRule>())
            {
                var forms = aNRRow.CharactersToNormalize?.CharacterToNormalize?.Select(c => c.Value) ?? Enumerable.Empty<string>();
                strTip += String.Format("{0}[ {1} ] -> {2}{3}", Environment.NewLine, String.Join(", ", forms),
                                        String.IsNullOrEmpty(aNRRow.NormalizedForm) ? "(removed)" : aNRRow.NormalizedForm,
                                        aNRRow.IsBeforeTransliteration ? " (before transliteration)" : null);
            }
            return strTip;
        }
    }
}