
namespace SilEncConverters40.EcTranslators.AzureOpenAI
{
    partial class QueryForAzureKeyDeploymentNameAndEndpoint
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
            this.buttonCancel = new System.Windows.Forms.Button();
            this.labelAzureOpenAiKey = new System.Windows.Forms.Label();
            this.labelAzureOpenAiEndpoint = new System.Windows.Forms.Label();
            this.linkLabelInstructions = new System.Windows.Forms.LinkLabel();
            this.labelAzureOpeAiDeploymentName = new System.Windows.Forms.Label();
            this.textBoxAzureOpenAiKey = new System.Windows.Forms.TextBox();
            this.textBoxAzureOpenAiDeploymentName = new System.Windows.Forms.TextBox();
            this.textBoxAzureOpenAiEndpoint = new System.Windows.Forms.TextBox();
            this.buttonOK = new System.Windows.Forms.Button();
            this.tableLayoutPanel1.SuspendLayout();
            this.SuspendLayout();
            // 
            // tableLayoutPanel1
            // 
            this.tableLayoutPanel1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.tableLayoutPanel1.ColumnCount = 4;
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel1.Controls.Add(this.buttonCancel, 2, 4);
            this.tableLayoutPanel1.Controls.Add(this.labelAzureOpenAiKey, 0, 0);
            this.tableLayoutPanel1.Controls.Add(this.labelAzureOpenAiEndpoint, 0, 2);
            this.tableLayoutPanel1.Controls.Add(this.linkLabelInstructions, 0, 3);
            this.tableLayoutPanel1.Controls.Add(this.labelAzureOpeAiDeploymentName, 0, 1);
            this.tableLayoutPanel1.Controls.Add(this.textBoxAzureOpenAiKey, 1, 0);
            this.tableLayoutPanel1.Controls.Add(this.textBoxAzureOpenAiDeploymentName, 1, 1);
            this.tableLayoutPanel1.Controls.Add(this.textBoxAzureOpenAiEndpoint, 1, 2);
            this.tableLayoutPanel1.Controls.Add(this.buttonOK, 1, 4);
            this.tableLayoutPanel1.Location = new System.Drawing.Point(12, 12);
            this.tableLayoutPanel1.Name = "tableLayoutPanel1";
            this.tableLayoutPanel1.RowCount = 5;
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 50F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 50F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 50F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 20F));
            this.tableLayoutPanel1.Size = new System.Drawing.Size(577, 223);
            this.tableLayoutPanel1.TabIndex = 0;
            // 
            // buttonCancel
            // 
            this.buttonCancel.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.buttonCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.buttonCancel.Location = new System.Drawing.Point(241, 197);
            this.buttonCancel.Name = "buttonCancel";
            this.buttonCancel.Size = new System.Drawing.Size(75, 23);
            this.buttonCancel.TabIndex = 7;
            this.buttonCancel.Text = "Cancel";
            this.buttonCancel.UseVisualStyleBackColor = true;
            // 
            // labelAzureOpenAiKey
            // 
            this.labelAzureOpenAiKey.Anchor = System.Windows.Forms.AnchorStyles.Right;
            this.labelAzureOpenAiKey.AutoSize = true;
            this.labelAzureOpenAiKey.Location = new System.Drawing.Point(26, 18);
            this.labelAzureOpenAiKey.Name = "labelAzureOpenAiKey";
            this.labelAzureOpenAiKey.Size = new System.Drawing.Size(128, 13);
            this.labelAzureOpenAiKey.TabIndex = 1;
            this.labelAzureOpenAiKey.Text = "Enter Azure Open AI Key:";
            // 
            // labelAzureOpenAiEndpoint
            // 
            this.labelAzureOpenAiEndpoint.Anchor = System.Windows.Forms.AnchorStyles.Right;
            this.labelAzureOpenAiEndpoint.AutoSize = true;
            this.labelAzureOpenAiEndpoint.Location = new System.Drawing.Point(3, 118);
            this.labelAzureOpenAiEndpoint.Name = "labelAzureOpenAiEndpoint";
            this.labelAzureOpenAiEndpoint.Size = new System.Drawing.Size(151, 13);
            this.labelAzureOpenAiEndpoint.TabIndex = 3;
            this.labelAzureOpenAiEndpoint.Text = "Enter Language API Endpoint:";
            // 
            // linkLabelInstructions
            // 
            this.linkLabelInstructions.AutoSize = true;
            this.tableLayoutPanel1.SetColumnSpan(this.linkLabelInstructions, 4);
            this.linkLabelInstructions.Location = new System.Drawing.Point(3, 150);
            this.linkLabelInstructions.Name = "linkLabelInstructions";
            this.linkLabelInstructions.Size = new System.Drawing.Size(335, 13);
            this.linkLabelInstructions.TabIndex = 5;
            this.linkLabelInstructions.TabStop = true;
            this.linkLabelInstructions.Text = "see {0} for instructions on creating your own Azure Open AI Resource";
            this.linkLabelInstructions.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.linkLabelInstructions_LinkClicked);
            // 
            // labelAzureOpeAiDeploymentName
            // 
            this.labelAzureOpeAiDeploymentName.Anchor = System.Windows.Forms.AnchorStyles.Right;
            this.labelAzureOpeAiDeploymentName.AutoSize = true;
            this.labelAzureOpeAiDeploymentName.Location = new System.Drawing.Point(29, 68);
            this.labelAzureOpeAiDeploymentName.Name = "labelAzureOpeAiDeploymentName";
            this.labelAzureOpeAiDeploymentName.Size = new System.Drawing.Size(125, 13);
            this.labelAzureOpeAiDeploymentName.TabIndex = 1;
            this.labelAzureOpeAiDeploymentName.Text = "Enter Deployment Name:";
            // 
            // textBoxAzureOpenAiKey
            // 
            this.textBoxAzureOpenAiKey.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right)));
            this.tableLayoutPanel1.SetColumnSpan(this.textBoxAzureOpenAiKey, 3);
            this.textBoxAzureOpenAiKey.Location = new System.Drawing.Point(160, 15);
            this.textBoxAzureOpenAiKey.Name = "textBoxAzureOpenAiKey";
            this.textBoxAzureOpenAiKey.Size = new System.Drawing.Size(414, 20);
            this.textBoxAzureOpenAiKey.TabIndex = 2;
            // 
            // textBoxAzureOpenAiDeploymentName
            // 
            this.textBoxAzureOpenAiDeploymentName.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right)));
            this.tableLayoutPanel1.SetColumnSpan(this.textBoxAzureOpenAiDeploymentName, 3);
            this.textBoxAzureOpenAiDeploymentName.Location = new System.Drawing.Point(160, 65);
            this.textBoxAzureOpenAiDeploymentName.Name = "textBoxAzureOpenAiDeploymentName";
            this.textBoxAzureOpenAiDeploymentName.Size = new System.Drawing.Size(414, 20);
            this.textBoxAzureOpenAiDeploymentName.TabIndex = 2;
            // 
            // textBoxAzureOpenAiEndpoint
            // 
            this.textBoxAzureOpenAiEndpoint.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right)));
            this.tableLayoutPanel1.SetColumnSpan(this.textBoxAzureOpenAiEndpoint, 3);
            this.textBoxAzureOpenAiEndpoint.Location = new System.Drawing.Point(160, 115);
            this.textBoxAzureOpenAiEndpoint.Name = "textBoxAzureOpenAiEndpoint";
            this.textBoxAzureOpenAiEndpoint.Size = new System.Drawing.Size(414, 20);
            this.textBoxAzureOpenAiEndpoint.TabIndex = 4;
            // 
            // buttonOK
            // 
            this.buttonOK.Anchor = System.Windows.Forms.AnchorStyles.Right;
            this.buttonOK.Location = new System.Drawing.Point(160, 197);
            this.buttonOK.Name = "buttonOK";
            this.buttonOK.Size = new System.Drawing.Size(75, 23);
            this.buttonOK.TabIndex = 6;
            this.buttonOK.Text = "&OK";
            this.buttonOK.UseVisualStyleBackColor = true;
            this.buttonOK.Click += new System.EventHandler(this.buttonOK_Click);
            // 
            // QueryForAzureKeyDeploymentNameAndEndpoint
            // 
            this.AcceptButton = this.buttonOK;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.CancelButton = this.buttonCancel;
            this.ClientSize = new System.Drawing.Size(601, 247);
            this.Controls.Add(this.tableLayoutPanel1);
            this.Name = "QueryForAzureKeyDeploymentNameAndEndpoint";
            this.Text = "Enter Azure Open AI Resource Key, Deployment Name, and Endpoint";
            this.tableLayoutPanel1.ResumeLayout(false);
            this.tableLayoutPanel1.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel1;
        private System.Windows.Forms.Button buttonOK;
        private System.Windows.Forms.Button buttonCancel;
        private System.Windows.Forms.Label labelAzureOpenAiKey;
        private System.Windows.Forms.Label labelAzureOpenAiEndpoint;
        private System.Windows.Forms.TextBox textBoxAzureOpenAiKey;
        private System.Windows.Forms.TextBox textBoxAzureOpenAiEndpoint;
        private System.Windows.Forms.LinkLabel linkLabelInstructions;
        private System.Windows.Forms.Label labelAzureOpeAiDeploymentName;
        private System.Windows.Forms.TextBox textBoxAzureOpenAiDeploymentName;
    }
}