using System;
using ECInterfaces;     // for Util

namespace SilEncConverters40.EcTranslators.AzureOpenAI
{
    partial class AzureOpenAiAutoConfigDialog
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
            this.labelSystemPromptAddition = new System.Windows.Forms.Label();
            this.comboBoxSystemPromptAdditions = new System.Windows.Forms.ComboBox();
            this.buttonSetAzureOpenAiApiKey = new System.Windows.Forms.Button();
            this.textBoxSourceLanguage = new System.Windows.Forms.TextBox();
            this.textBoxTargetLanguage = new System.Windows.Forms.TextBox();
            this.labelSystemPrompt = new System.Windows.Forms.Label();
            this.textBoxCompleteSystemPrompt = new System.Windows.Forms.TextBox();
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
            this.tableLayoutPanel1.Controls.Add(this.labelSourceLanguage, 0, 0);
            this.tableLayoutPanel1.Controls.Add(this.labelTargetLanguage, 0, 1);
            this.tableLayoutPanel1.Controls.Add(this.labelSystemPromptAddition, 0, 2);
            this.tableLayoutPanel1.Controls.Add(this.comboBoxSystemPromptAdditions, 1, 2);
            this.tableLayoutPanel1.Controls.Add(this.buttonSetAzureOpenAiApiKey, 1, 4);
            this.tableLayoutPanel1.Controls.Add(this.textBoxSourceLanguage, 1, 0);
            this.tableLayoutPanel1.Controls.Add(this.textBoxTargetLanguage, 1, 1);
            this.tableLayoutPanel1.Controls.Add(this.labelSystemPrompt, 0, 3);
            this.tableLayoutPanel1.Controls.Add(this.textBoxCompleteSystemPrompt, 1, 3);
            this.tableLayoutPanel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanel1.Location = new System.Drawing.Point(3, 3);
            this.tableLayoutPanel1.Name = "tableLayoutPanel1";
            this.tableLayoutPanel1.RowCount = 5;
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 50F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 50F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 50F));
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
            this.labelSourceLanguage.Location = new System.Drawing.Point(47, 18);
            this.labelSourceLanguage.Name = "labelSourceLanguage";
            this.labelSourceLanguage.Size = new System.Drawing.Size(91, 13);
            this.labelSourceLanguage.TabIndex = 6;
            this.labelSourceLanguage.Text = "Source language:";
            // 
            // labelTargetLanguage
            // 
            this.labelTargetLanguage.Anchor = System.Windows.Forms.AnchorStyles.Right;
            this.labelTargetLanguage.AutoSize = true;
            this.labelTargetLanguage.Location = new System.Drawing.Point(50, 68);
            this.labelTargetLanguage.Name = "labelTargetLanguage";
            this.labelTargetLanguage.Size = new System.Drawing.Size(88, 13);
            this.labelTargetLanguage.TabIndex = 8;
            this.labelTargetLanguage.Text = "Target language:";
            // 
            // labelSystemPromptAddition
            // 
            this.labelSystemPromptAddition.Anchor = System.Windows.Forms.AnchorStyles.Right;
            this.labelSystemPromptAddition.AutoSize = true;
            this.labelSystemPromptAddition.Location = new System.Drawing.Point(3, 118);
            this.labelSystemPromptAddition.Name = "labelSystemPromptAddition";
            this.labelSystemPromptAddition.Size = new System.Drawing.Size(135, 13);
            this.labelSystemPromptAddition.TabIndex = 12;
            this.labelSystemPromptAddition.Text = "Additions to system prompt:";
            // 
            // comboBoxSystemPromptAdditions
            // 
            this.comboBoxSystemPromptAdditions.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right)));
            this.comboBoxSystemPromptAdditions.FormattingEnabled = true;
            this.comboBoxSystemPromptAdditions.Location = new System.Drawing.Point(144, 114);
            this.comboBoxSystemPromptAdditions.Name = "comboBoxSystemPromptAdditions";
            this.comboBoxSystemPromptAdditions.Size = new System.Drawing.Size(449, 21);
            this.comboBoxSystemPromptAdditions.TabIndex = 3;
            this.comboBoxSystemPromptAdditions.TextChanged += new System.EventHandler(this.comboBoxSystemPromptAdditions_TextChanged);
            // 
            // buttonSetAzureOpenAiApiKey
            // 
            this.buttonSetAzureOpenAiApiKey.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.buttonSetAzureOpenAiApiKey.Location = new System.Drawing.Point(144, 368);
            this.buttonSetAzureOpenAiApiKey.Name = "buttonSetAzureOpenAiApiKey";
            this.buttonSetAzureOpenAiApiKey.Size = new System.Drawing.Size(290, 23);
            this.buttonSetAzureOpenAiApiKey.TabIndex = 5;
            this.buttonSetAzureOpenAiApiKey.Text = "Enter your Azure Open AI Resource Key";
            this.buttonSetAzureOpenAiApiKey.UseVisualStyleBackColor = true;
            this.buttonSetAzureOpenAiApiKey.Click += new System.EventHandler(this.ButtonSetAzureOpenAiKey_Click);
            // 
            // textBoxSourceLanguage
            // 
            this.textBoxSourceLanguage.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right)));
            this.textBoxSourceLanguage.Location = new System.Drawing.Point(144, 15);
            this.textBoxSourceLanguage.Name = "textBoxSourceLanguage";
            this.textBoxSourceLanguage.Size = new System.Drawing.Size(449, 20);
            this.textBoxSourceLanguage.TabIndex = 1;
            this.textBoxSourceLanguage.TextChanged += new System.EventHandler(this.textBox_TextChanged);
            // 
            // textBoxTargetLanguage
            // 
            this.textBoxTargetLanguage.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right)));
            this.textBoxTargetLanguage.Location = new System.Drawing.Point(144, 65);
            this.textBoxTargetLanguage.Name = "textBoxTargetLanguage";
            this.textBoxTargetLanguage.Size = new System.Drawing.Size(449, 20);
            this.textBoxTargetLanguage.TabIndex = 2;
            this.textBoxTargetLanguage.TextChanged += new System.EventHandler(this.textBox_TextChanged);
            // 
            // labelSystemPrompt
            // 
            this.labelSystemPrompt.Anchor = System.Windows.Forms.AnchorStyles.Right;
            this.labelSystemPrompt.AutoSize = true;
            this.labelSystemPrompt.Location = new System.Drawing.Point(59, 251);
            this.labelSystemPrompt.Name = "labelSystemPrompt";
            this.labelSystemPrompt.Size = new System.Drawing.Size(79, 13);
            this.labelSystemPrompt.TabIndex = 12;
            this.labelSystemPrompt.Text = "System prompt:";
            // 
            // textBoxCompleteSystemPrompt
            // 
            this.textBoxCompleteSystemPrompt.Dock = System.Windows.Forms.DockStyle.Fill;
            this.textBoxCompleteSystemPrompt.Location = new System.Drawing.Point(144, 153);
            this.textBoxCompleteSystemPrompt.Multiline = true;
            this.textBoxCompleteSystemPrompt.Name = "textBoxCompleteSystemPrompt";
            this.textBoxCompleteSystemPrompt.ReadOnly = true;
            this.textBoxCompleteSystemPrompt.Size = new System.Drawing.Size(449, 209);
            this.textBoxCompleteSystemPrompt.TabIndex = 3;
            // 
            // AzureOpenAiAutoConfigDialog
            // 
            this.ClientSize = new System.Drawing.Size(634, 479);
            this.Name = "AzureOpenAiAutoConfigDialog";
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
        private System.Windows.Forms.Label labelSystemPromptAddition;
        private System.Windows.Forms.ComboBox comboBoxSystemPromptAdditions;
        private System.Windows.Forms.Button buttonSetAzureOpenAiApiKey;
        private System.Windows.Forms.TextBox textBoxSourceLanguage;
        private System.Windows.Forms.TextBox textBoxTargetLanguage;
        private System.Windows.Forms.Label labelSystemPrompt;
        private System.Windows.Forms.TextBox textBoxCompleteSystemPrompt;
    }
}
