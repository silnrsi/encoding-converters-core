namespace BackTranslationHelper
{
	partial class ExampleEditorDialog
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
            this.tableLayoutPanel = new System.Windows.Forms.TableLayoutPanel();
            this.buttonOk = new System.Windows.Forms.Button();
            this.buttonCancel = new System.Windows.Forms.Button();
            this.textBoxExampleInput = new System.Windows.Forms.TextBox();
            this.textBoxExampleOutput = new System.Windows.Forms.TextBox();
            this.flowLayoutPanel = new System.Windows.Forms.FlowLayoutPanel();
            this.tableLayoutPanel.SuspendLayout();
            this.SuspendLayout();
            // 
            // tableLayoutPanel
            // 
            this.tableLayoutPanel.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.tableLayoutPanel.ColumnCount = 2;
            this.tableLayoutPanel.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tableLayoutPanel.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tableLayoutPanel.Controls.Add(this.buttonOk, 0, 3);
            this.tableLayoutPanel.Controls.Add(this.buttonCancel, 1, 3);
            this.tableLayoutPanel.Controls.Add(this.textBoxExampleInput, 0, 0);
            this.tableLayoutPanel.Controls.Add(this.textBoxExampleOutput, 0, 1);
            this.tableLayoutPanel.Controls.Add(this.flowLayoutPanel, 0, 2);
            this.tableLayoutPanel.Location = new System.Drawing.Point(12, 12);
            this.tableLayoutPanel.Name = "tableLayoutPanel";
            this.tableLayoutPanel.RowCount = 4;
            this.tableLayoutPanel.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tableLayoutPanel.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tableLayoutPanel.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel.Size = new System.Drawing.Size(742, 393);
            this.tableLayoutPanel.TabIndex = 0;
            // 
            // buttonOk
            // 
            this.buttonOk.Anchor = System.Windows.Forms.AnchorStyles.Right;
            this.buttonOk.Location = new System.Drawing.Point(293, 367);
            this.buttonOk.Name = "buttonOk";
            this.buttonOk.Size = new System.Drawing.Size(75, 23);
            this.buttonOk.TabIndex = 4;
            this.buttonOk.Text = "&OK";
            this.buttonOk.Click += new System.EventHandler(this.ButtonOk_Click);
            // 
            // buttonCancel
            // 
            this.buttonCancel.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.buttonCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.buttonCancel.Location = new System.Drawing.Point(374, 367);
            this.buttonCancel.Name = "buttonCancel";
            this.buttonCancel.Size = new System.Drawing.Size(75, 23);
            this.buttonCancel.TabIndex = 5;
            this.buttonCancel.Text = "&Cancel";
            this.buttonCancel.Click += new System.EventHandler(this.ButtonCancel_Click);
            // 
            // textBoxExampleInput
            // 
            this.tableLayoutPanel.SetColumnSpan(this.textBoxExampleInput, 2);
            this.textBoxExampleInput.Dock = System.Windows.Forms.DockStyle.Fill;
            this.textBoxExampleInput.Location = new System.Drawing.Point(3, 3);
            this.textBoxExampleInput.Multiline = true;
            this.textBoxExampleInput.Name = "textBoxExampleInput";
            this.textBoxExampleInput.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.textBoxExampleInput.Size = new System.Drawing.Size(736, 141);
            this.textBoxExampleInput.TabIndex = 1;
            // 
            // textBoxExampleOutput
            // 
            this.tableLayoutPanel.SetColumnSpan(this.textBoxExampleOutput, 2);
            this.textBoxExampleOutput.Dock = System.Windows.Forms.DockStyle.Fill;
            this.textBoxExampleOutput.Location = new System.Drawing.Point(3, 150);
            this.textBoxExampleOutput.Multiline = true;
            this.textBoxExampleOutput.Name = "textBoxExampleOutput";
            this.textBoxExampleOutput.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.textBoxExampleOutput.Size = new System.Drawing.Size(736, 141);
            this.textBoxExampleOutput.TabIndex = 2;
            // 
            // flowLayoutPanel
            // 
            this.tableLayoutPanel.SetColumnSpan(this.flowLayoutPanel, 2);
            this.flowLayoutPanel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.flowLayoutPanel.Location = new System.Drawing.Point(3, 297);
            this.flowLayoutPanel.Name = "flowLayoutPanel";
            this.flowLayoutPanel.Size = new System.Drawing.Size(736, 64);
            this.flowLayoutPanel.TabIndex = 7;
            // 
            // ExampleEditorDialog
            // 
            this.AcceptButton = this.buttonOk;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.CancelButton = this.buttonCancel;
            this.ClientSize = new System.Drawing.Size(766, 417);
            this.Controls.Add(this.tableLayoutPanel);
            this.Name = "ExampleEditorDialog";
            this.Text = "Add Example Editor";
            this.tableLayoutPanel.ResumeLayout(false);
            this.tableLayoutPanel.PerformLayout();
            this.ResumeLayout(false);

		}

		#endregion

		private System.Windows.Forms.TableLayoutPanel tableLayoutPanel;
		private System.Windows.Forms.Button buttonOk;
		private System.Windows.Forms.Button buttonCancel;
		private System.Windows.Forms.TextBox textBoxExampleInput;
		private System.Windows.Forms.TextBox textBoxExampleOutput;
		private System.Windows.Forms.FlowLayoutPanel flowLayoutPanel;
	}
}