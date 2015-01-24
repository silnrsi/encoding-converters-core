using System;
using System.IO;
using System.Windows.Forms;
using SilEncConverters40.Properties;

namespace SilEncConverters40
{
    public partial class AddNewProjectForm : Form
    {
        public AddNewProjectForm()
        {
            InitializeComponent();

            SourceLanguage = new AdaptItKBReader.LanguageInfo();
            TargetLanguage = new AdaptItKBReader.LanguageInfo();

            textBoxPunctuationSource.Text = SourceLanguage.Punctuation;
            textBoxPunctuationTarget.Text = TargetLanguage.Punctuation;

            labelDisplayFontNameSource.Text = SourceLanguage.FontName;
            labelDisplayFontNameTarget.Text = TargetLanguage.FontName;
        }

        public AdaptItKBReader.LanguageInfo SourceLanguage { get; set; }
        public AdaptItKBReader.LanguageInfo TargetLanguage { get; set; }

        private void TextBoxLanguageTextChanged(object sender, EventArgs e)
        {
            SourceLanguage.LangName = textBoxSourceLanguage.Text;
            TargetLanguage.LangName = textBoxTargetLanguage.Text;
            var bBothNonEmpty = (!String.IsNullOrEmpty(SourceLanguage.LangName) &&
                                 !String.IsNullOrEmpty(TargetLanguage.LangName));

            if (bBothNonEmpty)
            {
                var strProjectPath = AdaptItKBReader.AdaptItProjectFolder(null, SourceLanguage.LangName, TargetLanguage.LangName);
                var strDisplayMessage = String.Format("This would create the Adapt It project:{0}{0}{1}{0}{0}",
                                                      Environment.NewLine, strProjectPath);

                var bFolderExists = Directory.Exists(strProjectPath);
                if (bFolderExists)
                    strDisplayMessage += "(which already exists!)";

                textBoxDisplay.Text = strDisplayMessage;

                buttonOk.Enabled = !bFolderExists;
            }
            else
                buttonOk.Enabled = false;
        }

        private void ButtonOkClick(object sender, EventArgs e)
        {
            if (SourceLanguage.Punctuation.Length != TargetLanguage.Punctuation.Length)
            {
                MessageBox.Show(Resources.IDS_PunctuationListsMustBeSameLength,
                                AdaptItEncConverter.strDisplayName);

                if (tabControl.SelectedTab != tabPageAdvancedSettings)
                    tabControl.SelectedTab = tabPageAdvancedSettings;
                return;
            }

            DialogResult = DialogResult.OK;
            Close();
        }

        private void TextBoxPunctuationSourceTextChanged(object sender, EventArgs e)
        {
            SourceLanguage.Punctuation = textBoxPunctuationSource.Text;
        }

        private void TextBoxPunctuationTargetTextChanged(object sender, EventArgs e)
        {
            TargetLanguage.Punctuation = textBoxPunctuationTarget.Text;
        }

        private void CheckBoxR2LSourceCheckedChanged(object sender, EventArgs e)
        {
            SourceLanguage.RightToLeft = (checkBoxR2LSource.Checked)
                                             ? RightToLeft.Yes
                                             : RightToLeft.No;
        }

        private void CheckBoxR2LTargetCheckedChanged(object sender, EventArgs e)
        {
            TargetLanguage.RightToLeft = (checkBoxR2LTarget.Checked)
                                             ? RightToLeft.Yes
                                             : RightToLeft.No;
        }

        private void ButtonChooseSourceLanguageFontClick(object sender, EventArgs e)
        {
            DoFontDialog(SourceLanguage, labelDisplayFontNameSource);
        }

        private void ButtonChooseTargetLanguageFontClick(object sender, EventArgs e)
        {
            DoFontDialog(TargetLanguage, labelDisplayFontNameTarget);
        }

        private static void DoFontDialog(AdaptItKBReader.LanguageInfo langInfo, Control labelDisplayFontName)
        {
            var dlg = new FontDialog {Font = langInfo.FontToUse};
            if (dlg.ShowDialog() == DialogResult.OK)
            {
                labelDisplayFontName.Text = langInfo.FontName = dlg.Font.Name;
            }
        }
    }
}
