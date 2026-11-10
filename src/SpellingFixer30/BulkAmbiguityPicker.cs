// #define UsingJump2Toolbox
using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Threading;
using System.Windows.Forms;

namespace SpellingFixer30
{
    [ComVisible(false)]
    internal partial class BulkAmbiguityPicker : Form
    {
        internal const string cstrVernLhs = "ColumnVernacularLhs";
        internal const string cstrVernRhs = "ColumnVernacularRhs";
        internal const string cstrTransLhs = "ColumnRomanizedLhs";
        internal const string cstrTransRhs = "ColumnRomanizedRhs";
        internal const string cstrCountLhs = "ColumnCountLhs";
        internal const string cstrCountRhs = "ColumnCountRhs";
        internal const string cstrSelector = "ColumnSelector";

        internal const string cstrSelectValue = "Select";
        internal const string cstrB2GValue = "     \u2192";
        internal const string cstrG2BValue = "\u2190     ";
        internal const string cstrG2GValue = "\u2194";

        private Color colorGood = Color.MediumSeaGreen;
        private Color colorBad = Color.Red;

        internal const int cnBundleDividerHeight = 5;

        internal CscProject m_project = null;
        internal WordsToCheckBulk m_mapWordsToCheck = null;
        internal KnownGoodWordList m_mapKnownGoodWords = null;
        internal Bad2GoodMap m_mapBad2GoodWords = null;
        internal CustomToolTipForm customToolTip = null;

        private DataGridViewRow m_rowLastSelected = null;
        private int m_nLastSelectedIndex = 0;
        protected bool m_bFirstTime = true;

        internal BulkAmbiguityPicker(CscProject project, KnownGoodWordList mapKnownGoodWords, Bad2GoodMap mapBad2GoodWords,
                                     ref WordsToCheckBulk mapWordsToCheck)
        {
            InitializeComponent();

            m_project = project;
            m_mapWordsToCheck = mapWordsToCheck;
            m_mapKnownGoodWords = mapKnownGoodWords;
            m_mapBad2GoodWords = mapBad2GoodWords;

            dataGridViewCorrectFormPicker.Columns[cstrVernLhs].DefaultCellStyle.Font = m_project.Font;
            dataGridViewCorrectFormPicker.Columns[cstrVernRhs].DefaultCellStyle.Font = m_project.Font;
            dataGridViewCorrectFormPicker.Columns[cstrTransLhs].DefaultCellStyle.Font = m_project.TransliterationFont;
            dataGridViewCorrectFormPicker.Columns[cstrTransRhs].DefaultCellStyle.Font = m_project.TransliterationFont;

            dataGridViewCorrectFormPicker.RowTemplate.Height = Math.Max(m_project.Font.Height, m_project.TransliterationFont.Height) + 6;   // 6 for padding

            customToolTip = new CustomToolTipForm
            {
                Font = m_project.Font
            };

            this.helpProvider.SetHelpString(this.dataGridViewCorrectFormPicker, Properties.Resources.BulkGridHelp);

            Properties.Settings.Default.Reload();

            try
            {
                showTransliterationToolStripMenuItem.Checked = Properties.Settings.Default.ShowTransliteration;
            }
            catch { }
            try
            {
                this.showCountToolStripMenuItem.Checked = Properties.Settings.Default.ShowCountOnBulk;
            }
            catch { }
            UpdateColumnSpans();

            try
            {
                for (int i = 6; i >= 0; i--)
                {
                    string strHeaderName = Properties.Settings.Default.BulkEditDisplayIndex[i];
                    dataGridViewCorrectFormPicker.Columns[strHeaderName].DisplayIndex = i;
                }
            }
            catch (Exception)
            {
                dataGridViewCorrectFormPicker.Columns[cstrVernLhs].DisplayIndex = 0;
                dataGridViewCorrectFormPicker.Columns[cstrTransLhs].DisplayIndex = 1;
                dataGridViewCorrectFormPicker.Columns[cstrCountLhs].DisplayIndex = 2;
                dataGridViewCorrectFormPicker.Columns[cstrSelector].DisplayIndex = 3;
                dataGridViewCorrectFormPicker.Columns[cstrVernRhs].DisplayIndex = 4;
                dataGridViewCorrectFormPicker.Columns[cstrTransRhs].DisplayIndex = 5;
                dataGridViewCorrectFormPicker.Columns[cstrCountRhs].DisplayIndex = 6;
            }

            try
            {
                colorBad = Properties.Settings.Default.BadColor;
            }
            catch { }

            try
            {
                colorGood = Properties.Settings.Default.GoodColor;
            }
            catch { }

            InitializeExtras();
        }

        // the controls added in code (rather than in the designer)
        private SplitContainer _splitContainer;
        private ExamplesPane _examplesPane;
        private ToolStripMenuItem _menuShowExamplesPane;
        private ToolStripMenuItem _menuEditProject;
        private ToolStripMenuItem _menuRestart;
        private ContextMenuStrip _menuSelector;
        private ToolStripMenuItem _miRightIsCorrect, _miLeftIsCorrect, _miBothCorrect, _miUndecided,
                                  _miSplit, _miUndoSplit, _miEdit, _miWhy;
        private DataGridViewRow _menuRow;
        private readonly System.Windows.Forms.Timer _bundleTimer = new System.Windows.Forms.Timer { Interval = 200 };

        private void InitializeExtras()
        {
            // the examples pane goes to the right of the grid (in a split container, so the user can resize it)
            _examplesPane = new ExamplesPane(m_project.Font) { Dock = DockStyle.Fill };
            _examplesPane.CloseRequested += (s, e) => _menuShowExamplesPane.Checked = false;
            tableLayoutPanel.Controls.Remove(dataGridViewCorrectFormPicker);
            // (the minimum size of the pane is set once the container has its real size; see ShowExamplesPane)
            _splitContainer = new SplitContainer
            {
                Dock = DockStyle.Fill,
                FixedPanel = FixedPanel.Panel2
            };
            _splitContainer.Panel1.Controls.Add(dataGridViewCorrectFormPicker);
            _splitContainer.Panel2.Controls.Add(_examplesPane);
            _splitContainer.Panel2Collapsed = true;     // (until OnLoad, which shows it if it's supposed to be)
            tableLayoutPanel.Controls.Add(_splitContainer, 0, 0);
            tableLayoutPanel.SetColumnSpan(_splitContainer, 3);

            // (the examples pane, or the custom tooltip if it's hidden, show the contexts in the vernacular font)
            dataGridViewCorrectFormPicker.ShowCellToolTips = false;
            TurnOffColumnAutoSizing();

            // View menu additions
            _menuShowExamplesPane = new ToolStripMenuItem("Show &Examples Pane") { CheckOnClick = true };
            try
            {
                _menuShowExamplesPane.Checked = Properties.Settings.Default.ShowExamplesPane;
            }
            catch
            {
                _menuShowExamplesPane.Checked = true;
            }
            _menuShowExamplesPane.ToolTipText = "Show the examples (contexts) of the word under the mouse in a pane on the right";
            _menuShowExamplesPane.CheckedChanged += (s, e) => ShowExamplesPane(_menuShowExamplesPane.Checked, true);
            viewToolStripMenuItem.DropDownItems.Insert(viewToolStripMenuItem.DropDownItems.IndexOf(showCountToolStripMenuItem) + 1, _menuShowExamplesPane);

            _menuEditProject = new ToolStripMenuItem("Edit &Project Settings (e.g. Inconsistency Settings)...", null, (s, e) => EditProject());
            _menuEditProject.ToolTipText = "Open the project settings dialog (e.g. to change or analyze the Inconsistency Settings)";
            _menuRestart = new ToolStripMenuItem("Save and &Look for Inconsistencies Again from Scratch", null, (s, e) => RestartFromScratch(null));
            _menuRestart.ToolTipText = "Save the choices you've made so far, and then search all of the Words to Check again (e.g. after changing the settings)";
            viewToolStripMenuItem.DropDownItems.Add(new ToolStripSeparator());
            viewToolStripMenuItem.DropDownItems.Add(_menuEditProject);
            viewToolStripMenuItem.DropDownItems.Add(_menuRestart);

            // the right-click menu for the middle ('Select') column
            _miRightIsCorrect = new ToolStripMenuItem("The &right-hand word is the correct spelling    →", null, (s, e) => SetChoice(_menuRow, cstrB2GValue));
            _miLeftIsCorrect = new ToolStripMenuItem("The &left-hand word is the correct spelling    ←", null, (s, e) => SetChoice(_menuRow, cstrG2BValue));
            _miBothCorrect = new ToolStripMenuItem("&Both words are correct    ↔", null, (s, e) => SetChoice(_menuRow, cstrG2GValue));
            _miUndecided = new ToolStripMenuItem("&Undecided (back to 'Select')", null, (s, e) => SetChoice(_menuRow, cstrSelectValue));
            _miSplit = new ToolStripMenuItem("&Split into two rows (to correct each word separately)", null, (s, e) => SplitRow(_menuRow));
            _miUndoSplit = new ToolStripMenuItem("U&ndo the split", null, (s, e) => FakeUndo(_menuRow, _menuRow.Cells[cstrSelector]));
            _miEdit = new ToolStripMenuItem("&Edit the spellings (or see their characters)...", null, (s, e) => EditSpellings(_menuRow));
            _miWhy = new ToolStripMenuItem("&Why are these words shown together?...", null, (s, e) => ShowWhyTogether(_menuRow));
            _miWhy.ToolTipText = "See which Inconsistency Settings made these words look alike (and turn off any that shouldn't)";
            _menuSelector = new ContextMenuStrip();
            _menuSelector.Items.AddRange(new ToolStripItem[]
            {
                _miRightIsCorrect, _miLeftIsCorrect, _miBothCorrect, _miUndecided,
                new ToolStripSeparator(),
                _miSplit, _miUndoSplit,
                new ToolStripSeparator(),
                _miEdit, _miWhy
            });

            UpdateNormalizedColumns();

            _bundleTimer.Tick += BundleTimer_Tick;
            FormClosed += (s, e) =>
            {
                _bundleTimer.Stop();
                _bundleTimer.Dispose();
                _menuSelector.Dispose();
            };
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);
            ShowExamplesPane(_menuShowExamplesPane.Checked, false);
        }

        /// <summary>
        /// Shows or hides the examples pane (making the dialog wider or narrower to make room for it, if possible)
        /// </summary>
        private void ShowExamplesPane(bool show, bool save)
        {
            int paneWidth = 300;
            try
            {
                paneWidth = Math.Max(150, Properties.Settings.Default.ExamplesPaneWidth);
            }
            catch { }

            if (show == !_splitContainer.Panel2Collapsed)
                return;

            if (show)
            {
                var screen = Screen.FromControl(this).WorkingArea;
                Width = Math.Min(screen.Width, Width + paneWidth);
                if (Right > screen.Right)
                    Left = Math.Max(screen.Left, screen.Right - Width);
                _splitContainer.Panel2Collapsed = false;
                try
                {
                    var maxDistance = _splitContainer.Width - _splitContainer.Panel2MinSize - _splitContainer.SplitterWidth;
                    var distance = _splitContainer.Width - paneWidth - _splitContainer.SplitterWidth;
                    _splitContainer.SplitterDistance = Math.Max(_splitContainer.Panel1MinSize, Math.Min(maxDistance, distance));
                    if (_splitContainer.Width - _splitContainer.SplitterDistance - _splitContainer.SplitterWidth >= 150)
                        _splitContainer.Panel2MinSize = 150;
                }
                catch (InvalidOperationException)
                {
                    // (if the dialog is too small for the pane's width, leave the splitter where it is)
                }
                customToolTip.HideToolTip();
            }
            else
            {
                SaveExamplesPaneWidth();
                _splitContainer.Panel2Collapsed = true;
                Width = Math.Max(MinimumSize.Width, Width - paneWidth);
            }

            if (save)
            {
                Properties.Settings.Default.ShowExamplesPane = show;
                Properties.Settings.Default.Save();
            }
        }

        private void SaveExamplesPaneWidth()
        {
            if (_splitContainer.Panel2Collapsed)
                return;
            Properties.Settings.Default.ExamplesPaneWidth = _splitContainer.Panel2.Width;
            Properties.Settings.Default.Save();
        }

        private bool IsExamplesPaneShown => (_splitContainer != null) && !_splitContainer.Panel2Collapsed;

        /// <summary>
        /// The 'Transliteration' columns show either the transliteration or (if 'Show Transliteration' is unchecked) the
        /// normalized form, i.e. the form that's compared (words with the same normalized form are shown together)
        /// </summary>
        private void UpdateNormalizedColumns()
        {
            var showTransliteration = showTransliterationToolStripMenuItem.Checked;

            // the two words in a row always have the same normalized form (that's why they're shown together), so only
            //  show it once (but their transliterations differ, so show both of those)
            ColumnRomanizedRhs.Visible = showTransliteration;
            foreach (var column in new[] { ColumnRomanizedLhs, ColumnRomanizedRhs })
            {
                column.HeaderText = showTransliteration ? "Transliteration" : "Normalized";
                column.ToolTipText = showTransliteration
                    ? "The transliteration of the word"
                    : "The form of the word that's compared, after the transliteration and the Inconsistency Settings (words that have the same normalized form are shown together)";
            }

            foreach (DataGridViewRow row in dataGridViewCorrectFormPicker.Rows)
            {
                if (row.Cells[cstrVernLhs].Tag is SpellFixerWord sfwLhs)
                    row.Cells[cstrTransLhs].Value = showTransliteration ? sfwLhs.TransliteratedValue : sfwLhs.SimplifiedValue;
                if (row.Cells[cstrVernRhs].Tag is SpellFixerWord sfwRhs)
                    row.Cells[cstrTransRhs].Value = showTransliteration ? sfwRhs.TransliteratedValue : sfwRhs.SimplifiedValue;
                WidenColumnsToFit(row);
            }
        }

        protected override void OnShown(EventArgs e)
        {
            base.OnShown(e);

            // since it's fairly time-consuming determining the ambiguities, do it in the background
#if !UseBackgroundWorker
            StartBundleProcessing();
#else
            backgroundWorker.RunWorkerAsync(this);
#endif

            // save column display index so we can re-init next time
            if (Properties.Settings.Default.BulkEditDisplayIndex == null)
                Properties.Settings.Default.BulkEditDisplayIndex = new System.Collections.Specialized.StringCollection();

            foreach (DataGridViewColumn aGridCol in dataGridViewCorrectFormPicker.Columns)
                Properties.Settings.Default.BulkEditDisplayIndex.Insert(aGridCol.DisplayIndex, aGridCol.Name);
            Properties.Settings.Default.Save();
        }

#if false
        public new DialogResult ShowDialog()
        {
            DialogResult res = base.ShowDialog();

            // save column display index so we can re-init next time
            if (Properties.Settings.Default.BulkEditDisplayIndex == null)
                Properties.Settings.Default.BulkEditDisplayIndex = new System.Collections.Specialized.StringCollection();

            foreach (DataGridViewColumn aGridCol in dataGridViewCorrectFormPicker.Columns)
                Properties.Settings.Default.BulkEditDisplayIndex.Insert(aGridCol.DisplayIndex, aGridCol.Name);
            Properties.Settings.Default.Save();

            //// wait for the BackgroundWorker to finish the work.
            //while (this.backgroundWorker.IsBusy)
            //{
            //    System.Diagnostics.Trace.WriteLine("In ShowDialog, IsBusy: true!");
                
            //    // tell it to stop
            //    this.backgroundWorker.CancelAsync();

            //    // Keep UI messages moving, so the form remains 
            //    // responsive during the asynchronous operation.
            //    Application.DoEvents();
            //}

            return res;
        }
#endif

        protected void SetSelectorCellG2G(DataGridViewRow theRow, bool bUpdateStats)
        {
            theRow.Cells[cstrSelector].Value = cstrG2GValue;
            theRow.Cells[cstrVernLhs].Style.ForeColor = colorGood;
            theRow.Cells[cstrVernRhs].Style.ForeColor = colorGood;
            if (bUpdateStats)
            {
                if (theRow.Tag != null)
                    m_nNumBundles--;
                UpdateStats(--m_nNumInconsistencies, m_nNumBundles);
            }
        }

        protected void SetSelectorCellB2G(DataGridViewRow theRow, bool bUpdateStats)
        {
            theRow.Cells[cstrSelector].Value = cstrB2GValue;
            theRow.Cells[cstrVernLhs].Style.ForeColor = colorBad;
            theRow.Cells[cstrVernRhs].Style.ForeColor = colorGood;
            if (bUpdateStats)
            {
                if (theRow.Tag != null)
                    m_nNumBundles--;
                UpdateStats(--m_nNumInconsistencies, m_nNumBundles);
            }
        }

        protected void SetSelectorCellG2B(DataGridViewRow theRow, bool bUpdateStats)
        {
            // if the supposed good form is already a bad form or if the supposed bad form
            //  is already a good form, then the user should be warned
            var sfwGood = (SpellFixerWord)theRow.Cells[cstrVernLhs].Tag;
            var sfwBad = (SpellFixerWord)theRow.Cells[cstrVernRhs].Tag;
            var transitiveBadSpelling = string.Empty;

            if (m_mapBad2GoodWords.ContainsKey(sfwGood.Value))
            {
                var transitiveGoodSpelling = m_mapBad2GoodWords[sfwGood.Value];
                var res = MessageBox.Show(
                    $"The 'good' word '{sfwGood.Value}' is already assigned as a bad spelling for the good spelling '{transitiveGoodSpelling}'.\n\nClick 'Yes' to copy it to the clipboard so you can edit the Good/Bad pair and paste it as the correct Good spelling.",
                    "Warning: Good Spelling Already Assigned as Bad",
                    MessageBoxButtons.YesNoCancel,
                    MessageBoxIcon.Warning);
                if (res == DialogResult.Yes)
                {
                    Clipboard.SetText(transitiveGoodSpelling);
                    return;
                }
                else if (res == DialogResult.Cancel)
                    return;
            }
            else if (!String.IsNullOrEmpty(transitiveBadSpelling = m_mapBad2GoodWords.FirstOrDefault(b2g => b2g.Value == sfwBad.Value).Key))
            {
                var res = MessageBox.Show(
                    $"The 'bad' word '{sfwBad.Value}' is already assigned as the good spelling for another (badly spelled) word '{transitiveBadSpelling}'.\n\nClick 'Yes' to copy the good spelling word to the clipboard and edit the other spelling correction so you can correct its Good spelling.",
                    "Warning: Bad Spelling Already Assigned as Good",
                    MessageBoxButtons.YesNoCancel,
                    MessageBoxIcon.Warning);
                if (res == DialogResult.Yes)
                {
                    Clipboard.SetText(transitiveBadSpelling);
                    return;
                }
                else if (res == DialogResult.Cancel)
                    return;
            }

            theRow.Cells[cstrSelector].Value = cstrG2BValue;
            theRow.Cells[cstrVernLhs].Style.ForeColor = colorGood;
            theRow.Cells[cstrVernRhs].Style.ForeColor = colorBad;
            if (bUpdateStats)
            {
                if (theRow.Tag != null)
                    m_nNumBundles--;
                UpdateStats(--m_nNumInconsistencies, m_nNumBundles);
            }
        }

        protected void SetSelectorCellSelect(DataGridViewRow theRow, bool bUpdateStats)
        {
            theRow.Cells[cstrSelector].Value = cstrSelectValue;
            theRow.Cells[cstrVernLhs].Style.ForeColor = Color.Black;
            theRow.Cells[cstrVernRhs].Style.ForeColor = Color.Black;
            if (bUpdateStats)
            {
                if (theRow.Tag != null)
                    m_nNumBundles++;
                UpdateStats(++m_nNumInconsistencies, m_nNumBundles);
            }
        }

        /// <summary>
        /// Sets the choice for the row (one of the cstr*Value values), keeping the statistics right (i.e. only changing
        /// them when going from undecided ('Select') to a choice, or back)
        /// </summary>
        protected void SetChoice(DataGridViewRow theRow, string strValue)
        {
            if (theRow == null)
                return;
            var wasUndecided = (cstrSelectValue == (string)theRow.Cells[cstrSelector].Value);
            var isUndecided = (strValue == cstrSelectValue);
            var bUpdateStats = (wasUndecided != isUndecided);
            switch (strValue)
            {
                case cstrB2GValue:
                    SetSelectorCellB2G(theRow, bUpdateStats);
                    break;
                case cstrG2BValue:
                    SetSelectorCellG2B(theRow, bUpdateStats);
                    break;
                case cstrG2GValue:
                    SetSelectorCellG2G(theRow, bUpdateStats);
                    break;
                default:
                    SetSelectorCellSelect(theRow, bUpdateStats);
                    break;
            }
        }

        private bool CanUndoSplit(DataGridViewRow theRow)
        {
            return (theRow.Cells[cstrVernRhs].Value == null)
                && ((theRow.Index + 1) < dataGridViewCorrectFormPicker.Rows.Count)
                && (dataGridViewCorrectFormPicker.Rows[theRow.Index + 1].Cells[cstrVernRhs].Value == null);
        }

        private bool CanSplit(DataGridViewRow theRow)
        {
            return !String.IsNullOrEmpty((string)theRow.Cells[cstrVernLhs].Value)
                && !String.IsNullOrEmpty((string)theRow.Cells[cstrVernRhs].Value);
        }

        /// <summary>
        /// Shows the right-click menu for the middle ('Select') column of the given row
        /// </summary>
        private void ShowSelectorMenu(DataGridViewRow theRow, Point screenLocation)
        {
            _menuRow = theRow;
            SelectRowIndex(theRow.Index);

            var hasBoth = CanSplit(theRow);
            var value = (string)theRow.Cells[cstrSelector].Value;
            _miRightIsCorrect.Enabled = hasBoth;
            _miLeftIsCorrect.Enabled = hasBoth;
            _miBothCorrect.Enabled = !String.IsNullOrEmpty((string)theRow.Cells[cstrVernLhs].Value);
            _miRightIsCorrect.Checked = (value == cstrB2GValue);
            _miLeftIsCorrect.Checked = (value == cstrG2BValue);
            _miBothCorrect.Checked = (value == cstrG2GValue);
            _miUndecided.Enabled = (value != cstrSelectValue);
            _miSplit.Enabled = hasBoth;
            _miUndoSplit.Enabled = CanUndoSplit(theRow);
            _miEdit.Enabled = (theRow.Cells[cstrVernLhs].Tag != null);
            _miWhy.Enabled = hasBoth;
            _menuSelector.Show(screenLocation);
        }

        /// <summary>
        /// Splits a row into two (both words become left-hand words in their own rows), so the user can give each of them
        /// a correction separately (e.g. when both are misspelled)
        /// </summary>
        private void SplitRow(DataGridViewRow theRow)
        {
            if ((theRow == null) || !CanSplit(theRow))
                return;

            // in this scenerio, both are probably bad, so put them both as lhs in their own rows.
            // but first take them out of any multi-line set (since that'll be too hard to manage)
            DataGridViewCell theSelectorCell = theRow.Cells[cstrSelector];
            SpellFixerWord sfwBadLhs = (SpellFixerWord)theRow.Cells[cstrVernLhs].Tag;
            SpellFixerWord sfwBadRhs = (SpellFixerWord)theRow.Cells[cstrVernRhs].Tag;
            int nDistanceToSfwsRow = (int)theSelectorCell.Tag;
            DataGridViewRow theSfwsRow = (nDistanceToSfwsRow == 0) ? theRow :
                dataGridViewCorrectFormPicker.Rows[theRow.Index + nDistanceToSfwsRow];
            SpellFixerWords sfws = (SpellFixerWords)theSfwsRow.Tag;
            System.Diagnostics.Debug.Assert(sfws != null);
            sfws.Remove(sfwBadLhs);
            sfws.Remove(sfwBadRhs);
            if (sfws.Count == 0)
                theSfwsRow.Tag = null;  // if that was the last one

            // (a split row is undecided, but count it as such only once)
            SetChoice(theRow, cstrSelectValue);

            // now write these two in their own rows
            WriteCellData(theRow, sfwBadRhs, null);
            theRow.Cells[cstrSelector].Tag = 0;
            theRow.DividerHeight = cnBundleDividerHeight;

            // insert a row right here for the second one.
            int nNewIndex = theRow.Index;
            dataGridViewCorrectFormPicker.Rows.Insert(nNewIndex, 1);
            DataGridViewRow theNextRow = dataGridViewCorrectFormPicker.Rows[nNewIndex];

            // write it out.
            WriteCellData(theNextRow, sfwBadLhs, null);
            theNextRow.Cells[cstrSelector].Tag = 0;
            theNextRow.DividerHeight = cnBundleDividerHeight;

            // select the first row (so we don't change the selection (which apparently goes to the
            //  newly inserted row otherwise) so that Ctrl+Z then will undo it (see PreviewKeyDown)
            theNextRow.Selected = true;
        }

        /// <summary>
        /// Brings up the 'Add New Replacement Rule' dialog for the row's words, so the user can type in the correct
        /// spelling (or just see the characters in each word)
        /// </summary>
        private void EditSpellings(DataGridViewRow theRow)
        {
            if (theRow == null)
                return;

            DataGridViewCell theSelectorCell = theRow.Cells[cstrSelector];

            // typically, when editing, the left is bad and the right is good.
            string strBadColumnName = cstrVernLhs, strGoodColumnName = cstrVernRhs;

            // however, if the G2B arrow is selected, then swap them around and make the left side good
            //  (except, this fails to work when the right-side is null)
            bool bLeftIsGood = (cstrG2BValue == (string)theSelectorCell.Value) && (theRow.Cells[cstrVernRhs].Value != null);
            if (bLeftIsGood)
            {
                strBadColumnName = cstrVernRhs;
                strGoodColumnName = cstrVernLhs;
            }

            string strGoodValue = (string)theRow.Cells[strGoodColumnName].Value;
            SpellFixerWord sfwBad = (SpellFixerWord)theRow.Cells[strBadColumnName].Tag;

            if (String.IsNullOrEmpty(strGoodValue))
                strGoodValue = sfwBad.Value;

            QueryGoodSpelling aQuery = new QueryGoodSpelling(m_project.Font);
            if (aQuery.ShowDialog(sfwBad.Value, strGoodValue, sfwBad.Value, false, false) == DialogResult.OK)
            {
                // check in case they changed the bad spelling as well...
                if (aQuery.BadSpelling != sfwBad.Value)
                {
                    sfwBad.Value = aQuery.BadSpelling;
                    sfwBad.InitializeNonStaticData(m_project);
                }

                SpellFixerWord sfwGood = (SpellFixerWord)theRow.Cells[strGoodColumnName].Tag;
                if (sfwGood == null)    // could be empty or full
                {
                    sfwGood = m_project.GetNewSpellFixerWord(aQuery.GoodSpelling, null);
                }
                else
                {
                    sfwGood.Value = aQuery.GoodSpelling;
                    sfwGood.InitializeNonStaticData(m_project);
                }

                // write out the new values
                var wasUndecided = (cstrSelectValue == (string)theSelectorCell.Value);
                if (bLeftIsGood)
                {
                    WriteCellData(theRow, sfwGood, sfwBad);
                    SetSelectorCellG2B(theRow, wasUndecided);
                }
                else
                {
                    WriteCellData(theRow, sfwBad, sfwGood);
                    SetSelectorCellB2G(theRow, wasUndecided);
                }

                // update the sfws collection
                int nDistanceToSfwsRow = (int)theSelectorCell.Tag;
                DataGridViewRow theSfwsRow = (nDistanceToSfwsRow == 0) ? theRow :
                    dataGridViewCorrectFormPicker.Rows[theRow.Index + nDistanceToSfwsRow];
                SpellFixerWords sfws = (SpellFixerWords)theSfwsRow.Tag;
                if (sfws == null)
                {
                    sfws = new SpellFixerWords(2);
                    sfws.Add(sfwGood);
                    sfws.Add(sfwBad);
                    theSfwsRow.Tag = sfws;
                }
                else
                {
                    System.Diagnostics.Debug.Assert(sfws.Contains(sfwBad));
                    System.Diagnostics.Debug.Assert(sfws.Contains(sfwGood));
                }
            }
        }

        /// <summary>
        /// Shows which Inconsistency Settings made the row's words look alike, and lets the user turn any of them off
        /// (in which case the choices made so far are saved and the search starts over from scratch)
        /// </summary>
        private void ShowWhyTogether(DataGridViewRow theRow)
        {
            var sfwLhs = theRow?.Cells[cstrVernLhs].Tag as SpellFixerWord;
            var sfwRhs = theRow?.Cells[cstrVernRhs].Tag as SpellFixerWord;
            if (sfwLhs == null)
                return;

            customToolTip.HideToolTip();
            using (var dlg = new RuleTraceForm(m_project, sfwLhs.Value, sfwRhs?.Value))
            {
                if (dlg.ShowDialog(this) != DialogResult.OK)
                    return;

                var names = dlg.UncheckedDistinctionNames;
                if (names.Count > 0)
                    RestartFromScratch(() => m_project.DisableAmbiguityRules(names));
            }
        }

        /// <summary>
        /// Brings up the project settings dialog. If the user changes something, offers to save the choices made so far
        /// and look for inconsistencies again from scratch (since the bundles shown may no longer be right)
        /// </summary>
        private void EditProject()
        {
            customToolTip.HideToolTip();
            StopBackgroundWorker();     // (the words' normalized forms may change)

            bool changed = false;
            try
            {
                changed = m_project.EditProject(false);
            }
            catch (Exception ex)
            {
                MessageBox.Show(this, $"Unable to edit the project: {ex.Message}", CscProject.ApplicationCaption);
            }

            if (changed)
            {
                var res = MessageBox.Show(this,
                    "If you changed the Inconsistency Settings, the words shown here may no longer be the ones that the new settings would show." +
                    Environment.NewLine + Environment.NewLine +
                    "Do you want to save the choices you've made so far, and look for inconsistencies again from scratch with the new settings?",
                    CscProject.ApplicationCaption, MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                if (res == DialogResult.Yes)
                {
                    RestartFromScratch(null);
                    return;
                }
            }

            // otherwise, carry on looking thru the rest of the words (if any)
            if (m_mapWordsToCheck.Count > 0)
                StartBundleProcessing();
        }

        /// <summary>
        /// Saves the choices the user has made (as File, Save does), clears the grid, and searches all of the project's
        /// Words to Check again (e.g. after the Inconsistency Settings have been changed)
        /// </summary>
        /// <param name="beforeRestart">something to do after the choices are saved and before searching again (e.g. turning off some settings)</param>
        private void RestartFromScratch(Action beforeRestart)
        {
            customToolTip.HideToolTip();
            Cursor = Cursors.WaitCursor;
            try
            {
                StopBackgroundWorker();
                ProcessChosenRecords(true);
                beforeRestart?.Invoke();

                dataGridViewCorrectFormPicker.Rows.Clear();
                m_nNumBundles = 0;
                m_nNumInconsistencies = 0;
                m_rowLastSelected = null;
                m_nLastSelectedIndex = 0;
                m_bFirstTime = true;
                _examplesPane?.ShowExamples(null, null, true);

                m_project.RefillWordsToCheck(m_mapWordsToCheck);
                UpdateStats(0, 0);
            }
            catch (Exception ex)
            {
                MessageBox.Show(this, $"Unable to start over: {ex.Message}", CscProject.ApplicationCaption);
                return;
            }
            finally
            {
                Cursor = Cursors.Default;
            }

            StartBundleProcessing();
        }

        protected void ProcessCellClick(DataGridViewCellMouseEventArgs e)
        {
            DataGridViewRow theRow = dataGridViewCorrectFormPicker.Rows[e.RowIndex];
            DataGridViewCell theSelectorCell = theRow.Cells[cstrSelector];
            string strColumnName = dataGridViewCorrectFormPicker.Columns[e.ColumnIndex].Name;

#if UsingJump2Toolbox
            // Ctrl+Click on the vern field is Jump2Tbx
            if (ModifierKeys == Keys.Control)
            {
                string strJumpTarget = null;
                if ((strColumnName == cstrVernLhs) && (theRow.Cells[cstrVernLhs].Value != null))
                    strJumpTarget = (string)theRow.Cells[cstrVernLhs].Value;
                else if ((strColumnName == cstrVernRhs) && (theRow.Cells[cstrVernRhs].Value != null))
                    strJumpTarget = (string)theRow.Cells[cstrVernRhs].Value;

                if (!String.IsNullOrEmpty(strJumpTarget))
                    CscProject.Jump2Toolbox(strJumpTarget);
            }
            else 
#endif
            if (strColumnName == cstrSelector)
            {
                if (e.Button == MouseButtons.Left)
                {
                    SetChoice(theRow, (cstrSelectValue == (string)theSelectorCell.Value) ? cstrG2GValue : cstrSelectValue);
                }
                else if (e.Button == MouseButtons.Right)
                {
                    // right-click: a menu of the things that can be done with this row (choosing either word as
                    //  the correct one, splitting the row (or undoing that), editing, and seeing why they're together)
                    ShowSelectorMenu(theRow, Cursor.Position);
                }
            }
            else if ((strColumnName == cstrVernLhs)
                || (strColumnName == cstrTransLhs)
                || (strColumnName == cstrCountLhs))
            {
                if (e.Button == MouseButtons.Left)
                {
                    if (theRow.Cells[cstrVernRhs].Value != null)
                        SetChoice(theRow, cstrG2BValue);
                }
                else
                {
                    // in this case, we probably want to make the cell under the mouse the right-hand side (see help)
                    //  (but only if there are more than 3 ambiguities (i.e. don't do it for a single row; no need)
                    // the sfws is in the aGridRow.Cells[cstrSelector].Tag'th row below
                    int nIndex = theRow.Index;
                    nIndex += (int)theSelectorCell.Tag;
                    DataGridViewRow rowSfws = dataGridViewCorrectFormPicker.Rows[nIndex];
                    SpellFixerWords sfws = (SpellFixerWords)rowSfws.Tag;

                    if ((sfws != null) && (sfws.Count > 2))
                    {
                        DataGridViewCell theLhsVernCell = theRow.Cells[cstrVernLhs];
                        SpellFixerWord sfwNewHighest = (SpellFixerWord)theLhsVernCell.Tag;
                        System.Diagnostics.Debug.Assert(sfwNewHighest.Value == (string)theLhsVernCell.Value);

                        // just do it (i.e. I've removed the MsgBox asking if the user wants it)
                        sfws.Remove(sfwNewHighest);
                        sfws.Insert(0, sfwNewHighest);

                        SpellFixerWord sfwMostLikely = sfws[0];
                        int nNumRows = (sfws.Count - 1);
                        nIndex = rowSfws.Index - (nNumRows - 1);
                        DataGridViewRow aGridRow = null;
                        for (int i = 0; i < nNumRows; i++)
                        {
                            SpellFixerWord sfw = sfws[i + 1];
                            aGridRow = dataGridViewCorrectFormPicker.Rows[i + nIndex];
                            WriteCellData(aGridRow, sfw, sfwMostLikely);
                            aGridRow.Cells[cstrSelector].Tag = sfws.Count - i - 2;
                        }
                        aGridRow.Tag = sfws;
                    }
                }
            }
            else if (strColumnName == cstrVernRhs)
            {
                // if the vern cell is clicked AND null, then either the left or right mouse button means 'edit'.
                // Otherwise, the left mouse button means 'select as good' and 'right' means 'edit'
                if ((theRow.Cells[cstrVernRhs].Value == null) || (e.Button == MouseButtons.Right))
                    EditSpellings(theRow);
                else
                    SetChoice(theRow, cstrB2GValue);
            }
            else if ((strColumnName == cstrTransRhs)
               || (strColumnName == cstrCountRhs))
            {
                if (theRow.Cells[cstrVernRhs].Value != null)
                    SetChoice(theRow, cstrB2GValue);
            }
        }

        protected void FakeUndo(DataGridViewRow theRow, DataGridViewCell theSelectorCell)
        {
            // undo the split: i.e. take the next row's vern form and move it to this rows rhs.
            System.Diagnostics.Debug.Assert((theRow.Index + 1) < dataGridViewCorrectFormPicker.Rows.Count);
            DataGridViewRow theNextRow = dataGridViewCorrectFormPicker.Rows[theRow.Index + 1];
            SpellFixerWord sfwLhs = (SpellFixerWord)theRow.Cells[cstrVernLhs].Tag;
            SpellFixerWord sfwRhs = (SpellFixerWord)theNextRow.Cells[cstrVernLhs].Tag;
            int nDistanceToSfwsRow = (int)theSelectorCell.Tag;
            if (nDistanceToSfwsRow == 1)
            {
                // in this case, it means we're about to clobber the row that has the bundle
                SpellFixerWords sfws = (SpellFixerWords)theNextRow.Tag;
                theRow.Tag = sfws;
                theSelectorCell.Tag = --nDistanceToSfwsRow;
            }
            else if (nDistanceToSfwsRow == 0)
            {
                if (theRow.Tag == null)
                {
                    SpellFixerWords sfws = new SpellFixerWords(2);
                    sfws.Add(sfwRhs);
                    sfws.Add(sfwLhs);
                    theRow.Tag = sfws;
                    System.Diagnostics.Debug.Assert(0 == (int)theSelectorCell.Tag);
                }
                else
                    System.Diagnostics.Debug.Assert(false, "call Bob: I wasn't expecting this to be non-null!");
            }

            // then adjust the index to the sfws row
            dataGridViewCorrectFormPicker.Rows.RemoveAt(theNextRow.Index);
            WriteCellData(theRow, sfwLhs, sfwRhs);
        }

        private void dataGridViewCorrectFormPicker_CellMouseUp(object sender, DataGridViewCellMouseEventArgs e)
        {
            if ((e.RowIndex < 0) || (e.RowIndex >= dataGridViewCorrectFormPicker.Rows.Count)
                || (e.ColumnIndex < 0) || (e.ColumnIndex >= dataGridViewCorrectFormPicker.Columns.Count))
                return;

            try
            {
                ProcessCellClick(e);
            }
            catch (Exception ex)
            {
                MessageBox.Show(String.Format("Mouse Up: Couldn't handle request! Reason: {0}", ex.Message));
            }
        }

        private void ProcessChosenRecords(bool bRemoveRow)
        {
            Cursor cur = Cursor;
            Cursor = Cursors.WaitCursor;

            // in this case, go thru the rows that have been "selected" and process them (i.e. either
            //  put them in the KnownGood list or the Bad2Good list). For the rest, put them back
            //  into the map so it gets saved for later processing.
            int nIndex = 0;
            while (nIndex < dataGridViewCorrectFormPicker.Rows.Count)
            {
                DataGridViewRow aGridRow = dataGridViewCorrectFormPicker.Rows[nIndex];
                DataGridViewCell theSelectorCell = aGridRow.Cells[cstrSelector];
                if (cstrB2GValue == (string)theSelectorCell.Value)
                {
                    // assign the correct spelling fix for Bad2Good pair
                    string strBadSpelling = (string)aGridRow.Cells[cstrVernLhs].Value;
                    SpellFixerWord sfwReplacement = (SpellFixerWord)aGridRow.Cells[cstrVernRhs].Tag;
                    if( sfwReplacement != null )
                        m_project.AssignCorrectSpelling(strBadSpelling, sfwReplacement.Value, false, sfwReplacement.ContextStrings, true);
                }
                else if (cstrG2BValue == (string)theSelectorCell.Value)
                {
                    // assign the correct spelling fix for Bad2Good pair (but in reverse)
                    string strBadSpelling = (string)aGridRow.Cells[cstrVernRhs].Value;
                    SpellFixerWord sfwReplacement = (SpellFixerWord)aGridRow.Cells[cstrVernLhs].Tag;
                    if (sfwReplacement != null)
                        m_project.AssignCorrectSpelling(strBadSpelling, sfwReplacement.Value, false, sfwReplacement.ContextStrings, true);
                }
                else if (cstrG2GValue == (string)theSelectorCell.Value)
                {
                    // both are good, so add them to the dictionary
                    SpellFixerWord sfwGood = (SpellFixerWord)aGridRow.Cells[cstrVernLhs].Tag;
                    if (sfwGood != null)
                        m_project.AddToDictionary(sfwGood);

                    sfwGood = (SpellFixerWord)aGridRow.Cells[cstrVernRhs].Tag;
                    if (sfwGood != null)
                        m_project.AddToDictionary(sfwGood);
                }
                else if (!bRemoveRow) // probably cstrSelectValue, but if not... don't know what to do with it anyway
                {
                    // this means that the user hasn't dealt with these words yet, so put them back 
                    //  into the map to be saved for next invocation.
                    SpellFixerWord sfwGood = (SpellFixerWord)aGridRow.Cells[cstrVernLhs].Tag;
                    if ((sfwGood != null) && !m_mapWordsToCheck.ContainsKey(sfwGood.Value))
                        m_mapWordsToCheck.Add(sfwGood);
                    sfwGood = (SpellFixerWord)aGridRow.Cells[cstrVernRhs].Tag;
                    if ((sfwGood != null) && !m_mapWordsToCheck.ContainsKey(sfwGood.Value))
                        m_mapWordsToCheck.Add(sfwGood);

                    // go to next one
                    nIndex++;
                    continue;
                }
                else
                {
                    nIndex++;
                    continue;
                }

                if (bRemoveRow)
                    dataGridViewCorrectFormPicker.Rows.RemoveAt(nIndex);
                else
                    nIndex++;
            }

            m_project.SaveProjectData();

            Cursor = cur;
        }

        private void showTransliterationToolStripMenuItem_CheckStateChanged(object sender, EventArgs e)
        {
            Properties.Settings.Default.ShowTransliteration = this.showTransliterationToolStripMenuItem.Checked;
            Properties.Settings.Default.Save();
            UpdateColumnSpans();
            UpdateNormalizedColumns();
        }

        private void showCountToolStripMenuItem_CheckStateChanged(object sender, EventArgs e)
        {
            Properties.Settings.Default.ShowCountOnBulk = this.showCountToolStripMenuItem.Checked;
            Properties.Settings.Default.Save();
            UpdateColumnSpans();            
        }

        private void UpdateColumnSpans()
        {
            dataGridViewCorrectFormPicker.Columns[cstrCountLhs].Visible = showCountToolStripMenuItem.Checked;
            dataGridViewCorrectFormPicker.Columns[cstrCountRhs].Visible = showCountToolStripMenuItem.Checked;
        }

        private void dataGridViewCorrectFormPicker_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            // (e.g. before the first bundle has been found)
            if ((m_rowLastSelected == null) || (m_rowLastSelected.Index < 0))
                return;

            if (    e.Control
                &&  (e.KeyCode == Keys.Z)
                &&  CanUndoSplit(m_rowLastSelected))
            {
                FakeUndo(m_rowLastSelected, m_rowLastSelected.Cells[cstrSelector]);
            }
            else if ((e.KeyCode == Keys.Apps) || (e.Shift && (e.KeyCode == Keys.F10)))
            {
                // the keyboard way to get the right-click menu (shown under the middle cell of the selected row)
                var rect = dataGridViewCorrectFormPicker.GetCellDisplayRectangle(ColumnSelector.Index, m_rowLastSelected.Index, false);
                ShowSelectorMenu(m_rowLastSelected, dataGridViewCorrectFormPicker.PointToScreen(new Point(rect.Left, rect.Bottom)));
            }
            else
            {
                DataGridViewCell theSelectorCell = m_rowLastSelected.Cells[cstrSelector];
                var hasRhs = (m_rowLastSelected.Cells[cstrVernRhs].Value != null);
                switch (e.KeyData)
                {
                    case Keys.Right:
                        if (hasRhs)
                            SetChoice(m_rowLastSelected, cstrB2GValue);
                        break;

                    case Keys.Left:
                        if (hasRhs)
                            SetChoice(m_rowLastSelected, cstrG2BValue);
                        break;

                    case Keys.Space:
                        SetChoice(m_rowLastSelected, (cstrSelectValue == (string)theSelectorCell.Value) ? cstrG2GValue : cstrSelectValue);
                        break;

                    default:
                        return;
                }

                SelectRowIndex(m_nLastSelectedIndex + 1);
            }
        }

        private void SelectRowIndex(int nIndex)
        {
            foreach (DataGridViewRow aRow in dataGridViewCorrectFormPicker.SelectedRows)
                aRow.Selected = false;
            if( (nIndex >= 0) && (nIndex < dataGridViewCorrectFormPicker.RowCount))
                dataGridViewCorrectFormPicker.Rows[nIndex].Selected = true;
        }

        private void dataGridViewCorrectFormPicker_SelectionChanged(object sender, EventArgs e)
        {
            if (m_rowLastSelected != null)
            {
                m_rowLastSelected.Selected = false;
                System.Diagnostics.Debug.Assert(dataGridViewCorrectFormPicker.SelectedRows.Count < 2);
                if (dataGridViewCorrectFormPicker.SelectedRows.Count == 1)
                {
                    m_rowLastSelected = dataGridViewCorrectFormPicker.SelectedRows[0];
                    m_nLastSelectedIndex = m_rowLastSelected.Index;
                }
            }

            // (e.g. when moving thru the rows with the keyboard) show the examples of the selected row's left-hand word
            if (IsExamplesPaneShown && (dataGridViewCorrectFormPicker.SelectedRows.Count == 1)
                && (dataGridViewCorrectFormPicker.SelectedRows[0].Cells[cstrVernLhs].Tag is SpellFixerWord sfw))
            {
                _examplesPane.ShowExamples(sfw.Value, sfw.ContextStrings.Take(m_project.MaxContextStrings));
            }
        }

        // set = running; reset = paused (the words not yet searched stay in m_mapWordsToCheck, so if the user saves
        //  and exits while paused, they're kept for the next session as usual)
        private readonly ManualResetEvent _bundleProcessingRunning = new ManualResetEvent(true);

        private void buttonPause_Click(object sender, EventArgs e)
        {
            var pausing = _bundleProcessingRunning.WaitOne(0);
            if (pausing)
                _bundleProcessingRunning.Reset();
            else
                _bundleProcessingRunning.Set();

            buttonPause.Text = pausing ? "&Resume" : "&Pause";
            UpdateStats(m_nNumInconsistencies, m_nNumBundles);
        }

        private bool IsPaused => !_bundleProcessingRunning.WaitOne(0);

#if !UseBackgroundWorker
        // The search for bundles runs on a background thread, which puts the bundles it finds in this queue; a UI timer
        //  (BundleTimer_Tick) adds them to the grid. (It used to Control.Invoke the UI thread for every word it checked.
        //  Since Windows gives posted messages priority over mouse and keyboard input, that constant stream of them could
        //  keep e.g. a click on the Pause button from being handled for a minute or more--especially in a slow VM)
        private readonly System.Collections.Concurrent.ConcurrentQueue<SpellFixerWords> _foundBundles = new System.Collections.Concurrent.ConcurrentQueue<SpellFixerWords>();
        private volatile bool _bundleProcessingCancelRequested = false;
        private volatile bool _bundleProcessingDone = true;
        private Exception _bundleProcessingError;
        private int _processedCount;
        private Thread _bundleProcessingThread;

        private const int MaxBundlesAddedPerTick = 100;

        private void StartBundleProcessing()
        {
            progressBar.Minimum = 0;
            progressBar.Maximum = m_mapWordsToCheck.Count + 1;
            progressBar.Value = 1;
            progressBar.Visible = true;
            buttonPause.Text = "&Pause";
            buttonPause.Visible = true;
            _bundleProcessingRunning.Set();

            _bundleProcessingCancelRequested = false;
            _bundleProcessingDone = false;
            _bundleProcessingError = null;
            _processedCount = 0;

            // (indexes of the word lists by normalized form, so each word doesn't mean searching the whole lists)
            var knownGoodIndex = new SimplifiedFormIndex(m_mapKnownGoodWords);
            var wordsToCheckIndex = new SimplifiedFormIndex(m_mapWordsToCheck);

            _bundleProcessingThread = new Thread(() =>
            {
                try
                {
                    while (m_mapWordsToCheck.Count > 0 && !_bundleProcessingCancelRequested)
                    {
                        // wait here while paused (but not if the dialog is closing)
                        while (!_bundleProcessingRunning.WaitOne(100))
                            if (_bundleProcessingCancelRequested)
                                break;
                        if (_bundleProcessingCancelRequested)
                            break;

                        var sfwWord = m_mapWordsToCheck.Values.FirstOrDefault();
                        if (sfwWord == null)
                            break;

                        var sfws = m_project.GetAmbiguousBundle(sfwWord, m_mapWordsToCheck, knownGoodIndex, wordsToCheckIndex);

                        if (sfws != null)
                        {
                            var found = false;
                            foreach (SpellFixerWord sfwInBundle in sfws)
                            {
                                if (m_mapWordsToCheck.ContainsKey(sfwInBundle.Value))
                                {
                                    m_mapWordsToCheck.Remove(sfwInBundle.Value);
                                    found = true;
                                }
                            }

                            if (!found)
                            {
                                // (e.g. the word's key isn't its value, because its spelling was edited)
                                m_mapWordsToCheck.Remove(sfwWord.Value);
                                System.Diagnostics.Debug.Fail("didn't find the word we're searching for in the list of ambiguous words!?");
                            }

                            _foundBundles.Enqueue(sfws);
                        }
                        else
                        {
                            m_mapWordsToCheck.Remove(sfwWord.Value);
                        }

                        Interlocked.Increment(ref _processedCount);
                    }
                }
                catch (Exception ex)
                {
                    _bundleProcessingError = ex;
                }
                finally
                {
                    _bundleProcessingDone = true;
                }
            })
            {
                IsBackground = true,
                Name = "SpellFixer bundle search"
            };
            _bundleProcessingThread.Start();
            _bundleTimer.Start();
        }

        /// <summary>
        /// Adds the bundles the background thread has found to the grid (a limited number each time, so the UI stays
        /// responsive) and updates the progress bar
        /// </summary>
        private void BundleTimer_Tick(object sender, EventArgs e)
        {
            // (WriteCellData looks in the Bad2Good list)
            if (m_mapBad2GoodWords.DoingLoadOrSave)
                return;

            AddFoundBundles(MaxBundlesAddedPerTick);
            progressBar.Value = Math.Min(Volatile.Read(ref _processedCount) + 1, progressBar.Maximum);

            if (_bundleProcessingDone && _foundBundles.IsEmpty)
                OnBundleProcessingFinished();
        }

        private void AddFoundBundles(int maxBundles)
        {
            // (stop after a bit, so the UI gets a chance to handle the user's clicks and keys in between)
            var timer = Stopwatch.StartNew();
            for (int i = 0; (i < maxBundles) && _foundBundles.TryDequeue(out var sfws); i++)
            {
                UpdateProgress(sfws);
                if ((maxBundles < Int32.MaxValue) && (timer.ElapsedMilliseconds > 100))
                    break;
            }
        }

        /// <summary>
        /// The text columns used to be auto-sized to 'AllCells', which means the grid re-measured every row each time one
        /// was added (i.e. O(n^2)--it took about a minute to add ~650 rows of Devanagari, during which the dialog didn't
        /// respond). Instead, they're only ever widened, as needed, to fit the rows that are written (see WriteCellData)
        /// </summary>
        private void TurnOffColumnAutoSizing()
        {
            foreach (DataGridViewColumn column in dataGridViewCorrectFormPicker.Columns)
            {
                if (column.AutoSizeMode != DataGridViewAutoSizeColumnMode.AllCells)
                    continue;
                var width = column.Width;
                column.AutoSizeMode = DataGridViewAutoSizeColumnMode.None;
                column.Width = Math.Max(width, column.GetPreferredWidth(DataGridViewAutoSizeColumnMode.ColumnHeader, true));
            }
        }

        private void WidenColumnsToFit(DataGridViewRow aGridRow)
        {
            foreach (var columnName in new[] { cstrVernLhs, cstrTransLhs, cstrCountLhs, cstrVernRhs, cstrTransRhs, cstrCountRhs })
            {
                var cell = aGridRow.Cells[columnName];
                var text = cell.Value?.ToString();
                if (String.IsNullOrEmpty(text))
                    continue;
                var column = dataGridViewCorrectFormPicker.Columns[columnName];
                var font = column.DefaultCellStyle.Font ?? dataGridViewCorrectFormPicker.DefaultCellStyle.Font ?? Font;
                var padding = column.DefaultCellStyle.Padding.Horizontal + 12;
                var width = TextRenderer.MeasureText(text, font).Width + padding;
                if (width > column.Width)
                    column.Width = width;
            }
        }

        private void OnBundleProcessingFinished()
        {
            _bundleTimer.Stop();
            progressBar.Visible = false;
            buttonPause.Visible = false;
            UpdateStats(m_nNumInconsistencies, m_nNumBundles);

            if (_bundleProcessingError != null)
            {
                var error = _bundleProcessingError;
                _bundleProcessingError = null;
                MessageBox.Show(this, $"An error occurred while looking for inconsistencies: {error.Message}{Environment.NewLine}{Environment.NewLine}" +
                                      "(The ones found so far are shown, and you can save your choices as usual.)",
                                CscProject.ApplicationCaption, MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
            else if ((dataGridViewCorrectFormPicker.Rows.Count == 0) && !_bundleProcessingCancelRequested)
                MessageBox.Show(this, "No inconsistently spelled words found with the currently selected Inconsistency Settings!", CscProject.ApplicationCaption);
        }

        /// <summary>
        /// Stops the background search (the words it hasn't gotten to yet stay in m_mapWordsToCheck), and adds any bundles
        /// it found that aren't in the grid yet (so they're saved for the next session if the dialog is closing)
        /// </summary>
        private void StopBackgroundWorker()
        {
            if (_bundleProcessingThread != null && _bundleProcessingThread.IsAlive)
            {
                _bundleProcessingCancelRequested = true;
                _bundleProcessingRunning.Set(); // (in case it's paused)

                // (the thread no longer calls into the UI thread, so it's safe to just wait for it)
                _bundleProcessingThread.Join();
            }

            _bundleTimer.Stop();
            if (!IsDisposed)
            {
                AddFoundBundles(Int32.MaxValue);
                progressBar.Visible = false;
                buttonPause.Visible = false;
            }
        }
#else
        private void backgroundWorker_DoWork(object sender, DoWorkEventArgs e)
        {
            DoWork(sender as BackgroundWorker, e.Argument as BulkAmbiguityPicker);
        }

        protected void DoWork(BackgroundWorker worker, BulkAmbiguityPicker dlg)
        {
            int nTotalCount = dlg.m_mapWordsToCheck.Count;
            do
            {
                foreach (SpellFixerWord sfwWord in dlg.m_mapWordsToCheck.Values)
                {
                    // get all the words ambiguous with this word
                    SpellFixerWords sfws = dlg.m_project.GetAmbiguousBundle(sfwWord, dlg.m_mapWordsToCheck);

                    if (sfws != null)
                    {
                        System.Diagnostics.Trace.WriteLine(String.Format("Found Bundle: {0}, {1} ambiguities, {2} more to check",
                            sfwWord.Value, sfws.Count, dlg.m_mapWordsToCheck.Count));

                        // remove all the words in this bundle from the list of words to check (so 
                        //  we don't hit any of them again)
                        foreach (SpellFixerWord sfwInBundle in sfws)
                            if (dlg.m_mapWordsToCheck.ContainsKey(sfwInBundle.Value))
                                dlg.m_mapWordsToCheck.Remove(sfwInBundle.Value);

                        worker.ReportProgress(100 - (dlg.m_mapWordsToCheck.Count * 100 / nTotalCount), sfws);
                    }
                    else
                        dlg.m_mapWordsToCheck.Remove(sfwWord.Value);

                    break;  // every time (but this is still easier than the alternative
                }
            } while (!worker.CancellationPending && (dlg.m_mapWordsToCheck.Count > 0));
        }

        private void backgroundWorker_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            try
            {
                if (e.UserState == null)
                    return;

                var bundleToLoad = (SpellFixerWords)e.UserState;
#if DEBUG
                var ambiguities = String.Join(", ", bundleToLoad);
                System.Diagnostics.Trace.WriteLine($"Loading Bundle: {ambiguities}");
#endif

                SafeInvoke(dataGridViewCorrectFormPicker, () =>
                {
                    UpdateProgress(bundleToLoad);
                });

                SafeInvoke(progressBar, () => progressBar.Value = e.ProgressPercentage);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        private void backgroundWorker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            progressBar.Visible = false;
            if (dataGridViewCorrectFormPicker.Rows.Count == 0)
                MessageBox.Show("No inconsistently spelled words found with the currently selected Inconsistency Settings!");
        }

        private void StopBackgroundWorker()
        {
            if (backgroundWorker.IsBusy)

            {
                backgroundWorker.CancelAsync();
                while (backgroundWorker.IsBusy)
                    Application.DoEvents();
            }
        }
#endif

        public static void SafeInvoke(Control control, Action action)
        {
            try
            {
                // this can happen if the user is closing the form while we're still processing the translation calls.
                if (control.IsDisposed)
                    return;

                control.Invoke(action);
            }
            catch
            {
            }
        }

        // statistics
        private int m_nNumBundles = 0;
        private int m_nNumInconsistencies = 0;

        protected void UpdateStats(int nInconsistencies, int nBundles)
        {
            var paused = IsPaused ? " (searching for more is paused)" : null;
            SafeInvoke(labelInconsistencies, () => labelInconsistencies.Text = $"{nInconsistencies} (potential) inconsistencies left in {nBundles} bundles{paused}");
        }

        protected void UpdateProgress(SpellFixerWords sfws)
        {
            System.Diagnostics.Debug.Assert(sfws.Count >= 2);
            if (sfws.Count < 2)
                return;

            // for us the UserState is a bundle. Add it to the grid
            SpellFixerWord sfwMostLikely = sfws[0];

            int i = 0, nUndecided = 0;
            DataGridViewRow aGridRow = null;
            while (++i < sfws.Count)
            {
                SpellFixerWord sfw = sfws[i];
                int nIndex = this.dataGridViewCorrectFormPicker.Rows.Add();
                aGridRow = this.dataGridViewCorrectFormPicker.Rows[nIndex];

                WriteCellData(aGridRow, sfw, sfwMostLikely);
                aGridRow.Cells[cstrSelector].Tag = sfws.Count - i - 1;  // indicates the number of rows below that has the row.Tag with the sfws (see below)

                // if both words are already known to be good, start it as 'both correct' (it's still shown, so the user
                //  can change it if one of them is really a misspelling of the other)
                if (m_mapKnownGoodWords.ContainsKey(sfw.Value) && m_mapKnownGoodWords.ContainsKey(sfwMostLikely.Value))
                    SetSelectorCellG2G(aGridRow, false);
                else
                    nUndecided++;
            }

            aGridRow.Tag = sfws;
            aGridRow.DividerHeight = cnBundleDividerHeight;

            // update stats (a bundle counts as left to do until its last row is decided -- see SetSelectorCell*):
            m_nNumInconsistencies += nUndecided;
            if (cstrSelectValue == (string)aGridRow.Cells[cstrSelector].Value)
                m_nNumBundles++;
            UpdateStats(m_nNumInconsistencies, m_nNumBundles);

            if (m_bFirstTime)
            {
                // finally, select the selector cell in the first row
                m_nLastSelectedIndex = 0;
                m_rowLastSelected = dataGridViewCorrectFormPicker.Rows[m_nLastSelectedIndex];
                m_rowLastSelected.Cells[cstrSelector].Selected = true;
                dataGridViewCorrectFormPicker.Focus();
                m_bFirstTime = false;
            }
        }

        protected void WriteCellData(DataGridViewRow aGridRow, SpellFixerWord sfwLhs, SpellFixerWord sfwRhs)
        {
            var maxContextStrings = m_project.MaxContextStrings;
            var lhsWord = sfwLhs.Value;
            aGridRow.Cells[cstrVernLhs].Value = lhsWord;
            aGridRow.Cells[cstrVernLhs].ToolTipText = String.Join(Environment.NewLine, sfwLhs.ContextStrings.Take(maxContextStrings));
            aGridRow.Cells[cstrVernLhs].Tag = sfwLhs;
            aGridRow.Cells[cstrTransLhs].Value = showTransliterationToolStripMenuItem.Checked 
                ? sfwLhs.TransliteratedValue
                : sfwLhs.SimplifiedValue;
            aGridRow.Cells[cstrCountLhs].Value = sfwLhs.Count;

            // if we know one or the other of them is a good word, then indicate that by the color
            if (m_mapKnownGoodWords.ContainsKey(lhsWord))
                aGridRow.Cells[cstrVernLhs].Style.ForeColor = colorGood;
            else if (m_mapBad2GoodWords.ContainsKey(lhsWord))
                aGridRow.Cells[cstrVernLhs].Style.ForeColor = colorBad;

            aGridRow.Cells[cstrSelector].Value = cstrSelectValue;

            if (sfwRhs != null)
            {
                var rhsValue = sfwRhs.Value;
                aGridRow.Cells[cstrVernRhs].Value = rhsValue;
                aGridRow.Cells[cstrVernRhs].ToolTipText = String.Join(Environment.NewLine, sfwRhs.ContextStrings.Take(maxContextStrings));
                aGridRow.Cells[cstrVernRhs].Tag = sfwRhs;
                aGridRow.Cells[cstrTransRhs].Value = showTransliterationToolStripMenuItem.Checked 
                    ? sfwRhs.TransliteratedValue
                    : sfwRhs.SimplifiedValue;
                aGridRow.Cells[cstrCountRhs].Value = sfwRhs.Count;

                if (m_mapKnownGoodWords.ContainsKey(rhsValue))
                    aGridRow.Cells[cstrVernRhs].Style.ForeColor = colorGood;
                else if (m_mapBad2GoodWords.ContainsKey(rhsValue))
                    aGridRow.Cells[cstrVernRhs].Style.ForeColor = colorBad;
            }
            else
            {
                aGridRow.Cells[cstrVernRhs].Value = null;
                aGridRow.Cells[cstrVernRhs].ToolTipText = null;
                aGridRow.Cells[cstrVernRhs].Tag = null;
                aGridRow.Cells[cstrTransRhs].Value = null;
                aGridRow.Cells[cstrCountRhs].Value = null;
            }

            WidenColumnsToFit(aGridRow);
        }

        private void saveToolStripMenuItem_Click(object sender, EventArgs e)
        {
            try
            {
                ProcessChosenRecords(true);
            }
            catch (Exception ex)
            {
                MessageBox.Show(String.Format("Save: Couldn't handle request! Reason: {0}", ex.Message));
            }
        }

        private void saveAndExitToolStripMenuItem_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.OK;
            this.Close();
            // it'll all get processed during FormClosing
        }

        private void BulkAmbiguityPicker_FormClosing(object sender, FormClosingEventArgs e)
        {
            try
            {
                StopBackgroundWorker();
                SaveExamplesPaneWidth();
                customToolTip.HideToolTip();
                if (DialogResult != DialogResult.Cancel)
                    ProcessChosenRecords(false);
            }
            catch (Exception ex)
            {
                MessageBox.Show(String.Format("OK: Couldn't handle request! Reason: {0}", ex.Message));
                throw ex;
            }
        }

        private void exitToolStripMenuItem_Click(object sender, EventArgs e)
        {
            // since we'd have to requery the ambiguities (since words may have been added to the 
            //  dictionary by the next time this is called), just dump everything and next time, and 
            //  it will all be re-generated (i.e. clear out the map, just in case it wasn't finished 
            //  loading so nothing gets saved)
            m_mapWordsToCheck.Clear();
            DialogResult = DialogResult.Cancel;
            this.Close();
        }

        private void dataGridViewCorrectFormPicker_CellMouseMove(object sender, DataGridViewCellMouseEventArgs e)
        {
            if (IsExamplesPaneShown)
            {
                // show the examples of the word under the mouse in the side pane (they stay there when the mouse moves
                //  off the words, so the user can e.g. scroll thru them)
                if ((e.RowIndex >= 0) && (e.ColumnIndex >= 0) && !_examplesPane.IsPinned)
                {
                    var columnName = dataGridViewCorrectFormPicker.Columns[e.ColumnIndex].Name;
                    if (((columnName == cstrVernLhs) || (columnName == cstrVernRhs))
                        && (dataGridViewCorrectFormPicker.Rows[e.RowIndex].Cells[e.ColumnIndex].Tag is SpellFixerWord sfw))
                    {
                        _examplesPane.ShowExamples(sfw.Value, sfw.ContextStrings.Take(m_project.MaxContextStrings));
                    }
                }
                return;
            }

            if (e.RowIndex >= 0 && e.ColumnIndex >= 0)
            {
                var cell = dataGridViewCorrectFormPicker.Rows[e.RowIndex].Cells[e.ColumnIndex];
                var tooltipText = cell.ToolTipText;
                if (!string.IsNullOrEmpty(tooltipText))
                {
                    // Show near mouse pointer
                    Point screenPoint = dataGridViewCorrectFormPicker.PointToScreen(e.Location);
                    customToolTip.ShowToolTip(tooltipText, m_project.Font, new Point(0, 0));
                }
                else
                {
                    customToolTip.HideToolTip();
                }
            }
            else
            {
                customToolTip.HideToolTip();
            }
        }

        // Hide tooltip when mouse leaves the grid
        private void dataGridViewCorrectFormPicker_MouseLeave(object sender, EventArgs e)
        {
            customToolTip.HideToolTip();
        }

        protected int m_nReorderLastColumn = 0;
        private void dataGridViewCorrectFormPicker_ColumnDisplayIndexChanged(object sender, DataGridViewColumnEventArgs e)
        {
            if ((e.Column.Name == cstrCountRhs)
                || (e.Column.Name == cstrTransRhs)
                || (e.Column.Name == cstrVernRhs))
            {
                if (e.Column.DisplayIndex < 4)
                {
                    // thanks to: http://forums.microsoft.com/MSDN/ShowPost.aspx?PostID=462805&SiteID=1
                    m_nReorderLastColumn = e.Column.Index;
                    MouseEventHandler mouseEventHandlerToRemove = null;
                    MouseEventHandler mouseEventHandler = delegate(object sender1, MouseEventArgs mouseEventArgs)
                    {
                        this.dataGridViewCorrectFormPicker.Columns[m_nReorderLastColumn].DisplayIndex = 4;
                        this.dataGridViewCorrectFormPicker.MouseUp -= mouseEventHandlerToRemove;
                    };
                    mouseEventHandlerToRemove = mouseEventHandler;
                    dataGridViewCorrectFormPicker.MouseUp += mouseEventHandler;
                }
            }
        }

        protected void UpdateColors()
        {
            foreach (DataGridViewRow aRow in dataGridViewCorrectFormPicker.Rows)
            {
                string strSelectionValue = (string)aRow.Cells[cstrSelector].Value;
                if (strSelectionValue == cstrSelectValue)
                    SetSelectorCellSelect(aRow, false);
                else if (strSelectionValue == cstrB2GValue)
                    SetSelectorCellB2G(aRow, false);
                else if (strSelectionValue == cstrG2BValue)
                    SetSelectorCellG2B(aRow, false);
                else if (strSelectionValue == cstrG2GValue)
                    SetSelectorCellG2G(aRow, false);
                else
                    System.Diagnostics.Debug.Assert(false);
            }
        }

        private void setCorrectSpellingColorToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (colorDialog.ShowDialog() == DialogResult.OK)
            {
                colorGood = colorDialog.Color;
                Properties.Settings.Default.GoodColor = colorGood;
                Properties.Settings.Default.Save();
                UpdateColors();
            }
        }

        private void setIncorrectSpellingColorToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (colorDialog.ShowDialog() == DialogResult.OK)
            {
                colorBad = colorDialog.Color;
                Properties.Settings.Default.BadColor = colorBad;
                Properties.Settings.Default.Save();
                UpdateColors();
            }
        }

        // opens the help document (PDF) in the user's default PDF viewer rather than
        //  embedding a viewer in the dialog. Per the installer sources (silconverters repo:
        //  Installer\SFM Converter MM[\64bit]\SFMConverterMM_MergeModule.wxs and
        //  Installer\SpellFixerEcMM\SpellFixerEc_MergeModule.wxs), this PDF and
        //  SpellingFixer30.dll both land under the same "MergeRedirectFolder" (i.e. the same
        //  INSTALLDIR), in a sibling "Help" subfolder - regardless of whether this dll ends up
        //  hosted by SFMConv.exe (Bulk SFM Converter) or loaded in-process as a COM component
        //  (e.g. by Word via the SpellFixer.dot macro). So resolve it relative to this
        //  assembly's own location rather than Application.StartupPath (which would instead give
        //  the host process's exe folder, e.g. WINWORD.EXE's, in the COM-hosted case).
        private const string cstrHelpFileName = "Help for ConsistentSpellFixer.pdf";

        private void helpToolStripMenuItem_Click(object sender, EventArgs e)
        {
            string strAssemblyDir = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
            string strHelpFilePath = Path.Combine(strAssemblyDir ?? String.Empty, "Help", cstrHelpFileName);
            try
            {
                Process.Start(new ProcessStartInfo(strHelpFilePath) { UseShellExecute = true });
            }
            catch (Exception ex)
            {
                MessageBox.Show(String.Format("Couldn't open Help document '{0}'! Reason: {1}", strHelpFilePath, ex.Message));
            }
        }
    }
}