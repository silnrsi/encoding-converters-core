// Created by Bob Eaton on Oct 21 2023
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
using System.Linq;

namespace SilEncConverters40
{
    public partial class Py3ScriptAutoConfigDialog : PythonAutoConfigDialog
    {
		private const string OpenFileDialogTitleBrowseForDll = "Browse for Python 3 DLL";
		private const string OpenFileDialogTitleBrowseForScript = "Browse for Python Script";

		private const string OpenFileDialogDefaultExtBrowseForDll = "dll";
		private const string OpenFileDialogDefaultExtBrowseForScript = "py";

		private const string OpenFileDialogFilterBrowseForDll = "Python 3 DLL (Python3 DLL)|Python3*.dll";
		private const string OpenFileDialogFilterBrowseForScript = "Python scripts (*.py)|*.py";

		public Py3ScriptAutoConfigDialog(
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
                Py3ScriptEncConverter.strHtmlFilename,
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

                textBoxFileSpec.Text = Py3ScriptEncConverter.ScriptPath(ConverterIdentifier);
                textBoxPython3Path.Text = Py3ScriptEncConverter.DistroPath(ConverterIdentifier);

                IsModified = false;
            }
            m_bInitialized = true;
            Util.DebugWriteLine(this, "END");
        }

        public Py3ScriptAutoConfigDialog(
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

        // this method is called either when the user clicks the "Apply" or "OK" buttons *OR* if she
        //  tries to switch to the Test or Advanced tab. This is the dialog's one opportunity
        //  to make sure that the user has correctly configured a legitimate converter.
        protected override bool OnApply()
        {
            Util.DebugWriteLine(this, "BEGIN");
            // Get the converter identifier from the Setup tab controls.
            var scriptPath = textBoxFileSpec.Text;
            var pythonDistroPath = textBoxPython3Path.Text;

            // if we're actually on the setup tab, then do some further checking as well.
            if (tabControl.SelectedTab == tabPageSetup)
            {
                // only do these message boxes if we're on the Setup tab itself, because if this OnApply
                //  is being called as a result of the user switching to the Test tab, that code will
                //  already put up an error message and we don't need two error messages.
                if (String.IsNullOrEmpty(scriptPath))
                {
                    MessageBox.Show(this, "Choose a Python script first!", EncConverters.cstrCaption);
                    return false;
                }
                else if (!File.Exists(scriptPath))
                {
                    MessageBox.Show(this, "File doesn't exist!", EncConverters.cstrCaption);
                    return false;
                }
                else if (String.IsNullOrEmpty(pythonDistroPath))
                {
                    MessageBox.Show(this, "You have to Browse to the path for the Python 3.* DLL file (e.g. Python312.dll)!", EncConverters.cstrCaption);
                    return false;
                }
            }

            ConverterIdentifier = $"{scriptPath};{pythonDistroPath}";

            Util.DebugWriteLine(this, "END");
            return base.OnApply();
        }

        protected override string ProgID
        {
            get { return typeof(Py3ScriptEncConverter).FullName; }
        }

        protected override string ImplType
        {
            get { return EncConverters.strTypeSILPy3Script; }
        }

        protected override string DefaultFriendlyName
        {
            // as the default, make it the same as the table name (w/o extension)
            get { return Path.GetFileNameWithoutExtension(Py3ScriptEncConverter.ScriptPath(ConverterIdentifier)); }
        }

        private void buttonBrowse_Click(object sender, EventArgs e)
        {
			// since we use the same dialog for the python script and the python dll, don't keep the 'FileName' around,
			//	when searching for the latter
			InitializeOpenFileDialog(openFileDialogBrowse.FileName, OpenFileDialogDefaultExtBrowseForScript, OpenFileDialogFilterBrowseForScript, OpenFileDialogTitleBrowseForScript);
            var scriptPath = Py3ScriptEncConverter.ScriptPath(ConverterIdentifier);
            if (!String.IsNullOrEmpty(scriptPath))
                openFileDialogBrowse.InitialDirectory = Path.GetDirectoryName(scriptPath);
            else
                openFileDialogBrowse.InitialDirectory = Util.CommonAppDataPath() + EncConverters.strDefMapsTablesPath;

            if (openFileDialogBrowse.ShowDialog() == DialogResult.OK)
            {
                ResetFields();
                textBoxFileSpec.Text = openFileDialogBrowse.FileName;
            }
        }

        private void buttonBrowseForPythonDll_Click(object sender, EventArgs e)
		{
			// since we use the same dialog for the python script and the python dll, don't keep the 'FileName' around,
			//	when searching for the latter
			InitializeOpenFileDialog(openFileDialogBrowse.FileName, OpenFileDialogDefaultExtBrowseForDll, OpenFileDialogFilterBrowseForDll, OpenFileDialogTitleBrowseForDll);

			var distroPath = Py3ScriptEncConverter.DistroPath(ConverterIdentifier);
			if (!String.IsNullOrEmpty(distroPath))
				openFileDialogBrowse.InitialDirectory = Path.GetDirectoryName(distroPath);
			else
				openFileDialogBrowse.InitialDirectory = @"C:\";

			if (openFileDialogBrowse.ShowDialog() == DialogResult.OK)
			{
				ResetFields();
				textBoxPython3Path.Text = openFileDialogBrowse.FileName;
			}
		}

		private void InitializeOpenFileDialog(string fileName, string defaultExt, string filter, string title)
		{
			if (!string.IsNullOrEmpty(fileName) && fileName.Substring(fileName.Length - defaultExt.Length + 1).ToLower() != $".{defaultExt}")
				fileName = "";
			openFileDialogBrowse.Title = title;
			openFileDialogBrowse.DefaultExt = defaultExt;
			openFileDialogBrowse.Filter = filter;
			openFileDialogBrowse.FileName = fileName;
		}

		private void textBox_TextChanged(object sender, EventArgs e)
        {
            if (m_bInitialized) // but only do this after we're already initialized
            {
                IsModified = (((TextBox)sender).Text.Length > 0);
            }
        }

        private void radioButton_CheckedChanged(object sender, EventArgs e)
        {
            IsModified = true;
        }
    }
}

