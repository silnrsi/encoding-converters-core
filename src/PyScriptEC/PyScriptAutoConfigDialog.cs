// Created by Jim Kornelsen on Nov 14 2011
//
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.IO;
using ECInterfaces;                     // for IEncConverter

//uncomment the following line for verbose debugging output using Console.WriteLine
//#define VERBOSE_DEBUGGING

namespace SilEncConverters40
{
    public class PyScriptAutoConfigDialog : SilEncConverters40.AutoConfigDialog
    {
        protected bool m_bInitialized = false;  // set at the end of Initialize (to block certain events until we're ready for them)

        public PyScriptAutoConfigDialog (
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
#if VERBOSE_DEBUGGING
            Console.WriteLine("PyScriptAutoConfigDialog ctor BEGIN");
#endif
			InitializeComponent();
#if VERBOSE_DEBUGGING
            Console.WriteLine("Initialized PyScriptAutoConfigDialog component.");
#endif
			base.Initialize (
                aECs,
                PyScriptEncConverter.strHtmlFilename,
                strDisplayName,
                strFriendlyName,
                strConverterIdentifier,
                eConversionType,
                strLhsEncodingId,
                strRhsEncodingId,
                lProcessTypeFlags,
                bIsInRepository);
#if VERBOSE_DEBUGGING
            Console.WriteLine("Initialized base.");
#endif
            // if we're editing a CC table/spellfixer project, then set the Converter Spec and say it's unmodified
            if (m_bEditMode)
            {
#if VERBOSE_DEBUGGING
                Console.WriteLine("Edit mode");
#endif
				System.Diagnostics.Debug.Assert(!String.IsNullOrEmpty(ConverterIdentifier));
                textBoxFileSpec.Text = ConverterIdentifier;
                IsModified = false;
            }

            m_bInitialized = true;
#if VERBOSE_DEBUGGING
            Console.WriteLine("PyScriptAutoConfigDialog ctor END");
#endif
		}

        public PyScriptAutoConfigDialog (
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

        private System.Windows.Forms.Label labelScriptFile;
        private System.Windows.Forms.TextBox textBoxFileSpec;
        private System.Windows.Forms.Button buttonBrowse;
        private System.Windows.Forms.OpenFileDialog openFileDialogBrowse;

        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel1;

        // This code was NOT generated!
        // So feel free to modify it as needed.
        private void InitializeComponent()
        {
            this.tableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
            this.labelScriptFile = new System.Windows.Forms.Label();
            this.textBoxFileSpec = new System.Windows.Forms.TextBox();
            this.buttonBrowse = new System.Windows.Forms.Button();
            this.openFileDialogBrowse = new System.Windows.Forms.OpenFileDialog();
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
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 20F));
            this.tableLayoutPanel1.Controls.Add(this.labelScriptFile, 0, 0);
            this.tableLayoutPanel1.Controls.Add(this.textBoxFileSpec, 1, 0);
            this.tableLayoutPanel1.Controls.Add(this.buttonBrowse, 2, 0);
            this.tableLayoutPanel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanel1.Location = new System.Drawing.Point(3, 3);
            this.tableLayoutPanel1.Name = "tableLayoutPanel1";
            this.tableLayoutPanel1.RowCount = 2;
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel1.Size = new System.Drawing.Size(596, 394);
            this.tableLayoutPanel1.TabIndex = 1;
            // 
            // labelScriptFile
            // 
            this.labelScriptFile.Anchor = System.Windows.Forms.AnchorStyles.Right;
            this.labelScriptFile.AutoSize = true;
            this.labelScriptFile.Location = new System.Drawing.Point(3, 8);
            this.labelScriptFile.Name = "labelScriptFile";
            this.labelScriptFile.Size = new System.Drawing.Size(87, 13);
            this.labelScriptFile.TabIndex = 0;
            this.labelScriptFile.Text = "Python script file:";
            // 
            // textBoxFileSpec
            // 
            this.textBoxFileSpec.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.textBoxFileSpec.Location = new System.Drawing.Point(96, 4);
            this.textBoxFileSpec.Name = "textBoxFileSpec";
            this.textBoxFileSpec.Size = new System.Drawing.Size(400, 20);
            this.textBoxFileSpec.TabIndex = 1;
            this.textBoxFileSpec.TextChanged += new System.EventHandler(this.textBoxFileSpec_TextChanged);
            // 
            // buttonBrowse
            // 
            this.buttonBrowse.AutoSize = true;
            this.buttonBrowse.Dock = System.Windows.Forms.DockStyle.Fill;
            this.buttonBrowse.Location = new System.Drawing.Point(502, 3);
            this.buttonBrowse.Name = "buttonBrowse";
            this.buttonBrowse.Size = new System.Drawing.Size(91, 23);
            this.buttonBrowse.TabIndex = 2;
            this.buttonBrowse.Text = "Browse...";
            this.buttonBrowse.Click += new System.EventHandler(this.buttonBrowse_Click);
            // 
            // openFileDialogBrowse
            // 
            this.openFileDialogBrowse.DefaultExt = "py";
            this.openFileDialogBrowse.Filter = "Python scrips (*.py)|*.py";
            this.openFileDialogBrowse.Title = "Browse for Python script";
            // 
            // PyScriptAutoConfigDialog
            // 
            this.ClientSize = new System.Drawing.Size(634, 479);
            this.Name = "PyScriptAutoConfigDialog";
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

        protected void UpdateUI(bool bVisible)
        {
#if VERBOSE_DEBUGGING
            Console.WriteLine("UpdateUI BEGIN");
#endif
/*
            buttonSaveInRepository.Visible =
                groupBoxExpects.Visible =
                groupBoxReturns.Visible = bVisible;
*/
#if VERBOSE_DEBUGGING
            Console.WriteLine("UpdateUI END");
#endif
		}

        protected override void SetConvTypeControls()
        {
            //SetRbValuesFromConvType(radioButtonExpectsUnicode, radioButtonExpectsLegacy, radioButtonReturnsUnicode,
            //    radioButtonReturnsLegacy);
        }

        // this method is called either when the user clicks the "Apply" or "OK" buttons *OR* if she
        //  tries to switch to the Test or Advanced tab. This is the dialog's one opportunity
        //  to make sure that the user has correctly configured a legitimate converter.
        protected override bool OnApply()
        {
            Console.Error.WriteLine("OnApply() BEGIN");
            // Get the converter identifier from the Setup tab controls.
            ConverterIdentifier = textBoxFileSpec.Text;
            //SetConvTypeFromRbControls(radioButtonExpectsUnicode, radioButtonExpectsLegacy,
            //    radioButtonReturnsUnicode, radioButtonReturnsLegacy);

            // if we're actually on the setup tab, then do some further checking as well.
            if (tabControl.SelectedTab == tabPageSetup)
            {
                // only do these message boxes if we're on the Setup tab itself, because if this OnApply
                //  is being called as a result of the user switching to the Test tab, that code will
                //  already put up an error message and we don't need two error messages.
                if (String.IsNullOrEmpty(ConverterIdentifier))
                {
                    MessageBox.Show(this, "Choose a Python script first!", EncConverters.cstrCaption);
                    return false;
                }
                else if (!File.Exists(ConverterIdentifier))
                {
                    MessageBox.Show(this, "File doesn't exist!", EncConverters.cstrCaption);
                    return false;
                }
            }
            Console.Error.WriteLine("OnApply() END");
            return base.OnApply();
        }

/*
        protected override bool ShouldRemoveBeforeAdd
        {
            get { return true; }
        }

        protected override bool ShouldFriendlyNameBeReadOnly
        {
            get { return false; }
        }

        protected override bool GetFontMapping(string strFriendlyName, out string strLhsFont, out string strRhsFont)
        {
            return base.GetFontMapping(strFriendlyName, out strLhsFont, out strRhsFont);
        }
*/

        protected override string ProgID
        {
            get { return typeof(PyScriptEncConverter).FullName; }
        }

        protected override string ImplType
        {
            get { return EncConverters.strTypeSILPyScript; }
        }

        protected override string DefaultFriendlyName
        {
            // as the default, make it the same as the table name (w/o extension)
            get { return Path.GetFileNameWithoutExtension(ConverterIdentifier); }
        }

        private void buttonBrowse_Click(object sender, EventArgs e)
        {
            if (!String.IsNullOrEmpty(ConverterIdentifier))
                openFileDialogBrowse.InitialDirectory = Path.GetDirectoryName(ConverterIdentifier);
            else
                openFileDialogBrowse.InitialDirectory = Util.GetSpecialFolderPath(Environment.SpecialFolder.CommonApplicationData) + EncConverters.strDefMapsTablesPath;

            if (openFileDialogBrowse.ShowDialog() == DialogResult.OK)
            {
                ResetFields();
                textBoxFileSpec.Text = openFileDialogBrowse.FileName;
            }
        }

        private void textBoxFileSpec_TextChanged(object sender, EventArgs e)
        {
            if (m_bInitialized) // but only do this after we're already initialized
            {
                IsModified = (((TextBox)sender).Text.Length > 0);
                //ProcessType &= ~(int)ProcessTypeFlags.SpellingFixerProject;
                UpdateUI(IsModified);
            }
        }

/*
        protected override bool SetupTabSelected_MakeSaveInRepositoryVisible
        {
            get { return !IsSpellFixerProject; }
        }

        private void buttonAddSpellFixer_Click(object sender, EventArgs e)
        {
            try
            {
                SpellFixerByReflection aSF = new SpellFixerByReflection();
                aSF.LoginProject();
                ((EncConverters)m_aECs).Reinitialize();
                FriendlyName = aSF.SpellFixerEncConverterName;
                m_aEC = m_aECs[FriendlyName];
                if (m_aEC != null)
                {
                    textBoxFileSpec.Text = ConverterIdentifier = m_aEC.ConverterIdentifier;
                    ConversionType = m_aEC.ConversionType;
                    ProcessType = m_aEC.ProcessType;
                    UpdateUI(false);
                    aSF.QueryForSpellingCorrectionIfTableEmpty("incorect");
                    aSF.EditSpellingFixes();
                    IsInRepository = true;
                }
            }
            catch (Exception)
            {
                // usually just a "no project selected message, so .... ignoring it
                // MessageBox.Show(ex.Message, EncConverters.cstrCaption);
            }
        }

        private void radioButton_CheckedChanged(object sender, EventArgs e)
        {
            IsModified = true;
        }
*/
    }
}

