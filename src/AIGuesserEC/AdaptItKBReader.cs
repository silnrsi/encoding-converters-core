using System;
using System.Drawing;
using System.Collections.Generic;       // for Dictionary<>
using System.Windows.Forms;
using System.Xml.Linq;
using System.IO;                        // for file I/O
using System.Xml;                       // for XMLDocument
using System.Xml.XPath;                 // for XPathNavigator
using ECInterfaces;                     // for IEncConverter

namespace SilEncConverters40
{
    /// <summary>
    /// Base class for an AdaptIt Knowledge Base EncConverter
    /// </summary>
    public abstract class AdaptItKBReader : EncConverter
    {
        #region Member Variable Definitions

        public abstract string strRegValueForConfigProgId { get; }
        
        protected DateTime m_timeModifiedKB = DateTime.MinValue;
        protected DateTime m_timeModifiedKbMapOfMaps = DateTime.MinValue;
        protected DateTime m_timeModifiedProj = DateTime.MinValue;
        protected DateTime m_timeModifiedNorm = DateTime.MinValue;
        protected Dictionary<int, Dictionary<string, string>> m_mapOfMaps = new Dictionary<int, Dictionary<string, string>>();
        protected Dictionary<int, Dictionary<string, string>> m_mapOfReversalMaps;
        protected Dictionary<string, List<string>> m_mapNormalFormToSourcePhrases = new Dictionary<string, List<string>>();
        protected char[] m_caDelimitersForward;
        protected char[] m_caDelimitersReverse;
        protected string      m_strKnowledgeBaseFileSpec;
        protected string m_strFallbackConverterName;
        protected string m_strProjectFileSpec;
        public bool              NormalizerDirectionForward = true;
        public NormalizeFlags    NormalizerFlags = NormalizeFlags.None;

        protected bool        m_bLegacy;
        protected bool        m_bReverseLookup;   // we have to keep track of the direction since it might be different than m_bForward
        protected bool m_bHasNamespace = true;
        internal MapOfMaps MapOfMaps;   // this is a special XElement implementation used to edit the KB

        private LanguageInfo _liSourceLang, _liTargetLang, _liNavigationLang;

        public const string cstrAdaptItXmlNamespace = "http://www.sil.org/computing/schemas/AdaptIt KB.xsd";
        public const string cstrAdaptItProjectFilename = "AI-ProjectConfiguration.aic";
        public const string cstrAdaptItPunctuationPairsNRSource = "PunctuationPairsSourceSet(stores space for an empty cell)\t";
        public const string cstrAdaptItPunctuationPairsNRTarget = "PunctuationPairsTargetSet(stores space for an empty cell)\t";
        public const string cstrAdaptItPunctuationPairsLegacy = "PunctuationPairs(stores space for an empty cell)\t";
        public const string CstrAdaptItPunct = "?.,;:\"'!()<>{}[]“”‘’…-";

        private const string CstrSourceFont = "SourceFont";
        private const string CstrTargetFont = "TargetFont";
        private const string CstrNavTextFont = "NavTextFont";

        private const string CstrRtlSource = "SourceIsRTL\t";
        private const string CstrRtlTarget = "TargetIsRTL\t";
        private const string CstrRtlNavText = "NavTextIsRTL\t";

        private const string CstrSourceLanguageName	= "SourceLanguageName\t";
        private const string CstrTargetLanguageName = "TargetLanguageName\t";

        private const string CstrFaceName = "FaceName\t";
        private const string cstrDefaultFont = "Arial Unicode MS";

        public static char[] CaSplitChars = new [] { '\r', '\n', '\t', ' ' };

        protected DirectableEncConverter theFallbackEc;

        public bool FileHasNamespace
        {
            get { return m_bHasNamespace; }
            set { m_bHasNamespace = value; }
        }

        #endregion Member Variable Definitions

        #region Initialization
        public AdaptItKBReader(string sProgId, string sImplementType)
            : base(sProgId, sImplementType)
        {
        }

        public override void Initialize(string converterName, string converterSpec, 
            ref string lhsEncodingID, ref string rhsEncodingID, ref ConvType conversionType, 
            ref Int32 processTypeFlags, Int32 codePageInput, Int32 codePageOutput, bool bAdding)
        {
            base.Initialize(converterName, converterSpec, ref lhsEncodingID, ref rhsEncodingID, ref conversionType, ref processTypeFlags, codePageInput, codePageOutput, bAdding);

            m_bLegacy = (EncConverter.NormalizeLhsConversionType(conversionType) == NormConversionType.eLegacy);

            ParseConverterSpec(converterSpec, out string knowledgebaseFileSpec, out string fallbackConverterName,
                               out bool normalizerDirectionForward, out NormalizeFlags normalizeFlags);

            m_strKnowledgeBaseFileSpec = knowledgebaseFileSpec;
            m_strFallbackConverterName = fallbackConverterName;
            NormalizerDirectionForward = normalizerDirectionForward;
            NormalizerFlags = normalizeFlags;

            string strProjectFolder = Path.GetDirectoryName(m_strKnowledgeBaseFileSpec);
            m_strProjectFileSpec = Path.Combine(strProjectFolder, cstrAdaptItProjectFilename);

            if (bAdding)
            {
                // if we're supposedly adding this one, then clobber our copy of its last modified 
                // (there was a problem with us instantiating lots of these things in a row and
                //  not detecting the change because the modified date was within a second of each 
                //  other)
                m_timeModifiedKB = m_timeModifiedProj = DateTime.MinValue;
            }
        }

        public static void ParseConverterSpec(string converterSpec, out string knowledgebaseFileSpec, out string fallbackConverterName,
                                       out bool normalizerDirectionForward, out NormalizeFlags normalizeFlags)
        {
            knowledgebaseFileSpec = fallbackConverterName = null;
            normalizerDirectionForward = true; // default is forward
            normalizeFlags = NormalizeFlags.None; // default is no flags

            // the converter spec has 1-4 pieces of information:
            //  a) the same as the original one -- the path to the KB file
            //  b) the name of the fallback converter to call for words not in the AI KB
            //  c & d) settings of direction and normalization form for the fallback converter
            string[] astrPaths = converterSpec.Split(new[] { ';' });
            if (astrPaths.Length >= 1)
                knowledgebaseFileSpec = astrPaths[0];

            if (astrPaths.Length >= 2)
                fallbackConverterName = astrPaths[1];

            if (astrPaths.Length >= 3)
                normalizerDirectionForward = !(astrPaths[2].ToLower() == "false");

            if (astrPaths.Length == 4)
                normalizeFlags = (NormalizeFlags)Enum.Parse(typeof(NormalizeFlags), astrPaths[3], true);
        }

        #endregion Initialization

        #region Misc helpers
        protected string XPathToMAP
        {
            get
            {
                if (FileHasNamespace)
                    return "/aikb:AdaptItKnowledgeBase/aikb:KB/aikb:MAP";
                return "//KB/MAP";  // use double-slash, because AI would like to keep the AdaptItKnowledgeBase element, but without the ns
            }
        }

        protected string XPathToTU
        {
            get
            {
                if (FileHasNamespace)
                    return "aikb:TU";
                return "TU";
            }
        }

        protected string XPathToRS
        {
            get
            {
                if (FileHasNamespace)
                    return "aikb:RS";
                return "RS";
            }
        }

         /* these are now deprecated since they don't support AI KB v2
        protected string XPathToKB
        {
            get
            {
                if (FileHasNamespace)
                    return "/aikb:AdaptItKnowledgeBase/aikb:KB";
                return "//KB";  // use double-slash, because AI would like to keep the AdaptItKnowledgeBase element, but without the ns
            }
        }

        protected string XPathToSpecificMAP(int nMapValue)
        {
            string str = String.Format("MAP[@mn=\"{0}\"]", nMapValue);
            if (FileHasNamespace)
                str = "aikb:" + str;
            return str;
        }

        protected string XPathToSpecificTU(string strSourceWord)
        {
            string str = String.Format("TU[@k=\"{0}\"]", strSourceWord);
            if (FileHasNamespace)
                str = "aikb:" + str;
            return str;
        }

        protected string XPathToSpecificRS(string strTargetWord)
        {
            string str = String.Format("RS[@a=\"{0}\"]", strTargetWord);
            if (FileHasNamespace)
                str = "aikb:" + str;
            return str;
        }
        */
        protected virtual bool Load(bool bMapOfMaps)
        {
            System.Diagnostics.Debug.Assert(!String.IsNullOrEmpty(m_strProjectFileSpec));
            System.Diagnostics.Debug.Assert(!String.IsNullOrEmpty(m_strKnowledgeBaseFileSpec));

            // see if the project file timestamp has changed (in which case, we should 
            //  reload the punctuation just in case it changed);
            DateTime timeModified = DateTime.Now; // don't care really, but have to initialize it.
            if( !DoesFileExist(m_strProjectFileSpec, ref timeModified) )
                EncConverters.ThrowError(ErrStatus.CantOpenReadMap, m_strProjectFileSpec);

            bool bSomethingChanged = false;

            // if we have "old" data
            if( timeModified > m_timeModifiedProj )
            {
                // get the punctuation out of the project file.
                string strProjectFileContents = null;
                using (StreamReader sr = File.OpenText(m_strProjectFileSpec)) 
                {
                    strProjectFileContents = sr.ReadToEnd();
                }

                if( m_bLegacy ) // legacy project file does it differently
                {
                    int nIndex = strProjectFileContents.IndexOf(cstrAdaptItPunctuationPairsLegacy) + cstrAdaptItPunctuationPairsLegacy.Length;
                    int nLength = strProjectFileContents.IndexOfAny(CaSplitChars, nIndex) - nIndex;
                    string strPunctuation = strProjectFileContents.Substring(nIndex, nLength);
                    InitializeDelimitersLegacy(strPunctuation, out m_caDelimitersForward, out m_caDelimitersReverse);
                }
                else    // NonRoman version
                {
                    _liSourceLang = new LanguageInfo(strProjectFileContents, cstrAdaptItPunctuationPairsNRSource, CstrSourceFont, CstrRtlSource, CstrSourceLanguageName);
                    m_caDelimitersForward = (_liSourceLang.Punctuation + " ").ToCharArray();
                    _liTargetLang = new LanguageInfo(strProjectFileContents, cstrAdaptItPunctuationPairsNRTarget, CstrTargetFont, CstrRtlTarget, CstrTargetLanguageName);
                    m_caDelimitersReverse = (_liTargetLang.Punctuation + " ").ToCharArray();
                    _liNavigationLang = new LanguageInfo(strProjectFileContents, null, CstrNavTextFont, CstrRtlNavText, null);
                }

                m_timeModifiedProj = timeModified;
                bSomethingChanged = true;
            }

            // next check on the knowledge base... make sure it's there and get the last time it was modified
            timeModified = DateTime.Now; // don't care really, but have to initialize it.
            if( !DoesFileExist(m_strKnowledgeBaseFileSpec, ref timeModified) )
                EncConverters.ThrowError(ErrStatus.CantOpenReadMap, m_strKnowledgeBaseFileSpec);

            // if we're using the new MapOfMaps class methods (like EditKb), then
            //  load *that* (otherwise, load the other)
            if (bMapOfMaps)
            {
                // if it has been modified or it's not already loaded...
                if ((MapOfMaps == null) || (timeModified > m_timeModifiedKbMapOfMaps))
                {
                    MapOfMaps = MapOfMaps.LoadXml(m_strKnowledgeBaseFileSpec);

                    // keep track of the modified date, so we can detect a new version to reload
                    m_timeModifiedKbMapOfMaps = timeModified;
                    bSomethingChanged = true;
                }
            }
            else
            {
                // if it has been modified or it's not already loaded...
                if (timeModified > m_timeModifiedKB)
                {
                    m_mapOfMaps.Clear();
                    m_mapOfReversalMaps = null;

                    // Since AdaptIt will make different records for two words which are canonically
                    //  equivalent, if we use the class object to read it in via ReadXml, that will throw
                    //  an exception in such a case. So see if using XmlDocument is any less restrictive
                    try
                    {
                        XmlDocument doc;
                        XPathNavigator navigator;
                        XmlNamespaceManager manager;
                        GetXmlDocument(out doc, out navigator, out manager);

                        XPathNodeIterator xpMapIterator = navigator.Select(XPathToMAP, manager);

                        List<string> astrTargetWords = new List<string>();
                        while (xpMapIterator.MoveNext())
                        {
                            // get the map number so we can make different maps for different size phrases
                            string strMapNum = xpMapIterator.Current.GetAttribute("mn", navigator.NamespaceURI);
                            int nMapNum = System.Convert.ToInt32(strMapNum, 10);
                            Dictionary<string, string> mapWords = new Dictionary<string, string>();
                            m_mapOfMaps.Add(nMapNum, mapWords);

                            XPathNodeIterator xpSourceWords = xpMapIterator.Current.Select(XPathToTU, manager);
                            while (xpSourceWords.MoveNext())
                            {
                                XPathNodeIterator xpTargetWords = xpSourceWords.Current.Select(XPathToRS, manager);

                                astrTargetWords.Clear();
                                while (xpTargetWords.MoveNext())
                                {
                                    string strTargetWord = xpTargetWords.Current.GetAttribute("a", navigator.NamespaceURI);
                                    astrTargetWords.Add(strTargetWord);
                                }

                                // if there are multiple target words for this form, then return it in Ample-like
                                //  %2%target1%target% format
                                string strTargetWordFull = null;
                                if (astrTargetWords.Count > 1)
                                {
                                    strTargetWordFull = String.Format("%{0}%", astrTargetWords.Count);
                                    foreach (string strTargetWord in astrTargetWords)
                                        strTargetWordFull += String.Format("{0}%", strTargetWord);
                                }
                                else if (astrTargetWords.Count == 1)
                                {
                                    strTargetWordFull = astrTargetWords[0];
                                    if (strTargetWordFull == "<Not In KB>")
                                        continue;   // skip this one so we *don't* get a match later on.
                                }

                                string strSourceWord = xpSourceWords.Current.GetAttribute("k", navigator.NamespaceURI);
                                System.Diagnostics.Debug.Assert(!mapWords.ContainsKey(strSourceWord), String.Format("The Knowledge Base has two different source records which are canonically equivalent! See if you can merge the two KB entries for word that look like, '{0}'", strSourceWord));
                                mapWords[strSourceWord] = strTargetWordFull;
                            }
                        }
                    }
                    catch (System.Data.DataException ex)
                    {
                        if (ex.Message == "A child row has multiple parents.")
                        {
                            // this happens when the knowledge base has invalid data in it (e.g. when there is two
                            //  canonically equivalent words in different records). This is technically a bug in 
                            //  AdaptIt.
                            throw new ApplicationException("The AdaptIt knowledge base has invalid data in it! Contact silconverters_support@sil.org", ex);
                        }

                        throw ex;
                    }
                    catch (Exception ex)
                    {
                        throw new ApplicationException(
                            String.Format(Properties.Resources.IDS_CantOpenAiKb,
                                Environment.NewLine, ex.Message));
                    }

                    // keep track of the modified date, so we can detect a new version to reload
                    m_timeModifiedKB = timeModified;
                    bSomethingChanged = true;
                }

                // next check on the fallback converter... if we have one...
                if (!String.IsNullOrEmpty(m_strFallbackConverterName) && (theFallbackEc == null))
                {
                    theFallbackEc = new DirectableEncConverter(m_strFallbackConverterName, NormalizerDirectionForward, NormalizerFlags);
                }
            }

            return bSomethingChanged;   // indicate whether the data was reinitialized or not
        }

        private string CallSafeNormalizedConvert(string strSourceWords)
        {
            try
            {
                if (theFallbackEc != null)
                    return theFallbackEc.Convert(strSourceWords);
            }
            catch
            {
            }
            return strSourceWords;
        }

        /*
        /// <summary>
        /// return the list of words/phrases that are similar to given source word.
        /// "similar" is defined as having the same normalized form as given in by
        /// the normalizing EncConverter whose path specification was passed in as
        /// the 2nd member of this EncConverter's ConverterIdentifier.
        /// </summary>
        /// <param name="strSourceWord"></param>
        /// <returns></returns>
        public List<string> GetSimilarWords(string strSourceWord)
        {
            if (theNormalizerEc == null)
                return null;    // not "Load"ed yet -- programmer error

            // just in case it hasn't been done yet, do the Load now.
            Load(false);

            // first normalize the given word
            string strNormalizedSourceWord = CallSafeNormalizedConvert(strSourceWord);
            List<string> lstOfSourcePhrasesWithTheSameNormalizedForm;
            if (m_mapNormalFormToSourcePhrases.TryGetValue(strNormalizedSourceWord,
                out lstOfSourcePhrasesWithTheSameNormalizedForm))
            {
                // if the KB has an identity lookup (which it shouldn't but can), then
                //  don't return that one.
                int nIndex = lstOfSourcePhrasesWithTheSameNormalizedForm.IndexOf(strSourceWord);
                if (nIndex != -1)
                {
                    lstOfSourcePhrasesWithTheSameNormalizedForm.RemoveAt(nIndex);

                    // if it now is empty, then just return null
                    if (lstOfSourcePhrasesWithTheSameNormalizedForm.Count == 0)
                        return null;
                }

                return lstOfSourcePhrasesWithTheSameNormalizedForm;
            }

            return null;
        }
        */

        private static string GetLanguageElement(string strProjectFileContents, string strLanguageType, 
            string strEntryName, string strDefaultValue)
        {
            int nIndex = strProjectFileContents.IndexOf(strLanguageType);
            if (nIndex != -1)
            {
                nIndex = strProjectFileContents.IndexOf(strEntryName, nIndex) + strEntryName.Length + 1;
                int nLength = strProjectFileContents.IndexOf('\n', nIndex) - nIndex - 1;
                return strProjectFileContents.Substring(nIndex, nLength);
            }
            return strDefaultValue;
        }

        protected void GetXmlDocument(out XmlDocument doc, out XPathNavigator navigator, out XmlNamespaceManager manager)
        {
            doc = new XmlDocument();
            doc.Load(m_strKnowledgeBaseFileSpec);
            navigator = doc.CreateNavigator();
            manager = new XmlNamespaceManager(navigator.NameTable);
            FileHasNamespace = (doc.InnerXml.IndexOf(cstrAdaptItXmlNamespace) != -1);
            if (FileHasNamespace)
                manager.AddNamespace("aikb", cstrAdaptItXmlNamespace);
        }

        // for a NonRoman AI Project, the punctuation is in adjacent rows e.g.:
        // PunctuationPairsSourceSet(stores space for an empty cell)	?.,;:"!()<>{}[]“”‘’
        // PunctuationPairsTargetSet(stores space for an empty cell)	?.,;:"!()<>{}[]“”‘’
        protected static char[] ReturnDelimiters(string s, int nIndex, int nLength)
        {
            string strPunctuation = s.Substring(nIndex, nLength);

            // put the delimiter chars into a char array (for use in a later 'Split' of the input string)
            char[] aChars = new char[strPunctuation.Length + 1];
            for (int i = 0; i < strPunctuation.Length; i++)
                aChars[i] = strPunctuation[i];
            aChars[strPunctuation.Length] = ' ';    // add a space so it does an implicit Trim()
            return aChars;
        }

        // for a Legacy AI Project, the punctuation is in adjacent characters e.g.:
        //  "??..,,;;::""!!(())<<>>[[]]{{}}||““””‘‘’’"
        protected void InitializeDelimitersLegacy(string strPunctuation, out char[] aPunctForward, out char[] aPunctReverse)
        {
            // initialize the output char arrays
            int nLen = strPunctuation.Length / 2;
            aPunctForward = new char[nLen + 1];
            aPunctReverse = new char[nLen + 1];

            // put the delimiter chars into a char array (for use in a later 'Split' of the input string)
            int i = 0, j = 0;
            while (i < nLen)
            {
                aPunctForward[i] = strPunctuation[j++];
                aPunctReverse[i++] = strPunctuation[j++];
            }

            aPunctForward[nLen] = aPunctReverse[nLen] = ' ';
        }

        private void GetAmbiguities(string str, out List<string> astrAmbiguities)
        {
            astrAmbiguities = new List<string>();
            int nIndex;
            if ((nIndex = str.IndexOf('%')) == 0)
            {
                nIndex = str.IndexOf('%', nIndex + 1);
                if (nIndex != -1)
                {
                    str = str.Substring(nIndex + 1);
                    string[] astrWords = str.Split(new[] { '%' }, StringSplitOptions.RemoveEmptyEntries);
                    astrAmbiguities.AddRange(astrWords);
                }
                else
                    astrAmbiguities.Add(str);
            }
            else
                astrAmbiguities.Add(str);
        }

        private ViewSourceFormsForm _dlgSourceFormsForm;
        /// <summary>
        /// Edit the knowledgebase and optionally return the selected source word
        /// </summary>
        /// <returns>the source word selected in the edit dialog</returns>
        public string EditKnowledgeBase(string strFilter, bool useShow = false)
        {
            if (!String.IsNullOrEmpty(strFilter))
                strFilter = strFilter.Trim(m_caDelimitersForward ?? CaSplitChars);

            if (Load(true) || useShow || (_dlgSourceFormsForm == null))
                _dlgSourceFormsForm = new ViewSourceFormsForm(MapOfMaps, _liSourceLang,
                                                              _liTargetLang,
                                                              m_caDelimitersForward ?? CaSplitChars,
                                                              m_caDelimitersReverse ?? CaSplitChars)
                                          {
                                              Parent = this
                                          };

            _dlgSourceFormsForm.SelectedWord = strFilter;

            if (useShow)
            {
                _dlgSourceFormsForm.Show();
            }
            else if (_dlgSourceFormsForm.ShowDialog() == DialogResult.OK)
            {
                return _dlgSourceFormsForm.SelectedWord;
            }
            return null;
        }

        internal void SaveMapOfMaps(MapOfMaps mapOfMaps)
        {
            mapOfMaps.SaveFile(m_strKnowledgeBaseFileSpec);

            // let's at least avoid reloading it if/since we're writing it, 
            //  so capture it's new timestamp as though we had read it in
            if (!DoesFileExist(m_strKnowledgeBaseFileSpec, ref m_timeModifiedKbMapOfMaps))
                EncConverters.ThrowError(ErrStatus.CantOpenReadMap, m_strKnowledgeBaseFileSpec);
        }

        public void MergeKb(ProgressBar progressBar)
        {
            Load(true); // load MapOfMaps if needed

            // create an open file dialog so the user can browse for the file to merge into this one
            OpenFileDialog dlgOpenFiles = new OpenFileDialog 
                                                {
                                                    Filter = "Adapt It Knowledge Base XML files (*.xml)|*.xml", 
                                                    Title = "Select AdaptIt KB (XML) files you want to merge (Click Cancel when finished)" 
                                                };

            if ((dlgOpenFiles.ShowDialog() != DialogResult.OK) || !File.Exists(dlgOpenFiles.FileName))
                return;

            var mapOtherKb = MapOfMaps.LoadXml(dlgOpenFiles.FileName);

            // iterate thru the maps first
            var mapMaps = mapOtherKb.MapOfSizeToMap;
            foreach (var kvpMap in mapMaps)
            {
                // see if that same map is in 'this'
                MapOfSourceWordElements mapOfSourceWordElementsThis;
                if (!MapOfMaps.TryGetValue(kvpMap.Key, out mapOfSourceWordElementsThis))
                {
                    // means that the map (1-10) isn't in 'this', so just add it wholesale
                    MapOfMaps.Add(kvpMap.Value);
                    continue;
                }

                // the same map (1-10) exists in 'this', so now iterate over the source words in 'other'
                //  to see if they are also in 'this'
                var sourceWordsOther = kvpMap.Value.MapOfSourceWords;
                if (progressBar != null)
                {
                    progressBar.Value = 0;
                    progressBar.Maximum = sourceWordsOther.Count;
                }

                var sourceWordElementsThis = mapOfSourceWordElementsThis.MapOfSourceWords;
                foreach (var kvpSourceWordOther in sourceWordsOther)
                {
                    if (progressBar != null)
                        progressBar.Value++;

                    System.Diagnostics.Debug.WriteLine("Checking for source word: " + kvpSourceWordOther.Key);
                    if (!sourceWordElementsThis.ContainsKey(kvpSourceWordOther.Key))
                    {
                        // means that 'this' didn't have the source word element from 'other', so add it wholesale
                        mapOfSourceWordElementsThis.Add(kvpSourceWordOther.Value);
                        continue;
                    }

                    // the same source word element exists in 'this', so now iterate over the target words in 'other'
                    //  to see if they are also in 'this'
                    var sourceWordElementThis = sourceWordElementsThis[kvpSourceWordOther.Key];
                    var targetWordsThis = sourceWordElementThis.MapOfTargetWords;
                    foreach (var kvpTargetWordElement in kvpSourceWordOther.Value.MapOfTargetWords)
                    {
                        if (!targetWordsThis.ContainsKey(kvpTargetWordElement.Key))
                        {
                            // means that 'this' didn't have the 'other' target word... add it wholesale
                            sourceWordElementThis.Add(kvpTargetWordElement.Value);
                            continue;
                        }
                    }
                }
            }

            SaveMapOfMaps(MapOfMaps);
        }

        public void KbReversal(ProgressBar progressBar)
        {
            Load(true); // load MapOfMaps if needed

            var strReversalFileName = AdaptItLookupFileSpec(null, MapOfMaps.TgtLangName, MapOfMaps.SrcLangName);
            var mapOfMapsReverse = File.Exists(strReversalFileName)
                                       ? MapOfMaps.LoadXml(strReversalFileName)
                                       : new MapOfMaps(MapOfMaps.TgtLangName, MapOfMaps.SrcLangName);

            // it's sort of complicated to iterate the data, because the MapOfMaps class is geared
            //  around *not* unpacking everything into the object
            // first, get all the source words in the file
            var lstSourceWords = MapOfMaps.ListOfAllSourceWordForms;
            if (progressBar != null)
            {
                progressBar.Value = 0;
                progressBar.Maximum = lstSourceWords.Count;
            }

            foreach (var sourceWordForm in lstSourceWords)
            {
                if (progressBar != null)
                    progressBar.Value++;

                // get the map that this source word is in
                MapOfSourceWordElements mapOfSourceWordElements;
                if (!MapOfMaps.TryGetValue(sourceWordForm, out mapOfSourceWordElements))
                    continue;

                // get the associated source word element
                SourceWordElement sourceWordElement;
                if (!mapOfSourceWordElements.TryGetValue(sourceWordForm, out sourceWordElement))
                    continue;

                // iterate through all the target word forms in that
                foreach (var targetWordElement in sourceWordElement.TargetWords())
                    mapOfMapsReverse.AddCouplet(targetWordElement.TargetWord, sourceWordElement.SourceWord);
            }

            var strProjectFolderName = AdaptItProjectFolderName(null, MapOfMaps.SrcLangName, MapOfMaps.TgtLangName);
            LanguageInfo liSource, liTarget, liNavigation;
            ReadLanguageInfo(strProjectFolderName, MapOfMaps.SrcLangName, MapOfMaps.TgtLangName,
                out liSource, out liTarget, out liNavigation);

            // create the reversal project (swap the source and target)
            WriteAdaptItProjectFiles(liTarget, liSource, liNavigation);

            mapOfMapsReverse.SaveFile(strReversalFileName);
        }

        const char CchNoNoChar = '\"';
        /// <summary>
        /// This version of AddEntryPair allows an option parameter to indicate whether
        /// you want the KB saved after the pair is added (primarily in case you want
        /// to *not* save it through an iteration cycle). If false, then you should 
        /// then call AddEntryPairSave() when finished to save it.
        /// Also, this flavor will save the KB sorted
        /// </summary>
        /// <param name="strSourceWord"></param>
        /// <param name="strTargetWord"></param>
        /// <param name="bSave"></param>
        public void AddEntryPair(string strSourceWord, string strTargetWord, bool bSave)
        {
            // trim up the input words in case the user didn't
            strSourceWord = strSourceWord.Trim(m_caDelimitersForward ?? CaSplitChars);
            if (String.IsNullOrEmpty(strSourceWord))
                throw new ApplicationException(Properties.Resources.IDS_CantHaveEmptySourceWord);
            if (strSourceWord.IndexOf(CchNoNoChar) != -1)
                throw new ApplicationException(Properties.Resources.IDS_CantHaveEmptySouIDS_CantHaveDoubleQuotes);

            strTargetWord = strTargetWord.Trim(m_caDelimitersReverse ?? CaSplitChars);
            if (String.IsNullOrEmpty(strTargetWord))
                throw new ApplicationException(Properties.Resources.IDS_CantHaveEmptyTargetWord);
            if (strTargetWord.IndexOf(CchNoNoChar) != -1)
                throw new ApplicationException(Properties.Resources.IDS_CantHaveEmptySouIDS_CantHaveDoubleQuotes);

            Load(true); // load MapOfMaps if needed

            MapOfMaps.AddCouplet(strSourceWord, strTargetWord);

            if (bSave)
                SaveMapOfMaps(MapOfMaps);
        }

        public void AddEntryPairSave()
        {
            if (MapOfMaps != null)
                SaveMapOfMaps(MapOfMaps);
        }

        public bool EditTargetWords(string strSourceWord)
        {
            if (String.IsNullOrEmpty(strSourceWord))
                throw new ApplicationException(Properties.Resources.IDS_CantHaveEmptySourceWord);

            // trim up the input words in case the user didn't
            strSourceWord = strSourceWord.Trim(m_caDelimitersForward ?? CaSplitChars);
            if (String.IsNullOrEmpty(strSourceWord))
                throw new ApplicationException(Properties.Resources.IDS_CantHaveEmptySourceWord);

#if !NotUsingNewClass
            Load(true); // load MapOfMaps if needed
            MapOfSourceWordElements mapOfSourceWordElements;
            if (MapOfMaps.TryGetValue(strSourceWord, out mapOfSourceWordElements))
            {
                var dlg = new ModifyTargetWordsForm(strSourceWord, mapOfSourceWordElements,
                    _liTargetLang, m_caDelimitersReverse ?? CaSplitChars);
                if (dlg.ShowDialog() == DialogResult.OK)
                {
                    SaveMapOfMaps(MapOfMaps);
                    return true;
                }
            }
#else
            // first see if this pair is already there
            int nMapValue;
            List<string> astrTargetWords = GetTargetWords(strSourceWord, out nMapValue);

            var dlg = new ModifyTargetWordsForm(strSourceWord, astrTargetWords, m_strTargetWordFontName);
            if (dlg.ShowDialog() == DialogResult.OK)
            {
                try
                {
                    XmlDocument doc;
                    XPathNavigator navigator;
                    XmlNamespaceManager manager;
                    GetXmlDocument(out doc, out navigator, out manager);

                    if (doc.DocumentElement != null)
                    {
                        XmlNode nodeKbNode = doc.DocumentElement.SelectSingleNode(XPathToKB, manager);
                        if (nodeKbNode == null)
                        {
                            doc.CreateElement(XPathToKB);   // no KB element, so create one
                            nodeKbNode = doc.DocumentElement.SelectSingleNode(XPathToKB, manager);
                        }

                        // see if the proper map entry is present (so we can add it, if not)
                        string strMapSelect = XPathToSpecificMAP(nMapValue);
                        XmlNode nodeMapEntry = nodeKbNode.SelectSingleNode(strMapSelect, manager);
                        if (nodeMapEntry == null)
                        {
                            // if not, then add it.
                            // xpathnavs are easier to use to add child elements
                            XPathNavigator xpnMap = nodeKbNode.CreateNavigator();
                            xpnMap.AppendChild(String.Format("<MAP mn=\"{0}\"/>", nMapValue));

                            // now try it again
                            nodeMapEntry = nodeKbNode.SelectSingleNode(strMapSelect, manager);
                        }

                        // see if the source word exists (so we can delete what's there now)
                        string strSourceWordSelect = XPathToSpecificTU(strSourceWord);
                        XmlNode nodeSourceWordEntry = nodeMapEntry.SelectSingleNode(strSourceWordSelect, manager);

                        // if it exists, then empty it
                        if (nodeSourceWordEntry != null)
                        {
                            // get rid of the existing children
                            while (nodeSourceWordEntry.ChildNodes.Count > 0)
                                nodeSourceWordEntry.RemoveChild(nodeSourceWordEntry.ChildNodes.Item(0));
                        }
                        else
                        {
                            // otherwise, add it
                            XPathNavigator xpnSourceWord = nodeMapEntry.CreateNavigator();
                            xpnSourceWord.AppendChild(String.Format("<TU f=\"0\" k=\"{0}\"/>", strSourceWord));

                            // now try it again
                            nodeSourceWordEntry = nodeMapEntry.SelectSingleNode(strSourceWordSelect, manager);
                        }

                        XPathNavigator xpnTargetWord = nodeSourceWordEntry.CreateNavigator();
                        List<string> lstNewTargetForms = dlg.NewTargetForms;
                        if (lstNewTargetForms.Count == 0)
                            xpnTargetWord.DeleteSelf();
                        else
                        {
                            foreach (string strNewTargetForm in dlg.NewTargetForms)
                                xpnTargetWord.AppendChild(String.Format("<RS n=\"1\" a=\"{0}\"/>", strNewTargetForm));
                        }

                        File.Copy(m_strKnowledgeBaseFileSpec, m_strKnowledgeBaseFileSpec + ".bak", true);

                        XmlTextWriter writer = new XmlTextWriter(m_strKnowledgeBaseFileSpec, Encoding.UTF8);
                        writer.Formatting = Formatting.Indented;
                        doc.Save(writer);
                        writer.Close();
                    }
                }
                catch (System.Data.DataException ex)
                {
                    if (ex.Message == "A child row has multiple parents.")
                    {
                        // this happens when the knowledge base has invalid data in it (e.g. when there is two
                        //  canonically equivalent words in different records). This is technically a bug in 
                        //  AdaptIt.
                        throw new ApplicationException("The AdaptIt knowledge base has invalid data in it! Contact silconverters_support@sil.org", ex);
                    }

                    throw ex;
                }


                return true;
            }

#endif
            return false;
        }

        internal List<string> GetTargetWords(string strSourceWord, out int nMapValue)
        {
            string strTargetWordsInMap;
            Dictionary<string, string> mapLookup;
            List<string> astrTargetWords;
            if (!TryGetTargetForm(strSourceWord, out strTargetWordsInMap, out nMapValue, out mapLookup))
            {
                // if there is no source word, then we'll just offer that as the new target
                astrTargetWords = new List<string> { strSourceWord };
            }
            else
            {
                GetAmbiguities(strTargetWordsInMap, out astrTargetWords);
            }
            return astrTargetWords;
        }

        protected bool TryGetTargetForm(string strSourceWord, out string strTargetWordsInMap,
            out int nMapValue, out Dictionary<string, string> mapLookup)
        {
            // first get the map for the number of words in the source string (e.g. "ke picche" would be map=2)
            nMapValue = strSourceWord.Split(CaSplitChars, StringSplitOptions.RemoveEmptyEntries).Length;
            if (nMapValue > 10)
                throw new ApplicationException("Cannot have a source phrase with more than 10 words!");

            // just in case it hasn't been done yet, do the Load now.
            Load(false);

            if (!m_mapOfMaps.TryGetValue(nMapValue, out mapLookup))
                mapLookup = new Dictionary<string, string>();

            // first see if this pair is already there
            return mapLookup.TryGetValue(strSourceWord, out strTargetWordsInMap);
        }

        #endregion Misc helpers

        #region Abstract Base Class Overrides
        protected override void PreConvert
            (
            EncodingForm        eInEncodingForm,
            ref EncodingForm    eInFormEngine,
            EncodingForm        eOutEncodingForm,
            ref EncodingForm    eOutFormEngine,
            ref NormalizeFlags  eNormalizeOutput,
            bool                bForward
            ) 
        {
            // let the base class do it's thing first
            base.PreConvert( eInEncodingForm, ref eInFormEngine,
                eOutEncodingForm, ref eOutFormEngine,
                ref eNormalizeOutput, bForward);
        
            // this converter only deals with 'String' flavors, so if it's 
            //  Unicode_to(_from)_Unicode, then we expect UTF-16 and if it's 
            //  Legacy_to(_from)_Legacy, then we expect LegacyString
            if( m_bLegacy )
                eInFormEngine = eOutFormEngine = EncodingForm.LegacyString;
            else
                eInFormEngine = eOutFormEngine = EncodingForm.UTF16;

            // the bForward that comes here might be different from the IEncConverter->DirectionForward
            //  (if it came in from a call to ConvertEx), so use *this* value to determine the direction
            //  for the forthcoming conversion (DoConvert).
            m_bReverseLookup = !bForward;

            // check to see if the file(s) need to be (re-)loaded at this point.
            Load(false);
        }

        protected override string GetConfigTypeName
        {
            get { return strRegValueForConfigProgId; }
        }
        #endregion Abstract Base Class Overrides

        public static string GetAttributeValue(XElement elem, string strAttributeLabel, string strDefaultValue)
        {
            var attr = elem.Attribute(strAttributeLabel);
            return (attr != null) ? attr.Value : strDefaultValue;
        }

        public static int GetAttributeValue(XElement elem, string strAttributeLabel, int nDefaultValue)
        {
            var attr = elem.Attribute(strAttributeLabel);
            if (attr != null)
            {
                int nValue = System.Convert.ToInt32(attr.Value);
                return nValue;
            }
            return nDefaultValue;
        }

        protected static string AdaptItGlossingLookupFileSpec(string strProjectFolderName,
            string strSourceLangName, string strTargetLangName)
        {
            return Path.Combine(AdaptItProjectFolder(strProjectFolderName, strSourceLangName, strTargetLangName),
                                AdaptItAutoConfigDialog.CstrAdaptItGlossingKb);
        }

        public static string AdaptItProjectFolderName(string strProjectFolderName,
                                                      string strSourceLangName, 
                                                      string strTargetLangName)
        {
            return String.IsNullOrEmpty(strProjectFolderName)
                       ? String.Format(@"{0} to {1} adaptations", strSourceLangName, strTargetLangName)
                       : strProjectFolderName;
        }

        protected static string AdaptationFileName(string strProjectFolderName,
            string strSourceLangName, string strTargetLangName)
        {
            return String.Format(@"{0}.xml",
                                 AdaptItProjectFolderName(strProjectFolderName, strSourceLangName, strTargetLangName));
        }

        public static string AdaptItLookupFileSpec(string strProjectFolderName,
            string strSourceLangName, string strTargetLangName)
        {
            return Path.Combine(AdaptItProjectFolder(strProjectFolderName, strSourceLangName, strTargetLangName),
                                AdaptationFileName(strProjectFolderName, strSourceLangName, strTargetLangName));
        }

        protected static string AdaptItProjectFileSpec(string strProjectFolderName,
            string strSourceLangName, string strTargetLangName)
        {
            return Path.Combine(AdaptItProjectFolder(strProjectFolderName, strSourceLangName, strTargetLangName),
                                "AI-ProjectConfiguration.aic");
        }

        /// <summary>
        /// returns something like: <My Documents>\Adapt It Unicode Work
        /// which is the root folder in the user's my documents folder for all adapt it projects
        /// </summary>
        public static string AdaptItWorkFolder
        {
            get
            {
                return Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments),
                                    AdaptItAutoConfigDialog.CstrAdaptItWorkingDirUnicode);
            }
        }

        /// <summary>
        /// returns something like: ...My Documents\Adapt It Unicode Work\Bundelkhandi to English adaptations
        /// which is the project folder for the Adapt It project
        /// </summary>
        /// <param name="strSourceLangName"></param>
        /// <param name="strTargetLangName"></param>
        /// <returns></returns>
        public static string AdaptItProjectFolder(string strProjectFolderName,
            string strSourceLangName, string strTargetLangName)
        {
            return Path.Combine(AdaptItWorkFolder,
                                AdaptItProjectFolderName(strProjectFolderName, strSourceLangName, strTargetLangName));
        }

        /// <summary>
        /// returns something like, <My Documents>\Adapt It Unicode Work\Bundelkhandi to English adaptations\Adaptations
        /// which contains the adaptations done in Adapt It (only used if the user has called to AI to
        /// do glossing; not in the OSE glossing tool)
        /// </summary>
        /// <param name="strSourceLangName"></param>
        /// <param name="strTargetLangName"></param>
        /// <returns></returns>
        public static string AdaptItProjectAdaptationsFolder(string strProjectFolderName,
            string strSourceLangName, string strTargetLangName)
        {
            return Path.Combine(AdaptItProjectFolder(strProjectFolderName, strSourceLangName, strTargetLangName),
                                "Adaptations");
        }

        /// <summary>
        /// returns something like: "Lookup in {0} to {1} adaptations", which is the EncConverter friendly name for the project
        /// </summary>
        /// <param name="strSourceLangName"></param>
        /// <param name="strTargetLangName"></param>
        /// <returns></returns>
        public static string AdaptItLookupConverterName(string strSourceLangName, string strTargetLangName)
        {
            return String.Format(@"Lookup in {0} to {1} adaptations",
                strSourceLangName, strTargetLangName);
        }

        public class LanguageInfo
        {
            public LanguageInfo()
            {
                Punctuation = CstrAdaptItPunct;
                RightToLeft = RightToLeft.No;
                FontName = cstrDefaultFont;
            }

            public LanguageInfo(string strProjectFileContents, string strPunctuationPairs,
                                string strFontName, string strRtl, string strLanguageName)
            {
                Punctuation = (!String.IsNullOrEmpty(strPunctuationPairs))
                                      ? GetProjectFileElement(strProjectFileContents, strPunctuationPairs)
                                      : null;
                FontName = GetLanguageElement(strProjectFileContents, strFontName, CstrFaceName, cstrDefaultFont);
                RightToLeft = (GetProjectFileElement(strProjectFileContents, strRtl) == "1")
                                  ? RightToLeft.Yes
                                  : RightToLeft.No;
                LangName = (!String.IsNullOrEmpty(strLanguageName))
                               ? GetProjectFileElement(strProjectFileContents, strLanguageName)
                               : null;
            }

            private static string GetProjectFileElement(string strProjectFileContents, string strElementName)
            {
                int nIndex = strProjectFileContents.IndexOf(strElementName, StringComparison.Ordinal);
                if (nIndex != -1)
                {
                    nIndex += strElementName.Length;
                    int nLength = strProjectFileContents.IndexOf('\n', nIndex) - nIndex - 1;
                    return strProjectFileContents.Substring(nIndex, nLength);
                }
                return null;
            }

            public string LangName { get; set; }
            public string Punctuation { get; set; }
            public Font FontToUse
            {
                get
                {
                    return new Font(FontName, 12);
                }
            }

            public string FontName { get; set; }
            public RightToLeft RightToLeft { get; set; }
        }

        private static void ReadLanguageInfo(string strProjectFolderName, string strLangNameSource, string strLangNameTarget,
            out LanguageInfo liSourceLang, out LanguageInfo liTargetLang, out LanguageInfo liNavigationLang)
        {
            // read the Project file
            var strProjectFilespec = AdaptItProjectFileSpec(strProjectFolderName,
                                                            strLangNameSource,
                                                            strLangNameTarget);
            System.Diagnostics.Debug.Assert(File.Exists(strProjectFilespec));

            // get the punctuation out of the project file.
            string strProjectFileContents = null;
            using (StreamReader sr = File.OpenText(strProjectFilespec))
            {
                strProjectFileContents = sr.ReadToEnd();
            }

            liSourceLang = new LanguageInfo(strProjectFileContents, cstrAdaptItPunctuationPairsNRSource, CstrSourceFont, CstrRtlSource, CstrSourceLanguageName);
            liTargetLang = new LanguageInfo(strProjectFileContents, cstrAdaptItPunctuationPairsNRTarget, CstrTargetFont, CstrRtlTarget, CstrTargetLanguageName);
            liNavigationLang = new LanguageInfo(strProjectFileContents, null, CstrNavTextFont, CstrRtlNavText, null);
        }

        public static void WriteAdaptItProjectFiles(LanguageInfo liSourceLang,
                                                    LanguageInfo liTargetLang,
                                                    LanguageInfo liNavigation)
        {
            var strProjectFolderName = AdaptItProjectFolderName(null, liSourceLang.LangName, liTargetLang.LangName);

            // create folders...
            EnsureAiFoldersExist(strProjectFolderName, liSourceLang.LangName, liTargetLang.LangName);

            // create Project file
            var strProjectFilespec = AdaptItProjectFileSpec(strProjectFolderName,
                                                            liSourceLang.LangName,
                                                            liTargetLang.LangName);
            if (!File.Exists(strProjectFilespec))
            {
                string strFormat = Properties.Settings.Default.DefaultAIProjectFile;
                string strProjectFileContents = String.Format(strFormat,
                    liSourceLang.FontName,
                    liTargetLang.FontName,
                    liNavigation.FontName,
                    liSourceLang.LangName,
                    liTargetLang.LangName,
                    Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments),
                    liSourceLang.Punctuation,
                    liTargetLang.Punctuation,
                    (liSourceLang.RightToLeft == RightToLeft.Yes) ? "1" : "0",
                    (liTargetLang.RightToLeft == RightToLeft.Yes) ? "1" : "0");
                File.WriteAllText(strProjectFilespec, strProjectFileContents);
            }

            // create main KB
            WriteKbFile(AdaptItLookupFileSpec(strProjectFolderName, liSourceLang.LangName, liTargetLang.LangName),
                        liSourceLang,
                        liTargetLang);

            WriteKbFile(AdaptItGlossingLookupFileSpec(strProjectFolderName, liSourceLang.LangName, liTargetLang.LangName),
                        liSourceLang, 
                        liTargetLang);
        }

        private static void WriteKbFile(string strFileSpec, LanguageInfo liSourceLang, LanguageInfo liTargetLang)
        {
            if (!File.Exists(strFileSpec))
            {
                string strFormat = Properties.Settings.Default.DefaultAIKBFile;
                string strKbContents = String.Format(strFormat, liSourceLang.LangName, liTargetLang.LangName);
                File.WriteAllText(strFileSpec, strKbContents);
            }
        }

        public static void EnsureAiFoldersExist(string strProjectFolderName, string strSourceLang, string strTargetLang)
        {
            var strAdaptationsFolder = AdaptItProjectAdaptationsFolder(strProjectFolderName,
                                                                       strSourceLang,
                                                                       strTargetLang);
            if (!Directory.Exists(strAdaptationsFolder))
                Directory.CreateDirectory(strAdaptationsFolder);
        }
    }
}
