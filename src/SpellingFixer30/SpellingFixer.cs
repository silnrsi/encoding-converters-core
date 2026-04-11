using System;
using System.Windows.Forms;
using System.Runtime.InteropServices;
using System.ComponentModel;            // for Description
using ECInterfaces;
using SilEncConverters40;
using System.Drawing;                   // for Font
using System.IO;                        // for File
using System.Reflection;                // for DefaultMemberAttribute
using System.Text;                      // for Encoding
using System.Data;                      // for DataTable
using System.Diagnostics;               // for Debug

namespace SpellingFixer30
{
    public enum SpellFixerMode
    {
        eUserSelect,
        eConsistentSpellingChecker,
        eSpellFixerLegacy
    }

	/// <summary>
	/// An EncConverter plug-in to help with a Spelling fixer helper
	/// </summary>
    [DefaultMemberAttribute("SpellFixerEncConverterName")]
    [Description("When instantiated, this object will query the user for the project to use (e.g. Hindi) for subsequent 'AssignCorrectSpelling' calls. Make this object global scope to keep from having to select the project with each usage."),Category("Data")] 
    [ComVisible(true)]
    [ClassInterface(ClassInterfaceType.AutoDual)]
    [Guid("31D2D085-6917-4163-BD02-F732C7039A59")]
    public class SpellingFixer
	{
        public const string cstrAttributeFontToUse = "SpellingFixer Display Font";
        public const string cstrAttributeFontSizeToUse = "SpellingFixer Display Font Size";
        public const string cstrAttributeFontRightToLeft = "SpellingFixer Display Right to Left";
        public const string cstrAttributeWordBoundaryDelimiter = "SpellingFixer Word Boundary Delimiter";
        public const string cstrAttributeNonWordChars = "SpellingFixer punctuation and whitespace characters";
		public const string cstrDefaultPunctuationAndWhitespace = "' ' tab nl '.' ',' '!' ':' ';' '-' \"'\" '\"' '‘' '’' '“' '”' '(' ')' '[' ']' '{' '}' '?' '|'";
        public const string cstrV3DefaultPunctuationAndWhitespaceAdds = " '?'";
        public const string cstrDefaultWordBoundaryDelimiter = "#";

        public const string SFConverterPrefixCsc = "Consistent Spelling for ";
		public const string SFConverterPrefix = "Clean Spelling for ";
		internal const string   cstrCaption = "Spelling Fixer";
        internal const string   strQuotedFormat = "\"{0}\"";
        private const string    cstrPrecWhiteSpace = "prec(ws)";
        private const string    cstrFollWhiteSpace = "fol(ws)";
        private const string    cstrIndentation = "    ";
        private const string    cstrCommentFormat = " c rule added while fixing: '{0}'";
        private const char      chSpace = ' ';

        private System.Drawing.Font m_font;
        private string              m_strWordBoundaryDelimiter;
        private string              m_strNonWordChars;
        private string              m_strConverterSpec;
        private string              m_strEncConverterName;
		private bool				m_isRightToLeft;

		// leave a default constructor which *doesn't* automatically log-in to a project for 
		//  COM clients that want to use CscProject via SelectProject below.
		public SpellingFixer()
		{
        }

        /// <summary>
        /// If you use the default ctor, and want to log into a SpellFixer project (as opposed to CscProject)
        /// then use this method.
        /// </summary>
        public void LoginProject()
        {
            LoginSF login = new LoginSF();
            if( login.ShowDialog() == DialogResult.OK )
            {
                m_font = login.FontToUse;
				m_isRightToLeft = login.IsRightToLeft;
                m_strConverterSpec = login.ConverterSpec;
                SpellFixerEncConverterName = login.EncConverterName;
                WordBoundaryDelimiter = login.WordBoundaryDelimiter;
                PunctuationAndWhiteSpace = login.Punctuation;
            }
            else
                throw new ExternalException("No project selected");
        }

        public SpellingFixer(string strProjectName)
        {
            LoginSF login = new LoginSF();
            if( login.LoadProject(strProjectName) )
            {
                m_font = login.FontToUse;
				m_isRightToLeft = login.IsRightToLeft;
				m_strConverterSpec = login.ConverterSpec;
                SpellFixerEncConverterName = login.EncConverterName;
                WordBoundaryDelimiter = login.WordBoundaryDelimiter;
                PunctuationAndWhiteSpace = login.Punctuation;
            }
            else
                throw new ExternalException("No project selected");
        }

        public SpellingFixer(string strProjectName, Font font, string strConverterSpec, string strEncConverterName,
            [Optional, DefaultParameterValue(SpellingFixer.cstrDefaultWordBoundaryDelimiter)] string strWordBoundaryDelimiter, 
            [Optional, DefaultParameterValue(cstrDefaultPunctuationAndWhitespace)] string strPunctuationAndWhiteSpace,
            [Optional, DefaultParameterValue(false)] bool bLegacy, [Optional, DefaultParameterValue(1252)] int cp,
			[Optional, DefaultParameterValue(false)] bool bRightToLeft)
        {
            m_font = font;
			m_isRightToLeft = bRightToLeft;
			m_strConverterSpec = strConverterSpec;
            SpellFixerEncConverterName = strEncConverterName;
            WordBoundaryDelimiter = strWordBoundaryDelimiter;
            PunctuationAndWhiteSpace = strPunctuationAndWhiteSpace;
        }

        public static string GetDefaultPunctuation
        {
            get { return cstrDefaultPunctuationAndWhitespace; }
        }

        [Description("The EncConverters process type flag for the SpellingFixer30 converters."), Category("Data")] 
        static public ProcessTypeFlags  SFProcessType
        {
            get { return ProcessTypeFlags.SpellingFixerProject; }
        }

        [Description("Returns the name of the EncConverter to use to correct the spelling for the selected project."),Category("Data")]
        public string SpellFixerEncConverterName
        {
            get { return m_strEncConverterName; }
            set { m_strEncConverterName = value; }
        }

        [Description("Returns the font associated with the selected project."), Category("Data")]
        public Font ProjectFont
        {
            get { return m_font; }
        }

        [Description("Returns the instance of the IEncConverter interface to use to correct the spelling for the selected project."),Category("Data")]
        public IEncConverter SpellFixerEncConverter
        {
            get 
            { 
                if( m_strEncConverterName == null )
                    return null;

				var aECs = new EncConverters();
				IEncConverter aEC;
				if (aECs.ContainsKey(m_strEncConverterName))
                    aEC = aECs[m_strEncConverterName];
                else
                {
                    aEC = new CcEncConverter();
                    string strDummy = null;
                    int nProcType = 0;
					var eConvType = ConvType.Unicode_to_Unicode;
					aEC.Initialize(m_strEncConverterName, m_strConverterSpec, ref strDummy, ref strDummy,
						ref eConvType, ref nProcType, 0, 0, true);
				}

				return aEC;
            }
        }

        /// <summary>
        /// Use this from COM/VBA to cause us to query the user for the project they want to use and
        /// we'll return it.
        /// From .Net, you should use the static CscProject.LoadProject directly.
        /// </summary>
        /// <returns>CscProject -- the initialized project object selected by the user</returns>
        public CscProject SelectProject()
        {
            return CscProject.SelectProject();
        }

        /// <summary>
        /// Use this method to query the user for which kind of SpellFixer project they want to create:
        ///     eConsistentSpellingChecker: whole-word-only spell fixing and consistency checking
        ///     eSpellFixerLegacy: partial word spell fixing and consistent changes
        /// After calling this method (if it doesn't throw an exception because the user cancelled), then
        /// call SelectProject() or LoginProject() respectively to get the proper type of project.
        /// </summary>
        /// <returns>SpellFixerMode</returns>
        public SpellFixerMode QuerySpellFixProject()
        {
            SpellFixerModeSelect dlg = new SpellFixerModeSelect();
            if (dlg.ShowDialog() == DialogResult.OK)
            {
                return dlg.Mode;
            }

            throw new ExternalException("No project selected!");
        }

        /// <summary>
        /// Use this from COM/VBA to cause us to return a named project (but you must check for null in
        /// case it doesn't exist).
        /// From .Net, you should use the static CscProject.LoadProject directly.
        /// </summary>
        /// <param name="strProjectName"></param>
        /// <returns>CscProject -- the initialized project object assocated with the give name</returns>
        public CscProject LoadProject(string strProjectName)
        {
            return CscProject.LoadProject(strProjectName);
        }

        private string WordBoundaryDelimiter
        {
            get { return m_strWordBoundaryDelimiter; }
            set { m_strWordBoundaryDelimiter = value; }
        }

		private bool IsRightToLeft
		{
			get { return m_isRightToLeft; }
			set { m_isRightToLeft = value; }
		}

		private string PunctuationAndWhiteSpace
        {
            get { return m_strNonWordChars; }
            set { m_strNonWordChars = value; }
        }

		/// <summary>
		/// these are the legacy (pre-ConsistentSpellingFixer) type methods. For the Csc-based methods, see CscProject.cs
		/// </summary>
		/// <param name="strBadWord"></param>
		/// <exception cref="ExternalException"></exception>
        [Description("Call this method with the misspelled word and it will prompt you for the corrected spelling."),Category("Action")] 
        public void AssignCorrectSpelling(string strBadWord)
        {
            if (String.IsNullOrEmpty(m_strConverterSpec))
                throw new ExternalException("No project selected! Did you open a project?");

            // in case it was deleted by the user, recreate it now.
            if( File.Exists(m_strConverterSpec) )
            {
                // the file already exists... see if this word would otherwise already be altered by the cc table
                if( ChaChaChaChaChanges(strBadWord) )
                {
                    DialogResult res = MessageBox.Show(String.Format("There is already a replacement rule that affects the string ({0}). {1}{1}Click 'Retry' to display the existing rule or 'Ignore' to add a new rule{1}(which must be a longer string than the existing rule to override it).",strBadWord, Environment.NewLine), cstrCaption, MessageBoxButtons.AbortRetryIgnore );
                    if( res == DialogResult.Abort )
                    {
                        return;
                    }
                    else if( res == DialogResult.Retry )
                    {
                        this.FindReplacementRule(strBadWord);
                        return;
                    }
                }
            }
            else 
            {
				LoginSF.CreateCCTable(m_strConverterSpec, SpellFixerEncConverterName, PunctuationAndWhiteSpace);
			}

			QueryAndAppend(strBadWord);
        }

		protected void QueryAndAppend(string strBadWord)
		{
			var aQuery = new QueryFindReplaceDialog(m_font);
			if (aQuery.ShowDialog(strBadWord, strBadWord, strBadWord, IsRightToLeft, WordBoundaryDelimiter, false) == DialogResult.OK)
			{
				// if it was legacy encoded, then we need to convert the data to narrow using
				//  the code page the user specified (or we got out of the repository)
				Encoding enc = GetEncoding;

				// get a stream writer for these encoding and append
				var sw = new StreamWriter(m_strConverterSpec, true, enc);
				sw.WriteLine(FormatSubstitutionRule(aQuery.FindWhat, aQuery.ReplaceWith, WordBoundaryDelimiter, strBadWord));
				sw.Flush();
				sw.Close();
			}
		}

		[Description("Call this method with a misspelled word and it's replacement and they will be added to the fixup table."),Category("Action")] 
        public void AssignCorrectSpelling(string strBadWord, string strReplacement)
        {
            // in case it was deleted by the user, recreate it now.
            if( File.Exists(m_strConverterSpec) )
            {
                // the file already exists... see if this word would otherwise already be altered by the cc table
                if( ChaChaChaChaChanges(strBadWord) )
                {
                    DialogResult res = MessageBox.Show(String.Format("There is already a replacement rule that affects the string ({0}). {2}{2}Click 'Retry' to display the existing rule or 'Ignore' to continuing adding the new rule ({0})->({1}).",strBadWord, strReplacement, Environment.NewLine), cstrCaption, MessageBoxButtons.AbortRetryIgnore );
                    if( res == DialogResult.Abort )
                    {
                        return;
                    }
                    else if( res == DialogResult.Retry )
                    {
                        this.FindReplacementRule(strBadWord);
                        return;
                    }
                }
            }
            else 
            {
				LoginSF.CreateCCTable(m_strConverterSpec, SpellFixerEncConverterName, PunctuationAndWhiteSpace);
			}

			// save it as utf-8
			Encoding enc = GetEncoding;

            // get a stream writer for this encoding and append
            var sw = new StreamWriter(m_strConverterSpec,true,enc);
            sw.WriteLine(FormatSubstitutionRule(strBadWord,strReplacement,WordBoundaryDelimiter, strBadWord));
            sw.Flush();
            sw.Close();
        }

        [Description("Bring up the Fix Spelling dialog box with the replacement rule which changes (or results in) the give word."),Category("Action")] 
        public void FindReplacementRule(string strWord)
        {
            // first make sure the CC table exists
            if( (m_strConverterSpec != null) && File.Exists(m_strConverterSpec) )
            {
                CleanWord(ref strWord);

                // Open the CC table that has the mappings and put them in a DataTable.
                Encoding enc = GetEncoding;
				if (InitializeDataTableFromCCTable(m_strConverterSpec, enc, WordBoundaryDelimiter, out DataTable myTable))
				{
					// temporary filename for temporary CC tables (to check portions of the file at a time)
					string strTempName = Path.GetTempFileName();

                    // get a CC table EncConverter
                    IEncConverter aEC = new EncConverters().NewEncConverterByImplementationType(EncConverters.strTypeSILcc);
                    
                    // check to make sure that the whole table has a rule which changes it (it might not)
                    int nFoundIndex = -1;
                    if( ChaChaChaChaChanges(aEC, m_strConverterSpec, strWord) )
                    {
                        // do a binary search to find the one replacement rule that causes a change
                        int nLength = myTable.Rows.Count, nIndex = 0;
                        nFoundIndex = nIndex;
                        DataTable tblTestingRules = GetDataTable;

                        while( nLength > 1 )
                        {
                            // check the lower half
                            int nLowHalfLength = nLength / 2;
                            // GetPortionOfTable(myTable, nIndex, nLowHalfLength, ref tblTestingRules);
                            if( ChaChaChaChaChanges(aEC, strTempName, enc, strWord, myTable, nIndex, nLowHalfLength) )
                            {
                                // found in the lower half
                                nFoundIndex = nIndex;
                                nLength = nLowHalfLength;
                            }
                            else
                            {
                                // otherwise check in the upper half
                                // GetPortionOfTable(myTable, nIndex + nLowHalfLength, nLength - nLowHalfLength, ref tblTestingRules);
                                if( ChaChaChaChaChanges(aEC, strTempName, enc, strWord, myTable, nIndex + nLowHalfLength, nLength - nLowHalfLength) )
                                {
                                    // found in the upper half
                                    nIndex += nLowHalfLength;
                                    nFoundIndex = nIndex;
                                    nLength -= nLowHalfLength;
                                }
                            }
                        }
                    }

                    // clean up the temporary file.
                    File.Delete(strTempName);

                    // if we didn't see any rules that manipulate the input string, then see if any generate
                    //  the input string (i.e. compare the word against the right-hand side)
                    if( nFoundIndex == -1 )
                    {
                        // let's trim it of external spaces first
                        strWord = strWord.Trim();
                        for(nFoundIndex = 0; nFoundIndex < myTable.Rows.Count; nFoundIndex++)
                        {
                            DataRow row = myTable.Rows[nFoundIndex];
                            if( strWord == (string)row[strColumnRhs] )
                                break;
                        }
                    }

                    if( nFoundIndex == myTable.Rows.Count )
                    {
                        // none found
                        MessageBox.Show(String.Format("There are no substitution rules that apply to this word ({0})!", strWord), cstrCaption);
                    }

                    else if( (nFoundIndex >= 0) && (nFoundIndex < myTable.Rows.Count) )
                    {
                        DataRow row = myTable.Rows[nFoundIndex];
						var aQuery = new QueryFindReplaceDialog(m_font);
						DialogResult res = aQuery.ShowDialog((string)row[strColumnLhs], (string)row[strColumnRhs], GetComment(row), IsRightToLeft, WordBoundaryDelimiter, true);
						bool bRewrite = false;
                        if( res == DialogResult.Abort )
                        {
                            // this means to delete the bad substitution rule
                            myTable.Rows.RemoveAt(nFoundIndex);
                            bRewrite = true;
                        }
                        
                        // if the user clicks OK and has made a change...
                        if(     (res == DialogResult.OK)
                            &&  (   ((string)row[strColumnLhs] != aQuery.FindWhat)
                                ||  ((string)row[strColumnRhs] != aQuery.ReplaceWith)
                                )
                        )
                        {
                            // update the table and rewrite
                            row[strColumnLhs] = aQuery.FindWhat;
                            row[strColumnRhs] = aQuery.ReplaceWith;
                            row[strColumnCmt] = strWord;
                            bRewrite = true;
                        }

                        if( bRewrite )
                        {
                            // write the newly updated DataTable
                            LoginSF.ReWriteCCTableHeader(m_strConverterSpec, PunctuationAndWhiteSpace, enc, myTable.Rows.Count > 0);
                            AppendCCTableFromDataTable(m_strConverterSpec,enc,WordBoundaryDelimiter,PunctuationAndWhiteSpace, myTable);
                        }
                    }
                }
            }
        }

        [Description("Use this method to edit the list of spelling fixes")]
        public void EditSpellingFixes()
        {
            if( m_strConverterSpec != null )
            {
                // Open the CC table that has the mappings and put them in a DataTable.
                if( !File.Exists(m_strConverterSpec) )
                {
					LoginSF.CreateCCTable(m_strConverterSpec, SpellFixerEncConverterName, PunctuationAndWhiteSpace);
				}

				// if it was legacy encoded, then we need to convert the data to narrow using
				//  the code page the user specified (or we got out of the repository)
				Encoding enc = GetEncoding;
				if (InitializeDataTableFromCCTable(m_strConverterSpec, enc, WordBoundaryDelimiter, out DataTable myTable))
				{
					// now put up an editable grid with this data.
					DialogResult res = DialogResult.Cancel;
                    try
                    {
                        var dlg = new ViewBadGoodPairsDlg(myTable, m_font);
                        res = dlg.ShowDialog();
                    }
#if DEBUG
					catch (Exception ex)
					{
						MessageBox.Show(ex.Message, cstrCaption);
					}
#else
                    catch { }
#endif

					if ( res == DialogResult.OK )
                    {
                        LoginSF.ReWriteCCTableHeader(m_strConverterSpec,PunctuationAndWhiteSpace,enc, myTable.Rows.Count > 0);
                        AppendCCTableFromDataTable(m_strConverterSpec,enc,WordBoundaryDelimiter,PunctuationAndWhiteSpace, myTable);
                    }
                }
            }
        }

        protected Encoding GetEncoding
        {
            get
            {
				var enc = new UTF8Encoding();
				return enc;
            }
        }

        protected static DataTable GetDataTable
        {
            get
            {
                DataTable myTable = new DataTable("SpellingFixesList");
                myTable.Columns.Add(new DataColumn(strColumnLhs,typeof(string)));
                myTable.Columns.Add(new DataColumn(strColumnRhs,typeof(string)));
                myTable.Columns.Add(new DataColumn(strColumnCmt,typeof(string)));
                return myTable;
            }
        }

        // clean input word
        private static void CleanWord(ref string str)
        {
            // then strip off invalid chars (that sometimes come in from Word)
            if (!String.IsNullOrEmpty(str))
            {
                int nIndexBadChar = 0;
                var aBadChars = new char[] {'\r', '\n'};
                while( (nIndexBadChar = str.IndexOfAny(aBadChars)) != -1 )
                    str = str.Remove(nIndexBadChar,1);
            }
        }

        private static string GetComment(DataRow row)
        {
            string strRet = null;
            if( row[strColumnCmt] != System.DBNull.Value )
                strRet = row[strColumnCmt].ToString();
            return strRet;
        }

        private bool ChaChaChaChaChanges(string strWord)
        {
            IEncConverter aEC = new EncConverters().NewEncConverterByImplementationType(EncConverters.strTypeSILcc);
            return ChaChaChaChaChanges(aEC, m_strConverterSpec, strWord);
        }

        private bool ChaChaChaChaChanges(IEncConverter aEC, string strFileName, string strWord)
        {
            string strDummy = null;
            int lProcessType = (int)SpellingFixer.SFProcessType;
			var eConvType = ConvType.Unicode_to_Unicode;
			aEC.Initialize("dummyname", strFileName, ref strDummy, ref strDummy, ref eConvType, ref lProcessType, 0, 0, true);
            return (aEC.Convert(strWord) != strWord);
        }

        private bool ChaChaChaChaChanges(IEncConverter aEC, string strFileName, Encoding enc, string strWord, DataTable tblData, int nTableIndex, int nNumRows)
        {
            this.WriteCCTableFromDataTable(strFileName, enc, tblData, nTableIndex, nNumRows);
            return ChaChaChaChaChanges(aEC,strFileName,strWord);
        }

        private void GetPortionOfTable(DataTable myTable, int nIndex, int nLength, ref DataTable tblTestingRules)
        {
            tblTestingRules.Clear();
            for(int i = nIndex; (nLength-- > 0); nIndex++)
            {
                DataRow row = myTable.Rows[nIndex];
                DataRow newRow = tblTestingRules.NewRow();
                newRow[strColumnLhs] = row[strColumnLhs];
                newRow[strColumnRhs] = row[strColumnRhs];
                newRow[strColumnCmt] = row[strColumnCmt];
                tblTestingRules.Rows.Add(newRow);
            }
        }

        internal static string FormatSubstitutionRule(string strBad, string strGood, string strWordBoundaryDelimiter, string strCommentWord)
        {
            // if the user indicated a word boundary condition (i.e. #pete, ete#, or #pete#)
            //  then we have to put special stuff in the CC table to search for 
            //  preceding or trailing whitespace.
            // spuriously, there may be certain characters which we can't tolerate
            CleanWord(ref strBad);
            CleanWord(ref strGood);
            CleanWord(ref strCommentWord);

            string strDelimiter = strWordBoundaryDelimiter;
            int nDelimiterLen = strDelimiter.Length;
            string strLhsFormat = null;
            try
            {
                if( strBad.Substring(0,nDelimiterLen) == strDelimiter )
                {
                    strLhsFormat = cstrPrecWhiteSpace + chSpace;
                    strBad = strBad.Remove(0,nDelimiterLen);
                }
            }
            catch {}    // don't care, but don't want to check lengths to avoid ArgumentOutOfRangeException

            strLhsFormat += strQuotedFormat;

            int nIndex = strBad.Length - nDelimiterLen;
            try
            {
                if( strBad.Substring(nIndex) == strDelimiter )
                {
                    strBad = strBad.Substring(0,nIndex);
                    strLhsFormat += chSpace + cstrFollWhiteSpace;
                }
            }
            catch {}    // don't care, but don't want to check lengths to avoid ArgumentOutOfRangeException

            string str = cstrIndentation + String.Format(strLhsFormat,strBad) + " > " + String.Format(strQuotedFormat,strGood);
            if( !String.IsNullOrEmpty(strCommentWord) )
                str += String.Format(cstrCommentFormat, strCommentWord);
            return str;
        }

        internal const string strColumnLhs = "Bad Spelling";
        internal const string strColumnRhs = "Good Spelling";
        internal const string strColumnCmt = "Comment";

        internal static void AppendCCTableFromDataTable
            (
            string      strConverterSpec, 
            Encoding    enc,
            string      strWordBoundaryDelimiter,
            string      strPunctuationAndWhiteSpace,
            DataTable   myTable
            )
        {
            // get a stream writer to write the new pairs
            StreamWriter sw = new StreamWriter(strConverterSpec,true,enc);

            AppendCCTableFromDataTable(sw, strWordBoundaryDelimiter, strPunctuationAndWhiteSpace, myTable, 0, myTable.Rows.Count);

            sw.Flush();
            sw.Close();
        }

        internal void WriteCCTableFromDataTable(string strFilename, Encoding enc, DataTable tbl, int nTableIndex, int nNumRows)
        {
            if( File.Exists(strFilename) )
                File.Delete(strFilename);

            StreamWriter sw = new StreamWriter(strFilename,false,enc);
			LoginSF.CreateCCTable(sw, SpellFixerEncConverterName, PunctuationAndWhiteSpace);
			AppendCCTableFromDataTable(sw, WordBoundaryDelimiter, PunctuationAndWhiteSpace, tbl, nTableIndex, nNumRows);
            sw.Flush();
            sw.Close();
        }

        internal static void AppendCCTableFromDataTable
            (
            StreamWriter    sw, 
            string          strWordBoundaryDelimiter,
            string          strPunctuationAndWhiteSpace,
            DataTable       myTable,
            int             nTableIndex,
            int             nNumRows
            )
        {
            // iterate the rows and write the pairs
            Debug.Assert(nTableIndex + nNumRows <= myTable.Rows.Count);
            for(int i = nTableIndex; nNumRows-- > 0; i++)
            {
                try
                {
                    DataRow row = myTable.Rows[i];
                            
                    string strBadSpelling = row[strColumnLhs].ToString();
                    string strGoodSpelling = row[strColumnRhs].ToString();
                    string strCommentWord = GetComment(row);

                    sw.WriteLine(FormatSubstitutionRule(strBadSpelling, strGoodSpelling, strWordBoundaryDelimiter, strCommentWord));
                }
                catch {}
            }
        }

        internal static StreamReader InitReaderPastHeader(string strConverterSpec, Encoding enc)
        {
            // get a stream writer for these encoding and append
            StreamReader sr = new StreamReader(strConverterSpec, enc);

            // skip past the header lines
            string line = null;
            do
            {
                line = sr.ReadLine();

            } while ((line != LoginSF.CctableLastHeaderLine) && (line != null));

            return sr;
        }

        internal static bool InitializeDataTableFromCCTable
            (
            string          strConverterSpec, 
            Encoding        enc, 
            string          strWordBoundaryDelimiter,
            out DataTable   myTable
            )
        {
            // get a stream writer for these encoding and append
            StreamReader sr = InitReaderPastHeader(strConverterSpec,enc);

            myTable = GetDataTable;
            string line = null;
            while ((line = sr.ReadLine()) != null)
            {
                string strLhs = null;
                try
                {
                    // we have a preceding white space qualifier if the first part of the
                    //  string is the cstrPrecWhiteSpace.
                    if( line.Substring(cstrIndentation.Length,cstrPrecWhiteSpace.Length) == cstrPrecWhiteSpace )
                        strLhs = strWordBoundaryDelimiter;
                }
                catch {}    // don't care, but don't want to check lengths (sometimes gives ArgumentOutOfRangeException)

                int nLhsLeftIdx = line.IndexOf('\"',0) + 1;
                int nLhsRightIdx = line.IndexOf('\"',nLhsLeftIdx);
                Debug.Assert( (nLhsLeftIdx != -1) && (nLhsRightIdx != -1) && ((nLhsRightIdx-nLhsLeftIdx) < line.Length) );
                if( (nLhsLeftIdx != -1) && (nLhsRightIdx != -1) && ((nLhsRightIdx-nLhsLeftIdx) < line.Length) )
                {
                    strLhs += line.Substring(nLhsLeftIdx,nLhsRightIdx-nLhsLeftIdx);

                    bool bFollWhiteSpace = (line.IndexOf(cstrFollWhiteSpace,nLhsRightIdx + 1) != -1);
                    if( bFollWhiteSpace )
                        strLhs += strWordBoundaryDelimiter;

                    int nRhsLeftIdx = line.IndexOf('\"',nLhsRightIdx + 1) + 1;
                    int nRhsRightIdx = line.IndexOf('\"',nRhsLeftIdx);
                    Debug.Assert( (nRhsLeftIdx != -1) && (nRhsRightIdx != -1) && ((nRhsRightIdx-nRhsLeftIdx) < line.Length) );
                    if( (nRhsLeftIdx != -1) && (nRhsRightIdx != -1) && ((nRhsRightIdx-nRhsLeftIdx) < line.Length) )
                    {
                        string strRhs = line.Substring(nRhsLeftIdx,nRhsRightIdx-nRhsLeftIdx);
                        string strCommentWord = null;
                        nRhsRightIdx += cstrCommentFormat.Length - 3;
                        if( nRhsRightIdx <= line.Length )
                            strCommentWord = line.Substring(nRhsRightIdx, line.Length - nRhsRightIdx - 1);

                        DataRow rowNew = myTable.NewRow();
                        rowNew[strColumnLhs] = strLhs;
                        rowNew[strColumnRhs] = strRhs;
                        rowNew[strColumnCmt] = strCommentWord;
                        myTable.Rows.Add(rowNew);
                    }
                }
            }
            sr.Close();

            return true;
        }

        [Description("Returns the name of the EncConverter to use to correct the spelling for the selected project."),Category("Data")] 
        public override string ToString()
        {
            return SpellFixerEncConverterName;
        } 
    }
}
