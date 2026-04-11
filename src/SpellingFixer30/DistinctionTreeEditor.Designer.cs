using SilEncConverters40;

namespace SpellingFixer30
{
    partial class DistinctionTreeEditor
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(DistinctionTreeEditor));
            this.tableLayoutPanel = new System.Windows.Forms.TableLayoutPanel();
            this.textBoxDescription = new System.Windows.Forms.TextBox();
            this.labelDescription = new System.Windows.Forms.Label();
            this.textBoxRuleName = new System.Windows.Forms.TextBox();
            this.labelRuleName = new System.Windows.Forms.Label();
            this.buttonCancel = new System.Windows.Forms.Button();
            this.buttonSave = new System.Windows.Forms.Button();
            this.labelSimplifyingRules = new System.Windows.Forms.Label();
            this.treeViewDistinctions = new System.Windows.Forms.TreeView();
            this.contextMenuStripFrom = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.addFromValueToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.labelChangeFrom = new System.Windows.Forms.Label();
            this.labelChangeInto = new System.Windows.Forms.Label();
            this.buttonTransliterate = new System.Windows.Forms.Button();
            this.helpProvider = new System.Windows.Forms.HelpProvider();
            this.ecTextBoxFrom = new EcTextBox();
            this.ecTextBoxTo = new EcTextBox();
            this.tableLayoutPanel.SuspendLayout();
            this.contextMenuStripFrom.SuspendLayout();
            this.SuspendLayout();
            // 
            // tableLayoutPanel
            // 
            this.tableLayoutPanel.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.tableLayoutPanel.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.tableLayoutPanel.ColumnCount = 3;
            this.tableLayoutPanel.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.tableLayoutPanel.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tableLayoutPanel.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tableLayoutPanel.Controls.Add(this.textBoxDescription, 1, 1);
            this.tableLayoutPanel.Controls.Add(this.labelDescription, 0, 1);
            this.tableLayoutPanel.Controls.Add(this.textBoxRuleName, 1, 0);
            this.tableLayoutPanel.Controls.Add(this.labelRuleName, 0, 0);
            this.tableLayoutPanel.Controls.Add(this.buttonCancel, 2, 6);
            this.tableLayoutPanel.Controls.Add(this.buttonSave, 1, 6);
            this.tableLayoutPanel.Controls.Add(this.labelSimplifyingRules, 0, 2);
            this.tableLayoutPanel.Controls.Add(this.treeViewDistinctions, 1, 2);
            this.tableLayoutPanel.Controls.Add(this.ecTextBoxFrom, 1, 5);
            this.tableLayoutPanel.Controls.Add(this.labelChangeFrom, 1, 4);
            this.tableLayoutPanel.Controls.Add(this.ecTextBoxTo, 2, 5);
            this.tableLayoutPanel.Controls.Add(this.labelChangeInto, 2, 4);
            this.tableLayoutPanel.Controls.Add(this.buttonTransliterate, 0, 5);
            this.tableLayoutPanel.Location = new System.Drawing.Point(12, 12);
            this.tableLayoutPanel.Name = "tableLayoutPanel";
            this.tableLayoutPanel.RowCount = 7;
            this.tableLayoutPanel.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 30F));
            this.tableLayoutPanel.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 30F));
            this.tableLayoutPanel.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel.Size = new System.Drawing.Size(415, 440);
            this.tableLayoutPanel.TabIndex = 0;
            // 
            // textBoxDescription
            // 
            this.tableLayoutPanel.SetColumnSpan(this.textBoxDescription, 2);
            this.textBoxDescription.Dock = System.Windows.Forms.DockStyle.Fill;
            this.helpProvider.SetHelpString(this.textBoxDescription, "This box shows the description of the distinction rule you are editing. ");
            this.textBoxDescription.Location = new System.Drawing.Point(93, 29);
            this.textBoxDescription.Name = "textBoxDescription";
            this.helpProvider.SetShowHelp(this.textBoxDescription, true);
            this.textBoxDescription.Size = new System.Drawing.Size(319, 20);
            this.textBoxDescription.TabIndex = 0;
            // 
            // labelDescription
            // 
            this.labelDescription.Anchor = System.Windows.Forms.AnchorStyles.Right;
            this.labelDescription.AutoSize = true;
            this.labelDescription.Location = new System.Drawing.Point(24, 32);
            this.labelDescription.Name = "labelDescription";
            this.labelDescription.Size = new System.Drawing.Size(63, 13);
            this.labelDescription.TabIndex = 3;
            this.labelDescription.Text = "Description:";
            // 
            // textBoxRuleName
            // 
            this.textBoxRuleName.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.tableLayoutPanel.SetColumnSpan(this.textBoxRuleName, 2);
            this.helpProvider.SetHelpString(this.textBoxRuleName, resources.GetString("textBoxRuleName.HelpString"));
            this.textBoxRuleName.Location = new System.Drawing.Point(93, 3);
            this.textBoxRuleName.Name = "textBoxRuleName";
            this.textBoxRuleName.ReadOnly = true;
            this.helpProvider.SetShowHelp(this.textBoxRuleName, true);
            this.textBoxRuleName.Size = new System.Drawing.Size(319, 20);
            this.textBoxRuleName.TabIndex = 2;
            // 
            // labelRuleName
            // 
            this.labelRuleName.Anchor = System.Windows.Forms.AnchorStyles.Right;
            this.labelRuleName.AutoSize = true;
            this.labelRuleName.Location = new System.Drawing.Point(24, 6);
            this.labelRuleName.Name = "labelRuleName";
            this.labelRuleName.Size = new System.Drawing.Size(63, 13);
            this.labelRuleName.TabIndex = 3;
            this.labelRuleName.Text = "Rule Name:";
            // 
            // buttonCancel
            // 
            this.buttonCancel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.buttonCancel.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.buttonCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.helpProvider.SetHelpString(this.buttonCancel, resources.GetString("buttonCancel.HelpString"));
            this.buttonCancel.Location = new System.Drawing.Point(255, 414);
            this.buttonCancel.Name = "buttonCancel";
            this.helpProvider.SetShowHelp(this.buttonCancel, true);
            this.buttonCancel.Size = new System.Drawing.Size(75, 23);
            this.buttonCancel.TabIndex = 6;
            this.buttonCancel.Text = "Cancel";
            this.buttonCancel.UseVisualStyleBackColor = true;
            this.buttonCancel.Click += new System.EventHandler(this.buttonCancel_Click);
            // 
            // buttonSave
            // 
            this.buttonSave.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.buttonSave.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.helpProvider.SetHelpString(this.buttonSave, resources.GetString("buttonSave.HelpString"));
            this.buttonSave.Location = new System.Drawing.Point(174, 414);
            this.buttonSave.Name = "buttonSave";
            this.helpProvider.SetShowHelp(this.buttonSave, true);
            this.buttonSave.Size = new System.Drawing.Size(75, 23);
            this.buttonSave.TabIndex = 5;
            this.buttonSave.Text = "&Save";
            this.buttonSave.UseVisualStyleBackColor = true;
            this.buttonSave.Click += new System.EventHandler(this.buttonSave_Click);
            // 
            // labelSimplifyingRules
            // 
            this.labelSimplifyingRules.Anchor = System.Windows.Forms.AnchorStyles.Right;
            this.labelSimplifyingRules.AutoSize = true;
            this.labelSimplifyingRules.Location = new System.Drawing.Point(3, 60);
            this.labelSimplifyingRules.Name = "labelSimplifyingRules";
            this.labelSimplifyingRules.Size = new System.Drawing.Size(84, 13);
            this.labelSimplifyingRules.TabIndex = 8;
            this.labelSimplifyingRules.Text = "Simplifying rules:";
            // 
            // treeViewDistinctions
            // 
            this.treeViewDistinctions.CheckBoxes = true;
            this.tableLayoutPanel.SetColumnSpan(this.treeViewDistinctions, 2);
            this.treeViewDistinctions.ContextMenuStrip = this.contextMenuStripFrom;
            this.treeViewDistinctions.Dock = System.Windows.Forms.DockStyle.Fill;
            this.treeViewDistinctions.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F);
            this.helpProvider.SetHelpString(this.treeViewDistinctions, "This tree shows all of the simplification processes for the named rule. You can c" +
                    "lick on the \'From:\' nodes in order to edit the string values. You can also ");
            this.treeViewDistinctions.Location = new System.Drawing.Point(93, 55);
            this.treeViewDistinctions.Name = "treeViewDistinctions";
            this.tableLayoutPanel.SetRowSpan(this.treeViewDistinctions, 2);
            this.helpProvider.SetShowHelp(this.treeViewDistinctions, true);
            this.treeViewDistinctions.ShowNodeToolTips = true;
            this.treeViewDistinctions.Size = new System.Drawing.Size(319, 280);
            this.treeViewDistinctions.TabIndex = 1;
            this.treeViewDistinctions.NodeMouseClick += new System.Windows.Forms.TreeNodeMouseClickEventHandler(this.treeViewDistinctions_NodeMouseClick);
            // 
            // contextMenuStripFrom
            // 
            this.contextMenuStripFrom.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.addFromValueToolStripMenuItem});
            this.contextMenuStripFrom.Name = "contextMenuStripFrom";
            this.contextMenuStripFrom.Size = new System.Drawing.Size(177, 26);
            // 
            // addFromValueToolStripMenuItem
            // 
            this.addFromValueToolStripMenuItem.Name = "addFromValueToolStripMenuItem";
            this.addFromValueToolStripMenuItem.Size = new System.Drawing.Size(176, 22);
            this.addFromValueToolStripMenuItem.Text = "&Add new \'From\' value";
            this.addFromValueToolStripMenuItem.Click += new System.EventHandler(this.addFromValueToolStripMenuItem_Click);
            // 
            // labelChangeFrom
            // 
            this.labelChangeFrom.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.labelChangeFrom.AutoSize = true;
            this.labelChangeFrom.Location = new System.Drawing.Point(93, 355);
            this.labelChangeFrom.Name = "labelChangeFrom";
            this.labelChangeFrom.Size = new System.Drawing.Size(70, 13);
            this.labelChangeFrom.TabIndex = 11;
            this.labelChangeFrom.Text = "Change From";
            // 
            // labelChangeInto
            // 
            this.labelChangeInto.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.labelChangeInto.AutoSize = true;
            this.labelChangeInto.Location = new System.Drawing.Point(255, 355);
            this.labelChangeInto.Name = "labelChangeInto";
            this.labelChangeInto.Size = new System.Drawing.Size(65, 13);
            this.labelChangeInto.TabIndex = 13;
            this.labelChangeInto.Text = "Change Into";
            // 
            // buttonTransliterate
            // 
            this.buttonTransliterate.Anchor = System.Windows.Forms.AnchorStyles.Right;
            this.buttonTransliterate.AutoSize = true;
            this.buttonTransliterate.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.helpProvider.SetHelpString(this.buttonTransliterate, resources.GetString("buttonTransliterate.HelpString"));
            this.buttonTransliterate.Location = new System.Drawing.Point(12, 378);
            this.buttonTransliterate.Name = "buttonTransliterate";
            this.helpProvider.SetShowHelp(this.buttonTransliterate, true);
            this.buttonTransliterate.Size = new System.Drawing.Size(75, 23);
            this.buttonTransliterate.TabIndex = 4;
            this.buttonTransliterate.Text = "Transliterate";
            this.buttonTransliterate.UseVisualStyleBackColor = true;
            this.buttonTransliterate.Click += new System.EventHandler(this.buttonTransliterate_Click);
            // 
            // ecTextBoxFrom
            // 
            this.ecTextBoxFrom.Dock = System.Windows.Forms.DockStyle.Fill;
            this.ecTextBoxFrom.Font = new System.Drawing.Font("Doulos SIL", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.helpProvider.SetHelpString(this.ecTextBoxFrom, resources.GetString("ecTextBoxFrom.HelpString"));
            this.ecTextBoxFrom.Location = new System.Drawing.Point(93, 371);
            this.ecTextBoxFrom.Name = "ecTextBoxFrom";
            this.ecTextBoxFrom.ReadOnly = true;
            this.helpProvider.SetShowHelp(this.ecTextBoxFrom, true);
            this.ecTextBoxFrom.Size = new System.Drawing.Size(156, 37);
            this.ecTextBoxFrom.TabIndex = 2;
            // 
            // ecTextBoxTo
            // 
            this.ecTextBoxTo.Dock = System.Windows.Forms.DockStyle.Fill;
            this.ecTextBoxTo.Font = new System.Drawing.Font("Doulos SIL", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.helpProvider.SetHelpString(this.ecTextBoxTo, "This box shows the result of the simplifying rule: the value in the \'Change From\'" +
                    " field (if it occurs in a word) will be replaced by this value when comparing fo" +
                    "r potentially misspelled words.");
            this.ecTextBoxTo.Location = new System.Drawing.Point(255, 371);
            this.ecTextBoxTo.Name = "ecTextBoxTo";
            this.ecTextBoxTo.ReadOnly = true;
            this.helpProvider.SetShowHelp(this.ecTextBoxTo, true);
            this.ecTextBoxTo.Size = new System.Drawing.Size(157, 37);
            this.ecTextBoxTo.TabIndex = 3;
            // 
            // DistinctionTreeEditor
            // 
            this.AcceptButton = this.buttonSave;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.CancelButton = this.buttonCancel;
            this.ClientSize = new System.Drawing.Size(439, 464);
            this.Controls.Add(this.tableLayoutPanel);
            this.HelpButton = true;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "DistinctionTreeEditor";
            this.Text = "Distinction Editor";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.DistinctionTreeEditor_FormClosing);
            this.tableLayoutPanel.ResumeLayout(false);
            this.tableLayoutPanel.PerformLayout();
            this.contextMenuStripFrom.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel;
        private System.Windows.Forms.Button buttonSave;
        private System.Windows.Forms.Button buttonCancel;
        private System.Windows.Forms.TreeView treeViewDistinctions;
        private System.Windows.Forms.Label labelRuleName;
        private System.Windows.Forms.TextBox textBoxRuleName;
        private System.Windows.Forms.Label labelDescription;
        private System.Windows.Forms.TextBox textBoxDescription;
        private System.Windows.Forms.Label labelSimplifyingRules;
        private EcTextBox ecTextBoxFrom;
        private System.Windows.Forms.Label labelChangeFrom;
        private EcTextBox ecTextBoxTo;
        private System.Windows.Forms.Label labelChangeInto;
        private System.Windows.Forms.Button buttonTransliterate;
        private System.Windows.Forms.HelpProvider helpProvider;
        private System.Windows.Forms.ContextMenuStrip contextMenuStripFrom;
        private System.Windows.Forms.ToolStripMenuItem addFromValueToolStripMenuItem;
    }
}