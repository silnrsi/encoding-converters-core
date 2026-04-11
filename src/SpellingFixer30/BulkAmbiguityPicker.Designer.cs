namespace SpellingFixer30
{
    partial class BulkAmbiguityPicker
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
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle1 = new System.Windows.Forms.DataGridViewCellStyle();
            this.tableLayoutPanel = new System.Windows.Forms.TableLayoutPanel();
            this.dataGridViewCorrectFormPicker = new System.Windows.Forms.DataGridView();
            this.ColumnVernacularLhs = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.ColumnRomanizedLhs = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.ColumnCountLhs = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.ColumnSelector = new System.Windows.Forms.DataGridViewButtonColumn();
            this.ColumnVernacularRhs = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.ColumnRomanizedRhs = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.ColumnCountRhs = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.progressBar = new System.Windows.Forms.ProgressBar();
            this.labelInconsistencies = new System.Windows.Forms.Label();
            this.backgroundWorker = new System.ComponentModel.BackgroundWorker();
            this.helpProvider = new System.Windows.Forms.HelpProvider();
            this.menuStrip = new System.Windows.Forms.MenuStrip();
            this.fileToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.saveToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.saveAndExitToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator1 = new System.Windows.Forms.ToolStripSeparator();
            this.exitToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.viewToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.showTransliterationToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.showCountToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator2 = new System.Windows.Forms.ToolStripSeparator();
            this.setCorrectSpellingColorToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.setIncorrectSpellingColorToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.colorDialog = new System.Windows.Forms.ColorDialog();
            this.tableLayoutPanel.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridViewCorrectFormPicker)).BeginInit();
            this.menuStrip.SuspendLayout();
            this.SuspendLayout();
            // 
            // tableLayoutPanel
            // 
            this.tableLayoutPanel.ColumnCount = 2;
            this.tableLayoutPanel.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 20F));
            this.tableLayoutPanel.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 20F));
            this.tableLayoutPanel.Controls.Add(this.dataGridViewCorrectFormPicker, 0, 0);
            this.tableLayoutPanel.Controls.Add(this.progressBar, 1, 1);
            this.tableLayoutPanel.Controls.Add(this.labelInconsistencies, 0, 1);
            this.tableLayoutPanel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanel.Location = new System.Drawing.Point(0, 24);
            this.tableLayoutPanel.Name = "tableLayoutPanel";
            this.tableLayoutPanel.RowCount = 2;
            this.tableLayoutPanel.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel.Size = new System.Drawing.Size(567, 306);
            this.tableLayoutPanel.TabIndex = 0;
            // 
            // dataGridViewCorrectFormPicker
            // 
            this.dataGridViewCorrectFormPicker.AllowUserToAddRows = false;
            this.dataGridViewCorrectFormPicker.AllowUserToDeleteRows = false;
            this.dataGridViewCorrectFormPicker.AllowUserToOrderColumns = true;
            this.dataGridViewCorrectFormPicker.AllowUserToResizeRows = false;
            this.dataGridViewCorrectFormPicker.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.Fill;
            this.dataGridViewCorrectFormPicker.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dataGridViewCorrectFormPicker.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.ColumnVernacularLhs,
            this.ColumnRomanizedLhs,
            this.ColumnCountLhs,
            this.ColumnSelector,
            this.ColumnVernacularRhs,
            this.ColumnRomanizedRhs,
            this.ColumnCountRhs});
            this.tableLayoutPanel.SetColumnSpan(this.dataGridViewCorrectFormPicker, 2);
            this.dataGridViewCorrectFormPicker.Dock = System.Windows.Forms.DockStyle.Fill;
            this.dataGridViewCorrectFormPicker.EditMode = System.Windows.Forms.DataGridViewEditMode.EditProgrammatically;
            this.dataGridViewCorrectFormPicker.Location = new System.Drawing.Point(3, 3);
            this.dataGridViewCorrectFormPicker.Name = "dataGridViewCorrectFormPicker";
            this.dataGridViewCorrectFormPicker.RowHeadersVisible = false;
            this.dataGridViewCorrectFormPicker.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.helpProvider.SetShowHelp(this.dataGridViewCorrectFormPicker, true);
            this.dataGridViewCorrectFormPicker.Size = new System.Drawing.Size(561, 271);
            this.dataGridViewCorrectFormPicker.TabIndex = 2;
            this.dataGridViewCorrectFormPicker.CellMouseUp += new System.Windows.Forms.DataGridViewCellMouseEventHandler(this.dataGridViewCorrectFormPicker_CellMouseUp);
            this.dataGridViewCorrectFormPicker.ColumnDisplayIndexChanged += new System.Windows.Forms.DataGridViewColumnEventHandler(this.dataGridViewCorrectFormPicker_ColumnDisplayIndexChanged);
            this.dataGridViewCorrectFormPicker.SelectionChanged += new System.EventHandler(this.dataGridViewCorrectFormPicker_SelectionChanged);
            this.dataGridViewCorrectFormPicker.PreviewKeyDown += new System.Windows.Forms.PreviewKeyDownEventHandler(this.dataGridViewCorrectFormPicker_PreviewKeyDown);
            this.dataGridViewCorrectFormPicker.CellMouseMove += dataGridViewCorrectFormPicker_CellMouseMove;
            this.dataGridViewCorrectFormPicker.MouseLeave += new System.EventHandler(this.dataGridViewCorrectFormPicker_MouseLeave);
            // 
            // ColumnVernacularLhs
            // 
            this.ColumnVernacularLhs.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.AllCells;
            this.ColumnVernacularLhs.Frozen = true;
            this.ColumnVernacularLhs.HeaderText = "Vernacular";
            this.ColumnVernacularLhs.Name = "ColumnVernacularLhs";
            this.ColumnVernacularLhs.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable;
            this.ColumnVernacularLhs.ToolTipText = "Vernacular for left-hand side";
            this.ColumnVernacularLhs.Width = 64;
            // 
            // ColumnRomanizedLhs
            // 
            this.ColumnRomanizedLhs.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.AllCells;
            this.ColumnRomanizedLhs.Frozen = true;
            this.ColumnRomanizedLhs.HeaderText = "Transliteration";
            this.ColumnRomanizedLhs.Name = "ColumnRomanizedLhs";
            this.ColumnRomanizedLhs.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable;
            this.ColumnRomanizedLhs.ToolTipText = "Transliteration for left-hand side";
            this.ColumnRomanizedLhs.Width = 79;
            // 
            // ColumnCountLhs
            // 
            this.ColumnCountLhs.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.AllCells;
            this.ColumnCountLhs.Frozen = true;
            this.ColumnCountLhs.HeaderText = "Count";
            this.ColumnCountLhs.Name = "ColumnCountLhs";
            this.ColumnCountLhs.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable;
            this.ColumnCountLhs.ToolTipText = "Count for left-hand side";
            this.ColumnCountLhs.Width = 41;
            // 
            // ColumnSelector
            // 
            this.ColumnSelector.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.ColumnHeader;
            dataGridViewCellStyle1.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleCenter;
            dataGridViewCellStyle1.Font = new System.Drawing.Font("Times New Roman", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(177)));
            this.ColumnSelector.DefaultCellStyle = dataGridViewCellStyle1;
            this.ColumnSelector.HeaderText = "Click if both correct";
            this.ColumnSelector.Name = "ColumnSelector";
            this.ColumnSelector.ToolTipText = "Click on the cells in this column to indicate that both words valid words";
            this.ColumnSelector.Width = 94;
            // 
            // ColumnVernacularRhs
            // 
            this.ColumnVernacularRhs.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.AllCells;
            this.ColumnVernacularRhs.HeaderText = "Vernacular";
            this.ColumnVernacularRhs.Name = "ColumnVernacularRhs";
            this.ColumnVernacularRhs.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable;
            this.ColumnVernacularRhs.ToolTipText = "Vernacular for right-hand side";
            this.ColumnVernacularRhs.Width = 64;
            // 
            // ColumnRomanizedRhs
            // 
            this.ColumnRomanizedRhs.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.AllCells;
            this.ColumnRomanizedRhs.HeaderText = "Transliteration";
            this.ColumnRomanizedRhs.Name = "ColumnRomanizedRhs";
            this.ColumnRomanizedRhs.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable;
            this.ColumnRomanizedRhs.ToolTipText = "Transliteration for right-hand side";
            this.ColumnRomanizedRhs.Width = 79;
            // 
            // ColumnCountRhs
            // 
            this.ColumnCountRhs.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.AllCells;
            this.ColumnCountRhs.HeaderText = "Count";
            this.ColumnCountRhs.Name = "ColumnCountRhs";
            this.ColumnCountRhs.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable;
            this.ColumnCountRhs.ToolTipText = "Count for right-hand side";
            this.ColumnCountRhs.Width = 41;
            // 
            // progressBar
            // 
            this.progressBar.Dock = System.Windows.Forms.DockStyle.Fill;
            this.progressBar.Location = new System.Drawing.Point(286, 280);
            this.progressBar.Name = "progressBar";
            this.progressBar.Size = new System.Drawing.Size(278, 23);
            this.progressBar.TabIndex = 4;
            // 
            // labelInconsistencies
            // 
            this.labelInconsistencies.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.labelInconsistencies.AutoSize = true;
            this.labelInconsistencies.Location = new System.Drawing.Point(3, 285);
            this.labelInconsistencies.Name = "labelInconsistencies";
            this.labelInconsistencies.Size = new System.Drawing.Size(0, 13);
            this.labelInconsistencies.TabIndex = 5;
            // 
            // backgroundWorker
            // 
            this.backgroundWorker.WorkerReportsProgress = true;
            this.backgroundWorker.WorkerSupportsCancellation = true;
#if UseBackgroundWorker
            this.backgroundWorker.DoWork += new System.ComponentModel.DoWorkEventHandler(this.backgroundWorker_DoWork);
            this.backgroundWorker.ProgressChanged += new System.ComponentModel.ProgressChangedEventHandler(this.backgroundWorker_ProgressChanged);
            this.backgroundWorker.RunWorkerCompleted += new System.ComponentModel.RunWorkerCompletedEventHandler(this.backgroundWorker_RunWorkerCompleted);
#endif
            // 
            // menuStrip
            // 
            this.menuStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.fileToolStripMenuItem,
            this.viewToolStripMenuItem});
            this.menuStrip.Location = new System.Drawing.Point(0, 0);
            this.menuStrip.Name = "menuStrip";
            this.menuStrip.Size = new System.Drawing.Size(567, 24);
            this.menuStrip.TabIndex = 1;
            this.menuStrip.Text = "menuStrip1";
            // 
            // fileToolStripMenuItem
            // 
            this.fileToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.saveToolStripMenuItem,
            this.saveAndExitToolStripMenuItem,
            this.toolStripSeparator1,
            this.exitToolStripMenuItem});
            this.fileToolStripMenuItem.Name = "fileToolStripMenuItem";
            this.fileToolStripMenuItem.Size = new System.Drawing.Size(37, 20);
            this.fileToolStripMenuItem.Text = "&File";
            // 
            // saveToolStripMenuItem
            // 
            this.saveToolStripMenuItem.Name = "saveToolStripMenuItem";
            this.saveToolStripMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.S)));
            this.saveToolStripMenuItem.Size = new System.Drawing.Size(223, 22);
            this.saveToolStripMenuItem.Text = "&Save";
            this.saveToolStripMenuItem.ToolTipText = "Save the selections that you\'ve made and remove them from the table";
            this.saveToolStripMenuItem.Click += new System.EventHandler(this.saveToolStripMenuItem_Click);
            // 
            // saveAndExitToolStripMenuItem
            // 
            this.saveAndExitToolStripMenuItem.Name = "saveAndExitToolStripMenuItem";
            this.saveAndExitToolStripMenuItem.Size = new System.Drawing.Size(223, 22);
            this.saveAndExitToolStripMenuItem.Text = "Save and E&xit";
            this.saveAndExitToolStripMenuItem.ToolTipText = "Save the selections that you\'ve made and dismiss this dialog box";
            this.saveAndExitToolStripMenuItem.Click += new System.EventHandler(this.saveAndExitToolStripMenuItem_Click);
            // 
            // toolStripSeparator1
            // 
            this.toolStripSeparator1.Name = "toolStripSeparator1";
            this.toolStripSeparator1.Size = new System.Drawing.Size(220, 6);
            // 
            // exitToolStripMenuItem
            // 
            this.exitToolStripMenuItem.Name = "exitToolStripMenuItem";
            this.exitToolStripMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Alt | System.Windows.Forms.Keys.F4)));
            this.exitToolStripMenuItem.Size = new System.Drawing.Size(223, 22);
            this.exitToolStripMenuItem.Text = "&Exit (without saving)";
            this.exitToolStripMenuItem.ToolTipText = "Dismiss the dialog box without saving the selections that you\'ve made";
            this.exitToolStripMenuItem.Click += new System.EventHandler(this.exitToolStripMenuItem_Click);
            // 
            // viewToolStripMenuItem
            // 
            this.viewToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.showTransliterationToolStripMenuItem,
            this.showCountToolStripMenuItem,
            this.toolStripSeparator2,
            this.setCorrectSpellingColorToolStripMenuItem,
            this.setIncorrectSpellingColorToolStripMenuItem});
            this.viewToolStripMenuItem.Name = "viewToolStripMenuItem";
            this.viewToolStripMenuItem.Size = new System.Drawing.Size(44, 20);
            this.viewToolStripMenuItem.Text = "&View";
            // 
            // showTransliterationToolStripMenuItem
            // 
            this.showTransliterationToolStripMenuItem.Checked = true;
            this.showTransliterationToolStripMenuItem.CheckOnClick = true;
            this.showTransliterationToolStripMenuItem.CheckState = System.Windows.Forms.CheckState.Checked;
            this.showTransliterationToolStripMenuItem.Name = "showTransliterationToolStripMenuItem";
            this.showTransliterationToolStripMenuItem.Size = new System.Drawing.Size(217, 22);
            this.showTransliterationToolStripMenuItem.Text = "Show &Transliteration";
            this.showTransliterationToolStripMenuItem.CheckStateChanged += new System.EventHandler(this.showTransliterationToolStripMenuItem_CheckStateChanged);
            // 
            // showCountToolStripMenuItem
            // 
            this.showCountToolStripMenuItem.Checked = true;
            this.showCountToolStripMenuItem.CheckOnClick = true;
            this.showCountToolStripMenuItem.CheckState = System.Windows.Forms.CheckState.Checked;
            this.showCountToolStripMenuItem.Name = "showCountToolStripMenuItem";
            this.showCountToolStripMenuItem.Size = new System.Drawing.Size(217, 22);
            this.showCountToolStripMenuItem.Text = "Show &Count";
            this.showCountToolStripMenuItem.CheckStateChanged += new System.EventHandler(this.showCountToolStripMenuItem_CheckStateChanged);
            // 
            // toolStripSeparator2
            // 
            this.toolStripSeparator2.Name = "toolStripSeparator2";
            this.toolStripSeparator2.Size = new System.Drawing.Size(214, 6);
            // 
            // setCorrectSpellingColorToolStripMenuItem
            // 
            this.setCorrectSpellingColorToolStripMenuItem.Name = "setCorrectSpellingColorToolStripMenuItem";
            this.setCorrectSpellingColorToolStripMenuItem.Size = new System.Drawing.Size(217, 22);
            this.setCorrectSpellingColorToolStripMenuItem.Text = "&Set Correct Spelling Color";
            this.setCorrectSpellingColorToolStripMenuItem.Click += new System.EventHandler(this.setCorrectSpellingColorToolStripMenuItem_Click);
            // 
            // setIncorrectSpellingColorToolStripMenuItem
            // 
            this.setIncorrectSpellingColorToolStripMenuItem.Name = "setIncorrectSpellingColorToolStripMenuItem";
            this.setIncorrectSpellingColorToolStripMenuItem.Size = new System.Drawing.Size(217, 22);
            this.setIncorrectSpellingColorToolStripMenuItem.Text = "Set &Incorrect Spelling Color";
            this.setIncorrectSpellingColorToolStripMenuItem.Click += new System.EventHandler(this.setIncorrectSpellingColorToolStripMenuItem_Click);
            // 
            // colorDialog
            // 
            this.colorDialog.SolidColorOnly = true;
            // 
            // BulkAmbiguityPicker
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.AutoSize = true;
            this.ClientSize = new System.Drawing.Size(567, 330);
            this.Controls.Add(this.tableLayoutPanel);
            this.Controls.Add(this.menuStrip);
            this.HelpButton = true;
            this.MainMenuStrip = this.menuStrip;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "BulkAmbiguityPicker";
            this.Text = "Choose Correct Spelling";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.BulkAmbiguityPicker_FormClosing);
            this.tableLayoutPanel.ResumeLayout(false);
            this.tableLayoutPanel.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridViewCorrectFormPicker)).EndInit();
            this.menuStrip.ResumeLayout(false);
            this.menuStrip.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

#endregion

        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel;
        private System.Windows.Forms.DataGridView dataGridViewCorrectFormPicker;
        private System.ComponentModel.BackgroundWorker backgroundWorker;
        private System.Windows.Forms.ProgressBar progressBar;
        private System.Windows.Forms.HelpProvider helpProvider;
        private System.Windows.Forms.MenuStrip menuStrip;
        private System.Windows.Forms.ToolStripMenuItem fileToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem saveToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem saveAndExitToolStripMenuItem;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator1;
        private System.Windows.Forms.ToolStripMenuItem exitToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem viewToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem showTransliterationToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem showCountToolStripMenuItem;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator2;
        private System.Windows.Forms.ToolStripMenuItem setCorrectSpellingColorToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem setIncorrectSpellingColorToolStripMenuItem;
        private System.Windows.Forms.ColorDialog colorDialog;
        private System.Windows.Forms.Label labelInconsistencies;
        private System.Windows.Forms.DataGridViewTextBoxColumn ColumnVernacularLhs;
        private System.Windows.Forms.DataGridViewTextBoxColumn ColumnRomanizedLhs;
        private System.Windows.Forms.DataGridViewTextBoxColumn ColumnCountLhs;
        private System.Windows.Forms.DataGridViewButtonColumn ColumnSelector;
        private System.Windows.Forms.DataGridViewTextBoxColumn ColumnVernacularRhs;
        private System.Windows.Forms.DataGridViewTextBoxColumn ColumnRomanizedRhs;
        private System.Windows.Forms.DataGridViewTextBoxColumn ColumnCountRhs;
    }
}