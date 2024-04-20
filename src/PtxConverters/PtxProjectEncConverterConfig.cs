using System;
using System.Collections.Generic;
using System.Text;
using System.Runtime.InteropServices;   // for the class attributes
using ECInterfaces;                     // for IEncConverter
using System.IO;
using System.Windows.Forms;
using System.Linq;
using static System.Resources.ResXFileRef;

namespace SilEncConverters40.PtxConverters
{
    public class PtxProjectEncConverterConfig : EncConverterConfig
    {
        public const string DefaultPathToMyParatextProjects = @"C:\My Paratext 9 Projects";
		public const string ConverterFriendlyNameFormat = "Paratext Project {0}";

		public PtxProjectEncConverterConfig()
            : base
            (
            typeof(PtxProjectEncConverter).FullName,
            PtxProjectEncConverter.DisplayName,
            null,
            ProcessTypeFlags.DontKnow
            )
            {
            }

        public override bool Configure
        (
        IEncConverters aECs,
        string strFriendlyName,
        ConvType eConversionType,
        string strLhsEncodingID,
        string strRhsEncodingID
        )
        {
            var pathToMyParatextProjects = DefaultPathToMyParatextProjects;
            if (!Directory.Exists(pathToMyParatextProjects))
            {
                var folderBrowseDialog = new FolderBrowserDialog { Description = $"Browse for your 'My Paratext 9 Projects' folder (default: '{DefaultPathToMyParatextProjects}')" };
                if (folderBrowseDialog.ShowDialog() == DialogResult.OK)
                {
                    pathToMyParatextProjects = folderBrowseDialog.SelectedPath;
                }
                else
                {
                    return false;
                }
            }

            var paratextProjectNames = Directory.GetDirectories(pathToMyParatextProjects)
                                                .Where(p => Directory.GetFiles(p, "*.sfm").Any())	// only include sub-folders that have SFM documents
                                                .Select(p => Path.GetFileName(p))
                                                .ToList();

            var projectPickerDialog = new ProjectListForm(paratextProjectNames);
            if (projectPickerDialog.ShowDialog() == DialogResult.OK)
            {
				// just in case this is an edit situation, remove the original definition
				if (!String.IsNullOrEmpty(strFriendlyName) && aECs[strFriendlyName] != null)
					aECs.Remove(strFriendlyName);

				// add the new one
				var converterId = projectPickerDialog.SelectedDisplayName;
				ConverterFriendlyName = String.Format(ConverterFriendlyNameFormat, converterId);
                aECs.AddConversionMap(ConverterFriendlyName, converterId, ConvType.Unicode_to_Unicode,
                                      PtxProjectEncConverter.ImplementTypePtxProjectData, "Unicode", "Unicode",
                                      ProcessTypeFlags.Translation);

				IsInRepository = true;
                return true;
            }

            return false;
        }

        public override void DisplayTestPage
            (
            IEncConverters aECs,
            string strFriendlyName,
            string strConverterIdentifier,
            ConvType eConversionType,
            string strTestData
            )
        {
            InitializeFromThis(ref strFriendlyName, ref strConverterIdentifier,
                ref eConversionType, ref strTestData);

            var form = new AutoConfigDialog();
            form.Initialize
            (
                aECs,
                strFriendlyName,
                strConverterIdentifier,
                eConversionType,
                strTestData
            );

            base.DisplayTestPage(form);
        }
    }
}
