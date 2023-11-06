
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(QueryForEndpointAndApiKey));
            this.tableLayoutPanel = new System.Windows.Forms.TableLayoutPanel();
            this.labelNllbModel = new System.Windows.Forms.Label();
            this.comboBoxNllbModel = new System.Windows.Forms.ComboBox();
            this.labelNllbApiKey = new System.Windows.Forms.Label();
            this.textBoxNllbApiKey = new System.Windows.Forms.TextBox();
            this.labelNllbEndpoint = new System.Windows.Forms.Label();
            this.textBoxNllbEndpoint = new System.Windows.Forms.TextBox();
            this.labelNllbInstructions = new System.Windows.Forms.Label();
            this.buttonOK = new System.Windows.Forms.Button();
            this.buttonCancel = new System.Windows.Forms.Button();
            this.toolTip = new System.Windows.Forms.ToolTip(this.components);
            this.tableLayoutPanel.SuspendLayout();
            this.SuspendLayout();
            // 
            // tableLayoutPanel
            // 
            this.tableLayoutPanel.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.tableLayoutPanel.ColumnCount = 3;
            this.tableLayoutPanel.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.tableLayoutPanel.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 40F));
            this.tableLayoutPanel.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 60F));
            this.tableLayoutPanel.Controls.Add(this.labelNllbModel, 0, 0);
            this.tableLayoutPanel.Controls.Add(this.comboBoxNllbModel, 1, 0);
            this.tableLayoutPanel.Controls.Add(this.labelNllbApiKey, 0, 1);
            this.tableLayoutPanel.Controls.Add(this.textBoxNllbApiKey, 1, 1);
            this.tableLayoutPanel.Controls.Add(this.labelNllbEndpoint, 0, 2);
            this.tableLayoutPanel.Controls.Add(this.textBoxNllbEndpoint, 1, 2);
            this.tableLayoutPanel.Controls.Add(this.labelNllbInstructions, 0, 3);
            this.tableLayoutPanel.Controls.Add(this.buttonOK, 1, 4);
            this.tableLayoutPanel.Controls.Add(this.buttonCancel, 2, 4);
            this.tableLayoutPanel.Location = new System.Drawing.Point(12, 12);
            this.tableLayoutPanel.Name = "tableLayoutPanel";
            this.tableLayoutPanel.RowCount = 5;
            this.tableLayoutPanel.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 50F));
            this.tableLayoutPanel.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 50F));
            this.tableLayoutPanel.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 50F));
            this.tableLayoutPanel.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel.Size = new System.Drawing.Size(611, 304);
            this.tableLayoutPanel.TabIndex = 0;
            // 
            // labelNllbModel
            // 
            this.labelNllbModel.Anchor = System.Windows.Forms.AnchorStyles.Right;
            this.labelNllbModel.AutoSize = true;
            this.labelNllbModel.Location = new System.Drawing.Point(4, 18);
            this.labelNllbModel.Name = "labelNllbModel";
            this.labelNllbModel.Size = new System.Drawing.Size(69, 13);
            this.labelNllbModel.TabIndex = 1;
            this.labelNllbModel.Text = "NLLB Model:";
            // 
            // comboBoxNllbModel
            // 
            this.comboBoxNllbModel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right)));
            this.tableLayoutPanel.SetColumnSpan(this.comboBoxNllbModel, 2);
            this.comboBoxNllbModel.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.comboBoxNllbModel.FormattingEnabled = true;
            this.comboBoxNllbModel.Items.AddRange(new object[] {
            "facebook/nllb-200-distilled-600M",
            "facebook/nllb-200-distilled-1.3B",
            "facebook/nllb-200-3.3B"});
            this.comboBoxNllbModel.Location = new System.Drawing.Point(79, 14);
            this.comboBoxNllbModel.Name = "comboBoxNllbModel";
            this.comboBoxNllbModel.Size = new System.Drawing.Size(529, 21);
            this.comboBoxNllbModel.TabIndex = 2;
            this.toolTip.SetToolTip(this.comboBoxNllbModel, resources.GetString("comboBoxNllbModel.ToolTip"));
            // 
            // labelNllbApiKey
            // 
            this.labelNllbApiKey.Anchor = System.Windows.Forms.AnchorStyles.Right;
            this.labelNllbApiKey.AutoSize = true;
            this.labelNllbApiKey.Location = new System.Drawing.Point(27, 68);
            this.labelNllbApiKey.Name = "labelNllbApiKey";
            this.labelNllbApiKey.Size = new System.Drawing.Size(46, 13);
            this.labelNllbApiKey.TabIndex = 3;
            this.labelNllbApiKey.Text = "Api Key:";
            // 
            // textBoxNllbApiKey
            // 
            this.textBoxNllbApiKey.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right)));
            this.tableLayoutPanel.SetColumnSpan(this.textBoxNllbApiKey, 2);
            this.textBoxNllbApiKey.Location = new System.Drawing.Point(79, 65);
            this.textBoxNllbApiKey.Name = "textBoxNllbApiKey";
            this.textBoxNllbApiKey.Size = new System.Drawing.Size(529, 20);
            this.textBoxNllbApiKey.TabIndex = 4;
            this.toolTip.SetToolTip(this.textBoxNllbApiKey, "Enter the API key to use with your model (Stored in the settings.py file, default" +
        ": your_api_key_here)");
            // 
            // labelNllbEndpoint
            // 
            this.labelNllbEndpoint.Anchor = System.Windows.Forms.AnchorStyles.Right;
            this.labelNllbEndpoint.AutoSize = true;
            this.labelNllbEndpoint.Location = new System.Drawing.Point(3, 118);
            this.labelNllbEndpoint.Name = "labelNllbEndpoint";
            this.labelNllbEndpoint.Size = new System.Drawing.Size(70, 13);
            this.labelNllbEndpoint.TabIndex = 5;
            this.labelNllbEndpoint.Text = "Api Endpoint:";
            // 
            // textBoxNllbEndpoint
            // 
            this.textBoxNllbEndpoint.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right)));
            this.tableLayoutPanel.SetColumnSpan(this.textBoxNllbEndpoint, 2);
            this.textBoxNllbEndpoint.Location = new System.Drawing.Point(79, 115);
            this.textBoxNllbEndpoint.Name = "textBoxNllbEndpoint";
            this.textBoxNllbEndpoint.Size = new System.Drawing.Size(529, 20);
            this.textBoxNllbEndpoint.TabIndex = 6;
            this.toolTip.SetToolTip(this.textBoxNllbEndpoint, "Enter the Host portion of the Endpoint (default is: http://localhost:8000). You c" +
        "an leave this blank to revert to http://localhost:8000. (Stored in the settings." +
        "py file)\r\n");
            // 
            // labelNllbInstructions
            // 
            this.labelNllbInstructions.AutoSize = true;
            this.tableLayoutPanel.SetColumnSpan(this.labelNllbInstructions, 3);
            this.labelNllbInstructions.Dock = System.Windows.Forms.DockStyle.Fill;
            this.labelNllbInstructions.Location = new System.Drawing.Point(3, 150);
            this.labelNllbInstructions.Name = "labelNllbInstructions";
            this.labelNllbInstructions.Size = new System.Drawing.Size(605, 125);
            this.labelNllbInstructions.TabIndex = 9;
            this.labelNllbInstructions.Text = "See the instructions on the About tab for how to build your NLLB Docker Container" +
    " to support an API Key, modify the port number and serve it to the local area ne" +
    "twork you are on.\r\n";
            this.labelNllbInstructions.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // buttonOK
            // 
            this.buttonOK.Anchor = System.Windows.Forms.AnchorStyles.Right;
            this.buttonOK.Location = new System.Drawing.Point(132, 278);
            this.buttonOK.Name = "buttonOK";
            this.buttonOK.Size = new System.Drawing.Size(155, 23);
            this.buttonOK.TabIndex = 10;
            this.buttonOK.Text = "&Create Docker Project";
            this.buttonOK.UseVisualStyleBackColor = true;
            this.buttonOK.Click += new System.EventHandler(this.buttonOK_Click);
            // 
            // buttonCancel
            // 
            this.buttonCancel.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.buttonCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.buttonCancel.Location = new System.Drawing.Point(293, 278);
            this.buttonCancel.Name = "buttonCancel";
            this.buttonCancel.Size = new System.Drawing.Size(75, 23);
            this.buttonCancel.TabIndex = 11;
            this.buttonCancel.Text = "Cancel";
            this.buttonCancel.UseVisualStyleBackColor = true;
            // 
            // QueryForEndpointAndApiKey
            // 
            this.AcceptButton = this.buttonOK;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.CancelButton = this.buttonCancel;
            this.ClientSize = new System.Drawing.Size(635, 328);
            this.Controls.Add(this.tableLayoutPanel);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "QueryForEndpointAndApiKey";
            this.Text = "NLLB Model Configuration";
            this.tableLayoutPanel.ResumeLayout(false);
            this.tableLayoutPanel.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel;
        private System.Windows.Forms.Button buttonOK;
        private System.Windows.Forms.Button buttonCancel;
        private System.Windows.Forms.Label labelNllbApiKey;
        private System.Windows.Forms.Label labelNllbEndpoint;
        private System.Windows.Forms.Label labelNllbInstructions;
        private System.Windows.Forms.ToolTip toolTip;
        private System.Windows.Forms.Label labelNllbModel;
        private System.Windows.Forms.TextBox textBoxNllbApiKey;
        private System.Windows.Forms.TextBox textBoxNllbEndpoint;
        private System.Windows.Forms.ComboBox comboBoxNllbModel;
    }
}