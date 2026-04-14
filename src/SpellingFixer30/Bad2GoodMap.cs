using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;

namespace SpellingFixer30
{
    internal class Bad2GoodMap : Dictionary<string,string>
    {
		public const string WordBoundaryCharacter = "#";

		protected DateTime m_dtLastRead = DateTime.MinValue;
        private string m_strFileSpec = null;
        public bool DoingLoadOrSave = false;

        public Bad2GoodMap(string strPath)
        {
            m_strFileSpec = strPath;
        }

        public new void Add(string strBadWord, string strGoodWord)
        {
            CheckForOutOfDate();
            base.Add(strBadWord, strGoodWord);
        }

        public new string this[string key]
        {
            get
            {
                CheckForOutOfDate();
                return (string)base[key];
            }
        }

        public new int Count
        {
            get
            {
                CheckForOutOfDate();
                return base.Count;
            }
        }

        public new bool ContainsValue(string value)
        {
            CheckForOutOfDate();
            return base.ContainsValue(value);
        }

        public new Enumerator GetEnumerator()
        {
            // this might be out of date, for example, during Save after we write the header
            //  CheckForOutOfDate();
            return base.GetEnumerator();
        }

        public new bool ContainsKey(string key)
        {
            // CheckForOutOfDate();
            return base.ContainsKey(key);
        }

        public void CheckForOutOfDate()
        {
            var dtLastModified = DateTime.MinValue;
            var fileExists = CscProject.DoesFileExist(m_strFileSpec, ref dtLastModified);

            if (DoingLoadOrSave)
            {
                // if we're in the middle of a load or save, don't try to reload (but the 
                //  timestamp shouldn't be out of date or we haven't sequenced things properly)
                // read: if you're going to save, then make sure to call this method before 
                //  you set m_bDoingLoadOrSave to true.
                System.Diagnostics.Debug.Assert(!fileExists || (dtLastModified == m_dtLastRead));
                return;
            }

            // if the file has changed since we last read it, reload it (note the "!=" comparison,
            //  not ">", in case the file got replaced with an older copy, since we back them up)
            if (fileExists && (dtLastModified != m_dtLastRead))
            {
                base.Clear();
                LoadTable();
            }
        }

        public static Bad2GoodMap LoadTable(string strPath)
        {
            Bad2GoodMap map = new Bad2GoodMap(strPath);
            map.LoadTable();
            return map;
        }

        protected void LoadTable()
        {
            try
            {
                DoingLoadOrSave = true;

                // get a stream writer for this encoding and append
                if (CscProject.DoesFileExist(m_strFileSpec, ref m_dtLastRead))
                {
                    StreamReader sr = SpellingFixer.InitReaderPastHeader(m_strFileSpec, Encoding.UTF8);

                    string line = null;
                    while ((line = sr.ReadLine()) != null)
                    {
                        int nLhsLeftIdx = line.IndexOf('\"', 0) + 1;
                        int nLhsRightIdx = line.IndexOf('\"', nLhsLeftIdx);
                        System.Diagnostics.Debug.Assert((nLhsLeftIdx != -1) && (nLhsRightIdx != -1) && ((nLhsRightIdx - nLhsLeftIdx) < line.Length));
                        if ((nLhsLeftIdx != -1) && (nLhsRightIdx != -1) && ((nLhsRightIdx - nLhsLeftIdx) < line.Length))
                        {
                            string strLhs = line.Substring(nLhsLeftIdx, nLhsRightIdx - nLhsLeftIdx);

                            int nRhsLeftIdx = line.IndexOf('\"', nLhsRightIdx + 1) + 1;
                            int nRhsRightIdx = line.IndexOf('\"', nRhsLeftIdx);
                            System.Diagnostics.Debug.Assert((nRhsLeftIdx != -1) && (nRhsRightIdx != -1) && ((nRhsRightIdx - nRhsLeftIdx) < line.Length));

                            if ((nRhsLeftIdx != -1) && (nRhsRightIdx != -1) && ((nRhsRightIdx - nRhsLeftIdx) < line.Length))
                            {
                                string strRhs = line.Substring(nRhsLeftIdx, nRhsRightIdx - nRhsLeftIdx);
                                base.Add(strLhs, strRhs);
                            }
                        }
                    }
                    sr.Close();
                }
            }
            finally
            {
                DoingLoadOrSave = false;
            }
        }

        public static void SaveTable(Bad2GoodMap map, string strFileSpec, string strEncConverterName, string strPunctuationAndWhiteSpace, string strCustomCode)
        {
            // always make a backup
            CscProject.BackupFile(strFileSpec);
            map.SaveTable(strFileSpec, strEncConverterName, strPunctuationAndWhiteSpace, strCustomCode);
        }

        protected void SaveTable(string strFileSpec, string strEncConverterName, string strPunctuationAndWhiteSpace, string strCustomCode)
        {
            try
            {
                CheckForOutOfDate();    // in case something else changed it, reload it before we change it.

                DoingLoadOrSave = true;

                // get a stream writer for this encoding and append
                StreamWriter sw = new StreamWriter(strFileSpec, false, Encoding.UTF8);
				CreateCCTableCsc(sw, strEncConverterName, strPunctuationAndWhiteSpace, strCustomCode, true);

                // order them in the file first by those with a hyphen in alphabetical order,
                //  followed by those without hyphens in alphabetical order. (so that a non-hypthenated
                //  word that is a substring of a hyphenated word doesn't get matched first).
                foreach (KeyValuePair<string, string> kvp in this.OrderBy(kvp => !kvp.Key.Contains("-"))
                                                                 .ThenBy(kvp => kvp.Key))
                {
                    // always surround the word with delimiters, because in this application
                    //  it's always full word form searching
                    string strOrigWord = kvp.Key;
                    string strBadSpelling = $"{WordBoundaryCharacter}{strOrigWord}{WordBoundaryCharacter}";
                    string strReplacement = kvp.Value;
                    sw.WriteLine(SpellingFixer.FormatSubstitutionRule(strBadSpelling, strReplacement, WordBoundaryCharacter, strOrigWord));
                }
                sw.Flush();
                sw.Close();

                m_strFileSpec = strFileSpec;
                CscProject.DoesFileExist(m_strFileSpec, ref m_dtLastRead);
            }
            finally
            {
                DoingLoadOrSave = false;
            }
        }

		internal static void CreateCCTable(FileStream fs, string strEncConverterName, string strPunctuation, string strCustomCode, bool bUnicode)
		{
			// write out the header lines.
			StreamWriter sw = new StreamWriter(fs);
			CreateCCTableCsc(sw, strEncConverterName, strPunctuation, strCustomCode, bUnicode);
			sw.Flush();
			sw.Close();
		}

		internal static string CctableLastHeaderLine
		{
			get { return "c Last Header Line: DON'T modify the table beyond this point (or your changes may be overwritten)"; }
		}

		internal const string DummyRule = "    d31 > d31 c dummy rule so that the table isn't empty (can be removed if you have rules below the Last Header Line below)";
		internal const string CustomRuleEndComment = "c +----------end custom changes----------+";

		internal static void CreateCCTableCsc(StreamWriter sw, string strEncConverterName, string strPunctuation, string strCustomCode, bool bUnicode)
		{
			// write out the header lines.
			string strVersion = Assembly.GetExecutingAssembly().GetName().Version.ToString();
			sw.WriteLine(String.Format("c This cc table was created by SpellingFixer30.dll v{0} on {1}.", strVersion, DateTime.Now.ToShortDateString()));
			sw.WriteLine(String.Format("c It can be accessed as the '{0}' EncConverter", strEncConverterName));
			sw.WriteLine("c If you know how to program CC, you can add special processing between the");
			sw.WriteLine("c 'start custom changes' and 'end custom changes' comments below.");
			string strBeginStmt = "begin >";
			if (bUnicode)
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
			if ((strCCTableSpec != null) && File.Exists(strCCTableSpec))
			{
				const string strTempExt = ".new";
				// get a stream writer for these encoding and append
				var sr = new StreamReader(strCCTableSpec, enc);
				var sw = new StreamWriter(strCCTableSpec + strTempExt, false, enc);

				// this is for version 1.2.0.0
				string strVersion = Assembly.GetExecutingAssembly().GetName().Version.ToString();
				var copiedDummyRule = false;

				// copy the read stuff to the output and update the 'ws' store line
				for (string line = sr.ReadLine(); line != null; line = sr.ReadLine())
				{
					if (line.Contains("c This cc table was created by SpellingFixerEC.dll v"))
						line = String.Format("c This cc table was created by SpellingFixerEC.dll v{0} on {1}.", strVersion, DateTime.Now.ToShortDateString());
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
					if (line == CctableLastHeaderLine)
						break;
				}

				sw.Flush();
				sw.Close();
				sr.Close();

				string strBackupFilename = strCCTableSpec + ".bak";
				if (File.Exists(strBackupFilename))
					File.Delete(strBackupFilename);
				File.Move(strCCTableSpec, strBackupFilename);
				File.Move(strCCTableSpec + strTempExt, strCCTableSpec);
			}
		}
	}
}