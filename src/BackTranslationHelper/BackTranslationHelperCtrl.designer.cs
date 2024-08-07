
using System;

namespace BackTranslationHelper
{
    partial class BackTranslationHelperCtrl
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

        #region Component Designer generated code

        /// <summary> 
        /// Required method for Designer support - do not modify 
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(BackTranslationHelperCtrl));
            this.menuStrip = new System.Windows.Forms.MenuStrip();
            this.settingsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.removeEncConverterToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.addEncConverterToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.fontsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.sourceTextToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.targetTextToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.displayRighttoleftToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.sourceRightToLeftToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.targetRightToLeftToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.autoSaveToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.hideColumn1LabelsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.hideCurrentTargetTextToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.hideSourceTextToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.SentenceSplittingMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.AddExampleMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.PurgeExamplesMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripTextBoxStatus = new System.Windows.Forms.ToolStripTextBox();
            this.tableLayoutPanel = new System.Windows.Forms.TableLayoutPanel();
            this.labelForSourceData = new System.Windows.Forms.Label();
            this.textBoxSourceData = new System.Windows.Forms.TextBox();
            this.labelForExistingTargetData = new System.Windows.Forms.Label();
            this.textBoxTargetTextExisting = new System.Windows.Forms.TextBox();
            this.buttonFillExistingTargetText = new System.Windows.Forms.Button();
            this.labelForPossibleTargetTranslation1 = new System.Windows.Forms.Label();
            this.textBoxPossibleTargetTranslation1 = new System.Windows.Forms.TextBox();
            this.contextMenuHideDefault = new System.Windows.Forms.ContextMenu();
            this.buttonFillTargetTextOption1 = new System.Windows.Forms.Button();
            this.labelForPossibleTargetTranslation2 = new System.Windows.Forms.Label();
            this.textBoxPossibleTargetTranslation2 = new System.Windows.Forms.TextBox();
            this.buttonFillTargetTextOption2 = new System.Windows.Forms.Button();
            this.labelForPossibleTargetTranslation3 = new System.Windows.Forms.Label();
            this.textBoxPossibleTargetTranslation3 = new System.Windows.Forms.TextBox();
            this.buttonFillTargetTextOption3 = new System.Windows.Forms.Button();
            this.labelForPossibleTargetTranslation4 = new System.Windows.Forms.Label();
            this.textBoxPossibleTargetTranslation4 = new System.Windows.Forms.TextBox();
            this.buttonFillTargetTextOption4 = new System.Windows.Forms.Button();
            this.labelForTargetTranslation = new System.Windows.Forms.Label();
            this.textBoxTargetBackTranslation = new System.Windows.Forms.TextBox();
            this.progressBar = new System.Windows.Forms.ProgressBar();
            this.buttonClose = new System.Windows.Forms.Button();
            this.buttonWriteTextToTarget = new System.Windows.Forms.Button();
            this.buttonNextSection = new System.Windows.Forms.Button();
            this.buttonSkip = new System.Windows.Forms.Button();
            this.buttonSubstitute = new System.Windows.Forms.Button();
            this.substitutionContextMenu = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.addSubtitutionMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.findSubstitutionRuleMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.editSubtitutionsMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.assignNewSubstitutionProjectMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolTip = new System.Windows.Forms.ToolTip(this.components);
            this.buttonPinToTop = new System.Windows.Forms.Button();
            this.buttonPauseUpdating = new System.Windows.Forms.Button();
            this.fontDialog = new System.Windows.Forms.FontDialog();
            this.menuStrip.SuspendLayout();
            this.tableLayoutPanel.SuspendLayout();
            this.substitutionContextMenu.SuspendLayout();
            this.SuspendLayout();
            // 
            // menuStrip
            // 
            this.menuStrip.Dock = System.Windows.Forms.DockStyle.None;
            this.menuStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.settingsToolStripMenuItem,
            this.toolStripTextBoxStatus});
            this.menuStrip.Location = new System.Drawing.Point(0, 0);
            this.menuStrip.Name = "menuStrip";
			this.menuStrip.Size = new System.Drawing.Size(571, 27);
			this.menuStrip.TabIndex = 0;
            this.menuStrip.Text = "menuStrip1";
            // 
            // settingsToolStripMenuItem
            // 
            this.settingsToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.removeEncConverterToolStripMenuItem,
            this.addEncConverterToolStripMenuItem,
            this.fontsToolStripMenuItem,
            this.displayRighttoleftToolStripMenuItem,
            this.autoSaveToolStripMenuItem,
            this.hideColumn1LabelsToolStripMenuItem,
            this.hideCurrentTargetTextToolStripMenuItem,
            this.hideSourceTextToolStripMenuItem,
            this.SentenceSplittingMenuItem,
            this.AddExampleMenuItem,
            this.PurgeExamplesMenuItem});
            this.settingsToolStripMenuItem.Name = "settingsToolStripMenuItem";
            this.settingsToolStripMenuItem.Size = new System.Drawing.Size(61, 23);
            this.settingsToolStripMenuItem.Text = "&Settings";
			this.settingsToolStripMenuItem.DropDownOpening += SettingsToolStripMenuItem_DropDownOpening;
			// 
			// removeEncConverterToolStripMenuItem
			// 
			this.removeEncConverterToolStripMenuItem.Name = "removeEncConverterToolStripMenuItem";
            this.removeEncConverterToolStripMenuItem.Size = new System.Drawing.Size(341, 22);
            this.removeEncConverterToolStripMenuItem.Text = "Edit, Remove or Reorder &Translators/EncConverters";
            this.removeEncConverterToolStripMenuItem.ToolTipText = "Click to bring up a dialog to edit, remove or reorder the Translators/EncConverte" +
    "rs.";
            this.removeEncConverterToolStripMenuItem.Click += new System.EventHandler(this.ChangeEncConverterToolStripMenuItem_Click);
            // 
            // addEncConverterToolStripMenuItem
            // 
            this.addEncConverterToolStripMenuItem.Name = "addEncConverterToolStripMenuItem";
            this.addEncConverterToolStripMenuItem.Size = new System.Drawing.Size(341, 22);
            this.addEncConverterToolStripMenuItem.Text = "&Add Translator/EncConverter";
            this.addEncConverterToolStripMenuItem.ToolTipText = "Click to add an additional Translator/EncConverter to give multiple options for t" +
    "he translated draft of the source text (e.g. DeepL Translator)";
            this.addEncConverterToolStripMenuItem.Click += new System.EventHandler(this.AddEncConverterToolStripMenuItem_Click);
            // 
            // fontsToolStripMenuItem
            // 
            this.fontsToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.sourceTextToolStripMenuItem,
            this.targetTextToolStripMenuItem});
            this.fontsToolStripMenuItem.Name = "fontsToolStripMenuItem";
            this.fontsToolStripMenuItem.Size = new System.Drawing.Size(341, 22);
            this.fontsToolStripMenuItem.Text = "&Fonts";
            // 
            // sourceTextToolStripMenuItem
            // 
            this.sourceTextToolStripMenuItem.Name = "sourceTextToolStripMenuItem";
            this.sourceTextToolStripMenuItem.Size = new System.Drawing.Size(133, 22);
            this.sourceTextToolStripMenuItem.Text = "&Source text";
            this.sourceTextToolStripMenuItem.Click += new System.EventHandler(this.SourceTextToolStripMenuItem_Click);
            // 
            // targetTextToolStripMenuItem
            // 
            this.targetTextToolStripMenuItem.Name = "targetTextToolStripMenuItem";
            this.targetTextToolStripMenuItem.Size = new System.Drawing.Size(133, 22);
            this.targetTextToolStripMenuItem.Text = "&Target text";
            this.targetTextToolStripMenuItem.Click += new System.EventHandler(this.TargetTextToolStripMenuItem_Click);
            // 
            // displayRighttoleftToolStripMenuItem
            // 
            this.displayRighttoleftToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.sourceRightToLeftToolStripMenuItem,
            this.targetRightToLeftToolStripMenuItem});
            this.displayRighttoleftToolStripMenuItem.Name = "displayRighttoleftToolStripMenuItem";
            this.displayRighttoleftToolStripMenuItem.Size = new System.Drawing.Size(341, 22);
            this.displayRighttoleftToolStripMenuItem.Text = "&Display right-to-left";
            this.displayRighttoleftToolStripMenuItem.DropDownOpening += new System.EventHandler(this.DisplayRighttoleftToolStripMenuItem_DropDownOpening);
            // 
            // sourceRightToLeftToolStripMenuItem
            // 
            this.sourceRightToLeftToolStripMenuItem.CheckOnClick = true;
            this.sourceRightToLeftToolStripMenuItem.Name = "sourceRightToLeftToolStripMenuItem";
            this.sourceRightToLeftToolStripMenuItem.Size = new System.Drawing.Size(110, 22);
            this.sourceRightToLeftToolStripMenuItem.Text = "&Source";
            this.sourceRightToLeftToolStripMenuItem.CheckedChanged += new System.EventHandler(this.SourceRtlOverrideToolStripMenuItem_CheckedChanged);
            // 
            // targetRightToLeftToolStripMenuItem
            // 
            this.targetRightToLeftToolStripMenuItem.CheckOnClick = true;
            this.targetRightToLeftToolStripMenuItem.Name = "targetRightToLeftToolStripMenuItem";
            this.targetRightToLeftToolStripMenuItem.Size = new System.Drawing.Size(110, 22);
            this.targetRightToLeftToolStripMenuItem.Text = "&Target";
            this.targetRightToLeftToolStripMenuItem.CheckedChanged += new System.EventHandler(this.TargetRtlOverrideToolRightToLeftStripMenuItem_CheckedChanged);
            // 
            // autoSaveToolStripMenuItem
            // 
            this.autoSaveToolStripMenuItem.Checked = true;
            this.autoSaveToolStripMenuItem.CheckOnClick = true;
            this.autoSaveToolStripMenuItem.CheckState = System.Windows.Forms.CheckState.Checked;
            this.autoSaveToolStripMenuItem.Name = "autoSaveToolStripMenuItem";
            this.autoSaveToolStripMenuItem.Size = new System.Drawing.Size(341, 22);
            this.autoSaveToolStripMenuItem.Text = "&Auto-Save";
            this.autoSaveToolStripMenuItem.ToolTipText = "If this is checked, then the translated text changes will be saved when clicking " +
    "the \'Next\' button";
            // 
            // hideColumn1LabelsToolStripMenuItem
            // 
            this.hideColumn1LabelsToolStripMenuItem.CheckOnClick = true;
            this.hideColumn1LabelsToolStripMenuItem.Name = "hideColumn1LabelsToolStripMenuItem";
            this.hideColumn1LabelsToolStripMenuItem.Size = new System.Drawing.Size(341, 22);
            this.hideColumn1LabelsToolStripMenuItem.Text = "&Hide column1 labels";
            this.hideColumn1LabelsToolStripMenuItem.ToolTipText = "Check this menu item to hide the column 1 labels to make more room for the transl" +
    "ated text options";
            this.hideColumn1LabelsToolStripMenuItem.CheckStateChanged += new System.EventHandler(this.HideColumn1LabelsToolStripMenuItem_CheckStateChanged);
            // 
            // hideCurrentTargetTextToolStripMenuItem
            // 
            this.hideCurrentTargetTextToolStripMenuItem.CheckOnClick = true;
            this.hideCurrentTargetTextToolStripMenuItem.Name = "hideCurrentTargetTextToolStripMenuItem";
            this.hideCurrentTargetTextToolStripMenuItem.Size = new System.Drawing.Size(341, 22);
            this.hideCurrentTargetTextToolStripMenuItem.Text = "Hide &Current Target Text";
            this.hideCurrentTargetTextToolStripMenuItem.ToolTipText = "Check this menu item to hide the Current Target Text row (e.g. to get more screen" +
    " real estate)";
            this.hideCurrentTargetTextToolStripMenuItem.CheckStateChanged += new System.EventHandler(this.HideCurrentTargetTextToolStripMenuItem_CheckStateChanged);
            // 
            // hideSourceTextToolStripMenuItem
            // 
            this.hideSourceTextToolStripMenuItem.CheckOnClick = true;
            this.hideSourceTextToolStripMenuItem.Name = "hideSourceTextToolStripMenuItem";
            this.hideSourceTextToolStripMenuItem.Size = new System.Drawing.Size(341, 22);
            this.hideSourceTextToolStripMenuItem.Text = "Hide &Source Text";
            this.hideSourceTextToolStripMenuItem.ToolTipText = "Check this menu item to hide the Source Text row (e.g. to get more screen real es" +
    "tate)";
            this.hideSourceTextToolStripMenuItem.CheckStateChanged += new System.EventHandler(this.HideSourceTextToolStripMenuItem_CheckStateChanged);
            // 
            // SentenceSplittingMenuItem
            // 
            this.SentenceSplittingMenuItem.CheckOnClick = true;
            this.SentenceSplittingMenuItem.Name = "SentenceSplittingMenuItem";
            this.SentenceSplittingMenuItem.Size = new System.Drawing.Size(341, 22);
            this.SentenceSplittingMenuItem.Text = "Sentence S&plitting";
            this.SentenceSplittingMenuItem.ToolTipText = resources.GetString("SentenceSplittingMenuItem.ToolTipText");
            this.SentenceSplittingMenuItem.CheckStateChanged += new System.EventHandler(this.SplitSentencesMenuItem_CheckStateChanged);
            // 
            // AddExampleMenuItem
            // 
            this.AddExampleMenuItem.Name = "AddExampleMenuItem";
            this.AddExampleMenuItem.Size = new System.Drawing.Size(341, 22);
            this.AddExampleMenuItem.Text = "Add &Example";
            this.AddExampleMenuItem.ToolTipText = resources.GetString("AddExampleMenuItem.ToolTipText");
            this.AddExampleMenuItem.Click += new System.EventHandler(this.AddExampleMenuItem_Click);
            // 
            // PurgeExamplesMenuItem
            // 
            this.PurgeExamplesMenuItem.Name = "PurgeExamplesMenuItem";
            this.PurgeExamplesMenuItem.Size = new System.Drawing.Size(341, 22);
            this.PurgeExamplesMenuItem.Text = "&Purge Examples/Reset Prompt Based Translators";
            this.PurgeExamplesMenuItem.ToolTipText = "For Chat Prompt-based Translators (e.g. Vertex AI or Azure Open AI) click here to" +
    " purge any collected examples or to reset the converter";
            this.PurgeExamplesMenuItem.Click += new System.EventHandler(this.PurgeExamplesMenuItem_Click);
            // 
            // toolStripTextBoxStatus
            // 
            this.toolStripTextBoxStatus.Alignment = System.Windows.Forms.ToolStripItemAlignment.Right;
            this.toolStripTextBoxStatus.BackColor = System.Drawing.SystemColors.ButtonShadow;
            this.toolStripTextBoxStatus.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.toolStripTextBoxStatus.Name = "toolStripTextBoxStatus";
            this.toolStripTextBoxStatus.ReadOnly = true;
            this.toolStripTextBoxStatus.Size = new System.Drawing.Size(500, 23);
            this.toolStripTextBoxStatus.ToolTipText = "This shows if any substitutions were made when processing the text in the \'Target" +
    " Translation\' edit box";
            // 
            // tableLayoutPanel
            // 
            this.tableLayoutPanel.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			// It's possible that if this control is edited, the designer will remove these two lines.
			//    if these two lines are re-added, then the tableLayoutPanel will lose 2 pixels of height
			//    for every verse we process. Removing them caused that to stop (and it still allows resizing)
			//    if the form it's in is resized.
			// Restore and keep this note here too if it is removed, so the next person won't spend hours trying to figure this out too :-)
			// this.tableLayoutPanel.AutoSize = true;
			// this.tableLayoutPanel.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
			this.tableLayoutPanel.ColumnCount = 7;
            this.tableLayoutPanel.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 108F));
            this.tableLayoutPanel.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.tableLayoutPanel.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.tableLayoutPanel.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.tableLayoutPanel.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.tableLayoutPanel.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.tableLayoutPanel.Controls.Add(this.labelForSourceData, 0, 0);
            this.tableLayoutPanel.Controls.Add(this.textBoxSourceData, 1, 0);
            this.tableLayoutPanel.Controls.Add(this.labelForExistingTargetData, 0, 1);
            this.tableLayoutPanel.Controls.Add(this.textBoxTargetTextExisting, 1, 1);
            this.tableLayoutPanel.Controls.Add(this.buttonFillExistingTargetText, 6, 1);
            this.tableLayoutPanel.Controls.Add(this.labelForPossibleTargetTranslation1, 0, 2);
            this.tableLayoutPanel.Controls.Add(this.textBoxPossibleTargetTranslation1, 1, 2);
            this.tableLayoutPanel.Controls.Add(this.buttonFillTargetTextOption1, 6, 2);
            this.tableLayoutPanel.Controls.Add(this.labelForPossibleTargetTranslation2, 0, 3);
            this.tableLayoutPanel.Controls.Add(this.textBoxPossibleTargetTranslation2, 1, 3);
            this.tableLayoutPanel.Controls.Add(this.buttonFillTargetTextOption2, 6, 3);
            this.tableLayoutPanel.Controls.Add(this.labelForPossibleTargetTranslation3, 0, 4);
            this.tableLayoutPanel.Controls.Add(this.textBoxPossibleTargetTranslation3, 1, 4);
            this.tableLayoutPanel.Controls.Add(this.buttonFillTargetTextOption3, 6, 4);
            this.tableLayoutPanel.Controls.Add(this.labelForPossibleTargetTranslation4, 0, 5);
            this.tableLayoutPanel.Controls.Add(this.textBoxPossibleTargetTranslation4, 1, 5);
            this.tableLayoutPanel.Controls.Add(this.buttonFillTargetTextOption4, 6, 5);
            this.tableLayoutPanel.Controls.Add(this.labelForTargetTranslation, 0, 6);
            this.tableLayoutPanel.Controls.Add(this.textBoxTargetBackTranslation, 1, 6);
            this.tableLayoutPanel.Controls.Add(this.progressBar, 0, 7);
            this.tableLayoutPanel.Controls.Add(this.buttonClose, 1, 7);
            this.tableLayoutPanel.Controls.Add(this.buttonWriteTextToTarget, 2, 7);
            this.tableLayoutPanel.Controls.Add(this.buttonNextSection, 3, 7);
            this.tableLayoutPanel.Controls.Add(this.buttonSkip, 4, 7);
            this.tableLayoutPanel.Controls.Add(this.buttonSubstitute, 5, 7);
            this.tableLayoutPanel.Location = new System.Drawing.Point(3, 32);
            this.tableLayoutPanel.Name = "tableLayoutPanel";
            this.tableLayoutPanel.Padding = new System.Windows.Forms.Padding(10);
            this.tableLayoutPanel.RowCount = 8;
            this.tableLayoutPanel.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 14.28571F));
            this.tableLayoutPanel.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 14.28571F));
            this.tableLayoutPanel.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 14.28571F));
            this.tableLayoutPanel.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 14.28571F));
            this.tableLayoutPanel.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 14.28571F));
            this.tableLayoutPanel.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 14.28571F));
            this.tableLayoutPanel.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 14.28571F));
            this.tableLayoutPanel.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel.Size = new System.Drawing.Size(711, 387);
            this.tableLayoutPanel.TabIndex = 1;
            // 
            // labelForSourceData
            // 
            this.labelForSourceData.Anchor = System.Windows.Forms.AnchorStyles.Right;
            this.labelForSourceData.AutoSize = true;
            this.labelForSourceData.Location = new System.Drawing.Point(47, 27);
            this.labelForSourceData.Name = "labelForSourceData";
            this.labelForSourceData.Size = new System.Drawing.Size(68, 13);
            this.labelForSourceData.TabIndex = 9;
            this.labelForSourceData.Text = "Source Text:";
            // 
            // textBoxSourceData
            // 
            this.textBoxSourceData.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.tableLayoutPanel.SetColumnSpan(this.textBoxSourceData, 6);
            this.textBoxSourceData.Dock = System.Windows.Forms.DockStyle.Fill;
            this.textBoxSourceData.Location = new System.Drawing.Point(121, 13);
            this.textBoxSourceData.Multiline = true;
            this.textBoxSourceData.Name = "textBoxSourceData";
            this.textBoxSourceData.ReadOnly = true;
            this.textBoxSourceData.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.textBoxSourceData.Size = new System.Drawing.Size(577, 41);
            this.textBoxSourceData.TabIndex = 11;
            // 
            // labelForExistingTargetData
            // 
            this.labelForExistingTargetData.Anchor = System.Windows.Forms.AnchorStyles.Right;
            this.labelForExistingTargetData.AutoSize = true;
            this.labelForExistingTargetData.Location = new System.Drawing.Point(13, 74);
            this.labelForExistingTargetData.Name = "labelForExistingTargetData";
            this.labelForExistingTargetData.Size = new System.Drawing.Size(102, 13);
            this.labelForExistingTargetData.TabIndex = 9;
            this.labelForExistingTargetData.Text = "Current Target Text:";
            // 
            // textBoxTargetTextExisting
            // 
            this.textBoxTargetTextExisting.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.tableLayoutPanel.SetColumnSpan(this.textBoxTargetTextExisting, 5);
            this.textBoxTargetTextExisting.Dock = System.Windows.Forms.DockStyle.Fill;
            this.textBoxTargetTextExisting.Location = new System.Drawing.Point(121, 60);
            this.textBoxTargetTextExisting.Multiline = true;
            this.textBoxTargetTextExisting.Name = "textBoxTargetTextExisting";
            this.textBoxTargetTextExisting.ReadOnly = true;
            this.textBoxTargetTextExisting.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.textBoxTargetTextExisting.Size = new System.Drawing.Size(548, 41);
            this.textBoxTargetTextExisting.TabIndex = 11;
            // 
            // buttonFillExistingTargetText
            // 
            this.buttonFillExistingTargetText.Image = global::BackTranslationHelper.Properties.Resources.FillDownHS;
            this.buttonFillExistingTargetText.Location = new System.Drawing.Point(675, 60);
            this.buttonFillExistingTargetText.Name = "buttonFillExistingTargetText";
            this.buttonFillExistingTargetText.Size = new System.Drawing.Size(23, 23);
            this.buttonFillExistingTargetText.TabIndex = 8;
            this.buttonFillExistingTargetText.Text = "  &1";
            this.buttonFillExistingTargetText.UseVisualStyleBackColor = true;
            this.buttonFillExistingTargetText.Click += new System.EventHandler(this.ButtonFillExistingTargetText_Click);
            // 
            // labelForPossibleTargetTranslation1
            // 
            this.labelForPossibleTargetTranslation1.Anchor = System.Windows.Forms.AnchorStyles.Right;
            this.labelForPossibleTargetTranslation1.AutoSize = true;
            this.labelForPossibleTargetTranslation1.Location = new System.Drawing.Point(49, 119);
            this.labelForPossibleTargetTranslation1.Margin = new System.Windows.Forms.Padding(3);
            this.labelForPossibleTargetTranslation1.Name = "labelForPossibleTargetTranslation1";
            this.labelForPossibleTargetTranslation1.Padding = new System.Windows.Forms.Padding(0, 3, 0, 0);
            this.labelForPossibleTargetTranslation1.Size = new System.Drawing.Size(66, 16);
            this.labelForPossibleTargetTranslation1.TabIndex = 9;
            this.labelForPossibleTargetTranslation1.Text = "Translator 1:";
            // 
            // textBoxPossibleTargetTranslation1
            // 
            this.textBoxPossibleTargetTranslation1.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.tableLayoutPanel.SetColumnSpan(this.textBoxPossibleTargetTranslation1, 5);
            this.textBoxPossibleTargetTranslation1.ContextMenu = this.contextMenuHideDefault;
            this.textBoxPossibleTargetTranslation1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.textBoxPossibleTargetTranslation1.Location = new System.Drawing.Point(121, 107);
            this.textBoxPossibleTargetTranslation1.Multiline = true;
            this.textBoxPossibleTargetTranslation1.Name = "textBoxPossibleTargetTranslation1";
            this.textBoxPossibleTargetTranslation1.ReadOnly = true;
            this.textBoxPossibleTargetTranslation1.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.textBoxPossibleTargetTranslation1.Size = new System.Drawing.Size(548, 41);
            this.textBoxPossibleTargetTranslation1.TabIndex = 11;
			this.textBoxPossibleTargetTranslation1.ContextMenu = contextMenuHideDefault;
			this.textBoxPossibleTargetTranslation1.PreviewKeyDown += TextBoxPossibleTargetTranslation_PreviewKeyDown;
			this.textBoxPossibleTargetTranslation1.MouseUp += TextBoxPossibleTargetTranslation_MouseUp;
			// 
			// buttonFillTargetTextOption1
			// 
			this.buttonFillTargetTextOption1.Image = global::BackTranslationHelper.Properties.Resources.FillDownHS;
            this.buttonFillTargetTextOption1.Location = new System.Drawing.Point(675, 107);
            this.buttonFillTargetTextOption1.Name = "buttonFillTargetTextOption1";
            this.buttonFillTargetTextOption1.Size = new System.Drawing.Size(23, 23);
            this.buttonFillTargetTextOption1.TabIndex = 8;
            this.toolTip.SetToolTip(this.buttonFillTargetTextOption1, "Click on this button to transfer this translated text to the editable text box be" +
        "low (e.g. if this is the better option to start with).");
            this.buttonFillTargetTextOption1.UseVisualStyleBackColor = true;
            this.buttonFillTargetTextOption1.Click += new System.EventHandler(this.ButtonFillTargetTextOption1_Click);
            // 
            // labelForPossibleTargetTranslation2
            // 
            this.labelForPossibleTargetTranslation2.Anchor = System.Windows.Forms.AnchorStyles.Right;
            this.labelForPossibleTargetTranslation2.AutoSize = true;
            this.labelForPossibleTargetTranslation2.Location = new System.Drawing.Point(49, 166);
            this.labelForPossibleTargetTranslation2.Margin = new System.Windows.Forms.Padding(3);
            this.labelForPossibleTargetTranslation2.Name = "labelForPossibleTargetTranslation2";
            this.labelForPossibleTargetTranslation2.Padding = new System.Windows.Forms.Padding(0, 3, 0, 0);
            this.labelForPossibleTargetTranslation2.Size = new System.Drawing.Size(66, 16);
            this.labelForPossibleTargetTranslation2.TabIndex = 9;
            this.labelForPossibleTargetTranslation2.Text = "Translator 2:";
            // 
            // textBoxPossibleTargetTranslation2
            // 
            this.textBoxPossibleTargetTranslation2.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.tableLayoutPanel.SetColumnSpan(this.textBoxPossibleTargetTranslation2, 5);
            this.textBoxPossibleTargetTranslation2.ContextMenu = this.contextMenuHideDefault;
            this.textBoxPossibleTargetTranslation2.Dock = System.Windows.Forms.DockStyle.Fill;
            this.textBoxPossibleTargetTranslation2.Location = new System.Drawing.Point(121, 154);
            this.textBoxPossibleTargetTranslation2.Multiline = true;
            this.textBoxPossibleTargetTranslation2.Name = "textBoxPossibleTargetTranslation2";
            this.textBoxPossibleTargetTranslation2.ReadOnly = true;
            this.textBoxPossibleTargetTranslation2.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.textBoxPossibleTargetTranslation2.Size = new System.Drawing.Size(548, 41);
            this.textBoxPossibleTargetTranslation2.TabIndex = 12;
			this.textBoxPossibleTargetTranslation2.ContextMenu = contextMenuHideDefault;
			this.textBoxPossibleTargetTranslation2.PreviewKeyDown += TextBoxPossibleTargetTranslation_PreviewKeyDown;
			this.textBoxPossibleTargetTranslation2.MouseUp += TextBoxPossibleTargetTranslation_MouseUp;
			// 
			// buttonFillTargetTextOption2
			// 
			this.buttonFillTargetTextOption2.Image = global::BackTranslationHelper.Properties.Resources.FillDownHS;
            this.buttonFillTargetTextOption2.Location = new System.Drawing.Point(675, 154);
            this.buttonFillTargetTextOption2.Name = "buttonFillTargetTextOption2";
            this.buttonFillTargetTextOption2.Size = new System.Drawing.Size(23, 23);
            this.buttonFillTargetTextOption2.TabIndex = 8;
            this.toolTip.SetToolTip(this.buttonFillTargetTextOption2, "Click to copy this version to the editable box below.");
            this.buttonFillTargetTextOption2.UseVisualStyleBackColor = true;
            this.buttonFillTargetTextOption2.Click += new System.EventHandler(this.ButtonFillTargetTextOption2_Click);
            // 
            // labelForPossibleTargetTranslation3
            // 
            this.labelForPossibleTargetTranslation3.Anchor = System.Windows.Forms.AnchorStyles.Right;
            this.labelForPossibleTargetTranslation3.AutoSize = true;
            this.labelForPossibleTargetTranslation3.Location = new System.Drawing.Point(49, 213);
            this.labelForPossibleTargetTranslation3.Margin = new System.Windows.Forms.Padding(3);
            this.labelForPossibleTargetTranslation3.Name = "labelForPossibleTargetTranslation3";
            this.labelForPossibleTargetTranslation3.Padding = new System.Windows.Forms.Padding(0, 3, 0, 0);
            this.labelForPossibleTargetTranslation3.Size = new System.Drawing.Size(66, 16);
            this.labelForPossibleTargetTranslation3.TabIndex = 9;
            this.labelForPossibleTargetTranslation3.Text = "Translator 3:";
            // 
            // textBoxPossibleTargetTranslation3
            // 
            this.textBoxPossibleTargetTranslation3.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.tableLayoutPanel.SetColumnSpan(this.textBoxPossibleTargetTranslation3, 5);
            this.textBoxPossibleTargetTranslation3.ContextMenu = this.contextMenuHideDefault;
            this.textBoxPossibleTargetTranslation3.Dock = System.Windows.Forms.DockStyle.Fill;
            this.textBoxPossibleTargetTranslation3.Location = new System.Drawing.Point(121, 201);
            this.textBoxPossibleTargetTranslation3.Multiline = true;
            this.textBoxPossibleTargetTranslation3.Name = "textBoxPossibleTargetTranslation3";
            this.textBoxPossibleTargetTranslation3.ReadOnly = true;
            this.textBoxPossibleTargetTranslation3.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.textBoxPossibleTargetTranslation3.Size = new System.Drawing.Size(548, 41);
            this.textBoxPossibleTargetTranslation3.TabIndex = 13;
			this.textBoxPossibleTargetTranslation3.ContextMenu = contextMenuHideDefault;
			this.textBoxPossibleTargetTranslation3.PreviewKeyDown += TextBoxPossibleTargetTranslation_PreviewKeyDown;
			this.textBoxPossibleTargetTranslation3.MouseUp += TextBoxPossibleTargetTranslation_MouseUp;
			// 
			// buttonFillTargetTextOption3
			// 
			this.buttonFillTargetTextOption3.Image = global::BackTranslationHelper.Properties.Resources.FillDownHS;
            this.buttonFillTargetTextOption3.Location = new System.Drawing.Point(675, 201);
            this.buttonFillTargetTextOption3.Name = "buttonFillTargetTextOption3";
            this.buttonFillTargetTextOption3.Size = new System.Drawing.Size(23, 23);
            this.buttonFillTargetTextOption3.TabIndex = 8;
            this.toolTip.SetToolTip(this.buttonFillTargetTextOption3, "Click to copy this version to the editable box below.");
            this.buttonFillTargetTextOption3.UseVisualStyleBackColor = true;
            this.buttonFillTargetTextOption3.Click += new System.EventHandler(this.ButtonFillTargetTextOption3_Click);
            // 
            // labelForPossibleTargetTranslation4
            // 
            this.labelForPossibleTargetTranslation4.Anchor = System.Windows.Forms.AnchorStyles.Right;
            this.labelForPossibleTargetTranslation4.AutoSize = true;
            this.labelForPossibleTargetTranslation4.Location = new System.Drawing.Point(49, 260);
            this.labelForPossibleTargetTranslation4.Margin = new System.Windows.Forms.Padding(3);
            this.labelForPossibleTargetTranslation4.Name = "labelForPossibleTargetTranslation4";
            this.labelForPossibleTargetTranslation4.Padding = new System.Windows.Forms.Padding(0, 3, 0, 0);
            this.labelForPossibleTargetTranslation4.Size = new System.Drawing.Size(66, 16);
            this.labelForPossibleTargetTranslation4.TabIndex = 9;
            this.labelForPossibleTargetTranslation4.Text = "Translator 4:";
            // 
            // textBoxPossibleTargetTranslation4
            // 
            this.textBoxPossibleTargetTranslation4.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.tableLayoutPanel.SetColumnSpan(this.textBoxPossibleTargetTranslation4, 5);
            this.textBoxPossibleTargetTranslation4.ContextMenu = this.contextMenuHideDefault;
            this.textBoxPossibleTargetTranslation4.Dock = System.Windows.Forms.DockStyle.Fill;
            this.textBoxPossibleTargetTranslation4.Location = new System.Drawing.Point(121, 248);
            this.textBoxPossibleTargetTranslation4.Multiline = true;
            this.textBoxPossibleTargetTranslation4.Name = "textBoxPossibleTargetTranslation4";
            this.textBoxPossibleTargetTranslation4.ReadOnly = true;
            this.textBoxPossibleTargetTranslation4.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.textBoxPossibleTargetTranslation4.Size = new System.Drawing.Size(548, 41);
            this.textBoxPossibleTargetTranslation4.TabIndex = 14;
			this.textBoxPossibleTargetTranslation4.ContextMenu = contextMenuHideDefault;
			this.textBoxPossibleTargetTranslation4.PreviewKeyDown += TextBoxPossibleTargetTranslation_PreviewKeyDown;
			this.textBoxPossibleTargetTranslation4.MouseUp += TextBoxPossibleTargetTranslation_MouseUp;
			// 
			// buttonFillTargetTextOption4
			// 
			this.buttonFillTargetTextOption4.Image = global::BackTranslationHelper.Properties.Resources.FillDownHS;
            this.buttonFillTargetTextOption4.Location = new System.Drawing.Point(675, 248);
            this.buttonFillTargetTextOption4.Name = "buttonFillTargetTextOption4";
            this.buttonFillTargetTextOption4.Size = new System.Drawing.Size(23, 23);
            this.buttonFillTargetTextOption4.TabIndex = 9;
            this.toolTip.SetToolTip(this.buttonFillTargetTextOption4, "Click to copy this version to the editable box below.");
            this.buttonFillTargetTextOption4.UseVisualStyleBackColor = true;
            this.buttonFillTargetTextOption4.Click += new System.EventHandler(this.ButtonFillTargetTextOption4_Click);
            // 
            // labelForTargetTranslation
            // 
            this.labelForTargetTranslation.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.labelForTargetTranslation.AutoSize = true;
            this.labelForTargetTranslation.Location = new System.Drawing.Point(19, 295);
            this.labelForTargetTranslation.Margin = new System.Windows.Forms.Padding(3);
            this.labelForTargetTranslation.Name = "labelForTargetTranslation";
            this.labelForTargetTranslation.Padding = new System.Windows.Forms.Padding(0, 3, 0, 0);
            this.labelForTargetTranslation.Size = new System.Drawing.Size(96, 16);
            this.labelForTargetTranslation.TabIndex = 9;
            this.labelForTargetTranslation.Text = "Target Translation:";
            // 
            // textBoxTargetBackTranslation
            // 
            this.tableLayoutPanel.SetColumnSpan(this.textBoxTargetBackTranslation, 6);
            this.textBoxTargetBackTranslation.Dock = System.Windows.Forms.DockStyle.Fill;
            this.textBoxTargetBackTranslation.Location = new System.Drawing.Point(121, 295);
            this.textBoxTargetBackTranslation.Multiline = true;
            this.textBoxTargetBackTranslation.Name = "textBoxTargetBackTranslation";
            this.textBoxTargetBackTranslation.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.textBoxTargetBackTranslation.Size = new System.Drawing.Size(577, 41);
            this.textBoxTargetBackTranslation.TabIndex = 1;
            this.textBoxTargetBackTranslation.Enter += new System.EventHandler(this.TextBoxTargetBackTranslation_Enter);
            this.textBoxTargetBackTranslation.PreviewKeyDown += new System.Windows.Forms.PreviewKeyDownEventHandler(this.TextBoxTargetBackTranslation_PreviewKeyDown);
			this.textBoxTargetBackTranslation.TextChanged += TextBoxTargetBackTranslation_TextChanged;
			// 
			// progressBar
			// 
			this.progressBar.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.progressBar.Location = new System.Drawing.Point(13, 351);
            this.progressBar.Name = "progressBar";
            this.progressBar.Size = new System.Drawing.Size(102, 23);
            this.progressBar.TabIndex = 13;
            this.toolTip.SetToolTip(this.progressBar, "Click to cancel long running translations");
            this.progressBar.Visible = false;
            // 
            // buttonClose
            // 
            this.buttonClose.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.buttonClose.Location = new System.Drawing.Point(121, 349);
            this.buttonClose.Name = "buttonClose";
            this.buttonClose.Size = new System.Drawing.Size(75, 25);
            this.buttonClose.TabIndex = 6;
            this.buttonClose.Text = "Close";
            this.toolTip.SetToolTip(this.buttonClose, "Click to close this dialog");
            this.buttonClose.UseVisualStyleBackColor = true;
            this.buttonClose.Click += new System.EventHandler(this.ButtonClose_Click);
            // 
            // buttonWriteTextToTarget
            // 
            this.buttonWriteTextToTarget.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.buttonWriteTextToTarget.Location = new System.Drawing.Point(202, 349);
            this.buttonWriteTextToTarget.Name = "buttonWriteTextToTarget";
            this.buttonWriteTextToTarget.Size = new System.Drawing.Size(104, 25);
            this.buttonWriteTextToTarget.TabIndex = 2;
            this.buttonWriteTextToTarget.Text = "&Save Changes";
            this.toolTip.SetToolTip(this.buttonWriteTextToTarget, "Click to save/write out the translated text back to the main program (e.g. Parate" +
        "xt or Word).");
            this.buttonWriteTextToTarget.UseVisualStyleBackColor = true;
            this.buttonWriteTextToTarget.Click += new System.EventHandler(this.ButtonWriteTextToTarget_Click);
            // 
            // buttonNextSection
            // 
            this.buttonNextSection.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.buttonNextSection.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.buttonNextSection.Location = new System.Drawing.Point(312, 349);
            this.buttonNextSection.Name = "buttonNextSection";
            this.buttonNextSection.Size = new System.Drawing.Size(75, 25);
            this.buttonNextSection.TabIndex = 3;
            this.buttonNextSection.Text = "&Next";
            this.toolTip.SetToolTip(this.buttonNextSection, "Click to save/write out the translated text back to the main program and move to " +
        "the next verse (Paratext) or paragraph (Word).");
            this.buttonNextSection.UseVisualStyleBackColor = true;
            this.buttonNextSection.Click += new System.EventHandler(this.ButtonNextSection_Click);
            // 
            // buttonSkip
            // 
            this.buttonSkip.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.buttonSkip.Location = new System.Drawing.Point(393, 349);
            this.buttonSkip.Name = "buttonSkip";
            this.buttonSkip.Size = new System.Drawing.Size(75, 25);
            this.buttonSkip.TabIndex = 10;
            this.buttonSkip.Text = "&Skip";
            this.toolTip.SetToolTip(this.buttonSkip, "Click to move to the next verse (Paratext) or paragraph (Word) without saving/wri" +
        "ting out the translated text.");
            this.buttonSkip.UseVisualStyleBackColor = true;
            this.buttonSkip.Click += new System.EventHandler(this.ButtonSkip_Click);
            // 
            // buttonSubstitute
            // 
            this.buttonSubstitute.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.buttonSubstitute.ContextMenuStrip = this.substitutionContextMenu;
            this.buttonSubstitute.Location = new System.Drawing.Point(474, 349);
            this.buttonSubstitute.Name = "buttonSubstitute";
            this.buttonSubstitute.Size = new System.Drawing.Size(119, 25);
            this.buttonSubstitute.TabIndex = 11;
            this.buttonSubstitute.Text = "&Add Substitution";
            this.toolTip.SetToolTip(this.buttonSubstitute, "Select a word or phrase in the \'Target Translation\' box that you want to configur" +
        "e a substitute for (think: Find and Replace) and click this button");
            this.buttonSubstitute.UseVisualStyleBackColor = true;
            this.buttonSubstitute.Click += new System.EventHandler(this.ButtonSubstitute_Click);
            // 
            // substitutionContextMenu
            // 
            this.substitutionContextMenu.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.addSubtitutionMenuItem,
            this.findSubstitutionRuleMenuItem,
            this.editSubtitutionsMenuItem,
            this.assignNewSubstitutionProjectMenuItem});
            this.substitutionContextMenu.Name = "substitutionContextMenu";
            this.substitutionContextMenu.Size = new System.Drawing.Size(285, 92);
            // 
            // addSubtitutionMenuItem
            // 
            this.addSubtitutionMenuItem.Name = "addSubtitutionMenuItem";
            this.addSubtitutionMenuItem.Size = new System.Drawing.Size(284, 22);
            this.addSubtitutionMenuItem.Text = "&Add Substitution";
            this.addSubtitutionMenuItem.ToolTipText = "Select a word or phrase in the \'Target Translation\' box that you want to configur" +
    "e a substitute for (think: Find and Replace) and click this menu item (or just c" +
    "lick the associated button)";
            this.addSubtitutionMenuItem.Click += new System.EventHandler(this.ButtonSubstitute_Click);
            // 
            // findSubstitutionRuleMenuItem
            // 
            this.findSubstitutionRuleMenuItem.Name = "findSubstitutionRuleMenuItem";
            this.findSubstitutionRuleMenuItem.Size = new System.Drawing.Size(284, 22);
            this.findSubstitutionRuleMenuItem.Text = "&Find Substitution Rule";
            this.findSubstitutionRuleMenuItem.ToolTipText = "Select a word or phrase in the \'Target Translation\' box that you want to find the" +
    " substitution rule for and click this menu item (e.g. to add a boundary conditio" +
    "n if the find/replace was too \'greedy\')";
            this.findSubstitutionRuleMenuItem.Click += new System.EventHandler(this.FindSubstitutionRuleMenuItem_Click);
            // 
            // editSubtitutionsMenuItem
            // 
            this.editSubtitutionsMenuItem.Name = "editSubtitutionsMenuItem";
            this.editSubtitutionsMenuItem.Size = new System.Drawing.Size(284, 22);
            this.editSubtitutionsMenuItem.Text = "&Edit Substitutions";
            this.editSubtitutionsMenuItem.ToolTipText = "Click this to see the existing substitution so you can edit one or more of them (" +
    "e.g. to add a boundary condition if the find/replace was too \'greedy\')";
            this.editSubtitutionsMenuItem.Click += new System.EventHandler(this.EditSubtitutionsMenuItem_Click);
            // 
            // assignNewSubstitutionProjectMenuItem
            // 
            this.assignNewSubstitutionProjectMenuItem.Name = "assignNewSubstitutionProjectMenuItem";
            this.assignNewSubstitutionProjectMenuItem.Size = new System.Drawing.Size(284, 22);
            this.assignNewSubstitutionProjectMenuItem.Text = "&Choose/Create new substitution project";
            this.assignNewSubstitutionProjectMenuItem.Click += new System.EventHandler(this.AssignNewSubstitutionProjectMenuItem_Click);
            // 
            // buttonPinToTop
            // 
            this.buttonPinToTop.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.buttonPinToTop.Image = global::BackTranslationHelper.Properties.Resources.pinup;
            this.buttonPinToTop.Location = new System.Drawing.Point(685, 0);
            this.buttonPinToTop.Margin = new System.Windows.Forms.Padding(0);
            this.buttonPinToTop.Name = "buttonPinToTop";
            this.buttonPinToTop.Size = new System.Drawing.Size(30, 30);
            this.buttonPinToTop.TabIndex = 3;
            this.buttonPinToTop.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageAboveText;
            this.toolTip.SetToolTip(this.buttonPinToTop, "Toggle this button to pin this dialog box so that it remains on top of other wind" +
        "ows");
            this.buttonPinToTop.UseVisualStyleBackColor = true;
            this.buttonPinToTop.Click += new System.EventHandler(this.ButtonPinToTop_Click);
            // 
            // buttonPauseUpdating
            // 
            this.buttonPauseUpdating.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.buttonPauseUpdating.Image = global::BackTranslationHelper.Properties.Resources.Pause;
            this.buttonPauseUpdating.Location = new System.Drawing.Point(655, 0);
            this.buttonPauseUpdating.Margin = new System.Windows.Forms.Padding(0);
            this.buttonPauseUpdating.Name = "buttonPauseUpdating";
            this.buttonPauseUpdating.Size = new System.Drawing.Size(30, 30);
            this.buttonPauseUpdating.TabIndex = 3;
			this.buttonPauseUpdating.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageAboveText;
			this.toolTip.SetToolTip(this.buttonPauseUpdating, "Toggle this button to pause/restart automatic translation when clicking in another verse in a Paratext Project (i.e. so the new verse doesn't automatically get translated)");
			this.buttonPauseUpdating.UseVisualStyleBackColor = true;
            this.buttonPauseUpdating.Visible = false;
            this.buttonPauseUpdating.Click += new System.EventHandler(this.ButtonPauseUpdating_Click);
            // 
            // fontDialog
            // 
            this.fontDialog.Font = new System.Drawing.Font("Arial Unicode MS", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.fontDialog.ShowColor = true;
            // 
            // BackTranslationHelperCtrl
            // 
            this.AutoSize = true;
            this.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.Controls.Add(this.buttonPauseUpdating);
            this.Controls.Add(this.buttonPinToTop);
            this.Controls.Add(this.tableLayoutPanel);
            this.Controls.Add(this.menuStrip);
            this.Name = "BackTranslationHelperCtrl";
            this.Size = new System.Drawing.Size(717, 419);
            this.menuStrip.ResumeLayout(false);
            this.menuStrip.PerformLayout();
            this.tableLayoutPanel.ResumeLayout(false);
            this.tableLayoutPanel.PerformLayout();
            this.substitutionContextMenu.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.MenuStrip menuStrip;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel;
        private System.Windows.Forms.TextBox textBoxTargetBackTranslation;
        private System.Windows.Forms.Button buttonWriteTextToTarget;
        private System.Windows.Forms.Button buttonNextSection;
        private System.Windows.Forms.ToolTip toolTip;
        private System.Windows.Forms.Button buttonClose;
        private System.Windows.Forms.ToolStripMenuItem settingsToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem removeEncConverterToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem fontsToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem sourceTextToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem targetTextToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem sourceRightToLeftToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem targetRightToLeftToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem autoSaveToolStripMenuItem;
        private System.Windows.Forms.Button buttonFillTargetTextOption1;
        private System.Windows.Forms.ToolStripMenuItem addEncConverterToolStripMenuItem;
        private System.Windows.Forms.Button buttonFillTargetTextOption2;
        private System.Windows.Forms.Button buttonFillTargetTextOption3;
        private System.Windows.Forms.Button buttonFillTargetTextOption4;
        private System.Windows.Forms.Label labelForSourceData;
        private System.Windows.Forms.Label labelForExistingTargetData;
        private System.Windows.Forms.Label labelForPossibleTargetTranslation1;
        private System.Windows.Forms.Label labelForTargetTranslation;
        private System.Windows.Forms.ToolStripMenuItem hideColumn1LabelsToolStripMenuItem;
        private System.Windows.Forms.Button buttonFillExistingTargetText;
        private System.Windows.Forms.Button buttonSkip;
        private System.Windows.Forms.FontDialog fontDialog;
        private System.Windows.Forms.TextBox textBoxTargetTextExisting;
        private System.Windows.Forms.TextBox textBoxSourceData;
        private System.Windows.Forms.TextBox textBoxPossibleTargetTranslation1;
        private System.Windows.Forms.TextBox textBoxPossibleTargetTranslation2;
        private System.Windows.Forms.TextBox textBoxPossibleTargetTranslation3;
        private System.Windows.Forms.TextBox textBoxPossibleTargetTranslation4;
        private System.Windows.Forms.ToolStripMenuItem displayRighttoleftToolStripMenuItem;
        private System.Windows.Forms.ToolStripTextBox toolStripTextBoxStatus;
        private System.Windows.Forms.Button buttonSubstitute;
        private System.Windows.Forms.ContextMenuStrip substitutionContextMenu;
        private System.Windows.Forms.ToolStripMenuItem addSubtitutionMenuItem;
        private System.Windows.Forms.ToolStripMenuItem findSubstitutionRuleMenuItem;
        private System.Windows.Forms.ToolStripMenuItem editSubtitutionsMenuItem;
        private System.Windows.Forms.ToolStripMenuItem assignNewSubstitutionProjectMenuItem;
        private System.Windows.Forms.ToolStripMenuItem hideCurrentTargetTextToolStripMenuItem;
        private System.Windows.Forms.Button buttonPinToTop;
        private System.Windows.Forms.ToolStripMenuItem hideSourceTextToolStripMenuItem;
        private System.Windows.Forms.ProgressBar progressBar;
        private System.Windows.Forms.Label labelForPossibleTargetTranslation2;
        private System.Windows.Forms.Label labelForPossibleTargetTranslation3;
        private System.Windows.Forms.Label labelForPossibleTargetTranslation4;
        private System.Windows.Forms.ContextMenu contextMenuHideDefault;
        public System.Windows.Forms.ToolStripMenuItem SentenceSplittingMenuItem;
		private System.Windows.Forms.ToolStripMenuItem AddExampleMenuItem;
		private System.Windows.Forms.ToolStripMenuItem PurgeExamplesMenuItem;
		public System.Windows.Forms.Button buttonPauseUpdating;
	}
}
