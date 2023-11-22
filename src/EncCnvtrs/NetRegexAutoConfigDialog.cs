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

//uncomment the following line for verbose debugging output using Console.WriteLine
//#define VERBOSE_DEBUGGING

namespace SilEncConverters40
{
    public partial class NetRegexAutoConfigDialog : SilEncConverters40.AutoConfigDialog
    {
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

            m_bInitialized = true;
#if VERBOSE_DEBUGGING
            Console.WriteLine("NetRegexAutoConfigDialog ctor END");
#endif
        }

        private void InitRegexControls(string strConverterIdentifier)
        {
            if (!ParseConverterIdentifier(strConverterIdentifier, out string strFindWhat, out string strReplaceWith, out RegexOptions options))
                return;

            textBoxSearchFor.Text = strFindWhat;
            textBoxReplaceWith.Text = strReplaceWith;
            checkBoxIgnoreCase.Checked = (options & RegexOptions.IgnoreCase) == RegexOptions.IgnoreCase;
			checkBoxMultiline.Checked = (options & RegexOptions.Multiline) == RegexOptions.Multiline;
			checkBoxSingleLine.Checked = (options & RegexOptions.Singleline) == RegexOptions.Singleline;
			checkBoxExplicitCapture.Checked = (options & RegexOptions.ExplicitCapture) == RegexOptions.ExplicitCapture;
			checkBoxIgnorePatternWhitespace.Checked = (options & RegexOptions.IgnorePatternWhitespace) == RegexOptions.IgnorePatternWhitespace;
			checkBoxRightToLeft.Checked = (options & RegexOptions.RightToLeft) == RegexOptions.RightToLeft;
			checkBoxECMAScript.Checked = (options & RegexOptions.ECMAScript) == RegexOptions.ECMAScript;
			checkBoxCultureInvariant.Checked = (options & RegexOptions.CultureInvariant) == RegexOptions.CultureInvariant;
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

        private string GetConverterSpec
        {
            get
            {
				var options = RegexOptions.None;
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

				var str = $"{{{textBoxSearchFor.Text}}}{RegexDelimiter}{{{textBoxReplaceWith.Text}}};{(int)options}";
				return str;
            }
        }

        // this method is called either when the user clicks the "Apply" or "OK" buttons *OR* if she
        //  tries to switch to the Test or Advanced tab. This is the dialog's one opportunity
        //  to make sure that the user has correctly configured a legitimate converter.
        protected override bool OnApply()
        {
#if VERBOSE_DEBUGGING
            Console.Error.WriteLine("OnApply() BEGIN");
#endif

            // if we're actually on the setup tab, then do some further checking as well.
            if (tabControl.SelectedTab == tabPageSetup)
            {
                var strSearchFor = textBoxSearchFor.Text;
                // only do these message boxes if we're on the Setup tab itself, because if this OnApply
                //  is being called as a result of the user switching to the Test tab, that code will
                //  already put up an error message and we don't need two error messages.
                if (String.IsNullOrEmpty(strSearchFor))
                {
                    MessageBox.Show(this, "Enter a regular expression first!", EncConverters.cstrCaption);
                    return false;
                }
            }

            // Get the converter identifier from the Setup tab controls.
            ConverterIdentifier = GetConverterSpec;

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
            comboBoxPreviousSearches.SelectedItem = strConverterIdentifier;
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
            if (!m_bInitialized || IsModified) 
                return;

            IsModified = true;
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
            if (!m_bInitialized)
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

