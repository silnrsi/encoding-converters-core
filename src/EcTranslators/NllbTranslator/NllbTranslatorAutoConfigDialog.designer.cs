using System;
using ECInterfaces;     // for Util

namespace SilEncConverters40.EcTranslators.NllbTranslator
{
	partial class NllbTranslatorAutoConfigDialog
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
            this.openFileDialogBrowse = new System.Windows.Forms.OpenFileDialog();
            this.tableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
            this.labelSourceLanguage = new System.Windows.Forms.Label();
            this.labelTargetLanguage = new System.Windows.Forms.Label();
            this.comboBoxSourceLanguages = new System.Windows.Forms.ComboBox();
            this.comboBoxTargetLanguages = new System.Windows.Forms.ComboBox();
            this.buttonSetNllbTranslateApiKey = new System.Windows.Forms.Button();
            this.tabControl.SuspendLayout();
            this.tabPageSetup.SuspendLayout();
            this.tableLayoutPanel1.SuspendLayout();
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
            // openFileDialogBrowse
            // 
            this.openFileDialogBrowse.DefaultExt = "tec";
            this.openFileDialogBrowse.Filter = "TECkit (compiled) files (*.tec)|*.tec|TECkit (compilable) files (*.map)|*.map";
            this.openFileDialogBrowse.Title = "Browse for TECkit map";
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
            this.tableLayoutPanel1.Controls.Add(this.buttonSetNllbTranslateApiKey, 1, 3);
            this.tableLayoutPanel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanel1.Location = new System.Drawing.Point(3, 3);
            this.tableLayoutPanel1.Name = "tableLayoutPanel1";
            this.tableLayoutPanel1.RowCount = 4;
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 50F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 50F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 20F));
            this.tableLayoutPanel1.Size = new System.Drawing.Size(596, 394);
            this.tableLayoutPanel1.TabIndex = 2;
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
            this.comboBoxSourceLanguages.SelectedIndexChanged += new System.EventHandler(this.ComboBoxSourceLanguages_SelectedIndexChanged_1);
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
            this.comboBoxTargetLanguages.SelectedIndexChanged += new System.EventHandler(this.ComboBoxTargetLanguages_SelectedIndexChanged_1);
            // 
            // buttonSetNllbTranslateApiKey
            // 
            this.buttonSetNllbTranslateApiKey.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.buttonSetNllbTranslateApiKey.Location = new System.Drawing.Point(100, 368);
            this.buttonSetNllbTranslateApiKey.Name = "buttonSetNllbTranslateApiKey";
            this.buttonSetNllbTranslateApiKey.Size = new System.Drawing.Size(294, 23);
            this.buttonSetNllbTranslateApiKey.TabIndex = 14;
            this.buttonSetNllbTranslateApiKey.Text = "Enter the Nllb Translator Key";
            this.buttonSetNllbTranslateApiKey.UseVisualStyleBackColor = true;
            this.buttonSetNllbTranslateApiKey.Visible = false;
            this.buttonSetNllbTranslateApiKey.Click += new System.EventHandler(this.ButtonSetNllbTranslateApiKey_Click);
            // 
            // NllbTranslatorAutoConfigDialog
            // 
            this.ClientSize = new System.Drawing.Size(634, 479);
            this.Name = "NllbTranslatorAutoConfigDialog";
            this.Controls.SetChildIndex(this.tabControl, 0);
            this.Controls.SetChildIndex(this.buttonApply, 0);
            this.Controls.SetChildIndex(this.buttonCancel, 0);
            this.Controls.SetChildIndex(this.buttonOK, 0);
            this.Controls.SetChildIndex(this.buttonSaveInRepository, 0);
            this.tabControl.ResumeLayout(false);
            this.tabPageSetup.ResumeLayout(false);
            this.tableLayoutPanel1.ResumeLayout(false);
            this.tableLayoutPanel1.PerformLayout();
            this.ResumeLayout(false);

		}

		#endregion
		private System.Windows.Forms.OpenFileDialog openFileDialogBrowse;
		private System.Windows.Forms.TableLayoutPanel tableLayoutPanel1;
		private System.Windows.Forms.Label labelSourceLanguage;
		private System.Windows.Forms.Label labelTargetLanguage;
		private System.Windows.Forms.ComboBox comboBoxSourceLanguages;
		private System.Windows.Forms.ComboBox comboBoxTargetLanguages;
		private System.Windows.Forms.Button buttonSetNllbTranslateApiKey;
	}
}
