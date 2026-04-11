namespace SpellingFixer30
{
    partial class QueryFindReplaceDialog
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(QueryFindReplaceDialog));
            this.tableLayoutPanel = new System.Windows.Forms.TableLayoutPanel();
            this.labelFind = new System.Windows.Forms.Label();
            this.textBoxFindWord = new SilEncConverters40.EcTextBox();
            this.labelReplace = new System.Windows.Forms.Label();
            this.textBoxReplaceWord = new SilEncConverters40.EcTextBox();
            this.groupBoxWordBoundaries = new System.Windows.Forms.GroupBox();
            this.checkBoxWordFinal = new System.Windows.Forms.CheckBox();
            this.checkBoxWordInitial = new System.Windows.Forms.CheckBox();
            this.labelUnicodeCodes = new System.Windows.Forms.Label();
            this.labelAddedFromOriginalWord = new System.Windows.Forms.Label();
            this.textBoxOriginalWord = new System.Windows.Forms.TextBox();
            this.buttonDelete = new System.Windows.Forms.Button();
            this.buttonOk = new System.Windows.Forms.Button();
            this.buttonCancel = new System.Windows.Forms.Button();
            this.textBoxWordBoundaryFindText = new System.Windows.Forms.TextBox();
            this.toolTip = new System.Windows.Forms.ToolTip(this.components);
            this.tableLayoutPanel.SuspendLayout();
            this.groupBoxWordBoundaries.SuspendLayout();
            this.SuspendLayout();
            // 
            // tableLayoutPanel
            // 
            this.tableLayoutPanel.ColumnCount = 5;
            this.tableLayoutPanel.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.tableLayoutPanel.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.tableLayoutPanel.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.tableLayoutPanel.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.tableLayoutPanel.Controls.Add(this.labelFind, 0, 0);
            this.tableLayoutPanel.Controls.Add(this.textBoxFindWord, 1, 0);
            this.tableLayoutPanel.Controls.Add(this.labelReplace, 0, 1);
            this.tableLayoutPanel.Controls.Add(this.textBoxReplaceWord, 1, 1);
            this.tableLayoutPanel.Controls.Add(this.groupBoxWordBoundaries, 0, 2);
            this.tableLayoutPanel.Controls.Add(this.labelUnicodeCodes, 0, 3);
            this.tableLayoutPanel.Controls.Add(this.labelAddedFromOriginalWord, 0, 4);
            this.tableLayoutPanel.Controls.Add(this.textBoxOriginalWord, 2, 4);
            this.tableLayoutPanel.Controls.Add(this.buttonDelete, 0, 5);
            this.tableLayoutPanel.Controls.Add(this.buttonOk, 3, 5);
            this.tableLayoutPanel.Controls.Add(this.buttonCancel, 4, 5);
            this.tableLayoutPanel.Controls.Add(this.textBoxWordBoundaryFindText, 2, 2);
            this.tableLayoutPanel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanel.Location = new System.Drawing.Point(0, 0);
            this.tableLayoutPanel.Margin = new System.Windows.Forms.Padding(2, 2, 2, 2);
            this.tableLayoutPanel.Name = "tableLayoutPanel";
            this.tableLayoutPanel.Padding = new System.Windows.Forms.Padding(12, 13, 12, 13);
            this.tableLayoutPanel.RowCount = 6;
            this.tableLayoutPanel.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel.Size = new System.Drawing.Size(496, 311);
            this.tableLayoutPanel.TabIndex = 0;
            // 
            // labelFind
            // 
            this.labelFind.Anchor = System.Windows.Forms.AnchorStyles.Right;
            this.labelFind.AutoSize = true;
            this.labelFind.Location = new System.Drawing.Point(47, 18);
            this.labelFind.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.labelFind.Name = "labelFind";
            this.labelFind.Size = new System.Drawing.Size(30, 13);
            this.labelFind.TabIndex = 0;
            this.labelFind.Text = "Find:";
            // 
            // textBoxFindWord
            // 
            this.tableLayoutPanel.SetColumnSpan(this.textBoxFindWord, 4);
            this.textBoxFindWord.Dock = System.Windows.Forms.DockStyle.Fill;
            this.textBoxFindWord.Location = new System.Drawing.Point(81, 15);
            this.textBoxFindWord.Margin = new System.Windows.Forms.Padding(2, 2, 2, 2);
            this.textBoxFindWord.Name = "textBoxFindWord";
            this.textBoxFindWord.Size = new System.Drawing.Size(401, 20);
            this.textBoxFindWord.TabIndex = 2;
            this.toolTip.SetToolTip(this.textBoxFindWord, "The word (or part of a word) to replace");
            this.textBoxFindWord.TextChanged += new System.EventHandler(this.TextBoxFindWord_TextChanged);
            this.textBoxFindWord.Enter += new System.EventHandler(this.TextBoxFindWord_Enter);
            // 
            // labelReplace
            // 
            this.labelReplace.Anchor = System.Windows.Forms.AnchorStyles.Right;
            this.labelReplace.AutoSize = true;
            this.labelReplace.Location = new System.Drawing.Point(27, 42);
            this.labelReplace.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.labelReplace.Name = "labelReplace";
            this.labelReplace.Size = new System.Drawing.Size(50, 13);
            this.labelReplace.TabIndex = 1;
            this.labelReplace.Text = "Replace:";
            // 
            // textBoxReplaceWord
            // 
            this.tableLayoutPanel.SetColumnSpan(this.textBoxReplaceWord, 4);
            this.textBoxReplaceWord.Dock = System.Windows.Forms.DockStyle.Fill;
            this.textBoxReplaceWord.Location = new System.Drawing.Point(81, 39);
            this.textBoxReplaceWord.Margin = new System.Windows.Forms.Padding(2, 2, 2, 2);
            this.textBoxReplaceWord.Name = "textBoxReplaceWord";
            this.textBoxReplaceWord.Size = new System.Drawing.Size(401, 20);
            this.textBoxReplaceWord.TabIndex = 1;
            this.toolTip.SetToolTip(this.textBoxReplaceWord, "The replacement word");
            this.textBoxReplaceWord.TextChanged += new System.EventHandler(this.TextBoxReplaceWord_TextChanged);
            this.textBoxReplaceWord.Enter += new System.EventHandler(this.TextBoxReplaceWord_Enter);
            // 
            // groupBoxWordBoundaries
            // 
            this.tableLayoutPanel.SetColumnSpan(this.groupBoxWordBoundaries, 2);
            this.groupBoxWordBoundaries.Controls.Add(this.checkBoxWordFinal);
            this.groupBoxWordBoundaries.Controls.Add(this.checkBoxWordInitial);
            this.groupBoxWordBoundaries.Location = new System.Drawing.Point(14, 63);
            this.groupBoxWordBoundaries.Margin = new System.Windows.Forms.Padding(2, 2, 2, 2);
            this.groupBoxWordBoundaries.Name = "groupBoxWordBoundaries";
            this.groupBoxWordBoundaries.Padding = new System.Windows.Forms.Padding(5, 5, 5, 5);
            this.groupBoxWordBoundaries.Size = new System.Drawing.Size(150, 53);
            this.groupBoxWordBoundaries.TabIndex = 3;
            this.groupBoxWordBoundaries.TabStop = false;
            this.groupBoxWordBoundaries.Text = "Word Boundaries";
            // 
            // checkBoxWordFinal
            // 
            this.checkBoxWordFinal.AutoSize = true;
            this.checkBoxWordFinal.Location = new System.Drawing.Point(94, 19);
            this.checkBoxWordFinal.Margin = new System.Windows.Forms.Padding(2, 2, 2, 2);
            this.checkBoxWordFinal.Name = "checkBoxWordFinal";
            this.checkBoxWordFinal.Size = new System.Drawing.Size(45, 17);
            this.checkBoxWordFinal.TabIndex = 5;
            this.checkBoxWordFinal.Text = "End";
            this.checkBoxWordFinal.UseVisualStyleBackColor = true;
            this.checkBoxWordFinal.CheckedChanged += new System.EventHandler(this.CheckBoxWordBoundary_CheckedChanged);
            // 
            // checkBoxWordInitial
            // 
            this.checkBoxWordInitial.AutoSize = true;
            this.checkBoxWordInitial.Location = new System.Drawing.Point(14, 19);
            this.checkBoxWordInitial.Margin = new System.Windows.Forms.Padding(2, 2, 2, 2);
            this.checkBoxWordInitial.Name = "checkBoxWordInitial";
            this.checkBoxWordInitial.Size = new System.Drawing.Size(73, 17);
            this.checkBoxWordInitial.TabIndex = 4;
            this.checkBoxWordInitial.Text = "Beginning";
            this.toolTip.SetToolTip(this.checkBoxWordInitial, "Click to force the find to use use an initial ");
            this.checkBoxWordInitial.UseVisualStyleBackColor = true;
            this.checkBoxWordInitial.CheckedChanged += new System.EventHandler(this.CheckBoxWordBoundary_CheckedChanged);
            // 
            // labelUnicodeCodes
            // 
            this.labelUnicodeCodes.AutoSize = true;
            this.labelUnicodeCodes.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.tableLayoutPanel.SetColumnSpan(this.labelUnicodeCodes, 5);
            this.labelUnicodeCodes.Dock = System.Windows.Forms.DockStyle.Fill;
            this.labelUnicodeCodes.Location = new System.Drawing.Point(14, 118);
            this.labelUnicodeCodes.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.labelUnicodeCodes.Name = "labelUnicodeCodes";
            this.labelUnicodeCodes.Padding = new System.Windows.Forms.Padding(5, 5, 5, 5);
            this.labelUnicodeCodes.Size = new System.Drawing.Size(468, 128);
            this.labelUnicodeCodes.TabIndex = 9;
            this.labelUnicodeCodes.Text = "labelUnicodeCodes";
            this.toolTip.SetToolTip(this.labelUnicodeCodes, "This area shows the Unicode code point values for the characters in the box above" +
        " which has focus. You can use this to see hidden characters (e.g. zero width joi" +
        "ner).");
            // 
            // labelAddedFromOriginalWord
            // 
            this.labelAddedFromOriginalWord.AutoSize = true;
            this.tableLayoutPanel.SetColumnSpan(this.labelAddedFromOriginalWord, 2);
            this.labelAddedFromOriginalWord.Location = new System.Drawing.Point(14, 246);
            this.labelAddedFromOriginalWord.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.labelAddedFromOriginalWord.Name = "labelAddedFromOriginalWord";
            this.labelAddedFromOriginalWord.Size = new System.Drawing.Size(136, 13);
            this.labelAddedFromOriginalWord.TabIndex = 10;
            this.labelAddedFromOriginalWord.Text = "Added while Clipboard had:";
            this.toolTip.SetToolTip(this.labelAddedFromOriginalWord, "This word was on the clipboard when the dialog opened.");
            this.labelAddedFromOriginalWord.Visible = false;
            // 
            // textBoxOriginalWord
            // 
            this.tableLayoutPanel.SetColumnSpan(this.textBoxOriginalWord, 3);
            this.textBoxOriginalWord.Dock = System.Windows.Forms.DockStyle.Fill;
            this.textBoxOriginalWord.Location = new System.Drawing.Point(168, 248);
            this.textBoxOriginalWord.Margin = new System.Windows.Forms.Padding(2, 2, 2, 2);
            this.textBoxOriginalWord.Name = "textBoxOriginalWord";
            this.textBoxOriginalWord.ReadOnly = true;
            this.textBoxOriginalWord.Size = new System.Drawing.Size(314, 20);
            this.textBoxOriginalWord.TabIndex = 11;
            this.toolTip.SetToolTip(this.textBoxOriginalWord, "Word originally on the Clipboard when this dialog opened.");
            this.textBoxOriginalWord.Visible = false;
            // 
            // buttonDelete
            // 
            this.buttonDelete.DialogResult = System.Windows.Forms.DialogResult.Abort;
            this.buttonDelete.Location = new System.Drawing.Point(14, 272);
            this.buttonDelete.Margin = new System.Windows.Forms.Padding(2, 2, 2, 2);
            this.buttonDelete.Name = "buttonDelete";
            this.buttonDelete.Size = new System.Drawing.Size(63, 24);
            this.buttonDelete.TabIndex = 8;
            this.buttonDelete.Text = "Delete";
            this.buttonDelete.UseVisualStyleBackColor = true;
            this.buttonDelete.Click += new System.EventHandler(this.ButtonDelete_Click);
            // 
            // buttonOk
            // 
            this.buttonOk.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.buttonOk.Location = new System.Drawing.Point(354, 272);
            this.buttonOk.Margin = new System.Windows.Forms.Padding(2, 2, 2, 2);
            this.buttonOk.Name = "buttonOk";
            this.buttonOk.Size = new System.Drawing.Size(62, 24);
            this.buttonOk.TabIndex = 6;
            this.buttonOk.Text = "OK";
            this.buttonOk.UseVisualStyleBackColor = true;
            // 
            // buttonCancel
            // 
            this.buttonCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.buttonCancel.Location = new System.Drawing.Point(420, 272);
            this.buttonCancel.Margin = new System.Windows.Forms.Padding(2, 2, 2, 2);
            this.buttonCancel.Name = "buttonCancel";
            this.buttonCancel.Size = new System.Drawing.Size(62, 24);
            this.buttonCancel.TabIndex = 7;
            this.buttonCancel.Text = "Cancel";
            this.buttonCancel.UseVisualStyleBackColor = true;
            // 
            // textBoxWordBoundaryFindText
            // 
            this.textBoxWordBoundaryFindText.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right)));
            this.tableLayoutPanel.SetColumnSpan(this.textBoxWordBoundaryFindText, 3);
            this.textBoxWordBoundaryFindText.Location = new System.Drawing.Point(168, 79);
            this.textBoxWordBoundaryFindText.Margin = new System.Windows.Forms.Padding(2, 2, 2, 2);
            this.textBoxWordBoundaryFindText.Name = "textBoxWordBoundaryFindText";
            this.textBoxWordBoundaryFindText.ReadOnly = true;
            this.textBoxWordBoundaryFindText.Size = new System.Drawing.Size(314, 20);
            this.textBoxWordBoundaryFindText.TabIndex = 8;
            // 
            // QueryFindReplaceDialog
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(496, 311);
            this.Controls.Add(this.tableLayoutPanel);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Margin = new System.Windows.Forms.Padding(2, 2, 2, 2);
            this.Name = "QueryFindReplaceDialog";
            this.Text = "Find & Replace (CC)";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.QueryFindReplaceDialog_FormClosing);
            this.tableLayoutPanel.ResumeLayout(false);
            this.tableLayoutPanel.PerformLayout();
            this.groupBoxWordBoundaries.ResumeLayout(false);
            this.groupBoxWordBoundaries.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel;
        private System.Windows.Forms.Label labelFind;
        private System.Windows.Forms.Label labelReplace;
        private System.Windows.Forms.Button buttonDelete;
        private SilEncConverters40.EcTextBox textBoxFindWord;
        private System.Windows.Forms.ToolTip toolTip;
        private SilEncConverters40.EcTextBox textBoxReplaceWord;
        private System.Windows.Forms.Label labelUnicodeCodes;
        private System.Windows.Forms.Label labelAddedFromOriginalWord;
        private System.Windows.Forms.TextBox textBoxOriginalWord;
        private System.Windows.Forms.Button buttonOk;
        private System.Windows.Forms.Button buttonCancel;
        private System.Windows.Forms.CheckBox checkBoxWordInitial;
        private System.Windows.Forms.CheckBox checkBoxWordFinal;
        private System.Windows.Forms.GroupBox groupBoxWordBoundaries;
        private System.Windows.Forms.TextBox textBoxWordBoundaryFindText;
    }
}