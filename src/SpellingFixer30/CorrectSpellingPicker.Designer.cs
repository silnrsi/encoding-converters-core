namespace SpellingFixer30
{
    partial class CorrectSpellingPicker
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(CorrectSpellingPicker));
            this.tableLayoutPanel = new System.Windows.Forms.TableLayoutPanel();
            this.buttonAddToDictionary = new System.Windows.Forms.Button();
            this.buttonIgnoreAlways = new System.Windows.Forms.Button();
            this.listBoxAmbiguities = new System.Windows.Forms.ListBox();
            this.richTextBoxContext = new System.Windows.Forms.RichTextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.buttonIgnoreOnce = new System.Windows.Forms.Button();
            this.buttonChange = new System.Windows.Forms.Button();
            this.buttonChangeAll = new System.Windows.Forms.Button();
            this.label2 = new System.Windows.Forms.Label();
            this.buttonCancel = new System.Windows.Forms.Button();
            this.buttonUndo = new System.Windows.Forms.Button();
            this.buttonOptions = new System.Windows.Forms.Button();
            this.buttonAddSuggestion = new System.Windows.Forms.Button();
            this.checkBoxShowWordInfo = new System.Windows.Forms.CheckBox();
            this.backgroundWorker = new System.ComponentModel.BackgroundWorker();
            this.toolTip = new System.Windows.Forms.ToolTip(this.components);
            this.helpProvider = new System.Windows.Forms.HelpProvider();
            this.tableLayoutPanel.SuspendLayout();
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
            this.tableLayoutPanel.Controls.Add(this.buttonAddToDictionary, 4, 3);
            this.tableLayoutPanel.Controls.Add(this.buttonIgnoreAlways, 4, 2);
            this.tableLayoutPanel.Controls.Add(this.listBoxAmbiguities, 0, 6);
            this.tableLayoutPanel.Controls.Add(this.richTextBoxContext, 0, 1);
            this.tableLayoutPanel.Controls.Add(this.label1, 0, 0);
            this.tableLayoutPanel.Controls.Add(this.buttonIgnoreOnce, 4, 1);
            this.tableLayoutPanel.Controls.Add(this.buttonChange, 4, 6);
            this.tableLayoutPanel.Controls.Add(this.buttonChangeAll, 4, 7);
            this.tableLayoutPanel.Controls.Add(this.label2, 0, 5);
            this.tableLayoutPanel.Controls.Add(this.buttonCancel, 4, 11);
            this.tableLayoutPanel.Controls.Add(this.buttonUndo, 3, 11);
            this.tableLayoutPanel.Controls.Add(this.buttonOptions, 1, 11);
            this.tableLayoutPanel.Controls.Add(this.buttonAddSuggestion, 4, 8);
            this.tableLayoutPanel.Controls.Add(this.checkBoxShowWordInfo, 0, 10);
            this.tableLayoutPanel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanel.Location = new System.Drawing.Point(0, 0);
            this.tableLayoutPanel.Name = "tableLayoutPanel";
            this.tableLayoutPanel.RowCount = 12;
            this.tableLayoutPanel.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 30F));
            this.tableLayoutPanel.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 29F));
            this.tableLayoutPanel.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 70F));
            this.tableLayoutPanel.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel.Size = new System.Drawing.Size(444, 331);
            this.tableLayoutPanel.TabIndex = 0;
            // 
            // buttonAddToDictionary
            // 
            this.buttonAddToDictionary.AutoSize = true;
            this.buttonAddToDictionary.Dock = System.Windows.Forms.DockStyle.Fill;
            this.helpProvider.SetHelpString(this.buttonAddToDictionary, "Click this button to add this word to the dictionary of known good words");
            this.buttonAddToDictionary.Location = new System.Drawing.Point(342, 74);
            this.buttonAddToDictionary.Name = "buttonAddToDictionary";
            this.helpProvider.SetShowHelp(this.buttonAddToDictionary, true);
            this.buttonAddToDictionary.Size = new System.Drawing.Size(99, 26);
            this.buttonAddToDictionary.TabIndex = 5;
            this.buttonAddToDictionary.Text = "Add to &Dictionary";
            this.toolTip.SetToolTip(this.buttonAddToDictionary, "Add to the dictionary of known good words");
            this.buttonAddToDictionary.UseVisualStyleBackColor = true;
            this.buttonAddToDictionary.Click += new System.EventHandler(this.buttonAddToDictionary_Click);
            // 
            // buttonIgnoreAlways
            // 
            this.buttonIgnoreAlways.AutoSize = true;
            this.buttonIgnoreAlways.Dock = System.Windows.Forms.DockStyle.Fill;
            this.helpProvider.SetHelpString(this.buttonIgnoreAlways, "Click this button to skip this word and move to the next potentially misspelled w" +
                    "ord (this session only--the only way to permanently ignore it is to add it to th" +
                    "e dictionary)");
            this.buttonIgnoreAlways.Location = new System.Drawing.Point(342, 45);
            this.buttonIgnoreAlways.Name = "buttonIgnoreAlways";
            this.helpProvider.SetShowHelp(this.buttonIgnoreAlways, true);
            this.buttonIgnoreAlways.Size = new System.Drawing.Size(99, 23);
            this.buttonIgnoreAlways.TabIndex = 4;
            this.buttonIgnoreAlways.Text = "I&gnore All";
            this.toolTip.SetToolTip(this.buttonIgnoreAlways, "Skip always during this session");
            this.buttonIgnoreAlways.UseVisualStyleBackColor = true;
            this.buttonIgnoreAlways.Click += new System.EventHandler(this.buttonIgnoreAlways_Click);
            // 
            // listBoxAmbiguities
            // 
            this.tableLayoutPanel.SetColumnSpan(this.listBoxAmbiguities, 4);
            this.listBoxAmbiguities.Dock = System.Windows.Forms.DockStyle.Fill;
            this.helpProvider.SetHelpString(this.listBoxAmbiguities, resources.GetString("listBoxAmbiguities.HelpString"));
            this.listBoxAmbiguities.Location = new System.Drawing.Point(3, 141);
            this.listBoxAmbiguities.Name = "listBoxAmbiguities";
            this.tableLayoutPanel.SetRowSpan(this.listBoxAmbiguities, 4);
            this.helpProvider.SetShowHelp(this.listBoxAmbiguities, true);
            this.listBoxAmbiguities.Size = new System.Drawing.Size(333, 134);
            this.listBoxAmbiguities.TabIndex = 1;
            this.listBoxAmbiguities.MouseMove += new System.Windows.Forms.MouseEventHandler(this.listBoxAmbiguities_MouseMove);
            // 
            // richTextBoxContext
            // 
            this.tableLayoutPanel.SetColumnSpan(this.richTextBoxContext, 4);
            this.richTextBoxContext.Dock = System.Windows.Forms.DockStyle.Fill;
            this.helpProvider.SetHelpString(this.richTextBoxContext, resources.GetString("richTextBoxContext.HelpString"));
            this.richTextBoxContext.Location = new System.Drawing.Point(3, 16);
            this.richTextBoxContext.Name = "richTextBoxContext";
            this.tableLayoutPanel.SetRowSpan(this.richTextBoxContext, 4);
            this.helpProvider.SetShowHelp(this.richTextBoxContext, true);
            this.richTextBoxContext.Size = new System.Drawing.Size(333, 106);
            this.richTextBoxContext.TabIndex = 0;
            this.richTextBoxContext.Text = "";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.tableLayoutPanel.SetColumnSpan(this.label1, 5);
            this.label1.Location = new System.Drawing.Point(3, 0);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(113, 13);
            this.label1.TabIndex = 2;
            this.label1.Text = "Word not in dictionary:";
            // 
            // buttonIgnoreOnce
            // 
            this.buttonIgnoreOnce.AutoSize = true;
            this.buttonIgnoreOnce.Dock = System.Windows.Forms.DockStyle.Fill;
            this.helpProvider.SetHelpString(this.buttonIgnoreOnce, "Click this button to skip this word and move to the next potentially misspelled w" +
                    "ord (it will be flagged again on its next occurrence)");
            this.buttonIgnoreOnce.Location = new System.Drawing.Point(342, 16);
            this.buttonIgnoreOnce.Name = "buttonIgnoreOnce";
            this.helpProvider.SetShowHelp(this.buttonIgnoreOnce, true);
            this.buttonIgnoreOnce.Size = new System.Drawing.Size(99, 23);
            this.buttonIgnoreOnce.TabIndex = 3;
            this.buttonIgnoreOnce.Text = "&Ignore Once";
            this.toolTip.SetToolTip(this.buttonIgnoreOnce, "Skip once");
            this.buttonIgnoreOnce.UseVisualStyleBackColor = true;
            this.buttonIgnoreOnce.Click += new System.EventHandler(this.buttonIgnoreOnce_Click);
            // 
            // buttonChange
            // 
            this.buttonChange.AutoSize = true;
            this.buttonChange.Dock = System.Windows.Forms.DockStyle.Fill;
            this.helpProvider.SetHelpString(this.buttonChange, "Click this button to change this occurrence of the word to the selected word from" +
                    " the Suggestions list");
            this.buttonChange.Location = new System.Drawing.Point(342, 141);
            this.buttonChange.Name = "buttonChange";
            this.helpProvider.SetShowHelp(this.buttonChange, true);
            this.buttonChange.Size = new System.Drawing.Size(99, 23);
            this.buttonChange.TabIndex = 6;
            this.buttonChange.Text = "&Change";
            this.toolTip.SetToolTip(this.buttonChange, "Replace (once) with selected suggestion");
            this.buttonChange.UseVisualStyleBackColor = true;
            this.buttonChange.Click += new System.EventHandler(this.buttonChange_Click);
            // 
            // buttonChangeAll
            // 
            this.buttonChangeAll.AutoSize = true;
            this.buttonChangeAll.Dock = System.Windows.Forms.DockStyle.Fill;
            this.helpProvider.SetHelpString(this.buttonChangeAll, resources.GetString("buttonChangeAll.HelpString"));
            this.buttonChangeAll.Location = new System.Drawing.Point(342, 170);
            this.buttonChangeAll.Name = "buttonChangeAll";
            this.helpProvider.SetShowHelp(this.buttonChangeAll, true);
            this.buttonChangeAll.Size = new System.Drawing.Size(99, 23);
            this.buttonChangeAll.TabIndex = 7;
            this.buttonChangeAll.Text = "C&hange All";
            this.toolTip.SetToolTip(this.buttonChangeAll, "Replace (always) with selected suggestion");
            this.buttonChangeAll.UseVisualStyleBackColor = true;
            this.buttonChangeAll.Click += new System.EventHandler(this.buttonChangeAll_Click);
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(3, 125);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(68, 13);
            this.label2.TabIndex = 8;
            this.label2.Text = "Suggestions:";
            // 
            // buttonCancel
            // 
            this.buttonCancel.AutoSize = true;
            this.buttonCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.helpProvider.SetHelpString(this.buttonCancel, "Click this button to close the dialog and stop the spelling check");
            this.buttonCancel.Location = new System.Drawing.Point(342, 304);
            this.buttonCancel.Name = "buttonCancel";
            this.helpProvider.SetShowHelp(this.buttonCancel, true);
            this.buttonCancel.Size = new System.Drawing.Size(99, 23);
            this.buttonCancel.TabIndex = 9;
            this.buttonCancel.Text = "Close";
            this.toolTip.SetToolTip(this.buttonCancel, "Close spelling check dialog");
            this.buttonCancel.UseVisualStyleBackColor = true;
            this.buttonCancel.Click += new System.EventHandler(this.buttonCancel_Click);
            // 
            // buttonUndo
            // 
            this.buttonUndo.AutoSize = true;
            this.helpProvider.SetHelpString(this.buttonUndo, "Click this button to undo the last change");
            this.buttonUndo.Location = new System.Drawing.Point(277, 304);
            this.buttonUndo.Name = "buttonUndo";
            this.helpProvider.SetShowHelp(this.buttonUndo, true);
            this.buttonUndo.Size = new System.Drawing.Size(59, 23);
            this.buttonUndo.TabIndex = 10;
            this.buttonUndo.Text = "&Undo";
            this.toolTip.SetToolTip(this.buttonUndo, "Undo last change");
            this.buttonUndo.UseVisualStyleBackColor = true;
            this.buttonUndo.Visible = false;
            this.buttonUndo.Click += new System.EventHandler(this.buttonUndo_Click);
            // 
            // buttonOptions
            // 
            this.buttonOptions.Location = new System.Drawing.Point(77, 304);
            this.buttonOptions.Name = "buttonOptions";
            this.buttonOptions.Size = new System.Drawing.Size(75, 23);
            this.buttonOptions.TabIndex = 11;
            this.buttonOptions.Text = "&Options...";
            this.buttonOptions.UseVisualStyleBackColor = true;
            this.buttonOptions.Visible = false;
            this.buttonOptions.Click += new System.EventHandler(this.buttonOptions_Click);
            // 
            // buttonAddSuggestion
            // 
            this.buttonAddSuggestion.Dock = System.Windows.Forms.DockStyle.Fill;
            this.helpProvider.SetHelpString(this.buttonAddSuggestion, "Click this to add a new suggestion which you can then choose \'Change\' or \'Change " +
                    "All\' for");
            this.buttonAddSuggestion.Location = new System.Drawing.Point(342, 199);
            this.buttonAddSuggestion.Name = "buttonAddSuggestion";
            this.helpProvider.SetShowHelp(this.buttonAddSuggestion, true);
            this.buttonAddSuggestion.Size = new System.Drawing.Size(99, 23);
            this.buttonAddSuggestion.TabIndex = 13;
            this.buttonAddSuggestion.Text = "Add Su&ggestion";
            this.toolTip.SetToolTip(this.buttonAddSuggestion, "Enter a new suggestion");
            this.buttonAddSuggestion.UseVisualStyleBackColor = true;
            this.buttonAddSuggestion.Click += new System.EventHandler(this.buttonAddSuggestion_Click);
            // 
            // checkBoxShowWordInfo
            // 
            this.checkBoxShowWordInfo.AutoSize = true;
            this.checkBoxShowWordInfo.Checked = true;
            this.checkBoxShowWordInfo.CheckState = System.Windows.Forms.CheckState.Checked;
            this.tableLayoutPanel.SetColumnSpan(this.checkBoxShowWordInfo, 4);
            this.helpProvider.SetHelpString(this.checkBoxShowWordInfo, "Check this box to display a transliteration of the word and the occurrence count");
            this.checkBoxShowWordInfo.Location = new System.Drawing.Point(3, 281);
            this.checkBoxShowWordInfo.Name = "checkBoxShowWordInfo";
            this.helpProvider.SetShowHelp(this.checkBoxShowWordInfo, true);
            this.checkBoxShowWordInfo.Size = new System.Drawing.Size(133, 17);
            this.checkBoxShowWordInfo.TabIndex = 12;
            this.checkBoxShowWordInfo.Text = "Show &extra information";
            this.toolTip.SetToolTip(this.checkBoxShowWordInfo, "Check this box to display a transliteration of the word and the occurrence count");
            this.checkBoxShowWordInfo.UseVisualStyleBackColor = true;
            this.checkBoxShowWordInfo.CheckStateChanged += new System.EventHandler(this.checkBoxShowWordInfo_CheckStateChanged);
            // 
            // backgroundWorker
            // 
            this.backgroundWorker.WorkerSupportsCancellation = true;
            this.backgroundWorker.DoWork += new System.ComponentModel.DoWorkEventHandler(this.backgroundWorker_DoWork);
            this.backgroundWorker.RunWorkerCompleted += new System.ComponentModel.RunWorkerCompletedEventHandler(this.backgroundWorker_RunWorkerCompleted);
            // 
            // toolTip
            // 
            this.toolTip.AutomaticDelay = 100;
            this.toolTip.AutoPopDelay = 10000;
            this.toolTip.InitialDelay = 100;
            this.toolTip.ReshowDelay = 20;
            // 
            // CorrectSpellingPicker
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.CancelButton = this.buttonCancel;
            this.ClientSize = new System.Drawing.Size(444, 331);
            this.Controls.Add(this.tableLayoutPanel);
            this.HelpButton = true;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "CorrectSpellingPicker";
            this.StartPosition = System.Windows.Forms.FormStartPosition.Manual;
            this.Text = "Spell Check";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.CorrectSpellingPicker_FormClosing);
            this.tableLayoutPanel.ResumeLayout(false);
            this.tableLayoutPanel.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel;
        private System.Windows.Forms.RichTextBox richTextBoxContext;
        private System.Windows.Forms.ListBox listBoxAmbiguities;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Button buttonIgnoreOnce;
        private System.Windows.Forms.Button buttonIgnoreAlways;
        private System.Windows.Forms.Button buttonAddToDictionary;
        private System.Windows.Forms.Button buttonChange;
        private System.Windows.Forms.Button buttonChangeAll;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Button buttonCancel;
        private System.Windows.Forms.Button buttonUndo;
        private System.Windows.Forms.Button buttonOptions;
        private System.ComponentModel.BackgroundWorker backgroundWorker;
		private System.Windows.Forms.CheckBox checkBoxShowWordInfo;
		private System.Windows.Forms.Button buttonAddSuggestion;
		private System.Windows.Forms.ToolTip toolTip;
        private System.Windows.Forms.HelpProvider helpProvider;
    }
}