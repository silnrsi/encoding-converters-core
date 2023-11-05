
namespace SilEncConverters40.EcTranslators.NllbTranslator
{
    partial class QueryForEndpointAndApiKey
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
            this.tableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
            this.buttonOK = new System.Windows.Forms.Button();
            this.buttonCancel = new System.Windows.Forms.Button();
            this.labelNllbApiKey = new System.Windows.Forms.Label();
            this.labelNllbEndpoint = new System.Windows.Forms.Label();
            this.textBoxNllbApiKey = new System.Windows.Forms.TextBox();
            this.textBoxNllbEndpoint = new System.Windows.Forms.TextBox();
            this.labelNllbInstructions = new System.Windows.Forms.Label();
            this.toolTip = new System.Windows.Forms.ToolTip(this.components);
            this.tableLayoutPanel1.SuspendLayout();
            this.SuspendLayout();
            // 
            // tableLayoutPanel1
            // 
            this.tableLayoutPanel1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.tableLayoutPanel1.ColumnCount = 3;
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel1.Controls.Add(this.buttonOK, 1, 3);
            this.tableLayoutPanel1.Controls.Add(this.buttonCancel, 2, 3);
            this.tableLayoutPanel1.Controls.Add(this.labelNllbApiKey, 0, 0);
            this.tableLayoutPanel1.Controls.Add(this.labelNllbEndpoint, 0, 1);
            this.tableLayoutPanel1.Controls.Add(this.textBoxNllbApiKey, 1, 0);
            this.tableLayoutPanel1.Controls.Add(this.textBoxNllbEndpoint, 1, 1);
            this.tableLayoutPanel1.Controls.Add(this.labelNllbInstructions, 0, 2);
            this.tableLayoutPanel1.Location = new System.Drawing.Point(12, 12);
            this.tableLayoutPanel1.Name = "tableLayoutPanel1";
            this.tableLayoutPanel1.RowCount = 4;
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 50F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 50F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel1.Size = new System.Drawing.Size(498, 184);
            this.tableLayoutPanel1.TabIndex = 0;
            // 
            // buttonOK
            // 
            this.buttonOK.Anchor = System.Windows.Forms.AnchorStyles.Right;
            this.buttonOK.Location = new System.Drawing.Point(134, 158);
            this.buttonOK.Name = "buttonOK";
            this.buttonOK.Size = new System.Drawing.Size(75, 23);
            this.buttonOK.TabIndex = 6;
            this.buttonOK.Text = "&OK";
            this.buttonOK.UseVisualStyleBackColor = true;
            this.buttonOK.Click += new System.EventHandler(this.buttonOK_Click);
            // 
            // buttonCancel
            // 
            this.buttonCancel.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.buttonCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.buttonCancel.Location = new System.Drawing.Point(215, 158);
            this.buttonCancel.Name = "buttonCancel";
            this.buttonCancel.Size = new System.Drawing.Size(75, 23);
            this.buttonCancel.TabIndex = 7;
            this.buttonCancel.Text = "Cancel";
            this.buttonCancel.UseVisualStyleBackColor = true;
            // 
            // labelNllbApiKey
            // 
            this.labelNllbApiKey.Anchor = System.Windows.Forms.AnchorStyles.Right;
            this.labelNllbApiKey.AutoSize = true;
            this.labelNllbApiKey.Location = new System.Drawing.Point(54, 18);
            this.labelNllbApiKey.Name = "labelNllbApiKey";
            this.labelNllbApiKey.Size = new System.Drawing.Size(74, 13);
            this.labelNllbApiKey.TabIndex = 1;
            this.labelNllbApiKey.Text = "Enter Api Key:";
            // 
            // labelNllbEndpoint
            // 
            this.labelNllbEndpoint.Anchor = System.Windows.Forms.AnchorStyles.Right;
            this.labelNllbEndpoint.AutoSize = true;
            this.labelNllbEndpoint.Location = new System.Drawing.Point(3, 68);
            this.labelNllbEndpoint.Name = "labelNllbEndpoint";
            this.labelNllbEndpoint.Size = new System.Drawing.Size(125, 13);
            this.labelNllbEndpoint.TabIndex = 3;
            this.labelNllbEndpoint.Text = "Enter NLLB Api Endpoint";
            // 
            // textBoxNllbApiKey
            // 
            this.textBoxNllbApiKey.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right)));
            this.tableLayoutPanel1.SetColumnSpan(this.textBoxNllbApiKey, 2);
            this.textBoxNllbApiKey.Location = new System.Drawing.Point(134, 15);
            this.textBoxNllbApiKey.Name = "textBoxNllbApiKey";
            this.textBoxNllbApiKey.Size = new System.Drawing.Size(361, 20);
            this.textBoxNllbApiKey.TabIndex = 2;
            this.toolTip.SetToolTip(this.textBoxNllbApiKey, "Enter the API key you put into the server.py file in the github repository (defau" +
        "lt: your_api_key_here)");
            // 
            // textBoxNllbEndpoint
            // 
            this.textBoxNllbEndpoint.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right)));
            this.tableLayoutPanel1.SetColumnSpan(this.textBoxNllbEndpoint, 2);
            this.textBoxNllbEndpoint.Location = new System.Drawing.Point(134, 65);
            this.textBoxNllbEndpoint.Name = "textBoxNllbEndpoint";
            this.textBoxNllbEndpoint.Size = new System.Drawing.Size(361, 20);
            this.textBoxNllbEndpoint.TabIndex = 4;
            this.toolTip.SetToolTip(this.textBoxNllbEndpoint, "Enter the Host portion of the Endpoint (default is: http://localhost:8000). You c" +
        "an leave this blank to revert to http://localhost:8000");
            // 
            // labelNllbInstructions
            // 
            this.labelNllbInstructions.AutoSize = true;
            this.tableLayoutPanel1.SetColumnSpan(this.labelNllbInstructions, 3);
            this.labelNllbInstructions.Dock = System.Windows.Forms.DockStyle.Fill;
            this.labelNllbInstructions.Location = new System.Drawing.Point(3, 100);
            this.labelNllbInstructions.Name = "labelNllbInstructions";
            this.labelNllbInstructions.Size = new System.Drawing.Size(492, 55);
            this.labelNllbInstructions.TabIndex = 8;
            this.labelNllbInstructions.Text = "See the instructions on the About tab for how to build your NLLB Docker Container" +
    " to support an API Key, modify the port number and serve it to the local area ne" +
    "twork you are on";
            this.labelNllbInstructions.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // QueryForEndpointAndApiKey
            // 
            this.AcceptButton = this.buttonOK;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.CancelButton = this.buttonCancel;
            this.ClientSize = new System.Drawing.Size(522, 208);
            this.Controls.Add(this.tableLayoutPanel1);
            this.Name = "QueryForEndpointAndApiKey";
            this.Text = "Enter Azure Translator Resource Key and Location";
            this.tableLayoutPanel1.ResumeLayout(false);
            this.tableLayoutPanel1.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel1;
        private System.Windows.Forms.Button buttonOK;
        private System.Windows.Forms.Button buttonCancel;
        private System.Windows.Forms.Label labelNllbApiKey;
        private System.Windows.Forms.Label labelNllbEndpoint;
        private System.Windows.Forms.TextBox textBoxNllbApiKey;
        private System.Windows.Forms.TextBox textBoxNllbEndpoint;
        private System.Windows.Forms.Label labelNllbInstructions;
        private System.Windows.Forms.ToolTip toolTip;
    }
}