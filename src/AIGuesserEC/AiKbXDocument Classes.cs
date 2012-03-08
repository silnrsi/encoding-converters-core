using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Windows.Forms;
using System.Xml;
using System.Xml.Linq;
using System.Xml.XPath;
using System.Xml.Xsl;
using ECInterfaces;

namespace SilEncConverters40
{
    [ClassInterface(ClassInterfaceType.None)]
    internal class TargetWordElement
    {
        public const string CstrElementNameTargetWord = "RS";
        public const string CstrAttributeNameNumberOfOccurrences = "n";
        public const string CstrAttributeNameTargetWord = "a";
        public const string CstrAttributeNameDeleteFlag = "df";
        public const string CstrAttributeNameCreationDateTime = "cDT";
        public const string CstrAttributeNameModifiedDateTime = "mDT";
        public const string CstrAttributeNameDeletedDateTime = "dDT";
        public const string CstrAttributeNameUserComputer = "wC";

        private readonly XElement _elem;
        public TargetWordElement(XElement elem)
        {
            _elem = elem;
        }

        public string TargetWord 
        {
            get { return AdaptItKBReader.GetAttributeValue(Xml, CstrAttributeNameTargetWord, ""); }
        }

        public void TrimTargetWord(char[] achTrim)
        {
            var strTargetWord = TargetWord;
            Xml.SetAttributeValue(CstrAttributeNameTargetWord, strTargetWord.Trim(achTrim));
        }

        /// <summary>
        /// create a fragment of XML for the target word form (ai: "RS" element)
        /// for simplification purposes, we only care about the target word form
        /// and all other attribute values will be created as default values or
        /// otherwise ignored (it causes too much grief for Chorus to merge when
        /// two people have modified it)
        /// </summary>
        /// <param name="strTargetWordForm">e.g. "ασδγδ"</param>
        /// <param name="bIsKbV2">indicates whether to create kbVersion 2 (from AI v6 ff) or the earlier format</param>
        /// <returns>the XElement fragment created</returns>
        public static TargetWordElement CreateNewTargetWordElement(string strTargetWordForm, bool bIsKbV2)
        {
            /*
             * in AI v5.2.3 and previous
                <RS n="1" a="ασδγδ" />
             * in AI v6.0.0:
                <RS n="1" a="ασδγδ" df="1"
                   cDT="2010-11-29T01:28:21Z" wC="Bruce:BEWLAT"
                   dDT="2010-11-29T01:28:22Z"/>
            */
            if (strTargetWordForm == null)
                strTargetWordForm = "";

            var xml = new XElement(CstrElementNameTargetWord,
                                   new XAttribute(CstrAttributeNameNumberOfOccurrences, "1"),
                                   new XAttribute(CstrAttributeNameTargetWord, strTargetWordForm));

            
            if (bIsKbV2)
            {
                xml.Add(new XAttribute(CstrAttributeNameDeleteFlag, "0"),      // means 'false' (don't ask)
                        new XAttribute(CstrAttributeNameCreationDateTime, DateTimeStamp),
                        new XAttribute(CstrAttributeNameUserComputer, UserComputer));
            }

            return new TargetWordElement(xml);
        }

        private static string DateTimeStamp
        {
            get
            {
                // AI 6.0 uses: "%Y-%m-%dT%H:%M:%SZ" (e.g. "2011-10-12T19:35:10Z")
                // so for .Net, that would be: 'u', but with the space being replaced by
                //  a 'T' (I don't know why... according to http://www.csharp-examples.net/string-format-datetime/
                //  you get the 'T' when you ask for SortableDateTime, which is "s" format
                //  and you get the final 'Z', without the 'T' when it's UniversalSortableDateTime)
                return DateTime.Now.ToString("u").Replace(' ', 'T');
            }
        }

        private static string UserComputer
        {
            get
            {
                var user = System.Security.Principal.WindowsIdentity.GetCurrent();
                if (user == null)
                    return "<Username>:<Computername> unknown";
                
                // in .Net user.Name = <ComputerName>\\<Username>, but AI wants
                //  it to be: <Username>:<COMPUTERNAME>
                const string cstrUserNameDelimiter = "\\";
                var str = user.Name;
                if (str.Contains(cstrUserNameDelimiter))
                {
                    var astr = str.Split(new[] {cstrUserNameDelimiter}, StringSplitOptions.RemoveEmptyEntries);
                    Debug.Assert(astr.Length == 2);
                    if (astr.Length >= 2) 
                        return astr[1] + ':' + astr[0].ToUpper();
                }

                return str;
            }
        }

        public XElement Xml
        {
            get { return _elem; }
        }

        public void ChangeTargetWordForm(string strNewValue)
        {
            // if there is no value currently, then also update the modified date
            var strTargetWord = TargetWord;
            Xml.SetAttributeValue(CstrAttributeNameTargetWord, strNewValue);
            if (!String.IsNullOrEmpty(strTargetWord))
                Xml.SetAttributeValue(CstrAttributeNameModifiedDateTime, DateTimeStamp);
        }
    }

    [ClassInterface(ClassInterfaceType.None)]
    internal class SourceWordElement
    {
        public const string CstrElementNameSourceWord = "TU";
        public const string CstrAttributeNameForce = "f";
        public const string CstrAttributeNameSourceWord = "k";

        public MapOfSourceWordElements ParentMap;
        private readonly XElement _elem;

        public SourceWordElement(XElement elem, MapOfSourceWordElements parentMap)
        {
            _elem = elem;
            ParentMap = parentMap;
        }

        public string SourceWord
        {
            get { return AdaptItKBReader.GetAttributeValue(Xml, CstrAttributeNameSourceWord, ""); }
        }

        public static SourceWordElement CreateNewSourceWordElement(string strSourceWordForm, MapOfSourceWordElements mapOfSourceWordElements)
        {
            /*
              <TU f="0" k="Δ">
                <RS n="1" a="ασδγδ" />
              </TU>
            */
            var xml = new XElement(CstrElementNameSourceWord,
                                   new XAttribute(CstrAttributeNameForce, "0"), // means 'false' (don't ask)
                                   new XAttribute(CstrAttributeNameSourceWord, strSourceWordForm));

            return new SourceWordElement(xml, mapOfSourceWordElements);
        }

        /// <summary>
        /// This property can be used to get a new TargetWordElement associated with
        /// this Source word element
        /// </summary>
        [DebuggerBrowsableAttribute(DebuggerBrowsableState.Never)]  // so we don't add in the debugger
        public TargetWordElement GetNewTargetWord
        {
            get
            {
                var targetWordElement = TargetWordElement.CreateNewTargetWordElement(null,
                                                                                     ParentMap.ParentMapOfMaps.IsKbV2);
                Xml.Add(targetWordElement.Xml);
                return targetWordElement;
            }
        }

        public XElement Xml
        {
            get { return _elem; }
        }

        /// <summary>
        /// This method is used to (possibly) add a new word form (e.g. via AddNewPair)
        /// But if it already exists, then it just bumps the count.
        /// </summary>
        /// <param name="strRhs">the target word form</param>
        public void AddWord(string strRhs)
        {
            Debug.Assert(!String.IsNullOrEmpty(strRhs));
            // if there's already one...
            if (Descendants().Any(twe => twe.TargetWord == strRhs))
            {
                // just return (I don't care about the "NumberOfOccurrences" that I should
                //  bump the count and it just seems to make the ChorusMerger go into an
                //  infinite processing loop when they're different
                return;
            }

            // otherwise, add this to the doc
            var targetWordElement = TargetWordElement.CreateNewTargetWordElement(strRhs,
                                                                                 ParentMap.ParentMapOfMaps.IsKbV2);
            Xml.Add(targetWordElement.Xml);
        }

        public IEnumerable<TargetWordElement> Descendants()
        {
            return Xml.Descendants().Select(descendent => 
                new TargetWordElement(descendent)).ToList();
        }

        private XElement GetDecendent(string strTargetWord)
        {
            var strXPath = String.Format("{0}[@{1} = \"{2}\"]",
                                         TargetWordElement.CstrElementNameTargetWord,
                                         TargetWordElement.CstrAttributeNameTargetWord,
                                         strTargetWord);
            return Xml.XPathSelectElement(strXPath);
        }

        public void Remove(string strTargetWord)
        {
            var elem = GetDecendent(strTargetWord);
            if (elem != null)
                elem.Remove();
        }

        public void ReorderTargetForms(int nIndexOld, int nIndexNew)
        {
            XElement[] targetWords = Xml.Descendants().ToArray();
            Xml.Descendants().Remove();
            var targetWord = targetWords[nIndexOld];
            targetWords.SetValue(targetWords[nIndexNew], nIndexOld);
            targetWords.SetValue(targetWord, nIndexNew);
            foreach (var xElement in targetWords)
                Xml.Add(xElement);
        }

        public void MoveTargetFormDown(string strTargetWord)
        {
            var targetWords = Xml.Descendants().ToArray();
            Xml.Descendants().Remove();
            bool bFound = false;
            for (int i = 0; i < (targetWords.Length - 1); i++)
            {
                var thisTargetWord = targetWords[i];
                if (strTargetWord == AdaptItKBReader.GetAttributeValue(thisTargetWord,
                                                                       TargetWordElement.CstrAttributeNameTargetWord,
                                                                       ""))
                {
                    bFound = true;
                    Xml.Add(targetWords[i++ + 1]);
                }

                Xml.Add(thisTargetWord);
            }

            if (!bFound)
                Xml.Add(targetWords.Last());
        }
    }

    [ClassInterface(ClassInterfaceType.None)]
    internal class MapOfSourceWordElements
    {
        public int NumOfWordsPerPhrase;

        public MapOfMaps ParentMapOfMaps;
        private XElement _elem;

        public MapOfSourceWordElements(XElement elem, int numOfWordsPerPhrase, MapOfMaps parentMapOfMaps)
        {
            _elem = elem;
            NumOfWordsPerPhrase = numOfWordsPerPhrase;
            ParentMapOfMaps = parentMapOfMaps;
        }

        public const string CstrElementNameNumOfWordsPerPhraseMap = "MAP";
        public const string CstrAttributeNameNumOfWordsPerPhrase = "mn";

        public XElement Xml
        {
            get { return _elem; }
        }

        public static MapOfSourceWordElements CreateNewMapOfSourceWordElements(int nNumOfWordsPerPhrase, 
            MapOfMaps mapOfMaps)
        {
            /*
            <MAP mn="1">
              <TU f="0" k="Δ">
                <RS n="1" a="ασδγδ" />
              </TU>
            </MAP>
            */
            var elem = new XElement(CstrElementNameNumOfWordsPerPhraseMap,
                                    new XAttribute(CstrAttributeNameNumOfWordsPerPhrase, nNumOfWordsPerPhrase));

            return new MapOfSourceWordElements(elem, nNumOfWordsPerPhrase, mapOfMaps);
        }

        public SourceWordElement AddCouplet(string strLhs, string strRhs)
        {
            Debug.Assert(!String.IsNullOrEmpty(strLhs) && !String.IsNullOrEmpty(strRhs));
            SourceWordElement sourceWordElement;
            if (!TryGetValue(strLhs, out sourceWordElement))
            {
                sourceWordElement = SourceWordElement.CreateNewSourceWordElement(strLhs, this);
                Xml.Add(sourceWordElement.Xml);
            }

            sourceWordElement.AddWord(strRhs);
            return sourceWordElement;
        }

        public bool TryGetValue(string strSourceWord, out SourceWordElement sourceWordElement)
        {
            XElement elem = GetDecendent(strSourceWord);
            if (elem == null)
            {
                sourceWordElement = null;
                return false;
            }
            sourceWordElement = new SourceWordElement(elem, this);
            return true;
        }

        private XElement GetDecendent(string strSourceWord)
        {
            var strXPath = String.Format("{0}[@{1} = '{2}']",
                                         SourceWordElement.CstrElementNameSourceWord,
                                         SourceWordElement.CstrAttributeNameSourceWord,
                                         strSourceWord);
            return Xml.XPathSelectElement(strXPath);
        }

        public void Remove(string strSourceWord)
        {
            var elem = GetDecendent(strSourceWord);
            if (elem != null)
                elem.Remove();
        }

        public void ReplaceSourceWordElement(string strSourceWord, XElement elem)
        {
            Remove(strSourceWord);
            Xml.Add(elem);
        }
    }

    [ClassInterface(ClassInterfaceType.None)]
    internal class MapOfMaps
    {
        public string DocVersion { get; set; }
        public string KbVersion { get; set; }
        public string SrcLangName { get; set; }
        public string TgtLangName { get; set; }
        public string Max { get; set; }
        public string GlossingKb { get; set; }

        public const string CstrElementNameKnowledgeBase = "KB";
        public const string CstrAttributeNameDocVersion = "docVersion";
        public const string CstrAttributeNameKbVersion = "kbVersion";
        public const string CstrAttributeNameSourceLanguageName = "srcName";
        public const string CstrAttributeNameTargetLanguageName = "tgtName";
        public const string CstrAttributeNameMax = "max";
        public const string CstrAttributeNameGlossingKb = "glossingKB";

        public XDocument XDocument;
        public bool IsKbV2;

        private XElement _elem;
        private MapOfMaps(XElement elem)
        {
            _elem = elem;
        }

        public MapOfMaps(string strSrcLangName, string strTgtLangName)
        {
            _elem = CreateMapOfMapsElement(strSrcLangName, strTgtLangName);
            XDocument = NewAiXdocument;
            XDocument.Add(_elem);
        }

        public XElement Xml
        {
            get { return _elem; }
        }

        public static MapOfMaps LoadXml(string strFilename)
        {
            if (!File.Exists(strFilename))
                EncConverters.ThrowError(ErrStatus.CantOpenReadMap, strFilename);

            var doc = XDocument.Load(strFilename);

            // determine if this is the new or old KB format (old: {docVersion}; 
            //  new: {kbVersion})
            var elemKb = doc.Elements().First();
            XAttribute attr;
            var strSrcLanguage = ((attr = elemKb.Attribute(CstrAttributeNameSourceLanguageName)) != null)
                                     ? attr.Value
                                     : null;
            var strTgtLanguage = ((attr = elemKb.Attribute(CstrAttributeNameTargetLanguageName)) != null)
                                     ? attr.Value
                                     : null;
            attr = elemKb.Attribute(CstrAttributeNameDocVersion);
            var mapOfMaps = new MapOfMaps(elemKb)
                                {
                                    XDocument = doc, 
                                    IsKbV2 = (attr == null),
                                    SrcLangName = strSrcLanguage,
                                    TgtLangName = strTgtLanguage
                                };

            return mapOfMaps;
        }
        
        public SourceWordElement AddCouplet(string strLhs, string strRhs)
        {
            Debug.Assert(!String.IsNullOrEmpty(strLhs) && !String.IsNullOrEmpty(strRhs));
            string[] astrLhsWords = strLhs.Split(AdaptItKBReader.CaSplitChars, StringSplitOptions.RemoveEmptyEntries);
            if (astrLhsWords.Length == 0)
                throw new ApplicationException(Properties.Resources.IDS_CantHaveEmptySourceWord);

            MapOfSourceWordElements mapOfSourceWordElements;
            if (!TryGetValue(astrLhsWords.Length, out mapOfSourceWordElements))
            {
                mapOfSourceWordElements = MapOfSourceWordElements.CreateNewMapOfSourceWordElements(astrLhsWords.Length,
                                                                                                   this);
                Xml.Add(mapOfSourceWordElements.Xml);
            }

            return mapOfSourceWordElements.AddCouplet(strLhs, strRhs);
        }

        public List<string> ListOfAllSourceWordForms
        {
            get
            {
                var strXPath = String.Format("//{0}",
                                             SourceWordElement.CstrElementNameSourceWord);

                return (from sourceWord in Xml.XPathSelectElements(strXPath)
                        select sourceWord.Attribute(SourceWordElement.CstrAttributeNameSourceWord)
                        into attrSourceWord 
                        where attrSourceWord != null 
                        select attrSourceWord.Value).ToList();
            }
        }

        private bool TryGetValue(int nMapValue, out MapOfSourceWordElements mapOfSourceWordElements)
        {
            var strXPath = String.Format("{0}[@{1} = '{2}']",
                                         MapOfSourceWordElements.CstrElementNameNumOfWordsPerPhraseMap,
                                         MapOfSourceWordElements.CstrAttributeNameNumOfWordsPerPhrase,
                                         nMapValue);
            var elem = Xml.XPathSelectElement(strXPath);
            if (elem == null)
            {
                mapOfSourceWordElements = null;
                return false;
            }
            mapOfSourceWordElements = new MapOfSourceWordElements(elem, nMapValue, this);
            return true;
        }

        public static void SortAiKb(string strFilename)
        {
            // Load the knowledge base
            var xDocument = XDocument.Load(strFilename);
            SaveFile(strFilename, xDocument);
        }

        public void SaveFile(string strFilename)
        {
            SaveFile(strFilename, XDocument);
        }

        private static void SaveFile(string strFilename, XDocument xDocument)
        {
            Debug.Assert(!String.IsNullOrEmpty(strFilename) && (xDocument != null));

            // first make a backup
            if (!Directory.Exists(Path.GetDirectoryName(strFilename)))
                Directory.CreateDirectory(Path.GetDirectoryName(strFilename));

            // save it with an extra extn.
            const string cstrExtraExtnToAvoidClobberingFilesWithFailedSaves = ".bad";
            string strTempFilename = strFilename + cstrExtraExtnToAvoidClobberingFilesWithFailedSaves;

            // it's *really bad* if there are any source word elements without
            //  source word values or target word elements without target word
            //  values, so if they exist, let's get rid of them immediately
            CleanUpDocument(ref xDocument);

            // create the root portions of the XML document and sort the running fragment 
            //  into it.
            var doc = NewAiXdocument;

            var xslt = new XslCompiledTransform();
            xslt.Load(XmlReader.Create(new StringReader(Properties.Resources.SortAIKB)));

            using (XmlWriter writer = doc.CreateWriter())
            {
                // Execute the transform and output the results to a writer.
                xslt.Transform(xDocument.CreateReader(), writer);
                writer.Close();
            }

            doc.Save(strTempFilename);

            // backup the last version to appdata
            // Note: doing File.Move leaves the old file security settings rather than replacing them
            // based on the target directory. Copy, on the other hand, inherits
            // security settings from the target folder, which is what we want to do.
            if (File.Exists(strFilename))
                File.Copy(strFilename, GetBackupFilename(strFilename), true);
            File.Delete(strFilename);
            File.Copy(strTempFilename, strFilename, true);
            File.Delete(strTempFilename);
        }

        private static XDocument NewAiXdocument
        {
            get
            {
                return new XDocument(
                    new XDeclaration("1.0", "utf-8", "yes"),
                    new XComment(Properties.Resources.AdaptItKbDescription));
            }
        }

        private static void CleanUpDocument(ref XDocument doc)
        {
            // get rid of the TU[@k = ""]
            var strXPath = String.Format("//{0}[@{1} = \"\"]",
                                         SourceWordElement.CstrElementNameSourceWord,
                                         SourceWordElement.CstrAttributeNameSourceWord);
            PergeXPath(ref doc, strXPath);

            // get rid of the RS[@a = ""]
            strXPath = String.Format("//{0}[@{1} = \"\"]",
                                     TargetWordElement.CstrElementNameTargetWord,
                                     TargetWordElement.CstrAttributeNameTargetWord);
            PergeXPath(ref doc, strXPath);
        }

        private static void PergeXPath(ref XDocument doc, string strXPath)
        {
            XElement elem;
            while ((elem = doc.XPathSelectElement(strXPath)) != null)
            {
                Debug.Assert(false, String.Format("oops, found: {0} in the KB... removing",
                                                  elem));
                elem.Remove();
            }
        }

        private static string GetBackupFilename(string strFilename)
        {
            return Application.UserAppDataPath + @"\Backup of " + Path.GetFileName(strFilename);
        }

        protected XElement CreateMapOfMapsElement(string strSrcLangName, string strTgtLangName)
        {
            /*
              <KB docVersion="4" srcName="Greek" tgtName="English" max="1">
                <MAP mn="1">
                  <TU f="0" k="Δ">
                    <RS n="1" a="ασδγδ" />
                  </TU>
                </MAP>
              </KB>
            */
            SrcLangName = strSrcLangName;
            TgtLangName = strTgtLangName;
            Max = "1";
            var elem = new XElement(CstrElementNameKnowledgeBase,
                                    (IsKbV2)
                                        ? new XAttribute(CstrAttributeNameKbVersion, 2)
                                        : new XAttribute(CstrAttributeNameDocVersion, 4),
                                    new XAttribute(CstrAttributeNameSourceLanguageName, SrcLangName),
                                    new XAttribute(CstrAttributeNameTargetLanguageName, TgtLangName),
                                    new XAttribute(CstrAttributeNameMax, Max));

            if (IsKbV2)
                elem.Add(new XAttribute(CstrAttributeNameGlossingKb, "0"));

            return elem;
        }

        public bool TryGetValue(string strSourceWord, out MapOfSourceWordElements mapOfSourceWordElements)
        {
            // this is a helper to find the proper MapOfSourceWordElements based on the
            //  source word. First we have to find the map to look into
            // first get the map for the number of words in the source string (e.g. "ke picche" would be map=2)
            Debug.Assert(!String.IsNullOrEmpty(strSourceWord));
            int nMapValue = strSourceWord.Split(AdaptItKBReader.CaSplitChars, StringSplitOptions.RemoveEmptyEntries).Length;
            return TryGetValue(nMapValue, out mapOfSourceWordElements);
        }

        /// <summary>
        /// This helper can be used to change the source word to some other value (which
        /// might include it being put into a different map)
        /// </summary>
        /// <param name="strOldSourceWord"></param>
        /// <param name="strNewSourceWord"></param>
        /// <returns></returns>
        public SourceWordElement ChangeSourceWord(string strOldSourceWord, 
            string strNewSourceWord)
        {
            Debug.Assert(!String.IsNullOrEmpty(strOldSourceWord) && !String.IsNullOrEmpty(strNewSourceWord));
            MapOfSourceWordElements mapOfSourceWordElements;
            if (TryGetValue(strOldSourceWord, out mapOfSourceWordElements))
            {
                SourceWordElement sourceWordElement, sourceWordElementNew = null;
                if (mapOfSourceWordElements.TryGetValue(strOldSourceWord, out sourceWordElement))
                {
                    foreach (var targetWordElement in sourceWordElement.Descendants())
                    {
                        // use AddCouplet since this may involve adding a new map)
                        sourceWordElementNew = AddCouplet(strNewSourceWord,
                                                          targetWordElement.TargetWord);
                    }

                    // finally remove the old one
                    mapOfSourceWordElements.Remove(strOldSourceWord);
                }

                return sourceWordElementNew;
            }
            return null;
        }
    }
}
