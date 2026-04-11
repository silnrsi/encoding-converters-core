// #define UsingJump2Toolbox
using System;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
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
                    if (cstrSelectValue == (string)theSelectorCell.Value)
                        SetSelectorCellG2G(theRow, true);
                    else
                        SetSelectorCellSelect(theRow, true);
                }
                // see if we're supposed to "undo" the split (that's done by the else case)
                else if ((theRow.Cells[cstrVernRhs].Value == null)
                    && ((e.RowIndex + 1) < dataGridViewCorrectFormPicker.Rows.Count)
                    && (dataGridViewCorrectFormPicker.Rows[e.RowIndex + 1].Cells[cstrVernRhs].Value == null)
                    )
                {
                    FakeUndo(theRow, theSelectorCell);
                }
                else if (!String.IsNullOrEmpty((string)theRow.Cells[cstrVernLhs].Value)
                    && !String.IsNullOrEmpty((string)theRow.Cells[cstrVernRhs].Value))
                {
                    // in this scenerio, both are probably bad, so put them both as lhs in their own rows.
                    // but first take them out of any multi-line set (since that'll be too hard to manage)
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

                    // now write these two in their own rows
                    WriteCellData(theRow, sfwBadRhs, null);
                    theRow.Cells[cstrSelector].Tag = 0;
                    theRow.DividerHeight = cnBundleDividerHeight;
                    // perhaps I should create empty sfws

                    // insert a row right here for the second one.
                    int nNewIndex = theRow.Index;
                    dataGridViewCorrectFormPicker.Rows.Insert(nNewIndex, 1);
                    DataGridViewRow theNextRow = dataGridViewCorrectFormPicker.Rows[nNewIndex];

                    // write it out.
                    WriteCellData(theNextRow, sfwBadLhs, null);
                    theNextRow.Cells[cstrSelector].Tag = 0;
                    theNextRow.DividerHeight = cnBundleDividerHeight;
                    // perhaps I should create empty sfws

                    // select the first row (so we don't change the selection (which apparently goes to the
                    //  newly inserted row otherwise) so that Ctrl+Z then will undo it (see PreviewKeyDown)
                    theNextRow.Selected = true;
                }
            }
            else if ((strColumnName == cstrVernLhs)
                || (strColumnName == cstrTransLhs)
                || (strColumnName == cstrCountLhs))
            {
                if (e.Button == MouseButtons.Left)
                {
                    SetSelectorCellG2B(theRow, true);
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
                {
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
                    if (aQuery.ShowDialog(sfwBad.Value, strGoodValue, sfwBad.Value, false) == DialogResult.OK)
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
                        if (bLeftIsGood)
                        {
                            WriteCellData(theRow, sfwGood, sfwBad);
                            SetSelectorCellG2B(theRow, true);
                        }
                        else
                        {
                            WriteCellData(theRow, sfwBad, sfwGood);
                            SetSelectorCellB2G(theRow, true);
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
                else
                    SetSelectorCellB2G(theRow, true);
            }
            else if ((strColumnName == cstrTransRhs)
               || (strColumnName == cstrCountRhs))
            {
                SetSelectorCellB2G(theRow, true);
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
            System.Diagnostics.Debug.Assert(m_rowLastSelected != null);

            if (    e.Control 
                &&  (e.KeyCode == Keys.Z)
                &&  (m_rowLastSelected.Cells[cstrVernRhs].Value == null)
                && ((m_rowLastSelected.Index + 1) < dataGridViewCorrectFormPicker.Rows.Count)
                && (dataGridViewCorrectFormPicker.Rows[m_rowLastSelected.Index + 1].Cells[cstrVernRhs].Value == null))
            {
                FakeUndo(m_rowLastSelected, m_rowLastSelected.Cells[cstrSelector]);
            }
            else
            {
                DataGridViewCell theSelectorCell = m_rowLastSelected.Cells[cstrSelector];
                switch (e.KeyData)
                {
                    case Keys.Right:
                        SetSelectorCellB2G(m_rowLastSelected, true);
                        break;

                    case Keys.Left:
                        SetSelectorCellG2B(m_rowLastSelected, true);
                        break;

                    case Keys.Space:
                        if (cstrSelectValue == (string)theSelectorCell.Value)
                            SetSelectorCellG2G(m_rowLastSelected, true);
                        else
                            SetSelectorCellSelect(m_rowLastSelected, true);
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
        }

#if !UseBackgroundWorker
        private volatile bool _bundleProcessingCancelRequested = false;
        private Thread _bundleProcessingThread;
        private ManualResetEvent waitForAllBundlesToFinish;

        private void StartBundleProcessing()
        {
            progressBar.Minimum = 0;
            progressBar.Maximum = m_mapWordsToCheck.Count + 1;
            progressBar.Value = 1;
            progressBar.Visible = true;
            SafeInvoke(progressBar, () => progressBar.Show());

            waitForAllBundlesToFinish = new ManualResetEvent(false);
            _bundleProcessingCancelRequested = false;

            _bundleProcessingThread = new Thread(() =>
            {
                int nTotalCount = m_mapWordsToCheck.Count;
                int processedCount = 0;

                while (m_mapWordsToCheck.Count > 0 && !_bundleProcessingCancelRequested)
                {
                    var sfwWord = m_mapWordsToCheck.Values.FirstOrDefault();
                    if (sfwWord == null)
                        break;

                    var sfws = m_project.GetAmbiguousBundle(sfwWord, m_mapWordsToCheck);

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
                            System.Diagnostics.Debug.Fail("didn't find the word we're searching for in the list of ambiguous words!?");
                        }

                        while (m_mapBad2GoodWords.DoingLoadOrSave)
                        {
                            Thread.Sleep(50);
                        }

                        SafeInvoke(dataGridViewCorrectFormPicker, () =>
                        {
                            UpdateProgress(sfws);
                        });
                    }
                    else
                    {
                        m_mapWordsToCheck.Remove(sfwWord.Value);
                    }

                    processedCount++;
                    SafeInvoke(progressBar, () => progressBar.Value = Math.Min(processedCount + 1, progressBar.Maximum));
                }

                waitForAllBundlesToFinish.Set();
            });
            _bundleProcessingThread.Start();

            // Keep UI responsive while waiting for thread to finish
            while (!waitForAllBundlesToFinish.WaitOne(50))
                Application.DoEvents();

            SafeInvoke(progressBar, () => progressBar.Visible = false);
            if (dataGridViewCorrectFormPicker.Rows.Count == 0)
                MessageBox.Show("No inconsistently spelled words found with the currently selected Inconsistency Settings!");
        }

        private void StopBackgroundWorker()
        {
            if (_bundleProcessingThread != null && _bundleProcessingThread.IsAlive)
            {
                _bundleProcessingCancelRequested = true;

                // Wait for the thread to finish
                while (_bundleProcessingThread.IsAlive)
                {
                    Application.DoEvents();
                    Thread.Sleep(10);
                }
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
            SafeInvoke(labelInconsistencies, () => labelInconsistencies.Text = $"{nInconsistencies} (potential) inconsistencies left in {nBundles} bundles");
        }

        protected void UpdateProgress(SpellFixerWords sfws)
        {
            System.Diagnostics.Debug.Assert(sfws.Count >= 2);
            if (sfws.Count < 2)
                return;

            // for us the UserState is a bundle. Add it to the grid
            SpellFixerWord sfwMostLikely = sfws[0];

            int i = 0;
            DataGridViewRow aGridRow = null;
            while (++i < sfws.Count)
            {
                SpellFixerWord sfw = sfws[i];
                int nIndex = this.dataGridViewCorrectFormPicker.Rows.Add();
                aGridRow = this.dataGridViewCorrectFormPicker.Rows[nIndex];

                WriteCellData(aGridRow, sfw, sfwMostLikely);
                aGridRow.Cells[cstrSelector].Tag = sfws.Count - i - 1;  // indicates the number of rows below that has the row.Tag with the sfws (see below)
            }

            aGridRow.Tag = sfws;
            aGridRow.DividerHeight = cnBundleDividerHeight;

            // update stats:
            m_nNumInconsistencies += sfws.Count - 1;
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
    }
}