namespace SpellingFixer30
{
    partial class AddNewProjectForm
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(AddNewProjectForm));
            this.tableLayoutPanel = new System.Windows.Forms.TableLayoutPanel();
            this.labelName = new System.Windows.Forms.Label();
            this.textBoxName = new System.Windows.Forms.TextBox();
            this.labelFont = new System.Windows.Forms.Label();
            this.buttonChooseFont = new System.Windows.Forms.Button();
            this.labelFontChosen = new System.Windows.Forms.Label();
            this.checkBoxRtL = new System.Windows.Forms.CheckBox();
            this.labelWordBoundaryDelimiter = new System.Windows.Forms.Label();
            this.textBoxWordBoundaryDelimiter = new System.Windows.Forms.TextBox();
            this.labelAddlPunct = new System.Windows.Forms.Label();
            this.textBoxAddlPunctuation = new System.Windows.Forms.TextBox();
            this.buttonCancel = new System.Windows.Forms.Button();
            this.buttonOk = new System.Windows.Forms.Button();
            this.toolTips = new System.Windows.Forms.ToolTip(this.components);
            this.fontDialog = new System.Windows.Forms.FontDialog();
            this.tableLayoutPanel.SuspendLayout();
            this.SuspendLayout();
            // 
            // tableLayoutPanel
            // 
            this.tableLayoutPanel.AutoSize = true;
            this.tableLayoutPanel.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.tableLayoutPanel.ColumnCount = 4;
            this.tableLayoutPanel.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 120F));
            this.tableLayoutPanel.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.tableLayoutPanel.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 20F));
            this.tableLayoutPanel.Controls.Add(this.labelName, 0, 0);
            this.tableLayoutPanel.Controls.Add(this.textBoxName, 1, 0);
            this.tableLayoutPanel.Controls.Add(this.labelFont, 0, 1);
            this.tableLayoutPanel.Controls.Add(this.buttonChooseFont, 1, 1);
            this.tableLayoutPanel.Controls.Add(this.labelFontChosen, 2, 1);
            this.tableLayoutPanel.Controls.Add(this.checkBoxRtL, 1, 2);
            this.tableLayoutPanel.Controls.Add(this.labelWordBoundaryDelimiter, 0, 3);
            this.tableLayoutPanel.Controls.Add(this.textBoxWordBoundaryDelimiter, 1, 3);
            this.tableLayoutPanel.Controls.Add(this.labelAddlPunct, 0, 4);
            this.tableLayoutPanel.Controls.Add(this.textBoxAddlPunctuation, 1, 4);
            this.tableLayoutPanel.Controls.Add(this.buttonCancel, 2, 5);
            this.tableLayoutPanel.Controls.Add(this.buttonOk, 1, 5);
            this.tableLayoutPanel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanel.Location = new System.Drawing.Point(0, 0);
            this.tableLayoutPanel.Name = "tableLayoutPanel";
            this.tableLayoutPanel.RowCount = 6;
            this.tableLayoutPanel.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 20F));
            this.tableLayoutPanel.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 20F));
            this.tableLayoutPanel.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 20F));
            this.tableLayoutPanel.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 20F));
            this.tableLayoutPanel.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 20F));
            this.tableLayoutPanel.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel.Size = new System.Drawing.Size(463, 315);
            this.tableLayoutPanel.TabIndex = 15;
            // 
            // labelName
            // 
            this.labelName.Anchor = System.Windows.Forms.AnchorStyles.Right;
            this.labelName.AutoSize = true;
            this.labelName.Location = new System.Drawing.Point(79, 22);
            this.labelName.Name = "labelName";
            this.labelName.Size = new System.Drawing.Size(38, 13);
            this.labelName.TabIndex = 16;
            this.labelName.Text = "&Name:";
            // 
            // textBoxName
            // 
            this.textBoxName.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right)));
            this.tableLayoutPanel.SetColumnSpan(this.textBoxName, 2);
            this.textBoxName.Location = new System.Drawing.Point(123, 18);
            this.textBoxName.Name = "textBoxName";
            this.textBoxName.Size = new System.Drawing.Size(317, 20);
            this.textBoxName.TabIndex = 0;
            this.toolTips.SetToolTip(this.textBoxName, "Enter the name of a new projects (e.g. \'Hindi to English fixes\')");
            // 
            // labelFont
            // 
            this.labelFont.Anchor = System.Windows.Forms.AnchorStyles.Right;
            this.labelFont.AutoSize = true;
            this.labelFont.Location = new System.Drawing.Point(86, 79);
            this.labelFont.Name = "labelFont";
            this.labelFont.Size = new System.Drawing.Size(31, 13);
            this.labelFont.TabIndex = 16;
            this.labelFont.Text = "Font:";
            // 
            // buttonChooseFont
            // 
            this.buttonChooseFont.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.buttonChooseFont.Location = new System.Drawing.Point(123, 74);
            this.buttonChooseFont.Name = "buttonChooseFont";
            this.buttonChooseFont.Size = new System.Drawing.Size(84, 23);
            this.buttonChooseFont.TabIndex = 2;
            this.buttonChooseFont.Text = "Choose &Font";
            this.toolTips.SetToolTip(this.buttonChooseFont, "Click to choose the font to be used for displaying the Find-Replace pairs (e.g. \'" +
        "Arial Unicode MS, 12 pt\')");
            this.buttonChooseFont.UseVisualStyleBackColor = true;
            this.buttonChooseFont.Click += new System.EventHandler(this.ButtonChooseFont_Click);
            // 
            // labelFontChosen
            // 
            this.labelFontChosen.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right)));
            this.labelFontChosen.Location = new System.Drawing.Point(227, 76);
            this.labelFontChosen.Name = "labelFontChosen";
            this.labelFontChosen.Size = new System.Drawing.Size(213, 18);
            this.labelFontChosen.TabIndex = 19;
            // 
            // checkBoxRtL
            // 
            this.checkBoxRtL.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.checkBoxRtL.CheckAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.checkBoxRtL.Cursor = System.Windows.Forms.Cursors.Hand;
            this.checkBoxRtL.Location = new System.Drawing.Point(123, 130);
            this.checkBoxRtL.Name = "checkBoxRtL";
            this.checkBoxRtL.Size = new System.Drawing.Size(98, 24);
            this.checkBoxRtL.TabIndex = 3;
            this.checkBoxRtL.Text = "&Right-to-Left";
            this.toolTips.SetToolTip(this.checkBoxRtL, "Check this if the data is right-to-left");
            // 
            // labelWordBoundaryDelimiter
            // 
            this.labelWordBoundaryDelimiter.Anchor = System.Windows.Forms.AnchorStyles.Right;
            this.labelWordBoundaryDelimiter.Location = new System.Drawing.Point(27, 180);
            this.labelWordBoundaryDelimiter.Name = "labelWordBoundaryDelimiter";
            this.labelWordBoundaryDelimiter.Size = new System.Drawing.Size(90, 38);
            this.labelWordBoundaryDelimiter.TabIndex = 21;
            this.labelWordBoundaryDelimiter.Text = "&Word boundary delimiter:";
            // 
            // textBoxWordBoundaryDelimiter
            // 
            this.textBoxWordBoundaryDelimiter.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.textBoxWordBoundaryDelimiter.Location = new System.Drawing.Point(123, 189);
            this.textBoxWordBoundaryDelimiter.Name = "textBoxWordBoundaryDelimiter";
            this.textBoxWordBoundaryDelimiter.Size = new System.Drawing.Size(24, 20);
            this.textBoxWordBoundaryDelimiter.TabIndex = 4;
            this.textBoxWordBoundaryDelimiter.Text = "#";
            this.toolTips.SetToolTip(this.textBoxWordBoundaryDelimiter, resources.GetString("textBoxWordBoundaryDelimiter.ToolTip"));
            // 
            // labelAddlPunct
            // 
            this.labelAddlPunct.Anchor = System.Windows.Forms.AnchorStyles.Right;
            this.labelAddlPunct.AutoSize = true;
            this.labelAddlPunct.Location = new System.Drawing.Point(5, 243);
            this.labelAddlPunct.Name = "labelAddlPunct";
            this.labelAddlPunct.Size = new System.Drawing.Size(112, 26);
            this.labelAddlPunct.TabIndex = 23;
            this.labelAddlPunct.Text = "Additional &punctuation and whitespace:";
            this.labelAddlPunct.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // textBoxAddlPunctuation
            // 
            this.textBoxAddlPunctuation.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right)));
            this.tableLayoutPanel.SetColumnSpan(this.textBoxAddlPunctuation, 2);
            this.textBoxAddlPunctuation.Location = new System.Drawing.Point(123, 246);
            this.textBoxAddlPunctuation.Name = "textBoxAddlPunctuation";
            this.textBoxAddlPunctuation.Size = new System.Drawing.Size(317, 20);
            this.textBoxAddlPunctuation.TabIndex = 5;
            this.toolTips.SetToolTip(this.textBoxAddlPunctuation, "Enter any additional punctuation or whitespace characters needed for this languag" +
        "e, separated by spaces (these are used for boundary condition checking)");
            // 
            // buttonCancel
            // 
            this.buttonCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.buttonCancel.Location = new System.Drawing.Point(227, 288);
            this.buttonCancel.Name = "buttonCancel";
            this.buttonCancel.Size = new System.Drawing.Size(75, 23);
            this.buttonCancel.TabIndex = 7;
            this.buttonCancel.Text = "&Cancel";
            this.buttonCancel.UseVisualStyleBackColor = true;
            // 
            // buttonOk
            // 
            this.buttonOk.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.buttonOk.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.buttonOk.Location = new System.Drawing.Point(146, 288);
            this.buttonOk.Name = "buttonOk";
            this.buttonOk.Size = new System.Drawing.Size(75, 23);
            this.buttonOk.TabIndex = 6;
            this.buttonOk.Text = "OK";
            this.buttonOk.UseVisualStyleBackColor = true;
            this.buttonOk.Click += new System.EventHandler(this.ButtonOk_Click);
            // 
            // toolTips
            // 
            this.toolTips.AutoPopDelay = 30000;
            this.toolTips.InitialDelay = 500;
            this.toolTips.ReshowDelay = 100;
            // 
            // AddNewProjectForm
            // 
            this.AcceptButton = this.buttonOk;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.CancelButton = this.buttonCancel;
            this.ClientSize = new System.Drawing.Size(463, 315);
            this.Controls.Add(this.tableLayoutPanel);
            this.Name = "AddNewProjectForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Add New Project";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.AddNewProjectForm_FormClosing);
            this.tableLayoutPanel.ResumeLayout(false);
            this.tableLayoutPanel.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel;
        private System.Windows.Forms.Label labelName;
        private System.Windows.Forms.TextBox textBoxName;
        private System.Windows.Forms.Label labelFont;
        private System.Windows.Forms.Button buttonChooseFont;
        private System.Windows.Forms.Label labelFontChosen;
        private System.Windows.Forms.CheckBox checkBoxRtL;
        private System.Windows.Forms.Label labelWordBoundaryDelimiter;
        private System.Windows.Forms.TextBox textBoxWordBoundaryDelimiter;
        private System.Windows.Forms.Label labelAddlPunct;
        private System.Windows.Forms.TextBox textBoxAddlPunctuation;
        private System.Windows.Forms.Button buttonCancel;
        private System.Windows.Forms.Button buttonOk;
        private System.Windows.Forms.ToolTip toolTips;
        private System.Windows.Forms.FontDialog fontDialog;
    }
}