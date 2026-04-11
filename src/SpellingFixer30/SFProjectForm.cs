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

            if (Directory.Exists(CscProject.VernacularScriptSystemsDirectory))
			{
                string strVssDir = CscProject.VernacularScriptSystemsDirectory;
                saveFileDialogVssFile.InitialDirectory = strVssDir;

                var di = new DirectoryInfo(strVssDir);
				foreach (FileInfo fiVSSs in di.GetFiles(String.Format("*.{0}", CscProject.VernacularScriptSystemFileExt)))
				{
					string strName = fiVSSs.Name;
					this.comboBoxVernScriptType.Items.Add(strName.Substring(0, strName.Length - fiVSSs.Extension.Length));
				}
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
                    Type typeProject = Type.GetType("SpellingFixer30.CscIndicProject");
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
                // Move values from object to controls
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

            // help stuff
            helpProvider.SetHelpString(this.comboBoxVernScriptType, Properties.Resources.comboBoxVernScriptTypeHelp);
            helpProvider.SetHelpString(this.checkBoxCompareDoubleChars, Properties.Resources.checkBoxCompareDoubleCharsHelp);
            helpProvider.SetHelpString(this.checkBoxIgnoreCase, Properties.Resources.checkBoxIgnoreCaseHelp);
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
                string strProjectFolder = String.Format(@"{0}\{1}", CscProject.ProjectsDefaultDirectory, m_project.Name);
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
            if (saveFileDialogVssFile.ShowDialog() == DialogResult.OK)
            {
                if (Path.GetDirectoryName(saveFileDialogVssFile.FileName) != strFolder)
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
                    var strVernScriptFilename = Path.Combine(CscProject.VernacularScriptSystemsDirectory,
                        $"{this.comboBoxVernScriptType.SelectedItem}.{CscProject.VernacularScriptSystemFileExt}");

                    sfssp = VernScriptSystem.LoadVsvXml(strVernScriptFilename);
                }

                // if we aren't editing, then set all check boxes to true.
                else if (m_project == null)
                {
                    var strProgId = sfssp.Properties.ProgId ?? "SpellingFixer30.CscIndicProject";
                    Type typeProject = Type.GetType(strProgId);
                    m_project = (CscProject)Activator.CreateInstance(typeProject);
                    sfssp = m_project.Init();
                }

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

        protected DistinctionRuleCheckBox AddCheckBoxToTab(Distinction aRow)
        {
            // add a new row to the table layout thing for this new distinction rule
            // int nIndex = m_panelForDistinctionRows.RowStyles.Add(new RowStyle());
            DistinctionRuleCheckBox aDistinctionRule = new DistinctionRuleCheckBox(aRow, 
                (m_bNewProject || m_project.ContainsAmbiguityRule(aRow.Name)));
            UpdateToolTip(toolTip, aRow, aDistinctionRule);
            helpProvider.SetHelpString(aDistinctionRule, Properties.Resources.DistinctionRuleCheckBoxHelp);
            m_alstDistinctionRuleCheckBoxes.Add(aDistinctionRule);
            // m_panelForDistinctionRows.Controls.Add(aDistinctionRule, 0, nIndex);
            flowLayoutPanel.Controls.Add(aDistinctionRule);
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
            string strTip = null;
            foreach (NormalizeRule aNRRow in NormalizeRules)
            {
                strTip += String.Format("{0}[", Environment.NewLine);
                List<string> astrAmbiguousStrings = new List<string>();
                strTip += String.Join(", ", aNRRow.CharactersToNormalize.CharacterToNormalize.Select(c => c.Value));
                strTip = strTip.Substring(0, strTip.Length - 2);
                strTip += String.Format(" ] -> {0}", aNRRow.NormalizedForm);
            }
            return strTip;
        }
    }
}