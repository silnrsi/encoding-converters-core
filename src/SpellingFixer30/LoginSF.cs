using System;
using System.Windows.Forms;
using System.Drawing;                   // for Font
using Microsoft.Win32;                  // for RegistryKey
using System.IO;                        // for File
using System.Runtime.InteropServices;   // for ComVisible
using System.Reflection;                // for Assembly
using System.Text;                      // for Encoding
using System.Data;                      // for DataTable
using ECInterfaces;
using SilEncConverters40;
using System.Diagnostics;

namespace SpellingFixer30
{
	/// <summary>
	/// Summary description for LoginSF.
	/// </summary>
    [ComVisible(false)]
    internal partial class LoginSF : Form
	{
        private bool    m_isRightToleft;
        private Font    m_font;
        private string  m_strConverterSpec;
        private string  m_strEncConverterName;
        private string  m_strWordBoundaryDelimiter;
        public  const string    cstrAddNewProjectButtonText = "&Add New Project";
        public  const string    cstrAddNewProjectButtonToolTipText = "Click to add new project";
        public  const string    cstrNewProjectGroupText = "New Project";
        private const string    cstrProjectMemoryKey = @"SOFTWARE\SIL\SilEncConverters40\SpellingFixerEC";
        private string  m_strNonWordCharacters;
        private const string  cstrProjectMostRecentProject = "MostRecentProject";


        public LoginSF()
		{
			//
			// Required for Windows Form Designer support
			//
			InitializeComponent();

            this.checkedListBoxProjects.ContextMenu = this.contextMenu;

            var aECs = new EncConverters();
            var myECs = aECs.FilterByProcessType(SpellingFixer.SFProcessType);
            
            string strPartialName;
            foreach(IEncConverter aEC in myECs.Values)
                if( (strPartialName = PartialName(aEC.Name)) != null )
                    checkedListBoxProjects.Items.Add(strPartialName);
                else
                    checkedListBoxProjects.Items.Add(aEC.Name);

            // decide which one to select
            int nIndex = checkedListBoxProjects.Items.Count - 1;
            try
            {
                RegistryKey keyLastSFProject = Registry.CurrentUser.OpenSubKey(cstrProjectMemoryKey);
                nIndex = checkedListBoxProjects.FindString((string)keyLastSFProject.GetValue(cstrProjectMostRecentProject));
            }
            catch {}

            if (nIndex != -1)
            {
                checkedListBoxProjects.SetItemChecked(nIndex, true);
            }
        }

        public bool ProjectsExist
        {
            get { return checkedListBoxProjects.Items.Count > 0; }
        }

        public Font FontToUse
        {
            get { return m_font; }
        }

        public string ConverterSpec
        {
            get { return m_strConverterSpec; }
        }

        public string EncConverterName
        {
            get { return m_strEncConverterName; }
        }

        public string WordBoundaryDelimiter
        {
            get { return m_strWordBoundaryDelimiter; }
        }

        public bool IsRightToLeft
        {
            get { return m_isRightToleft; }
        }

        public string Punctuation
        {
            get { return m_strNonWordCharacters; }
        }

        internal string FullName(string strProjName)
        {
            return SpellingFixer.SFConverterPrefix + strProjName;
        }

		internal string FullNameCsc(string strProjName)
		{
			return SpellingFixer.SFConverterPrefixCsc + strProjName;
		}

		internal string PartialName(string strFullName)
        {
            int nPrefixLen = SpellingFixer.SFConverterPrefix.Length;
            if(     (strFullName.Length > nPrefixLen)
                &&  (strFullName.Substring(0, nPrefixLen) == SpellingFixer.SFConverterPrefix) )
                return strFullName.Substring(nPrefixLen,strFullName.Length - nPrefixLen);
            return null;
        }

        internal static void CreateCCTable(string strCCTableSpec, string strEncConverterName, string strPunctuation)
        {
			if (!Directory.Exists(Path.GetDirectoryName(strCCTableSpec)))
			{
				Directory.CreateDirectory(Path.GetDirectoryName(strCCTableSpec));
			}
			CreateCCTable(new FileStream(strCCTableSpec, FileMode.Create), strEncConverterName, strPunctuation);
        }

        internal static void CreateCCTable(FileStream fs, string strEncConverterName, string strPunctuation)
        {
            // write out the header lines.
            var sw = new StreamWriter(fs);
            CreateCCTable(sw, strEncConverterName, strPunctuation);
            sw.Flush();
            sw.Close();
        }

        internal static string CctableLastHeaderLine
        {
            get { return "c Last Header Line: DON'T modify the table beyond this point (or your changes may be overwritten)"; }
        }

        internal const string DummyRule = "    d31 > d31 c dummy rule so that the table isn't empty (can be removed if you have rules below the Last Header Line below)";
        internal const string CustomRuleEndComment = "c +----------end custom changes----------+";

        internal static void CreateCCTable(StreamWriter sw, string strEncConverterName, string strPunctuation, string strCustomCode = DummyRule, bool bUnicode = true)
        {
            // write out the header lines.
            string strVersion = Assembly.GetExecutingAssembly().GetName().Version.ToString();
            sw.WriteLine(String.Format("c This cc table was created by SpellingFixerEC.dll v{0} on {1}.",strVersion, DateTime.Now.ToShortDateString()));
            sw.WriteLine(String.Format("c It can be accessed as the '{0}' EncConverter", strEncConverterName));
            sw.WriteLine("c If you know how to program CC, you can add special processing between the");
            sw.WriteLine("c 'start custom changes' and 'end custom changes' comments below.");
            string strBeginStmt = "begin >";
            if( bUnicode )
                strBeginStmt += " utf8";    // this is needed to interpret multi-byte UTF8 strings as a single character (e.g. in the 'ws' store)
            sw.WriteLine(strBeginStmt);
            sw.WriteLine(String.Format("    store(ws) {0} endstore", strPunctuation));
            sw.WriteLine("");
            sw.WriteLine("c +----------start custom changes----------+");
            if (!String.IsNullOrEmpty(strCustomCode))
            {
                sw.WriteLine(strCustomCode);
            }
            sw.WriteLine(CustomRuleEndComment);
            sw.WriteLine("");
            sw.WriteLine(CctableLastHeaderLine);
        }

        internal static void ReWriteCCTableHeader(string strCCTableSpec, string strPunctuation, Encoding enc, bool removeDummyRule)
        {
            // Open the CC table that has the mappings put them in a new file
            //  while re-writing the first part of the header
            if( (strCCTableSpec != null) && File.Exists(strCCTableSpec) )
            {
                const string strTempExt = ".new";
                // get a stream writer for these encoding and append
                var sr = new StreamReader(strCCTableSpec,enc);
                var sw = new StreamWriter(strCCTableSpec + strTempExt, false, enc);

                // this is for version 1.2.0.0
                string strVersion = Assembly.GetExecutingAssembly().GetName().Version.ToString();
                var copiedDummyRule = false;

                // copy the read stuff to the output and update the 'ws' store line
                for(string line = sr.ReadLine(); line != null; line = sr.ReadLine())
                {
                    if (line.Contains("c This cc table was created by SpellingFixerEC.dll v"))
                        line = String.Format("c This cc table was created by SpellingFixerEC.dll v{0} on {1}.",strVersion, DateTime.Now.ToShortDateString());
                    else if (line.Contains("store(ws)"))
                        line = String.Format("    store(ws) {0} endstore", strPunctuation);

                    // if we're re-writing the cc table with this method, then if we're going to be adding 
                    //  actual rules (i.e. removeDummyRule is true), then we can get rid of the dummy rule.
                    if (line.Contains(DummyRule))
                    {
                        if (removeDummyRule)
                            continue;
                        copiedDummyRule = true;
                    }
                    // otherwise, we need to put it back in
                    else if (line.Contains(CustomRuleEndComment) && !removeDummyRule && !copiedDummyRule)
                    {
                        sw.WriteLine(DummyRule);
                    }

                    sw.WriteLine(line);
                    
                    // stop when we get past the end of the header
                    if( line == CctableLastHeaderLine )
                        break;
                }

                sw.Flush();
                sw.Close();
                sr.Close();

                string strBackupFilename = strCCTableSpec + ".bak";
                if( File.Exists(strBackupFilename) )
                    File.Delete(strBackupFilename);
                File.Move(strCCTableSpec,strBackupFilename);
                File.Move(strCCTableSpec + strTempExt, strCCTableSpec);
            }
        }

        internal static string GetMapTableFolderPath
        {
            get
            {
                string strMapsTableDir = Util.CommonAppDataPath();
                strMapsTableDir += EncConverters.strDefMapsTablesPath;

                if (!Directory.Exists(strMapsTableDir))
                    Directory.CreateDirectory(strMapsTableDir);

                return strMapsTableDir;
            }
        }

        private void MenuItemAddNewProject_Click(object sender, EventArgs e)
        {
            AddNewProject();
        }

        private void AddNewProject()
        {
            using var dlg = new AddNewProjectForm(SpellingFixer.GetDefaultPunctuation)
            {
                WordBoundaryDelimiter = SpellingFixer.cstrDefaultWordBoundaryDelimiter
            };

            var res = dlg.ShowDialog();
            if (res == DialogResult.Cancel)
                return;

            UpdateRepo(false, dlg);
        }

        private void UpdateRepo(bool isEditing, AddNewProjectForm dlg)
        {
            // get the delimiter for word boundaries and disallow the /"/ character
            m_strWordBoundaryDelimiter = dlg.WordBoundaryDelimiter;
            m_isRightToleft = dlg.IsRightToLeft;

            bool bRewriteCCTable = false;

            string strPunctuation = dlg.GetAddlPunctuation(true);
            if (strPunctuation == null)
            {
                return; // it had a bad character
            }
            else if (strPunctuation != m_strNonWordCharacters)
            {
                // this means the file must be re-written
                bRewriteCCTable = true;
                m_strNonWordCharacters = strPunctuation;
            }

            // check for existing EncConverter with this same project information
            string strCCTableSpec = null;
            var strPartialName = dlg.NewProjectName;
            var strEncConverterName = FullName(strPartialName);
            var aECs = new EncConverters();
            IEncConverter aEC = aECs[strEncConverterName];
            if (aEC != null)
            {
                // if we're *not* in edit mode
                if (!isEditing)
                {
                    if (MessageBox.Show($"A project already exists by the name {strPartialName}. Click 'Yes' to overwrite",
										SpellingFixer.cstrCaption, MessageBoxButtons.YesNo)
                            == DialogResult.Yes)
                    {
                        // take it out of the check box list
                        checkedListBoxProjects.Items.Remove(strPartialName);
                        strCCTableSpec = aEC.ConverterIdentifier;
                        if (File.Exists(strCCTableSpec))
                        {
                            File.Delete(strCCTableSpec);
                            strCCTableSpec = null;
                        }

                        // remove the existing one and we'll add a new one next
                        aECs.Remove(aEC.Name);
                    }
                    else
                        return;
                }
                else    // edit mode
                {
                    if (MessageBox.Show($"Do you want to update the '{strPartialName}' project?",
										SpellingFixer.cstrCaption, MessageBoxButtons.YesNo)
                            == DialogResult.Yes)
                    {
                        // take it out of the check box list
                        checkedListBoxProjects.Items.Remove(strPartialName);

                        // save the spec so that we don't make one below
                        strCCTableSpec = aEC.ConverterIdentifier;

                        // the remove the converter since we'll add it back again next
                        aECs.Remove(aEC.Name);
                    }
                    else
                    {
                        return;
                    }
                }
            }

            // if we're aren't using the old cc table, then...
            if (String.IsNullOrEmpty(strCCTableSpec))
            {
                // now add it (put it in the normal 'MapsTables' folder in \pf\cf\sil\...)
                string strMapsTableDir = GetMapTableFolderPath;
                strCCTableSpec = strMapsTableDir + @"\" + strEncConverterName + ".cct";
                if (File.Exists(strCCTableSpec))
                {
                    // the converter doesn't exist, but a file with the name we would have 
                    //  given it does... ask the user if they want to overwrite it.
                    // TODO: this doesn't allow for complete flexibility. It might be nicer to
                    //  allow for any arbitrary name, but not if noone complains.
                    if (MessageBox.Show($"A file exists by the name {strCCTableSpec}. Click 'Yes' to overwrite",
										SpellingFixer.cstrCaption, MessageBoxButtons.YesNo)
                            == DialogResult.Yes)
                    {
                        File.Delete(strCCTableSpec);
                    }
                }
                bRewriteCCTable = false;
            }

            // now add the EncConverter
            // TODO: EncConverters needs a new interface to get the defining encodingID from 
            //  a FontName (so we can use it in this call) just like we can 'try' to get the
            //  code page given a font name (see 'CodePage' below)
            var eConvType = ConvType.Unicode_to_Unicode;

            aECs.Add(strEncConverterName, strCCTableSpec, eConvType, null, null, SpellingFixer.SFProcessType);

            Font font = dlg.SelectedFont;

            // add this 'displaying font' information to the converter as properties/attributes
            ECAttributes aECAttrs = aECs.Attributes(strEncConverterName, AttributeType.Converter);
            aECAttrs.Add(SpellingFixer.cstrAttributeFontToUse, font.Name);
            aECAttrs.Add(SpellingFixer.cstrAttributeFontRightToLeft, m_isRightToleft);
            aECAttrs.Add(SpellingFixer.cstrAttributeFontSizeToUse, font.Size);
            aECAttrs.Add(SpellingFixer.cstrAttributeWordBoundaryDelimiter, m_strWordBoundaryDelimiter);
            aECAttrs.Add(SpellingFixer.cstrAttributeNonWordChars, m_strNonWordCharacters);

            if (bRewriteCCTable)    // we are going to continue using the old file... so we must re-write it.
            {
                // if it was legacy encoded, then we need to convert the data to narrow using
                //  the code page the user specified (or we got out of the repository)
                var enc = new UTF8Encoding();

                if (SpellingFixer.InitializeDataTableFromCCTable(strCCTableSpec, enc, m_strWordBoundaryDelimiter, out DataTable myTable))
                {
					SpellingFixer.AppendCCTableFromDataTable(strCCTableSpec, enc, m_strWordBoundaryDelimiter, m_strNonWordCharacters, myTable);
                }
            }

            // finally, add the new project to the now-visible checkbox list
            ClearClickedItems();
            checkedListBoxProjects.Items.Add(strPartialName, CheckState.Checked);
        }

        private void ClearClickedItems()
        {
            foreach(int indexChecked in checkedListBoxProjects.CheckedIndices)
                checkedListBoxProjects.SetItemCheckState(indexChecked,CheckState.Unchecked);
        }

        private void ButtonOK_Click(object sender, System.EventArgs e)
        {
            // When the OK button is clicked, it means the user is choosing the project in 
            //  the project checkbox list. So use *that* information only to fill in the 
            //  member variables of what the caller needs (i.e. beware that it isn't 
            //  necessarily true that we added a project during this instantiation, so don't
            //  depend on the internal variables (e.g. m_font, etc.) having something 
            //  meaningful)
            var aCheckedItems = checkedListBoxProjects.CheckedItems;

            // should only be once checked item
            if( aCheckedItems.Count != 1 )
                return;
            string strEncConverterName = aCheckedItems[0].ToString();

            if (LoadProject(strEncConverterName))
            {
                DialogResult = DialogResult.OK;
                this.Close();
            }
        }

        internal bool LoadProject(string strProjectName)
        {
            if (String.IsNullOrEmpty(strProjectName))
                return false;

            // get the EncConverter that should have been added above by 'AddNewProject' button
            var aECs = new EncConverters();
            var possibleConverterName = strProjectName;
			if (!aECs.ContainsKey(possibleConverterName))
			{
				possibleConverterName = FullName(strProjectName);
			}
			else if (!aECs.ContainsKey(possibleConverterName))
			{
				Debug.Assert(false, "this shouldn't happen (it should have used CscProject.LoadProject)");
				possibleConverterName = FullNameCsc(strProjectName);
			}
			else if (!aECs.ContainsKey(possibleConverterName))
			{
				possibleConverterName = strProjectName;
			}

            IEncConverter aEC = aECs[possibleConverterName];
            if (aEC != null)
            {
                m_strEncConverterName = aEC.Name;
                m_strConverterSpec = aEC.ConverterIdentifier;
                ECAttributes aECAttrs = aECs.Attributes(aEC.Name, AttributeType.Converter);
                string strFontName = aECAttrs[SpellingFixer.cstrAttributeFontToUse];
                string sFontSize = aECAttrs[SpellingFixer.cstrAttributeFontSizeToUse];
                m_isRightToleft = aECAttrs.ContainsKey(SpellingFixer.cstrAttributeFontRightToLeft) && (aECAttrs[SpellingFixer.cstrAttributeFontRightToLeft].ToLower() == "true");
                m_strWordBoundaryDelimiter = aECAttrs[SpellingFixer.cstrAttributeWordBoundaryDelimiter];
                m_strNonWordCharacters = aECAttrs[SpellingFixer.cstrAttributeNonWordChars];

                // new in 1.2 (so it might not exist)
                m_strNonWordCharacters ??= SpellingFixer.GetDefaultPunctuation;

                // if this was added (without having been made on this computer), then
                //  these properties don't get added automatically. Must go to edit mode!
                if (String.IsNullOrEmpty(strFontName)
                    || String.IsNullOrEmpty(sFontSize)
                    || String.IsNullOrEmpty(m_strWordBoundaryDelimiter))
                {
                    MessageBox.Show("This converter is missing some important properties. You'll need to edit it again to set the font, and other values.", SpellingFixer.cstrCaption);
                    DoEdit(strProjectName, strFontName, sFontSize);
                    return false;
                }

                var fFontSize = GetFloatFontSize(sFontSize);

                if (!String.IsNullOrEmpty(strFontName) && (fFontSize != 0.0))
                    m_font = new Font(strFontName, fFontSize);

                RegistryKey keyLastSFProject = Registry.CurrentUser.CreateSubKey(cstrProjectMemoryKey);
                keyLastSFProject.SetValue(cstrProjectMostRecentProject, strProjectName);
                return true;
            }

            return false;
        }

        private static float GetFloatFontSize(string sFontSize)
        {
            var fFontSize = (float)0.0;
            try
            {
                fFontSize = (float)Convert.ToSingle(sFontSize);
            }
            catch { }

            return fFontSize;
        }

        private void ButtonCancel_Click(object sender, System.EventArgs e)
        {
            DialogResult = DialogResult.Cancel;
            this.Close();
        }

        private void CheckedListBoxProjects_SelectedIndexChanged(object sender, System.EventArgs e)
        {
            int nIndex = checkedListBoxProjects.SelectedIndex;
            ClearClickedItems();
            if( nIndex != -1 )
                checkedListBoxProjects.SetItemCheckState(nIndex,CheckState.Checked);
        }

        // get the point at which the right mouse button was clicked (for subsequent pop-up
        //  menu processing)
        private Point m_ptRightClicked;
        private void CheckedListBoxProjects_MouseUp(object sender, MouseEventArgs e)
        {
            // don't want now that I support double-click if( e.Button == MouseButtons.Right )
            m_ptRightClicked = new Point(e.X,e.Y);
        }
        
        private void MenuItemClick_Click(object sender, System.EventArgs e)
        {
            int nIndex = checkedListBoxProjects.IndexFromPoint(m_ptRightClicked);
            if( nIndex >= 0 )
            {
                ClearClickedItems();
                checkedListBoxProjects.SetItemChecked(nIndex,true);
            }
        }

        private void MenuItemDelete_Click(object sender, System.EventArgs e)
        {
            int nIndex = checkedListBoxProjects.IndexFromPoint(m_ptRightClicked);
            if( nIndex >= 0 )
            {
                string strProjectName = checkedListBoxProjects.Items[nIndex].ToString();
                if( MessageBox.Show(String.Format("Are you sure you want to delete the '{0}' project?",strProjectName), SpellingFixer.cstrCaption, MessageBoxButtons.YesNoCancel) == DialogResult.Yes)
                {
                    var aECs = new EncConverters();
                    IEncConverter aEC = aECs[FullName(strProjectName)];
                    if( aEC != null )
                    {
                        DialogResult res = MessageBox.Show(String.Format("Do you also want to delete the associated CC table '{0}'?", aEC.ConverterIdentifier), SpellingFixer.cstrCaption, MessageBoxButtons.YesNoCancel);
                        if( res == DialogResult.Yes)
                        {
                            if( File.Exists(aEC.ConverterIdentifier) )
                                File.Delete(aEC.ConverterIdentifier);
                        }
                        else if( res == DialogResult.Cancel )
                            return;

                        // remove it from the repository as well.
                        aECs.Remove(aEC.Name);
                    }

                    checkedListBoxProjects.Items.Remove(strProjectName);
                }
            }
        }

        private void MenuItemEdit_Click(object sender, System.EventArgs e)
        {
            // Provide a way to edit the info (in case the user wants to change the 
            //  font, size, or delimter. 
            int nIndex = checkedListBoxProjects.IndexFromPoint(m_ptRightClicked);
            if( nIndex >= 0 )
            {
                string strProjectName = checkedListBoxProjects.Items[nIndex].ToString();
                var aECs = new EncConverters();
                IEncConverter aEC = aECs[FullName(strProjectName)];
                if( aEC != null )
                {
                    m_strEncConverterName = aEC.Name;
                    m_strConverterSpec = aEC.ConverterIdentifier;
                    ECAttributes aECAttrs = aECs.Attributes(aEC.Name,AttributeType.Converter);

                    string strFontName = aECAttrs[SpellingFixer.cstrAttributeFontToUse];
                    string sFontSize = aECAttrs[SpellingFixer.cstrAttributeFontSizeToUse];
                    m_isRightToleft = aECAttrs.ContainsKey(SpellingFixer.cstrAttributeFontRightToLeft) && (aECAttrs[SpellingFixer.cstrAttributeFontRightToLeft] == "true");
                    m_strWordBoundaryDelimiter = aECAttrs[SpellingFixer.cstrAttributeWordBoundaryDelimiter];
                    m_strNonWordCharacters = aECAttrs[SpellingFixer.cstrAttributeNonWordChars];

                    DoEdit(strProjectName, strFontName, sFontSize);
                }
            }
        }

        private void DoEdit(string strProjectName, string strFontName, string sFontSize)
        {
            m_font = new Font(strFontName, GetFloatFontSize(sFontSize ?? "12.0"));

            using var dlg = new AddNewProjectForm(m_strNonWordCharacters)
            {
                NewProjectName = strProjectName,
                WordBoundaryDelimiter = m_strWordBoundaryDelimiter,
                SelectedFont = m_font,
                IsRightToLeft = m_isRightToleft,
            };

            var res = dlg.ShowDialog();
            if (res == DialogResult.Cancel)
                return;

            UpdateRepo(true, dlg);
        }

        private void MenuItemDeleteAll_Click(object sender, System.EventArgs e)
        {
            // verify first!
            if( MessageBox.Show("Are you sure you want to delete all the existing projects?", SpellingFixer.cstrCaption, MessageBoxButtons.YesNoCancel) == DialogResult.Yes)
            {
                var aECs = new EncConverters();
                foreach(string strProjectName in checkedListBoxProjects.Items)
                {
                    IEncConverter aEC = aECs[FullName(strProjectName)];
                    if (aEC != null)
                    {
                        // if they do this, then don't bother querying about saving the files.
                        if( File.Exists(aEC.ConverterIdentifier) )
                            File.Delete(aEC.ConverterIdentifier);

                        // remove it from the repository as well.
                        aECs.Remove(aEC.Name);
                    }
                }

                // now remove them from the checkbox list
                while( checkedListBoxProjects.Items.Count > 0 )
                    checkedListBoxProjects.Items.RemoveAt(0);
            }
        }

        // double click means edit
        private void CheckedListBoxProjects_DoubleClick(object sender, EventArgs e)
        {
            this.MenuItemEdit_Click(sender,e);
        }

        private void LoginSF_Shown(object sender, EventArgs e)
        {
            if (!ProjectsExist)
            {
                AddNewProject();
            }
        }
    }
}