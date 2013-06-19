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
		// On Linux looks for libIcuConvEC.so (adds lib- and -.so)
		[DllImport("IcuConvEC", EntryPoint = "IcuConvEC_ConverterNameList_start", CallingConvention = CallingConvention.Cdecl)]
		static extern unsafe int CppConverterNameList_start();

		[DllImport("IcuConvEC", EntryPoint = "IcuConvEC_ConverterNameList_next", CallingConvention = CallingConvention.Cdecl)]
		static extern unsafe string CppConverterNameList_next();

		[DllImport("IcuConvEC", EntryPoint = "IcuConvEC_GetDisplayName", CallingConvention = CallingConvention.Cdecl)]
		static extern unsafe string CppGetDisplayName(string strID);
		#endregion DLLImport Statements

		///<summary>
		/// set at the end of Initialize (to block certain events until we're ready for them)
		///</summary>
		protected bool m_bInitialized = false;
		private string [] translitIDs;

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
			EncConverter.DebugWriteLine(this, "BEGIN");
			InitializeComponent();
			fillListBox();
			EncConverter.DebugWriteLine(this, "Initialized component.");
			base.Initialize (
				aECs,
				IcuConvEncConverter.strHtmlFilename,
				strDisplayName,
				strFriendlyName,
				strConverterIdentifier,
				eConversionType,
				strLhsEncodingId,
				strRhsEncodingId,
				lProcessTypeFlags,
				bIsInRepository);
			EncConverter.DebugWriteLine(this, "Initialized base.");

			// if we're editing, then set the Converter Spec and say it's unmodified
			if (m_bEditMode)
			{
				EncConverter.DebugWriteLine(this, "Edit mode");
				System.Diagnostics.Debug.Assert(!String.IsNullOrEmpty(ConverterIdentifier));
				//listBoxConvName.SelectedValue = ConverterIdentifier;
				for (int i = 0; i < translitIDs.Length; i++)
				{
					if (translitIDs[i] == ConverterIdentifier)
					{
						listBoxConvName.SelectedIndex = i;
						break;
					}
				}
				IsModified = false;
			}

			m_bInitialized = true;
			EncConverter.DebugWriteLine(this, "END");
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
			this.tabControl.SuspendLayout();
			this.tabPageSetup.SuspendLayout();
			this.SuspendLayout();

			// 
			// Script file
			// 

			this.labelConvName = new System.Windows.Forms.Label();
			this.labelConvName.Text = "Converator:";
			this.labelConvName.Anchor = System.Windows.Forms.AnchorStyles.Right;
			this.labelConvName.AutoSize = true;
			this.labelConvName.Name = "labelConvName";
			this.labelConvName.TabIndex = 0;

			this.listBoxConvName = new System.Windows.Forms.ListBox();
			this.listBoxConvName.Anchor = System.Windows.Forms.AnchorStyles.Left;
			this.listBoxConvName.Location = new System.Drawing.Point(88, 14);
			this.listBoxConvName.Name = "listBoxConvName";
			this.listBoxConvName.Size = new System.Drawing.Size(400, 400);
			this.listBoxConvName.TabIndex = 1;
			this.listBoxConvName.SelectedIndexChanged +=
				new System.EventHandler (
					this.listBoxConvName_SelectedIndexChanged);

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
			this.tableLayoutPanel1.Controls.Add(this.labelConvName,      0, 0);
			this.tableLayoutPanel1.Controls.Add(this.listBoxConvName,      1, 0);

			this.tabPageSetup.Controls.Add(this.tableLayoutPanel1);

			this.ClientSize = new System.Drawing.Size(634, 479);
			this.Name = "IcuConvAutoConfigDialog";
			this.tabControl.ResumeLayout(false);
			this.tabPageSetup.ResumeLayout(false);
			this.tableLayoutPanel1.ResumeLayout(false);
			this.tableLayoutPanel1.PerformLayout();
			this.ResumeLayout(false);
		}

		private void fillListBox()
		{
			this.listBoxConvName.Items.Clear();
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
			EncConverter.DebugWriteLine(this, "BEGIN");
			// Get the converter identifier from the Setup tab controls.
			ConverterIdentifier = translitIDs[listBoxConvName.SelectedIndex];
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
			EncConverter.DebugWriteLine(this, "END");
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
				EncConverter.DebugWriteLine(this,
                    lb.SelectedIndex.ToString() + "\n" + lb.GetItemText(lb.SelectedItem) + "lb_SelectedIndexChanged"); 
				IsModified = true;
			}
		}
	}
}
