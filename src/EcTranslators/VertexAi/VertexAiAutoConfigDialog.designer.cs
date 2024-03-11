using System;
using ECInterfaces;     // for Util

namespace SilEncConverters40.EcTranslators.VertexAi
{
    partial class VertexAiAutoConfigDialog
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
            this.buttonSetVertexAiApiKey = new System.Windows.Forms.Button();
            this.textBoxSourceLanguage = new System.Windows.Forms.TextBox();
            this.textBoxTargetLanguage = new System.Windows.Forms.TextBox();
            this.labelSystemPrompt = new System.Windows.Forms.Label();
            this.textBoxCompleteSystemPrompt = new System.Windows.Forms.TextBox();
            this.labelProjectId = new System.Windows.Forms.Label();
            this.textBoxProjectId = new System.Windows.Forms.TextBox();
            this.labelLocationId = new System.Windows.Forms.Label();
            this.textBoxLocationId = new System.Windows.Forms.TextBox();
            this.labelPublisher = new System.Windows.Forms.Label();
            this.textBoxPublisher = new System.Windows.Forms.TextBox();
            this.labelModelId = new System.Windows.Forms.Label();
            this.textBoxModelId = new System.Windows.Forms.TextBox();
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
			this.tableLayoutPanel1.Controls.Add(this.labelProjectId, 0, 0);
			this.tableLayoutPanel1.Controls.Add(this.textBoxProjectId, 1, 0);
			this.tableLayoutPanel1.Controls.Add(this.labelLocationId, 0, 1);
            this.tableLayoutPanel1.Controls.Add(this.textBoxLocationId, 1, 1);
            this.tableLayoutPanel1.Controls.Add(this.labelPublisher, 0, 2);
            this.tableLayoutPanel1.Controls.Add(this.textBoxPublisher, 1, 2);
            this.tableLayoutPanel1.Controls.Add(this.labelModelId, 0, 3);
            this.tableLayoutPanel1.Controls.Add(this.textBoxModelId, 1, 3);
			this.tableLayoutPanel1.Controls.Add(this.labelSourceLanguage, 0, 4);
			this.tableLayoutPanel1.Controls.Add(this.textBoxSourceLanguage, 1, 4);
			this.tableLayoutPanel1.Controls.Add(this.labelTargetLanguage, 0, 5);
			this.tableLayoutPanel1.Controls.Add(this.textBoxTargetLanguage, 1, 5);
			this.tableLayoutPanel1.Controls.Add(this.labelSystemPromptAddition, 0, 6);
			this.tableLayoutPanel1.Controls.Add(this.comboBoxSystemPromptAdditions, 1, 6);
			this.tableLayoutPanel1.Controls.Add(this.labelSystemPrompt, 0, 7);
			this.tableLayoutPanel1.Controls.Add(this.textBoxCompleteSystemPrompt, 1, 7);
			this.tableLayoutPanel1.Controls.Add(this.buttonSetVertexAiApiKey, 1, 8);
			this.tableLayoutPanel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanel1.Location = new System.Drawing.Point(3, 3);
            this.tableLayoutPanel1.Name = "tableLayoutPanel1";
            this.tableLayoutPanel1.RowCount = 9;
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 40F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 40F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 40F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 40F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 40F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 40F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 40F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel1.Size = new System.Drawing.Size(596, 394);
            this.tableLayoutPanel1.TabIndex = 1;
            // 
            // labelSourceLanguage
            // 
            this.labelSourceLanguage.Anchor = System.Windows.Forms.AnchorStyles.Right;
            this.labelSourceLanguage.AutoSize = true;
            this.labelSourceLanguage.Location = new System.Drawing.Point(47, 173);
            this.labelSourceLanguage.Name = "labelSourceLanguage";
            this.labelSourceLanguage.Size = new System.Drawing.Size(91, 13);
            this.labelSourceLanguage.TabIndex = 6;
            this.labelSourceLanguage.Text = "Source language:";
            // 
            // labelTargetLanguage
            // 
            this.labelTargetLanguage.Anchor = System.Windows.Forms.AnchorStyles.Right;
            this.labelTargetLanguage.AutoSize = true;
            this.labelTargetLanguage.Location = new System.Drawing.Point(50, 213);
            this.labelTargetLanguage.Name = "labelTargetLanguage";
            this.labelTargetLanguage.Size = new System.Drawing.Size(88, 13);
            this.labelTargetLanguage.TabIndex = 8;
            this.labelTargetLanguage.Text = "Target language:";
            // 
            // labelSystemPromptAddition
            // 
            this.labelSystemPromptAddition.Anchor = System.Windows.Forms.AnchorStyles.Right;
            this.labelSystemPromptAddition.AutoSize = true;
            this.labelSystemPromptAddition.Location = new System.Drawing.Point(3, 253);
            this.labelSystemPromptAddition.Name = "labelSystemPromptAddition";
            this.labelSystemPromptAddition.Size = new System.Drawing.Size(135, 13);
            this.labelSystemPromptAddition.TabIndex = 12;
            this.labelSystemPromptAddition.Text = "Additions to system prompt:";
            // 
            // comboBoxSystemPromptAdditions
            // 
            this.comboBoxSystemPromptAdditions.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right)));
            this.comboBoxSystemPromptAdditions.FormattingEnabled = true;
            this.comboBoxSystemPromptAdditions.Location = new System.Drawing.Point(144, 249);
            this.comboBoxSystemPromptAdditions.Name = "comboBoxSystemPromptAdditions";
            this.comboBoxSystemPromptAdditions.Size = new System.Drawing.Size(449, 21);
            this.comboBoxSystemPromptAdditions.TabIndex = 3;
            this.comboBoxSystemPromptAdditions.TextChanged += new System.EventHandler(this.comboBoxSystemPromptAdditions_TextChanged);
            // 
            // buttonSetVertexAiApiKey
            // 
            this.buttonSetVertexAiApiKey.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.buttonSetVertexAiApiKey.Location = new System.Drawing.Point(144, 368);
            this.buttonSetVertexAiApiKey.Name = "buttonSetVertexAiApiKey";
            this.buttonSetVertexAiApiKey.Size = new System.Drawing.Size(326, 23);
            this.buttonSetVertexAiApiKey.TabIndex = 5;
            this.buttonSetVertexAiApiKey.Text = "Enter your GoogleCloud Vertex AI Credentials (json)";
            this.buttonSetVertexAiApiKey.UseVisualStyleBackColor = true;
            this.buttonSetVertexAiApiKey.Click += new System.EventHandler(this.ButtonSetVertexAiKey_Click);
            // 
            // textBoxSourceLanguage
            // 
            this.textBoxSourceLanguage.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right)));
            this.textBoxSourceLanguage.Location = new System.Drawing.Point(144, 170);
            this.textBoxSourceLanguage.Name = "textBoxSourceLanguage";
            this.textBoxSourceLanguage.Size = new System.Drawing.Size(449, 20);
            this.textBoxSourceLanguage.TabIndex = 1;
            this.textBoxSourceLanguage.TextChanged += new System.EventHandler(this.textBox_TextChanged);
            // 
            // textBoxTargetLanguage
            // 
            this.textBoxTargetLanguage.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right)));
            this.textBoxTargetLanguage.Location = new System.Drawing.Point(144, 210);
            this.textBoxTargetLanguage.Name = "textBoxTargetLanguage";
            this.textBoxTargetLanguage.Size = new System.Drawing.Size(449, 20);
            this.textBoxTargetLanguage.TabIndex = 2;
            this.textBoxTargetLanguage.TextChanged += new System.EventHandler(this.textBox_TextChanged);
            // 
            // labelSystemPrompt
            // 
            this.labelSystemPrompt.Anchor = System.Windows.Forms.AnchorStyles.Right;
            this.labelSystemPrompt.AutoSize = true;
            this.labelSystemPrompt.Location = new System.Drawing.Point(59, 316);
            this.labelSystemPrompt.Name = "labelSystemPrompt";
            this.labelSystemPrompt.Size = new System.Drawing.Size(79, 13);
            this.labelSystemPrompt.TabIndex = 12;
            this.labelSystemPrompt.Text = "System prompt:";
            // 
            // textBoxCompleteSystemPrompt
            // 
            this.textBoxCompleteSystemPrompt.Dock = System.Windows.Forms.DockStyle.Fill;
            this.textBoxCompleteSystemPrompt.Location = new System.Drawing.Point(144, 283);
            this.textBoxCompleteSystemPrompt.Multiline = true;
            this.textBoxCompleteSystemPrompt.Name = "textBoxCompleteSystemPrompt";
            this.textBoxCompleteSystemPrompt.ReadOnly = true;
            this.textBoxCompleteSystemPrompt.Size = new System.Drawing.Size(449, 79);
            this.textBoxCompleteSystemPrompt.TabIndex = 3;
            // 
            // labelProjectId
            // 
            this.labelProjectId.Anchor = System.Windows.Forms.AnchorStyles.Right;
            this.labelProjectId.AutoSize = true;
            this.labelProjectId.Location = new System.Drawing.Point(83, 13);
            this.labelProjectId.Name = "labelProjectId";
            this.labelProjectId.Size = new System.Drawing.Size(55, 13);
            this.labelProjectId.TabIndex = 6;
            this.labelProjectId.Text = "Project Id:";
            // 
            // textBoxProjectId
            // 
            this.textBoxProjectId.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right)));
            this.textBoxProjectId.Location = new System.Drawing.Point(144, 10);
            this.textBoxProjectId.Name = "textBoxProjectId";
            this.textBoxProjectId.Size = new System.Drawing.Size(449, 20);
            this.textBoxProjectId.TabIndex = 1;
            this.textBoxProjectId.TextChanged += new System.EventHandler(this.TextBoxVertexResourceParameters_TextChanged);
            // 
            // labelLocationId
            // 
            this.labelLocationId.Anchor = System.Windows.Forms.AnchorStyles.Right;
            this.labelLocationId.AutoSize = true;
            this.labelLocationId.Location = new System.Drawing.Point(75, 53);
            this.labelLocationId.Name = "labelLocationId";
            this.labelLocationId.Size = new System.Drawing.Size(63, 13);
            this.labelLocationId.TabIndex = 6;
            this.labelLocationId.Text = "Location Id:";
            // 
            // textBoxLocationId
            // 
            this.textBoxLocationId.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right)));
            this.textBoxLocationId.Location = new System.Drawing.Point(144, 50);
            this.textBoxLocationId.Name = "textBoxLocationId";
            this.textBoxLocationId.Size = new System.Drawing.Size(449, 20);
            this.textBoxLocationId.TabIndex = 1;
            this.textBoxLocationId.TextChanged += new System.EventHandler(this.TextBoxVertexResourceParameters_TextChanged);
            // 
            // labelPublisher
            // 
            this.labelPublisher.Anchor = System.Windows.Forms.AnchorStyles.Right;
            this.labelPublisher.AutoSize = true;
            this.labelPublisher.Location = new System.Drawing.Point(85, 93);
            this.labelPublisher.Name = "labelPublisher";
            this.labelPublisher.Size = new System.Drawing.Size(53, 13);
            this.labelPublisher.TabIndex = 6;
            this.labelPublisher.Text = "Publisher:";
            // 
            // textBoxPublisher
            // 
            this.textBoxPublisher.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right)));
            this.textBoxPublisher.Location = new System.Drawing.Point(144, 90);
            this.textBoxPublisher.Name = "textBoxPublisher";
            this.textBoxPublisher.Size = new System.Drawing.Size(449, 20);
            this.textBoxPublisher.TabIndex = 1;
            this.textBoxPublisher.TextChanged += new System.EventHandler(this.TextBoxVertexResourceParameters_TextChanged);
            // 
            // labelModelId
            // 
            this.labelModelId.Anchor = System.Windows.Forms.AnchorStyles.Right;
            this.labelModelId.AutoSize = true;
            this.labelModelId.Location = new System.Drawing.Point(87, 133);
            this.labelModelId.Name = "labelModelId";
            this.labelModelId.Size = new System.Drawing.Size(51, 13);
            this.labelModelId.TabIndex = 6;
            this.labelModelId.Text = "Model Id:";
            // 
            // textBoxModelId
            // 
            this.textBoxModelId.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right)));
            this.textBoxModelId.Location = new System.Drawing.Point(144, 130);
            this.textBoxModelId.Name = "textBoxModelId";
            this.textBoxModelId.Size = new System.Drawing.Size(449, 20);
            this.textBoxModelId.TabIndex = 1;
            this.textBoxModelId.TextChanged += new System.EventHandler(this.TextBoxVertexResourceParameters_TextChanged);
            // 
            // VertexAiAutoConfigDialog
            // 
            this.ClientSize = new System.Drawing.Size(634, 479);
            this.Name = "VertexAiAutoConfigDialog";
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
        private System.Windows.Forms.Button buttonSetVertexAiApiKey;
        private System.Windows.Forms.TextBox textBoxSourceLanguage;
        private System.Windows.Forms.TextBox textBoxTargetLanguage;
        private System.Windows.Forms.Label labelSystemPrompt;
        private System.Windows.Forms.TextBox textBoxCompleteSystemPrompt;
		private System.Windows.Forms.Label labelProjectId;
		private System.Windows.Forms.TextBox textBoxProjectId;
		private System.Windows.Forms.Label labelLocationId;
		private System.Windows.Forms.TextBox textBoxLocationId;
		private System.Windows.Forms.Label labelPublisher;
		private System.Windows.Forms.TextBox textBoxPublisher;
		private System.Windows.Forms.Label labelModelId;
		private System.Windows.Forms.TextBox textBoxModelId;
	}
}
