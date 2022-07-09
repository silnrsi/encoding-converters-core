using System;
using ECInterfaces;     // for Util

namespace SilEncConverters40.EcTranslators.BingTranslator
{
    partial class BingTranslatorAutoConfigDialog
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
            this.labelTransducerType = new System.Windows.Forms.Label();
            this.tableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
            this.groupBoxTransductionType = new System.Windows.Forms.GroupBox();
            this.radioButtonTranslate = new System.Windows.Forms.RadioButton();
            this.radioButtonDictionaryLookup = new System.Windows.Forms.RadioButton();
            this.radioButtonTransliterate = new System.Windows.Forms.RadioButton();
            this.radioButtonTranslateWithTransliteration = new System.Windows.Forms.RadioButton();
            this.labelSourceLanguage = new System.Windows.Forms.Label();
            this.labelTargetLanguage = new System.Windows.Forms.Label();
            this.comboBoxSourceLanguages = new System.Windows.Forms.ComboBox();
            this.comboBoxTargetLanguages = new System.Windows.Forms.ComboBox();
            this.labelTargetScript = new System.Windows.Forms.Label();
            this.comboBoxTargetScripts = new System.Windows.Forms.ComboBox();
            this.labelSourceScript = new System.Windows.Forms.Label();
            this.textBoxSourceScript = new System.Windows.Forms.TextBox();
            this.buttonSetBingTranslateApiKey = new System.Windows.Forms.Button();
            this.openFileDialogBrowse = new System.Windows.Forms.OpenFileDialog();
            this.tabControl.SuspendLayout();
            this.tabPageSetup.SuspendLayout();
            this.tableLayoutPanel1.SuspendLayout();
            this.groupBoxTransductionType.SuspendLayout();
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
            // labelTransducerType
            // 
            this.labelTransducerType.Anchor = System.Windows.Forms.AnchorStyles.Right;
            this.labelTransducerType.AutoSize = true;
            this.labelTransducerType.Location = new System.Drawing.Point(7, 35);
            this.labelTransducerType.Name = "labelTransducerType";
            this.labelTransducerType.Size = new System.Drawing.Size(87, 13);
            this.labelTransducerType.TabIndex = 0;
            this.labelTransducerType.Text = "Transducer type:";
            // 
            // tableLayoutPanel1
            // 
            this.tableLayoutPanel1.ColumnCount = 3;
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.tableLayoutPanel1.Controls.Add(this.labelTransducerType, 0, 0);
            this.tableLayoutPanel1.Controls.Add(this.groupBoxTransductionType, 1, 0);
            this.tableLayoutPanel1.Controls.Add(this.labelSourceLanguage, 0, 1);
            this.tableLayoutPanel1.Controls.Add(this.labelTargetLanguage, 0, 2);
            this.tableLayoutPanel1.Controls.Add(this.comboBoxSourceLanguages, 1, 1);
            this.tableLayoutPanel1.Controls.Add(this.comboBoxTargetLanguages, 1, 2);
            this.tableLayoutPanel1.Controls.Add(this.labelTargetScript, 0, 4);
            this.tableLayoutPanel1.Controls.Add(this.comboBoxTargetScripts, 1, 4);
            this.tableLayoutPanel1.Controls.Add(this.labelSourceScript, 0, 3);
            this.tableLayoutPanel1.Controls.Add(this.textBoxSourceScript, 1, 3);
            this.tableLayoutPanel1.Controls.Add(this.buttonSetBingTranslateApiKey, 1, 6);
            this.tableLayoutPanel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanel1.Location = new System.Drawing.Point(3, 3);
            this.tableLayoutPanel1.Name = "tableLayoutPanel1";
            this.tableLayoutPanel1.RowCount = 7;
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 50F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 50F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 50F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 50F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 20F));
            this.tableLayoutPanel1.Size = new System.Drawing.Size(596, 394);
            this.tableLayoutPanel1.TabIndex = 1;
            // 
            // groupBoxTransductionType
            // 
            this.groupBoxTransductionType.Controls.Add(this.radioButtonTranslate);
            this.groupBoxTransductionType.Controls.Add(this.radioButtonDictionaryLookup);
            this.groupBoxTransductionType.Controls.Add(this.radioButtonTransliterate);
            this.groupBoxTransductionType.Controls.Add(this.radioButtonTranslateWithTransliteration);
            this.groupBoxTransductionType.Dock = System.Windows.Forms.DockStyle.Fill;
            this.groupBoxTransductionType.Location = new System.Drawing.Point(100, 3);
            this.groupBoxTransductionType.Name = "groupBoxTransductionType";
            this.groupBoxTransductionType.Size = new System.Drawing.Size(493, 77);
            this.groupBoxTransductionType.TabIndex = 1;
            this.groupBoxTransductionType.TabStop = false;
            this.groupBoxTransductionType.Text = "Transduction Type";
            // 
            // radioButtonTranslate
            // 
            this.radioButtonTranslate.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.radioButtonTranslate.AutoSize = true;
            this.radioButtonTranslate.Location = new System.Drawing.Point(24, 30);
            this.radioButtonTranslate.Name = "radioButtonTranslate";
            this.radioButtonTranslate.Size = new System.Drawing.Size(69, 17);
            this.radioButtonTranslate.TabIndex = 2;
            this.radioButtonTranslate.Text = "Translate";
            this.radioButtonTranslate.UseVisualStyleBackColor = true;
            this.radioButtonTranslate.CheckedChanged += new System.EventHandler(this.radioButtonTranslate_CheckedChanged);
            // 
            // radioButtonDictionaryLookup
            // 
            this.radioButtonDictionaryLookup.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.radioButtonDictionaryLookup.AutoSize = true;
            this.radioButtonDictionaryLookup.Location = new System.Drawing.Point(333, 32);
            this.radioButtonDictionaryLookup.Name = "radioButtonDictionaryLookup";
            this.radioButtonDictionaryLookup.Size = new System.Drawing.Size(111, 17);
            this.radioButtonDictionaryLookup.TabIndex = 5;
            this.radioButtonDictionaryLookup.Text = "Dictionary Lookup";
            this.radioButtonDictionaryLookup.UseVisualStyleBackColor = true;
            this.radioButtonDictionaryLookup.CheckedChanged += new System.EventHandler(this.radioButtonDictionaryLookup_CheckedChanged);
            // 
            // radioButtonTransliterate
            // 
            this.radioButtonTransliterate.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.radioButtonTransliterate.AutoSize = true;
            this.radioButtonTransliterate.Location = new System.Drawing.Point(244, 32);
            this.radioButtonTransliterate.Name = "radioButtonTransliterate";
            this.radioButtonTransliterate.Size = new System.Drawing.Size(83, 17);
            this.radioButtonTransliterate.TabIndex = 4;
            this.radioButtonTransliterate.Text = "Transliterate";
            this.radioButtonTransliterate.UseVisualStyleBackColor = true;
            this.radioButtonTransliterate.CheckedChanged += new System.EventHandler(this.radioButtonTransliterate_CheckedChanged);
            // 
            // radioButtonTranslateWithTransliteration
            // 
            this.radioButtonTranslateWithTransliteration.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.radioButtonTranslateWithTransliteration.AutoSize = true;
            this.radioButtonTranslateWithTransliteration.Location = new System.Drawing.Point(99, 30);
            this.radioButtonTranslateWithTransliteration.Name = "radioButtonTranslateWithTransliteration";
            this.radioButtonTranslateWithTransliteration.Size = new System.Drawing.Size(139, 17);
            this.radioButtonTranslateWithTransliteration.TabIndex = 3;
            this.radioButtonTranslateWithTransliteration.Text = "Translate + Transliterate";
            this.radioButtonTranslateWithTransliteration.UseVisualStyleBackColor = true;
            this.radioButtonTranslateWithTransliteration.CheckedChanged += new System.EventHandler(this.radioButtonTranslateWithTransliteration_CheckedChanged);
            // 
            // labelSourceLanguage
            // 
            this.labelSourceLanguage.Anchor = System.Windows.Forms.AnchorStyles.Right;
            this.labelSourceLanguage.AutoSize = true;
            this.labelSourceLanguage.Location = new System.Drawing.Point(3, 101);
            this.labelSourceLanguage.Name = "labelSourceLanguage";
            this.labelSourceLanguage.Size = new System.Drawing.Size(91, 13);
            this.labelSourceLanguage.TabIndex = 6;
            this.labelSourceLanguage.Text = "Source language:";
            // 
            // labelTargetLanguage
            // 
            this.labelTargetLanguage.Anchor = System.Windows.Forms.AnchorStyles.Right;
            this.labelTargetLanguage.AutoSize = true;
            this.labelTargetLanguage.Location = new System.Drawing.Point(6, 151);
            this.labelTargetLanguage.Name = "labelTargetLanguage";
            this.labelTargetLanguage.Size = new System.Drawing.Size(88, 13);
            this.labelTargetLanguage.TabIndex = 8;
            this.labelTargetLanguage.Text = "Target language:";
            // 
            // comboBoxSourceLanguages
            // 
            this.comboBoxSourceLanguages.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right)));
            this.comboBoxSourceLanguages.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.comboBoxSourceLanguages.FormattingEnabled = true;
            this.comboBoxSourceLanguages.Location = new System.Drawing.Point(100, 97);
            this.comboBoxSourceLanguages.Name = "comboBoxSourceLanguages";
            this.comboBoxSourceLanguages.Size = new System.Drawing.Size(493, 21);
            this.comboBoxSourceLanguages.TabIndex = 7;
            this.comboBoxSourceLanguages.SelectedIndexChanged += new System.EventHandler(this.comboBoxSourceLanguages_SelectedIndexChanged);
            // 
            // comboBoxTargetLanguages
            // 
            this.comboBoxTargetLanguages.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right)));
            this.comboBoxTargetLanguages.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.comboBoxTargetLanguages.FormattingEnabled = true;
            this.comboBoxTargetLanguages.Location = new System.Drawing.Point(100, 147);
            this.comboBoxTargetLanguages.Name = "comboBoxTargetLanguages";
            this.comboBoxTargetLanguages.Size = new System.Drawing.Size(493, 21);
            this.comboBoxTargetLanguages.TabIndex = 9;
            this.comboBoxTargetLanguages.SelectedIndexChanged += new System.EventHandler(this.comboBoxTargetLanguages_SelectedIndexChanged);
            // 
            // labelTargetScript
            // 
            this.labelTargetScript.Anchor = System.Windows.Forms.AnchorStyles.Right;
            this.labelTargetScript.AutoSize = true;
            this.labelTargetScript.Location = new System.Drawing.Point(25, 251);
            this.labelTargetScript.Name = "labelTargetScript";
            this.labelTargetScript.Size = new System.Drawing.Size(69, 13);
            this.labelTargetScript.TabIndex = 12;
            this.labelTargetScript.Text = "Target script:";
            // 
            // comboBoxTargetScripts
            // 
            this.comboBoxTargetScripts.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right)));
            this.comboBoxTargetScripts.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.comboBoxTargetScripts.Enabled = false;
            this.comboBoxTargetScripts.FormattingEnabled = true;
            this.comboBoxTargetScripts.Location = new System.Drawing.Point(100, 247);
            this.comboBoxTargetScripts.Name = "comboBoxTargetScripts";
            this.comboBoxTargetScripts.Size = new System.Drawing.Size(493, 21);
            this.comboBoxTargetScripts.TabIndex = 13;
            this.comboBoxTargetScripts.SelectedIndexChanged += new System.EventHandler(this.comboBoxTargetScripts_SelectedIndexChanged);
            // 
            // labelSourceScript
            // 
            this.labelSourceScript.Anchor = System.Windows.Forms.AnchorStyles.Right;
            this.labelSourceScript.AutoSize = true;
            this.labelSourceScript.Location = new System.Drawing.Point(22, 201);
            this.labelSourceScript.Name = "labelSourceScript";
            this.labelSourceScript.Size = new System.Drawing.Size(72, 13);
            this.labelSourceScript.TabIndex = 10;
            this.labelSourceScript.Text = "Source script:";
            // 
            // textBoxSourceScript
            // 
            this.textBoxSourceScript.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right)));
            this.textBoxSourceScript.Location = new System.Drawing.Point(100, 198);
            this.textBoxSourceScript.Name = "textBoxSourceScript";
            this.textBoxSourceScript.ReadOnly = true;
            this.textBoxSourceScript.Size = new System.Drawing.Size(493, 20);
            this.textBoxSourceScript.TabIndex = 11;
            // 
            // buttonSetBingTranslateApiKey
            // 
            this.buttonSetBingTranslateApiKey.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.buttonSetBingTranslateApiKey.Location = new System.Drawing.Point(100, 368);
            this.buttonSetBingTranslateApiKey.Name = "buttonSetBingTranslateApiKey";
            this.buttonSetBingTranslateApiKey.Size = new System.Drawing.Size(214, 23);
            this.buttonSetBingTranslateApiKey.TabIndex = 14;
            this.buttonSetBingTranslateApiKey.Text = "Enter your own Azure Translate Api Key";
            this.buttonSetBingTranslateApiKey.UseVisualStyleBackColor = true;
            this.buttonSetBingTranslateApiKey.Click += new System.EventHandler(this.buttonSetBingTranslateApiKey_Click);
            // 
            // openFileDialogBrowse
            // 
            this.openFileDialogBrowse.DefaultExt = "tec";
            this.openFileDialogBrowse.Filter = "TECkit (compiled) files (*.tec)|*.tec|TECkit (compilable) files (*.map)|*.map";
            this.openFileDialogBrowse.Title = "Browse for TECkit map";
            // 
            // BingTranslatorAutoConfigDialog
            // 
            this.ClientSize = new System.Drawing.Size(634, 479);
            this.Name = "BingTranslatorAutoConfigDialog";
            this.Controls.SetChildIndex(this.tabControl, 0);
            this.Controls.SetChildIndex(this.buttonApply, 0);
            this.Controls.SetChildIndex(this.buttonCancel, 0);
            this.Controls.SetChildIndex(this.buttonOK, 0);
            this.Controls.SetChildIndex(this.buttonSaveInRepository, 0);
            this.tabControl.ResumeLayout(false);
            this.tabPageSetup.ResumeLayout(false);
            this.tableLayoutPanel1.ResumeLayout(false);
            this.tableLayoutPanel1.PerformLayout();
            this.groupBoxTransductionType.ResumeLayout(false);
            this.groupBoxTransductionType.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Label labelTransducerType;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel1;
        private System.Windows.Forms.OpenFileDialog openFileDialogBrowse;
		private System.Windows.Forms.GroupBox groupBoxTransductionType;
		private System.Windows.Forms.RadioButton radioButtonTranslate;
		private System.Windows.Forms.RadioButton radioButtonTranslateWithTransliteration;
		private System.Windows.Forms.RadioButton radioButtonDictionaryLookup;
		private System.Windows.Forms.RadioButton radioButtonTransliterate;
		private System.Windows.Forms.Label labelSourceLanguage;
		private System.Windows.Forms.Label labelTargetLanguage;
		private System.Windows.Forms.Label labelTargetScript;
		private System.Windows.Forms.ComboBox comboBoxSourceLanguages;
		private System.Windows.Forms.ComboBox comboBoxTargetLanguages;
		private System.Windows.Forms.ComboBox comboBoxTargetScripts;
		private System.Windows.Forms.Button buttonSetBingTranslateApiKey;
		private System.Windows.Forms.Label labelSourceScript;
		private System.Windows.Forms.TextBox textBoxSourceScript;
	}
}
