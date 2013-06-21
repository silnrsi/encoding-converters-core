// Created by Jim Kornelsen on Nov 21 2011
//
using System;
using System.Windows.Forms;
using System.IO;
using ECInterfaces;
using PerlExpressionEC.Properties;

namespace SilEncConverters40
{
    public class PerlExpressionAutoConfigDialog : SilEncConverters40.AutoConfigDialog
    {
        protected bool m_bInitialized = false;  // set at the end of Initialize (to block certain events until we're ready for them)

        public PerlExpressionAutoConfigDialog (
            IEncConverters aECs,
            string strDisplayName,
            string strFriendlyName,
            string strConverterIdentifier,
            ConvType eConversionType,
            string strLhsEncodingId,
            string strRhsEncodingId,
            int lProcessTypeFlags,
            bool bIsInRepository)
        {
            Util.DebugWriteLine(this, "BEGIN");
			InitializeComponent();
            Util.DebugWriteLine(this, "Initialized component.");
			base.Initialize (
                aECs,
                PerlExpressionEncConverter.strHtmlFilename,
                strDisplayName,
                strFriendlyName,
                strConverterIdentifier,
                eConversionType,
                strLhsEncodingId,
                strRhsEncodingId,
                lProcessTypeFlags,
                bIsInRepository);
            Util.DebugWriteLine(this, "Initialized base.");
            // if we're editing a CC table/spellfixer project, then set the Converter Spec and say it's unmodified
            if (m_bEditMode)
            {
                Util.DebugWriteLine(this, "Edit mode");
				System.Diagnostics.Debug.Assert(!String.IsNullOrEmpty(ConverterIdentifier));
                textBoxExpression.Text = ConverterIdentifier;
                IsModified = false;
            }

            // initialize the combo box with some examples that users can try
            LoadComboBoxFromSettings(comboBoxPreviousPerlExpressions);

            if (comboBoxPreviousPerlExpressions.Items.Count > 0)
                comboBoxPreviousPerlExpressions.SelectedIndex = 0;

            m_bInitialized = true;
            Util.DebugWriteLine(this, "END");
		}

        private void LoadComboBoxFromSettings(ComboBox comboBox)
        {
            comboBox.Items.Clear();
            foreach (var str in Settings.Default.RecentlyUsedExpressions)
                comboBox.Items.Add(str);
        }

        public PerlExpressionAutoConfigDialog (
            IEncConverters aECs,
            string strFriendlyName,
            string strConverterIdentifier,
            ConvType eConversionType,
            string strTestData)
        {
            InitializeComponent();
            base.Initialize (
                aECs,
                strFriendlyName,
                strConverterIdentifier,
                eConversionType,
                strTestData);
        }

        private System.Windows.Forms.Label              labelScriptFile;
        private System.Windows.Forms.TextBox            textBoxExpression;
        private Label label1;
        private ComboBox comboBoxPreviousPerlExpressions;
        private Button buttonDeleteSavedExpression;
        private System.Windows.Forms.TableLayoutPanel   tableLayoutPanel1;

        // This code was NOT generated!
        // So feel free to modify it as needed.
        private void InitializeComponent()
        {
            this.tableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
            this.labelScriptFile = new System.Windows.Forms.Label();
            this.textBoxExpression = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.comboBoxPreviousPerlExpressions = new System.Windows.Forms.ComboBox();
            this.buttonDeleteSavedExpression = new System.Windows.Forms.Button();
            this.tabControl.SuspendLayout();
            this.tabPageSetup.SuspendLayout();
            this.tableLayoutPanel1.SuspendLayout();
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
            this.tableLayoutPanel1.Controls.Add(this.labelScriptFile, 0, 0);
            this.tableLayoutPanel1.Controls.Add(this.textBoxExpression, 1, 0);
            this.tableLayoutPanel1.Controls.Add(this.label1, 0, 2);
            this.tableLayoutPanel1.Controls.Add(this.comboBoxPreviousPerlExpressions, 1, 2);
            this.tableLayoutPanel1.Controls.Add(this.buttonDeleteSavedExpression, 2, 2);
            this.tableLayoutPanel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanel1.Location = new System.Drawing.Point(3, 3);
            this.tableLayoutPanel1.Name = "tableLayoutPanel1";
            this.tableLayoutPanel1.RowCount = 4;
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 40F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel1.Size = new System.Drawing.Size(596, 394);
            this.tableLayoutPanel1.TabIndex = 1;
            // 
            // labelScriptFile
            // 
            this.labelScriptFile.Anchor = System.Windows.Forms.AnchorStyles.Right;
            this.labelScriptFile.AutoSize = true;
            this.labelScriptFile.Location = new System.Drawing.Point(31, 135);
            this.labelScriptFile.Name = "labelScriptFile";
            this.labelScriptFile.Size = new System.Drawing.Size(81, 13);
            this.labelScriptFile.TabIndex = 0;
            this.labelScriptFile.Text = "Perl expression:";
            // 
            // textBoxExpression
            // 
            this.textBoxExpression.AcceptsReturn = true;
            this.textBoxExpression.AcceptsTab = true;
            this.tableLayoutPanel1.SetColumnSpan(this.textBoxExpression, 2);
            this.textBoxExpression.Dock = System.Windows.Forms.DockStyle.Fill;
            this.textBoxExpression.Location = new System.Drawing.Point(118, 3);
            this.textBoxExpression.Multiline = true;
            this.textBoxExpression.Name = "textBoxExpression";
            this.textBoxExpression.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.textBoxExpression.Size = new System.Drawing.Size(475, 278);
            this.textBoxExpression.TabIndex = 1;
            this.textBoxExpression.TextChanged += new System.EventHandler(this.textBoxExpression_TextChanged);
            // 
            // label1
            // 
            this.label1.Anchor = System.Windows.Forms.AnchorStyles.Right;
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(3, 332);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(109, 13);
            this.label1.TabIndex = 0;
            this.label1.Text = "Previous expressions:";
            // 
            // comboBoxPreviousPerlExpressions
            // 
            this.comboBoxPreviousPerlExpressions.Dock = System.Windows.Forms.DockStyle.Fill;
            this.comboBoxPreviousPerlExpressions.FormattingEnabled = true;
            this.comboBoxPreviousPerlExpressions.Location = new System.Drawing.Point(118, 327);
            this.comboBoxPreviousPerlExpressions.Name = "comboBoxPreviousPerlExpressions";
            this.comboBoxPreviousPerlExpressions.Size = new System.Drawing.Size(380, 21);
            this.comboBoxPreviousPerlExpressions.TabIndex = 2;
            this.comboBoxPreviousPerlExpressions.SelectedIndexChanged += new System.EventHandler(this.comboBoxPreviousPerlExpressions_SelectedIndexChanged);
            // 
            // buttonDeleteSavedExpression
            // 
            this.buttonDeleteSavedExpression.Location = new System.Drawing.Point(504, 327);
            this.buttonDeleteSavedExpression.Name = "buttonDeleteSavedExpression";
            this.buttonDeleteSavedExpression.Size = new System.Drawing.Size(89, 23);
            this.buttonDeleteSavedExpression.TabIndex = 3;
            this.buttonDeleteSavedExpression.Text = "Delete";
            this.buttonDeleteSavedExpression.UseVisualStyleBackColor = true;
            this.buttonDeleteSavedExpression.Click += new System.EventHandler(this.buttonDeleteSavedExpression_Click);
            // 
            // PerlExpressionAutoConfigDialog
            // 
            this.ClientSize = new System.Drawing.Size(634, 479);
            this.Name = "PerlExpressionAutoConfigDialog";
            this.Controls.SetChildIndex(this.tabControl, 0);
            this.Controls.SetChildIndex(this.buttonApply, 0);
            this.Controls.SetChildIndex(this.buttonCancel, 0);
            this.Controls.SetChildIndex(this.buttonOK, 0);
            this.Controls.SetChildIndex(this.buttonSaveInRepository, 0);
            this.tabControl.ResumeLayout(false);
            this.tabPageSetup.ResumeLayout(false);
            this.tableLayoutPanel1.ResumeLayout(false);
            this.tableLayoutPanel1.PerformLayout();
            this.ResumeLayout(false);

        }

        // this method is called either when the user clicks the "Apply" or "OK" buttons *OR* if she
        //  tries to switch to the Test or Advanced tab. This is the dialog's one opportunity
        //  to make sure that the user has correctly configured a legitimate converter.
        protected override bool OnApply()
        {
            Util.DebugWriteLine(this, "BEGIN");
            // Get the converter identifier from the Setup tab controls.
            ConverterIdentifier = textBoxExpression.Text;

            // if we're actually on the setup tab, then do some further checking as well.
            if (tabControl.SelectedTab == tabPageSetup)
            {
                // only do these message boxes if we're on the Setup tab itself, because if this OnApply
                //  is being called as a result of the user switching to the Test tab, that code will
                //  already put up an error message and we don't need two error messages.
                if (String.IsNullOrEmpty(ConverterIdentifier))
                {
                    MessageBox.Show(this, "Enter a Perl expression first!", EncConverters.cstrCaption);
                    return false;
                }

                if (!Settings.Default.RecentlyUsedExpressions.Contains(ConverterIdentifier))
                {
                    Settings.Default.RecentlyUsedExpressions.Add(ConverterIdentifier);
                    Settings.Default.Save();
                    LoadComboBoxFromSettings(comboBoxPreviousPerlExpressions);
                }
            }

            Util.DebugWriteLine(this, "END");
            return base.OnApply();
        }

        protected override string ProgID
        {
            get { return typeof(PerlExpressionEncConverter).FullName; }
        }

        protected override string ImplType
        {
            get { return EncConverters.strTypeSILPerlExpression; }
        }

        protected override string DefaultFriendlyName
        {
            // as the default, make it the same as the table name (w/o extension)
            get { return Path.GetFileNameWithoutExtension(ConverterIdentifier); }
        }

        private void textBoxExpression_TextChanged(object sender, EventArgs e)
        {
            if (m_bInitialized) // but only do this after we're already initialized
                IsModified = (((TextBox)sender).Text.Length > 0);
        }

        private void comboBoxPreviousPerlExpressions_SelectedIndexChanged(object sender, EventArgs e)
        {
            // enter the new selected item into the text box
            if (m_bInitialized)
                textBoxExpression.Text = comboBoxPreviousPerlExpressions.SelectedItem.ToString();
        }

        private void buttonDeleteSavedExpression_Click(object sender, EventArgs e)
        {
            var str = comboBoxPreviousPerlExpressions.SelectedItem.ToString();
            if (!Settings.Default.RecentlyUsedExpressions.Contains(str))
                return;

            Settings.Default.RecentlyUsedExpressions.Remove(str);
            Settings.Default.Save();
            
            LoadComboBoxFromSettings(comboBoxPreviousPerlExpressions);

            // if there are any left...
            if (comboBoxPreviousPerlExpressions.Items.Count <= 0)
                return;

            // ... set something in the combo box so it doesn't keep showing the now deleted one
            // (but pretend we're not initialized so we don't overwrite what's in the text box
            m_bInitialized = false;
            comboBoxPreviousPerlExpressions.SelectedIndex = 0;
            m_bInitialized = true;
        }
    }
}

