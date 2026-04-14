namespace SpellingFixer30
{
    partial class SFProjectForm
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
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(SFProjectForm));
            this.tabControl = new System.Windows.Forms.TabControl();
            this.tabPageLanguage = new System.Windows.Forms.TabPage();
            this.tableLayoutPanel = new System.Windows.Forms.TableLayoutPanel();
            this.labelProjectName = new System.Windows.Forms.Label();
            this.textBoxProjectName = new System.Windows.Forms.TextBox();
            this.label2 = new System.Windows.Forms.Label();
            this.comboBoxLocale = new System.Windows.Forms.ComboBox();
            this.labelScriptSystemParameter = new System.Windows.Forms.Label();
            this.comboBoxVernScriptType = new System.Windows.Forms.ComboBox();
            this.labelVernFont = new System.Windows.Forms.Label();
            this.buttonFont = new System.Windows.Forms.Button();
            this.labelVFont = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.buttonChooseTransliterator = new System.Windows.Forms.Button();
            this.labelTransliterator = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.buttonTransliteratorFont = new System.Windows.Forms.Button();
            this.labelTransliterationFont = new System.Windows.Forms.Label();
            this.tabPageDistinctionRules = new System.Windows.Forms.TabPage();
            this.tableLayoutPanelDistinctions = new System.Windows.Forms.TableLayoutPanel();
            this.buttonSetAll = new System.Windows.Forms.Button();
            this.buttonClearAll = new System.Windows.Forms.Button();
            this.labelInstructions = new System.Windows.Forms.Label();
            this.checkBoxCompareDoubleChars = new System.Windows.Forms.CheckBox();
            this.checkBoxIgnoreCase = new System.Windows.Forms.CheckBox();
            this.buttonAddNew = new System.Windows.Forms.Button();
            this.flowLayoutPanel = new System.Windows.Forms.FlowLayoutPanel();
            this.fontDialogDisplay = new System.Windows.Forms.FontDialog();
            this.tableLayoutPanelTabs = new System.Windows.Forms.TableLayoutPanel();
            this.buttonOK = new System.Windows.Forms.Button();
            this.buttonCancel = new System.Windows.Forms.Button();
            this.toolTip = new System.Windows.Forms.ToolTip(this.components);
            this.helpProvider = new System.Windows.Forms.HelpProvider();
            this.saveFileDialogVssFile = new System.Windows.Forms.SaveFileDialog();
            this.labelAddlPunct = new System.Windows.Forms.Label();
            this.textBoxAdditionalPunctuation = new System.Windows.Forms.TextBox();
            this.tabControl.SuspendLayout();
            this.tabPageLanguage.SuspendLayout();
            this.tableLayoutPanel.SuspendLayout();
            this.tabPageDistinctionRules.SuspendLayout();
            this.tableLayoutPanelDistinctions.SuspendLayout();
            this.tableLayoutPanelTabs.SuspendLayout();
            this.SuspendLayout();
            // 
            // tabControl
            // 
            this.tableLayoutPanelTabs.SetColumnSpan(this.tabControl, 2);
            this.tabControl.Controls.Add(this.tabPageLanguage);
            this.tabControl.Controls.Add(this.tabPageDistinctionRules);
            this.tabControl.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tabControl.Location = new System.Drawing.Point(3, 3);
            this.tabControl.Name = "tabControl";
            this.tabControl.SelectedIndex = 0;
            this.tabControl.Size = new System.Drawing.Size(478, 354);
            this.tabControl.TabIndex = 0;
            this.tabControl.Enter += new System.EventHandler(this.tabControl_Enter);
            // 
            // tabPageLanguage
            // 
            this.tabPageLanguage.Controls.Add(this.tableLayoutPanel);
            this.tabPageLanguage.Location = new System.Drawing.Point(4, 22);
            this.tabPageLanguage.Name = "tabPageLanguage";
            this.tabPageLanguage.Padding = new System.Windows.Forms.Padding(3);
            this.tabPageLanguage.Size = new System.Drawing.Size(470, 328);
            this.tabPageLanguage.TabIndex = 0;
            this.tabPageLanguage.Text = "Language";
            this.tabPageLanguage.UseVisualStyleBackColor = true;
            // 
            // tableLayoutPanel
            // 
            this.tableLayoutPanel.ColumnCount = 3;
            this.tableLayoutPanel.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 150F));
            this.tableLayoutPanel.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.tableLayoutPanel.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel.Controls.Add(this.labelProjectName, 0, 0);
            this.tableLayoutPanel.Controls.Add(this.textBoxProjectName, 1, 0);
            this.tableLayoutPanel.Controls.Add(this.label2, 0, 1);
            this.tableLayoutPanel.Controls.Add(this.comboBoxLocale, 1, 1);
            this.tableLayoutPanel.Controls.Add(this.labelScriptSystemParameter, 0, 2);
            this.tableLayoutPanel.Controls.Add(this.comboBoxVernScriptType, 1, 2);
            this.tableLayoutPanel.Controls.Add(this.labelVernFont, 0, 3);
            this.tableLayoutPanel.Controls.Add(this.buttonFont, 1, 3);
            this.tableLayoutPanel.Controls.Add(this.labelVFont, 2, 3);
            this.tableLayoutPanel.Controls.Add(this.label1, 0, 4);
            this.tableLayoutPanel.Controls.Add(this.buttonChooseTransliterator, 1, 4);
            this.tableLayoutPanel.Controls.Add(this.labelTransliterator, 2, 4);
            this.tableLayoutPanel.Controls.Add(this.label3, 0, 5);
            this.tableLayoutPanel.Controls.Add(this.buttonTransliteratorFont, 1, 5);
            this.tableLayoutPanel.Controls.Add(this.labelTransliterationFont, 2, 5);
			this.tableLayoutPanel.Controls.Add(this.labelAddlPunct, 0, 6);
			this.tableLayoutPanel.Controls.Add(this.textBoxAdditionalPunctuation, 1, 6);
			this.tableLayoutPanel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanel.Location = new System.Drawing.Point(3, 3);
            this.tableLayoutPanel.Name = "tableLayoutPanel";
            this.tableLayoutPanel.RowCount = 8;
            this.tableLayoutPanel.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel.Size = new System.Drawing.Size(464, 322);
            this.tableLayoutPanel.TabIndex = 0;
            // 
            // labelProjectName
            // 
            this.labelProjectName.Anchor = System.Windows.Forms.AnchorStyles.Right;
            this.labelProjectName.AutoSize = true;
            this.labelProjectName.Location = new System.Drawing.Point(20, 6);
            this.labelProjectName.Name = "labelProjectName";
            this.labelProjectName.Size = new System.Drawing.Size(127, 13);
            this.labelProjectName.TabIndex = 0;
            this.labelProjectName.Text = "Project/Language &Name:";
            // 
            // textBoxProjectName
            // 
            this.tableLayoutPanel.SetColumnSpan(this.textBoxProjectName, 2);
            this.textBoxProjectName.Dock = System.Windows.Forms.DockStyle.Fill;
            this.helpProvider.SetHelpString(this.textBoxProjectName, resources.GetString("textBoxProjectName.HelpString"));
            this.textBoxProjectName.Location = new System.Drawing.Point(153, 3);
            this.textBoxProjectName.Name = "textBoxProjectName";
            this.helpProvider.SetShowHelp(this.textBoxProjectName, true);
            this.textBoxProjectName.Size = new System.Drawing.Size(308, 20);
            this.textBoxProjectName.TabIndex = 0;
            // 
            // label2
            // 
            this.label2.Anchor = System.Windows.Forms.AnchorStyles.Right;
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(15, 33);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(132, 13);
            this.label2.TabIndex = 2;
            this.label2.Text = "System &Language/Locale:";
            // 
            // comboBoxLocale
            // 
            this.comboBoxLocale.AutoCompleteMode = System.Windows.Forms.AutoCompleteMode.SuggestAppend;
            this.comboBoxLocale.AutoCompleteSource = System.Windows.Forms.AutoCompleteSource.ListItems;
            this.tableLayoutPanel.SetColumnSpan(this.comboBoxLocale, 2);
            this.comboBoxLocale.Dock = System.Windows.Forms.DockStyle.Fill;
            this.comboBoxLocale.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.comboBoxLocale.FormattingEnabled = true;
            this.helpProvider.SetHelpString(this.comboBoxLocale, resources.GetString("comboBoxLocale.HelpString"));
            this.comboBoxLocale.Location = new System.Drawing.Point(153, 29);
            this.comboBoxLocale.Name = "comboBoxLocale";
            this.helpProvider.SetShowHelp(this.comboBoxLocale, true);
            this.comboBoxLocale.Size = new System.Drawing.Size(308, 21);
            this.comboBoxLocale.Sorted = true;
            this.comboBoxLocale.TabIndex = 1;
            // 
            // labelScriptSystemParameter
            // 
            this.labelScriptSystemParameter.Anchor = System.Windows.Forms.AnchorStyles.Right;
            this.labelScriptSystemParameter.AutoSize = true;
            this.labelScriptSystemParameter.Location = new System.Drawing.Point(29, 60);
            this.labelScriptSystemParameter.Name = "labelScriptSystemParameter";
            this.labelScriptSystemParameter.Size = new System.Drawing.Size(118, 13);
            this.labelScriptSystemParameter.TabIndex = 4;
            this.labelScriptSystemParameter.Text = "Vernacular &Script Type:";
            // 
            // comboBoxVernScriptType
            // 
            this.comboBoxVernScriptType.AutoCompleteMode = System.Windows.Forms.AutoCompleteMode.SuggestAppend;
            this.comboBoxVernScriptType.AutoCompleteSource = System.Windows.Forms.AutoCompleteSource.ListItems;
            this.tableLayoutPanel.SetColumnSpan(this.comboBoxVernScriptType, 2);
            this.comboBoxVernScriptType.Dock = System.Windows.Forms.DockStyle.Fill;
            this.comboBoxVernScriptType.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.comboBoxVernScriptType.FormattingEnabled = true;
            this.helpProvider.SetHelpString(this.comboBoxVernScriptType, "The Vernacular Script Type");
            this.comboBoxVernScriptType.Location = new System.Drawing.Point(153, 56);
            this.comboBoxVernScriptType.Name = "comboBoxVernScriptType";
            this.helpProvider.SetShowHelp(this.comboBoxVernScriptType, true);
            this.comboBoxVernScriptType.Size = new System.Drawing.Size(308, 21);
            this.comboBoxVernScriptType.TabIndex = 2;
            this.comboBoxVernScriptType.SelectedIndexChanged += new System.EventHandler(this.comboBoxVernScriptType_SelectedIndexChanged);
            // 
            // labelVernFont
            // 
            this.labelVernFont.Anchor = System.Windows.Forms.AnchorStyles.Right;
            this.labelVernFont.AutoSize = true;
            this.labelVernFont.Location = new System.Drawing.Point(32, 88);
            this.labelVernFont.Name = "labelVernFont";
            this.labelVernFont.Size = new System.Drawing.Size(115, 13);
            this.labelVernFont.TabIndex = 5;
            this.labelVernFont.Text = "Vernacular &Script Font:";
            // 
            // buttonFont
            // 
            this.buttonFont.AutoSize = true;
            this.buttonFont.Enabled = false;
            this.helpProvider.SetHelpString(this.buttonFont, "Select the font to be used when displaying vernacular data for this project (defa" +
        "ult is \'Arial Unicode MS\'; 14 pt)");
            this.buttonFont.Location = new System.Drawing.Point(153, 83);
            this.buttonFont.Name = "buttonFont";
            this.helpProvider.SetShowHelp(this.buttonFont, true);
            this.buttonFont.Size = new System.Drawing.Size(94, 23);
            this.buttonFont.TabIndex = 3;
            this.buttonFont.Text = "Choose &Font";
            this.buttonFont.UseVisualStyleBackColor = true;
            this.buttonFont.Click += new System.EventHandler(this.buttonFont_Click);
            // 
            // labelVFont
            // 
            this.labelVFont.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.labelVFont.AutoSize = true;
            this.labelVFont.Location = new System.Drawing.Point(276, 88);
            this.labelVFont.Name = "labelVFont";
            this.labelVFont.Size = new System.Drawing.Size(72, 13);
            this.labelVFont.TabIndex = 10;
            this.labelVFont.Text = "labelVernFont";
            // 
            // label1
            // 
            this.label1.Anchor = System.Windows.Forms.AnchorStyles.Right;
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(76, 117);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(71, 13);
            this.label1.TabIndex = 6;
            this.label1.Text = "Transliterator:";
            // 
            // buttonChooseTransliterator
            // 
            this.buttonChooseTransliterator.AutoSize = true;
            this.helpProvider.SetHelpString(this.buttonChooseTransliterator, "Select the transliterator to be used for this project (default is \'Any to Latin\')" +
        "");
            this.buttonChooseTransliterator.Location = new System.Drawing.Point(153, 112);
            this.buttonChooseTransliterator.Name = "buttonChooseTransliterator";
            this.helpProvider.SetShowHelp(this.buttonChooseTransliterator, true);
            this.buttonChooseTransliterator.Size = new System.Drawing.Size(117, 23);
            this.buttonChooseTransliterator.TabIndex = 7;
            this.buttonChooseTransliterator.Text = "Choose Transliterator";
            this.buttonChooseTransliterator.UseVisualStyleBackColor = true;
            this.buttonChooseTransliterator.Click += new System.EventHandler(this.buttonChooseTransliterator_Click);
            // 
            // labelTransliterator
            // 
            this.labelTransliterator.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.labelTransliterator.AutoSize = true;
            this.labelTransliterator.Location = new System.Drawing.Point(276, 117);
            this.labelTransliterator.Name = "labelTransliterator";
            this.labelTransliterator.Size = new System.Drawing.Size(90, 13);
            this.labelTransliterator.TabIndex = 11;
            this.labelTransliterator.Text = "labelTransliterator";
            // 
            // label3
            // 
            this.label3.Anchor = System.Windows.Forms.AnchorStyles.Right;
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(52, 146);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(95, 13);
            this.label3.TabIndex = 8;
            this.label3.Text = "Transliterator Font:";
            // 
            // buttonTransliteratorFont
            // 
            this.buttonTransliteratorFont.AutoSize = true;
            this.helpProvider.SetHelpString(this.buttonTransliteratorFont, "Select the font to be used when displaying transliteration data for this project " +
        "(default is \'Doulos SIL\'; 12 pt)");
            this.buttonTransliteratorFont.Location = new System.Drawing.Point(153, 141);
            this.buttonTransliteratorFont.Name = "buttonTransliteratorFont";
            this.helpProvider.SetShowHelp(this.buttonTransliteratorFont, true);
            this.buttonTransliteratorFont.Size = new System.Drawing.Size(94, 23);
            this.buttonTransliteratorFont.TabIndex = 9;
            this.buttonTransliteratorFont.Text = "Choose &Font";
            this.buttonTransliteratorFont.UseVisualStyleBackColor = true;
            this.buttonTransliteratorFont.Click += new System.EventHandler(this.buttonTransliteratorFont_Click);
            // 
            // labelTransliterationFont
            // 
            this.labelTransliterationFont.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.labelTransliterationFont.AutoSize = true;
            this.labelTransliterationFont.Location = new System.Drawing.Point(276, 146);
            this.labelTransliterationFont.Name = "labelTransliterationFont";
            this.labelTransliterationFont.Size = new System.Drawing.Size(116, 13);
            this.labelTransliterationFont.TabIndex = 12;
            this.labelTransliterationFont.Text = "labelTransliterationFont";
            // 
            // tabPageDistinctionRules
            // 
            this.tabPageDistinctionRules.Controls.Add(this.tableLayoutPanelDistinctions);
            this.tabPageDistinctionRules.Location = new System.Drawing.Point(4, 22);
            this.tabPageDistinctionRules.Name = "tabPageDistinctionRules";
            this.tabPageDistinctionRules.Padding = new System.Windows.Forms.Padding(3);
            this.tabPageDistinctionRules.Size = new System.Drawing.Size(470, 328);
            this.tabPageDistinctionRules.TabIndex = 1;
            this.tabPageDistinctionRules.Text = "Inconsistency Settings";
            this.tabPageDistinctionRules.UseVisualStyleBackColor = true;
            // 
            // tableLayoutPanelDistinctions
            // 
            this.tableLayoutPanelDistinctions.ColumnCount = 4;
            this.tableLayoutPanelDistinctions.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.tableLayoutPanelDistinctions.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.tableLayoutPanelDistinctions.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 66.66666F));
            this.tableLayoutPanelDistinctions.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 33.33333F));
            this.tableLayoutPanelDistinctions.Controls.Add(this.buttonSetAll, 0, 2);
            this.tableLayoutPanelDistinctions.Controls.Add(this.buttonClearAll, 1, 2);
            this.tableLayoutPanelDistinctions.Controls.Add(this.labelInstructions, 3, 3);
            this.tableLayoutPanelDistinctions.Controls.Add(this.checkBoxCompareDoubleChars, 0, 0);
            this.tableLayoutPanelDistinctions.Controls.Add(this.checkBoxIgnoreCase, 0, 1);
            this.tableLayoutPanelDistinctions.Controls.Add(this.buttonAddNew, 2, 2);
            this.tableLayoutPanelDistinctions.Controls.Add(this.flowLayoutPanel, 0, 3);
            this.tableLayoutPanelDistinctions.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanelDistinctions.Location = new System.Drawing.Point(3, 3);
            this.tableLayoutPanelDistinctions.Name = "tableLayoutPanelDistinctions";
            this.tableLayoutPanelDistinctions.RowCount = 4;
            this.tableLayoutPanelDistinctions.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanelDistinctions.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanelDistinctions.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanelDistinctions.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanelDistinctions.Size = new System.Drawing.Size(464, 322);
            this.tableLayoutPanelDistinctions.TabIndex = 0;
            // 
            // buttonSetAll
            // 
            this.buttonSetAll.Location = new System.Drawing.Point(3, 49);
            this.buttonSetAll.Name = "buttonSetAll";
            this.buttonSetAll.Size = new System.Drawing.Size(75, 23);
            this.buttonSetAll.TabIndex = 1;
            this.buttonSetAll.Text = "S&et All";
            this.toolTip.SetToolTip(this.buttonSetAll, "Click to check all boxes below");
            this.buttonSetAll.UseVisualStyleBackColor = true;
            this.buttonSetAll.Click += new System.EventHandler(this.buttonSetAll_Click);
            // 
            // buttonClearAll
            // 
            this.buttonClearAll.Location = new System.Drawing.Point(84, 49);
            this.buttonClearAll.Name = "buttonClearAll";
            this.buttonClearAll.Size = new System.Drawing.Size(75, 23);
            this.buttonClearAll.TabIndex = 2;
            this.buttonClearAll.Text = "&Clear All";
            this.toolTip.SetToolTip(this.buttonClearAll, "Click to uncheck all boxes below");
            this.buttonClearAll.UseVisualStyleBackColor = true;
            this.buttonClearAll.Click += new System.EventHandler(this.buttonClearAll_Click);
            // 
            // labelInstructions
            // 
            this.labelInstructions.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.labelInstructions.AutoSize = true;
            this.labelInstructions.Location = new System.Drawing.Point(366, 153);
            this.labelInstructions.Name = "labelInstructions";
            this.labelInstructions.Size = new System.Drawing.Size(95, 91);
            this.labelInstructions.TabIndex = 3;
            this.labelInstructions.Text = "Set these options based on common problem areas with respect to spelling errors i" +
    "n the language being checked.";
            // 
            // checkBoxCompareDoubleChars
            // 
            this.checkBoxCompareDoubleChars.AutoSize = true;
            this.checkBoxCompareDoubleChars.Checked = true;
            this.checkBoxCompareDoubleChars.CheckState = System.Windows.Forms.CheckState.Checked;
            this.tableLayoutPanelDistinctions.SetColumnSpan(this.checkBoxCompareDoubleChars, 2);
            this.checkBoxCompareDoubleChars.Location = new System.Drawing.Point(3, 3);
            this.checkBoxCompareDoubleChars.Name = "checkBoxCompareDoubleChars";
            this.checkBoxCompareDoubleChars.Size = new System.Drawing.Size(156, 17);
            this.checkBoxCompareDoubleChars.TabIndex = 4;
            this.checkBoxCompareDoubleChars.Text = "Compare &double characters";
            this.toolTip.SetToolTip(this.checkBoxCompareDoubleChars, "Check whether to look for two consecutive letters (i.e. geminates) when comparing" +
        " words");
            this.checkBoxCompareDoubleChars.UseVisualStyleBackColor = true;
            // 
            // checkBoxIgnoreCase
            // 
            this.checkBoxIgnoreCase.AutoSize = true;
            this.tableLayoutPanelDistinctions.SetColumnSpan(this.checkBoxIgnoreCase, 2);
            this.checkBoxIgnoreCase.Location = new System.Drawing.Point(3, 26);
            this.checkBoxIgnoreCase.Name = "checkBoxIgnoreCase";
            this.checkBoxIgnoreCase.Size = new System.Drawing.Size(95, 17);
            this.checkBoxIgnoreCase.TabIndex = 5;
            this.checkBoxIgnoreCase.Text = "Com&pare Case";
            this.toolTip.SetToolTip(this.checkBoxIgnoreCase, "Check whether to compare differences in case when comparing words");
            this.checkBoxIgnoreCase.UseVisualStyleBackColor = true;
            // 
            // buttonAddNew
            // 
            this.buttonAddNew.Location = new System.Drawing.Point(165, 49);
            this.buttonAddNew.Name = "buttonAddNew";
            this.buttonAddNew.Size = new System.Drawing.Size(75, 23);
            this.buttonAddNew.TabIndex = 6;
            this.buttonAddNew.Text = "&Add New";
            this.toolTip.SetToolTip(this.buttonAddNew, "Click to add a new box below");
            this.buttonAddNew.UseVisualStyleBackColor = true;
            this.buttonAddNew.Click += new System.EventHandler(this.buttonAddNew_Click);
            // 
            // flowLayoutPanel
            // 
            this.tableLayoutPanelDistinctions.SetColumnSpan(this.flowLayoutPanel, 3);
            this.flowLayoutPanel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.flowLayoutPanel.FlowDirection = System.Windows.Forms.FlowDirection.TopDown;
            this.flowLayoutPanel.Location = new System.Drawing.Point(3, 78);
            this.flowLayoutPanel.Name = "flowLayoutPanel";
            this.flowLayoutPanel.Size = new System.Drawing.Size(357, 241);
            this.flowLayoutPanel.TabIndex = 7;
            // 
            // fontDialogDisplay
            // 
            this.fontDialogDisplay.Font = new System.Drawing.Font("Arial Unicode MS", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            // 
            // tableLayoutPanelTabs
            // 
            this.tableLayoutPanelTabs.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.tableLayoutPanelTabs.ColumnCount = 2;
            this.tableLayoutPanelTabs.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tableLayoutPanelTabs.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tableLayoutPanelTabs.Controls.Add(this.buttonOK, 0, 1);
            this.tableLayoutPanelTabs.Controls.Add(this.tabControl, 0, 0);
            this.tableLayoutPanelTabs.Controls.Add(this.buttonCancel, 1, 1);
            this.tableLayoutPanelTabs.Location = new System.Drawing.Point(12, 12);
            this.tableLayoutPanelTabs.Name = "tableLayoutPanelTabs";
            this.tableLayoutPanelTabs.RowCount = 2;
            this.tableLayoutPanelTabs.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanelTabs.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanelTabs.Size = new System.Drawing.Size(484, 389);
            this.tableLayoutPanelTabs.TabIndex = 1;
            // 
            // buttonOK
            // 
            this.buttonOK.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.buttonOK.Location = new System.Drawing.Point(164, 363);
            this.buttonOK.Name = "buttonOK";
            this.buttonOK.Size = new System.Drawing.Size(75, 23);
            this.buttonOK.TabIndex = 0;
            this.buttonOK.Text = "OK";
            this.buttonOK.UseVisualStyleBackColor = true;
            this.buttonOK.Click += new System.EventHandler(this.buttonOK_Click);
            // 
            // buttonCancel
            // 
            this.buttonCancel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.buttonCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.buttonCancel.Location = new System.Drawing.Point(245, 363);
            this.buttonCancel.Name = "buttonCancel";
            this.buttonCancel.Size = new System.Drawing.Size(75, 23);
            this.buttonCancel.TabIndex = 1;
            this.buttonCancel.Text = "Cancel";
            this.buttonCancel.UseVisualStyleBackColor = true;
            this.buttonCancel.Click += new System.EventHandler(this.buttonCancel_Click);
            // 
            // saveFileDialogVssFile
            // 
            this.saveFileDialogVssFile.DefaultExt = "xml";
            this.saveFileDialogVssFile.FileName = "Script";
            this.saveFileDialogVssFile.Filter = "Vernacular Script System files (*.xml)|*.xml";
            // 
            // labelAddlPunct
            // 
            this.labelAddlPunct.Anchor = System.Windows.Forms.AnchorStyles.Right;
            this.labelAddlPunct.AutoSize = true;
            this.labelAddlPunct.Location = new System.Drawing.Point(11, 167);
            this.labelAddlPunct.Name = "labelAddlPunct";
            this.labelAddlPunct.Size = new System.Drawing.Size(136, 26);
            this.labelAddlPunct.TabIndex = 24;
            this.labelAddlPunct.Text = "Additional &punctuation and whitespace:";
            this.labelAddlPunct.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.toolTip.SetToolTip(this.labelAddlPunct, "Enter any additional punctuation or whitespace characters needed for this languag" +
        "e, separated by spaces (these are used for boundary condition checking)");
            // 
            // textBoxAdditionalPunctuation
            // 
            this.tableLayoutPanel.SetColumnSpan(this.textBoxAdditionalPunctuation, 2);
            this.textBoxAdditionalPunctuation.Dock = System.Windows.Forms.DockStyle.Fill;
            this.helpProvider.SetHelpString(this.textBoxAdditionalPunctuation, resources.GetString("textBoxAdditionalPunctuation.HelpString"));
            this.textBoxAdditionalPunctuation.Location = new System.Drawing.Point(153, 170);
            this.textBoxAdditionalPunctuation.Name = "textBoxAdditionalPunctuation";
            this.helpProvider.SetShowHelp(this.textBoxAdditionalPunctuation, true);
            this.textBoxAdditionalPunctuation.Size = new System.Drawing.Size(308, 20);
            this.textBoxAdditionalPunctuation.TabIndex = 25;
            this.toolTip.SetToolTip(this.textBoxAdditionalPunctuation, "Enter any additional punctuation or whitespace characters needed for this languag" +
        "e, separated by spaces (these are used for boundary condition checking)");
            // 
            // SFProjectForm
            // 
            this.AcceptButton = this.buttonOK;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.CancelButton = this.buttonCancel;
            this.ClientSize = new System.Drawing.Size(508, 413);
            this.Controls.Add(this.tableLayoutPanelTabs);
            this.HelpButton = true;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "SFProjectForm";
            this.Text = "Configure Project";
            this.tabControl.ResumeLayout(false);
            this.tabPageLanguage.ResumeLayout(false);
            this.tableLayoutPanel.ResumeLayout(false);
            this.tableLayoutPanel.PerformLayout();
            this.tabPageDistinctionRules.ResumeLayout(false);
            this.tableLayoutPanelDistinctions.ResumeLayout(false);
            this.tableLayoutPanelDistinctions.PerformLayout();
            this.tableLayoutPanelTabs.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.TabControl tabControl;
        private System.Windows.Forms.TabPage tabPageLanguage;
        private System.Windows.Forms.TabPage tabPageDistinctionRules;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanelDistinctions;
        private System.Windows.Forms.Button buttonSetAll;
        private System.Windows.Forms.Button buttonClearAll;
        private System.Windows.Forms.Label labelInstructions;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel;
        private System.Windows.Forms.Label labelProjectName;
        private System.Windows.Forms.TextBox textBoxProjectName;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.ComboBox comboBoxLocale;
        private System.Windows.Forms.Label labelScriptSystemParameter;
        private System.Windows.Forms.ComboBox comboBoxVernScriptType;
        private System.Windows.Forms.Button buttonFont;
        private System.Windows.Forms.FontDialog fontDialogDisplay;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanelTabs;
        private System.Windows.Forms.Button buttonOK;
        private System.Windows.Forms.Button buttonCancel;
        private System.Windows.Forms.ToolTip toolTip;
        private System.Windows.Forms.CheckBox checkBoxCompareDoubleChars;
        private System.Windows.Forms.CheckBox checkBoxIgnoreCase;
        private System.Windows.Forms.HelpProvider helpProvider;
        private System.Windows.Forms.SaveFileDialog saveFileDialogVssFile;
        private System.Windows.Forms.Label labelVernFont;
        private System.Windows.Forms.Button buttonTransliteratorFont;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Button buttonChooseTransliterator;
        private System.Windows.Forms.Label labelVFont;
        private System.Windows.Forms.Label labelTransliterationFont;
        private System.Windows.Forms.Label labelTransliterator;
        private System.Windows.Forms.Button buttonAddNew;
        private System.Windows.Forms.FlowLayoutPanel flowLayoutPanel;
		private System.Windows.Forms.Label labelAddlPunct;
		private System.Windows.Forms.TextBox textBoxAdditionalPunctuation;
	}
}