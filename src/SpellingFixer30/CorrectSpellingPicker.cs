// #define UsingJump2Toolbox

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace SpellingFixer30
{
    internal partial class CorrectSpellingPicker : Form
    {
        protected string m_strCorrection = null;
        protected SpellFixerWord m_sfwCorrection = null;
        protected SpellFixResult m_eUserSelectedButton = SpellFixResult.Cancel;
        protected CscProject m_project = null;
		protected SpellFixerWord m_sfwWord = null;
		protected string m_strContext = null;
		protected SpellFixerWords m_ambiguities = null;

        public CorrectSpellingPicker(CscProject project)
        {
            InitializeComponent();
            m_project = project;

            this.richTextBoxContext.Font = m_project.Font;
            this.listBoxAmbiguities.Font = m_project.Font;

			Properties.Settings.Default.Reload();
			try
			{
				this.checkBoxShowWordInfo.Checked = Properties.Settings.Default.ShowStats;
			}
			catch { }
        }

		protected string DisplayValue(SpellFixerWord aWord)
		{
			return (ShowStatistics) ? String.Format("{0} ({1}, {2})", aWord.Value, aWord.TransliteratedValue, aWord.Count) : aWord.Value;
		}

        public void Show(ref SpellFixerWord sfwWord, string strContext, SpellFixerWords ambiguities)
        {
            // start the background worker to save the project settings
            this.backgroundWorker.RunWorkerAsync(m_project);

			// reset state from last round
			m_sfwCorrection = null;
			m_nLastIndex = -1;
			m_eUserSelectedButton = SpellFixResult.Cancel;
			m_strCorrection = null;

			// initialize our variables for this round
			m_sfwWord = sfwWord;
			m_strContext = strContext.Replace("\r", " ");
			m_strContext = m_strContext.Replace("\n", " ");
			m_strContext = m_strContext.Replace("  ", " ");
			m_ambiguities = ambiguities;
			InitControls();

            base.ShowDialog();

			sfwWord = m_sfwWord;	// in case the user added a suggestion, we have to change it in the caller
			this.backgroundWorker.CancelAsync();

			if (NoteChoice)
			{
				foreach (SpellFixerWord aWord in ambiguities)
					if (aWord.Value == m_strCorrection)
					{
						CorrectSpellingWord = aWord;
						break;
					}
			}

            // Wait for the BackgroundWorker to finish the download.
            while (this.backgroundWorker.IsBusy)
            {
                System.Diagnostics.Trace.WriteLine("In ShowDialog, IsBusy: true!");
                // Keep UI messages moving, so the form remains 
                // responsive during the asynchronous operation.
                Application.DoEvents();
            }
        }

		protected bool ShowStatistics
		{
			get { return this.checkBoxShowWordInfo.Checked; }
		}

		protected bool NoteChoice
		{
			get { return ((UserButtonChoice == SpellFixResult.ChangeOnce) || (UserButtonChoice == SpellFixResult.ChangeAlways)); }
		}

		protected void InitControls()
		{
			if (m_sfwWord != null)
			{
				// put the context string into the RichEdit control and select the word in question
				listBoxAmbiguities.Items.Clear();
				this.richTextBoxContext.Text = (String.IsNullOrEmpty(m_strContext)) ? m_sfwWord.Value : m_strContext;
				int nIndex = this.richTextBoxContext.Find(m_sfwWord.Value, 0, RichTextBoxFinds.None);
				if (nIndex != -1)
				{
					richTextBoxContext.SelectionStart = nIndex;
					richTextBoxContext.SelectionLength = m_sfwWord.Value.Length;
					richTextBoxContext.SelectionColor = Color.Red;
					richTextBoxContext.SelectedText = DisplayValue(m_sfwWord);
				}

				// put the ambiguities in the list view (in order of highest to lowest count)
				foreach (SpellFixerWord aWord in m_ambiguities)
				{
					string strValue = DisplayValue(aWord);
					listBoxAmbiguities.Items.Add(strValue);
				}

				listBoxAmbiguities.SelectedIndex = 0;
			}
		}

        public SpellFixerWord CorrectSpellingWord
        {
            get 
			{
				// shouldn't be asking for it, if it's not set
				System.Diagnostics.Debug.Assert(m_sfwCorrection != null);
				return m_sfwCorrection; 
			}
            set { m_sfwCorrection = value; }
        }

        public SpellFixResult UserButtonChoice
        {
            get { return m_eUserSelectedButton; }
            set { m_eUserSelectedButton = value; }
        }

        private void FinishUp(SpellFixResult eChoice)
        {
			UserButtonChoice = eChoice;

            if (NoteChoice)
            {
                System.Diagnostics.Debug.Assert(listBoxAmbiguities.SelectedIndex != -1);
                m_strCorrection = (string)listBoxAmbiguities.SelectedItem;
                if (ShowStatistics)
                {
                    int nIndex = m_strCorrection.IndexOf(" (");
                    if (nIndex != -1)
                        m_strCorrection = m_strCorrection.Substring(0, nIndex);
                    else
                        System.Diagnostics.Debug.Assert(false, "this shouldn't happen!");
                }
            }

			DialogResult = DialogResult.OK;
			this.Close();
		}

        private void buttonIgnoreOnce_Click(object sender, EventArgs e)
        {
            FinishUp(SpellFixResult.IgnoreOnce);
        }

        private void buttonIgnoreAlways_Click(object sender, EventArgs e)
        {
            FinishUp(SpellFixResult.IgnoreAlways);
        }

        private void buttonAddToDictionary_Click(object sender, EventArgs e)
        {
            FinishUp(SpellFixResult.AddToDictionary);
        }

        private void buttonChange_Click(object sender, EventArgs e)
        {
            FinishUp(SpellFixResult.ChangeOnce);
        }

        private void buttonChangeAll_Click(object sender, EventArgs e)
        {
            FinishUp(SpellFixResult.ChangeAlways);
        }

        private void buttonUndo_Click(object sender, EventArgs e)
        {
            FinishUp(SpellFixResult.Undo);
        }

        private void backgroundWorker_DoWork(object sender, DoWorkEventArgs e)
        {
			DoWork(sender as BackgroundWorker, e.Argument as CscProject);
        }

        protected void DoWork(BackgroundWorker worker, CscProject project)
        {
			while (!worker.CancellationPending && project.SaveNeeded)
				project.SaveData();
        }

        private void backgroundWorker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            System.Diagnostics.Trace.WriteLine("In ShowDialog, background worker finished");
        }

        private void CorrectSpellingPicker_FormClosing(object sender, FormClosingEventArgs e)
        {
            e.Cancel = true;
            Hide();
        }

        private void buttonCancel_Click(object sender, EventArgs e)
        {
            UserButtonChoice = SpellFixResult.Cancel;
            this.Close();
        }

		private void checkBoxShowWordInfo_CheckStateChanged(object sender, EventArgs e)
		{
			InitControls();
			Properties.Settings.Default.ShowStats = checkBoxShowWordInfo.Checked;
			Properties.Settings.Default.Save();
		}

		private void buttonAddSuggestion_Click(object sender, EventArgs e)
		{
			QueryGoodSpelling aQuery = new QueryGoodSpelling(this.richTextBoxContext.Font);
            if (aQuery.ShowDialog(m_sfwWord.Value, m_sfwWord.Value, m_sfwWord.Value, false) == DialogResult.OK)
			{
				// if the user changed it...
				if (m_sfwWord.Value != aQuery.BadSpelling)
				{
					// the clobber the context if it no longer applies
					if (m_strContext.IndexOf(aQuery.BadSpelling) == -1)
						m_strContext = aQuery.BadSpelling;
					m_sfwWord = m_project.GetNewSpellFixerWord(aQuery.BadSpelling, m_strContext);
				}

				m_ambiguities.Insert(0, m_project.GetNewSpellFixerWord(aQuery.GoodSpelling, null));
				InitControls();
			}
		}

		private int m_nLastIndex = -1;

		private void listBoxAmbiguities_MouseMove(object sender, MouseEventArgs e)
		{
			int nIndex = listBoxAmbiguities.IndexFromPoint(new Point(e.X, e.Y));
			if( (nIndex != -1) && (nIndex != m_nLastIndex) )
			{
				m_nLastIndex = nIndex;
				if (m_ambiguities.Count > m_nLastIndex - 1)
				{
					SpellFixerWord sfw = m_ambiguities[m_nLastIndex];
					toolTip.SetToolTip(listBoxAmbiguities, String.Join(Environment.NewLine, sfw.ContextStrings));
				}
			}
		}

        private void buttonOptions_Click(object sender, EventArgs e)
        {
            SFProjectForm dlg = new SFProjectForm(m_project);
            if (dlg.ShowDialog() == DialogResult.OK)
            {
                m_project.Init();
            }
        }

#if UsingJump2Toolbox
        private void richTextBoxContext_MouseUp(object sender, MouseEventArgs e)
        {
            if (ModifierKeys == Keys.Control)
            {
                string strWord = m_sfwWord.Value;
                CscProject.Jump2Toolbox(strWord);
            }
        }

        private void listBoxAmbiguities_MouseUp(object sender, MouseEventArgs e)
        {
            if (ModifierKeys == Keys.Control)
            {
                int nIndex = listBoxAmbiguities.IndexFromPoint(new Point(e.X, e.Y));
                if (nIndex != -1)
                {
                    SpellFixerWord sfw = m_ambiguities[nIndex];
                    string strWord = sfw.Value;
                    CscProject.Jump2Toolbox(strWord);
                }
            }
        }
#endif
    }
}