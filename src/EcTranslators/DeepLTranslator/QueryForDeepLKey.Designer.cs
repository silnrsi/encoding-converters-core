
namespace SilEncConverters40.EcTranslators.DeepLTranslator
{
	partial class QueryForDeepLKey
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
            this.buttonOK = new System.Windows.Forms.Button();
            this.buttonCancel = new System.Windows.Forms.Button();
            this.labelTranslatorKey = new System.Windows.Forms.Label();
            this.textBoxTranslatorKey = new System.Windows.Forms.TextBox();
            this.linkLabelInstructions = new System.Windows.Forms.LinkLabel();
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
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 35F));
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 65F));
            this.tableLayoutPanel1.Controls.Add(this.buttonOK, 1, 3);
            this.tableLayoutPanel1.Controls.Add(this.buttonCancel, 2, 3);
            this.tableLayoutPanel1.Controls.Add(this.labelTranslatorKey, 0, 0);
            this.tableLayoutPanel1.Controls.Add(this.textBoxTranslatorKey, 1, 0);
            this.tableLayoutPanel1.Controls.Add(this.linkLabelInstructions, 0, 2);
            this.tableLayoutPanel1.Location = new System.Drawing.Point(12, 12);
            this.tableLayoutPanel1.Name = "tableLayoutPanel1";
            this.tableLayoutPanel1.RowCount = 4;
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 50F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 50F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel1.Size = new System.Drawing.Size(662, 190);
            this.tableLayoutPanel1.TabIndex = 0;
            // 
            // buttonOK
            // 
            this.buttonOK.Anchor = System.Windows.Forms.AnchorStyles.Right;
            this.buttonOK.Location = new System.Drawing.Point(249, 164);
            this.buttonOK.Name = "buttonOK";
            this.buttonOK.Size = new System.Drawing.Size(75, 23);
            this.buttonOK.TabIndex = 6;
            this.buttonOK.Text = "&OK";
            this.buttonOK.UseVisualStyleBackColor = true;
            // 
            // buttonCancel
            // 
            this.buttonCancel.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.buttonCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.buttonCancel.Location = new System.Drawing.Point(330, 164);
            this.buttonCancel.Name = "buttonCancel";
            this.buttonCancel.Size = new System.Drawing.Size(75, 23);
            this.buttonCancel.TabIndex = 7;
            this.buttonCancel.Text = "Cancel";
            this.buttonCancel.UseVisualStyleBackColor = true;
            // 
            // labelTranslatorKey
            // 
            this.labelTranslatorKey.Anchor = System.Windows.Forms.AnchorStyles.Right;
            this.labelTranslatorKey.AutoSize = true;
            this.labelTranslatorKey.Location = new System.Drawing.Point(3, 18);
            this.labelTranslatorKey.Name = "labelTranslatorKey";
            this.labelTranslatorKey.Size = new System.Drawing.Size(141, 13);
            this.labelTranslatorKey.TabIndex = 1;
            this.labelTranslatorKey.Text = "Enter DeepL Translator Key:";
            // 
            // textBoxTranslatorKey
            // 
            this.textBoxTranslatorKey.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right)));
            this.tableLayoutPanel1.SetColumnSpan(this.textBoxTranslatorKey, 2);
            this.textBoxTranslatorKey.Location = new System.Drawing.Point(150, 15);
            this.textBoxTranslatorKey.Name = "textBoxTranslatorKey";
            this.textBoxTranslatorKey.Size = new System.Drawing.Size(509, 20);
            this.textBoxTranslatorKey.TabIndex = 2;
            // 
            // linkLabelInstructions
            // 
            this.linkLabelInstructions.AutoSize = true;
            this.tableLayoutPanel1.SetColumnSpan(this.linkLabelInstructions, 3);
            this.linkLabelInstructions.Location = new System.Drawing.Point(3, 100);
            this.linkLabelInstructions.Name = "linkLabelInstructions";
            this.linkLabelInstructions.Size = new System.Drawing.Size(414, 13);
            this.linkLabelInstructions.TabIndex = 5;
            this.linkLabelInstructions.TabStop = true;
            this.linkLabelInstructions.Text = "see {0} for instructions on creating your own DeepL or DeepL Pro Translator Resou" +
    "rce";
            this.linkLabelInstructions.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.linkLabelInstructions_LinkClicked);
            // 
            // QueryForDeepLKey
            // 
            this.AcceptButton = this.buttonOK;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.CancelButton = this.buttonCancel;
            this.ClientSize = new System.Drawing.Size(686, 214);
            this.Controls.Add(this.tableLayoutPanel1);
            this.Name = "QueryForDeepLKey";
            this.Text = "Enter DeepL Translator Resource Key";
            this.tableLayoutPanel1.ResumeLayout(false);
            this.tableLayoutPanel1.PerformLayout();
            this.ResumeLayout(false);

		}

		#endregion

		private System.Windows.Forms.TableLayoutPanel tableLayoutPanel1;
		private System.Windows.Forms.Button buttonOK;
		private System.Windows.Forms.Button buttonCancel;
		private System.Windows.Forms.Label labelTranslatorKey;
		private System.Windows.Forms.TextBox textBoxTranslatorKey;
		private System.Windows.Forms.LinkLabel linkLabelInstructions;
	}
}