using System;
using System.Collections;
using System.ComponentModel;
using System.Windows.Forms;
using System.Diagnostics;               // for Debug.Assert
using System.Collections.Generic;
using ECInterfaces;
using System.Linq;

namespace BackTranslationHelper
{
	/// <summary>
	/// Summary description for TranslatorListForm.
	/// </summary>
	public partial class TranslatorListForm : Form
	{
		private const string OkButtonTextSave = "&Save";

		protected Dictionary<string, string> m_mapLbItems2Tooltips = new Dictionary<string, string>();

        public TranslatorListForm(List<IEncConverter> translators)
        {
            InitializeComponent();

            // put the display names in a list box for the user to choose.
			foreach(var translator in translators)
			{
                listBoxTranslatorNames.Items.Add(translator.Name);
                m_mapLbItems2Tooltips[translator.Name] = translator.ToString();
            }

            // disable the add button (until an implementation type is selected)
            buttonRemove.Enabled = false;

            labelStatic.Text = $"Choose the Translator/EncConverter to remove";
        }

		private List<string> m_strConverterNamesInOrder;

		public List<string>	ConverterNamesInOrder
		{
			get	{ return m_strConverterNamesInOrder; }
		}

		private void buttonRemove_Click(object sender, System.EventArgs e)
		{
			Debug.Assert(this.listBoxTranslatorNames.SelectedItem != null);
			if (buttonRemove.Text != OkButtonTextSave)	// means we're doing Remove instead
				this.listBoxTranslatorNames.Items.Remove(this.listBoxTranslatorNames.SelectedItem);

			m_strConverterNamesInOrder = this.listBoxTranslatorNames.Items.Cast<string>().ToList();
			this.DialogResult = DialogResult.OK;
			this.Close();
		}

		private void listBoxTranslatorNames_SelectedIndexChanged(object sender, EventArgs e)
		{
			this.buttonRemove.Enabled = buttonMoveTranslatorDown.Enabled = buttonMoveTranslatorUp.Enabled = (this.listBoxTranslatorNames.SelectedItem != null);
		}

		private void buttonCancel_Click(object sender, System.EventArgs e)
		{
			m_strConverterNamesInOrder = null;
			this.DialogResult = DialogResult.Cancel;
			this.Close();
        }

        protected int m_nLastTooltipDisplayedIndex = ListBox.NoMatches;
        private void listBoxTranslatorNames_MouseMove(object sender, MouseEventArgs e)
        {
            int nIndex = this.listBoxTranslatorNames.IndexFromPoint(e.X, e.Y);
            if (nIndex != m_nLastTooltipDisplayedIndex)
            {
                m_nLastTooltipDisplayedIndex = nIndex;
                toolTip.Hide(listBoxTranslatorNames);
                if (nIndex != ListBox.NoMatches)
                {
                    timerTooltip.Stop();
                    timerTooltip.Start();
                }
            }
        }

        private void timerTooltip_Tick(object sender, EventArgs e)
        {
            try
            {
                if (m_nLastTooltipDisplayedIndex != ListBox.NoMatches)
                {
                    string strKey = (string)this.listBoxTranslatorNames.Items[m_nLastTooltipDisplayedIndex];

                    string strDescription;
                    if (m_mapLbItems2Tooltips.TryGetValue(strKey, out strDescription))
                        toolTip.SetToolTip(listBoxTranslatorNames, strDescription);
                }
            }
            finally
            {
                timerTooltip.Stop();
            }
        }

		private void buttonMoveTranslatorUp_Click(object sender, EventArgs e)
		{
			var currentIndex = this.listBoxTranslatorNames.SelectedIndex;
			if (currentIndex < 1)
				return;

			var selectedItem = this.listBoxTranslatorNames.SelectedItem;
			this.listBoxTranslatorNames.Items.Remove(selectedItem);
			this.listBoxTranslatorNames.Items.Insert(currentIndex - 1, selectedItem);
			this.listBoxTranslatorNames.SelectedIndex = currentIndex - 1;

			buttonRemove.Text = OkButtonTextSave;
		}

		private void buttonMoveTranslatorDown_Click(object sender, EventArgs e)
		{
			var currentIndex = this.listBoxTranslatorNames.SelectedIndex;
			if ((currentIndex >= (this.listBoxTranslatorNames.Items.Count - 1)) || (currentIndex < 0))
				return;

			var selectedItem = this.listBoxTranslatorNames.SelectedItem;
			this.listBoxTranslatorNames.Items.Remove(selectedItem);
			this.listBoxTranslatorNames.Items.Insert(currentIndex + 1, selectedItem);
			this.listBoxTranslatorNames.SelectedIndex = currentIndex + 1;

			buttonRemove.Text = OkButtonTextSave;
		}
	}
}
