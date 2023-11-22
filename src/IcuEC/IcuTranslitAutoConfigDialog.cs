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
using ECInterfaces;
using IcuEC.Properties;
using System.Linq;

// for IEncConverter

namespace SilEncConverters40
{
    public class IcuTranslitAutoConfigDialog : SilEncConverters40.AutoConfigDialog
    {
        #region DLLImport Statements
        [DllImport("IcuTranslitEC.dll", EntryPoint = "IcuTranslitEC_ConverterNameList_start", CallingConvention = CallingConvention.Cdecl)]
        static extern unsafe int CppConverterNameList_start();

        static string CppConverterNameList_next()
        {
            if (Util.IsUnix)
            {
                return CppConverterNameList_next_Linux();
            }
            else
            {
                return CppConverterNameList_next_Windows();
            }
        }

        static string CppGetDisplayName(string strID)
        {
            if (Util.IsUnix)
            { return CppGetDisplayName_Linux(strID); }
            else
            {
                return CppGetDisplayName_Windows(strID);
            }
        }

        [DllImport("IcuTranslitEC.dll", EntryPoint = "IcuTranslitEC_ConverterNameList_next", CallingConvention = CallingConvention.Cdecl)]
        [return: MarshalAs(UnmanagedType.LPStr)]
        static extern string CppConverterNameList_next_Linux();

        [DllImport("IcuTranslitEC.dll", EntryPoint = "IcuTranslitEC_GetDisplayName", CallingConvention = CallingConvention.Cdecl)]
        [return: MarshalAs(UnmanagedType.LPStr)]
        static extern string CppGetDisplayName_Linux(
            [MarshalAs(UnmanagedType.LPStr)] string strID);

        [DllImport("IcuTranslitEC.dll", EntryPoint = "IcuTranslitEC_ConverterNameList_next", CallingConvention = CallingConvention.Cdecl)]
        [return: MarshalAs(UnmanagedType.BStr)]
        static extern string CppConverterNameList_next_Windows();

        [DllImport("IcuTranslitEC.dll", EntryPoint = "IcuTranslitEC_GetDisplayName", CallingConvention = CallingConvention.Cdecl)]
        [return: MarshalAs(UnmanagedType.BStr)]
        static extern string CppGetDisplayName_Windows([MarshalAs(UnmanagedType.LPStr)] string strID);

        #endregion DLLImport Statements

        private TableLayoutPanel tableLayoutPanel1;
        private ListBox listBoxTranslitName;
        private RadioButton radioButtonBuiltIn;
        private RadioButton radioButtonCustom;
        private TextBox textBoxCustomTransliterator;
        private ComboBox comboBoxPreviousCustomTransliterators;
        private Label label1;
        private Button buttonDeletePreviousCustomTransliterators;  // set at the end of Initialize (to block certain events until we're ready for them)
        private Dictionary<string, string> mapTransliteratorIdsToFriendlyNames;

        public IcuTranslitAutoConfigDialog(
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
            base.Initialize(
                aECs,
                IcuTranslitEncConverter.strHtmlFilename,
                strDisplayName,
                strFriendlyName,
                strConverterIdentifier,
                eConversionType,
                strLhsEncodingId,
                strRhsEncodingId,
                lProcessTypeFlags | (int)ProcessTypeFlags.ICUTransliteration | (int)ProcessTypeFlags.Transliteration,
                bIsInRepository);
            Util.DebugWriteLine(this, "Initialized base.");

			mapTransliteratorIdsToFriendlyNames = GetSupportedTransliterators();
			FillListBox(mapTransliteratorIdsToFriendlyNames);

			// if we're editing, then set the Converter Spec and say it's unmodified
			if (m_bEditMode)
            {
                Util.DebugWriteLine(this, "Edit mode");
                System.Diagnostics.Debug.Assert(!String.IsNullOrEmpty(ConverterIdentifier));

				// first see if the converter identifier is one of the values in the text box
				var value = mapTransliteratorIdsToFriendlyNames.FirstOrDefault(kvp => kvp.Key == ConverterIdentifier || kvp.Value == ConverterIdentifier);
				if (!value.Equals(default(KeyValuePair<string, string>)))
				{
					listBoxTranslitName.SelectedItem = value.Value;
				}
				else
				{
					// it's a custom transliterator, so set the other radio box and load the text box as is
					textBoxCustomTransliterator.Text = ConverterIdentifier;
					radioButtonCustom.Checked = true;
					radioButtonCustom.PerformClick();
				}

                IsModified = false;
            }

            LoadComboBoxFromSettings(comboBoxPreviousCustomTransliterators, Settings.Default.RecentCustomTransliterators);

            if (comboBoxPreviousCustomTransliterators.Items.Count > 0)
                comboBoxPreviousCustomTransliterators.SelectedIndex = 0;

            m_bInitialized = true;
            Util.DebugWriteLine(this, "END");
        }

		private Dictionary<string, string> GetSupportedTransliterators()
		{
			var count = CppConverterNameList_start();

			var map = new Dictionary<string, string>();
			for (int i = 0; i < count; i++)
			{
				var translitId = CppConverterNameList_next();
				var translitName = CppGetDisplayName(translitId);
				map[translitId] = translitName;
			}
			return map;
		}

		public IcuTranslitAutoConfigDialog(
            IEncConverters aECs,
            string strFriendlyName,
            string strConverterIdentifier,
            ConvType eConversionType,
            string strTestData)
        {
            InitializeComponent();
            base.Initialize(
                aECs,
                strFriendlyName,
                strConverterIdentifier,
                eConversionType,
                strTestData);
            m_bInitialized = true;

			mapTransliteratorIdsToFriendlyNames = GetSupportedTransliterators();
			FillListBox(mapTransliteratorIdsToFriendlyNames);
		}

		// This code was NOT generated!
		// So feel free to modify it as needed.
		private void InitializeComponent()
        {
            this.tableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
            this.listBoxTranslitName = new System.Windows.Forms.ListBox();
            this.radioButtonBuiltIn = new System.Windows.Forms.RadioButton();
            this.radioButtonCustom = new System.Windows.Forms.RadioButton();
            this.textBoxCustomTransliterator = new System.Windows.Forms.TextBox();
            this.comboBoxPreviousCustomTransliterators = new System.Windows.Forms.ComboBox();
            this.label1 = new System.Windows.Forms.Label();
            this.buttonDeletePreviousCustomTransliterators = new System.Windows.Forms.Button();
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
            this.tableLayoutPanel1.Controls.Add(this.listBoxTranslitName, 1, 1);
            this.tableLayoutPanel1.Controls.Add(this.radioButtonBuiltIn, 1, 0);
            this.tableLayoutPanel1.Controls.Add(this.radioButtonCustom, 1, 2);
            this.tableLayoutPanel1.Controls.Add(this.textBoxCustomTransliterator, 1, 3);
            this.tableLayoutPanel1.Controls.Add(this.comboBoxPreviousCustomTransliterators, 1, 4);
            this.tableLayoutPanel1.Controls.Add(this.label1, 0, 4);
            this.tableLayoutPanel1.Controls.Add(this.buttonDeletePreviousCustomTransliterators, 2, 4);
            this.tableLayoutPanel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanel1.Location = new System.Drawing.Point(3, 3);
            this.tableLayoutPanel1.Name = "tableLayoutPanel1";
            this.tableLayoutPanel1.RowCount = 6;
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 50F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 50F));
            this.tableLayoutPanel1.Size = new System.Drawing.Size(596, 394);
            this.tableLayoutPanel1.TabIndex = 1;
            //
            // listBoxTranslitName
            //
            this.listBoxTranslitName.Dock = System.Windows.Forms.DockStyle.Fill;
            this.listBoxTranslitName.Location = new System.Drawing.Point(128, 26);
            this.listBoxTranslitName.Name = "listBoxTranslitName";
            this.listBoxTranslitName.Size = new System.Drawing.Size(370, 212);
            this.listBoxTranslitName.TabIndex = 1;
			this.listBoxTranslitName.Sorted = true;
            this.listBoxTranslitName.SelectedIndexChanged += new System.EventHandler(this.listBoxTranslitName_SelectedIndexChanged);
            //
            // radioButtonBuiltIn
            //
            this.radioButtonBuiltIn.AutoSize = true;
            this.radioButtonBuiltIn.Checked = true;
            this.radioButtonBuiltIn.Location = new System.Drawing.Point(128, 3);
            this.radioButtonBuiltIn.Name = "radioButtonBuiltIn";
            this.radioButtonBuiltIn.Size = new System.Drawing.Size(121, 17);
            this.radioButtonBuiltIn.TabIndex = 0;
            this.radioButtonBuiltIn.TabStop = true;
            this.radioButtonBuiltIn.Text = "&Built-in transliterators";
            this.radioButtonBuiltIn.UseVisualStyleBackColor = true;
            this.radioButtonBuiltIn.Click += new System.EventHandler(this.radioButtonBuiltIn_Click);
            //
            // radioButtonCustom
            //
            this.radioButtonCustom.AutoSize = true;
            this.radioButtonCustom.Location = new System.Drawing.Point(128, 244);
            this.radioButtonCustom.Name = "radioButtonCustom";
            this.radioButtonCustom.Size = new System.Drawing.Size(120, 17);
            this.radioButtonCustom.TabIndex = 2;
            this.radioButtonCustom.TabStop = true;
            this.radioButtonCustom.Text = "&Custom transliterator";
            this.radioButtonCustom.UseVisualStyleBackColor = true;
            this.radioButtonCustom.CheckedChanged += new System.EventHandler(this.radioButtonCustom_CheckedChanged);
            //
            // textBoxCustomTransliterator
            //
            this.textBoxCustomTransliterator.Dock = System.Windows.Forms.DockStyle.Fill;
            this.textBoxCustomTransliterator.Enabled = false;
            this.textBoxCustomTransliterator.Location = new System.Drawing.Point(128, 267);
            this.textBoxCustomTransliterator.Name = "textBoxCustomTransliterator";
            this.textBoxCustomTransliterator.Size = new System.Drawing.Size(370, 20);
            this.textBoxCustomTransliterator.TabIndex = 3;
            this.textBoxCustomTransliterator.TextChanged += new System.EventHandler(this.textBoxCustomTransliterator_TextChanged);
            //
            // comboBoxPreviousCustomTransliterators
            //
            this.comboBoxPreviousCustomTransliterators.Dock = System.Windows.Forms.DockStyle.Fill;
            this.comboBoxPreviousCustomTransliterators.Enabled = false;
            this.comboBoxPreviousCustomTransliterators.FormattingEnabled = true;
            this.comboBoxPreviousCustomTransliterators.Location = new System.Drawing.Point(128, 317);
            this.comboBoxPreviousCustomTransliterators.Name = "comboBoxPreviousCustomTransliterators";
            this.comboBoxPreviousCustomTransliterators.Size = new System.Drawing.Size(370, 21);
            this.comboBoxPreviousCustomTransliterators.TabIndex = 4;
            this.comboBoxPreviousCustomTransliterators.SelectedIndexChanged += new System.EventHandler(this.comboBoxPreviousCustomTransliterators_SelectedIndexChanged);
            //
            // label1
            //
            this.label1.Anchor = System.Windows.Forms.AnchorStyles.Right;
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(3, 322);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(119, 13);
            this.label1.TabIndex = 0;
            this.label1.Text = "Previous Custom Rules:";
            //
            // buttonDeletePreviousCustomTransliterators
            //
            this.buttonDeletePreviousCustomTransliterators.Enabled = false;
            this.buttonDeletePreviousCustomTransliterators.Location = new System.Drawing.Point(504, 317);
            this.buttonDeletePreviousCustomTransliterators.Name = "buttonDeletePreviousCustomTransliterators";
            this.buttonDeletePreviousCustomTransliterators.Size = new System.Drawing.Size(89, 23);
            this.buttonDeletePreviousCustomTransliterators.TabIndex = 5;
            this.buttonDeletePreviousCustomTransliterators.Text = "Delete";
            this.buttonDeletePreviousCustomTransliterators.UseVisualStyleBackColor = true;
            this.buttonDeletePreviousCustomTransliterators.Click += new System.EventHandler(this.buttonDeletePreviousCustomTransliterators_Click);
            //
            // IcuTranslitAutoConfigDialog
            //
            this.ClientSize = new System.Drawing.Size(634, 479);
            this.Name = "IcuTranslitAutoConfigDialog";
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

        private void FillListBox(Dictionary<string, string> mapTransliteratorIdsToFriendlyNames)
        {
            // Put the display names in the list box.
			foreach (var friendlyName in mapTransliteratorIdsToFriendlyNames.Values)
			{
				this.listBoxTranslitName.Items.Add(friendlyName);
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
            Util.DebugWriteLine(this, "BEGIN");

            // Get the converter identifier from the Setup tab controls.
            if (radioButtonBuiltIn.Checked && (listBoxTranslitName.SelectedIndex != -1))
                ConverterIdentifier = mapTransliteratorIdsToFriendlyNames.FirstOrDefault(kvp => kvp.Value == (string)listBoxTranslitName.SelectedItem).Key;
            else if (radioButtonCustom.Checked)
                ConverterIdentifier = textBoxCustomTransliterator.Text;

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

                if (radioButtonCustom.Checked &&
                    !Settings.Default.RecentCustomTransliterators.Contains(ConverterIdentifier))
                {
                    Settings.Default.RecentCustomTransliterators.Add(ConverterIdentifier);
                    Settings.Default.Save();
                    LoadComboBoxFromSettings(comboBoxPreviousCustomTransliterators, Settings.Default.RecentCustomTransliterators);
                }
            }

            Util.DebugWriteLine(this, "END");
            return base.OnApply();
        }

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

        private void listBoxTranslitName_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (!m_bInitialized)
                return;

            var lb = this.listBoxTranslitName;  // shorter nickname
            Util.DebugWriteLine(this,
                                lb.SelectedIndex.ToString() + "\n" + lb.GetItemText(lb.SelectedItem) + "lb_SelectedIndexChanged");
            IsModified = true;
        }

        private void buttonDeletePreviousCustomTransliterators_Click(object sender, EventArgs e)
        {
            DeleteSelectedItemFromComboBoxAndUpdateSettings(comboBoxPreviousCustomTransliterators,
                                                            Settings.Default.RecentCustomTransliterators);

            Settings.Default.Save();
        }

        private void radioButtonBuiltIn_Click(object sender, EventArgs e)
        {
            listBoxTranslitName.Enabled = true;
            textBoxCustomTransliterator.Enabled =
                comboBoxPreviousCustomTransliterators.Enabled =
                buttonDeletePreviousCustomTransliterators.Enabled = false;
			IsModified = true;
        }

        private void radioButtonCustom_CheckedChanged(object sender, EventArgs e)
        {
			var value = radioButtonCustom.Checked;
            listBoxTranslitName.Enabled = !value;
            textBoxCustomTransliterator.Enabled =
                comboBoxPreviousCustomTransliterators.Enabled =
                buttonDeletePreviousCustomTransliterators.Enabled = value;
			IsModified = true;
		}

		private void comboBoxPreviousCustomTransliterators_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (!m_bInitialized)
                return;

            var strConverterSpec = comboBoxPreviousCustomTransliterators.SelectedItem.ToString();
            textBoxCustomTransliterator.Text = strConverterSpec;
            IsModified = true;
        }

        private void textBoxCustomTransliterator_TextChanged(object sender, EventArgs e)
        {
            if (!m_bInitialized)
                return;

            IsModified = true;
        }
    }
}

