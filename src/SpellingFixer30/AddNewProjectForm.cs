using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;

namespace SpellingFixer30
{
    public partial class AddNewProjectForm : Form
    {
        public AddNewProjectForm(string addlPunctuation)
        {
            InitializeComponent();

            if (!String.IsNullOrEmpty(addlPunctuation))
            {
                textBoxAddlPunctuation.Text = DecodePunctuationForCC(addlPunctuation);
            }
        }

        public string NewProjectName
        {
            get { return textBoxName.Text; }
            set { textBoxName.Text = value; }
        }

        public string WordBoundaryDelimiter
        {
            get { return textBoxWordBoundaryDelimiter.Text; }
            set { textBoxWordBoundaryDelimiter.Text = value; }
        }

        public Font SelectedFont
        {
            get { return fontDialog.Font; }
            set 
            { 
                fontDialog.Font = value;
                labelFontChosen.Text = $"{fontDialog.Font.Name}, with size: {fontDialog.Font.Size}";
            }
        }

        public bool IsRightToLeft
        {
            get { return checkBoxRtL.Checked; }
            set { checkBoxRtL.Checked = value; }
        }

        public bool UserDefinedPunctuation { get; set; }

        public string GetAddlPunctuation(bool encoded)
        {
            var addlPunctuation = textBoxAddlPunctuation.Text;
            if (encoded)
            {
                addlPunctuation = EncodePunctuationForCC(addlPunctuation);
            }
            return addlPunctuation;
        }

        private void ButtonChooseFont_Click(object sender, EventArgs e)
        {
            if (fontDialog.ShowDialog() == DialogResult.OK)
            {
                textBoxWordBoundaryDelimiter.Font = textBoxAddlPunctuation.Font = SelectedFont = fontDialog.Font;                
            }
        }

        private void ButtonOk_Click(object sender, EventArgs e)
        {
        }

        private string EncodePunctuationForCC(string strPunctuation)
        {
            string strRet = null;

            if (UserDefinedPunctuation)
                return strPunctuation;

            else if (!String.IsNullOrEmpty(strPunctuation))
            {
                string[] astrChars = strPunctuation.Split(new char[] { ' ' });
                foreach (string strChar in astrChars)
                {
                    if (SpellingFixer.GetDefaultPunctuation.IndexOf(strChar) != -1)
                    {
                        MessageBox.Show(String.Format("There's no need to add the {0} character as Additional Punctuation because it's there by default,\r\nas are these: {1}",
                            strChar, DecodePunctuationForCCEx(SpellingFixer.GetDefaultPunctuation)), SpellingFixer.cstrCaption);
                        return null;
                    }
                    strRet += '\'' + strChar + "\' ";
                }
                if (!String.IsNullOrEmpty(strRet))
                    strRet = strRet.Substring(0, strRet.Length - 1);

                return SpellingFixer.GetDefaultPunctuation + ' ' + strRet;
            }

            return SpellingFixer.GetDefaultPunctuation;
        }

        private string DecodePunctuationForCC(string strPunctuation)
        {
            if (String.IsNullOrEmpty(strPunctuation))
                return null;

            // initialize it so that *we* take care of delimiting the punctuation
            UserDefinedPunctuation = false;

            // rather than assuming the additional punctuation was appended after an intact copy
            // of the default punctuation list, parse out every token (default and additional
            // alike) and diff them against the default list, since the user's additions might
            // have ended up interspersed in the middle of the default list rather than after it.
            var defaultTokens = TokenizePunctuationList(SpellingFixer.GetDefaultPunctuation);
            var givenTokens = TokenizePunctuationList(strPunctuation);

            // consume one occurrence of each default token out of the given tokens; whatever's
            // left over in givenTokens is the user's additional punctuation, and whatever's left
            // over in defaultTokens means the default list wasn't fully present, i.e. this isn't
            // the standard 'default tokens + additions' format at all -- it's free-form text that
            // the user is responsible for delimiting him/herself.
            var remainingDefaultTokens = new List<string>(defaultTokens);
            var additionalTokens = new List<string>();
            foreach (var token in givenTokens)
            {
                int nIndex = remainingDefaultTokens.IndexOf(token);
                if (nIndex != -1)
                    remainingDefaultTokens.RemoveAt(nIndex);
                else
                    additionalTokens.Add(token);
            }

            if (remainingDefaultTokens.Count > 0)
            {
                UserDefinedPunctuation = true;
                return strPunctuation;  // in this case, the user is responsible for delimiting the string him/herself
            }

            // if this is all there is, then the 'decoded' string is nothing.
            if (additionalTokens.Count == 0)
                return null;

            return DecodePunctuationForCCEx(String.Join(" ", additionalTokens));
        }

        /// <summary>
        /// Splits a space-delimited punctuation list into its individual tokens, e.g.
        /// "'.' tab nl '?'" -> [ "'.'", "tab", "nl", "'?'" ]. Unlike a plain String.Split(' '),
        /// this keeps a quoted token intact even when it quotes a literal space character (e.g.
        /// the "' '" token that represents the space character itself).
        /// </summary>
        private static List<string> TokenizePunctuationList(string strPunctuation)
        {
            var tokens = new List<string>();
            int i = 0;
            while (i < strPunctuation.Length)
            {
                // skip the space(s) delimiting tokens
                while ((i < strPunctuation.Length) && (strPunctuation[i] == ' '))
                    i++;
                if (i >= strPunctuation.Length)
                    break;

                int nStart = i;
                char ch = strPunctuation[i];
                if ((ch == '\'') || (ch == '\"'))
                {
                    // quoted token: consume through the matching closing quote, which may itself
                    // enclose a literal delimiter space (e.g. ' ' for the space character)
                    int nClose = strPunctuation.IndexOf(ch, i + 1);
                    i = (nClose == -1) ? strPunctuation.Length : nClose + 1;
                }
                else
                {
                    // bare word token (e.g. "tab" or "nl"): consume through the next delimiter
                    while ((i < strPunctuation.Length) && (strPunctuation[i] != ' '))
                        i++;
                }
                tokens.Add(strPunctuation.Substring(nStart, i - nStart));
            }
            return tokens;
        }

        private string DecodePunctuationForCCEx(string strPunctuation)
        {
            string strRet = null;
            string[] astrDelimitedChars = strPunctuation.Split(new char[] { ' ' });

            // each string should be in the form 'X', where X is the punctuation
            foreach (string strDelimitedChar in astrDelimitedChars)
            {
                if ((strDelimitedChar.IndexOfAny(new char[] { '\'', '\"' }) != -1)
                    && (strDelimitedChar.Length > 2))
                    strRet += strDelimitedChar.Substring(1, strDelimitedChar.Length - 2);
                else
                    strRet += strDelimitedChar;

                strRet += ' ';
            }

            if (!String.IsNullOrEmpty(strRet))
                strRet = strRet.Substring(0, strRet.Length - 1);

            return strRet;
        }

        private void AddNewProjectForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            // ignore the validation below, if the user is trying to close the dialog (either by the 
            //  upper right red 'X' or the Cancel button)
            if ((e.CloseReason == CloseReason.UserClosing) || (DialogResult == DialogResult.Cancel))
                return;

            if (WordBoundaryDelimiter.Contains('"'))
            {
                MessageBox.Show("Can't use the double-quote character for the word boundary delimiter",
                                SpellingFixer.cstrCaption);
                e.Cancel = true;
                return;
            }

            if (String.IsNullOrEmpty(NewProjectName))
            {
                MessageBox.Show("You must define a name for this converter (e.g. 'Hindi fixes')",
                                SpellingFixer.cstrCaption);
                e.Cancel = true;
                return;
            }

            if (String.IsNullOrEmpty(labelFontChosen.Text))
            {
                if (MessageBox.Show("Did you want to select a font to use when displaying Find-Replace pairs?",
                                    SpellingFixer.cstrCaption, MessageBoxButtons.YesNo)
                    == DialogResult.Yes)
                {
                    e.Cancel = true;
                    return;
                }
            }
        }
    }
}
