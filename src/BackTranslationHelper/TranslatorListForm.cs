using System;
using System.Collections;
using System.ComponentModel;
using System.Windows.Forms;
using System.Diagnostics;               // for Debug.Assert
using System.Collections.Generic;
using ECInterfaces;
using System.Linq;
using SilEncConverters40;
using System.Reflection;

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

            // disable the buttons (until an implementation type is selected)
            buttonRemove.Enabled = buttonEditSelected.Enabled =
				buttonMoveTranslatorDown.Enabled = buttonMoveTranslatorUp.Enabled = false;

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
			this.buttonRemove.Enabled = buttonEditSelected.Enabled = buttonMoveTranslatorDown.Enabled = buttonMoveTranslatorUp.Enabled = (this.listBoxTranslatorNames.SelectedItem != null);
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

		private void buttonEditSelected_Click(object sender, EventArgs e)
		{
			if (listBoxTranslatorNames.SelectedIndex == -1)
				return;

			// check for this item in the repository collection (should exist, but sometimes, doesn't!)
			var index = listBoxTranslatorNames.SelectedIndex;
			var converterName = listBoxTranslatorNames.SelectedItem as String;
			if (!DirectableEncConverter.EncConverters.ContainsKey(converterName))
				return;

			var theEC = DirectableEncConverter.EncConverters[converterName];
			Debug.Assert(theEC != null);

			// get the name (but if it's a temporary name, then just start from scratch (with no name)
			string strFriendlyName = theEC.Name;
			if (strFriendlyName.IndexOf(EncConverters.cstrTempConverterPrefix) == 0)
				strFriendlyName = null;

			bool bTooltipActive = toolTip.Active;
			toolTip.Active = false;
			if (DirectableEncConverter.EncConverters.AutoConfigureEx(theEC, theEC.ConversionType, ref strFriendlyName, theEC.LeftEncodingID, theEC.RightEncodingID))
			{
				// since even the name could theoretically change, remove the existing one and ...
				if (m_mapLbItems2Tooltips.ContainsKey(converterName))
					m_mapLbItems2Tooltips.Remove(converterName);

				// remove the old one from the listbox also
				if (this.listBoxTranslatorNames.Items.Contains(converterName))
					this.listBoxTranslatorNames.Items.Remove(converterName);

				// get a possible new name
				Debug.Assert(!String.IsNullOrEmpty(strFriendlyName));
				converterName = strFriendlyName;

				this.listBoxTranslatorNames.Items.Insert(index, converterName);
				this.listBoxTranslatorNames.SelectedIndex = index;

				// make it visible
				this.listBoxTranslatorNames.TopIndex = index;

				// also update the tooltip and fixup the button state
				// (but first re-get the converter since (for some types, e.g. CmpdEncConverter)
				//  it might be totally different.
				theEC = DirectableEncConverter.EncConverters[converterName];
				if (theEC != null)
					m_mapLbItems2Tooltips[converterName] = theEC.ToString();

				// finally, make this a Save situation:
				buttonRemove.Text = OkButtonTextSave;
			}
			toolTip.Active = bTooltipActive;
		}
	}
}
