namespace SilEncConverters40
{
    partial class AdaptItAutoConfigDialog
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(AdaptItAutoConfigDialog));
            this.tableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
            this.label1 = new System.Windows.Forms.Label();
            this.listBoxProjects = new System.Windows.Forms.ListBox();
            this.labelNormalizationConverter = new System.Windows.Forms.Label();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.radioButtonLegacy = new System.Windows.Forms.RadioButton();
            this.radioButtonUnicode = new System.Windows.Forms.RadioButton();
            this.textBoxNormalizationPath = new System.Windows.Forms.TextBox();
            this.buttonBrowseNormalizationMap = new System.Windows.Forms.Button();
            this.buttonCreateNewProject = new System.Windows.Forms.Button();
            this.buttonViewKb = new System.Windows.Forms.Button();
            this.labelFallbackDirection = new System.Windows.Forms.Label();
            this.checkBoxFallbackReverseDirection = new System.Windows.Forms.CheckBox();
            this.openFileDialogBrowse = new System.Windows.Forms.OpenFileDialog();
            this.tabControl.SuspendLayout();
            this.tabPageSetup.SuspendLayout();
            this.tableLayoutPanel1.SuspendLayout();
            this.groupBox1.SuspendLayout();
            this.SuspendLayout();
            // 
            // tabPageSetup
            // 
            this.tabPageSetup.Controls.Add(this.tableLayoutPanel1);
            // 
            // buttonApply
            // 
            this.helpProvider.SetHelpString(this.buttonApply, "Click this button to apply the configured values for this converter");
            this.helpProvider.SetShowHelp(this.buttonApply, true);
            // 
            // buttonCancel
            // 
            this.helpProvider.SetHelpString(this.buttonCancel, "Click this button to cancel this dialog");
            this.helpProvider.SetShowHelp(this.buttonCancel, true);
            // 
            // buttonOK
            // 
            this.helpProvider.SetHelpString(this.buttonOK, "Click this button to accept the configured values for this converter");
            this.helpProvider.SetShowHelp(this.buttonOK, true);
            // 
            // buttonSaveInRepository
            // 
            this.helpProvider.SetHelpString(this.buttonSaveInRepository, "\r\nClick to add this converter to the system repository permanently.\r\n    ");
            this.helpProvider.SetShowHelp(this.buttonSaveInRepository, true);
            // 
            // tableLayoutPanel1
            // 
            this.tableLayoutPanel1.ColumnCount = 3;
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.tableLayoutPanel1.Controls.Add(this.label1, 0, 1);
            this.tableLayoutPanel1.Controls.Add(this.listBoxProjects, 0, 2);
            this.tableLayoutPanel1.Controls.Add(this.labelNormalizationConverter, 0, 4);
            this.tableLayoutPanel1.Controls.Add(this.groupBox1, 0, 0);
            this.tableLayoutPanel1.Controls.Add(this.textBoxNormalizationPath, 1, 4);
            this.tableLayoutPanel1.Controls.Add(this.buttonBrowseNormalizationMap, 2, 4);
            this.tableLayoutPanel1.Controls.Add(this.buttonCreateNewProject, 1, 3);
            this.tableLayoutPanel1.Controls.Add(this.buttonViewKb, 0, 3);
            this.tableLayoutPanel1.Controls.Add(this.labelFallbackDirection, 0, 5);
            this.tableLayoutPanel1.Controls.Add(this.checkBoxFallbackReverseDirection, 1, 5);
            this.tableLayoutPanel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanel1.Location = new System.Drawing.Point(3, 3);
            this.tableLayoutPanel1.Name = "tableLayoutPanel1";
            this.tableLayoutPanel1.RowCount = 6;
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel1.Size = new System.Drawing.Size(596, 394);
            this.tableLayoutPanel1.TabIndex = 0;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(3, 59);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(48, 13);
            this.label1.TabIndex = 1;
            this.label1.Text = "Projects:";
            // 
            // listBoxProjects
            // 
            this.tableLayoutPanel1.SetColumnSpan(this.listBoxProjects, 3);
            this.listBoxProjects.Dock = System.Windows.Forms.DockStyle.Fill;
            this.listBoxProjects.FormattingEnabled = true;
            this.listBoxProjects.Location = new System.Drawing.Point(3, 75);
            this.listBoxProjects.Name = "listBoxProjects";
            this.listBoxProjects.Size = new System.Drawing.Size(590, 235);
            this.listBoxProjects.TabIndex = 2;
            this.listBoxProjects.SelectedIndexChanged += new System.EventHandler(this.listBoxProjects_SelectedIndexChanged);
            // 
            // labelNormalizationConverter
            // 
            this.labelNormalizationConverter.Anchor = System.Windows.Forms.AnchorStyles.Right;
            this.labelNormalizationConverter.AutoSize = true;
            this.labelNormalizationConverter.Location = new System.Drawing.Point(3, 350);
            this.labelNormalizationConverter.Name = "labelNormalizationConverter";
            this.labelNormalizationConverter.Size = new System.Drawing.Size(140, 13);
            this.labelNormalizationConverter.TabIndex = 3;
            this.labelNormalizationConverter.Text = "Optional Fallback converter:";
            // 
            // groupBox1
            // 
            this.groupBox1.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.groupBox1.AutoSize = true;
            this.groupBox1.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.tableLayoutPanel1.SetColumnSpan(this.groupBox1, 3);
            this.groupBox1.Controls.Add(this.radioButtonLegacy);
            this.groupBox1.Controls.Add(this.radioButtonUnicode);
            this.groupBox1.Location = new System.Drawing.Point(183, 3);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(230, 53);
            this.groupBox1.TabIndex = 0;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "AdaptIt Version";
            // 
            // radioButtonLegacy
            // 
            this.radioButtonLegacy.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.radioButtonLegacy.AutoSize = true;
            this.radioButtonLegacy.Location = new System.Drawing.Point(139, 17);
            this.radioButtonLegacy.Name = "radioButtonLegacy";
            this.radioButtonLegacy.Size = new System.Drawing.Size(85, 17);
            this.radioButtonLegacy.TabIndex = 1;
            this.radioButtonLegacy.TabStop = true;
            this.radioButtonLegacy.Text = "Legacy/Ansi";
            this.radioButtonLegacy.UseVisualStyleBackColor = true;
            this.radioButtonLegacy.Click += new System.EventHandler(this.radioButtonLegacy_Click);
            // 
            // radioButtonUnicode
            // 
            this.radioButtonUnicode.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.radioButtonUnicode.AutoSize = true;
            this.radioButtonUnicode.Location = new System.Drawing.Point(6, 17);
            this.radioButtonUnicode.Name = "radioButtonUnicode";
            this.radioButtonUnicode.Size = new System.Drawing.Size(127, 17);
            this.radioButtonUnicode.TabIndex = 0;
            this.radioButtonUnicode.TabStop = true;
            this.radioButtonUnicode.Text = "Non-Roman/Unicode";
            this.radioButtonUnicode.UseVisualStyleBackColor = true;
            this.radioButtonUnicode.Click += new System.EventHandler(this.radioButtonUnicode_Click);
            // 
            // textBoxNormalizationPath
            // 
            this.textBoxNormalizationPath.Dock = System.Windows.Forms.DockStyle.Fill;
            this.helpProvider.SetHelpString(this.textBoxNormalizationPath, resources.GetString("textBoxNormalizationPath.HelpString"));
            this.textBoxNormalizationPath.Location = new System.Drawing.Point(149, 345);
            this.textBoxNormalizationPath.Name = "textBoxNormalizationPath";
            this.helpProvider.SetShowHelp(this.textBoxNormalizationPath, true);
            this.textBoxNormalizationPath.Size = new System.Drawing.Size(414, 20);
            this.textBoxNormalizationPath.TabIndex = 4;
            this.textBoxNormalizationPath.TextChanged += new System.EventHandler(this.textBoxNormalizationPath_TextChanged);
            // 
            // buttonBrowseNormalizationMap
            // 
            this.buttonBrowseNormalizationMap.Location = new System.Drawing.Point(569, 345);
            this.buttonBrowseNormalizationMap.Name = "buttonBrowseNormalizationMap";
            this.buttonBrowseNormalizationMap.Size = new System.Drawing.Size(24, 23);
            this.buttonBrowseNormalizationMap.TabIndex = 5;
            this.buttonBrowseNormalizationMap.Text = "...";
            this.buttonBrowseNormalizationMap.UseVisualStyleBackColor = true;
            this.buttonBrowseNormalizationMap.Click += new System.EventHandler(this.buttonBrowseNormalizationMap_Click);
            // 
            // buttonCreateNewProject
            // 
            this.buttonCreateNewProject.Location = new System.Drawing.Point(149, 316);
            this.buttonCreateNewProject.Name = "buttonCreateNewProject";
            this.buttonCreateNewProject.Size = new System.Drawing.Size(148, 23);
            this.buttonCreateNewProject.TabIndex = 6;
            this.buttonCreateNewProject.Text = "Create New Project";
            this.buttonCreateNewProject.UseVisualStyleBackColor = true;
            this.buttonCreateNewProject.Click += new System.EventHandler(this.ButtonCreateNewProjectClick);
            // 
            // buttonViewKb
            // 
            this.buttonViewKb.Enabled = false;
            this.buttonViewKb.Location = new System.Drawing.Point(3, 316);
            this.buttonViewKb.Name = "buttonViewKb";
            this.buttonViewKb.Size = new System.Drawing.Size(137, 23);
            this.buttonViewKb.TabIndex = 7;
            this.buttonViewKb.Text = "View Knowledgebase";
            this.buttonViewKb.UseVisualStyleBackColor = true;
            this.buttonViewKb.Click += new System.EventHandler(this.ButtonViewKbClick);
            // 
            // labelFallbackDirection
            // 
            this.labelFallbackDirection.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.labelFallbackDirection.AutoSize = true;
            this.labelFallbackDirection.Location = new System.Drawing.Point(3, 376);
            this.labelFallbackDirection.Name = "labelFallbackDirection";
            this.labelFallbackDirection.Size = new System.Drawing.Size(95, 13);
            this.labelFallbackDirection.TabIndex = 9;
            this.labelFallbackDirection.Text = "Fallback Direction:";
            this.labelFallbackDirection.Visible = false;
            // 
            // checkBoxFallbackReverseDirection
            // 
            this.checkBoxFallbackReverseDirection.AutoSize = true;
            this.helpProvider.SetHelpString(this.checkBoxFallbackReverseDirection, "Check this to have the fallback TECkit map run in the reverse direction (for bi-d" +
        "irectional maps)");
            this.checkBoxFallbackReverseDirection.Location = new System.Drawing.Point(149, 374);
            this.checkBoxFallbackReverseDirection.Name = "checkBoxFallbackReverseDirection";
            this.helpProvider.SetShowHelp(this.checkBoxFallbackReverseDirection, true);
            this.checkBoxFallbackReverseDirection.Size = new System.Drawing.Size(66, 17);
            this.checkBoxFallbackReverseDirection.TabIndex = 10;
            this.checkBoxFallbackReverseDirection.Text = "&Reverse";
            this.checkBoxFallbackReverseDirection.UseVisualStyleBackColor = true;
            this.checkBoxFallbackReverseDirection.Visible = false;
            this.checkBoxFallbackReverseDirection.CheckedChanged += new System.EventHandler(this.checkBoxFallbackReverseDirection_CheckedChanged);
            // 
            // openFileDialogBrowse
            // 
            this.openFileDialogBrowse.DefaultExt = "cct";
            this.openFileDialogBrowse.Filter = "TECkit Map Files (*.map;*.tec)|*.map;*.tec|CC Tables Files (*.cct)|*.cct\r\n";
            this.openFileDialogBrowse.Title = "Browse for CC Table";
            // 
            // AdaptItAutoConfigDialog
            // 
            this.ClientSize = new System.Drawing.Size(634, 479);
            this.Name = "AdaptItAutoConfigDialog";
            this.Controls.SetChildIndex(this.tabControl, 0);
            this.Controls.SetChildIndex(this.buttonApply, 0);
            this.Controls.SetChildIndex(this.buttonCancel, 0);
            this.Controls.SetChildIndex(this.buttonOK, 0);
            this.Controls.SetChildIndex(this.buttonSaveInRepository, 0);
            this.tabControl.ResumeLayout(false);
            this.tabPageSetup.ResumeLayout(false);
            this.tableLayoutPanel1.ResumeLayout(false);
            this.tableLayoutPanel1.PerformLayout();
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel1;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.RadioButton radioButtonUnicode;
        private System.Windows.Forms.RadioButton radioButtonLegacy;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.ListBox listBoxProjects;
        private System.Windows.Forms.Label labelNormalizationConverter;
        private System.Windows.Forms.TextBox textBoxNormalizationPath;
        private System.Windows.Forms.Button buttonBrowseNormalizationMap;
        private System.Windows.Forms.OpenFileDialog openFileDialogBrowse;
        private System.Windows.Forms.Button buttonCreateNewProject;
        private System.Windows.Forms.Button buttonViewKb;
        private System.Windows.Forms.Label labelFallbackDirection;
        private System.Windows.Forms.CheckBox checkBoxFallbackReverseDirection;
    }
}
