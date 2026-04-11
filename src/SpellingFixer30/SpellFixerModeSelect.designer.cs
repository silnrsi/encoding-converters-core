namespace SpellingFixer30
{
    partial class SpellFixerModeSelect
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(SpellFixerModeSelect));
            this.flowLayoutPanel = new System.Windows.Forms.FlowLayoutPanel();
            this.radioButtonCsc30 = new System.Windows.Forms.RadioButton();
            this.radioButtonSpellFixerLegacy = new System.Windows.Forms.RadioButton();
            this.buttonOK = new System.Windows.Forms.Button();
            this.buttonCancel = new System.Windows.Forms.Button();
            this.toolTip = new System.Windows.Forms.ToolTip(this.components);
            this.flowLayoutPanel.SuspendLayout();
            this.SuspendLayout();
            // 
            // flowLayoutPanel
            // 
            this.flowLayoutPanel.Controls.Add(this.radioButtonCsc30);
            this.flowLayoutPanel.Controls.Add(this.radioButtonSpellFixerLegacy);
            this.flowLayoutPanel.Location = new System.Drawing.Point(13, 13);
            this.flowLayoutPanel.Name = "flowLayoutPanel";
            this.flowLayoutPanel.Size = new System.Drawing.Size(214, 84);
            this.flowLayoutPanel.TabIndex = 0;
            // 
            // radioButtonCsc30
            // 
            this.radioButtonCsc30.AutoSize = true;
            this.radioButtonCsc30.Checked = true;
            this.radioButtonCsc30.Location = new System.Drawing.Point(3, 3);
            this.radioButtonCsc30.Name = "radioButtonCsc30";
            this.radioButtonCsc30.Size = new System.Drawing.Size(175, 17);
            this.radioButtonCsc30.TabIndex = 0;
            this.radioButtonCsc30.TabStop = true;
            this.radioButtonCsc30.Text = "&Consistent Spelling Fixer Project";
            this.toolTip.SetToolTip(this.radioButtonCsc30, "Click to load a Consistent Spelling Fixer project (for whole word spelling fixes)");
            this.radioButtonCsc30.UseVisualStyleBackColor = true;
            // 
            // radioButtonSpellFixerLegacy
            // 
            this.radioButtonSpellFixerLegacy.AutoSize = true;
            this.radioButtonSpellFixerLegacy.Location = new System.Drawing.Point(3, 26);
            this.radioButtonSpellFixerLegacy.Name = "radioButtonSpellFixerLegacy";
            this.radioButtonSpellFixerLegacy.Size = new System.Drawing.Size(108, 17);
            this.radioButtonSpellFixerLegacy.TabIndex = 1;
            this.radioButtonSpellFixerLegacy.Text = "Legacy &SpellFixer";
            this.toolTip.SetToolTip(this.radioButtonSpellFixerLegacy, "Click to load a Legacy Spell Fixer project (supports partial word spelling change" +
                    "s also)");
            this.radioButtonSpellFixerLegacy.UseVisualStyleBackColor = true;
            // 
            // buttonOK
            // 
            this.buttonOK.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.buttonOK.Location = new System.Drawing.Point(41, 114);
            this.buttonOK.Name = "buttonOK";
            this.buttonOK.Size = new System.Drawing.Size(75, 23);
            this.buttonOK.TabIndex = 1;
            this.buttonOK.Text = "OK";
            this.buttonOK.UseVisualStyleBackColor = true;
            this.buttonOK.Click += new System.EventHandler(this.buttonOK_Click);
            // 
            // buttonCancel
            // 
            this.buttonCancel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.buttonCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.buttonCancel.Location = new System.Drawing.Point(122, 114);
            this.buttonCancel.Name = "buttonCancel";
            this.buttonCancel.Size = new System.Drawing.Size(75, 23);
            this.buttonCancel.TabIndex = 2;
            this.buttonCancel.Text = "Cancel";
            this.buttonCancel.UseVisualStyleBackColor = true;
            this.buttonCancel.Click += new System.EventHandler(this.buttonCancel_Click);
            // 
            // SpellFixerModeSelect
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(239, 149);
            this.Controls.Add(this.buttonCancel);
            this.Controls.Add(this.buttonOK);
            this.Controls.Add(this.flowLayoutPanel);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "SpellFixerModeSelect";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Mode Select";
            this.TopMost = true;
            this.flowLayoutPanel.ResumeLayout(false);
            this.flowLayoutPanel.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.FlowLayoutPanel flowLayoutPanel;
        private System.Windows.Forms.RadioButton radioButtonCsc30;
        private System.Windows.Forms.RadioButton radioButtonSpellFixerLegacy;
        private System.Windows.Forms.Button buttonOK;
        private System.Windows.Forms.Button buttonCancel;
        private System.Windows.Forms.ToolTip toolTip;
    }
}