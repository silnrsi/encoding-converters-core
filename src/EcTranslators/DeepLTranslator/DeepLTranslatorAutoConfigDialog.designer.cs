using System;
using ECInterfaces;     // for Util

namespace SilEncConverters40.EcTranslators.DeepLTranslator
{
    partial class DeepLTranslatorAutoConfigDialog
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
            this.tableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
            this.labelSourceLanguage = new System.Windows.Forms.Label();
            this.labelTargetLanguage = new System.Windows.Forms.Label();
            this.comboBoxSourceLanguages = new System.Windows.Forms.ComboBox();
            this.comboBoxTargetLanguages = new System.Windows.Forms.ComboBox();
            this.buttonSetDeepLTranslateApiKey = new System.Windows.Forms.Button();
            this.labelFormalityLevel = new System.Windows.Forms.Label();
            this.groupBoxFormalityLevel = new System.Windows.Forms.GroupBox();
            this.radioButtonFormalityMore = new System.Windows.Forms.RadioButton();
            this.radioButtonFormalityLess = new System.Windows.Forms.RadioButton();
            this.labelJuiceLeft = new System.Windows.Forms.Label();
            this.openFileDialogBrowse = new System.Windows.Forms.OpenFileDialog();
            this.tabControl.SuspendLayout();
            this.tabPageSetup.SuspendLayout();
            this.tableLayoutPanel1.SuspendLayout();
            this.groupBoxFormalityLevel.SuspendLayout();
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
            this.tableLayoutPanel1.Controls.Add(this.labelSourceLanguage, 0, 0);
            this.tableLayoutPanel1.Controls.Add(this.labelTargetLanguage, 0, 1);
            this.tableLayoutPanel1.Controls.Add(this.comboBoxSourceLanguages, 1, 0);
            this.tableLayoutPanel1.Controls.Add(this.comboBoxTargetLanguages, 1, 1);
            this.tableLayoutPanel1.Controls.Add(this.buttonSetDeepLTranslateApiKey, 1, 4);
            this.tableLayoutPanel1.Controls.Add(this.labelFormalityLevel, 0, 2);
            this.tableLayoutPanel1.Controls.Add(this.groupBoxFormalityLevel, 1, 2);
            this.tableLayoutPanel1.Controls.Add(this.labelJuiceLeft, 1, 3);
            this.tableLayoutPanel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanel1.Location = new System.Drawing.Point(3, 3);
            this.tableLayoutPanel1.Name = "tableLayoutPanel1";
            this.tableLayoutPanel1.RowCount = 5;
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 50F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 50F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 20F));
            this.tableLayoutPanel1.Size = new System.Drawing.Size(596, 394);
            this.tableLayoutPanel1.TabIndex = 1;
            // 
            // labelSourceLanguage
            // 
            this.labelSourceLanguage.Anchor = System.Windows.Forms.AnchorStyles.Right;
            this.labelSourceLanguage.AutoSize = true;
            this.labelSourceLanguage.Location = new System.Drawing.Point(3, 18);
            this.labelSourceLanguage.Name = "labelSourceLanguage";
            this.labelSourceLanguage.Size = new System.Drawing.Size(91, 13);
            this.labelSourceLanguage.TabIndex = 6;
            this.labelSourceLanguage.Text = "Source language:";
            // 
            // labelTargetLanguage
            // 
            this.labelTargetLanguage.Anchor = System.Windows.Forms.AnchorStyles.Right;
            this.labelTargetLanguage.AutoSize = true;
            this.labelTargetLanguage.Location = new System.Drawing.Point(6, 68);
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
            this.comboBoxSourceLanguages.Location = new System.Drawing.Point(100, 14);
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
            this.comboBoxTargetLanguages.Location = new System.Drawing.Point(100, 64);
            this.comboBoxTargetLanguages.Name = "comboBoxTargetLanguages";
            this.comboBoxTargetLanguages.Size = new System.Drawing.Size(493, 21);
            this.comboBoxTargetLanguages.TabIndex = 9;
            this.comboBoxTargetLanguages.SelectedIndexChanged += new System.EventHandler(this.comboBoxTargetLanguages_SelectedIndexChanged);
            // 
            // buttonSetDeepLTranslateApiKey
            // 
            this.buttonSetDeepLTranslateApiKey.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.buttonSetDeepLTranslateApiKey.Location = new System.Drawing.Point(100, 368);
            this.buttonSetDeepLTranslateApiKey.Name = "buttonSetDeepLTranslateApiKey";
            this.buttonSetDeepLTranslateApiKey.Size = new System.Drawing.Size(294, 23);
            this.buttonSetDeepLTranslateApiKey.TabIndex = 14;
            this.buttonSetDeepLTranslateApiKey.Text = "Enter your own DeepL or DeepL Pro Api Key";
            this.buttonSetDeepLTranslateApiKey.UseVisualStyleBackColor = true;
            this.buttonSetDeepLTranslateApiKey.Click += new System.EventHandler(this.buttonSetDeepLTranslateApiKey_Click);
            // 
            // labelFormalityLevel
            // 
            this.labelFormalityLevel.Anchor = System.Windows.Forms.AnchorStyles.Right;
            this.labelFormalityLevel.AutoSize = true;
            this.labelFormalityLevel.Location = new System.Drawing.Point(18, 135);
            this.labelFormalityLevel.Name = "labelFormalityLevel";
            this.labelFormalityLevel.Size = new System.Drawing.Size(76, 13);
            this.labelFormalityLevel.TabIndex = 8;
            this.labelFormalityLevel.Text = "Formality level:";
            // 
            // groupBoxFormalityLevel
            // 
            this.groupBoxFormalityLevel.Controls.Add(this.radioButtonFormalityMore);
            this.groupBoxFormalityLevel.Controls.Add(this.radioButtonFormalityLess);
            this.groupBoxFormalityLevel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.groupBoxFormalityLevel.Location = new System.Drawing.Point(100, 103);
            this.groupBoxFormalityLevel.Name = "groupBoxFormalityLevel";
            this.groupBoxFormalityLevel.Size = new System.Drawing.Size(493, 77);
            this.groupBoxFormalityLevel.TabIndex = 15;
            this.groupBoxFormalityLevel.TabStop = false;
            this.groupBoxFormalityLevel.Text = "Formality Level";
            // 
            // radioButtonFormalityMore
            // 
            this.radioButtonFormalityMore.AutoSize = true;
            this.radioButtonFormalityMore.Enabled = false;
            this.radioButtonFormalityMore.Location = new System.Drawing.Point(245, 30);
            this.radioButtonFormalityMore.Name = "radioButtonFormalityMore";
            this.radioButtonFormalityMore.Size = new System.Drawing.Size(49, 17);
            this.radioButtonFormalityMore.TabIndex = 0;
            this.radioButtonFormalityMore.TabStop = true;
            this.radioButtonFormalityMore.Text = "&More";
            this.radioButtonFormalityMore.UseVisualStyleBackColor = true;
            this.radioButtonFormalityMore.CheckedChanged += new System.EventHandler(this.radioButtonFormality_CheckedChanged);
            // 
            // radioButtonFormalityLess
            // 
            this.radioButtonFormalityLess.AutoSize = true;
            this.radioButtonFormalityLess.Enabled = false;
            this.radioButtonFormalityLess.Location = new System.Drawing.Point(178, 30);
            this.radioButtonFormalityLess.Name = "radioButtonFormalityLess";
            this.radioButtonFormalityLess.Size = new System.Drawing.Size(47, 17);
            this.radioButtonFormalityLess.TabIndex = 0;
            this.radioButtonFormalityLess.TabStop = true;
            this.radioButtonFormalityLess.Text = "&Less";
            this.radioButtonFormalityLess.UseVisualStyleBackColor = true;
            this.radioButtonFormalityLess.CheckedChanged += new System.EventHandler(this.radioButtonFormality_CheckedChanged);
            // 
            // labelJuiceLeft
            // 
            this.labelJuiceLeft.AutoSize = true;
            this.labelJuiceLeft.Location = new System.Drawing.Point(100, 183);
            this.labelJuiceLeft.Name = "labelJuiceLeft";
            this.labelJuiceLeft.Size = new System.Drawing.Size(79, 13);
            this.labelJuiceLeft.TabIndex = 16;
            this.labelJuiceLeft.Text = "Translator stats";
            // 
            // openFileDialogBrowse
            // 
            this.openFileDialogBrowse.DefaultExt = "tec";
            this.openFileDialogBrowse.Filter = "TECkit (compiled) files (*.tec)|*.tec|TECkit (compilable) files (*.map)|*.map";
            this.openFileDialogBrowse.Title = "Browse for TECkit map";
            // 
            // DeepLTranslatorAutoConfigDialog
            // 
            this.ClientSize = new System.Drawing.Size(634, 479);
            this.Name = "DeepLTranslatorAutoConfigDialog";
            this.Controls.SetChildIndex(this.tabControl, 0);
            this.Controls.SetChildIndex(this.buttonApply, 0);
            this.Controls.SetChildIndex(this.buttonCancel, 0);
            this.Controls.SetChildIndex(this.buttonOK, 0);
            this.Controls.SetChildIndex(this.buttonSaveInRepository, 0);
            this.tabControl.ResumeLayout(false);
            this.tabPageSetup.ResumeLayout(false);
            this.tableLayoutPanel1.ResumeLayout(false);
            this.tableLayoutPanel1.PerformLayout();
            this.groupBoxFormalityLevel.ResumeLayout(false);
            this.groupBoxFormalityLevel.PerformLayout();
            this.ResumeLayout(false);

		}

		#endregion
		private System.Windows.Forms.TableLayoutPanel tableLayoutPanel1;
		private System.Windows.Forms.OpenFileDialog openFileDialogBrowse;
		private System.Windows.Forms.Label labelSourceLanguage;
		private System.Windows.Forms.Label labelTargetLanguage;
		private System.Windows.Forms.ComboBox comboBoxSourceLanguages;
		private System.Windows.Forms.ComboBox comboBoxTargetLanguages;
		private System.Windows.Forms.Button buttonSetDeepLTranslateApiKey;
		private System.Windows.Forms.Label labelFormalityLevel;
		private System.Windows.Forms.GroupBox groupBoxFormalityLevel;
		private System.Windows.Forms.RadioButton radioButtonFormalityLess;
		private System.Windows.Forms.RadioButton radioButtonFormalityMore;
		private System.Windows.Forms.Label labelJuiceLeft;
	}
}
