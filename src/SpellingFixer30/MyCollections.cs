using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.Runtime.InteropServices;
using System.IO;
using System.Runtime.Serialization;                 // for SerializationException
using Microsoft.SqlServer.Server;
using System.Linq;


#if !SaveSoapFormatter
using System.Runtime.Serialization.Formatters.Binary;
#else
using System.Runtime.Serialization.Formatters.Soap; // for soap formatter
#endif

namespace SpellingFixer30
{
	/// <summary>
	/// The object that encapsulates all the information about a Word to check consistent 
	/// spelling with
	/// </summary>
    [ComVisible(true)]
    [ClassInterface(ClassInterfaceType.AutoDual)]
    public class SpellFixerWord
	{
		string m_strWord = null;
		List<string> m_lstContext = new List<string>();
		int m_nCount = 0;

		// these values have to be regeneraged on Load anyway, so no sense saving them
		string m_strTransliteratedWord = null;
		string m_strSimplifiedWord = null;

		/// <summary>
		/// The constructor initializes all the pieces of information we store for each "word"
		/// </summary>
		/// <param name="strWord">The actual word to test the consistent spelling of (which becomes the 'Value' property)</param>
		/// <param name="strSimplifiedWord">The simplification of the word which is used for comparison to find potential misspellings (becomes SimplifiedValue)</param>
		/// <param name="strTransliteratedValue">The transliteration of the word using the transliterator defined in the projects Vernacular Script System (becomes TransliteratedValue)</param>
		/// <param name="strContext">An optional string showing the surrounding context--for Tooltips, etc. (becomes Context)</param>
		public SpellFixerWord(CscProject project, string strWord, List<string> context)
		{
			Value = strWord;
			ContextStrings = context;
            IncrementCount();

            // and the stuff that happens each time fresh when it is loaded
            InitializeNonStaticData(project);
		}

        /// <summary>
        /// Call this method when serializing in to regenerate the non-static information
        /// (currently, the transliterated value and simplified value
        /// </summary>
        /// <param name="project">The project reference so we know *how* to Simplify and Transliterate</param>
        public void InitializeNonStaticData(CscProject project)
        {
            TransliteratedValue = project.TransliteratedWord(Value);
            SimplifiedValue = project.SimplifiedWord(TransliteratedValue, false);
        }

		// default parameter is the value (i.e. the actual word)
		/// <summary>
		/// The actual word to test the consistent spelling of in vernacular script form (e.g. किताब)
		/// Retentive when this object is saved to a store.
		/// </summary>
		[DispId(0)]
		public string Value
		{
			get { return m_strWord; }
			set { m_strWord = value; }
		}

		/// <summary>
		/// The number of times the word has occurred (i.e. been added via CscProject.AddWordToCheckList)
		/// Retentive when this object is saved to a store.
		/// </summary>
		public int Count
		{
			get { return m_nCount; }
			set { m_nCount = value; }
		}

		/// <summary>
		/// An optional string showing the surrounding context (for use in tooltips etc)
		/// Retentive when this object is saved to a store.
		/// </summary>
		public List<string> ContextStrings
        {
            get { return m_lstContext; }
            set { m_lstContext = value; }
        }

		/// <summary>
		/// Call this to increase the occurrence count of a word by 1
		/// Typically, an internal operation
		/// </summary>
		/// <returns>The new occurrence count</returns>
		public int IncrementCount() // short-cut
		{
			return ++m_nCount;
		}

		/// <summary>
		/// The transliterate form of the Value
		/// Non-retentive when this object is saved to a store (it is recalculated on each loading 
		/// in case the transliteration algorithm is changed)
		/// </summary>
		public string TransliteratedValue
		{
			get { return m_strTransliteratedWord; }
			set { m_strTransliteratedWord = value; }
		}

		/// <summary>
		/// The transliterated and simplified form of the word used for finding similar spellings
		/// (for example, hypothetically with the Indic Vernacular Script System)
		///		Transliterated: खीता becomes khītā
		///		Simplified: khītā becomes kita
		/// which will then have the same Simplified value (and therefore be ambiguous) with words like:
		///		कीता (kītā->kita), किता (kitā->kita), कीथा (kīthā->kita), and कीत (kīta->kita)
		/// Non-retentive when this object is saved to a store (it is recalculated on each loading 
		/// in case the transliteration or simplifying algorithm is changed)
		/// </summary>
		public string SimplifiedValue
		{
			get { return m_strSimplifiedWord; }
			set { m_strSimplifiedWord = value; }
		}

		public override string ToString()
		{
			return Value;
		}
	}

    [ComVisible(true)]
    [ClassInterface(ClassInterfaceType.AutoDual)]
    public class SpellFixerWords : List<SpellFixerWord>
	{
        public SpellFixerWords()
        {
        }

        public SpellFixerWords(int capacity)
            : base(capacity)
        {
        }
	}

    [ComVisible(true)]
    [ClassInterface(ClassInterfaceType.AutoDual)]
    public class SpellFixerWordBundles : ArrayList
	{
		public new SpellFixerWords this[int key]
		{
			get { return (SpellFixerWords)base[key]; }
		}
	}

	internal abstract class HashtableStore : Dictionary<string, SpellFixerWord>
    {
        protected DateTime m_dtLastRead = DateTime.MinValue;
        protected string m_strFileSpec = null;
        protected CscProject m_project = null;

        public HashtableStore(string strFileSpec, CscProject project)
        {
            m_strFileSpec = strFileSpec;
            m_project = project;
        }

        public virtual void Add(SpellFixerWord sfw)
        {
            CheckForOutOfDate();
            base.Add(sfw.Value, sfw);
        }

        public new SpellFixerWord this[string key]
        {
            get 
            {
                CheckForOutOfDate();
                return base[key]; 
            }
        }

        protected static void LoadTable(string strFilename, HashtableStore map, CscProject project)
        {
            map.LoadTable(strFilename, project);
        }

        public new int Count
        {
            get
            {
                CheckForOutOfDate();
                return base.Count;
            }
        }

        public new bool ContainsValue(SpellFixerWord value)
        {
            CheckForOutOfDate();
            return base.ContainsValue(value);
        }

        public new bool ContainsKey(string key)
        {
            CheckForOutOfDate();
            return base.ContainsKey(key);
        }

        protected void LoadTable(string strFilename, CscProject project)
        {
#if DEBUG
            m_bDoingLoadOrSave = true;
#endif
            if (CscProject.DoesFileExist(strFilename, ref m_dtLastRead))
            {
                SpellFixerHashtableStore xmlFile = null;
                try
                {
                    xmlFile = SpellFixerHashtableStore.LoadFromXml(strFilename);
                    xmlFile.CaseSensitive = true;

                    foreach (var item in xmlFile.Items)
                    {
                        var sfw = new SpellFixerWord(project, item.Key, item.ContextList.Contexts)
                        {
                            Count = item.Count,
                            Value = item.Value,
                        };
                        base.Add(item.Key, sfw);
                    }
                }
                catch (Exception e)
                {
                    throw new ApplicationException(String.Format("Unable to open data store {0}, reason: {1}", strFilename, e.Message));
                }
            }
#if DEBUG
            m_bDoingLoadOrSave = false;
#endif
        }

        public static void SaveTable(HashtableStore map, string strPath, string strFilename)
        {
            map.SaveTable(strPath, strFilename);
        }

        public void SaveTable(string strPath, string strFilename)
        {
#if DEBUG
            m_bDoingLoadOrSave = true;
#endif
            // always make a backup
            var strFileSpec = Path.Combine(strPath, strFilename);
            CscProject.BackupFile(strFileSpec);

            SpellFixerHashtableStore xmlFile = null;
            try
            {
                xmlFile = new SpellFixerHashtableStore
                {
                    CaseSensitive = true
                };
                foreach (var kvp in this)
                    xmlFile.Items.Add(new Item
                    {
                        Key = kvp.Key,
                        Value = kvp.Value.Value,
                        Count = kvp.Value.Count,
                        ContextList = new ContextList { Contexts = kvp.Value.ContextStrings }
                    });

                xmlFile.SaveToXml(strFileSpec);
            }
            catch (Exception e)
            {
                throw new ApplicationException(String.Format("Save to data store {0} failed, reason: {1}", strFileSpec, e.Message));
            }
            finally
            {
            }

            m_strFileSpec = strFileSpec;
            CscProject.DoesFileExist(m_strFileSpec, ref m_dtLastRead);
#if DEBUG
            m_bDoingLoadOrSave = false;
#endif
        }

#if DEBUG
        protected bool m_bDoingLoadOrSave = false;
#endif

        protected void CheckForOutOfDate()
        {
#if DEBUG
            System.Diagnostics.Debug.Assert(!m_bDoingLoadOrSave);
#endif
            DateTime dtLastModified = DateTime.MinValue;
            if (CscProject.DoesFileExist(m_strFileSpec, ref dtLastModified))
            {
                if (dtLastModified > m_dtLastRead)
                {
                    Clear();
                    LoadTable(m_strFileSpec, m_project);
                }
            }
        }
    }

    internal class KnownGoodWordList : HashtableStore
    {
        public static string MyFilename => "KnownGoodWordsStore.xml"; 

        public KnownGoodWordList(string strFileSpec, CscProject project)
            : base(strFileSpec, project)
        {
        }

        /// <summary>
        /// Returns a collection object containing SpellFixerWord objects
        /// </summary>
        /// <param name="strPath">Path to the project folder e.g. strPathToProjectFolder = @"C:\ProgramData\SIL\ConsistentSpellingFixer\Projects\Kangri"</param>
        /// <returns>KnownGoodWordList</returns>
        public static new KnownGoodWordList LoadTable(string strPathToProjectFolder, CscProject project)
        {
            string strFilename = Path.Combine(strPathToProjectFolder, MyFilename);
            KnownGoodWordList map = new KnownGoodWordList(strFilename, project);

            LoadTable(strFilename, map, project);
            
            return map;
        }

        /// <summary>
        /// Call to save a SpellFixerWord collection
        /// </summary>
        /// <param name="strPath">Path to the project folder e.g. strPathToProjectFolder = @"C:\ProgramData\SIL\ConsistentSpellingFixer\Projects\Kangri"</param>
        public static void SaveTable(KnownGoodWordList mapWords, string strPathToProjectFolder)
        {
            SaveTable(mapWords, strPathToProjectFolder, MyFilename);
        }
    }

    internal class WordsToCheck : HashtableStore
    {
        public static string MyFilename => "WordsToCheckStore.xml";

        public WordsToCheck(string strFileSpec, CscProject project)
            : base(strFileSpec, project)
        {
        }

        /// <summary>
        /// Returns a collection of SpellFixerWord objects that are in the list of "words to check".
        /// </summary>
        /// <param name="strPath">Path to the project folder e.g. strPathToProjectFolder = @"C:\ProgramData\SIL\ConsistentSpellingFixer\Projects\Kangri"</param>
		/// <returns>WordsToCheck</returns>
        public static new WordsToCheck LoadTable(string strPathToProjectFolder, CscProject project)
        {
            string strFilename = Path.Combine(strPathToProjectFolder, MyFilename);
            WordsToCheck map = new WordsToCheck(strFilename, project);

            LoadTable(strFilename, map, project);

            return map;
        }

        /// <summary>
        /// Call to save a SpellFixerWord collection
        /// </summary>
        /// <param name="strPath">Path to the project folder e.g. strPathToProjectFolder = @"C:\ProgramData\SIL\ConsistentSpellingFixer\Projects\Kangri"</param>
        public static void SaveTable(WordsToCheck mapWords, string strPathToProjectFolder)
        {
            SaveTable(mapWords, strPathToProjectFolder, WordsToCheck.MyFilename);
        }
	}

    internal class WordsToCheckBulk : WordsToCheck
    {
        public static new string MyFilename => "WordsToCheckBulkStore.xml";
        
        public WordsToCheckBulk()
            : base(null, null)
        {
        }

        public bool BulkFileExists(string strPathToProjectFolder)
        {
            string strFilename = Path.Combine(strPathToProjectFolder, MyFilename);
            m_strFileSpec = strFilename;
            return File.Exists(strFilename);
        }

        public void LoadTable(CscProject project)
        {
            m_project = project;
            string strFilename = Path.Combine(project.ProjectDirectory, MyFilename);
            LoadTable(strFilename, this, project);
        }

        public static void SaveTable(WordsToCheck mapWords, CscProject project)
        {
            SaveTable(mapWords, project.ProjectDirectory, MyFilename);
        }

        public void DeleteStore(string strPathToProjectFolder)
        {
            try 
	        {
                File.Delete(Path.Combine(strPathToProjectFolder, MyFilename));
	        }
	        catch { }
        }

        public void Copy(WordsToCheck mapBasis)
        {
            foreach (SpellFixerWord sfw in mapBasis.Values)
                Add(sfw);
        }
    }

	internal class IgnoreList : HashtableStore
    {
        public static string MyFilename => "IgnoreListStore.xml";

        public IgnoreList(string strFileSpec, CscProject project)
            : base(strFileSpec, project)
        {
        }

        /// <summary>
        /// Returns a collection of SpellFixerWord objects that are in the list of "words to ignore".
        /// </summary>
        /// <param name="strPath">Path to the project folder e.g. strPathToProjectFolder = @"C:\ProgramData\SIL\ConsistentSpellingFixer\Projects\Kangri"</param>
        /// <returns>IgnoreList</returns>
        public static new IgnoreList LoadTable(string strPathToProjectFolder, CscProject project)
		{
			string strFilename = Path.Combine(strPathToProjectFolder, MyFilename);
            IgnoreList map = new IgnoreList(strFilename, project);

			LoadTable(strFilename, map, project);

			return map;
		}

        /// <summary>
        /// Call to save a SpellFixerWord collection
        /// </summary>
        /// <param name="strPath">Path to the project folder e.g. strPathToProjectFolder = @"C:\ProgramData\SIL\ConsistentSpellingFixer\Projects\Kangri"</param>
        public static void SaveTable(IgnoreList mapWords, string strPathToProjectFolder)
		{
			SaveTable(mapWords, strPathToProjectFolder, MyFilename);
		}
    }

    internal class CountPlusNameSortList : SortedList<string, SpellFixerWord>
    {
        public CountPlusNameSortList()
            : base(StringComparer.Ordinal)  // so it won't think one word is the same as another just because it contains a ZWNJ
        {
        }

        public virtual void Add(SpellFixerWord sfw, bool bKnownGoodWord)
        {
            // this can be called twice (e.g. in the KnownGoodWordList vs. WordsToCheck list)
            //  so when checking whether we've done this word before or not, be sure to make
            //  sure we haven't already added it (they could have different counts, so use
            //  the Values rather than the Keys)
            var word = sfw.Value;
            if (!Values.Any(v => v.Value == word))
            {
                string strKey = CreateSortKey(sfw, bKnownGoodWord);
                base.Add(strKey, sfw);
            }
        }

        internal static string CreateSortKey(SpellFixerWord sfw, bool bKnownGoodWord)
        {
            // known good words should always be "higher" in sort order. This can be accomplished
            //	by *not* prepending the count (which would otherwise, make it sort lower)
            // UPDATE: this is apparently not used...
#if false
            string strFormat = "{0:00000000}{1}";
            if (bKnownGoodWord)
                strFormat = "D" + strFormat;    // arbitrary "D" (higher order than numbers) to trump the non dictionary words
#endif
            string strKey = String.Format("{0:00000000}{1}", sfw.Count, sfw.Value);
            return strKey;
        }
    }
}
