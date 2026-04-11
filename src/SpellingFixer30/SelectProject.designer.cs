namespace SpellingFixer30
{
    partial class SelectProject
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(SelectProject));
            this.checkedListBoxProjectNames = new System.Windows.Forms.CheckedListBox();
            this.contextMenuStrip = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.editToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.deleteToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator1 = new System.Windows.Forms.ToolStripSeparator();
            this.deleteAllToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator2 = new System.Windows.Forms.ToolStripSeparator();
            this.resetWordsToCheckListToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.editDictionaryToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.editListOfSpellingFixToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.tableLayoutPanel = new System.Windows.Forms.TableLayoutPanel();
            this.buttonOK = new System.Windows.Forms.Button();
            this.buttonCancel = new System.Windows.Forms.Button();
            this.buttonAddNewProject = new System.Windows.Forms.Button();
            this.buttonBrowseForProject = new System.Windows.Forms.Button();
            this.openFileDialog = new System.Windows.Forms.OpenFileDialog();
            this.helpProvider = new System.Windows.Forms.HelpProvider();
            this.contextMenuStrip.SuspendLayout();
            this.tableLayoutPanel.SuspendLayout();
            this.SuspendLayout();
            // 
            // checkedListBoxProjectNames
            // 
            this.checkedListBoxProjectNames.CheckOnClick = true;
            this.tableLayoutPanel.SetColumnSpan(this.checkedListBoxProjectNames, 5);
            this.checkedListBoxProjectNames.ContextMenuStrip = this.contextMenuStrip;
            this.checkedListBoxProjectNames.Dock = System.Windows.Forms.DockStyle.Fill;
            this.checkedListBoxProjectNames.FormattingEnabled = true;
            this.helpProvider.SetHelpString(this.checkedListBoxProjectNames, resources.GetString("checkedListBoxProjectNames.HelpString"));
            this.checkedListBoxProjectNames.Location = new System.Drawing.Point(3, 3);
            this.checkedListBoxProjectNames.Name = "checkedListBoxProjectNames";
            this.helpProvider.SetShowHelp(this.checkedListBoxProjectNames, true);
            this.checkedListBoxProjectNames.Size = new System.Drawing.Size(377, 184);
            this.checkedListBoxProjectNames.TabIndex = 0;
            this.checkedListBoxProjectNames.SelectedIndexChanged += new System.EventHandler(this.checkedListBoxProjectNames_SelectedIndexChanged);
            this.checkedListBoxProjectNames.MouseUp += new System.Windows.Forms.MouseEventHandler(this.checkedListBoxProjectNames_MouseUp);
            this.checkedListBoxProjectNames.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.checkedListBoxProjectNames_KeyPress);
            // 
            // contextMenuStrip
            // 
            this.contextMenuStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.editToolStripMenuItem,
            this.deleteToolStripMenuItem,
            this.toolStripSeparator1,
            this.deleteAllToolStripMenuItem,
            this.toolStripSeparator2,
            this.resetWordsToCheckListToolStripMenuItem,
            this.editDictionaryToolStripMenuItem,
            this.editListOfSpellingFixToolStripMenuItem});
            this.contextMenuStrip.Name = "contextMenuStrip";
            this.contextMenuStrip.Size = new System.Drawing.Size(211, 148);
            // 
            // editToolStripMenuItem
            // 
            this.editToolStripMenuItem.Name = "editToolStripMenuItem";
            this.editToolStripMenuItem.Size = new System.Drawing.Size(210, 22);
            this.editToolStripMenuItem.Text = "&Edit";
            this.editToolStripMenuItem.ToolTipText = "Edit project settings";
            this.editToolStripMenuItem.Click += new System.EventHandler(this.editToolStripMenuItem_Click);
            // 
            // deleteToolStripMenuItem
            // 
            this.deleteToolStripMenuItem.Name = "deleteToolStripMenuItem";
            this.deleteToolStripMenuItem.Size = new System.Drawing.Size(210, 22);
            this.deleteToolStripMenuItem.Text = "&Delete";
            this.deleteToolStripMenuItem.ToolTipText = "Delete project";
            this.deleteToolStripMenuItem.Click += new System.EventHandler(this.deleteToolStripMenuItem_Click);
            // 
            // toolStripSeparator1
            // 
            this.toolStripSeparator1.Name = "toolStripSeparator1";
            this.toolStripSeparator1.Size = new System.Drawing.Size(207, 6);
            // 
            // deleteAllToolStripMenuItem
            // 
            this.deleteAllToolStripMenuItem.Name = "deleteAllToolStripMenuItem";
            this.deleteAllToolStripMenuItem.Size = new System.Drawing.Size(210, 22);
            this.deleteAllToolStripMenuItem.Text = "Delete &All";
            this.deleteAllToolStripMenuItem.ToolTipText = "Delete all projects";
            this.deleteAllToolStripMenuItem.Click += new System.EventHandler(this.deleteAllToolStripMenuItem_Click);
            // 
            // toolStripSeparator2
            // 
            this.toolStripSeparator2.Name = "toolStripSeparator2";
            this.toolStripSeparator2.Size = new System.Drawing.Size(207, 6);
            // 
            // resetWordsToCheckListToolStripMenuItem
            // 
            this.resetWordsToCheckListToolStripMenuItem.Name = "resetWordsToCheckListToolStripMenuItem";
            this.resetWordsToCheckListToolStripMenuItem.Size = new System.Drawing.Size(210, 22);
            this.resetWordsToCheckListToolStripMenuItem.Text = "&Reset list of Words to Check";
            this.resetWordsToCheckListToolStripMenuItem.ToolTipText = "Each project maintains a list of words to check for inconsistencies. This command" +
                " resets the list";
            this.resetWordsToCheckListToolStripMenuItem.Click += new System.EventHandler(this.resetWordsToCheckListToolStripMenuItem_Click);
            // 
            // editDictionaryToolStripMenuItem
            // 
            this.editDictionaryToolStripMenuItem.Name = "editDictionaryToolStripMenuItem";
            this.editDictionaryToolStripMenuItem.Size = new System.Drawing.Size(210, 22);
            this.editDictionaryToolStripMenuItem.Text = "Edit Di&ctionary";
            this.editDictionaryToolStripMenuItem.ToolTipText = "Edit the dictionary of known correctly-spelled words";
            this.editDictionaryToolStripMenuItem.Click += new System.EventHandler(this.editDictionaryToolStripMenuItem_Click);
            // 
            // editListOfSpellingFixToolStripMenuItem
            // 
            this.editListOfSpellingFixToolStripMenuItem.Name = "editListOfSpellingFixToolStripMenuItem";
            this.editListOfSpellingFixToolStripMenuItem.Size = new System.Drawing.Size(210, 22);
            this.editListOfSpellingFixToolStripMenuItem.Text = "Edit &Spelling Fixes";
            this.editListOfSpellingFixToolStripMenuItem.ToolTipText = "Edit the database of spelling fixes";
            this.editListOfSpellingFixToolStripMenuItem.Click += new System.EventHandler(this.editListOfSpellingFixToolStripMenuItem_Click);
            // 
            // tableLayoutPanel
            // 
            this.tableLayoutPanel.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.tableLayoutPanel.ColumnCount = 5;
            this.tableLayoutPanel.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.tableLayoutPanel.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.tableLayoutPanel.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.tableLayoutPanel.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.tableLayoutPanel.Controls.Add(this.buttonOK, 3, 1);
            this.tableLayoutPanel.Controls.Add(this.buttonCancel, 4, 1);
            this.tableLayoutPanel.Controls.Add(this.checkedListBoxProjectNames, 0, 0);
            this.tableLayoutPanel.Controls.Add(this.buttonAddNewProject, 0, 1);
            this.tableLayoutPanel.Controls.Add(this.buttonBrowseForProject, 1, 1);
            this.tableLayoutPanel.Location = new System.Drawing.Point(12, 12);
            this.tableLayoutPanel.Name = "tableLayoutPanel";
            this.tableLayoutPanel.RowCount = 2;
            this.tableLayoutPanel.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel.Size = new System.Drawing.Size(383, 233);
            this.tableLayoutPanel.TabIndex = 1;
            // 
            // buttonOK
            // 
            this.buttonOK.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.helpProvider.SetHelpString(this.buttonOK, "Click this button to select the checked project");
            this.buttonOK.Location = new System.Drawing.Point(224, 207);
            this.buttonOK.Name = "buttonOK";
            this.helpProvider.SetShowHelp(this.buttonOK, true);
            this.buttonOK.Size = new System.Drawing.Size(75, 23);
            this.buttonOK.TabIndex = 1;
            this.buttonOK.Text = "OK";
            this.buttonOK.UseVisualStyleBackColor = true;
            this.buttonOK.Click += new System.EventHandler(this.buttonOK_Click);
            // 
            // buttonCancel
            // 
            this.buttonCancel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.buttonCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.helpProvider.SetHelpString(this.buttonCancel, "Click this button to cancel this dialog without selecting a project");
            this.buttonCancel.Location = new System.Drawing.Point(305, 207);
            this.buttonCancel.Name = "buttonCancel";
            this.helpProvider.SetShowHelp(this.buttonCancel, true);
            this.buttonCancel.Size = new System.Drawing.Size(75, 23);
            this.buttonCancel.TabIndex = 2;
            this.buttonCancel.Text = "Cancel";
            this.buttonCancel.UseVisualStyleBackColor = true;
            this.buttonCancel.Click += new System.EventHandler(this.buttonCancel_Click);
            // 
            // buttonAddNewProject
            // 
            this.buttonAddNewProject.AutoSize = true;
            this.buttonAddNewProject.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.helpProvider.SetHelpString(this.buttonAddNewProject, "Click this button to add a new blank project");
            this.buttonAddNewProject.Location = new System.Drawing.Point(3, 207);
            this.buttonAddNewProject.Name = "buttonAddNewProject";
            this.helpProvider.SetShowHelp(this.buttonAddNewProject, true);
            this.buttonAddNewProject.Size = new System.Drawing.Size(97, 23);
            this.buttonAddNewProject.TabIndex = 3;
            this.buttonAddNewProject.Text = "Add New Project";
            this.buttonAddNewProject.UseVisualStyleBackColor = true;
            this.buttonAddNewProject.Click += new System.EventHandler(this.buttonAddNewProject_Click);
            // 
            // buttonBrowseForProject
            // 
            this.helpProvider.SetHelpString(this.buttonBrowseForProject, "Click this button to browse for an existing project to add to this machine (e.g. " +
                    "from a network location)");
            this.buttonBrowseForProject.Location = new System.Drawing.Point(106, 207);
            this.buttonBrowseForProject.Name = "buttonBrowseForProject";
            this.helpProvider.SetShowHelp(this.buttonBrowseForProject, true);
            this.buttonBrowseForProject.Size = new System.Drawing.Size(75, 23);
            this.buttonBrowseForProject.TabIndex = 4;
            this.buttonBrowseForProject.Text = "Browse";
            this.buttonBrowseForProject.UseVisualStyleBackColor = true;
            this.buttonBrowseForProject.Click += new System.EventHandler(this.buttonBrowseForProject_Click);
            // 
            // openFileDialog
            // 
            this.openFileDialog.DefaultExt = "csf";
            this.openFileDialog.Filter = "Consistent Spelling Fixer project files (*.csf)|*.csf";
            this.openFileDialog.Title = "Browse for Consistent Spell Fixer project file";
            // 
            // SelectProject
            // 
            this.AcceptButton = this.buttonOK;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.CancelButton = this.buttonCancel;
            this.ClientSize = new System.Drawing.Size(407, 257);
            this.Controls.Add(this.tableLayoutPanel);
            this.HelpButton = true;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "SelectProject";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Select Consistent SpellFixing Project";
            this.contextMenuStrip.ResumeLayout(false);
            this.tableLayoutPanel.ResumeLayout(false);
            this.tableLayoutPanel.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.CheckedListBox checkedListBoxProjectNames;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel;
        private System.Windows.Forms.Button buttonOK;
        private System.Windows.Forms.Button buttonCancel;
        private System.Windows.Forms.Button buttonAddNewProject;
		private System.Windows.Forms.ContextMenuStrip contextMenuStrip;
		private System.Windows.Forms.ToolStripMenuItem editToolStripMenuItem;
		private System.Windows.Forms.ToolStripMenuItem deleteToolStripMenuItem;
        private System.Windows.Forms.Button buttonBrowseForProject;
        private System.Windows.Forms.OpenFileDialog openFileDialog;
        private System.Windows.Forms.HelpProvider helpProvider;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator1;
        private System.Windows.Forms.ToolStripMenuItem deleteAllToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem resetWordsToCheckListToolStripMenuItem;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator2;
        private System.Windows.Forms.ToolStripMenuItem editDictionaryToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem editListOfSpellingFixToolStripMenuItem;

    }
}