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

namespace SilEncConverters40
{
	public partial class PyScriptAutoConfigDialog : PythonAutoConfigDialog
	{
		public PyScriptAutoConfigDialog(
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
			Util.DebugWriteLine(this, "ctor1 BEGIN");
			InitializeComponent();
			Util.DebugWriteLine(this, "Initialized component.");
			base.Initialize(
				aECs,
				PyScriptEncConverter.strHtmlFilename,
				strDisplayName,
				strFriendlyName,
				strConverterIdentifier,
				eConversionType,
				strLhsEncodingId,
				strRhsEncodingId,
				lProcessTypeFlags | (int)ProcessTypeFlags.PythonScript,
				bIsInRepository);
			Util.DebugWriteLine(this, "Initialized base.");
			// if we're editing a CC table/spellfixer project, then set the
			// Converter Spec and say it's unmodified
			if (m_bEditMode)
			{
				Util.DebugWriteLine(this, "Edit mode");
				System.Diagnostics.Debug.Assert(
					!String.IsNullOrEmpty(ConverterIdentifier));
                textBoxFileSpec.Text = ConverterIdentifier;
				IsModified = false;
			}
			m_bInitialized = true;
			Util.DebugWriteLine(this, "END");
		}

		public PyScriptAutoConfigDialog(
			IEncConverters aECs,
			string strFriendlyName,
			string strConverterIdentifier,
			ConvType eConversionType,
			string strTestData)
		{
			Util.DebugWriteLine(this, "ctor2 BEGIN");
			InitializeComponent();
			base.Initialize(
				aECs,
				strFriendlyName,
				strConverterIdentifier,
				eConversionType,
				strTestData);
			Util.DebugWriteLine(this, "END");
		}

		protected void UpdateUI(bool bVisible)
		{
			Util.DebugWriteLine(this, "BEGIN");
			buttonSaveInRepository.Visible =
				groupBoxExpects.Visible =
				groupBoxReturns.Visible = bVisible;
			Util.DebugWriteLine(this, "END");
		}

		protected override void SetConvTypeControls()
		{
			SetRbValuesFromConvType(radioButtonExpectsUnicode, radioButtonExpectsLegacy, radioButtonReturnsUnicode,
				radioButtonReturnsLegacy);
		}

		// this method is called either when the user clicks the "Apply" or "OK" buttons *OR* if she
		//  tries to switch to the Test or Advanced tab. This is the dialog's one opportunity
		//  to make sure that the user has correctly configured a legitimate converter.
		protected override bool OnApply()
		{
			Util.DebugWriteLine(this, "BEGIN");
			// Get the converter identifier from the Setup tab controls.
			ConverterIdentifier = textBoxFileSpec.Text;
			SetConvTypeFromRbControls(radioButtonExpectsUnicode, radioButtonExpectsLegacy,
				radioButtonReturnsUnicode, radioButtonReturnsLegacy);

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
			Util.DebugWriteLine(this, "END");
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
				openFileDialogBrowse.InitialDirectory = Util.CommonAppDataPath() + EncConverters.strDefMapsTablesPath;

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

		private void radioButton_CheckedChanged(object sender, EventArgs e)
		{
			IsModified = true;
		}
	}
}

