using System;
using System.Drawing;
using System.Windows.Forms;

namespace SpellingFixer30
{
	/// <summary>
	/// Summary description for QueryGoodSpelling.
	/// </summary>
	internal partial class QueryGoodSpelling : Form
    {
        private string m_strBadWord;    // in case we change it

		public QueryGoodSpelling(Font font)
		{
			//
			// Required for Windows Form Designer support
			//
			InitializeComponent();

            textBoxBadWord.Font = font;
            textBoxReplacement.Font = font;
            labelOriginalReason.Font = font;

            helpProvider.SetHelpString(this.textBoxBadWord, Properties.Resources.textBoxBadWordHelp);
            helpProvider.SetHelpString(this.textBoxReplacement, Properties.Resources.textBoxReplacementHelp);
        }

        public DialogResult ShowDialog(string strBadWord, string strReplacement, string strOriginalWord, bool bShowDeleteAndSwapWordButtons)
        {
            textBoxBadWord.Text = strBadWord;
            textBoxReplacement.Text = strReplacement;

            textBoxReplacement.Focus();
            textBoxReplacement.SelectAll();

            this.buttonDelete.Visible = bShowDeleteAndSwapWordButtons;
			this.buttonSwapWords.Visible = bShowDeleteAndSwapWordButtons;

            if (strBadWord != strReplacement)
            {
                this.Text = bShowDeleteAndSwapWordButtons ? "Existing Replacement Rule" : "Add New Replacement Rule";
                if (!String.IsNullOrEmpty(strOriginalWord))
                {
                    this.labelOrigReasonLabel.Visible = true;
                    this.labelOriginalReason.Visible = true;
                    this.labelOriginalReason.Text = strOriginalWord;
                }
            }

            return base.ShowDialog();
        }

        public string   GoodSpelling
        {
            get { return m_strGoodWord; }
        }

        public string   BadSpelling
        {
            get { return m_strBadWord; }
        }

        private void buttonOK_Click(object sender, System.EventArgs e)
        {
            m_strGoodWord = this.textBoxReplacement.Text;
            m_strBadWord = this.textBoxBadWord.Text;
            // this.DialogResult = DialogResult.OK;
            this.Close();
        }

        private void buttonCancel_Click(object sender, System.EventArgs e)
        {
            // this.DialogResult = DialogResult.Cancel;
            this.Close();
        }

        private void UpdateUniCodes(string strInputString, Label labelUniCodes)
        {
            int nLenString = strInputString.Length;

            string strWhole = null, strPiece = null, strUPiece = null;
            foreach(char ch in strInputString)
            {
                if( ch == 0 )   // sometimes it's null (esp. for utf32)
                    strPiece = "nul (u0000)  ";
                else
                {
                    strUPiece = String.Format("{0:X}", (int)ch);

                    // left pad with 0's (there may be a better way to do this, but 
                    //  I don't know what it is)
                    while(strUPiece.Length < 4)  strUPiece = "0" + strUPiece;

                    strPiece = String.Format("{0:#} (u{1,4})  ", ch, strUPiece);
                }
                strWhole += strPiece;
            }

            labelUniCodes.Text = strWhole;
        }

        private void textBoxBadWord_TextChanged(object sender, EventArgs e)
        {
            UpdateUniCodes(this.textBoxBadWord.Text, this.labelUniCodesLhs);
        }

        private void textBoxReplacement_TextChanged(object sender, EventArgs e)
        {
            UpdateUniCodes(this.textBoxReplacement.Text, this.labelUniCodesRhs);
        }

        private void buttonDelete_Click(object sender, System.EventArgs e)
        {
            this.Close();   // button returns DialogResult.Abort
        }

        private void buttonSwapWords_Click(object sender, EventArgs e)
        {
            this.Close();   // button returns DialogResult.Retry
        }
    }
}
