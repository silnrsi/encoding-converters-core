using SilEncConverters40;
using System.ComponentModel;
using System.Windows.Forms;

namespace SpellingFixer30
{
    partial class QueryGoodSpelling
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(QueryGoodSpelling));
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.buttonOK = new System.Windows.Forms.Button();
            this.buttonCancel = new System.Windows.Forms.Button();
            this.labelUniCodesLhs = new System.Windows.Forms.Label();
            this.buttonDelete = new System.Windows.Forms.Button();
            this.labelOriginalReason = new System.Windows.Forms.TextBox();
            this.labelOrigReasonLabel = new System.Windows.Forms.Label();
            this.tableLayoutPanel = new System.Windows.Forms.TableLayoutPanel();
            this.textBoxBadWord = new SilEncConverters40.EcTextBox();
            this.textBoxReplacement = new SilEncConverters40.EcTextBox();
            this.buttonSwapWords = new System.Windows.Forms.Button();
            this.helpProvider = new System.Windows.Forms.HelpProvider();
            this.toolTip = new System.Windows.Forms.ToolTip(this.components);
            this.labelUniCodesRhs = new System.Windows.Forms.Label();
            this.tableLayoutPanel.SuspendLayout();
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.Anchor = System.Windows.Forms.AnchorStyles.Right;
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(7, 6);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(69, 13);
            this.label1.TabIndex = 0;
            this.label1.Text = "Bad Spelling:";
            // 
            // label2
            // 
            this.label2.Anchor = System.Windows.Forms.AnchorStyles.Right;
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(3, 32);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(73, 13);
            this.label2.TabIndex = 2;
            this.label2.Text = "Replacement:";
            // 
            // buttonOK
            // 
            this.buttonOK.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.buttonOK.Location = new System.Drawing.Point(376, 243);
            this.buttonOK.Name = "buttonOK";
            this.buttonOK.Size = new System.Drawing.Size(61, 23);
            this.buttonOK.TabIndex = 3;
            this.buttonOK.Text = "OK";
            this.buttonOK.Click += new System.EventHandler(this.buttonOK_Click);
            // 
            // buttonCancel
            // 
            this.buttonCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.buttonCancel.Location = new System.Drawing.Point(443, 243);
            this.buttonCancel.Name = "buttonCancel";
            this.buttonCancel.Size = new System.Drawing.Size(61, 23);
            this.buttonCancel.TabIndex = 4;
            this.buttonCancel.Text = "Cancel";
            this.buttonCancel.Click += new System.EventHandler(this.buttonCancel_Click);
            // 
            // labelUniCodesLhs
            // 
            this.labelUniCodesLhs.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.tableLayoutPanel.SetColumnSpan(this.labelUniCodesLhs, 5);
            this.helpProvider.SetHelpString(this.labelUniCodesLhs, "This area shows the Unicode code point values for the characters in the box above" +
        " which has focus. You can use this to see hidden characters (e.g. zero width joi" +
        "ner).");
            this.labelUniCodesLhs.Location = new System.Drawing.Point(3, 55);
            this.labelUniCodesLhs.Margin = new System.Windows.Forms.Padding(3);
            this.labelUniCodesLhs.Name = "labelUniCodesLhs";
            this.labelUniCodesLhs.Padding = new System.Windows.Forms.Padding(3);
            this.helpProvider.SetShowHelp(this.labelUniCodesLhs, true);
            this.labelUniCodesLhs.Size = new System.Drawing.Size(501, 75);
            this.labelUniCodesLhs.TabIndex = 5;
            this.labelUniCodesLhs.Text = "labelUniCodesLhs";
            // 
            // buttonDelete
            // 
            this.buttonDelete.DialogResult = System.Windows.Forms.DialogResult.Abort;
            this.buttonDelete.Location = new System.Drawing.Point(3, 243);
            this.buttonDelete.Name = "buttonDelete";
            this.buttonDelete.Size = new System.Drawing.Size(67, 23);
            this.buttonDelete.TabIndex = 6;
            this.buttonDelete.Text = "Delete";
            this.toolTip.SetToolTip(this.buttonDelete, "Click to delete this rule");
            this.buttonDelete.Visible = false;
            this.buttonDelete.Click += new System.EventHandler(this.buttonDelete_Click);
            // 
            // labelOriginalReason
            // 
            this.tableLayoutPanel.SetColumnSpan(this.labelOriginalReason, 4);
            this.labelOriginalReason.Dock = System.Windows.Forms.DockStyle.Fill;
            this.labelOriginalReason.Location = new System.Drawing.Point(82, 217);
            this.labelOriginalReason.Name = "labelOriginalReason";
            this.labelOriginalReason.ReadOnly = true;
            this.labelOriginalReason.Size = new System.Drawing.Size(422, 20);
            this.labelOriginalReason.TabIndex = 7;
            this.labelOriginalReason.Visible = false;
            // 
            // labelOrigReasonLabel
            // 
            this.labelOrigReasonLabel.Anchor = System.Windows.Forms.AnchorStyles.Right;
            this.labelOrigReasonLabel.AutoSize = true;
            this.labelOrigReasonLabel.Location = new System.Drawing.Point(15, 220);
            this.labelOrigReasonLabel.Name = "labelOrigReasonLabel";
            this.labelOrigReasonLabel.Size = new System.Drawing.Size(61, 13);
            this.labelOrigReasonLabel.TabIndex = 8;
            this.labelOrigReasonLabel.Text = "while fixing:";
            this.labelOrigReasonLabel.Visible = false;
            // 
            // tableLayoutPanel
            // 
            this.tableLayoutPanel.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.tableLayoutPanel.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.tableLayoutPanel.ColumnCount = 5;
            this.tableLayoutPanel.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.tableLayoutPanel.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.tableLayoutPanel.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.tableLayoutPanel.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.tableLayoutPanel.Controls.Add(this.label1, 0, 0);
            this.tableLayoutPanel.Controls.Add(this.label2, 0, 1);
            this.tableLayoutPanel.Controls.Add(this.labelUniCodesLhs, 0, 2);
            this.tableLayoutPanel.Controls.Add(this.labelOrigReasonLabel, 0, 4);
            this.tableLayoutPanel.Controls.Add(this.buttonDelete, 0, 5);
            this.tableLayoutPanel.Controls.Add(this.textBoxBadWord, 1, 0);
            this.tableLayoutPanel.Controls.Add(this.textBoxReplacement, 1, 1);
            this.tableLayoutPanel.Controls.Add(this.labelOriginalReason, 1, 4);
            this.tableLayoutPanel.Controls.Add(this.buttonSwapWords, 1, 5);
            this.tableLayoutPanel.Controls.Add(this.buttonOK, 3, 5);
            this.tableLayoutPanel.Controls.Add(this.buttonCancel, 4, 5);
            this.tableLayoutPanel.Controls.Add(this.labelUniCodesRhs, 0, 3);
            this.tableLayoutPanel.Location = new System.Drawing.Point(12, 12);
            this.tableLayoutPanel.Name = "tableLayoutPanel";
            this.tableLayoutPanel.RowCount = 6;
            this.tableLayoutPanel.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tableLayoutPanel.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tableLayoutPanel.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel.Size = new System.Drawing.Size(507, 270);
            this.tableLayoutPanel.TabIndex = 9;
            // 
            // textBoxBadWord
            // 
            this.tableLayoutPanel.SetColumnSpan(this.textBoxBadWord, 4);
            this.textBoxBadWord.Dock = System.Windows.Forms.DockStyle.Fill;
            this.helpProvider.SetHelpString(this.textBoxBadWord, "");
            this.textBoxBadWord.Location = new System.Drawing.Point(82, 3);
            this.textBoxBadWord.Name = "textBoxBadWord";
            this.helpProvider.SetShowHelp(this.textBoxBadWord, true);
            this.textBoxBadWord.Size = new System.Drawing.Size(422, 20);
            this.textBoxBadWord.TabIndex = 1;
            this.textBoxBadWord.Text = "textBoxBadWord";
            this.toolTip.SetToolTip(this.textBoxBadWord, "Contains the bad spelling form");
            this.textBoxBadWord.TextChanged += new System.EventHandler(this.textBoxBadWord_TextChanged);
            // 
            // textBoxReplacement
            // 
            this.tableLayoutPanel.SetColumnSpan(this.textBoxReplacement, 4);
            this.textBoxReplacement.Dock = System.Windows.Forms.DockStyle.Fill;
            this.helpProvider.SetHelpString(this.textBoxReplacement, "");
            this.textBoxReplacement.Location = new System.Drawing.Point(82, 29);
            this.textBoxReplacement.Name = "textBoxReplacement";
            this.helpProvider.SetShowHelp(this.textBoxReplacement, true);
            this.textBoxReplacement.Size = new System.Drawing.Size(422, 20);
            this.textBoxReplacement.TabIndex = 1;
            this.textBoxReplacement.Text = "textBoxReplacement";
            this.toolTip.SetToolTip(this.textBoxReplacement, "Contains the good spelling form (i.e. the replacment for when the bad spelling fo" +
        "rm occurs)");
            this.textBoxReplacement.TextChanged += new System.EventHandler(this.textBoxReplacement_TextChanged);
            // 
            // buttonSwapWords
            // 
            this.buttonSwapWords.DialogResult = System.Windows.Forms.DialogResult.Retry;
            this.buttonSwapWords.Location = new System.Drawing.Point(82, 243);
            this.buttonSwapWords.Name = "buttonSwapWords";
            this.buttonSwapWords.Size = new System.Drawing.Size(91, 23);
            this.buttonSwapWords.TabIndex = 10;
            this.buttonSwapWords.Text = "&Swap Words";
            this.toolTip.SetToolTip(this.buttonSwapWords, "Click to exchange the Bad Spelling word with the Replacement word");
            this.buttonSwapWords.UseVisualStyleBackColor = true;
            this.buttonSwapWords.Click += new System.EventHandler(this.buttonSwapWords_Click);
            // 
            // labelUniCodesRhs
            // 
            this.labelUniCodesRhs.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.tableLayoutPanel.SetColumnSpan(this.labelUniCodesRhs, 5);
            this.labelUniCodesRhs.Location = new System.Drawing.Point(3, 136);
            this.labelUniCodesRhs.Margin = new System.Windows.Forms.Padding(3);
            this.labelUniCodesRhs.Name = "labelUniCodesRhs";
            this.labelUniCodesRhs.Padding = new System.Windows.Forms.Padding(3);
            this.labelUniCodesRhs.Size = new System.Drawing.Size(501, 75);
            this.labelUniCodesRhs.TabIndex = 5;
            this.labelUniCodesRhs.Text = "labelUniCodesRhs";
            // 
            // QueryGoodSpelling
            // 
            this.AcceptButton = this.buttonOK;
            this.AutoScaleBaseSize = new System.Drawing.Size(5, 13);
            this.ClientSize = new System.Drawing.Size(531, 294);
            this.Controls.Add(this.tableLayoutPanel);
            this.HelpButton = true;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "QueryGoodSpelling";
            this.Text = "Fix Spelling";
            this.tableLayoutPanel.ResumeLayout(false);
            this.tableLayoutPanel.PerformLayout();
            this.ResumeLayout(false);

        }
		#endregion

        private System.Windows.Forms.Label label1 = null;
        private EcTextBox textBoxBadWord = null;
        private System.Windows.Forms.Label label2 = null;
        private EcTextBox textBoxReplacement = null;
        private System.Windows.Forms.Button buttonOK = null;
        private System.Windows.Forms.Button buttonCancel = null;
        private string m_strGoodWord;
        private System.Windows.Forms.Label labelUniCodesLhs;
        private System.Windows.Forms.Button buttonDelete;
        private System.Windows.Forms.TextBox labelOriginalReason;
        private System.Windows.Forms.Label labelOrigReasonLabel;
        private TableLayoutPanel tableLayoutPanel;
        private HelpProvider helpProvider;
        private ToolTip toolTip;
        private Button buttonSwapWords;
        private Label labelUniCodesRhs;
    }
}