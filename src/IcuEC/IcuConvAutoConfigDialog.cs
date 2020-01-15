// Created by Steve McConnel Feb 2, 2012 by copying and editing IcuTranslitAutoConfigDialog.cs

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
	public class IcuConvAutoConfigDialog : SilEncConverters40.AutoConfigDialog
	{
		#region DLLImport Statements
		[DllImport("IcuConvEC.dll", EntryPoint = "IcuConvEC_ConverterNameList_start", CallingConvention = CallingConvention.Cdecl)]
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
            {
                return CppGetDisplayName_Linux(strID);
            }
            else
            {
                return CppGetDisplayName_Windows(strID);
            }
        }

        [DllImport("IcuConvEC.dll", EntryPoint = "IcuConvEC_ConverterNameList_next", CallingConvention = CallingConvention.Cdecl)]
		[return : MarshalAs(UnmanagedType.LPStr)]
		static extern string CppConverterNameList_next_Linux();

		[DllImport("IcuConvEC.dll", EntryPoint = "IcuConvEC_GetDisplayName", CallingConvention = CallingConvention.Cdecl)]
		[return: MarshalAs(UnmanagedType.LPStr)]
		static extern string CppGetDisplayName_Linux(
			[MarshalAs(UnmanagedType.LPStr)] string strID);

        [DllImport("IcuConvEC.dll", EntryPoint = "IcuConvEC_ConverterNameList_next", CallingConvention = CallingConvention.Cdecl)]
        [return: MarshalAs(UnmanagedType.BStr)]
        static extern string CppConverterNameList_next_Windows();

        [DllImport("IcuConvEC.dll", EntryPoint = "IcuConvEC_GetDisplayName", CallingConvention = CallingConvention.Cdecl)]
        [return: MarshalAs(UnmanagedType.BStr)]
        static extern string CppGetDisplayName_Windows([MarshalAs(UnmanagedType.LPStr)] string strID);

        #endregion DLLImport Statements

		public IcuConvAutoConfigDialog (
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
			fillListBox();
			Util.DebugWriteLine(this, "Initialized component.");
			base.Initialize (
				aECs,
				IcuConvEncConverter.strHtmlFilename,
				strDisplayName,
				strFriendlyName,
				strConverterIdentifier,
				eConversionType,
				strLhsEncodingId,
				strRhsEncodingId,
				lProcessTypeFlags | (int)ProcessTypeFlags.ICUConverter,
				bIsInRepository);
			Util.DebugWriteLine(this, "Initialized base.");

			// if we're editing, then set the Converter Spec and say it's unmodified
			if (m_bEditMode)
			{
				Util.DebugWriteLine(this, "Edit mode");
				System.Diagnostics.Debug.Assert(!String.IsNullOrEmpty(ConverterIdentifier));
			    var index = listBoxConvName.FindString(ConverterIdentifier);
			    listBoxConvName.SelectedIndex = index;
				IsModified = false;
			}

			m_bInitialized = true;
			Util.DebugWriteLine(this, "END");
		}

		public IcuConvAutoConfigDialog (
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

		private System.Windows.Forms.Label labelConvName;
		private System.Windows.Forms.ListBox listBoxConvName;

		private System.Windows.Forms.TableLayoutPanel tableLayoutPanel1;

		// This code was NOT generated!
		// So feel free to modify it as needed.
		private void InitializeComponent()
		{
            this.tableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
            this.labelConvName = new System.Windows.Forms.Label();
            this.listBoxConvName = new System.Windows.Forms.ListBox();
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
            this.tableLayoutPanel1.ColumnCount = 2;
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.tableLayoutPanel1.Controls.Add(this.labelConvName, 0, 0);
            this.tableLayoutPanel1.Controls.Add(this.listBoxConvName, 1, 0);
            this.tableLayoutPanel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanel1.Location = new System.Drawing.Point(3, 3);
            this.tableLayoutPanel1.Name = "tableLayoutPanel1";
            this.tableLayoutPanel1.RowCount = 1;
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 20F));
            this.tableLayoutPanel1.Size = new System.Drawing.Size(596, 394);
            this.tableLayoutPanel1.TabIndex = 1;
            // 
            // labelConvName
            // 
            this.labelConvName.Anchor = System.Windows.Forms.AnchorStyles.Right;
            this.labelConvName.AutoSize = true;
            this.labelConvName.Location = new System.Drawing.Point(3, 190);
            this.labelConvName.Name = "labelConvName";
            this.labelConvName.Size = new System.Drawing.Size(56, 13);
            this.labelConvName.TabIndex = 0;
            this.labelConvName.Text = "Converter:";
            // 
            // listBoxConvName
            // 
            this.listBoxConvName.Dock = System.Windows.Forms.DockStyle.Fill;
            this.listBoxConvName.Location = new System.Drawing.Point(65, 3);
            this.listBoxConvName.Name = "listBoxConvName";
            this.listBoxConvName.Size = new System.Drawing.Size(528, 388);
            this.listBoxConvName.TabIndex = 1;
            this.listBoxConvName.SelectedIndexChanged += new System.EventHandler(this.listBoxConvName_SelectedIndexChanged);
            // 
            // IcuConvAutoConfigDialog
            // 
            this.ClientSize = new System.Drawing.Size(634, 479);
            this.Name = "IcuConvAutoConfigDialog";
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

		private void fillListBox()
		{
			this.listBoxConvName.Items.Clear();
#if true
		    var list = IcuConvEncConverter.GetAvailableConverterSpecs();
            list.ForEach(i => listBoxConvName.Items.Add(i));
#else
			int count = 0;
			
            count = CppConverterNameList_start();

			// Store the IDs in an array, and put the display names in the
			// list box.
			translitIDs = new string[count];
			for (int i = 0; i < count; i++)
			{
				translitIDs[i] = CppConverterNameList_next();
				string translitName   = CppGetDisplayName(translitIDs[i]);
				this.listBoxConvName.Items.Add(translitName);
			}

			// Sort listbox and corresponding translitIDs by display name.
			ListBox lb = this.listBoxConvName;  // shorter nickname
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
#endif
		}

		protected override void SetConvTypeControls()
		{
		}

		// this method is called either when the user clicks the "Apply" or "OK" buttons *OR*
		// if she tries to switch to the Test or Advanced tab. This is the dialog's one
		// opportunity to make sure that the user has correctly configured a legitimate
		// converter.
		protected override bool OnApply()
		{
			Util.DebugWriteLine(this, "BEGIN");

			if (listBoxConvName.SelectedItem == null)
				return false;
			// Get the converter identifier from the Setup tab controls.
			var strConverter = listBoxConvName.SelectedItem as String;
		    var nIndex = strConverter.IndexOf(" (aliases: ");
            if (nIndex != -1)
                strConverter = strConverter.Substring(0, nIndex);
            
            ConverterIdentifier = strConverter;

			//SetConvTypeFromRbControls(radioButtonExpectsUnicode, radioButtonExpectsLegacy,
			//    radioButtonReturnsUnicode, radioButtonReturnsLegacy);

			// if we're actually on the setup tab, then do some further checking as well.
			if (tabControl.SelectedTab == tabPageSetup)
			{
				// only do these message boxes if we're on the Setup tab itself, because if
				// this OnApply is being called as a result of the user switching to the Test
				// tab, that code will already put up an error message and we don't need two
				// error messages.
				if (String.IsNullOrEmpty(ConverterIdentifier))
				{
					MessageBox.Show(this, "Choose a transliterator first!", EncConverters.cstrCaption);
					return false;
				}
			}
			Util.DebugWriteLine(this, "END");
			return base.OnApply();
		}

		protected override string ProgID
		{
			get { return typeof(IcuConvEncConverter).FullName; }
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

		private void listBoxConvName_SelectedIndexChanged (
			object sender, EventArgs e)
		{
			if (m_bInitialized) // but only do this after we're already initialized
			{
				ListBox lb = this.listBoxConvName;  // shorter nickname
				Util.DebugWriteLine(this,
                    lb.SelectedIndex.ToString() + "\n" + lb.GetItemText(lb.SelectedItem) + "lb_SelectedIndexChanged"); 
				IsModified = true;
			}
		}
	}
}
