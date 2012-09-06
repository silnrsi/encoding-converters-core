// Created by Jim Kornelsen on Nov 14 2011
//
using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.IO;
using ECInterfaces;                     // for IEncConverter

namespace SilEncConverters40
{
    public class IcuTranslitAutoConfigDialog : SilEncConverters40.AutoConfigDialog
    {
        #region DLLImport Statements
        // On Linux looks for libIcuTranslitEC.so (adds lib- and -.so)
        //[DllImport("IcuTranslitEC", SetLastError=true)]
        [DllImport("IcuTranslitEC", EntryPoint="IcuTranslitEC_ConverterNameList_start")]
        static extern unsafe int CppConverterNameList_start();

        [DllImport("IcuTranslitEC", EntryPoint="IcuTranslitEC_ConverterNameList_next")]
        static extern unsafe string CppConverterNameList_next();

        [DllImport("IcuTranslitEC", EntryPoint="IcuTranslitEC_GetDisplayName")]
        static extern unsafe string CppGetDisplayName(string strID);
        #endregion DLLImport Statements

        protected bool m_bInitialized = false;  // set at the end of Initialize (to block certain events until we're ready for them)
        private string [] translitIDs;

        public IcuTranslitAutoConfigDialog (
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
            System.Diagnostics.Debug.WriteLine("IcuTranslitAutoConfigDialog ctor BEGIN");
            InitializeComponent();
            fillListBox();
            System.Diagnostics.Debug.WriteLine("Initialized IcuTranslitAutoConfigDialog component.");
            base.Initialize (
                aECs,
                IcuTranslitEncConverter.strHtmlFilename,
                strDisplayName,
                strFriendlyName,
                strConverterIdentifier,
                eConversionType,
                strLhsEncodingId,
                strRhsEncodingId,
                lProcessTypeFlags,
                bIsInRepository);
            System.Diagnostics.Debug.WriteLine("Initialized base.");

            // if we're editing, then set the Converter Spec and say it's unmodified
            if (m_bEditMode)
            {
                System.Diagnostics.Debug.WriteLine("Edit mode");
                System.Diagnostics.Debug.Assert(!String.IsNullOrEmpty(ConverterIdentifier));
                //listBoxTranslitName.SelectedValue = ConverterIdentifier;
                for (int i = 0; i < translitIDs.Length; i++)
                {
                    if (translitIDs[i] == ConverterIdentifier)
                    {
                        listBoxTranslitName.SelectedIndex = i;
                        break;
                    }
                }
                IsModified = false;
            }

            m_bInitialized = true;
            System.Diagnostics.Debug.WriteLine("IcuTranslitAutoConfigDialog ctor END");
        }

        public IcuTranslitAutoConfigDialog (
            IEncConverters aECs,
            string strFriendlyName,
            string strConverterIdentifier,
            ConvType eConversionType,
            string strTestData)
        {
            InitializeComponent();
            fillListBox();
            base.Initialize (
                aECs,
                strFriendlyName,
                strConverterIdentifier,
                eConversionType,
                strTestData);
            m_bInitialized = true;
        }

        private System.Windows.Forms.Label labelTranslitName;
        private System.Windows.Forms.ListBox listBoxTranslitName;

        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel1;

        // This code was NOT generated!
        // So feel free to modify it as needed.
        private void InitializeComponent()
        {
            this.tabControl.SuspendLayout();
            this.tabPageSetup.SuspendLayout();
            this.SuspendLayout();

            // 
            // Script file
            // 

            this.labelTranslitName = new System.Windows.Forms.Label();
            this.labelTranslitName.Text = "Transliterator:";
            this.labelTranslitName.Anchor = System.Windows.Forms.AnchorStyles.Right;
            this.labelTranslitName.AutoSize = true;
            this.labelTranslitName.Name = "labelTranslitName";
            this.labelTranslitName.TabIndex = 0;

            this.listBoxTranslitName = new System.Windows.Forms.ListBox();
            this.listBoxTranslitName.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.listBoxTranslitName.Location = new System.Drawing.Point(88, 14);
            this.listBoxTranslitName.Name = "listBoxTranslitName";
            this.listBoxTranslitName.Size = new System.Drawing.Size(400, 400);
            this.listBoxTranslitName.TabIndex = 1;
            this.listBoxTranslitName.SelectedIndexChanged +=
                new System.EventHandler (
                    this.listBoxTranslitName_SelectedIndexChanged);

            // 
            // Panel and Dialog Window
            // 

            this.tableLayoutPanel1 =
                new System.Windows.Forms.TableLayoutPanel();
            this.tableLayoutPanel1.SuspendLayout();
            this.tableLayoutPanel1.ColumnCount = 4;
            this.tableLayoutPanel1.RowCount    = 2;
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle()); // an empty row
            this.tableLayoutPanel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanel1.Location = new System.Drawing.Point(3, 3);
            this.tableLayoutPanel1.Name = "tableLayoutPanel1";
            this.tableLayoutPanel1.Size = new System.Drawing.Size(620, 394);
            this.tableLayoutPanel1.TabIndex = 1;
            this.tableLayoutPanel1.Controls.Add(this.labelTranslitName,      0, 0);
            this.tableLayoutPanel1.Controls.Add(this.listBoxTranslitName,      1, 0);

            this.tabPageSetup.Controls.Add(this.tableLayoutPanel1);

            this.ClientSize = new System.Drawing.Size(634, 479);
            this.Name = "IcuTranslitAutoConfigDialog";
            this.tabControl.ResumeLayout(false);
            this.tabPageSetup.ResumeLayout(false);
            this.tableLayoutPanel1.ResumeLayout(false);
            this.tableLayoutPanel1.PerformLayout();
            this.ResumeLayout(false);
        }

        private void fillListBox()
        {
            this.listBoxTranslitName.Items.Clear();
            int count = 0;
            try {
                count = CppConverterNameList_start();
            } catch (DllNotFoundException exc) {
                throw new Exception("Failed to load .so file. Check path.");
            } catch (EntryPointNotFoundException exc) {
                throw new Exception("Failed to find function in .so file.");
            }

            // Store the IDs in an array, and put the display names in the
            // list box.
            translitIDs = new string[count];
            for (int i = 0; i < count; i++)
            {
                translitIDs[i] = CppConverterNameList_next();
                string translitName   = CppGetDisplayName(translitIDs[i]);
                this.listBoxTranslitName.Items.Add(translitName);
            }

            // Sort listbox and corresponding translitIDs by display name.
            ListBox lb = this.listBoxTranslitName;  // shorter nickname
            if (lb.Items.Count > 1)
            {
                bool swapped;
                do
                {
                    int counter = lb.Items.Count - 1;
                    swapped = false;
                    
                    while (counter > 0)
                    {
                        // Compare the items
                        if (String.Compare(lb.Items[counter].ToString(),
                                           lb.Items[counter-1].ToString()) < 0)
                        {
                            // Swap the items.
                            object tempName = lb.Items[counter];
                            lb.Items[counter]   = lb.Items[counter-1];
                            lb.Items[counter-1] = tempName;
                            string tempID = translitIDs[counter];
                            translitIDs[counter]   = translitIDs[counter-1];
                            translitIDs[counter-1] = tempID;
                            swapped = true;
                        }
                        // Decrement the counter.
                        counter -= 1;
                    }
                }
                while((swapped==true));
            }
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
            System.Diagnostics.Debug.WriteLine("OnApply() BEGIN");
            // Get the converter identifier from the Setup tab controls.
            ConverterIdentifier = translitIDs[listBoxTranslitName.SelectedIndex];
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
                    MessageBox.Show(this, "Choose a transliterator first!", EncConverters.cstrCaption);
                    return false;
                }
            }
            System.Diagnostics.Debug.WriteLine("OnApply() END");
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
            get { return typeof(IcuTranslitEncConverter).FullName; }
        }

        protected override string ImplType
        {
            get { return EncConverters.strTypeSILicuTrans; }
        }

        protected override string DefaultFriendlyName
        {
            // as the default, make it the same as the table name (w/o extension)
            get { return ConverterIdentifier; }
        }

        private void listBoxTranslitName_SelectedIndexChanged (
            object sender, EventArgs e)
        {
            if (m_bInitialized) // but only do this after we're already initialized
            {
                ListBox lb = this.listBoxTranslitName;  // shorter nickname
                System.Diagnostics.Debug.WriteLine(lb.SelectedIndex.ToString()+ "\n" + lb.GetItemText(lb.SelectedItem),"lb_SelectedIndexChanged"); 
                IsModified = true;
                //ProcessType &= ~(int)ProcessTypeFlags.SpellingFixerProject;
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
                    listBoxTranslitName.Text = ConverterIdentifier = m_aEC.ConverterIdentifier;
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

