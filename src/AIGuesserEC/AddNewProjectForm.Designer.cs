namespace SilEncConverters40
{
    partial class AddNewProjectForm
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(AddNewProjectForm));
            this.tableLayoutPanel = new System.Windows.Forms.TableLayoutPanel();
            this.labelSourceLanguage = new System.Windows.Forms.Label();
            this.textBoxSourceLanguage = new System.Windows.Forms.TextBox();
            this.labelTargetLanguage = new System.Windows.Forms.Label();
            this.textBoxTargetLanguage = new System.Windows.Forms.TextBox();
            this.textBoxDisplay = new System.Windows.Forms.TextBox();
            this.buttonOk = new System.Windows.Forms.Button();
            this.buttonCancel = new System.Windows.Forms.Button();
            this.tabControl = new System.Windows.Forms.TabControl();
            this.tabPageBasicSettings = new System.Windows.Forms.TabPage();
            this.tabPageAdvancedSettings = new System.Windows.Forms.TabPage();
            this.tableLayoutPanelAdvanced = new System.Windows.Forms.TableLayoutPanel();
            this.labelAdvSourceLanguage = new System.Windows.Forms.Label();
            this.labelFont = new System.Windows.Forms.Label();
            this.labelPunctuation = new System.Windows.Forms.Label();
            this.labelR2L = new System.Windows.Forms.Label();
            this.buttonChooseSourceLanguageFont = new System.Windows.Forms.Button();
            this.textBoxPunctuationSource = new System.Windows.Forms.TextBox();
            this.checkBoxR2LSource = new System.Windows.Forms.CheckBox();
            this.labelAdvTargetLanguage = new System.Windows.Forms.Label();
            this.buttonChooseTargetLanguageFont = new System.Windows.Forms.Button();
            this.textBoxPunctuationTarget = new System.Windows.Forms.TextBox();
            this.checkBoxR2LTarget = new System.Windows.Forms.CheckBox();
            this.toolTip = new System.Windows.Forms.ToolTip(this.components);
            this.labelDisplayFontNameSource = new System.Windows.Forms.Label();
            this.labelDisplayFontNameTarget = new System.Windows.Forms.Label();
            this.tableLayoutPanel.SuspendLayout();
            this.tabControl.SuspendLayout();
            this.tabPageBasicSettings.SuspendLayout();
            this.tabPageAdvancedSettings.SuspendLayout();
            this.tableLayoutPanelAdvanced.SuspendLayout();
            this.SuspendLayout();
            // 
            // tableLayoutPanel
            // 
            this.tableLayoutPanel.ColumnCount = 2;
            this.tableLayoutPanel.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.tableLayoutPanel.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 20F));
            this.tableLayoutPanel.Controls.Add(this.labelSourceLanguage, 0, 0);
            this.tableLayoutPanel.Controls.Add(this.textBoxSourceLanguage, 1, 0);
            this.tableLayoutPanel.Controls.Add(this.labelTargetLanguage, 0, 1);
            this.tableLayoutPanel.Controls.Add(this.textBoxTargetLanguage, 1, 1);
            this.tableLayoutPanel.Controls.Add(this.textBoxDisplay, 1, 2);
            this.tableLayoutPanel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanel.Location = new System.Drawing.Point(3, 3);
            this.tableLayoutPanel.Name = "tableLayoutPanel";
            this.tableLayoutPanel.RowCount = 3;
            this.tableLayoutPanel.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 20F));
            this.tableLayoutPanel.Size = new System.Drawing.Size(798, 252);
            this.tableLayoutPanel.TabIndex = 0;
            // 
            // labelSourceLanguage
            // 
            this.labelSourceLanguage.Anchor = System.Windows.Forms.AnchorStyles.Right;
            this.labelSourceLanguage.AutoSize = true;
            this.labelSourceLanguage.Location = new System.Drawing.Point(3, 6);
            this.labelSourceLanguage.Name = "labelSourceLanguage";
            this.labelSourceLanguage.Size = new System.Drawing.Size(95, 13);
            this.labelSourceLanguage.TabIndex = 0;
            this.labelSourceLanguage.Text = "Source Language:";
            // 
            // textBoxSourceLanguage
            // 
            this.textBoxSourceLanguage.Dock = System.Windows.Forms.DockStyle.Fill;
            this.textBoxSourceLanguage.Location = new System.Drawing.Point(104, 3);
            this.textBoxSourceLanguage.Name = "textBoxSourceLanguage";
            this.textBoxSourceLanguage.Size = new System.Drawing.Size(691, 20);
            this.textBoxSourceLanguage.TabIndex = 0;
            this.textBoxSourceLanguage.TextChanged += new System.EventHandler(this.TextBoxLanguageTextChanged);
            // 
            // labelTargetLanguage
            // 
            this.labelTargetLanguage.Anchor = System.Windows.Forms.AnchorStyles.Right;
            this.labelTargetLanguage.AutoSize = true;
            this.labelTargetLanguage.Location = new System.Drawing.Point(6, 32);
            this.labelTargetLanguage.Name = "labelTargetLanguage";
            this.labelTargetLanguage.Size = new System.Drawing.Size(92, 13);
            this.labelTargetLanguage.TabIndex = 0;
            this.labelTargetLanguage.Text = "Target Language:";
            // 
            // textBoxTargetLanguage
            // 
            this.textBoxTargetLanguage.Dock = System.Windows.Forms.DockStyle.Fill;
            this.textBoxTargetLanguage.Location = new System.Drawing.Point(104, 29);
            this.textBoxTargetLanguage.Name = "textBoxTargetLanguage";
            this.textBoxTargetLanguage.Size = new System.Drawing.Size(691, 20);
            this.textBoxTargetLanguage.TabIndex = 1;
            this.textBoxTargetLanguage.TextChanged += new System.EventHandler(this.TextBoxLanguageTextChanged);
            // 
            // textBoxDisplay
            // 
            this.textBoxDisplay.Dock = System.Windows.Forms.DockStyle.Fill;
            this.textBoxDisplay.Location = new System.Drawing.Point(104, 55);
            this.textBoxDisplay.Multiline = true;
            this.textBoxDisplay.Name = "textBoxDisplay";
            this.textBoxDisplay.Size = new System.Drawing.Size(691, 194);
            this.textBoxDisplay.TabIndex = 2;
            // 
            // buttonOk
            // 
            this.buttonOk.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.buttonOk.Enabled = false;
            this.buttonOk.Location = new System.Drawing.Point(340, 302);
            this.buttonOk.Name = "buttonOk";
            this.buttonOk.Size = new System.Drawing.Size(75, 23);
            this.buttonOk.TabIndex = 0;
            this.buttonOk.Text = "&OK";
            this.buttonOk.UseVisualStyleBackColor = true;
            this.buttonOk.Click += new System.EventHandler(this.ButtonOkClick);
            // 
            // buttonCancel
            // 
            this.buttonCancel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.buttonCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.buttonCancel.Location = new System.Drawing.Point(421, 302);
            this.buttonCancel.Name = "buttonCancel";
            this.buttonCancel.Size = new System.Drawing.Size(75, 23);
            this.buttonCancel.TabIndex = 1;
            this.buttonCancel.Text = "Cancel";
            this.buttonCancel.UseVisualStyleBackColor = true;
            // 
            // tabControl
            // 
            this.tabControl.Controls.Add(this.tabPageBasicSettings);
            this.tabControl.Controls.Add(this.tabPageAdvancedSettings);
            this.tabControl.Location = new System.Drawing.Point(12, 12);
            this.tabControl.Name = "tabControl";
            this.tabControl.SelectedIndex = 0;
            this.tabControl.Size = new System.Drawing.Size(812, 284);
            this.tabControl.TabIndex = 1;
            // 
            // tabPageBasicSettings
            // 
            this.tabPageBasicSettings.Controls.Add(this.tableLayoutPanel);
            this.tabPageBasicSettings.Location = new System.Drawing.Point(4, 22);
            this.tabPageBasicSettings.Name = "tabPageBasicSettings";
            this.tabPageBasicSettings.Padding = new System.Windows.Forms.Padding(3);
            this.tabPageBasicSettings.Size = new System.Drawing.Size(804, 258);
            this.tabPageBasicSettings.TabIndex = 0;
            this.tabPageBasicSettings.Text = "Settings";
            this.tabPageBasicSettings.UseVisualStyleBackColor = true;
            // 
            // tabPageAdvancedSettings
            // 
            this.tabPageAdvancedSettings.Controls.Add(this.tableLayoutPanelAdvanced);
            this.tabPageAdvancedSettings.Location = new System.Drawing.Point(4, 22);
            this.tabPageAdvancedSettings.Name = "tabPageAdvancedSettings";
            this.tabPageAdvancedSettings.Padding = new System.Windows.Forms.Padding(3);
            this.tabPageAdvancedSettings.Size = new System.Drawing.Size(804, 258);
            this.tabPageAdvancedSettings.TabIndex = 1;
            this.tabPageAdvancedSettings.Text = "Advanced Settings";
            this.tabPageAdvancedSettings.UseVisualStyleBackColor = true;
            // 
            // tableLayoutPanelAdvanced
            // 
            this.tableLayoutPanelAdvanced.ColumnCount = 4;
            this.tableLayoutPanelAdvanced.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.tableLayoutPanelAdvanced.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.tableLayoutPanelAdvanced.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanelAdvanced.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.tableLayoutPanelAdvanced.Controls.Add(this.labelAdvSourceLanguage, 0, 1);
            this.tableLayoutPanelAdvanced.Controls.Add(this.labelFont, 1, 0);
            this.tableLayoutPanelAdvanced.Controls.Add(this.labelPunctuation, 2, 0);
            this.tableLayoutPanelAdvanced.Controls.Add(this.labelR2L, 3, 0);
            this.tableLayoutPanelAdvanced.Controls.Add(this.buttonChooseSourceLanguageFont, 1, 1);
            this.tableLayoutPanelAdvanced.Controls.Add(this.textBoxPunctuationSource, 2, 1);
            this.tableLayoutPanelAdvanced.Controls.Add(this.checkBoxR2LSource, 3, 1);
            this.tableLayoutPanelAdvanced.Controls.Add(this.labelAdvTargetLanguage, 0, 3);
            this.tableLayoutPanelAdvanced.Controls.Add(this.buttonChooseTargetLanguageFont, 1, 3);
            this.tableLayoutPanelAdvanced.Controls.Add(this.textBoxPunctuationTarget, 2, 3);
            this.tableLayoutPanelAdvanced.Controls.Add(this.checkBoxR2LTarget, 3, 3);
            this.tableLayoutPanelAdvanced.Controls.Add(this.labelDisplayFontNameSource, 1, 2);
            this.tableLayoutPanelAdvanced.Controls.Add(this.labelDisplayFontNameTarget, 1, 4);
            this.tableLayoutPanelAdvanced.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanelAdvanced.Location = new System.Drawing.Point(3, 3);
            this.tableLayoutPanelAdvanced.Name = "tableLayoutPanelAdvanced";
            this.tableLayoutPanelAdvanced.RowCount = 5;
            this.tableLayoutPanelAdvanced.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanelAdvanced.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanelAdvanced.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 50F));
            this.tableLayoutPanelAdvanced.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanelAdvanced.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanelAdvanced.Size = new System.Drawing.Size(798, 252);
            this.tableLayoutPanelAdvanced.TabIndex = 0;
            // 
            // labelAdvSourceLanguage
            // 
            this.labelAdvSourceLanguage.Anchor = System.Windows.Forms.AnchorStyles.Right;
            this.labelAdvSourceLanguage.AutoSize = true;
            this.labelAdvSourceLanguage.Location = new System.Drawing.Point(3, 28);
            this.labelAdvSourceLanguage.Name = "labelAdvSourceLanguage";
            this.labelAdvSourceLanguage.Size = new System.Drawing.Size(95, 13);
            this.labelAdvSourceLanguage.TabIndex = 0;
            this.labelAdvSourceLanguage.Text = "Source Language:";
            // 
            // labelFont
            // 
            this.labelFont.Anchor = System.Windows.Forms.AnchorStyles.Bottom;
            this.labelFont.AutoSize = true;
            this.labelFont.Location = new System.Drawing.Point(154, 0);
            this.labelFont.Name = "labelFont";
            this.labelFont.Size = new System.Drawing.Size(28, 13);
            this.labelFont.TabIndex = 1;
            this.labelFont.Text = "Font";
            // 
            // labelPunctuation
            // 
            this.labelPunctuation.Anchor = System.Windows.Forms.AnchorStyles.Bottom;
            this.labelPunctuation.AutoSize = true;
            this.labelPunctuation.Location = new System.Drawing.Point(449, 0);
            this.labelPunctuation.Name = "labelPunctuation";
            this.labelPunctuation.Size = new System.Drawing.Size(64, 13);
            this.labelPunctuation.TabIndex = 1;
            this.labelPunctuation.Text = "Punctuation";
            // 
            // labelR2L
            // 
            this.labelR2L.Anchor = System.Windows.Forms.AnchorStyles.Bottom;
            this.labelR2L.AutoSize = true;
            this.labelR2L.Location = new System.Drawing.Point(730, 0);
            this.labelR2L.Name = "labelR2L";
            this.labelR2L.Size = new System.Drawing.Size(65, 13);
            this.labelR2L.TabIndex = 1;
            this.labelR2L.Text = "Right to Left";
            // 
            // buttonChooseSourceLanguageFont
            // 
            this.buttonChooseSourceLanguageFont.Location = new System.Drawing.Point(104, 16);
            this.buttonChooseSourceLanguageFont.Name = "buttonChooseSourceLanguageFont";
            this.buttonChooseSourceLanguageFont.Size = new System.Drawing.Size(129, 23);
            this.buttonChooseSourceLanguageFont.TabIndex = 0;
            this.buttonChooseSourceLanguageFont.Text = "Click to Choose";
            this.buttonChooseSourceLanguageFont.UseVisualStyleBackColor = true;
            this.buttonChooseSourceLanguageFont.Click += new System.EventHandler(this.ButtonChooseSourceLanguageFontClick);
            // 
            // textBoxPunctuationSource
            // 
            this.textBoxPunctuationSource.Dock = System.Windows.Forms.DockStyle.Fill;
            this.textBoxPunctuationSource.Font = new System.Drawing.Font("Microsoft Sans Serif", 20.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.textBoxPunctuationSource.Location = new System.Drawing.Point(239, 16);
            this.textBoxPunctuationSource.Name = "textBoxPunctuationSource";
            this.textBoxPunctuationSource.Size = new System.Drawing.Size(485, 38);
            this.textBoxPunctuationSource.TabIndex = 1;
            this.textBoxPunctuationSource.TextChanged += new System.EventHandler(this.TextBoxPunctuationSourceTextChanged);
            // 
            // checkBoxR2LSource
            // 
            this.checkBoxR2LSource.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.checkBoxR2LSource.AutoSize = true;
            this.checkBoxR2LSource.CheckAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.checkBoxR2LSource.Location = new System.Drawing.Point(730, 16);
            this.checkBoxR2LSource.Name = "checkBoxR2LSource";
            this.checkBoxR2LSource.Size = new System.Drawing.Size(65, 38);
            this.checkBoxR2LSource.TabIndex = 2;
            this.checkBoxR2LSource.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.checkBoxR2LSource.UseVisualStyleBackColor = true;
            this.checkBoxR2LSource.CheckedChanged += new System.EventHandler(this.CheckBoxR2LSourceCheckedChanged);
            // 
            // labelAdvTargetLanguage
            // 
            this.labelAdvTargetLanguage.Anchor = System.Windows.Forms.AnchorStyles.Right;
            this.labelAdvTargetLanguage.AutoSize = true;
            this.labelAdvTargetLanguage.Location = new System.Drawing.Point(6, 122);
            this.labelAdvTargetLanguage.Name = "labelAdvTargetLanguage";
            this.labelAdvTargetLanguage.Size = new System.Drawing.Size(92, 13);
            this.labelAdvTargetLanguage.TabIndex = 0;
            this.labelAdvTargetLanguage.Text = "Target Language:";
            // 
            // buttonChooseTargetLanguageFont
            // 
            this.buttonChooseTargetLanguageFont.Location = new System.Drawing.Point(104, 110);
            this.buttonChooseTargetLanguageFont.Name = "buttonChooseTargetLanguageFont";
            this.buttonChooseTargetLanguageFont.Size = new System.Drawing.Size(129, 23);
            this.buttonChooseTargetLanguageFont.TabIndex = 3;
            this.buttonChooseTargetLanguageFont.Text = "Click to Choose";
            this.buttonChooseTargetLanguageFont.UseVisualStyleBackColor = true;
            this.buttonChooseTargetLanguageFont.Click += new System.EventHandler(this.ButtonChooseTargetLanguageFontClick);
            // 
            // textBoxPunctuationTarget
            // 
            this.textBoxPunctuationTarget.Dock = System.Windows.Forms.DockStyle.Fill;
            this.textBoxPunctuationTarget.Font = new System.Drawing.Font("Microsoft Sans Serif", 20.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.textBoxPunctuationTarget.Location = new System.Drawing.Point(239, 110);
            this.textBoxPunctuationTarget.Name = "textBoxPunctuationTarget";
            this.textBoxPunctuationTarget.Size = new System.Drawing.Size(485, 38);
            this.textBoxPunctuationTarget.TabIndex = 4;
            this.textBoxPunctuationTarget.TextChanged += new System.EventHandler(this.TextBoxPunctuationTargetTextChanged);
            // 
            // checkBoxR2LTarget
            // 
            this.checkBoxR2LTarget.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.checkBoxR2LTarget.AutoSize = true;
            this.checkBoxR2LTarget.CheckAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.checkBoxR2LTarget.Location = new System.Drawing.Point(730, 110);
            this.checkBoxR2LTarget.Name = "checkBoxR2LTarget";
            this.checkBoxR2LTarget.Size = new System.Drawing.Size(65, 38);
            this.checkBoxR2LTarget.TabIndex = 5;
            this.checkBoxR2LTarget.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.checkBoxR2LTarget.UseVisualStyleBackColor = true;
            this.checkBoxR2LTarget.CheckedChanged += new System.EventHandler(this.CheckBoxR2LTargetCheckedChanged);
            // 
            // labelDisplayFontNameSource
            // 
            this.labelDisplayFontNameSource.AutoSize = true;
            this.labelDisplayFontNameSource.Location = new System.Drawing.Point(104, 57);
            this.labelDisplayFontNameSource.Name = "labelDisplayFontNameSource";
            this.labelDisplayFontNameSource.Size = new System.Drawing.Size(35, 13);
            this.labelDisplayFontNameSource.TabIndex = 6;
            this.labelDisplayFontNameSource.Text = "label1";
            // 
            // labelDisplayFontNameTarget
            // 
            this.labelDisplayFontNameTarget.AutoSize = true;
            this.labelDisplayFontNameTarget.Location = new System.Drawing.Point(104, 151);
            this.labelDisplayFontNameTarget.Name = "labelDisplayFontNameTarget";
            this.labelDisplayFontNameTarget.Size = new System.Drawing.Size(35, 13);
            this.labelDisplayFontNameTarget.TabIndex = 6;
            this.labelDisplayFontNameTarget.Text = "label1";
            // 
            // AddNewProjectForm
            // 
            this.AcceptButton = this.buttonOk;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.CancelButton = this.buttonCancel;
            this.ClientSize = new System.Drawing.Size(836, 337);
            this.Controls.Add(this.tabControl);
            this.Controls.Add(this.buttonCancel);
            this.Controls.Add(this.buttonOk);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "AddNewProjectForm";
            this.Text = "Add New Adapt It Project";
            this.tableLayoutPanel.ResumeLayout(false);
            this.tableLayoutPanel.PerformLayout();
            this.tabControl.ResumeLayout(false);
            this.tabPageBasicSettings.ResumeLayout(false);
            this.tabPageAdvancedSettings.ResumeLayout(false);
            this.tableLayoutPanelAdvanced.ResumeLayout(false);
            this.tableLayoutPanelAdvanced.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel;
        private System.Windows.Forms.Label labelSourceLanguage;
        private System.Windows.Forms.TextBox textBoxSourceLanguage;
        private System.Windows.Forms.Label labelTargetLanguage;
        private System.Windows.Forms.TextBox textBoxTargetLanguage;
        private System.Windows.Forms.TextBox textBoxDisplay;
        private System.Windows.Forms.Button buttonOk;
        private System.Windows.Forms.Button buttonCancel;
        private System.Windows.Forms.TabControl tabControl;
        private System.Windows.Forms.TabPage tabPageBasicSettings;
        private System.Windows.Forms.TabPage tabPageAdvancedSettings;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanelAdvanced;
        private System.Windows.Forms.Label labelAdvSourceLanguage;
        private System.Windows.Forms.Label labelFont;
        private System.Windows.Forms.Label labelPunctuation;
        private System.Windows.Forms.Label labelR2L;
        private System.Windows.Forms.Button buttonChooseSourceLanguageFont;
        private System.Windows.Forms.TextBox textBoxPunctuationSource;
        private System.Windows.Forms.CheckBox checkBoxR2LSource;
        private System.Windows.Forms.Label labelAdvTargetLanguage;
        private System.Windows.Forms.Button buttonChooseTargetLanguageFont;
        private System.Windows.Forms.TextBox textBoxPunctuationTarget;
        private System.Windows.Forms.CheckBox checkBoxR2LTarget;
        private System.Windows.Forms.ToolTip toolTip;
        private System.Windows.Forms.Label labelDisplayFontNameSource;
        private System.Windows.Forms.Label labelDisplayFontNameTarget;
    }
}