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
            this.tableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
            this.labelFolderPath = new System.Windows.Forms.Label();
            this.textBoxDockerProjectFolder = new System.Windows.Forms.TextBox();
            this.buttonBrowse = new System.Windows.Forms.Button();
            this.buttonConfigureNllbModel = new System.Windows.Forms.Button();
            this.labelSourceLanguage = new System.Windows.Forms.Label();
            this.comboBoxSourceLanguages = new System.Windows.Forms.ComboBox();
            this.labelTargetLanguage = new System.Windows.Forms.Label();
            this.comboBoxTargetLanguages = new System.Windows.Forms.ComboBox();
            this.labelModelConfiguration = new System.Windows.Forms.Label();
            this.folderBrowserDialog = new System.Windows.Forms.FolderBrowserDialog();
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
            // tableLayoutPanel1
            // 
            this.tableLayoutPanel1.ColumnCount = 3;
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.tableLayoutPanel1.Controls.Add(this.labelFolderPath, 0, 0);
            this.tableLayoutPanel1.Controls.Add(this.textBoxDockerProjectFolder, 1, 0);
            this.tableLayoutPanel1.Controls.Add(this.buttonBrowse, 2, 0);
            this.tableLayoutPanel1.Controls.Add(this.buttonConfigureNllbModel, 1, 1);
            this.tableLayoutPanel1.Controls.Add(this.labelSourceLanguage, 0, 2);
            this.tableLayoutPanel1.Controls.Add(this.comboBoxSourceLanguages, 1, 2);
            this.tableLayoutPanel1.Controls.Add(this.labelTargetLanguage, 0, 3);
            this.tableLayoutPanel1.Controls.Add(this.comboBoxTargetLanguages, 1, 3);
            this.tableLayoutPanel1.Controls.Add(this.labelModelConfiguration, 1, 4);
            this.tableLayoutPanel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanel1.Location = new System.Drawing.Point(3, 3);
            this.tableLayoutPanel1.Name = "tableLayoutPanel1";
            this.tableLayoutPanel1.RowCount = 5;
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 50F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 50F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 50F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 50F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel1.Size = new System.Drawing.Size(596, 394);
            this.tableLayoutPanel1.TabIndex = 2;
            // 
            // labelFolderPath
            // 
            this.labelFolderPath.Anchor = System.Windows.Forms.AnchorStyles.Right;
            this.labelFolderPath.AutoSize = true;
            this.labelFolderPath.Location = new System.Drawing.Point(3, 18);
            this.labelFolderPath.Name = "labelFolderPath";
            this.labelFolderPath.Size = new System.Drawing.Size(113, 13);
            this.labelFolderPath.TabIndex = 1;
            this.labelFolderPath.Text = "Docker Project Folder:";
            // 
            // textBoxDockerProjectFolder
            // 
            this.textBoxDockerProjectFolder.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right)));
            this.textBoxDockerProjectFolder.Location = new System.Drawing.Point(122, 15);
            this.textBoxDockerProjectFolder.Name = "textBoxDockerProjectFolder";
            this.textBoxDockerProjectFolder.Size = new System.Drawing.Size(439, 20);
            this.textBoxDockerProjectFolder.TabIndex = 2;
            // 
            // buttonBrowse
            // 
            this.buttonBrowse.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right)));
            this.buttonBrowse.Location = new System.Drawing.Point(567, 13);
            this.buttonBrowse.Name = "buttonBrowse";
            this.buttonBrowse.Size = new System.Drawing.Size(26, 23);
            this.buttonBrowse.TabIndex = 3;
            this.buttonBrowse.Text = "...";
            this.buttonBrowse.Click += new System.EventHandler(this.buttonBrowse_Click);
            // 
            // buttonConfigureNllbModel
            // 
            this.buttonConfigureNllbModel.Location = new System.Drawing.Point(122, 53);
            this.buttonConfigureNllbModel.Name = "buttonConfigureNllbModel";
            this.buttonConfigureNllbModel.Size = new System.Drawing.Size(224, 23);
            this.buttonConfigureNllbModel.TabIndex = 3;
            this.buttonConfigureNllbModel.Text = "Configure NLLB Model";
            this.buttonConfigureNllbModel.UseVisualStyleBackColor = true;
            this.buttonConfigureNllbModel.Click += new System.EventHandler(this.ButtonSetNllbTranslateApiKey_Click);
            // 
            // labelSourceLanguage
            // 
            this.labelSourceLanguage.Anchor = System.Windows.Forms.AnchorStyles.Right;
            this.labelSourceLanguage.AutoSize = true;
            this.labelSourceLanguage.Location = new System.Drawing.Point(25, 118);
            this.labelSourceLanguage.Name = "labelSourceLanguage";
            this.labelSourceLanguage.Size = new System.Drawing.Size(91, 13);
            this.labelSourceLanguage.TabIndex = 4;
            this.labelSourceLanguage.Text = "Source language:";
            // 
            // comboBoxSourceLanguages
            // 
            this.comboBoxSourceLanguages.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right)));
            this.comboBoxSourceLanguages.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.comboBoxSourceLanguages.FormattingEnabled = true;
            this.comboBoxSourceLanguages.Location = new System.Drawing.Point(122, 114);
            this.comboBoxSourceLanguages.Name = "comboBoxSourceLanguages";
            this.comboBoxSourceLanguages.Size = new System.Drawing.Size(439, 21);
            this.comboBoxSourceLanguages.TabIndex = 5;
            this.comboBoxSourceLanguages.SelectedIndexChanged += new System.EventHandler(this.ComboBoxSourceLanguages_SelectedIndexChanged_1);
            // 
            // labelTargetLanguage
            // 
            this.labelTargetLanguage.Anchor = System.Windows.Forms.AnchorStyles.Right;
            this.labelTargetLanguage.AutoSize = true;
            this.labelTargetLanguage.Location = new System.Drawing.Point(28, 168);
            this.labelTargetLanguage.Name = "labelTargetLanguage";
            this.labelTargetLanguage.Size = new System.Drawing.Size(88, 13);
            this.labelTargetLanguage.TabIndex = 6;
            this.labelTargetLanguage.Text = "Target language:";
            // 
            // comboBoxTargetLanguages
            // 
            this.comboBoxTargetLanguages.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right)));
            this.comboBoxTargetLanguages.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.comboBoxTargetLanguages.FormattingEnabled = true;
            this.comboBoxTargetLanguages.Location = new System.Drawing.Point(122, 164);
            this.comboBoxTargetLanguages.Name = "comboBoxTargetLanguages";
            this.comboBoxTargetLanguages.Size = new System.Drawing.Size(439, 21);
            this.comboBoxTargetLanguages.TabIndex = 7;
            this.comboBoxTargetLanguages.SelectedIndexChanged += new System.EventHandler(this.ComboBoxTargetLanguages_SelectedIndexChanged_1);
            // 
            // labelModelConfiguration
            // 
            this.labelModelConfiguration.AutoSize = true;
            this.labelModelConfiguration.Dock = System.Windows.Forms.DockStyle.Fill;
            this.labelModelConfiguration.Location = new System.Drawing.Point(122, 200);
            this.labelModelConfiguration.Name = "labelModelConfiguration";
            this.labelModelConfiguration.Size = new System.Drawing.Size(439, 194);
            this.labelModelConfiguration.TabIndex = 8;
            // 
            // folderBrowserDialog
            // 
            this.folderBrowserDialog.Description = "Browse and create a folder into which we will write the Docker Project files";
            this.folderBrowserDialog.RootFolder = System.Environment.SpecialFolder.MyComputer;
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
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel1;
        private System.Windows.Forms.Label labelSourceLanguage;
        private System.Windows.Forms.Label labelTargetLanguage;
        private System.Windows.Forms.ComboBox comboBoxSourceLanguages;
        private System.Windows.Forms.ComboBox comboBoxTargetLanguages;
        private System.Windows.Forms.Button buttonConfigureNllbModel;
        private System.Windows.Forms.Label labelModelConfiguration;
        private System.Windows.Forms.Label labelFolderPath;
        private System.Windows.Forms.TextBox textBoxDockerProjectFolder;
        private System.Windows.Forms.Button buttonBrowse;
        private System.Windows.Forms.FolderBrowserDialog folderBrowserDialog;
    }
}
