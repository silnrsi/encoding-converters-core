// Created by Jim Kornelsen on Dec 6 2011
//
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.IO;
using ECInterfaces;
using System.Text.RegularExpressions;
using static SilEncConverters40.NetRegexEncConverter;
using System.Linq;

//uncomment the following line for verbose debugging output using Console.WriteLine
//#define VERBOSE_DEBUGGING

namespace SilEncConverters40
{
    public partial class NetRegexAutoConfigDialog : SilEncConverters40.AutoConfigDialog
    {
		public const int ColumnIndexEnabled = 0;
		public const int ColumnIndexFindWhat = 1;
		public const int ColumnIndexReplaceWith = 2;
		public const int ColumnIndexRegexOptions = 3;

		public NetRegexAutoConfigDialog (
            IEncConverters aECs,
            string strDisplayName,
            string strFriendlyName,
            string strConverterIdentifier,
            ConvType eConversionType,
            string strLhsEncodingId,
            string strRhsEncodingId,
            int lProcessTypeFlags,
            bool bIsInRepository)
        {
#if VERBOSE_DEBUGGING
            Console.WriteLine("NetRegexAutoConfigDialog ctor BEGIN");
#endif
			InitializeComponent();
#if VERBOSE_DEBUGGING
            Console.WriteLine("Initialized NetRegexAutoConfigDialog component.");
#endif
			base.Initialize (
                aECs,
                NetRegexEncConverter.strHtmlFilename,
                strDisplayName,
                strFriendlyName,
                strConverterIdentifier,
                eConversionType,
                strLhsEncodingId,
                strRhsEncodingId,
                lProcessTypeFlags | (int)ProcessTypeFlags.ICURegularExpression,	// not really ICU, but do we really need another one?
                bIsInRepository);
#if VERBOSE_DEBUGGING
            Console.WriteLine("Initialized base.");
#endif

            LoadComboBoxFromSettings(comboBoxPreviousSearches, Properties.Settings.Default.RecentRegExpressions);
            comboBoxPreviousSearches.SelectedIndex = 0;

            // if we're editing a converter, then set the controls per the ConverterSpec and mark it as unmodified
            if (m_bEditMode)
            {
#if VERBOSE_DEBUGGING
                Console.WriteLine("Edit mode");
#endif
				System.Diagnostics.Debug.Assert(!String.IsNullOrEmpty(ConverterIdentifier));
                InitRegexControls(ConverterIdentifier);

                IsModified = false;
            }
			else
			{
				_isModifiedChangesDisabled = true;  // so it doesn't look like an edit has taken place
				var editRow = dataGridViewRegularExpressions.Rows[dataGridViewRegularExpressions.Rows.Count - 1];
				editRow.Selected = true;

				_isModifiedChangesDisabled = false;
			}

			buttonAddRegularExpression.Enabled = false;
			m_bInitialized = true;
#if VERBOSE_DEBUGGING
            Console.WriteLine("NetRegexAutoConfigDialog ctor END");
#endif
        }

        private void InitRegexControls(string strConverterIdentifier)
        {
            if (!ParseConverterIdentifier(strConverterIdentifier, out List<string> findWhats, out List<string> replaceWiths,
										  out List<RegexOptions> regexOptions, out List<bool> disabledFlags))
                return;

			dataGridViewRegularExpressions.Rows.Clear();
			for (int i = 0; i < findWhats.Count; i++)
			{
				var findWhat = findWhats[i];
				var replaceWith = replaceWiths[i];
				RegexOptions options = regexOptions[i];
				var disabled = disabledFlags[i];
				var row = dataGridViewRegularExpressions.Rows.Add(new object[] { !disabled, findWhat, replaceWith, options });
			}
		}

		private bool _isModifiedChangesDisabled = false;

		private void InitializeEditBoxes(string findWhat, string replaceWith, RegexOptions regexOptions)
		{
			_isModifiedChangesDisabled = true;	// so it doesn't look like an edit has taken place

			textBoxSearchFor.Text = findWhat;
			textBoxReplaceWith.Text = replaceWith;

			checkBoxIgnoreCase.Checked = (regexOptions & RegexOptions.IgnoreCase) == RegexOptions.IgnoreCase;
			checkBoxMultiline.Checked = (regexOptions & RegexOptions.Multiline) == RegexOptions.Multiline;
			checkBoxSingleLine.Checked = (regexOptions & RegexOptions.Singleline) == RegexOptions.Singleline;
			checkBoxExplicitCapture.Checked = (regexOptions & RegexOptions.ExplicitCapture) == RegexOptions.ExplicitCapture;
			checkBoxIgnorePatternWhitespace.Checked = (regexOptions & RegexOptions.IgnorePatternWhitespace) == RegexOptions.IgnorePatternWhitespace;
			checkBoxRightToLeft.Checked = (regexOptions & RegexOptions.RightToLeft) == RegexOptions.RightToLeft;
			checkBoxECMAScript.Checked = (regexOptions & RegexOptions.ECMAScript) == RegexOptions.ECMAScript;
			checkBoxCultureInvariant.Checked = (regexOptions & RegexOptions.CultureInvariant) == RegexOptions.CultureInvariant;

			_isModifiedChangesDisabled = false;
		}

		public NetRegexAutoConfigDialog(
            IEncConverters aECs,
            string strFriendlyName,
            string strConverterIdentifier,
            ConvType eConversionType,
            string strTestData)
        {
            InitializeComponent();
            base.Initialize (
                aECs,
                strFriendlyName,
                strConverterIdentifier,
                eConversionType,
                strTestData);
        }

		private void buttonAddRegularExpression_Click(object sender, EventArgs e)
		{
			var searchFor = textBoxSearchFor.Text;
			if (String.IsNullOrEmpty(searchFor))
			{
				MessageBox.Show(this, "Enter at least a Find What value first!", EncConverters.cstrCaption);
				return;
			}

			var current = GetRegexSpec(out string findWhat, out string replaceWhat, out RegexOptions options);

			_isModifiedChangesDisabled = true;          // so we don't trigger SelectionChanged at this point

			// add a new 'new' row if the selected one is the new row and select it to be the one we're editing
			DataGridViewRow editRow;
			if ((editRow = dataGridViewRegularExpressions.SelectedRows[0]).IsNewRow)
			{
				editRow.Cells.Cast<DataGridViewCell>().ToList().ForEach(c => c.Value = null);

				var index = dataGridViewRegularExpressions.Rows.Add(1);
				editRow = dataGridViewRegularExpressions.Rows[index];
				editRow.Selected = true;
				editRow.Cells[ColumnIndexEnabled].Value = true; // by default
			}

			editRow = dataGridViewRegularExpressions.SelectedRows[0];
			editRow.Cells[ColumnIndexFindWhat].Value = findWhat;
			editRow.Cells[ColumnIndexReplaceWith].Value = replaceWhat;
			editRow.Cells[ColumnIndexRegexOptions].Value = options;

			_isModifiedChangesDisabled = false;
			buttonAddRegularExpression.Enabled = false;
		}

		private void dataGridViewRegularExpressions_SelectionChanged(object sender, EventArgs e)
		{
			if (buttonAddRegularExpression.Enabled || (dataGridViewRegularExpressions.SelectedRows.Count == 0))
			{
				if (_isModifiedChangesDisabled || CheckSaveRegExChanges())
					return;
			}

			var selectedRow = dataGridViewRegularExpressions.SelectedRows[0];
			if (selectedRow != null)
			{
				var regexOptions = selectedRow.Cells[ColumnIndexRegexOptions].Value ?? RegexOptions.None;
				InitializeEditBoxes(selectedRow.Cells[ColumnIndexFindWhat].Value?.ToString(),
									selectedRow.Cells[ColumnIndexReplaceWith].Value?.ToString(),
									(RegexOptions)Enum.Parse(typeof(RegexOptions), regexOptions.ToString()));
			}
		}

		private void dataGridViewRegularExpressions_CurrentCellDirtyStateChanged(object sender, EventArgs e)
		{
			if ((dataGridViewRegularExpressions.CurrentCell.ColumnIndex == ColumnIndexEnabled) && !_isModifiedChangesDisabled)
				controlChangedModified(null, null);
		}

		private void dataGridViewRegularExpressions_CellEndEdit(object sender, DataGridViewCellEventArgs e)
		{
			switch (e.ColumnIndex)
			{
				case ColumnIndexFindWhat:
					var newFindWhat = dataGridViewRegularExpressions.Rows[e.RowIndex].Cells[ColumnIndexFindWhat].Value;
					textBoxSearchFor.Text = newFindWhat?.ToString();
					break;

				case ColumnIndexReplaceWith:
					var newReplaceWith = dataGridViewRegularExpressions.Rows[e.RowIndex].Cells[ColumnIndexReplaceWith].Value;
					textBoxReplaceWith.Text = newReplaceWith?.ToString();
					break;

				default:
					break;
			}
		}

		private bool CheckSaveRegExChanges()
		{
			var res = MessageBox.Show(this, "You have made changes to the currently selected Regular Expression. To Save the changes, click 'Yes'. Or Click 'No' here to lose the changes?",
				EncConverters.cstrCaption, MessageBoxButtons.YesNo);
			if (res == DialogResult.Yes)
			{
				buttonAddRegularExpression_Click(null, null);
				return true;
			}

			return false;
		}

		private string GetRegexSpec(out string findWhat, out string replaceWhat, out RegexOptions options)
        {
			options = RegexOptions.None;
			if (checkBoxIgnoreCase.Checked)
				options |= RegexOptions.IgnoreCase;
			if (checkBoxMultiline.Checked)
				options |= RegexOptions.Multiline;
			if (checkBoxSingleLine.Checked)
				options |= RegexOptions.Singleline;
			if (checkBoxExplicitCapture.Checked)
				options |= RegexOptions.ExplicitCapture;
			if (checkBoxIgnorePatternWhitespace.Checked)
				options |= RegexOptions.IgnorePatternWhitespace;
			if (checkBoxRightToLeft.Checked)
				options |= RegexOptions.RightToLeft;
			if (checkBoxECMAScript.Checked)
				options |= RegexOptions.ECMAScript;
			if (checkBoxCultureInvariant.Checked)
				options |= RegexOptions.CultureInvariant;

			var str = BuildRegexStorageFormat(findWhat = textBoxSearchFor.Text, replaceWhat = textBoxReplaceWith.Text, (ushort)options);
			return str;
		}

		private string BuildRegexStorageFormat(string findWhat, string replaceWith, ushort options)
		{
			var strOptions = ((options & DisabledBitFlag) == DisabledBitFlag)
								? $"0x{options.ToString("X")}"
								: options.ToString();
			return $"{{{findWhat}}}{RegexDelimiter}{{{replaceWith}}};{strOptions}";
		}

		// this method is called either when the user clicks the "Apply" or "OK" buttons *OR* if she
		//  tries to switch to the Test or Advanced tab. This is the dialog's one opportunity
		//  to make sure that the user has correctly configured a legitimate converter.
		protected override bool OnApply()
        {
#if VERBOSE_DEBUGGING
            Console.Error.WriteLine("OnApply() BEGIN");
#endif
			if (buttonAddRegularExpression.Enabled)
				CheckSaveRegExChanges();

			// Get the converter identifier from the Setup tab controls.
			var runningRegularExpressions = new List<string>();
			foreach (DataGridViewRow row in dataGridViewRegularExpressions.Rows.Cast<DataGridViewRow>().Where(r => !r.IsNewRow))
			{
				var regexOptions = row.Cells[ColumnIndexRegexOptions].Value ?? RegexOptions.None;
				var isDisabled = !(bool)(row.Cells[ColumnIndexEnabled].Value ?? false);
				var options = (ushort)(int)regexOptions;
				if (isDisabled)
					options |= DisabledBitFlag;
				var findWhat = row.Cells[ColumnIndexFindWhat].Value.ToString();
				var replaceWith = row.Cells[ColumnIndexReplaceWith].Value?.ToString() ?? String.Empty;
				var storageFormat = BuildRegexStorageFormat(findWhat, replaceWith, options);
				runningRegularExpressions.Add(storageFormat);
			}

			ConverterIdentifier = String.Join(SeparatorRegularExpressions, runningRegularExpressions);

			UpdateRecentRegExpressions(ConverterIdentifier);

#if VERBOSE_DEBUGGING
            Console.Error.WriteLine("OnApply() END");
#endif
			return base.OnApply();
        }

        private void UpdateRecentRegExpressions(string strConverterIdentifier)
        {
            if (!Properties.Settings.Default.RecentRegExpressions.Contains(strConverterIdentifier))
            {
				Properties.Settings.Default.RecentRegExpressions.Add(strConverterIdentifier);
				Properties.Settings.Default.Save();
                LoadComboBoxFromSettings(comboBoxPreviousSearches, Properties.Settings.Default.RecentRegExpressions);
            }

			// setting SelectedItem triggers comboBoxPreviousSearches_SelectedIndexChanged, which we don't want now
			_isModifiedChangesDisabled = true;
			comboBoxPreviousSearches.SelectedItem = strConverterIdentifier;
			_isModifiedChangesDisabled = false;
        }

        protected override string ProgID
        {
            get { return typeof(NetRegexEncConverter).FullName; }
        }

        protected override string ImplType
        {
            get { return EncConverters.strTypeSILNetRegex; }
        }

        protected override string DefaultFriendlyName
        {
            // as the default, make it the same as the reg expression
            get { return ConverterIdentifier; }
        }

        private void controlChangedModified(object sender, EventArgs e)
        {
            if (!m_bInitialized || _isModifiedChangesDisabled) 
                return;

			IsModified = true;

			if (!buttonAddRegularExpression.Enabled)
				buttonAddRegularExpression.Enabled = true;
        }

        private void buttonPopupHelper_Click(object sender, EventArgs e)
        {
            contextMenuRegexChars.Show(MousePosition);
        }

        private void OnRegexHelperMenuClick(object sender, EventArgs e)
        {
            var tsmi = sender as ToolStripMenuItem;
            Debug.Assert(tsmi != null, "tsmi != null");
            var text = tsmi.Text;
            var nLength = text.IndexOf("  ");
            var str = text.Substring(0, nLength);
            textBoxSearchFor.Text += str;
        }

        private void comboBoxPreviousSearches_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (!m_bInitialized || _isModifiedChangesDisabled)
                return;

            var strConverterSpec = comboBoxPreviousSearches.SelectedItem.ToString();
            InitRegexControls(strConverterSpec);
        }

        private void buttonDeleteSavedExpression_Click(object sender, EventArgs e)
        {
            DeleteSelectedItemFromComboBoxAndUpdateSettings(comboBoxPreviousSearches,
															Properties.Settings.Default.RecentRegExpressions);

			Properties.Settings.Default.Save();
        }

		private void linkLabelOpenQuickReference_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
		{
			System.Diagnostics.Process.Start(Properties.Resources.NetRegexQuickReferenceLink);
		}
	}
}

