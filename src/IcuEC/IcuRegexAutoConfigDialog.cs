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
using IcuEC.Properties;

// for IEncConverter

//uncomment the following line for verbose debugging output using Console.WriteLine
//#define VERBOSE_DEBUGGING

namespace SilEncConverters40
{
    public class IcuRegexAutoConfigDialog : SilEncConverters40.AutoConfigDialog
    {
        public IcuRegexAutoConfigDialog (
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
            Console.WriteLine("IcuRegexAutoConfigDialog ctor BEGIN");
#endif
			InitializeComponent();
#if VERBOSE_DEBUGGING
            Console.WriteLine("Initialized IcuRegexAutoConfigDialog component.");
#endif
			base.Initialize (
                aECs,
                IcuRegexEncConverter.strHtmlFilename,
                strDisplayName,
                strFriendlyName,
                strConverterIdentifier,
                eConversionType,
                strLhsEncodingId,
                strRhsEncodingId,
                lProcessTypeFlags | (int)ProcessTypeFlags.ICURegularExpression,
                bIsInRepository);
#if VERBOSE_DEBUGGING
            Console.WriteLine("Initialized base.");
#endif

            LoadComboBoxFromSettings(comboBoxPreviousSearches, Settings.Default.RecentRegExpressions);
            comboBoxPreviousSearches.SelectedIndex = 0;

            // if we're editing a CC table/spellfixer project, then set the Converter Spec and say it's unmodified
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
            Console.WriteLine("IcuRegexAutoConfigDialog ctor END");
#endif
        }

        private void InitRegexControls(string strConverterIdentifier)
        {
            string strFindWhat, strReplaceWith;
            bool bIgnoreCase;
            if (!DeconstructConverterSpec(strConverterIdentifier, out strFindWhat, out strReplaceWith, out bIgnoreCase))
                return;

            textBoxSearchFor.Text = strFindWhat;
            textBoxReplaceWith.Text = strReplaceWith;
            checkBoxIgnoreCase.Checked = bIgnoreCase;
        }

        public IcuRegexAutoConfigDialog(
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

        private System.Windows.Forms.Label              labelSearchFor;
        private System.Windows.Forms.TextBox            textBoxSearchFor;
        private ContextMenuStrip contextMenuRegexChars;
        private IContainer components;
        private ToolStripMenuItem toolStripAnyChar;
        private Button buttonPopupHelper;
        private ToolStripMenuItem toolStrip0orMore;
        private ToolStripMenuItem toolStrip0or1FewAsPossible;
        private ToolStripMenuItem toolStrip0orMoreFewAsPossible;
        private ToolStripMenuItem toolStrip1orMore;
        private ToolStripMenuItem toolStrip1orMoreFewAsPossible;
        private ToolStripMenuItem toolStripMatchNtimes;
        private ToolStripMenuItem toolStripMatchAtLeastNtimes;
        private ToolStripMenuItem toolStripMatchN2Mtimes;
        private ToolStripSeparator toolStripSeparator1;
        private ToolStripMenuItem toolStripMatchAtBeginningOfLine;
        private ToolStripMenuItem toolStripMatchEndOfLine;
        private ToolStripMenuItem toolStripMatchWordBoundary;
        private ToolStripMenuItem toolStripMatchNonWordBoundary;
        private ToolStripMenuItem toolStripMatchDecimalDigit;
        private ToolStripMenuItem toolStripMatchNonDecimalDigit;
        private ToolStripMenuItem toolStripMatchWhiteSpace;
        private ToolStripMenuItem toolStripMatchNonWhiteSpace;
        private ToolStripSeparator toolStripSeparator2;
        private ToolStripMenuItem toolStripMatchOneOf;
        private ToolStripMenuItem toolStripMatchOr;
        private ToolStripMenuItem toolStripMatchUchar;
        private Label labelReplaceWith;
        private TextBox textBoxReplaceWith;
        private CheckBox checkBoxIgnoreCase;
        private Label label1;
        private ComboBox comboBoxPreviousSearches;
        private Button buttonDeleteSavedExpression;
        private System.Windows.Forms.TableLayoutPanel   tableLayoutPanel1;

        // This code was NOT generated!
        // So feel free to modify it as needed.
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            this.tableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
            this.textBoxReplaceWith = new System.Windows.Forms.TextBox();
            this.labelSearchFor = new System.Windows.Forms.Label();
            this.textBoxSearchFor = new System.Windows.Forms.TextBox();
            this.buttonPopupHelper = new System.Windows.Forms.Button();
            this.labelReplaceWith = new System.Windows.Forms.Label();
            this.checkBoxIgnoreCase = new System.Windows.Forms.CheckBox();
            this.label1 = new System.Windows.Forms.Label();
            this.comboBoxPreviousSearches = new System.Windows.Forms.ComboBox();
            this.buttonDeleteSavedExpression = new System.Windows.Forms.Button();
            this.contextMenuRegexChars = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.toolStripAnyChar = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStrip0orMore = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStrip0or1FewAsPossible = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStrip0orMoreFewAsPossible = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStrip1orMore = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStrip1orMoreFewAsPossible = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMatchNtimes = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMatchAtLeastNtimes = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMatchN2Mtimes = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator1 = new System.Windows.Forms.ToolStripSeparator();
            this.toolStripMatchAtBeginningOfLine = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMatchEndOfLine = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMatchWordBoundary = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMatchNonWordBoundary = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMatchDecimalDigit = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMatchNonDecimalDigit = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMatchWhiteSpace = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMatchNonWhiteSpace = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator2 = new System.Windows.Forms.ToolStripSeparator();
            this.toolStripMatchOneOf = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMatchOr = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMatchUchar = new System.Windows.Forms.ToolStripMenuItem();
            this.tabControl.SuspendLayout();
            this.tabPageSetup.SuspendLayout();
            this.tableLayoutPanel1.SuspendLayout();
            this.contextMenuRegexChars.SuspendLayout();
            this.SuspendLayout();
            // 
            // tabPageSetup
            // 
            this.tabPageSetup.Controls.Add(this.tableLayoutPanel1);
            // 
            // buttonApply
            // 
            this.helpProvider.SetHelpString(this.buttonApply, "Click this button to apply the configured values for this converter");
            this.helpProvider.SetShowHelp(this.buttonApply, true);
            // 
            // buttonCancel
            // 
            this.helpProvider.SetHelpString(this.buttonCancel, "Click this button to cancel this dialog");
            this.helpProvider.SetShowHelp(this.buttonCancel, true);
            // 
            // buttonOK
            // 
            this.helpProvider.SetHelpString(this.buttonOK, "Click this button to accept the configured values for this converter");
            this.helpProvider.SetShowHelp(this.buttonOK, true);
            // 
            // buttonSaveInRepository
            // 
            this.helpProvider.SetHelpString(this.buttonSaveInRepository, "\r\nClick to add this converter to the system repository permanently.\r\n    ");
            this.helpProvider.SetShowHelp(this.buttonSaveInRepository, true);
            // 
            // tableLayoutPanel1
            // 
            this.tableLayoutPanel1.ColumnCount = 3;
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.tableLayoutPanel1.Controls.Add(this.textBoxReplaceWith, 1, 2);
            this.tableLayoutPanel1.Controls.Add(this.labelSearchFor, 0, 1);
            this.tableLayoutPanel1.Controls.Add(this.textBoxSearchFor, 1, 1);
            this.tableLayoutPanel1.Controls.Add(this.buttonPopupHelper, 2, 1);
            this.tableLayoutPanel1.Controls.Add(this.labelReplaceWith, 0, 2);
            this.tableLayoutPanel1.Controls.Add(this.checkBoxIgnoreCase, 1, 3);
            this.tableLayoutPanel1.Controls.Add(this.label1, 0, 4);
            this.tableLayoutPanel1.Controls.Add(this.comboBoxPreviousSearches, 1, 4);
            this.tableLayoutPanel1.Controls.Add(this.buttonDeleteSavedExpression, 2, 4);
            this.tableLayoutPanel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanel1.Location = new System.Drawing.Point(3, 3);
            this.tableLayoutPanel1.Name = "tableLayoutPanel1";
            this.tableLayoutPanel1.RowCount = 5;
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 50F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 50F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 50F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 100F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 50F));
            this.tableLayoutPanel1.Size = new System.Drawing.Size(596, 394);
            this.tableLayoutPanel1.TabIndex = 1;
            // 
            // textBoxReplaceWith
            // 
            this.textBoxReplaceWith.Dock = System.Windows.Forms.DockStyle.Fill;
            this.textBoxReplaceWith.Location = new System.Drawing.Point(106, 103);
            this.textBoxReplaceWith.Name = "textBoxReplaceWith";
            this.textBoxReplaceWith.Size = new System.Drawing.Size(392, 20);
            this.textBoxReplaceWith.TabIndex = 3;
            this.textBoxReplaceWith.TextChanged += new System.EventHandler(this.controlChangedModified);
            // 
            // labelSearchFor
            // 
            this.labelSearchFor.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.labelSearchFor.AutoSize = true;
            this.labelSearchFor.Location = new System.Drawing.Point(41, 50);
            this.labelSearchFor.Name = "labelSearchFor";
            this.labelSearchFor.Size = new System.Drawing.Size(59, 13);
            this.labelSearchFor.TabIndex = 0;
            this.labelSearchFor.Text = "&Search for:";
            // 
            // textBoxSearchFor
            // 
            this.textBoxSearchFor.Dock = System.Windows.Forms.DockStyle.Fill;
            this.textBoxSearchFor.Location = new System.Drawing.Point(106, 53);
            this.textBoxSearchFor.Name = "textBoxSearchFor";
            this.textBoxSearchFor.Size = new System.Drawing.Size(392, 20);
            this.textBoxSearchFor.TabIndex = 1;
            this.textBoxSearchFor.TextChanged += new System.EventHandler(this.controlChangedModified);
            // 
            // buttonPopupHelper
            // 
            this.buttonPopupHelper.Location = new System.Drawing.Point(504, 53);
            this.buttonPopupHelper.Name = "buttonPopupHelper";
            this.buttonPopupHelper.Size = new System.Drawing.Size(20, 22);
            this.buttonPopupHelper.TabIndex = 2;
            this.buttonPopupHelper.Text = ">";
            this.buttonPopupHelper.UseVisualStyleBackColor = true;
            this.buttonPopupHelper.Click += new System.EventHandler(this.buttonPopupHelper_Click);
            // 
            // labelReplaceWith
            // 
            this.labelReplaceWith.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.labelReplaceWith.AutoSize = true;
            this.labelReplaceWith.Location = new System.Drawing.Point(28, 100);
            this.labelReplaceWith.Name = "labelReplaceWith";
            this.labelReplaceWith.Size = new System.Drawing.Size(72, 13);
            this.labelReplaceWith.TabIndex = 0;
            this.labelReplaceWith.Text = "&Replace with:";
            // 
            // checkBoxIgnoreCase
            // 
            this.checkBoxIgnoreCase.AutoSize = true;
            this.checkBoxIgnoreCase.Location = new System.Drawing.Point(106, 153);
            this.checkBoxIgnoreCase.Name = "checkBoxIgnoreCase";
            this.checkBoxIgnoreCase.Size = new System.Drawing.Size(88, 17);
            this.checkBoxIgnoreCase.TabIndex = 4;
            this.checkBoxIgnoreCase.Text = "&Ignore case?";
            this.checkBoxIgnoreCase.UseVisualStyleBackColor = true;
            this.checkBoxIgnoreCase.CheckedChanged += new System.EventHandler(this.controlChangedModified);
            // 
            // label1
            // 
            this.label1.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(3, 250);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(97, 13);
            this.label1.TabIndex = 0;
            this.label1.Text = "&Previous searches:";
            // 
            // comboBoxPreviousSearches
            // 
            this.comboBoxPreviousSearches.Dock = System.Windows.Forms.DockStyle.Fill;
            this.comboBoxPreviousSearches.FormattingEnabled = true;
            this.comboBoxPreviousSearches.Location = new System.Drawing.Point(106, 253);
            this.comboBoxPreviousSearches.Name = "comboBoxPreviousSearches";
            this.comboBoxPreviousSearches.Size = new System.Drawing.Size(392, 21);
            this.comboBoxPreviousSearches.TabIndex = 5;
            this.comboBoxPreviousSearches.SelectedIndexChanged += new System.EventHandler(this.comboBoxPreviousSearches_SelectedIndexChanged);
            // 
            // buttonDeleteSavedExpression
            // 
            this.buttonDeleteSavedExpression.Location = new System.Drawing.Point(504, 253);
            this.buttonDeleteSavedExpression.Name = "buttonDeleteSavedExpression";
            this.buttonDeleteSavedExpression.Size = new System.Drawing.Size(89, 23);
            this.buttonDeleteSavedExpression.TabIndex = 6;
            this.buttonDeleteSavedExpression.Text = "Delete";
            this.buttonDeleteSavedExpression.UseVisualStyleBackColor = true;
            this.buttonDeleteSavedExpression.Click += new System.EventHandler(this.buttonDeleteSavedExpression_Click);
            // 
            // contextMenuRegexChars
            // 
            this.contextMenuRegexChars.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolStripAnyChar,
            this.toolStrip0orMore,
            this.toolStrip0or1FewAsPossible,
            this.toolStrip0orMoreFewAsPossible,
            this.toolStrip1orMore,
            this.toolStrip1orMoreFewAsPossible,
            this.toolStripMatchNtimes,
            this.toolStripMatchAtLeastNtimes,
            this.toolStripMatchN2Mtimes,
            this.toolStripSeparator1,
            this.toolStripMatchAtBeginningOfLine,
            this.toolStripMatchEndOfLine,
            this.toolStripMatchWordBoundary,
            this.toolStripMatchNonWordBoundary,
            this.toolStripMatchDecimalDigit,
            this.toolStripMatchNonDecimalDigit,
            this.toolStripMatchWhiteSpace,
            this.toolStripMatchNonWhiteSpace,
            this.toolStripSeparator2,
            this.toolStripMatchOneOf,
            this.toolStripMatchOr,
            this.toolStripMatchUchar});
            this.contextMenuRegexChars.Name = "contextMenuRegexChars";
            this.contextMenuRegexChars.Size = new System.Drawing.Size(599, 456);
            // 
            // toolStripAnyChar
            // 
            this.toolStripAnyChar.Name = "toolStripAnyChar";
            this.toolStripAnyChar.Size = new System.Drawing.Size(598, 22);
            this.toolStripAnyChar.Text = ".  Match any character";
            this.toolStripAnyChar.Click += new System.EventHandler(this.OnRegexHelperMenuClick);
            // 
            // toolStrip0orMore
            // 
            this.toolStrip0orMore.Name = "toolStrip0orMore";
            this.toolStrip0orMore.Size = new System.Drawing.Size(598, 22);
            this.toolStrip0orMore.Text = "*  Match 0 or more times, as many as possible";
            this.toolStrip0orMore.Click += new System.EventHandler(this.OnRegexHelperMenuClick);
            // 
            // toolStrip0or1FewAsPossible
            // 
            this.toolStrip0or1FewAsPossible.Name = "toolStrip0or1FewAsPossible";
            this.toolStrip0or1FewAsPossible.Size = new System.Drawing.Size(598, 22);
            this.toolStrip0or1FewAsPossible.Text = "?  Match 0 or 1 times, but prefer one time";
            this.toolStrip0or1FewAsPossible.Click += new System.EventHandler(this.OnRegexHelperMenuClick);
            // 
            // toolStrip0orMoreFewAsPossible
            // 
            this.toolStrip0orMoreFewAsPossible.Name = "toolStrip0orMoreFewAsPossible";
            this.toolStrip0orMoreFewAsPossible.Size = new System.Drawing.Size(598, 22);
            this.toolStrip0orMoreFewAsPossible.Text = "*?  Match 0 or more times, as few as possible";
            this.toolStrip0orMoreFewAsPossible.Click += new System.EventHandler(this.OnRegexHelperMenuClick);
            // 
            // toolStrip1orMore
            // 
            this.toolStrip1orMore.Name = "toolStrip1orMore";
            this.toolStrip1orMore.Size = new System.Drawing.Size(598, 22);
            this.toolStrip1orMore.Text = "+  Match 1 or more times, as many as possible";
            this.toolStrip1orMore.Click += new System.EventHandler(this.OnRegexHelperMenuClick);
            // 
            // toolStrip1orMoreFewAsPossible
            // 
            this.toolStrip1orMoreFewAsPossible.Name = "toolStrip1orMoreFewAsPossible";
            this.toolStrip1orMoreFewAsPossible.Size = new System.Drawing.Size(598, 22);
            this.toolStrip1orMoreFewAsPossible.Text = "+?  Match 1 or more times, as few as possible";
            this.toolStrip1orMoreFewAsPossible.Click += new System.EventHandler(this.OnRegexHelperMenuClick);
            // 
            // toolStripMatchNtimes
            // 
            this.toolStripMatchNtimes.Name = "toolStripMatchNtimes";
            this.toolStripMatchNtimes.Size = new System.Drawing.Size(598, 22);
            this.toolStripMatchNtimes.Text = "{n}  Match exactly n times";
            this.toolStripMatchNtimes.Click += new System.EventHandler(this.OnRegexHelperMenuClick);
            // 
            // toolStripMatchAtLeastNtimes
            // 
            this.toolStripMatchAtLeastNtimes.Name = "toolStripMatchAtLeastNtimes";
            this.toolStripMatchAtLeastNtimes.Size = new System.Drawing.Size(598, 22);
            this.toolStripMatchAtLeastNtimes.Text = "{n,}  Match at least n times, but as many as possible";
            this.toolStripMatchAtLeastNtimes.Click += new System.EventHandler(this.OnRegexHelperMenuClick);
            // 
            // toolStripMatchN2Mtimes
            // 
            this.toolStripMatchN2Mtimes.Name = "toolStripMatchN2Mtimes";
            this.toolStripMatchN2Mtimes.Size = new System.Drawing.Size(598, 22);
            this.toolStripMatchN2Mtimes.Text = "{n,m}  Match between n and m times, as many as possible";
            this.toolStripMatchN2Mtimes.Click += new System.EventHandler(this.OnRegexHelperMenuClick);
            // 
            // toolStripSeparator1
            // 
            this.toolStripSeparator1.Name = "toolStripSeparator1";
            this.toolStripSeparator1.Size = new System.Drawing.Size(595, 6);
            // 
            // toolStripMatchAtBeginningOfLine
            // 
            this.toolStripMatchAtBeginningOfLine.Name = "toolStripMatchAtBeginningOfLine";
            this.toolStripMatchAtBeginningOfLine.Size = new System.Drawing.Size(598, 22);
            this.toolStripMatchAtBeginningOfLine.Text = "^  Match at the beginning of a line";
            this.toolStripMatchAtBeginningOfLine.Click += new System.EventHandler(this.OnRegexHelperMenuClick);
            // 
            // toolStripMatchEndOfLine
            // 
            this.toolStripMatchEndOfLine.Name = "toolStripMatchEndOfLine";
            this.toolStripMatchEndOfLine.Size = new System.Drawing.Size(598, 22);
            this.toolStripMatchEndOfLine.Text = "$  Match at the end of a line";
            this.toolStripMatchEndOfLine.Click += new System.EventHandler(this.OnRegexHelperMenuClick);
            // 
            // toolStripMatchWordBoundary
            // 
            this.toolStripMatchWordBoundary.Name = "toolStripMatchWordBoundary";
            this.toolStripMatchWordBoundary.Size = new System.Drawing.Size(598, 22);
            this.toolStripMatchWordBoundary.Text = "\\b  Match if the current position is a word boundary";
            this.toolStripMatchWordBoundary.Click += new System.EventHandler(this.OnRegexHelperMenuClick);
            // 
            // toolStripMatchNonWordBoundary
            // 
            this.toolStripMatchNonWordBoundary.Name = "toolStripMatchNonWordBoundary";
            this.toolStripMatchNonWordBoundary.Size = new System.Drawing.Size(598, 22);
            this.toolStripMatchNonWordBoundary.Text = "\\B  Match if the current position is not a word boundary";
            this.toolStripMatchNonWordBoundary.Click += new System.EventHandler(this.OnRegexHelperMenuClick);
            // 
            // toolStripMatchDecimalDigit
            // 
            this.toolStripMatchDecimalDigit.Name = "toolStripMatchDecimalDigit";
            this.toolStripMatchDecimalDigit.Size = new System.Drawing.Size(598, 22);
            this.toolStripMatchDecimalDigit.Text = "\\d  Match any number or decimal digit";
            this.toolStripMatchDecimalDigit.Click += new System.EventHandler(this.OnRegexHelperMenuClick);
            // 
            // toolStripMatchNonDecimalDigit
            // 
            this.toolStripMatchNonDecimalDigit.Name = "toolStripMatchNonDecimalDigit";
            this.toolStripMatchNonDecimalDigit.Size = new System.Drawing.Size(598, 22);
            this.toolStripMatchNonDecimalDigit.Text = "\\D  Match any character that is not a decimal digit";
            this.toolStripMatchNonDecimalDigit.Click += new System.EventHandler(this.OnRegexHelperMenuClick);
            // 
            // toolStripMatchWhiteSpace
            // 
            this.toolStripMatchWhiteSpace.Name = "toolStripMatchWhiteSpace";
            this.toolStripMatchWhiteSpace.Size = new System.Drawing.Size(598, 22);
            this.toolStripMatchWhiteSpace.Text = "\\s  Match a white space character";
            this.toolStripMatchWhiteSpace.Click += new System.EventHandler(this.OnRegexHelperMenuClick);
            // 
            // toolStripMatchNonWhiteSpace
            // 
            this.toolStripMatchNonWhiteSpace.Name = "toolStripMatchNonWhiteSpace";
            this.toolStripMatchNonWhiteSpace.Size = new System.Drawing.Size(598, 22);
            this.toolStripMatchNonWhiteSpace.Text = "\\S  Match a non-white space character";
            this.toolStripMatchNonWhiteSpace.Click += new System.EventHandler(this.OnRegexHelperMenuClick);
            // 
            // toolStripSeparator2
            // 
            this.toolStripSeparator2.Name = "toolStripSeparator2";
            this.toolStripSeparator2.Size = new System.Drawing.Size(595, 6);
            // 
            // toolStripMatchOneOf
            // 
            this.toolStripMatchOneOf.Name = "toolStripMatchOneOf";
            this.toolStripMatchOneOf.Size = new System.Drawing.Size(598, 22);
            this.toolStripMatchOneOf.Text = "[chars]  Match any one character from the set";
            this.toolStripMatchOneOf.Click += new System.EventHandler(this.OnRegexHelperMenuClick);
            // 
            // toolStripMatchOr
            // 
            this.toolStripMatchOr.Name = "toolStripMatchOr";
            this.toolStripMatchOr.Size = new System.Drawing.Size(598, 22);
            this.toolStripMatchOr.Text = "|  \'A|B\' matches either A or B";
            this.toolStripMatchOr.Click += new System.EventHandler(this.OnRegexHelperMenuClick);
            // 
            // toolStripMatchUchar
            // 
            this.toolStripMatchUchar.Name = "toolStripMatchUchar";
            this.toolStripMatchUchar.Size = new System.Drawing.Size(598, 22);
            this.toolStripMatchUchar.Text = "\\uHHHH  Match the character with the hex value HHHH (e.g. \\u0020 matches the SPAC" +
    "E character)";
            this.toolStripMatchUchar.Click += new System.EventHandler(this.OnRegexHelperMenuClick);
            // 
            // IcuRegexAutoConfigDialog
            // 
            this.ClientSize = new System.Drawing.Size(634, 479);
            this.Name = "IcuRegexAutoConfigDialog";
            this.Controls.SetChildIndex(this.tabControl, 0);
            this.Controls.SetChildIndex(this.buttonApply, 0);
            this.Controls.SetChildIndex(this.buttonCancel, 0);
            this.Controls.SetChildIndex(this.buttonOK, 0);
            this.Controls.SetChildIndex(this.buttonSaveInRepository, 0);
            this.tabControl.ResumeLayout(false);
            this.tabPageSetup.ResumeLayout(false);
            this.tableLayoutPanel1.ResumeLayout(false);
            this.tableLayoutPanel1.PerformLayout();
            this.contextMenuRegexChars.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        private const string CstrRegexDelimiter = "->";
        private const string CstrRegexIgnoreCaseFlag = " /i";

        private string GetConverterSpec
        {
            get
            {
                var str = String.Format("{0}{1}{2}",
                                        textBoxSearchFor.Text,
                                        CstrRegexDelimiter,
                                        textBoxReplaceWith.Text);

                if (checkBoxIgnoreCase.Checked)
                    str += CstrRegexIgnoreCaseFlag;

                return str;
            }
        }

        private static bool DeconstructConverterSpec(string strConverterSpec,
            out string strFindWhat, out string strReplaceWith, out bool bIgnoreCase)
        {
            var nIndex = strConverterSpec.IndexOf(CstrRegexDelimiter);
            if (nIndex == -1)
            {
                strFindWhat = strReplaceWith = null;
                bIgnoreCase = false;
                return false;
            }

            strFindWhat = strConverterSpec.Substring(0, nIndex);
            nIndex += CstrRegexDelimiter.Length;

            var nLengthReplaceWith = strConverterSpec.Length - nIndex;
            if (strConverterSpec.Contains(CstrRegexIgnoreCaseFlag))
            {
                bIgnoreCase = true;
                nLengthReplaceWith -= CstrRegexIgnoreCaseFlag.Length;
            }
            else
                bIgnoreCase = false;

            strReplaceWith = strConverterSpec.Substring(nIndex, nLengthReplaceWith);
            return true;
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

                if (strSearchFor.Contains(CstrRegexDelimiter))
                {
                    MessageBox.Show(this, @"The 'Search For' string can't contain the delimiter, '->'. Use the sequence '\u002d\u003e' instead for the same result.", EncConverters.cstrCaption);
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
            if (!Settings.Default.RecentRegExpressions.Contains(strConverterIdentifier))
            {
                Settings.Default.RecentRegExpressions.Add(strConverterIdentifier);
                Settings.Default.Save();
                LoadComboBoxFromSettings(comboBoxPreviousSearches, Settings.Default.RecentRegExpressions);
            }
            comboBoxPreviousSearches.SelectedItem = strConverterIdentifier;
        }

        protected override string ProgID
        {
            get { return typeof(IcuRegexEncConverter).FullName; }
        }

        protected override string ImplType
        {
            get { return EncConverters.strTypeSILicuRegex; }
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
                                                            Settings.Default.RecentRegExpressions);

            Settings.Default.Save();
        }
    }
}

