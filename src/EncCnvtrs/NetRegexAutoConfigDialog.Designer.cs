using System.ComponentModel;
using System.Windows.Forms;

namespace SilEncConverters40
{
    partial class NetRegexAutoConfigDialog
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
		// This code was NOT generated!
		// So feel free to modify it as needed.
		private void InitializeComponent()
		{
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(NetRegexAutoConfigDialog));
            this.tableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
            this.textBoxReplaceWith = new System.Windows.Forms.TextBox();
            this.labelSearchFor = new System.Windows.Forms.Label();
            this.textBoxSearchFor = new System.Windows.Forms.TextBox();
            this.buttonPopupHelper = new System.Windows.Forms.Button();
            this.labelReplaceWith = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.comboBoxPreviousSearches = new System.Windows.Forms.ComboBox();
            this.buttonDeleteSavedExpression = new System.Windows.Forms.Button();
            this.linkLabelOpenQuickReference = new System.Windows.Forms.LinkLabel();
            this.groupBoxRegexOptions = new System.Windows.Forms.GroupBox();
            this.flowLayoutPanelOptions = new System.Windows.Forms.FlowLayoutPanel();
            this.checkBoxIgnoreCase = new System.Windows.Forms.CheckBox();
            this.checkBoxMultiline = new System.Windows.Forms.CheckBox();
            this.checkBoxSingleLine = new System.Windows.Forms.CheckBox();
            this.checkBoxExplicitCapture = new System.Windows.Forms.CheckBox();
            this.checkBoxIgnorePatternWhitespace = new System.Windows.Forms.CheckBox();
            this.checkBoxRightToLeft = new System.Windows.Forms.CheckBox();
            this.checkBoxECMAScript = new System.Windows.Forms.CheckBox();
            this.checkBoxCultureInvariant = new System.Windows.Forms.CheckBox();
            this.buttonAddRegularExpression = new System.Windows.Forms.Button();
            this.dataGridViewRegularExpressions = new System.Windows.Forms.DataGridView();
            this.ColumnEnabled = new System.Windows.Forms.DataGridViewCheckBoxColumn();
            this.ColumnFindWhat = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.ColumnReplaceWith = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.ColumnOptions = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.contextMenuRegexChars = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.toolStripAnyChar = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStrip0orMore = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStrip0or1FewAsPossible = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStrip0orMoreFewAsPossible = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStrip1orMore = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStrip1orMoreFewAsPossible = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMatch0or1AsFewAsPossible = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMatchNtimes = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMatchAtLeastNtimes = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMatchN2Mtimes = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMatchAtLeastNtimesAsFewAsPossible = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMatchN2MtimesAsFewAsPossible = new System.Windows.Forms.ToolStripMenuItem();
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
            this.toolStripMatchNoneOf = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMatchOneOfRange = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMatchOr = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMatchUchar = new System.Windows.Forms.ToolStripMenuItem();
            this.toolTip = new System.Windows.Forms.ToolTip(this.components);
            this.tabControl.SuspendLayout();
            this.tabPageSetup.SuspendLayout();
            this.tableLayoutPanel1.SuspendLayout();
            this.groupBoxRegexOptions.SuspendLayout();
            this.flowLayoutPanelOptions.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridViewRegularExpressions)).BeginInit();
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
            this.tableLayoutPanel1.Controls.Add(this.label1, 0, 4);
            this.tableLayoutPanel1.Controls.Add(this.comboBoxPreviousSearches, 1, 4);
            this.tableLayoutPanel1.Controls.Add(this.buttonDeleteSavedExpression, 2, 4);
            this.tableLayoutPanel1.Controls.Add(this.linkLabelOpenQuickReference, 1, 5);
            this.tableLayoutPanel1.Controls.Add(this.groupBoxRegexOptions, 1, 3);
            this.tableLayoutPanel1.Controls.Add(this.buttonAddRegularExpression, 2, 3);
            this.tableLayoutPanel1.Controls.Add(this.dataGridViewRegularExpressions, 1, 0);
            this.tableLayoutPanel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanel1.Location = new System.Drawing.Point(3, 3);
            this.tableLayoutPanel1.Name = "tableLayoutPanel1";
            this.tableLayoutPanel1.RowCount = 6;
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 40F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 40F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 20F));
            this.tableLayoutPanel1.Size = new System.Drawing.Size(596, 394);
            this.tableLayoutPanel1.TabIndex = 1;
            // 
            // textBoxReplaceWith
            // 
            this.textBoxReplaceWith.Dock = System.Windows.Forms.DockStyle.Fill;
            this.textBoxReplaceWith.Location = new System.Drawing.Point(118, 209);
            this.textBoxReplaceWith.Name = "textBoxReplaceWith";
            this.textBoxReplaceWith.Size = new System.Drawing.Size(380, 20);
            this.textBoxReplaceWith.TabIndex = 3;
            this.textBoxReplaceWith.TextChanged += new System.EventHandler(this.controlChangedModified);
            // 
            // labelSearchFor
            // 
            this.labelSearchFor.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.labelSearchFor.AutoSize = true;
            this.labelSearchFor.Location = new System.Drawing.Point(53, 166);
            this.labelSearchFor.Name = "labelSearchFor";
            this.labelSearchFor.Size = new System.Drawing.Size(59, 13);
            this.labelSearchFor.TabIndex = 0;
            this.labelSearchFor.Text = "&Search for:";
            // 
            // textBoxSearchFor
            // 
            this.textBoxSearchFor.Dock = System.Windows.Forms.DockStyle.Fill;
            this.textBoxSearchFor.Location = new System.Drawing.Point(118, 169);
            this.textBoxSearchFor.Name = "textBoxSearchFor";
            this.textBoxSearchFor.Size = new System.Drawing.Size(380, 20);
            this.textBoxSearchFor.TabIndex = 1;
            this.textBoxSearchFor.TextChanged += new System.EventHandler(this.controlChangedModified);
            // 
            // buttonPopupHelper
            // 
            this.buttonPopupHelper.Location = new System.Drawing.Point(504, 169);
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
            this.labelReplaceWith.Location = new System.Drawing.Point(40, 206);
            this.labelReplaceWith.Name = "labelReplaceWith";
            this.labelReplaceWith.Size = new System.Drawing.Size(72, 13);
            this.labelReplaceWith.TabIndex = 0;
            this.labelReplaceWith.Text = "&Replace with:";
            // 
            // label1
            // 
            this.label1.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(3, 352);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(109, 13);
            this.label1.TabIndex = 0;
            this.label1.Text = "&Previous expressions:";
            // 
            // comboBoxPreviousSearches
            // 
            this.comboBoxPreviousSearches.Dock = System.Windows.Forms.DockStyle.Fill;
            this.comboBoxPreviousSearches.FormattingEnabled = true;
            this.comboBoxPreviousSearches.Location = new System.Drawing.Point(118, 355);
            this.comboBoxPreviousSearches.Name = "comboBoxPreviousSearches";
            this.comboBoxPreviousSearches.Size = new System.Drawing.Size(380, 21);
            this.comboBoxPreviousSearches.TabIndex = 5;
            this.comboBoxPreviousSearches.SelectedIndexChanged += new System.EventHandler(this.comboBoxPreviousSearches_SelectedIndexChanged);
            // 
            // buttonDeleteSavedExpression
            // 
            this.buttonDeleteSavedExpression.Location = new System.Drawing.Point(504, 355);
            this.buttonDeleteSavedExpression.Name = "buttonDeleteSavedExpression";
            this.buttonDeleteSavedExpression.Size = new System.Drawing.Size(89, 23);
            this.buttonDeleteSavedExpression.TabIndex = 6;
            this.buttonDeleteSavedExpression.Text = "Delete";
            this.buttonDeleteSavedExpression.UseVisualStyleBackColor = true;
            this.buttonDeleteSavedExpression.Click += new System.EventHandler(this.buttonDeleteSavedExpression_Click);
            // 
            // linkLabelOpenQuickReference
            // 
            this.linkLabelOpenQuickReference.AutoSize = true;
            this.linkLabelOpenQuickReference.Location = new System.Drawing.Point(118, 381);
            this.linkLabelOpenQuickReference.Name = "linkLabelOpenQuickReference";
            this.linkLabelOpenQuickReference.Size = new System.Drawing.Size(205, 13);
            this.linkLabelOpenQuickReference.TabIndex = 8;
            this.linkLabelOpenQuickReference.TabStop = true;
            this.linkLabelOpenQuickReference.Text = ".Net Regular Expression Quick Reference";
            this.linkLabelOpenQuickReference.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.linkLabelOpenQuickReference_LinkClicked);
            // 
            // groupBoxRegexOptions
            // 
            this.groupBoxRegexOptions.Controls.Add(this.flowLayoutPanelOptions);
            this.groupBoxRegexOptions.Dock = System.Windows.Forms.DockStyle.Fill;
            this.groupBoxRegexOptions.Location = new System.Drawing.Point(118, 249);
            this.groupBoxRegexOptions.Name = "groupBoxRegexOptions";
            this.groupBoxRegexOptions.Size = new System.Drawing.Size(380, 100);
            this.groupBoxRegexOptions.TabIndex = 9;
            this.groupBoxRegexOptions.TabStop = false;
            this.groupBoxRegexOptions.Text = "RegexOptions";
            // 
            // flowLayoutPanelOptions
            // 
            this.flowLayoutPanelOptions.Controls.Add(this.checkBoxIgnoreCase);
            this.flowLayoutPanelOptions.Controls.Add(this.checkBoxMultiline);
            this.flowLayoutPanelOptions.Controls.Add(this.checkBoxSingleLine);
            this.flowLayoutPanelOptions.Controls.Add(this.checkBoxExplicitCapture);
            this.flowLayoutPanelOptions.Controls.Add(this.checkBoxIgnorePatternWhitespace);
            this.flowLayoutPanelOptions.Controls.Add(this.checkBoxRightToLeft);
            this.flowLayoutPanelOptions.Controls.Add(this.checkBoxECMAScript);
            this.flowLayoutPanelOptions.Controls.Add(this.checkBoxCultureInvariant);
            this.flowLayoutPanelOptions.Dock = System.Windows.Forms.DockStyle.Fill;
            this.flowLayoutPanelOptions.Location = new System.Drawing.Point(3, 16);
            this.flowLayoutPanelOptions.Name = "flowLayoutPanelOptions";
            this.flowLayoutPanelOptions.Padding = new System.Windows.Forms.Padding(3, 3, 6, 3);
            this.flowLayoutPanelOptions.Size = new System.Drawing.Size(374, 81);
            this.flowLayoutPanelOptions.TabIndex = 7;
            // 
            // checkBoxIgnoreCase
            // 
            this.checkBoxIgnoreCase.AutoSize = true;
            this.checkBoxIgnoreCase.Location = new System.Drawing.Point(6, 6);
            this.checkBoxIgnoreCase.Name = "checkBoxIgnoreCase";
            this.checkBoxIgnoreCase.Size = new System.Drawing.Size(82, 17);
            this.checkBoxIgnoreCase.TabIndex = 4;
            this.checkBoxIgnoreCase.Text = "&Ignore case";
            this.toolTip.SetToolTip(this.checkBoxIgnoreCase, "Check to use case-insensitive matching.");
            this.checkBoxIgnoreCase.UseVisualStyleBackColor = true;
            this.checkBoxIgnoreCase.CheckedChanged += new System.EventHandler(this.controlChangedModified);
            // 
            // checkBoxMultiline
            // 
            this.checkBoxMultiline.AutoSize = true;
            this.checkBoxMultiline.Location = new System.Drawing.Point(94, 6);
            this.checkBoxMultiline.Name = "checkBoxMultiline";
            this.checkBoxMultiline.Size = new System.Drawing.Size(64, 17);
            this.checkBoxMultiline.TabIndex = 4;
            this.checkBoxMultiline.Text = "&Multiline";
            this.toolTip.SetToolTip(this.checkBoxMultiline, "Use multiline mode, where ^ and $ indicate the beginning and end of each line (in" +
        "stead of the beginning and end of the input string).");
            this.checkBoxMultiline.UseVisualStyleBackColor = true;
            this.checkBoxMultiline.CheckedChanged += new System.EventHandler(this.controlChangedModified);
            // 
            // checkBoxSingleLine
            // 
            this.checkBoxSingleLine.AutoSize = true;
            this.checkBoxSingleLine.Location = new System.Drawing.Point(164, 6);
            this.checkBoxSingleLine.Name = "checkBoxSingleLine";
            this.checkBoxSingleLine.Size = new System.Drawing.Size(71, 17);
            this.checkBoxSingleLine.TabIndex = 4;
            this.checkBoxSingleLine.Text = "&Singleline";
            this.toolTip.SetToolTip(this.checkBoxSingleLine, "Use single-line mode, where the period (.) matches every character (instead of ev" +
        "ery character except \\n).");
            this.checkBoxSingleLine.UseVisualStyleBackColor = true;
            this.checkBoxSingleLine.CheckedChanged += new System.EventHandler(this.controlChangedModified);
            // 
            // checkBoxExplicitCapture
            // 
            this.checkBoxExplicitCapture.AutoSize = true;
            this.checkBoxExplicitCapture.Location = new System.Drawing.Point(241, 6);
            this.checkBoxExplicitCapture.Name = "checkBoxExplicitCapture";
            this.checkBoxExplicitCapture.Size = new System.Drawing.Size(99, 17);
            this.checkBoxExplicitCapture.TabIndex = 4;
            this.checkBoxExplicitCapture.Text = "&Explicit Capture";
            this.toolTip.SetToolTip(this.checkBoxExplicitCapture, "Do not capture unnamed groups. The only valid captures are explicitly named or nu" +
        "mbered groups of the form (?<name> subexpression).");
            this.checkBoxExplicitCapture.UseVisualStyleBackColor = true;
            this.checkBoxExplicitCapture.CheckedChanged += new System.EventHandler(this.controlChangedModified);
            // 
            // checkBoxIgnorePatternWhitespace
            // 
            this.checkBoxIgnorePatternWhitespace.AutoSize = true;
            this.checkBoxIgnorePatternWhitespace.Location = new System.Drawing.Point(6, 29);
            this.checkBoxIgnorePatternWhitespace.Name = "checkBoxIgnorePatternWhitespace";
            this.checkBoxIgnorePatternWhitespace.Size = new System.Drawing.Size(153, 17);
            this.checkBoxIgnorePatternWhitespace.TabIndex = 4;
            this.checkBoxIgnorePatternWhitespace.Text = "&Ignore Pattern Whitespace";
            this.toolTip.SetToolTip(this.checkBoxIgnorePatternWhitespace, "Exclude unescaped white space from the pattern, and enable comments after a numbe" +
        "r sign (#).");
            this.checkBoxIgnorePatternWhitespace.UseVisualStyleBackColor = true;
            this.checkBoxIgnorePatternWhitespace.CheckedChanged += new System.EventHandler(this.controlChangedModified);
            // 
            // checkBoxRightToLeft
            // 
            this.checkBoxRightToLeft.AutoSize = true;
            this.checkBoxRightToLeft.Location = new System.Drawing.Point(165, 29);
            this.checkBoxRightToLeft.Name = "checkBoxRightToLeft";
            this.checkBoxRightToLeft.Size = new System.Drawing.Size(88, 17);
            this.checkBoxRightToLeft.TabIndex = 4;
            this.checkBoxRightToLeft.Text = "&Right To Left";
            this.toolTip.SetToolTip(this.checkBoxRightToLeft, "Change the search direction. Search moves from right to left instead of from left" +
        " to right.");
            this.checkBoxRightToLeft.UseVisualStyleBackColor = true;
            this.checkBoxRightToLeft.CheckedChanged += new System.EventHandler(this.controlChangedModified);
            // 
            // checkBoxECMAScript
            // 
            this.checkBoxECMAScript.AutoSize = true;
            this.checkBoxECMAScript.Location = new System.Drawing.Point(259, 29);
            this.checkBoxECMAScript.Name = "checkBoxECMAScript";
            this.checkBoxECMAScript.Size = new System.Drawing.Size(86, 17);
            this.checkBoxECMAScript.TabIndex = 4;
            this.checkBoxECMAScript.Text = "&ECMA Script";
            this.toolTip.SetToolTip(this.checkBoxECMAScript, "Enable ECMAScript-compliant behavior for the expression.");
            this.checkBoxECMAScript.UseVisualStyleBackColor = true;
            this.checkBoxECMAScript.CheckedChanged += new System.EventHandler(this.controlChangedModified);
            // 
            // checkBoxCultureInvariant
            // 
            this.checkBoxCultureInvariant.AutoSize = true;
            this.checkBoxCultureInvariant.Location = new System.Drawing.Point(6, 52);
            this.checkBoxCultureInvariant.Name = "checkBoxCultureInvariant";
            this.checkBoxCultureInvariant.Size = new System.Drawing.Size(103, 17);
            this.checkBoxCultureInvariant.TabIndex = 4;
            this.checkBoxCultureInvariant.Text = "&Culture Invariant";
            this.toolTip.SetToolTip(this.checkBoxCultureInvariant, "Ignore cultural differences in language.");
            this.checkBoxCultureInvariant.UseVisualStyleBackColor = true;
            this.checkBoxCultureInvariant.CheckedChanged += new System.EventHandler(this.controlChangedModified);
            // 
            // buttonAddRegularExpression
            // 
            this.buttonAddRegularExpression.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right)));
            this.buttonAddRegularExpression.Location = new System.Drawing.Point(504, 287);
            this.buttonAddRegularExpression.Name = "buttonAddRegularExpression";
            this.buttonAddRegularExpression.Size = new System.Drawing.Size(89, 23);
            this.buttonAddRegularExpression.TabIndex = 11;
            this.buttonAddRegularExpression.Text = "S&ave";
            this.toolTip.SetToolTip(this.buttonAddRegularExpression, "Choose your expression for \'Search for\' and \'Replace With\', set any options you w" +
        "ant and click the \'Add\' button to add the operation to the queue of possible reg" +
        "ular expression operations");
            this.buttonAddRegularExpression.UseVisualStyleBackColor = true;
            this.buttonAddRegularExpression.Click += new System.EventHandler(this.buttonAddRegularExpression_Click);
            // 
            // dataGridViewRegularExpressions
            // 
            this.dataGridViewRegularExpressions.AllowDrop = true;
            this.dataGridViewRegularExpressions.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.DisplayedCells;
            this.dataGridViewRegularExpressions.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dataGridViewRegularExpressions.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.ColumnEnabled,
            this.ColumnFindWhat,
            this.ColumnReplaceWith,
            this.ColumnOptions});
            this.dataGridViewRegularExpressions.Dock = System.Windows.Forms.DockStyle.Fill;
            this.dataGridViewRegularExpressions.EditMode = System.Windows.Forms.DataGridViewEditMode.EditOnF2;
            this.dataGridViewRegularExpressions.Location = new System.Drawing.Point(118, 3);
            this.dataGridViewRegularExpressions.MultiSelect = false;
            this.dataGridViewRegularExpressions.Name = "dataGridViewRegularExpressions";
            this.dataGridViewRegularExpressions.RowHeadersVisible = false;
            this.dataGridViewRegularExpressions.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.dataGridViewRegularExpressions.Size = new System.Drawing.Size(380, 160);
            this.dataGridViewRegularExpressions.TabIndex = 12;
            this.toolTip.SetToolTip(this.dataGridViewRegularExpressions, resources.GetString("dataGridViewRegularExpressions.ToolTip"));
            this.dataGridViewRegularExpressions.CellContentClick += new System.Windows.Forms.DataGridViewCellEventHandler(this.dataGridViewRegularExpressions_CellContentClick);
            this.dataGridViewRegularExpressions.CellEndEdit += new System.Windows.Forms.DataGridViewCellEventHandler(this.dataGridViewRegularExpressions_CellEndEdit);
            this.dataGridViewRegularExpressions.CurrentCellDirtyStateChanged += new System.EventHandler(this.dataGridViewRegularExpressions_CurrentCellDirtyStateChanged);
            this.dataGridViewRegularExpressions.SelectionChanged += new System.EventHandler(this.dataGridViewRegularExpressions_SelectionChanged);
            this.dataGridViewRegularExpressions.DragDrop += new System.Windows.Forms.DragEventHandler(this.dataGridViewRegularExpressions_DragDrop);
            this.dataGridViewRegularExpressions.DragOver += new System.Windows.Forms.DragEventHandler(this.dataGridViewRegularExpressions_DragOver);
            this.dataGridViewRegularExpressions.MouseDown += new System.Windows.Forms.MouseEventHandler(this.dataGridViewRegularExpressions_MouseDown);
            // 
            // ColumnEnabled
            // 
            this.ColumnEnabled.HeaderText = "Enabled";
            this.ColumnEnabled.Name = "ColumnEnabled";
            this.ColumnEnabled.Width = 52;
            // 
            // ColumnFindWhat
            // 
            this.ColumnFindWhat.HeaderText = "Find What";
            this.ColumnFindWhat.Name = "ColumnFindWhat";
            this.ColumnFindWhat.Width = 81;
            // 
            // ColumnReplaceWith
            // 
            this.ColumnReplaceWith.HeaderText = "Replace With";
            this.ColumnReplaceWith.Name = "ColumnReplaceWith";
            this.ColumnReplaceWith.Width = 97;
            // 
            // ColumnOptions
            // 
            this.ColumnOptions.HeaderText = "Options";
            this.ColumnOptions.Name = "ColumnOptions";
            this.ColumnOptions.ReadOnly = true;
            this.ColumnOptions.Width = 68;
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
            this.toolStripMatch0or1AsFewAsPossible,
            this.toolStripMatchNtimes,
            this.toolStripMatchAtLeastNtimes,
            this.toolStripMatchN2Mtimes,
            this.toolStripMatchAtLeastNtimesAsFewAsPossible,
            this.toolStripMatchN2MtimesAsFewAsPossible,
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
            this.toolStripMatchNoneOf,
            this.toolStripMatchOneOfRange,
            this.toolStripMatchOr,
            this.toolStripMatchUchar});
            this.contextMenuRegexChars.Name = "contextMenuRegexChars";
            this.contextMenuRegexChars.Size = new System.Drawing.Size(1068, 566);
            // 
            // toolStripAnyChar
            // 
            this.toolStripAnyChar.Name = "toolStripAnyChar";
            this.toolStripAnyChar.Size = new System.Drawing.Size(1067, 22);
            this.toolStripAnyChar.Text = ".  Matches any single character except (To match a literal period character (. or" +
    " \\u002E), you must precede it with the escape character (\\.)).";
            this.toolStripAnyChar.Click += new System.EventHandler(this.OnRegexHelperMenuClick);
            // 
            // toolStrip0orMore
            // 
            this.toolStrip0orMore.Name = "toolStrip0orMore";
            this.toolStrip0orMore.Size = new System.Drawing.Size(1067, 22);
            this.toolStrip0orMore.Text = "*  Matches the previous element zero or more times.";
            this.toolStrip0orMore.Click += new System.EventHandler(this.OnRegexHelperMenuClick);
            // 
            // toolStrip0or1FewAsPossible
            // 
            this.toolStrip0or1FewAsPossible.Name = "toolStrip0or1FewAsPossible";
            this.toolStrip0or1FewAsPossible.Size = new System.Drawing.Size(1067, 22);
            this.toolStrip0or1FewAsPossible.Text = "?  Matches the previous element zero or one time.";
            this.toolStrip0or1FewAsPossible.Click += new System.EventHandler(this.OnRegexHelperMenuClick);
            // 
            // toolStrip0orMoreFewAsPossible
            // 
            this.toolStrip0orMoreFewAsPossible.Name = "toolStrip0orMoreFewAsPossible";
            this.toolStrip0orMoreFewAsPossible.Size = new System.Drawing.Size(1067, 22);
            this.toolStrip0orMoreFewAsPossible.Text = "*?  Matches the previous element zero or more times, but as few times as possible" +
    ".";
            this.toolStrip0orMoreFewAsPossible.Click += new System.EventHandler(this.OnRegexHelperMenuClick);
            // 
            // toolStrip1orMore
            // 
            this.toolStrip1orMore.Name = "toolStrip1orMore";
            this.toolStrip1orMore.Size = new System.Drawing.Size(1067, 22);
            this.toolStrip1orMore.Text = "+  Matches the previous element one or more times.";
            this.toolStrip1orMore.Click += new System.EventHandler(this.OnRegexHelperMenuClick);
            // 
            // toolStrip1orMoreFewAsPossible
            // 
            this.toolStrip1orMoreFewAsPossible.Name = "toolStrip1orMoreFewAsPossible";
            this.toolStrip1orMoreFewAsPossible.Size = new System.Drawing.Size(1067, 22);
            this.toolStrip1orMoreFewAsPossible.Text = "+?  Matches the previous element one or more times, but as few times as possible." +
    "";
            this.toolStrip1orMoreFewAsPossible.Click += new System.EventHandler(this.OnRegexHelperMenuClick);
            // 
            // toolStripMatch0or1AsFewAsPossible
            // 
            this.toolStripMatch0or1AsFewAsPossible.Name = "toolStripMatch0or1AsFewAsPossible";
            this.toolStripMatch0or1AsFewAsPossible.Size = new System.Drawing.Size(1067, 22);
            this.toolStripMatch0or1AsFewAsPossible.Text = "??  Matches the previous element zero or one time, but as few times as possible.";
            // 
            // toolStripMatchNtimes
            // 
            this.toolStripMatchNtimes.Name = "toolStripMatchNtimes";
            this.toolStripMatchNtimes.Size = new System.Drawing.Size(1067, 22);
            this.toolStripMatchNtimes.Text = "{n}  Matches the previous element exactly n times.";
            this.toolStripMatchNtimes.Click += new System.EventHandler(this.OnRegexHelperMenuClick);
            // 
            // toolStripMatchAtLeastNtimes
            // 
            this.toolStripMatchAtLeastNtimes.Name = "toolStripMatchAtLeastNtimes";
            this.toolStripMatchAtLeastNtimes.Size = new System.Drawing.Size(1067, 22);
            this.toolStripMatchAtLeastNtimes.Text = "{n,}  Matches the previous element at least n times.";
            this.toolStripMatchAtLeastNtimes.Click += new System.EventHandler(this.OnRegexHelperMenuClick);
            // 
            // toolStripMatchN2Mtimes
            // 
            this.toolStripMatchN2Mtimes.Name = "toolStripMatchN2Mtimes";
            this.toolStripMatchN2Mtimes.Size = new System.Drawing.Size(1067, 22);
            this.toolStripMatchN2Mtimes.Text = "{n,m}  Matches the previous element at least n times, but no more than m times.";
            this.toolStripMatchN2Mtimes.Click += new System.EventHandler(this.OnRegexHelperMenuClick);
            // 
            // toolStripMatchAtLeastNtimesAsFewAsPossible
            // 
            this.toolStripMatchAtLeastNtimesAsFewAsPossible.Name = "toolStripMatchAtLeastNtimesAsFewAsPossible";
            this.toolStripMatchAtLeastNtimesAsFewAsPossible.Size = new System.Drawing.Size(1067, 22);
            this.toolStripMatchAtLeastNtimesAsFewAsPossible.Text = "{n,}?  Matches the previous element at least n times, but as few times as possibl" +
    "e.";
            this.toolStripMatchAtLeastNtimesAsFewAsPossible.Click += new System.EventHandler(this.OnRegexHelperMenuClick);
            // 
            // toolStripMatchN2MtimesAsFewAsPossible
            // 
            this.toolStripMatchN2MtimesAsFewAsPossible.Name = "toolStripMatchN2MtimesAsFewAsPossible";
            this.toolStripMatchN2MtimesAsFewAsPossible.Size = new System.Drawing.Size(1067, 22);
            this.toolStripMatchN2MtimesAsFewAsPossible.Text = "{n,m}?  Matches the previous element between n and m times, but as few times as p" +
    "ossible.";
            this.toolStripMatchN2MtimesAsFewAsPossible.Click += new System.EventHandler(this.OnRegexHelperMenuClick);
            // 
            // toolStripSeparator1
            // 
            this.toolStripSeparator1.Name = "toolStripSeparator1";
            this.toolStripSeparator1.Size = new System.Drawing.Size(1064, 6);
            // 
            // toolStripMatchAtBeginningOfLine
            // 
            this.toolStripMatchAtBeginningOfLine.Name = "toolStripMatchAtBeginningOfLine";
            this.toolStripMatchAtBeginningOfLine.Size = new System.Drawing.Size(1067, 22);
            this.toolStripMatchAtBeginningOfLine.Text = "^  By default, the match must start at the beginning of the string; in multiline " +
    "mode, it must start at the beginning of the line.";
            this.toolStripMatchAtBeginningOfLine.Click += new System.EventHandler(this.OnRegexHelperMenuClick);
            // 
            // toolStripMatchEndOfLine
            // 
            this.toolStripMatchEndOfLine.Name = "toolStripMatchEndOfLine";
            this.toolStripMatchEndOfLine.Size = new System.Drawing.Size(1067, 22);
            this.toolStripMatchEndOfLine.Text = "$  By default, the match must occur at the end of the string or before \\n at the " +
    "end of the string; in multiline mode, it must occur before the end of the line o" +
    "r before \\n at the end of the line.";
            this.toolStripMatchEndOfLine.Click += new System.EventHandler(this.OnRegexHelperMenuClick);
            // 
            // toolStripMatchWordBoundary
            // 
            this.toolStripMatchWordBoundary.Name = "toolStripMatchWordBoundary";
            this.toolStripMatchWordBoundary.Size = new System.Drawing.Size(1067, 22);
            this.toolStripMatchWordBoundary.Text = "\\b  The match must occur on a boundary between a \\w (alphanumeric) and a \\W (nona" +
    "lphanumeric) character.";
            this.toolStripMatchWordBoundary.Click += new System.EventHandler(this.OnRegexHelperMenuClick);
            // 
            // toolStripMatchNonWordBoundary
            // 
            this.toolStripMatchNonWordBoundary.Name = "toolStripMatchNonWordBoundary";
            this.toolStripMatchNonWordBoundary.Size = new System.Drawing.Size(1067, 22);
            this.toolStripMatchNonWordBoundary.Text = "\\B  The match must not occur on a \\b boundary.";
            this.toolStripMatchNonWordBoundary.Click += new System.EventHandler(this.OnRegexHelperMenuClick);
            // 
            // toolStripMatchDecimalDigit
            // 
            this.toolStripMatchDecimalDigit.Name = "toolStripMatchDecimalDigit";
            this.toolStripMatchDecimalDigit.Size = new System.Drawing.Size(1067, 22);
            this.toolStripMatchDecimalDigit.Text = "\\d  Matches any decimal digit.";
            this.toolStripMatchDecimalDigit.Click += new System.EventHandler(this.OnRegexHelperMenuClick);
            // 
            // toolStripMatchNonDecimalDigit
            // 
            this.toolStripMatchNonDecimalDigit.Name = "toolStripMatchNonDecimalDigit";
            this.toolStripMatchNonDecimalDigit.Size = new System.Drawing.Size(1067, 22);
            this.toolStripMatchNonDecimalDigit.Text = "\\D  Matches any character other than a decimal digit.";
            this.toolStripMatchNonDecimalDigit.Click += new System.EventHandler(this.OnRegexHelperMenuClick);
            // 
            // toolStripMatchWhiteSpace
            // 
            this.toolStripMatchWhiteSpace.Name = "toolStripMatchWhiteSpace";
            this.toolStripMatchWhiteSpace.Size = new System.Drawing.Size(1067, 22);
            this.toolStripMatchWhiteSpace.Text = "\\s  Matches any white-space character.";
            this.toolStripMatchWhiteSpace.Click += new System.EventHandler(this.OnRegexHelperMenuClick);
            // 
            // toolStripMatchNonWhiteSpace
            // 
            this.toolStripMatchNonWhiteSpace.Name = "toolStripMatchNonWhiteSpace";
            this.toolStripMatchNonWhiteSpace.Size = new System.Drawing.Size(1067, 22);
            this.toolStripMatchNonWhiteSpace.Text = "\\S  Matches any non-white-space character.";
            this.toolStripMatchNonWhiteSpace.Click += new System.EventHandler(this.OnRegexHelperMenuClick);
            // 
            // toolStripSeparator2
            // 
            this.toolStripSeparator2.Name = "toolStripSeparator2";
            this.toolStripSeparator2.Size = new System.Drawing.Size(1064, 6);
            // 
            // toolStripMatchOneOf
            // 
            this.toolStripMatchOneOf.Name = "toolStripMatchOneOf";
            this.toolStripMatchOneOf.Size = new System.Drawing.Size(1067, 22);
            this.toolStripMatchOneOf.Text = "[chars]  Matches any single character in character_group. By default, the match i" +
    "s case-sensitive.";
            this.toolStripMatchOneOf.Click += new System.EventHandler(this.OnRegexHelperMenuClick);
            // 
            // toolStripMatchNoneOf
            // 
            this.toolStripMatchNoneOf.Name = "toolStripMatchNoneOf";
            this.toolStripMatchNoneOf.Size = new System.Drawing.Size(1067, 22);
            this.toolStripMatchNoneOf.Text = "[^chars]  Negation: Matches any single character that is not in character_group. " +
    "By default, characters in character_group are case-sensitive.";
            this.toolStripMatchNoneOf.Click += new System.EventHandler(this.OnRegexHelperMenuClick);
            // 
            // toolStripMatchOneOfRange
            // 
            this.toolStripMatchOneOfRange.Name = "toolStripMatchOneOfRange";
            this.toolStripMatchOneOfRange.Size = new System.Drawing.Size(1067, 22);
            this.toolStripMatchOneOfRange.Text = "[first-last]  Character range: Matches any single character in the range from fir" +
    "st to last. (e.g. [A-Z] is any upper case Latin alphabetic character)";
            this.toolStripMatchOneOfRange.Click += new System.EventHandler(this.OnRegexHelperMenuClick);
            // 
            // toolStripMatchOr
            // 
            this.toolStripMatchOr.Name = "toolStripMatchOr";
            this.toolStripMatchOr.Size = new System.Drawing.Size(1067, 22);
            this.toolStripMatchOr.Text = "|  Matches any one element separated by the vertical bar (|) character. (e.g. \'A|" +
    "B\' matches either A or B)";
            this.toolStripMatchOr.Click += new System.EventHandler(this.OnRegexHelperMenuClick);
            // 
            // toolStripMatchUchar
            // 
            this.toolStripMatchUchar.Name = "toolStripMatchUchar";
            this.toolStripMatchUchar.Size = new System.Drawing.Size(1067, 22);
            this.toolStripMatchUchar.Text = "\\uHHHH  Match the character with the hex value HHHH (e.g. \\u0020 matches the SPAC" +
    "E character)";
            this.toolStripMatchUchar.Click += new System.EventHandler(this.OnRegexHelperMenuClick);
            // 
            // NetRegexAutoConfigDialog
            // 
            this.ClientSize = new System.Drawing.Size(634, 479);
            this.Name = "NetRegexAutoConfigDialog";
            this.Controls.SetChildIndex(this.tabControl, 0);
            this.Controls.SetChildIndex(this.buttonApply, 0);
            this.Controls.SetChildIndex(this.buttonCancel, 0);
            this.Controls.SetChildIndex(this.buttonOK, 0);
            this.Controls.SetChildIndex(this.buttonSaveInRepository, 0);
            this.tabControl.ResumeLayout(false);
            this.tabPageSetup.ResumeLayout(false);
            this.tableLayoutPanel1.ResumeLayout(false);
            this.tableLayoutPanel1.PerformLayout();
            this.groupBoxRegexOptions.ResumeLayout(false);
            this.flowLayoutPanelOptions.ResumeLayout(false);
            this.flowLayoutPanelOptions.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridViewRegularExpressions)).EndInit();
            this.contextMenuRegexChars.ResumeLayout(false);
            this.ResumeLayout(false);

		}

		private System.Windows.Forms.Label labelSearchFor;
		private System.Windows.Forms.TextBox textBoxSearchFor;
		private ContextMenuStrip contextMenuRegexChars;
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
		private ToolStripMenuItem toolStripMatchNoneOf;
		private ToolStripMenuItem toolStripMatchOneOfRange;
		private ToolStripMenuItem toolStripMatchOr;
		private ToolStripMenuItem toolStripMatchUchar;
		private Label labelReplaceWith;
		private TextBox textBoxReplaceWith;
		private CheckBox checkBoxIgnoreCase;
		private Label label1;
		private ComboBox comboBoxPreviousSearches;
		private Button buttonDeleteSavedExpression;
		private System.Windows.Forms.TableLayoutPanel tableLayoutPanel1;

		#endregion

		private FlowLayoutPanel flowLayoutPanelOptions;
		private CheckBox checkBoxMultiline;
		private CheckBox checkBoxSingleLine;
		private CheckBox checkBoxExplicitCapture;
		private CheckBox checkBoxIgnorePatternWhitespace;
		private CheckBox checkBoxRightToLeft;
		private CheckBox checkBoxECMAScript;
		private CheckBox checkBoxCultureInvariant;
		private ToolTip toolTip;
		private ToolStripMenuItem toolStripMatch0or1AsFewAsPossible;
		private ToolStripMenuItem toolStripMatchAtLeastNtimesAsFewAsPossible;
		private ToolStripMenuItem toolStripMatchN2MtimesAsFewAsPossible;
		private LinkLabel linkLabelOpenQuickReference;
		private GroupBox groupBoxRegexOptions;
		private Button buttonAddRegularExpression;
		private DataGridView dataGridViewRegularExpressions;
		private DataGridViewCheckBoxColumn ColumnEnabled;
		private DataGridViewTextBoxColumn ColumnFindWhat;
		private DataGridViewTextBoxColumn ColumnReplaceWith;
		private DataGridViewTextBoxColumn ColumnOptions;
	}
}
