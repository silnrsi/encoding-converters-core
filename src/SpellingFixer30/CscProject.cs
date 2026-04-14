#define SaveSoapFormatter

using ECInterfaces;
using Microsoft.Win32;                  // for RegistryKey
using SilEncConverters40;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Data;
using System.Drawing;
using System.Globalization;							// for CultureInfo
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;   // for the class attributes
using System.Runtime.Serialization;                 // for SerializationException
using System.Runtime.Serialization.Formatters.Soap;
using System.Text;
using System.Text.RegularExpressions;
using System.Windows.Forms;
using System.Xml;
using System.Xml.Serialization;
using System.Xml.Xsl;
#if UsingJump2Toolbox
using Jump2Toolbox;
#endif

namespace SpellingFixer30
{
    public enum WordCheckResult
    {
        Unknown = 0,                // means it's in the Words to check list
        InGoodWordDictionary = 1,   // white list
        InBadWordDictionary = 2     // black list (with a correction specified)
    };

    public enum SpellFixResult
    {
        Cancel,
        IgnoreOnce,
        IgnoreAlways,
        AddToDictionary,
        ChangeOnce,
        ChangeAlways,
        Undo,
        NotAmbiguous
    }

    [Serializable()]
    [ComVisible(true)]
    [ClassInterface(ClassInterfaceType.AutoDual)]
    public abstract class CscProject
    {
        public static readonly string ProgDataDefaultSpellFixerRootPathSuffix = Path.Combine("SIL", "ConsistentSpellingFixer");
        public const string DefaultProjectsFolderPathSuffix = "Projects";
        public const string DefaultScriptSystemFolderSuffix = "ScriptSystems";
        public const string RegistryDefaultScriptSystemSubFolderKey = "VernacularScriptSystemFolder";
        public const string RegistryProjectsFolderRootKey = @"ProjectsFolder";
        public const string DefaultTransliteratorEncConverterName = "Any-Latin";
        public const string ApplicationCaption = "Consistent Spelling Fixer";
        public const string DefaultEncConverterFileNamePrefix = "Consistent Spelling for ";
        public const string ProjectFileExt = "csf";
        public const string VernacularScriptSystemFileExt = "xml";
		public const string RegistryCscRootKey = @"SOFTWARE\SIL\ConsistentSpellingFixer";

		public const string ChooseProjectException = "You must choose a project file to continue";

        protected string _projectName = null;       // [CommonApplicationData]SIL\ConsistentSpellingFixer\Projects\<name>\<name>.csf
        protected string _scriptSystemName = null;  // [CommonApplicationData]SIL\ConsistentSpellingFixer\ScriptSystems\<name>.xml
        protected CultureInfo _locale = null;       // e.g. Hindi (India)

        protected string _customCcCode = null;      // SFM processing custom code for SpellFixer CC table.

        protected bool _compareDoubleChars = true;
        protected bool _compareCase = true;

        protected Font _projectFont = null;
        protected Font _transliteratorFont = null;
        protected ArrayList _ruleNames = new ArrayList();

        protected int _wordsInContext = 5;
        protected int _maxContextStrings = 20;

        public HashSet<string> MarkersToAvoidForSpellingCheck;

#if UsingJump2Toolbox
        protected static Jump2Toolbox.CToolbox m_aJump2Tbx = null;
#endif

        [Flags]
        protected enum SaveDbFlags
        {
            None = 0,
            KnownGoodWords = 1,
            Bad2GoodWords = 2,
            IgnoreWords = 4,
            CheckWords = 8
        }

        // the following are not stored in the project file, but are used in the project objects
        // Since all objects are actually created by "LoadProject" which serializes the object in from
        //  scratch, these will always be uninitialized... so you never need to initialize them with some 
        //  object (e.g. m_mapS2SBad2GoodWords = new Bad2GoodMap()... is useless, because those instructions
        //  will never be called when this thing is serialized in).
        // If some object absolutely must have something (i.e. not be null), then you must initialize it
        //  during Init (which is called during LoadProject) and create it there (c.f. m_achTrimPunctuation)
        [NonSerializedAttribute()]
        internal KnownGoodWordList m_mapS2SfwKnownGoodWords = null;
        [NonSerializedAttribute()]
        internal Bad2GoodMap m_mapS2SBad2GoodWords = null;
        [NonSerializedAttribute()]
        internal IgnoreList m_mapS2SfwIgnoreWords = null;
        [NonSerializedAttribute()]
        internal WordsToCheck m_mapS2SfwWordsToCheck = null;
        [NonSerializedAttribute()]
        internal AmbiguityListMaps m_mapAmbiguities = null;
        [NonSerializedAttribute()]
        protected DirectableEncConverter m_aEcTrans = null;
        [NonSerializedAttribute()]
        internal CorrectSpellingPicker m_dlgQueryFix = null;
        [NonSerializedAttribute()]
        protected string m_strLastCorrection = null;
        [NonSerializedAttribute()]
        protected SaveDbFlags m_eSaveNeeded = SaveDbFlags.None;
        [NonSerializedAttribute()]
        protected string[] m_astrIgnoreStrsBeforeTransliteration = null;
        [NonSerializedAttribute()]
        protected string[] m_astrIgnoreStrsAfterTransliteration = null;
        [NonSerializedAttribute()]
        protected List<char> m_achTrimPunctuation = null;
        [NonSerializedAttribute()]
        protected List<Tuple<Regex,string>> RegexChanges = new List<Tuple<Regex, string>>();

        public CscProject()
        {
            // users of the default ctor must call Init explicitly (once they've determined the project name)
        }

        internal VernScriptSystem Init()
        {
            m_dlgQueryFix = new CorrectSpellingPicker(this);

            VernScriptSystem sfssp = null;
            try
            {
                var strVSSFileSpec = VernacularScriptSystemFileSpec;
                sfssp = VernScriptSystem.LoadVsvXml(strVSSFileSpec);

                if (_ruleNames.Count > 0)
                {
                    m_mapAmbiguities = new AmbiguityListMaps();
                    foreach (string strRuleName in _ruleNames)
                    {
                        System.Diagnostics.Trace.WriteLine(String.Format("strRuleName: {0}", strRuleName));
                        var distinction = sfssp.Distinctions.DistinctionList.FirstOrDefault(d => d.Name == strRuleName);
                        if (distinction != null)
                        {
                            foreach (var normalizeRule in distinction.NormalizeRules)
                            {
                                var characterToNormalize = normalizeRule.CharactersToNormalize.CharacterToNormalize.Select(c => c.Value).ToList();
                                AddAmbiguityRule(distinction.Name, normalizeRule.NormalizedForm, characterToNormalize);
                            }
                        }
                        else
                        {
                            MessageBox.Show(String.Format("The project is configured to use an ambiguity rule named '{0}', but it's not in the file '{1}'!? You should edit the project to fix this.",
                                strRuleName, strVSSFileSpec));
                        }
                    }
                }

                Transliterator = GetTransliterator(sfssp);

                // this ought to exist (so it isn't null) even if there are no extra punctuation
                m_achTrimPunctuation = new List<char>();
                if (sfssp.ExtraPunctuation != null)
                {
                    var ignorePunctuations = sfssp.ExtraPunctuation.IgnorePunctuations;
                    foreach (var ignorePunctuation in ignorePunctuations)
                    {
                        var strPunct = ignorePunctuation.Value;
                        if (strPunct.Length > 1)
                            throw new ApplicationException($"{ApplicationCaption} doesn't currently support multi-character punctuation. If you really need it, email silconverters_support@sil.org to ask for support.");
                        m_achTrimPunctuation.Add(strPunct[0]);
                    }
                }

                // get the list of regex changes to use to preprocess the data before checking them
                if (sfssp.PreprocessDataViaRegex != null)
                {
                    foreach (var regexChange in sfssp.PreprocessDataViaRegex.RegexChangeList)
                    {
                        var options = RegexOptions.None;
                        if (regexChange.IgnoreCase)
                            options |= RegexOptions.IgnoreCase;
                        if (regexChange.Multiline)
                            options |= RegexOptions.Multiline;
                        if (regexChange.ExplicitCapture)
                            options |= RegexOptions.ExplicitCapture;
                        if (regexChange.Compiled)
                            options |= RegexOptions.Compiled;
                        if (regexChange.CultureInvariant)
                            options |= RegexOptions.CultureInvariant;
                        if (regexChange.RightToLeft)
                            options |= RegexOptions.RightToLeft;
                        try
                        {
                            var regex = new Regex(regexChange.Pattern, options);
                            RegexChanges.Add(Tuple.Create(regex, regexChange.ReplaceWith));
                        }
                        catch (ArgumentException ex)
                        {
                            MessageBox.Show(String.Format("The regex pattern '{0}' is invalid: {1}", regexChange.Pattern, ex.Message));
                        }
                    }
                }

                // get list of characters we're supposed to strip out of words
                if (sfssp.IgnoreCharactersBeforeTransliteration != null)
                {
                    var ignoreCharactersBeforeTransliteration = sfssp.IgnoreCharactersBeforeTransliteration.Characters;
                    m_astrIgnoreStrsBeforeTransliteration = ignoreCharactersBeforeTransliteration.Select(c => c.Value).ToArray();
                }

                // get list of characters we're supposed to strip out of words
                if (sfssp.IgnoreCharactersAfterTransliteration != null)
                {
                    var ignoreCharactersAfterTransliteration = sfssp.IgnoreCharactersAfterTransliteration.Characters;
                    m_astrIgnoreStrsAfterTransliteration = ignoreCharactersAfterTransliteration.Select(c => c.Value).ToArray();
                }
            }
#if DEBUG
            catch (Exception ex)
            {
                // don't think I want to do this in the release product or we'll never be able to edit the project.
                throw ex;
#else
            catch
            {
#endif
            }

            // read in the bad 2 good mapping
            m_mapS2SBad2GoodWords = Bad2GoodMap.LoadTable(SpellFixerEncConverterSpec);

            // read in the known good list and regenerate all the simplified forms (in case
            //  the rules changed)
            string strProjectDirectory = ProjectDirectory;
            m_mapS2SfwKnownGoodWords = KnownGoodWordList.LoadTable(strProjectDirectory, this);
            foreach (SpellFixerWord aWord in m_mapS2SfwKnownGoodWords.Values)
                aWord.InitializeNonStaticData(this);

            // read in the running list of words to check (and regenerate their simplified
            //  forms also)
            m_mapS2SfwWordsToCheck = WordsToCheck.LoadTable(strProjectDirectory, this);
            foreach (SpellFixerWord aWord in m_mapS2SfwWordsToCheck.Values)
                aWord.InitializeNonStaticData(this);

            // anything that was added to the ignore list in the last round, reload them so
            //  we can reconsider them
            m_mapS2SfwIgnoreWords = IgnoreList.LoadTable(strProjectDirectory, this);

            // for the new session, add the ignore words to the words to check
            foreach (SpellFixerWord aWord in m_mapS2SfwIgnoreWords.Values)
                AddWordToCheckList(aWord.Value, false, aWord.ContextStrings);

            m_mapS2SfwIgnoreWords.Clear();   // and then clear it out to start afresh

            return sfssp;
        }

        internal static DirectableEncConverter GetTransliterator(VernScriptSystem sfssp)
        {
            var vernSystem = sfssp.Properties;
            var strTransEcName = vernSystem.TransliteratorEncConverterName ?? DefaultTransliteratorEncConverterName;
            var bForward = vernSystem.TransliteratorEncConverterDirectionForward ?? true;
            if (!Enum.TryParse(vernSystem.TransliteratorEncConverterNormalize, out NormalizeFlags eNormForm))
                eNormForm = NormalizeFlags.None;

            IEncConverter aIEC = DirectableEncConverter.EncConverters[strTransEcName];

            if ((aIEC == null) && (strTransEcName == DefaultTransliteratorEncConverterName))
            {
                InsureAnyToLatin();
                aIEC = DirectableEncConverter.EncConverters[strTransEcName];
            }

            return (aIEC != null) ? new DirectableEncConverter(aIEC.Name, bForward, eNormForm) : null;
        }

        internal static void InsureAnyToLatin()
        {
            IEncConverter aIEC = DirectableEncConverter.EncConverters[DefaultTransliteratorEncConverterName];

            if (aIEC == null)
            {
                // must not exist; try to create it
                DirectableEncConverter.EncConverters.Add(DefaultTransliteratorEncConverterName, "Any-Latin", ConvType.Unicode_to_from_Unicode,
                    null, null, ProcessTypeFlags.ICUTransliteration);
            }
        }

        public IEncConverter SpellFixerEncConverter
        {
            get
            {
                // get it from the repository...
                EncConverters aECs = new EncConverters();
                IEncConverter aEC = aECs[SpellFixerEncConverterName];

                if (aEC == null)
                {
                    // if it's not there, then add it.
                    aECs.Add(SpellFixerEncConverterName, SpellFixerEncConverterSpec, ConvType.Unicode_to_Unicode,
                        "UNICODE", "UNICODE", ProcessTypeFlags.SpellingFixerProject);

                    aEC = aECs[SpellFixerEncConverterName];

                    // probably can't happen, but as a final fall back, just create it as a temporary thing
                    if (aEC == null)
                    {
                        aEC = new CcEncConverter();
                        string strDummy = null;
                        int nProcType = 0;
                        ConvType eConvType = ConvType.Unicode_to_Unicode;
                        aEC.Initialize(SpellFixerEncConverterName, SpellFixerEncConverterSpec,
                            ref strDummy, ref strDummy, ref eConvType,
                            ref nProcType, 0, 0, true);
                    }
                }
                else
                {
                    SpellFixerEncConverterSpec = aEC.ToString();
                }

                // just in case it hasn't been saved yet.
                if ((m_eSaveNeeded & SaveDbFlags.Bad2GoodWords) > 0)
                    SaveProjectData();

                return aEC;
            }
        }

        public string SpellFixerCustomCcCode
        {
            get { return _customCcCode; }
            set { _customCcCode = value; }
        }

        public string SpellFixerEncConverterName
        {
            get { return DefaultEncConverterFileNamePrefix + Name; }
        }

        private string _spellFixerEncConverterSpec;
        internal string SpellFixerEncConverterSpec
        {
            get
            {
                return // _spellFixerEncConverterSpec ??
                       Path.Combine(ProjectDirectory, $"{SpellFixerEncConverterName}.cct");
            }
            set
            {
                // if it already exists in a different location, then store/use that instead
                _spellFixerEncConverterSpec = value;
            }
        }

        internal void SaveBad2GoodMap()
		{
			var strPunctuation = GetWordBoundaryPunctuation;

			Bad2GoodMap.SaveTable(m_mapS2SBad2GoodWords, SpellFixerEncConverterSpec,
									SpellFixerEncConverterName, strPunctuation, SpellFixerCustomCcCode);
		}

		private string GetWordBoundaryPunctuation
		{
			get
			{
				string strPunctuation = SpellingFixer.GetDefaultPunctuation;
				if (m_achTrimPunctuation.Count > 0)
					strPunctuation = String.Format("{0} {1}", strPunctuation, ExtraPunctuation);
				return strPunctuation;
			}
		}

		internal string ExtraPunctuation
        {
            get
            {
				if (m_achTrimPunctuation == null || m_achTrimPunctuation.Count == 0)
					return String.Empty;
				string strExtraPunctuation = null;
                foreach (char ch in m_achTrimPunctuation)
                    strExtraPunctuation += String.Format("'{0}' ", ch);
                return strExtraPunctuation.Substring(0, strExtraPunctuation.Length - 1);
            }
        }

        internal bool SaveNeeded
        {
            get { return (m_eSaveNeeded != SaveDbFlags.None); }
        }

        internal void SaveData()
        {
            if (SaveNeeded)
            {
                if ((m_eSaveNeeded & SaveDbFlags.KnownGoodWords) > 0)
                {
                    KnownGoodWordList.SaveTable(m_mapS2SfwKnownGoodWords, ProjectDirectory);
                    m_eSaveNeeded &= ~SaveDbFlags.KnownGoodWords;
                    System.Diagnostics.Trace.WriteLine("In CscProject: SaveData: KnownGoodWords");
                }
                else if ((m_eSaveNeeded & SaveDbFlags.Bad2GoodWords) > 0)
                {
                    SaveBad2GoodMap();
                    m_eSaveNeeded &= ~SaveDbFlags.Bad2GoodWords;
                    System.Diagnostics.Trace.WriteLine("In CscProject: SaveData: Bad2GoodWords");
                }
                else if ((m_eSaveNeeded & SaveDbFlags.CheckWords) > 0)
                {
                    WordsToCheck.SaveTable(m_mapS2SfwWordsToCheck, ProjectDirectory);
                    m_eSaveNeeded &= ~SaveDbFlags.CheckWords;
                    System.Diagnostics.Trace.WriteLine("In CscProject: SaveData: WordsToCheck");
                }
                else if ((m_eSaveNeeded & SaveDbFlags.IgnoreWords) > 0)
                {
                    IgnoreList.SaveTable(m_mapS2SfwIgnoreWords, ProjectDirectory);
                    m_eSaveNeeded &= ~SaveDbFlags.IgnoreWords;
                    System.Diagnostics.Trace.WriteLine("In CscProject: SaveData: IgnoreWords");
                }
            }
        }

        /// <summary>
        /// The root folder for the ConsistentSpellingFixer project and data files (by default: 'C:\ProgramData\SIL\ConsistentSpellingFixer')
        /// </summary>
        internal static string ConsistentSpellingFixerDefaultRoot
        {
            get
            {
                return Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData), ProgDataDefaultSpellFixerRootPathSuffix);
            }
        }

        /// <summary>
        /// Returns the root folder for the Projects folder (by default: 'C:\ProgramData\SIL\ConsistentSpellingFixer\Projects')
        /// To change it, set the 'HKLM\SOFTWARE\SIL\ConsistentSpellingFixer[ProjectsFolder]' key in regedit
        /// </summary>
        public static string ProjectsDefaultDirectory
        {
            get
            {
                return Path.Combine(ConsistentSpellingFixerDefaultRoot, DefaultProjectsFolderPathSuffix);
            }
        }

        /// <summary>
        /// Returns the root folder for the Vernacular Script System folder (by default: 'C:\ProgramData\SIL\ConsistentSpellingFixer\ScriptSystem')
        /// To change it, set the 'HKLM\SOFTWARE\SIL\ConsistentSpellingFixer[VernacularScriptSystemFolder]' key in regedit
        /// </summary>
        public static string ScriptSystemDefaultDirectory
        {
            get
            {
                return Path.Combine(ConsistentSpellingFixerDefaultRoot, DefaultScriptSystemFolderSuffix);
            }
        }

        internal string VernacularScriptSystemFileSpec
        {
            get
			{
				return CreateVernacularScriptSystemFileSpec(VernacularScriptSystemName);
			}
		}

		public static string CreateVernacularScriptSystemFileSpec(string vernacularScriptSystemName)
		{
			return Path.Combine(VernacularScriptSystemsDirectory,
								$"{vernacularScriptSystemName}.{VernacularScriptSystemFileExt}");
		}

		/// <summary>
		/// Returns the root folder for the Vernacular Script System folder (by default: 'C:\ProgramData\SIL\ConsistentSpellingFixer\ScriptSystems')
		/// To change it, set the 'RegistryCscRootKey' property
		/// </summary>
		public static string VernacularScriptSystemsDirectory
        {
            get
            {
                var keyCscRoot = Registry.LocalMachine.OpenSubKey(RegistryCscRootKey);
                var vernScriptDirectory = ((string)keyCscRoot?.GetValue(RegistryDefaultScriptSystemSubFolderKey)) ??
                                            ScriptSystemDefaultDirectory;

                return vernScriptDirectory;
            }

            set
            {
                RegistryKey keyProject = Registry.LocalMachine.OpenSubKey(RegistryCscRootKey);
                if (keyProject != null)
                {
                    var vernScriptDirectory = value;
                    if (vernScriptDirectory.Substring(vernScriptDirectory.Length - 1, 1) == @"\")
                        vernScriptDirectory = value.Substring(0, value.Length - 1);
                    keyProject.SetValue(RegistryDefaultScriptSystemSubFolderKey, vernScriptDirectory);
                }
            }
        }

        protected static string CheckForFolderExistence(string strFolderPath, string strDescription)
        {
            if (!String.IsNullOrEmpty(strFolderPath))
            {
                if (!Directory.Exists(strFolderPath))
                {
                    // Dude! where's your car?
                    var dlg = new FolderBrowserDialog
                    {
                        SelectedPath = ProjectsDefaultDirectory,
                        Description = strDescription
                    };
                    if (dlg.ShowDialog() == DialogResult.OK)
                        throw new ApplicationException(dlg.SelectedPath);
                    else
                        throw new ApplicationException(ChooseProjectException);
                }
            }

            return strFolderPath;
        }

        internal static string GetProjectsFolder
        {
            get
            {
                RegistryKey keyCscRoot = Registry.LocalMachine.OpenSubKey(RegistryCscRootKey);
                if (keyCscRoot != null)
                {
                    return (string)keyCscRoot.GetValue(RegistryProjectsFolderRootKey);
                }
                return ProjectsDefaultDirectory;
            }
        }

        internal static List<string> GetProjectNames
        {
            get
            {
                // get the root projects folder either from the registry or the default in ProgramData
                var keyCscRoot = Registry.LocalMachine.OpenSubKey(RegistryCscRootKey);
                var projectsRootFolder = ((string)keyCscRoot?.GetValue(RegistryProjectsFolderRootKey)) ??
                                            ProjectsDefaultDirectory;

                // save it in the settings (for the current app)
                if (Properties.Settings.Default.MapProjectNameToProjectPath == null)
                    Properties.Settings.Default.MapProjectNameToProjectPath = new StringCollection();

                // get the list of projects we already know about (for this app/user)
                var mapProjectNameToProjectPath = SettingToDictionary(Properties.Settings.Default.MapProjectNameToProjectPath);
                if (projectsRootFolder != null)
                {
                    var projectPaths = new List<string>();
                    if (!Directory.Exists(projectsRootFolder))
                    {
                        Directory.CreateDirectory(projectsRootFolder);
                    }
                    else
                    {
                        projectPaths = Directory.GetDirectories(projectsRootFolder).ToList();
                    }

                    // compare w/ what's in the settings, and add any that are missing
                    var bAdded = false;
                    foreach (var projectPath in projectPaths)
                    {
                        var projectName = Path.GetFileName(projectPath);

                        // either it doesn't exist, or the path has changed
                        if (!mapProjectNameToProjectPath.Keys.Contains(projectName) ||
                            (mapProjectNameToProjectPath[projectName].FirstOrDefault()?.ToLower() != projectPath.ToLower()))
                        {
                            var lstProjectPaths = new List<string> { projectPath };
                            mapProjectNameToProjectPath[projectName] = lstProjectPaths;
                            bAdded = true;
                        }
                    }

                    if (bAdded)
                    {
                        Properties.Settings.Default.MapProjectNameToProjectPath = SettingFromDictionary(mapProjectNameToProjectPath);
                        Properties.Settings.Default.Save();
                    }
                }

                return mapProjectNameToProjectPath.Keys.ToList();
            }
        }

        internal static string GetProjectDirectory(string projectName)
        {
            System.Diagnostics.Debug.Assert(!String.IsNullOrEmpty(projectName));
            var projectDirectory = Path.Combine(GetProjectsFolder, projectName);
            
            try
            {
                projectDirectory = CheckForFolderExistence(projectDirectory, String.Format("The {0} project folder for the {1} project is missing! Browse to relocated it", ApplicationCaption, projectName));
            }
            catch (ApplicationException ex)
            {
                if (ex.Message == ChooseProjectException)
                    throw ex;

                // otherwise, this is the new folder the user selected
                projectDirectory = ex.Message;
                if (!Directory.Exists(projectDirectory))
                    throw ex;
            }

            return projectDirectory;
        }

        protected const char chNeverUsedChar = '\u0009';  // add to words we replace, so we don't process them again

		public static Dictionary<string, List<string>> SettingToDictionary(StringCollection data)
        {
            var map = new Dictionary<string, List<string>>();
            for (var i = 0; i < data.Count; i += 2)
            {
                map.Add(data[i], data[i + 1].Split(new[] { chNeverUsedChar }, StringSplitOptions.RemoveEmptyEntries).ToList());
            }

            return map;
        }

        public static StringCollection SettingFromDictionary(Dictionary<string, List<string>> map)
        {
            var lst = new StringCollection();
            foreach (KeyValuePair<string, List<string>> kvp in map)
            {
                lst.Add(kvp.Key);
                lst.Add(String.Join(chNeverUsedChar.ToString(), kvp.Value));
            }

            return lst;
        }

        /// <summary>
		/// Returns the root folder for this project (by default: 'C:\ProgramData\SIL\ConsistentSpellingFixer\Projects\<projectName>')
		/// </summary>
		public string ProjectDirectory
        {
            get { return GetProjectDirectory(Name); }
            set { AddProjectKey(value, Name); }
        }

        /// <summary>
        /// Add a project to the system by setting the project path and name
        /// </summary>
        /// <param name="projectDirectory">Path to the project folder</param>
        /// <param name="strName">Name of the project file (e.g. Kangri.csf)</param>
        public static void AddProjectKey(string projectDirectory, string projectName)
        {
            System.Diagnostics.Debug.Assert(!String.IsNullOrEmpty(projectName) && (projectDirectory.Last() != '\\'));

            // save it in the settings (for the current app)
            if (Properties.Settings.Default.MapProjectNameToProjectPath == null)
                Properties.Settings.Default.MapProjectNameToProjectPath = new StringCollection();

            var mapProjectNameToProjectPath = SettingToDictionary(Properties.Settings.Default.MapProjectNameToProjectPath);
            var lstProjectName = new List<string> { projectDirectory };
            mapProjectNameToProjectPath[projectName] = lstProjectName;
            Properties.Settings.Default.MapProjectNameToProjectPath = SettingFromDictionary(mapProjectNameToProjectPath);
            Properties.Settings.Default.Save();

            if (!Directory.Exists(projectDirectory))
                Directory.CreateDirectory(projectDirectory);
        }

        public static void BackupFile(string fileSpec)
        {
            var i = 0;
            var newFileSpec = fileSpec;
            while (File.Exists(newFileSpec))
            {
                newFileSpec = Path.Combine(Path.GetDirectoryName(fileSpec), $"{Path.GetFileNameWithoutExtension(fileSpec)} ({i++}){Path.GetExtension(fileSpec)}.bak");
            }

            if (fileSpec != newFileSpec)
            {
                while (!CscProjectStore.CanWriteToFile(fileSpec))
                {
                    if (MessageBox.Show($"Can't write to the file {fileSpec}. But you can (manually) delete it, so it'll be recreated.", CscProject.ApplicationCaption, MessageBoxButtons.OKCancel) == DialogResult.Cancel)
                        return;
                }

                File.Move(fileSpec, newFileSpec);
            }
        }

        /// <summary>
		/// Call this method to bring up the UI for selecting (or creating) a Consistent Spell Fixing project
		/// </summary>
		/// <returns>The selected CscProject object</returns>
        public static CscProject SelectProject()
        {
            CscProject proj = null;
            SelectProject dlg = new SelectProject();
            if (dlg.ShowDialog() == DialogResult.OK)
                proj = dlg.SelectedProject;

            if (proj == null)
                throw new ApplicationException(ChooseProjectException);
            return proj;
        }

        /// <summary>
        /// Call this method to bring up the UI for editing a Consistent Spell Fixing Project
        /// </summary>
        public void EditProject()
        {
            SFProjectForm dlg = new SFProjectForm(this);
            dlg.ShowDialog();
        }

        /// <summary>
        /// Call this method to load the project information for the named project
        /// </summary>
        /// <param name="projectName">The name of the project to load</param>
        /// <returns>An initialized CscProject object</returns>
        public static CscProject LoadProject(string projectName)
        {
            var projectDirectory = GetProjectDirectory(projectName);
            var projectPath = Path.Combine(projectDirectory, $"{projectName}.{ProjectFileExt}");

            CscProject project = new CscBaseProject();
            try
            {
                CscProjectStore.TransferToCscProject(projectPath, project);
                project.Init(); // need to call Init

				// make sure the associated EncConverter for processing spelling changes is in the repo
				project.EnsureEncConverterExists(project.SpellFixerEncConverterName, project.SpellFixerEncConverterSpec);
			}
            catch (Exception ex)
            {
                MessageBox.Show("Failed to open project file. Reason: " + ex.Message);
            }

            return project;
        }

		internal bool EnsureEncConverterExists(string encConverterName, string encConverterSpec)
		{
			// get the EncConverter that should have been added above by 'AddNewProject' button
			var aECs = DirectableEncConverter.EncConverters;
			var aEC = aECs[encConverterName];
			if (aEC == null)
			{
				// add it (put it in the normal 'MapsTables' folder in \pf\cf\sil\...)
				if (File.Exists(encConverterSpec))
				{
					// the converter doesn't exist, but a file with the name we would have 
					//  given it does... ask the user if they want to overwrite it.
					// TODO: this doesn't allow for complete flexibility. It might be nicer to
					//  allow for any arbitrary name, but not if noone complains.
					if (MessageBox.Show(String.Format("A file exists by the name {0}. Click 'Yes' to overwrite", encConverterSpec), SpellingFixer.cstrCaption, MessageBoxButtons.YesNoCancel) == DialogResult.Yes)
					{
						File.Delete(encConverterSpec);
					}
				}

				// now add the EncConverter
				// TODO: EncConverters needs a new interface to get the defining encodingID from 
				//  a FontName (so we can use it in this call) just like we can 'try' to get the
				//  code page given a font name (see 'CodePage' below)
				var eConvType = ConvType.Unicode_to_Unicode;

				aECs.Add(encConverterName, encConverterSpec, eConvType, null, null, SpellingFixer.SFProcessType);

				// add this 'displaying font' information to the converter as properties/attributes
				ECAttributes aECAttrs = aECs.Attributes(encConverterName, AttributeType.Converter);
				aECAttrs.Add(SpellingFixer.cstrAttributeFontToUse, _projectFont.Name);
				aECAttrs.Add(SpellingFixer.cstrAttributeFontRightToLeft, false);    // since we transliterate it to latin, it isn't L2R anymore)
				aECAttrs.Add(SpellingFixer.cstrAttributeFontSizeToUse, _projectFont.Size);
				aECAttrs.Add(SpellingFixer.cstrAttributeWordBoundaryDelimiter, Bad2GoodMap.WordBoundaryCharacter);  // not sure this is needed anymore, since this doesn't do non-whole word forms
				aECAttrs.Add(SpellingFixer.cstrAttributeNonWordChars, GetWordBoundaryPunctuation);
			}

			if (!File.Exists(encConverterSpec))
			{
				SaveBad2GoodMap();
			}

			return true;
		}

		/// <summary>
		/// Call this method to delete the project with the given name
		/// </summary>
		/// <param name="projectName">The name of the project to delete</param>
		/// <returns>true if the data files were deleted or false if not</returns>
		public static bool DeleteProject(string projectName)
        {
            // see if they want to delete the data files also
            var projectPath = GetProjectDirectory(projectName);
            DialogResult res;
            if ((res = MessageBox.Show($"Do you also want to delete all the data files in the {projectPath} folder?", 
                                       ApplicationCaption, MessageBoxButtons.YesNoCancel)) == DialogResult.Yes)
            {
                if (!String.IsNullOrEmpty(projectPath))
                {
                    foreach (var strFilename in Directory.GetFiles(projectPath, "*.*", SearchOption.AllDirectories))
                    {
                        try
                        {
                            File.Delete(strFilename);
                        }
                        catch { }
                    }
                    try
                    {
                        Directory.Delete(projectPath, true);
                    }
                    catch { }
                }
            }
            else if (res == DialogResult.Cancel)
                return false;

            // remove it from the settings (for the current app) and save
            var mapProjectNameToProjectPath = SettingToDictionary(Properties.Settings.Default.MapProjectNameToProjectPath);
            mapProjectNameToProjectPath.Remove(projectName);
            Properties.Settings.Default.MapProjectNameToProjectPath = SettingFromDictionary(mapProjectNameToProjectPath);
            Properties.Settings.Default.Save();

            return false;
        }

        /// <summary>
        /// Call this method before you release the project object to make sure that all the data is stored
        /// </summary>
        public void SaveProjectData()
        {
            while (SaveNeeded)
                SaveData();
        }

        /// <summary>
        /// Call this method to save the project information (typically only used internally)
        /// </summary>
        /// <param name="project">The project object</param>
        public static void SaveProject(CscProject project)
        {
            string strFilename = project.ProjectDirectory;
            if (!Directory.Exists(strFilename))
                Directory.CreateDirectory(strFilename);

            strFilename = Path.Combine(strFilename, $"{project.Name}.{ProjectFileExt}");
#if true
            CscProjectStore.TransferFromCscProject(strFilename, project);
#else
            FileStream fs = new FileStream(strFilename, FileMode.Create);

            // Construct a SoapFormatter and use it 
            // to serialize the data to the stream.
#if !SaveSoapFormatter
            BinaryFormatter formatter = new BinaryFormatter();
#else
            SoapFormatter formatter = new SoapFormatter();
#endif
            try
            {
                formatter.Serialize(fs, project);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Failed to save! Reason: " + ex.Message);
            }
            finally
            {
                fs.Close();
            }
#endif
        }

        /// <summary>
        /// The name of the project
        /// </summary>
        public string Name
        {
            get { return _projectName; }
            internal set { _projectName = value; }
        }

        internal string VernacularScriptSystemName
        {
            get { return _scriptSystemName; }
            set { _scriptSystemName = value; }
        }

        /// <summary>
		/// The font to be used for the display of data in the project
		/// If you call this from the client application, then be sure to call 'SaveProject' to make it retentive
		/// </summary>
        public Font Font
        {
            get { return _projectFont; }
            set { _projectFont = value; }
        }

        public Font TransliterationFont
        {
            get { return _transliteratorFont; }
            set { _transliteratorFont = value; }
        }

        /// <summary>
        /// get or set the value which indicates whether the project should also compare word which differ on
        /// the presence or absense of double consonants (e.g. "kk" and "k" will be treated identically).
        /// </summary>
        public bool CompareDoubleChars
        {
            get { return _compareDoubleChars; }
            set { _compareDoubleChars = value; }
        }

        /// <summary>
        /// get or set the value which indicates whether the project should also compare words which differ on
        /// the basis of case (e.g. "K" and "k" will be treated identically).
        /// </summary>
        public bool CompareCase
        {
            get { return _compareCase; }
            set { _compareCase = value; }
        }

        public CultureInfo Locale
        {
            get { return _locale; }
            internal set { _locale = value; }
        }

        /// <summary>
        /// get or set the value of how the maximum number of words to include before and after the word we're checking
        /// (e.g. '3' means upto 3 words before and 3 words after the word)
        /// </summary>
        public int WordsInContext
        {
            get { return _wordsInContext; }
            set { _wordsInContext = value; }
        }

        /// <summary>
        /// get or set the value of how the maximum number of context lines to include in the tooltip
        /// (use '-1' to display all)
        /// </summary>
        public int MaxContextStrings
        {
            get { return _maxContextStrings; }
            set { _maxContextStrings = value; }
        }

        internal void ResetAmbiguityRules()
        {
            _ruleNames.Clear();
            m_mapAmbiguities = new AmbiguityListMaps();
        }

        /// <summary>
        /// Call this method to add the given word to the dictionary of known good words
        /// </summary>
        /// <param name="strGoodWord">The word to add to the dictionary</param>
        /// <param name="Context">Use this optional parameter to provide a context for the word (to show in the tooltip of the Spell Check dialog</param>
        public void AddToDictionary(string strGoodWord, List<string> context, bool dontSave = false)
        {
            AddToDictionary(new SpellFixerWord(this, strGoodWord, context));
            if (!dontSave)
                SaveProjectData();
        }

        internal void AddToDictionary(SpellFixerWord sfw, bool dontSave = false)
        {
            string strWord = sfw.Value;
            if (!m_mapS2SfwKnownGoodWords.ContainsKey(strWord))
            {
                m_mapS2SfwKnownGoodWords.Add(sfw);
                m_eSaveNeeded |= SaveDbFlags.KnownGoodWords;
            }

            if (m_mapS2SfwWordsToCheck.ContainsKey(strWord))
            {
                m_mapS2SfwWordsToCheck.Remove(strWord);
                m_eSaveNeeded |= SaveDbFlags.CheckWords;
            }
        }

        internal virtual SpellFixerWord GetNewSpellFixerWord(string strWord, string context)
        {
            return new SpellFixerWord(this, strWord, new List<string> { context });
        }

        /// <summary>
        /// This method brings up a dialog with all of the words-to-check in a grid for the user to choose
        /// them in bulk.
        /// </summary>
        public void QueryForSpellingCorrectionsBulk()
        {
            WordsToCheckBulk mapWordsToCheckBulk = new WordsToCheckBulk();
            try
            {
                // see if the store exists (i.e. if the user left off with some unselected.
                if (mapWordsToCheckBulk.BulkFileExists(ProjectDirectory))
                {
                    DialogResult res = MessageBox.Show("You have some data left over from a previous session. Do you want to work with that data (Yes) or start over from scratch (No)?", ApplicationCaption, MessageBoxButtons.YesNoCancel);

                    if (res == DialogResult.Yes)
                        mapWordsToCheckBulk.LoadTable(this);
                    else if (res == DialogResult.Cancel)
                        return;

                    // don't leave it lying around
                    mapWordsToCheckBulk.DeleteStore(ProjectDirectory);
                }

                // if it's empty at this point, then clone the WordsToCheck list
                if (mapWordsToCheckBulk.Count == 0)
                    mapWordsToCheckBulk.Copy(m_mapS2SfwWordsToCheck);

                // bring up the dialog to do the work
                var dlg = new BulkAmbiguityPicker(this, m_mapS2SfwKnownGoodWords, m_mapS2SBad2GoodWords,
                                                  ref mapWordsToCheckBulk);
                dlg.ShowDialog();
            }
            catch (Exception)
            {
                throw;
            }
            finally
            {
                if (mapWordsToCheckBulk.Count > 0)
                    WordsToCheckBulk.SaveTable(mapWordsToCheckBulk, this);
                SaveProjectData();
            }
        }

        /// <summary>
        /// Call this method to remove the given word from the dictionary of known good words
        /// </summary>
        /// <param name="strWord">The word to remove from the dictionary</param>
        public void RemoveFromDictionary(string strWord, bool dontSave = false)
        {
            this.m_mapS2SfwKnownGoodWords.Remove(strWord);
            m_eSaveNeeded |= SaveDbFlags.KnownGoodWords;
            if (!dontSave)
                SaveProjectData();
        }

        /// <summary>
        /// Call this method with a misspelled word and it will provide a dialog box to prompt the user
        /// for the corrected spelling.
        /// </summary>
        /// <param name="strBadWord">The misspelled word</param>
        public void AssignCorrectSpelling(string strBadWord)
        {
            // if it's already in the bad to good list, then use the former 'good' value as the replacement
            string strReplacement = strBadWord;
            if (m_mapS2SBad2GoodWords.ContainsKey(strBadWord))
                strReplacement = m_mapS2SBad2GoodWords[strBadWord];

            var aQuery = new QueryGoodSpelling(Font);
			if (aQuery.ShowDialog(strBadWord, strReplacement, strBadWord, false) == DialogResult.OK)
			{
				// UPDATE: 2026-04-14: I was passing 'false' for bNoUI, but since this always shows a dialog, it should be yes, right?
				AssignCorrectSpelling(aQuery.BadSpelling, aQuery.GoodSpelling, bNoUI: true, ContextForReplacement: null);
			}
        }

        /// <summary>
        /// Call this method with a misspelled word and its replacement and they will be added to the 
        /// fixup CC table. If used programmatically, you can also request no user-interface by passing
        /// 'true' for the optional third parameter (e.g. if the word is already in the dictionary there
        /// will otherwise be a dialog box informing the user)
        /// </summary>
        /// <param name="strBadWord">The misspelled word</param>
        /// <param name="strReplacement">The corrected spelling</param>
        /// <param name="bNoUI">'true' to suppress message boxes. Used for when the misspelled word is already in the dictionary (asking the user if they'd like to remove it from the dictionary)</param>
        public void AssignCorrectSpelling(string strBadWord, string strReplacement, 
                                          bool bNoUI, List<string> ContextForReplacement, bool dontSave = false)
        {
            // make sure the bad word isn't already in the known good list
            if (m_mapS2SfwKnownGoodWords.ContainsKey(strBadWord))
            {
                if (!bNoUI)
                {
                    DialogResult res = MessageBox.Show(String.Format("The word '{0}' is already marked as a good word. Would you like to remove it from the good word list?", strBadWord), ApplicationCaption, MessageBoxButtons.YesNoCancel);
                    if (res != DialogResult.Yes)
                        return;
                }

                // otherwise
                RemoveFromDictionary(strBadWord, dontSave);
            }

            // in any case, the replacement must be part of the known good list
            AddToDictionary(strReplacement, ContextForReplacement, dontSave);

            // if the bad word was in the WordsToCheck list, then remove it
            if (m_mapS2SfwWordsToCheck.ContainsKey(strBadWord))
            {
                m_mapS2SfwWordsToCheck.Remove(strBadWord);
                m_eSaveNeeded |= SaveDbFlags.CheckWords;
                if (!dontSave)
                    SaveProjectData();
            }

            // see if a bad2good mapping already exists
            if (m_mapS2SBad2GoodWords.ContainsKey(strBadWord))
            {
                // if this exact match is already there, then we're done.
                if (m_mapS2SBad2GoodWords[strBadWord] == strReplacement)
                    return;

                if (!bNoUI)
                {
                    DialogResult res = MessageBox.Show(String.Format("The word '{0}' is already mapped to the corrected spelling '{1}'. Do you want to change this to the new corrected spelling '{2}'?",
                        strBadWord, m_mapS2SBad2GoodWords[strBadWord], strReplacement), ApplicationCaption, MessageBoxButtons.YesNoCancel);
                    if (res != DialogResult.Yes)
                        return;
                }

                // otherwise
                m_mapS2SBad2GoodWords.Remove(strBadWord);
            }

            // finally, add the list
            m_mapS2SBad2GoodWords.Add(strBadWord, strReplacement);
            m_eSaveNeeded |= SaveDbFlags.Bad2GoodWords; // to indicate that it needs to be saved

            if (!dontSave)  // but some callers don't want to save right away (since they're doing a batch)
                SaveBad2GoodMap();
        }

        /// <summary>
        /// If the user edited the replacement rules outside of this application, call this method to reload it
        /// (e.g. we call this when we're in the SpellCorrection dialog and the user clicks the 'Refresh' button)
        /// </summary>
        public void ReplacementRulesCheckForOutsideChange()
        {
            m_mapS2SBad2GoodWords.CheckForOutOfDate();
        }

        /// <summary>
        /// Call this method to bring up the Fix Spelling dialog box for the replacement rule which 
        /// changes the given word
        /// </summary>
        /// <param name="strBadWord">The word to search for the replacement rule (only for the 'bad spelling' form)</param>
        public void FindReplacementRule(string strBadWord)
        {
            strBadWord = TrimWhiteSpaceAndPunctuation(strBadWord);
            if (String.IsNullOrEmpty(strBadWord))  // don't bother if it's uninteresting
                return;

            bool bFound = false;
            string strWordKey = null;

            // first see if it matches a good value (in which case, the user ought to change it
            strWordKey = m_mapS2SBad2GoodWords.FirstOrDefault(kvp => kvp.Value == strBadWord).Key;
            if (!String.IsNullOrEmpty(strWordKey))
            {
                bFound = true;
            }
            else if (m_mapS2SBad2GoodWords.ContainsKey(strBadWord))
            {
                bFound = true;
                strWordKey = strBadWord;
            }
            // otherwise, go thru the words one-by-one and see if they're inside the given word (somewhere)
            //  e.g. suppose a rule exists for 'नास' => 'नाश', it will show a change for 'नास-बान' if a rule 
            //  for 'नास-बान' doesn't exist. Solution is to add a rule for 'नास-बान' => 'नास-बान' so it'll 
            //  supercede the other rule. (if you didn't want it to be changed, to 'नाश-बान', though you probably would)
            else
            {
                // currently, the only way this can happen is if the key word is a partial 
                //  match on one side or the other of a hyphen. So split on hyphens and check all parts
                var separator = new char[] { '-' };
                foreach (string strKey in m_mapS2SBad2GoodWords.Keys)
                {
                    var astrParts = strBadWord.Split(separator, StringSplitOptions.RemoveEmptyEntries);
                    if (astrParts.Any(part => strKey == part))
                    {
                        bFound = true;
                        strWordKey = strKey;
                        break;
                    }
                }
            }

            // if we found it...
            if (bFound)
            {
                var strReplacement = m_mapS2SBad2GoodWords[strWordKey];
                QueryGoodSpelling aQuery = new QueryGoodSpelling(Font);
                var res = aQuery.ShowDialog(strWordKey, strReplacement, strWordKey, true);
                if ((res == DialogResult.OK) && (strReplacement != aQuery.GoodSpelling))
                {
                    // means fixed the spelling of the replacement (or bad spelling?)
                    AssignCorrectSpelling(aQuery.BadSpelling, aQuery.GoodSpelling, bNoUI: false, ContextForReplacement: null);
                }
                else if (res == DialogResult.Abort) // means delete the rule
                {
                    m_mapS2SBad2GoodWords.Remove(strWordKey);
                    SaveBad2GoodMap();
                }
                else if (res == DialogResult.Retry) // means swap the words
                {
                    m_mapS2SBad2GoodWords.Remove(strWordKey);
                    AssignCorrectSpelling(strReplacement, strWordKey, bNoUI: false, ContextForReplacement: null);
                }
            }
            else
            {
                // none found
                MessageBox.Show(String.Format("There are no substitution rules that apply to the word '{0}'!", strBadWord), ApplicationCaption);
            }
        }

        /// <summary>
        /// Call this method to bring up a table with the full list of spelling fixes to edit
        /// </summary>
        public void EditSpellingFixes()
        {
            if (m_mapS2SBad2GoodWords.Count == 0)
            {
                MessageBox.Show("There are no spelling fix pairs in this project!", ApplicationCaption);
            }
            else
            {
                EditDictionaryOrBad2GoodList(false);
            }
        }

        protected void EditDictionaryOrBad2GoodList(bool bEditKnownGoodList)
        {
            ViewBadGoodPairsDlg dlg = new ViewBadGoodPairsDlg(this, ref m_mapS2SfwKnownGoodWords, ref m_mapS2SBad2GoodWords, _projectFont, bEditKnownGoodList);
            if (dlg.ShowDialog() == DialogResult.OK)
            {
                m_eSaveNeeded |= (SaveDbFlags.KnownGoodWords | SaveDbFlags.Bad2GoodWords);

                // in case the known good list changed, remove any words that were in the Words to Check
                foreach (string strGoodWord in m_mapS2SfwKnownGoodWords.Keys)
                    if (m_mapS2SfwWordsToCheck.ContainsKey(strGoodWord))
                    {
                        m_mapS2SfwWordsToCheck.Remove(strGoodWord);
                        m_eSaveNeeded |= SaveDbFlags.CheckWords;
                    }

                // all good words in the bad-to-good list must be in the dictionary... (or something
                //  internal isn't working correctly)
                bool bDoItAgain = false;
                do
                {
                    bDoItAgain = false;
                    foreach (KeyValuePair<string, string> kvp in m_mapS2SBad2GoodWords)
                        if (!m_mapS2SfwKnownGoodWords.ContainsKey((string)kvp.Value))
                        {
                            System.Diagnostics.Debug.Assert(false);

                            // Nevertheless, something should be done about this situation.
                            // The most likely scenerio is that it *was* in the dictionary and it was deleted.
                            // Let's assume that the 'fix' rule is now obsolete and remove it also.
                            // [can't remove an item from a collection that you're iterating over, so do it in
                            //  stages--thus the outer do-while loop]
                            bDoItAgain = true;
                            m_mapS2SBad2GoodWords.Remove(kvp.Key);
                            m_eSaveNeeded |= SaveDbFlags.Bad2GoodWords;
                            break;
                        }
                } while (bDoItAgain);

                SaveProjectData();
            }
        }

        /// <summary>
        /// Call this method to bring up a table with the list of known correctly spelled words to edit
        /// </summary>
        public void EditDictionary()
        {
            if (m_mapS2SfwKnownGoodWords.Count == 0)
            {
                MessageBox.Show("There are no words in the dictionary!", ApplicationCaption);
            }
            else
            {
                EditDictionaryOrBad2GoodList(true);
            }
        }

        internal void AddAmbiguityRule(string strRuleName, string strNormalizedForm, List<string> astrAmbiguousForms)
        {
            ArrayList aList = new ArrayList();
            if (m_mapAmbiguities.ContainsKey(strNormalizedForm))
                m_mapAmbiguities[strNormalizedForm].AddRange(astrAmbiguousForms);
            else
            {
                aList.AddRange(astrAmbiguousForms);
                m_mapAmbiguities.Add(strNormalizedForm, aList);
            }
        }

        internal bool ContainsAmbiguityRule(string strRuleName)
        {
            return _ruleNames.Contains(strRuleName);
        }

        internal void AddAmbiguityRuleName(string strRuleName)
        {
            if (!_ruleNames.Contains(strRuleName))
                _ruleNames.Add(strRuleName);
        }

        /// <summary>
        /// Call this to reset the count of words in the Words to check list (basically, whenever
        /// you plan to send the full corpus of words again from scratch).
        /// </summary>
        public void ResetCheckList()
        {
            m_mapS2SfwWordsToCheck.Clear();
            m_eSaveNeeded |= SaveDbFlags.CheckWords;
        }

        public bool AddParatextSpellingStatusToCheckLists(string filespec) 
        {
            if (!File.Exists(filespec))
                return false;

            var doc = new XmlDocument();
            doc.Load(filespec);

            var statusNodes = doc.SelectNodes("/SpellingStatus/Status");
            if (statusNodes == null)
                return false;

            foreach (XmlNode statusNode in statusNodes)
            {
                var wordAttr = statusNode.Attributes?["Word"]?.Value;
                var stateAttr = statusNode.Attributes?["State"]?.Value;

                if (string.IsNullOrEmpty(wordAttr) || string.IsNullOrEmpty(stateAttr))
                    continue;

                List<string> context = null;
                // Try to get context from m_mapS2SfwWordsToCheck if available
                if (m_mapS2SfwWordsToCheck.ContainsKey(wordAttr))
                    context = m_mapS2SfwWordsToCheck[wordAttr].ContextStrings;

                if (stateAttr == "R")
                {
                    AddToDictionary(wordAttr, context, true);
                }
                else if (stateAttr == "W")
                {
                    var correctionNode = statusNode.SelectSingleNode("Correction");
                    if (correctionNode != null)
                    {
                        var goodWord = correctionNode.InnerText;
                        List<string> goodContext = null;
                        if (m_mapS2SfwWordsToCheck.ContainsKey(goodWord))
                            goodContext = m_mapS2SfwWordsToCheck[goodWord].ContextStrings;

                        AssignCorrectSpelling(wordAttr, goodWord, true, goodContext, true);
                    }
                }
            }

            SaveProjectData();
            return true;
        }

        /// <summary>
        /// Call this method to add a word to the collection of words that will be checked against each
        /// other. This collection is retentive across use, so if you intend for the full list to be
        /// created each time, then you should call the ResetCheckList() method prior to this method 
        /// (repeatedly) to add the words.
        /// </summary>
        /// <param name="strWord">The word to add to the check list (e.g. किताब)</param>
        /// <param name="bTrim">Flag indicating whether this routine should trim whitespace on either end of the word</param>
        /// <param name="Context">Optional context (string) parameter if you'd like that to be stored with the word also</param>
        /// <returns>WordCheckResult enumeration indicating the result of adding the word (e.g. could have already been in the list, etc)</returns>
        public WordCheckResult AddWordToCheckList(string strWord, bool bTrim, List<string> context)
        {
            if (!String.IsNullOrEmpty(strWord))
            {
                if (bTrim)
                    strWord = TrimWhiteSpaceAndPunctuation(strWord);
                if (String.IsNullOrEmpty(strWord))
                    return WordCheckResult.Unknown;
                if (m_mapS2SfwKnownGoodWords.ContainsKey(strWord))
                {
                    AddNewContextToCollection(strWord, context, m_mapS2SfwKnownGoodWords);
                    return WordCheckResult.InGoodWordDictionary;
                }
                else if (m_mapS2SBad2GoodWords.ContainsKey(strWord))
                    return WordCheckResult.InBadWordDictionary;
                else
                {
                    if (!AddNewContextToCollection(strWord, context, m_mapS2SfwWordsToCheck))
                    {
                        m_mapS2SfwWordsToCheck.Add(new SpellFixerWord(this, strWord, context));
                    }
                }
            }
            return WordCheckResult.Unknown;
        }

        private bool AddNewContextToCollection(string strWord, List<string> context, HashtableStore collection)
        {
            if (collection.TryGetValue(strWord, out SpellFixerWord sfw))
            {
                if ((context != null) && (context.Count > 0) && 
                    (sfw.ContextStrings.Count < MaxContextStrings))
                {
                    var newContexts = context.Except(sfw.ContextStrings).ToList();
                    if (newContexts.Count > 0)
                        sfw.ContextStrings.AddRange(newContexts);
                    sfw.Count += newContexts.Count;
                }
                return true;
            }
            return false;
        }

        internal static bool DoesFileExist(string strFileName, ref DateTime TimeModified)
        {
            bool bRet = true;

            try
            {
                FileInfo fi = new FileInfo(strFileName);
                TimeModified = fi.LastWriteTime;
                bRet = fi.Exists;
            }
            catch
            {
                bRet = false;
            }

            return bRet;
        }

        internal string TrimWhiteSpaceAndPunctuation(string word)
        {
            if (string.IsNullOrWhiteSpace(word))
                return string.Empty;

            // Normalize Unicode text for consistent representation
            word = word.Normalize(NormalizationForm.FormKC);

            List<char> trimChars = m_achTrimPunctuation;
            int start = 0, end = word.Length - 1;

            static bool ShouldTrim(char ch, List<char> trimChars)
            {
                return ch <= 0x0020
                    || char.IsDigit(ch)
                    || char.IsPunctuation(ch)
                    || char.IsWhiteSpace(ch)
                    || trimChars.Contains(ch);
            }

            // Trim from start
            while (start <= end && ShouldTrim(word[start], trimChars))
                start++;

            // Trim from end
            while (end >= start && ShouldTrim(word[end], trimChars))
                end--;

            return (start > end)
                ? string.Empty
                : word.Substring(start, end - start + 1);
        }

        /// <summary>
        /// Call this method to retrieve the words which are ambiguously related to the given word according
        /// to the ambiguity rules given in the projects "Vernacular Script System" definition. The result 
        /// can then be used by the client application to display for the user to choose the correct value
        /// or it can be passed back to the 'QueryForSpellingCorrection' to have this assembly query the user
        /// Sub-class of this class may redefine how the list of ambiguous words is determined
        /// </summary>
        /// <param name="strWordToCheck">The word to search for ambiguities of</param>
        /// <returns>A sorted array of SpellFixerWord objects with the highest count occuring as the lowest array index (i.e. [0] = highest count)</returns>
        public virtual SpellFixerWords GetAmbiguousWords(string strWordToCheck)
        {
            if (!String.IsNullOrEmpty(strWordToCheck))
            {
                string strSimplifiedWord = SimplifiedWord(strWordToCheck, true);

                CountPlusNameSortList sl = new CountPlusNameSortList();
                FindMatchingSimplifiedFormsInHashTableValue(strWordToCheck, strSimplifiedWord,
                    m_mapS2SfwKnownGoodWords, true, ref sl);

                FindMatchingSimplifiedFormsInHashTableValue(strWordToCheck, strSimplifiedWord,
                    m_mapS2SfwWordsToCheck, false, ref sl);

                if (sl.Count > 0)
                {
                    SpellFixerWords astrListAmbs = new SpellFixerWords();
                    foreach (SpellFixerWord sfw in sl.Values)
                        astrListAmbs.Insert(0, sfw);
                    return astrListAmbs;
                }
            }

            return null;
        }

        /// <summary>
        /// Call this method to retrieve the words which are ambiguously related to the given word and
        /// you want the "Bundle" to *include* the given word as well. (c.f. GetAmbiguousWords)
        /// </summary>
        /// <param name="sfwWordToCheck">The word to represent the ambiguous words in a bundle. This object will be included in the returned results</param>
        /// <returns>A sorted array of SpellFixerWord objects with the highest count occuring as the lowest array index (i.e. [0] = highest count)</returns>
        internal virtual SpellFixerWords GetAmbiguousBundle(SpellFixerWord sfwWordToCheck, WordsToCheckBulk mapWordsToCheck)
        {
            string strWordToCheck = sfwWordToCheck.Value;
            if (!String.IsNullOrEmpty(strWordToCheck))
            {
                string strSimplifiedWord = SimplifiedWord(strWordToCheck, true);

                CountPlusNameSortList sl = new CountPlusNameSortList();
                FindMatchingSimplifiedFormsInHashTableValue(strWordToCheck, strSimplifiedWord,
                    m_mapS2SfwKnownGoodWords, true, ref sl);

                FindMatchingSimplifiedFormsInHashTableValue(strWordToCheck, strSimplifiedWord,
                    mapWordsToCheck, false, ref sl);

                if (sl.Count > 0)
                {
                    // if the user wants us to add the word in question, do that here:
                    sl.Add(sfwWordToCheck, m_mapS2SfwKnownGoodWords.ContainsKey(strWordToCheck));

                    SpellFixerWords astrListAmbs = new SpellFixerWords();
                    foreach (SpellFixerWord sfw in sl.Values)
                        astrListAmbs.Insert(0, sfw);

                    // if there's only 1, then it's not really ambiguous
                    return (astrListAmbs.Count > 1) ? astrListAmbs : null;
                }
            }

            return null;
        }

        /*
        internal WordsToCheckBulk CloneWordsToCheck
        {
            get 
            {
                System.Diagnostics.Debug.Assert((m_mapS2SfwWordsToCheckBulk != null) && (m_mapS2SfwWordsToCheckBulk.Count == 0), "This shouldn't be called while there are some values left from before");
                return m_mapS2SfwWordsToCheckBulk.Clone(this); 
            }
        }
        */

        /// <summary>
        /// Call this method to get a collection of SpellFixerWords (collection) objects representing
        /// all the words in the Words-to-Check list that are ambiguous with each other in bundles
        /// </summary>
        /// <returns>SpellFixerWordBundles collection of SpellFixerWords (collection) objects</returns>
        public virtual SpellFixerWordBundles GetAmbiguousBundles()
        {
            // this is a simplification for Mark's Word Macro so that we can return to him bundles of
            //	ambiguities
            SpellFixerWordBundles sfwb = new SpellFixerWordBundles();

            // make a copy so we don't muck with the existing one
            WordsToCheckBulk mapWordsToCheckBulk = new WordsToCheckBulk();
            mapWordsToCheckBulk.Copy(m_mapS2SfwWordsToCheck);

            try
            {
                do
                {
                    foreach (SpellFixerWord sfwWord in mapWordsToCheckBulk.Values)
                    {
                        // get all the words ambiguous with this word
                        SpellFixerWords sfws = GetAmbiguousBundle(sfwWord, mapWordsToCheckBulk);

                        if (sfws != null)
                        {
                            System.Diagnostics.Trace.WriteLine(String.Format("Found Bundle: {0}, {1} ambiguities, {2} more to check",
                                sfwWord.Value, sfws.Count, mapWordsToCheckBulk.Count));

                            // add this bundle to the collection
                            sfwb.Add(sfws);

                            // remove all the words in this bundle from the list of words to check (so 
                            //  we don't hit any of them again)
                            foreach (SpellFixerWord sfwInBundle in sfws)
                                if (mapWordsToCheckBulk.ContainsKey(sfwInBundle.Value))
                                    mapWordsToCheckBulk.Remove(sfwInBundle.Value);
                        }
                        else
                            mapWordsToCheckBulk.Remove(sfwWord.Value);

                        break;  // every time (but this is still easier than the alternative
                    }
                } while (mapWordsToCheckBulk.Count > 0);
            }
            catch (Exception ex)
            {
                throw ex;
            }

            return sfwb;
        }

        internal void FindMatchingSimplifiedFormsInHashTableValue(string strWordToCheck,
            string strSimplifiedWord, HashtableStore ht, bool bMatchIndicatesKnownGoodWord,
            ref CountPlusNameSortList sl)
        {
            var startingChar = strSimplifiedWord[0];
            foreach (var kvp in ht.Where(kvp => kvp.Value.SimplifiedValue.FirstOrDefault() == startingChar))
            {
                SpellFixerWord aWord = kvp.Value;
                if ((strSimplifiedWord == aWord.SimplifiedValue) && (strWordToCheck != kvp.Key))
                {
                    sl.Add(aWord, bMatchIndicatesKnownGoodWord);
                }
            }

            //foreach (var kvp in ht)
            //{
            //    SpellFixerWord aWord = kvp.Value;
            //    if ((strSimplifiedWord == aWord.SimplifiedValue)
            //        && (strWordToCheck != kvp.Key))
            //    {
            //        sl.Add(aWord, bMatchIndicatesKnownGoodWord);
            //    }
            //}
        }

        internal virtual string SimplifiedWord(string strWord, bool bTransliterate)
        {
            string strSimplifiedWord = strWord;

            // see if we're supposed to ignore case
            if (CompareCase && (Locale != null))
                strSimplifiedWord = strSimplifiedWord.ToLower(Locale);

            // then transliterate the word for the rest of the simplifications
            strSimplifiedWord = (bTransliterate) ? TransliteratedWord(strSimplifiedWord) : strSimplifiedWord;

            // might do the ignore case *afterwards*
            if (CompareCase && (Locale == null))
                strSimplifiedWord = strSimplifiedWord.ToLowerInvariant();

            // see if we're supposed to strip out consecutive double-characters
            if (CompareDoubleChars)
            {
                if (strSimplifiedWord.Length > 1)
                {
                    var sb = new StringBuilder();
                    var chLast = strSimplifiedWord[0];
                    sb.Append(chLast);

                    for (int i = 1; i < strSimplifiedWord.Length; i++)
                    {
                        var chCurrent = strSimplifiedWord[i];
                        if (chCurrent != chLast)
                        {
                            sb.Append(chCurrent);
                        }
                        chLast = chCurrent;
                    }
                    strSimplifiedWord = sb.ToString();
                }
            }

            // go thru each of the ambiguity rules and reduce the word further.
            if (m_mapAmbiguities != null)
                foreach (string strKey in m_mapAmbiguities.Keys)
                    foreach (string strValue in m_mapAmbiguities[strKey])
                        strSimplifiedWord = strSimplifiedWord.Replace(strValue, strKey);

            // we now have a simplified word to check against our list
            return strSimplifiedWord;
        }

        internal DirectableEncConverter Transliterator
        {
            get { return m_aEcTrans; }
            set { m_aEcTrans = value; }
        }

        public bool HasPreprocessRegex
        {
            get { return RegexChanges.Any(); }
        }

        internal virtual string TransliteratedWord(string strWordVernacular)
        {
            // First remove any "before transliteration" strings
            if (m_astrIgnoreStrsBeforeTransliteration != null)
            {
                foreach (string strIgnoreString in m_astrIgnoreStrsBeforeTransliteration)
                    strWordVernacular = strWordVernacular.Replace(strIgnoreString, null);
            }

            string strTransliterated = null;
            if ((Transliterator != null) && (!String.IsNullOrEmpty(strWordVernacular)))
            {
                strTransliterated = Transliterator.Convert(strWordVernacular);

                // apply the "CharactersToIgnore" to strip out unexpected/desired results of the transliteration
                if ((m_astrIgnoreStrsAfterTransliteration != null) && (strTransliterated != null))
                    foreach (string strIgnoreString in m_astrIgnoreStrsAfterTransliteration)
                        strTransliterated = strTransliterated.Replace(strIgnoreString, null);
            }

            return strTransliterated;
        }

        public string PreprocessData(string data)
        {
            var result = data;
            foreach (var regexChangeTuple in RegexChanges)
            {
                result = regexChangeTuple.Item1.Replace(result, regexChangeTuple.Item2);
            }
            return result;
        }
    }

    internal sealed class CscProjectDeserializationBinder : SerializationBinder
    {
        public override Type BindToType(string assemblyName, string typeName)
        {
            // If a client app is trying to serialize in an older version, then just redirect it to this 
            //  assembly name (we can do this because there's no change to the actual content; just a difference
            //  in the version name)
            if (assemblyName == "SpellingFixer30, Version=3.0.0.2, Culture=neutral, PublicKeyToken=f1447bae1e63f485")
            {
                assemblyName = System.Reflection.Assembly.GetExecutingAssembly().FullName;
                System.Diagnostics.Debug.Assert(assemblyName == "SpellingFixer30, Version=3.0.0.3, Culture=neutral, PublicKeyToken=f1447bae1e63f485");
            }

            return Type.GetType(String.Format("{0}, {1}", typeName, assemblyName));
        }
    }
}
