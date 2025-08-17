namespace SilEncConverters40
{
    partial class ViewSourceFormsForm
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(ViewSourceFormsForm));
            this.tableLayoutPanel = new System.Windows.Forms.TableLayoutPanel();
            this.labelFilter = new System.Windows.Forms.Label();
            this.buttonCancel = new System.Windows.Forms.Button();
            this.listBoxSourceWordForms = new System.Windows.Forms.ListBox();
            this.contextMenuStrip = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.deleteToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.editToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.buttonOK = new System.Windows.Forms.Button();
            this.textBoxFilter = new System.Windows.Forms.TextBox();
            this.buttonAddNewSourceWord = new System.Windows.Forms.Button();
            this.buttonOtherCommands = new System.Windows.Forms.Button();
            this.toolTip = new System.Windows.Forms.ToolTip(this.components);
            this.progressBar = new System.Windows.Forms.ProgressBar();
            this.contextMenuStripOtherCommands = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.createReversalProjectToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.mergeProjectsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.targetFormDisplayControl = new SilEncConverters40.TargetFormDisplayControl();
            this.tableLayoutPanel.SuspendLayout();
            this.contextMenuStrip.SuspendLayout();
            this.contextMenuStripOtherCommands.SuspendLayout();
            this.SuspendLayout();
            // 
            // tableLayoutPanel
            // 
            this.tableLayoutPanel.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.tableLayoutPanel.ColumnCount = 3;
            this.tableLayoutPanel.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 41.73442F));
            this.tableLayoutPanel.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.tableLayoutPanel.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 58.26558F));
            this.tableLayoutPanel.Controls.Add(this.labelFilter, 0, 0);
            this.tableLayoutPanel.Controls.Add(this.buttonCancel, 2, 4);
            this.tableLayoutPanel.Controls.Add(this.listBoxSourceWordForms, 0, 2);
            this.tableLayoutPanel.Controls.Add(this.buttonOK, 1, 4);
            this.tableLayoutPanel.Controls.Add(this.textBoxFilter, 0, 1);
            this.tableLayoutPanel.Controls.Add(this.buttonAddNewSourceWord, 0, 3);
            this.tableLayoutPanel.Controls.Add(this.targetFormDisplayControl, 1, 1);
            this.tableLayoutPanel.Controls.Add(this.buttonOtherCommands, 2, 0);
            this.tableLayoutPanel.Location = new System.Drawing.Point(13, 14);
            this.tableLayoutPanel.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.tableLayoutPanel.Name = "tableLayoutPanel";
            this.tableLayoutPanel.RowCount = 5;
            this.tableLayoutPanel.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel.Size = new System.Drawing.Size(870, 591);
            this.tableLayoutPanel.TabIndex = 0;
            // 
            // labelFilter
            // 
            this.labelFilter.Anchor = System.Windows.Forms.AnchorStyles.Bottom;
            this.labelFilter.AutoSize = true;
            this.labelFilter.Location = new System.Drawing.Point(80, 25);
            this.labelFilter.Name = "labelFilter";
            this.labelFilter.Size = new System.Drawing.Size(153, 20);
            this.labelFilter.TabIndex = 4;
            this.labelFilter.Text = "Source Words Filter:";
            // 
            // buttonCancel
            // 
            this.buttonCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.buttonCancel.Location = new System.Drawing.Point(437, 551);
            this.buttonCancel.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.buttonCancel.Name = "buttonCancel";
            this.buttonCancel.Size = new System.Drawing.Size(112, 35);
            this.buttonCancel.TabIndex = 1;
            this.buttonCancel.Text = "Cancel";
            this.buttonCancel.UseVisualStyleBackColor = true;
            this.buttonCancel.Click += new System.EventHandler(this.buttonCancel_Click);
            // 
            // listBoxSourceWordForms
            // 
            this.listBoxSourceWordForms.ContextMenuStrip = this.contextMenuStrip;
            this.listBoxSourceWordForms.Dock = System.Windows.Forms.DockStyle.Fill;
            this.listBoxSourceWordForms.FormattingEnabled = true;
            this.listBoxSourceWordForms.ItemHeight = 20;
            this.listBoxSourceWordForms.Location = new System.Drawing.Point(4, 86);
            this.listBoxSourceWordForms.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.listBoxSourceWordForms.Name = "listBoxSourceWordForms";
            this.listBoxSourceWordForms.Size = new System.Drawing.Size(305, 410);
            this.listBoxSourceWordForms.Sorted = true;
            this.listBoxSourceWordForms.TabIndex = 1;
            this.listBoxSourceWordForms.SelectedIndexChanged += new System.EventHandler(this.listBoxSourceWordForms_SelectedIndexChanged);
            // 
            // contextMenuStrip
            // 
            this.contextMenuStrip.ImageScalingSize = new System.Drawing.Size(24, 24);
            this.contextMenuStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.deleteToolStripMenuItem,
            this.editToolStripMenuItem});
            this.contextMenuStrip.Name = "contextMenuStrip";
            this.contextMenuStrip.Size = new System.Drawing.Size(135, 68);
            this.contextMenuStrip.Opening += new System.ComponentModel.CancelEventHandler(this.contextMenuStrip_Opening);
            // 
            // deleteToolStripMenuItem
            // 
            this.deleteToolStripMenuItem.Name = "deleteToolStripMenuItem";
            this.deleteToolStripMenuItem.Size = new System.Drawing.Size(134, 32);
            this.deleteToolStripMenuItem.Text = "&Delete";
            this.deleteToolStripMenuItem.ToolTipText = "Click to delete the selected word";
            this.deleteToolStripMenuItem.Click += new System.EventHandler(this.deleteToolStripMenuItem_Click);
            // 
            // editToolStripMenuItem
            // 
            this.editToolStripMenuItem.Name = "editToolStripMenuItem";
            this.editToolStripMenuItem.Size = new System.Drawing.Size(134, 32);
            this.editToolStripMenuItem.Text = "&Edit";
            this.editToolStripMenuItem.ToolTipText = "Click to edit the selected word";
            this.editToolStripMenuItem.Click += new System.EventHandler(this.editToolStripMenuItem_Click);
            // 
            // buttonOK
            // 
            this.buttonOK.Enabled = false;
            this.buttonOK.Location = new System.Drawing.Point(317, 551);
            this.buttonOK.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.buttonOK.Name = "buttonOK";
            this.buttonOK.Size = new System.Drawing.Size(112, 35);
            this.buttonOK.TabIndex = 0;
            this.buttonOK.Text = "&Return";
            this.buttonOK.UseVisualStyleBackColor = true;
            this.buttonOK.Click += new System.EventHandler(this.buttonOK_Click);
            // 
            // textBoxFilter
            // 
            this.textBoxFilter.Dock = System.Windows.Forms.DockStyle.Fill;
            this.textBoxFilter.Location = new System.Drawing.Point(4, 50);
            this.textBoxFilter.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.textBoxFilter.Name = "textBoxFilter";
            this.textBoxFilter.Size = new System.Drawing.Size(305, 26);
            this.textBoxFilter.TabIndex = 0;
            this.toolTip.SetToolTip(this.textBoxFilter, "Enter some text here to filter the knowledgebase in the list below");
            this.textBoxFilter.TextChanged += new System.EventHandler(this.textBoxFilter_TextChanged);
            // 
            // buttonAddNewSourceWord
            // 
            this.buttonAddNewSourceWord.Location = new System.Drawing.Point(4, 506);
            this.buttonAddNewSourceWord.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.buttonAddNewSourceWord.Name = "buttonAddNewSourceWord";
            this.buttonAddNewSourceWord.Size = new System.Drawing.Size(170, 35);
            this.buttonAddNewSourceWord.TabIndex = 3;
            this.buttonAddNewSourceWord.Text = "Add new";
            this.buttonAddNewSourceWord.UseVisualStyleBackColor = true;
            this.buttonAddNewSourceWord.Click += new System.EventHandler(this.buttonAddNewSourceWord_Click);
            // 
            // buttonOtherCommands
            // 
            this.buttonOtherCommands.Anchor = System.Windows.Forms.AnchorStyles.Right;
            this.buttonOtherCommands.ContextMenuStrip = this.contextMenuStrip;
            this.buttonOtherCommands.Font = new System.Drawing.Font("Segoe MDL2 Assets", 16F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.buttonOtherCommands.Location = new System.Drawing.Point(821, 3);
            this.buttonOtherCommands.Name = "buttonOtherCommands";
            this.buttonOtherCommands.Size = new System.Drawing.Size(46, 39);
            this.buttonOtherCommands.TabIndex = 6;
            this.buttonOtherCommands.Text = "";
            this.toolTip.SetToolTip(this.buttonOtherCommands, "Click here for other operations on an Adapt It Knowledgebase");
            this.buttonOtherCommands.UseVisualStyleBackColor = true;
            this.buttonOtherCommands.MouseDown += new System.Windows.Forms.MouseEventHandler(this.buttonOtherCommands_MouseDown);
            // 
            // progressBar
            // 
            this.progressBar.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.progressBar.Location = new System.Drawing.Point(0, 615);
            this.progressBar.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.progressBar.Name = "progressBar";
            this.progressBar.Size = new System.Drawing.Size(896, 10);
            this.progressBar.TabIndex = 3;
            this.progressBar.Visible = false;
            // 
            // contextMenuStripOtherCommands
            // 
            this.contextMenuStripOtherCommands.ImageScalingSize = new System.Drawing.Size(24, 24);
            this.contextMenuStripOtherCommands.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.createReversalProjectToolStripMenuItem,
            this.mergeProjectsToolStripMenuItem});
            this.contextMenuStripOtherCommands.Name = "contextMenuStrip";
            this.contextMenuStripOtherCommands.Size = new System.Drawing.Size(263, 68);
            // 
            // createReversalProjectToolStripMenuItem
            // 
            this.createReversalProjectToolStripMenuItem.Name = "createReversalProjectToolStripMenuItem";
            this.createReversalProjectToolStripMenuItem.Size = new System.Drawing.Size(262, 32);
            this.createReversalProjectToolStripMenuItem.Text = "&Create Reversal Project";
            this.createReversalProjectToolStripMenuItem.ToolTipText = "Click this menu to create a project for the reverse direction";
            this.createReversalProjectToolStripMenuItem.Click += new System.EventHandler(this.CreateReversalProjectToolStripMenuItemClick);
            // 
            // mergeProjectsToolStripMenuItem
            // 
            this.mergeProjectsToolStripMenuItem.Name = "mergeProjectsToolStripMenuItem";
            this.mergeProjectsToolStripMenuItem.Size = new System.Drawing.Size(262, 32);
            this.mergeProjectsToolStripMenuItem.Text = "&Merge Projects";
            this.mergeProjectsToolStripMenuItem.ToolTipText = "Click this menu to merge another project into this project";
            this.mergeProjectsToolStripMenuItem.Click += new System.EventHandler(this.mergeProjectsToolStripMenuItem_Click);
            // 
            // targetFormDisplayControl
            // 
            this.targetFormDisplayControl.CallToDeleteSourceWord = null;
            this.targetFormDisplayControl.CallToSetModified = null;
            this.tableLayoutPanel.SetColumnSpan(this.targetFormDisplayControl, 2);
            this.targetFormDisplayControl.Dock = System.Windows.Forms.DockStyle.Fill;
            this.targetFormDisplayControl.Location = new System.Drawing.Point(319, 53);
            this.targetFormDisplayControl.Margin = new System.Windows.Forms.Padding(6, 8, 6, 8);
            this.targetFormDisplayControl.Name = "targetFormDisplayControl";
            this.tableLayoutPanel.SetRowSpan(this.targetFormDisplayControl, 3);
            this.targetFormDisplayControl.Size = new System.Drawing.Size(545, 485);
            this.targetFormDisplayControl.TabIndex = 2;
            this.targetFormDisplayControl.TargetWordFont = null;
            this.targetFormDisplayControl.TargetWordRightToLeft = System.Windows.Forms.RightToLeft.No;
            this.targetFormDisplayControl.Load += new System.EventHandler(this.targetFormDisplayControl_Load);
            // 
            // ViewSourceFormsForm
            // 
            this.AcceptButton = this.buttonOK;
            this.AutoScaleDimensions = new System.Drawing.SizeF(9F, 20F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.CancelButton = this.buttonCancel;
            this.ClientSize = new System.Drawing.Size(896, 625);
            this.Controls.Add(this.progressBar);
            this.Controls.Add(this.tableLayoutPanel);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.Name = "ViewSourceFormsForm";
            this.Text = "View Knowledge Base";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.ViewSourceFormsForm_FormClosing);
            this.tableLayoutPanel.ResumeLayout(false);
            this.tableLayoutPanel.PerformLayout();
            this.contextMenuStrip.ResumeLayout(false);
            this.contextMenuStripOtherCommands.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel;
        private System.Windows.Forms.Button buttonOK;
        private System.Windows.Forms.Button buttonCancel;
        private System.Windows.Forms.ListBox listBoxSourceWordForms;
        private TargetFormDisplayControl targetFormDisplayControl;
        private System.Windows.Forms.ContextMenuStrip contextMenuStrip;
        private System.Windows.Forms.ToolStripMenuItem deleteToolStripMenuItem;
        private System.Windows.Forms.TextBox textBoxFilter;
        private System.Windows.Forms.Button buttonAddNewSourceWord;
        private System.Windows.Forms.ToolStripMenuItem editToolStripMenuItem;
        private System.Windows.Forms.ToolTip toolTip;
        private System.Windows.Forms.ProgressBar progressBar;
		private System.Windows.Forms.Label labelFilter;
		private System.Windows.Forms.Button buttonOtherCommands;
		private System.Windows.Forms.ContextMenuStrip contextMenuStripOtherCommands;
		private System.Windows.Forms.ToolStripMenuItem createReversalProjectToolStripMenuItem;
		private System.Windows.Forms.ToolStripMenuItem mergeProjectsToolStripMenuItem;
	}
}
