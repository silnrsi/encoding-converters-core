using System;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;

namespace SpellingFixer30
{
	/// <summary>
	/// The Fix Spelling dialog: lets the user edit a bad spelling and its replacement (showing the code points of
	/// both), optionally with a list of similar words from the project to pick the replacement from.
	/// It's normally shown modally via ShowDialog(...), but a client can also show it non-modally (Show(owner)),
	/// load new words into it with LoadWord, and handle the Accepting event to act on OK (and keep the dialog
	/// open if that fails).
	/// </summary>
	public partial class QueryGoodSpelling : Form
    {
        private const int SimilarWordsColumn = 5;
        private const int SimilarWordsWidth = 175;

        private string m_strBadWord;    // in case we change it
        private readonly Func<string, SpellFixerWords> m_getSimilarWords;

        /// <summary>
        /// Raised when the user clicks OK (BadSpelling and GoodSpelling are already set). Set Cancel to keep the
        /// dialog open.
        /// </summary>
        public event EventHandler<QueryGoodSpellingAcceptingEventArgs> Accepting;

		public QueryGoodSpelling(Font font)
            : this(font, null)
		{
        }

        /// <summary>
        /// Constructor for a dialog that also lists the words similar to the bad spelling (e.g. pass
        /// CscProject.GetAmbiguousWords). The list is hidden when getSimilarWords is null or returns nothing.
        /// </summary>
        public QueryGoodSpelling(Font font, Func<string, SpellFixerWords> getSimilarWords)
		{
			//
			// Required for Windows Form Designer support
			//
			InitializeComponent();

            m_getSimilarWords = getSimilarWords;

            if (font != null)
            {
                textBoxBadWord.Font = font;
                textBoxReplacement.Font = font;
                labelOriginalReason.Font = font;
                listBoxSimilarWords.Font = font;
            }

            helpProvider.SetHelpString(this.textBoxBadWord, Properties.Resources.textBoxBadWordHelp);
            helpProvider.SetHelpString(this.textBoxReplacement, Properties.Resources.textBoxReplacementHelp);

            // make room for the list
            if (m_getSimilarWords != null)
                ClientSize = new Size(ClientSize.Width + SimilarWordsWidth, ClientSize.Height);
        }

        /// <summary>
        /// Loads the words into the dialog (which may already be showing, if it was shown non-modally)
        /// </summary>
        public void LoadWord(string strBadWord, string strReplacement, string strOriginalWord, bool bShowDelete, bool bShowSwapWords)
        {
            textBoxBadWord.Text = strBadWord;
            textBoxReplacement.Text = strReplacement;
            RefreshSimilarWords();  // (TextChanged isn't raised if the bad word is the same as before)

            // so typing (or Ctrl+V) replaces the replacement (Focus() alone doesn't work before the form is shown)
            ActiveControl = textBoxReplacement;
            textBoxReplacement.Focus();
            textBoxReplacement.SelectAll();

            this.buttonDelete.Visible = bShowDelete;
			this.buttonSwapWords.Visible = bShowSwapWords;

            this.labelOrigReasonLabel.Visible = this.labelOriginalReason.Visible = false;
            if (strBadWord != strReplacement)
            {
                this.Text = bShowDelete ? "Existing Replacement Rule" : "Add New Replacement Rule";
                if (!String.IsNullOrEmpty(strOriginalWord))
                {
                    this.labelOrigReasonLabel.Visible = true;
                    this.labelOriginalReason.Visible = true;
                    this.labelOriginalReason.Text = strOriginalWord;
                }
            }
            else
                this.Text = "Fix Spelling";
        }

        public DialogResult ShowDialog(string strBadWord, string strReplacement, string strOriginalWord, bool bShowDelete, bool bShowSwapWords)
        {
            LoadWord(strBadWord, strReplacement, strOriginalWord, bShowDelete, bShowSwapWords);
            return DialogOwner.ShowDialog(this);   // (on top, if what it was opened from is)
        }

        public string   GoodSpelling
        {
            get { return m_strGoodWord; }
        }

        public string   BadSpelling
        {
            get { return m_strBadWord; }
        }

        /// <summary>
        /// The box with the bad spelling (e.g. so a client can switch keyboards when it gets the focus)
        /// </summary>
        public TextBox BadWordTextBox => textBoxBadWord;

        /// <summary>
        /// The box with the replacement (good spelling)
        /// </summary>
        public TextBox ReplacementTextBox => textBoxReplacement;

        /// <summary>
        /// Whether the vernacular text (the two text boxes and the similar words) is right-to-left
        /// </summary>
        public bool VernacularRightToLeft
        {
            get { return textBoxBadWord.RightToLeft == RightToLeft.Yes; }
            set
            {
                var rtl = value ? RightToLeft.Yes : RightToLeft.No;
                textBoxBadWord.RightToLeft = textBoxReplacement.RightToLeft = listBoxSimilarWords.RightToLeft = rtl;
            }
        }

        private void RefreshSimilarWords()
        {
            SpellFixerWords similarWords = null;
            var strBadWord = textBoxBadWord.Text.Trim();
            if ((m_getSimilarWords != null) && !String.IsNullOrEmpty(strBadWord))
            {
                try
                {
                    similarWords = m_getSimilarWords(strBadWord);
                }
                catch
                {
                    // this runs on every keystroke, so don't complain
                }
            }

            listBoxSimilarWords.BeginUpdate();
            listBoxSimilarWords.Items.Clear();
            if (similarWords != null)
                listBoxSimilarWords.Items.AddRange(similarWords.Where(w => w != null)
                                                               .Select(w => (object)new SimilarWordItem(w))
                                                               .ToArray());
            listBoxSimilarWords.EndUpdate();

            var show = listBoxSimilarWords.Items.Count > 0;
            if (panelSimilarWords.Visible != show)
            {
                tableLayoutPanel.ColumnStyles[SimilarWordsColumn] = new ColumnStyle(SizeType.Absolute, show ? ScaledSimilarWordsWidth : 0);
                panelSimilarWords.Visible = show;
            }
        }

        private bool m_isLoaded;

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);     // (this is where the form's sizes, incl. the table's column widths, get auto-scaled)
            m_isLoaded = true;
        }

        // the list's width: before the form is loaded, its auto-scaling will scale it; after that, scale it ourselves
        private int ScaledSimilarWordsWidth
        {
            get
            {
                if (!m_isLoaded)
                    return SimilarWordsWidth;
                using (var g = CreateGraphics())
                    return (int)Math.Round(SimilarWordsWidth * g.DpiX / 96f);
            }
        }

        private class SimilarWordItem
        {
            public SimilarWordItem(SpellFixerWord word)
            {
                Word = word.Value;
                Count = word.Count;
            }

            public string Word { get; }
            public int Count { get; }

            public override string ToString() => $"{Word} ({Count})";
        }

        private void listBoxSimilarWords_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (listBoxSimilarWords.SelectedItem is SimilarWordItem item)
                textBoxReplacement.Text = item.Word;
        }

        private void listBoxSimilarWords_DoubleClick(object sender, EventArgs e)
        {
            if (listBoxSimilarWords.SelectedItem is SimilarWordItem)
                buttonOK.PerformClick();
        }

        private void buttonOK_Click(object sender, System.EventArgs e)
        {
            m_strGoodWord = this.textBoxReplacement.Text;
            m_strBadWord = this.textBoxBadWord.Text;

            var accepting = Accepting;
            if (accepting != null)
            {
                var args = new QueryGoodSpellingAcceptingEventArgs(m_strBadWord, m_strGoodWord);
                accepting(this, args);
                if (args.Cancel)
                {
                    this.DialogResult = DialogResult.None;  // (so a modal dialog stays open)
                    return;
                }
            }

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
            RefreshSimilarWords();
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

    /// <summary>
    /// The arguments of QueryGoodSpelling.Accepting
    /// </summary>
    public class QueryGoodSpellingAcceptingEventArgs : EventArgs
    {
        public QueryGoodSpellingAcceptingEventArgs(string badSpelling, string goodSpelling)
        {
            BadSpelling = badSpelling;
            GoodSpelling = goodSpelling;
        }

        public string BadSpelling { get; }
        public string GoodSpelling { get; }

        /// <summary>
        /// Set to true to keep the dialog open
        /// </summary>
        public bool Cancel { get; set; }
    }
}
