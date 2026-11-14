using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Windows.Forms;
using System.Xml;
using System.Xml.Schema;
using System.Xml.Serialization;

namespace SpellingFixer30
{
    [XmlRoot("CscProject")]
    public class CscProjectStore
    {
        [XmlAttribute("ProjectName")]
        public string ProjectName { get; set; }

        [XmlAttribute("ScriptSystemName")]
        public string ScriptSystemName { get; set; }

        [XmlAttribute("CustomCcCode")]
        public string CustomCcCode { get; set; }

        [XmlAttribute("CompareDoubleChars")]
        public bool CompareDoubleChars { get; set; }

        [XmlAttribute("CompareCase")]
        public bool CompareCase { get; set; }

        [XmlAttribute("WordsInContext")]
        public int WordsInContext { get; set; }

        [XmlAttribute("MaxContextStringsInToolTip")]
        public int MaxContextStrings { get; set; }

        /// <summary>
        /// The Paratext project (if any) whose 'shared' folder this project's data is synchronized through
        /// </summary>
        [XmlAttribute("ParatextProjectName")]
        public string ParatextProjectName { get; set; }

        /// <summary>
        /// When the Inconsistency Settings were last analyzed (or the offer to analyze them was declined), as yyyy-MM-dd
        /// </summary>
        [XmlAttribute("LastRuleAnalysis")]
        public string LastRuleAnalysis { get; set; }

        internal const string LastRuleAnalysisFormat = "yyyy-MM-dd";

        internal static DateTime? ParseLastRuleAnalysis(string value)
        {
            return DateTime.TryParseExact(value, LastRuleAnalysisFormat, CultureInfo.InvariantCulture, DateTimeStyles.None, out var date)
                    ? date
                    : (DateTime?)null;
        }

        internal static string FormatLastRuleAnalysis(DateTime? value)
        {
            return value?.ToString(LastRuleAnalysisFormat, CultureInfo.InvariantCulture);
        }

        [XmlElement("ProjectFont")]
        public SerializableFont ProjectFontData;

        [XmlIgnore]
        public Font ProjectFont
        {
            get => ProjectFontData?.ToFont() ?? new Font("Arial Unicode MS", 14f, FontStyle.Regular, GraphicsUnit.Point);
            set => ProjectFontData = new SerializableFont(value);
        }

        [XmlElement("TransliteratorFont")]
        public SerializableFont TransliteratorFontData;

        [XmlIgnore]
        public Font TransliteratorFont
        {
            get => TransliteratorFontData?.ToFont() ?? new Font("Arial Unicode MS", 14f, FontStyle.Regular, GraphicsUnit.Point);
            set => TransliteratorFontData = new SerializableFont(value);
        }

        [XmlElement("CultureInfo")]
        public SerializableCultureInfo LocaleData;

        [XmlIgnore]
        public CultureInfo Locale
        {
            get => LocaleData?.ToCultureInfo() ?? CultureInfo.InvariantCulture;
            set => LocaleData = new SerializableCultureInfo(value);
        }

        [XmlElement("RuleNames")]
        public RuleNameData RuleNames { get; set; }

        /// <summary>
        /// The (keys of the) Inconsistency Settings analysis suggestions the user asked not to be shown again
        /// </summary>
        [XmlArray("DismissedRuleSuggestions")]
        [XmlArrayItem("Suggestion")]
        public List<string> DismissedRuleSuggestions { get; set; }

        [XmlElement("MarkersToNotCheck")]
        public MarkersToNotCheckData MarkersToNotCheck { get; set; }

        public XmlSchema GetSchema()
        {
            return null; // No schema provided
        }

        public static CscProjectStore LoadFromXml(string filePath)
        {
            XmlSerializer serializer = new XmlSerializer(typeof(CscProjectStore));
            using (FileStream fs = new FileStream(filePath, FileMode.Open, FileAccess.Read))
            {
                return (CscProjectStore)serializer.Deserialize(fs);
            }
        }

        public void SaveToXml(string filePath)
        {
            while (!CanWriteToFile(filePath))
            {
                if (MessageBox.Show($"Can't write to the file {filePath}. But you can (manually) delete it, so it'll be recreated.", CscProject.ApplicationCaption, MessageBoxButtons.OKCancel) == DialogResult.Cancel)
                    return;
            }

            var serializer = new XmlSerializer(typeof(CscProjectStore));
            using (var writer = System.IO.File.Create(filePath))
            {
                serializer.Serialize(writer, this);
            }
        }

        public static void TransferToCscProject(string filePath, CscProject project)
        {
            var store = LoadFromXml(filePath);
            if (store == null || project == null)
                throw new ArgumentNullException();

            // Transfer simple properties
            project.Name = store.ProjectName;
            project.VernacularScriptSystemName = store.ScriptSystemName;
            project.SpellFixerCustomCcCode = store.CustomCcCode;
            project.CompareDoubleChars = store.CompareDoubleChars;
            project.CompareCase = store.CompareCase;
            project.Font = store.ProjectFont;
            project.TransliterationFont = store.TransliteratorFont;
            project.Locale = store.Locale;
            project.WordsInContext = store.WordsInContext;
            project.MaxContextStrings = store.MaxContextStrings;
            project.ParatextProjectName = store.ParatextProjectName;
            project.LastRuleAnalysis = ParseLastRuleAnalysis(store.LastRuleAnalysis);
            project.DismissedRuleSuggestions = new HashSet<string>(store.DismissedRuleSuggestions ?? new List<string>());
            project.MarkersToAvoidForSpellingCheck = store.MarkersToNotCheck?.MarkersToNotCheck ?? new MarkersToNotCheckData().MarkersToNotCheck;

            // Transfer RuleNames
            project.ResetAmbiguityRules();
            if (store.RuleNames != null)
            {
                foreach (var ruleName in store.RuleNames.RuleNames)
                {
                    if (ruleName is string strRuleName)
                        project.AddAmbiguityRuleName(strRuleName);
                }
            }
        }

        public static CscProjectStore TransferFromCscProject(string filePath, CscProject project)
        {
            if (project == null)
                throw new ArgumentNullException(nameof(project));

            var store = new CscProjectStore
            {
                ProjectName = project.Name,
                ScriptSystemName = project.VernacularScriptSystemName,
                CustomCcCode = null,    // the custom code lives in the .cct file itself (see Bad2GoodMap.SaveTable); it's only read from older project files
                CompareDoubleChars = project.CompareDoubleChars,
                CompareCase = project.CompareCase,
                WordsInContext = project.WordsInContext,
                MaxContextStrings = project.MaxContextStrings,
                ParatextProjectName = String.IsNullOrEmpty(project.ParatextProjectName) ? null : project.ParatextProjectName,
                LastRuleAnalysis = FormatLastRuleAnalysis(project.LastRuleAnalysis),
                DismissedRuleSuggestions = ((project.DismissedRuleSuggestions?.Count ?? 0) > 0) ? project.DismissedRuleSuggestions.OrderBy(k => k).ToList() : null,

                ProjectFont = project.Font,
                TransliteratorFont = project.TransliterationFont,
                Locale = project.Locale,
                RuleNames = new RuleNameData(),
                MarkersToNotCheck = (project.MarkersToAvoidForSpellingCheck != null)
                    ? new MarkersToNotCheckData { MarkersToNotCheck = project.MarkersToAvoidForSpellingCheck }
                    : new MarkersToNotCheckData()   // to get the default set
            };

            // Transfer RuleNames
            // _ruleNames is protected, so use ContainsAmbiguityRule and AddAmbiguityRuleName if needed
            // If you have a public getter for rule names, use that. Otherwise, you may need to expose it.
            var ruleNamesField = project.GetType().GetField("_ruleNames", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
            if (ruleNamesField != null)
            {
                var ruleNames = ruleNamesField.GetValue(project) as IEnumerable;
                if (ruleNames != null)
                {
                    foreach (var ruleName in ruleNames)
                    {
                        if (ruleName is string strRuleName)
                            store.RuleNames.RuleNames.Add(strRuleName);
                    }
                }
            }

            store.SaveToXml(filePath);
            return store;
        }

        /// <summary>
        /// Updates only the ParatextProjectName attribute of an existing project file (so the rest of the
        /// project's settings, which the user may be in the middle of editing, aren't saved yet)
        /// </summary>
        public static void UpdateParatextProjectName(string filePath, string paratextProjectName)
        {
            UpdateProjectFile(filePath, store => store.ParatextProjectName = String.IsNullOrEmpty(paratextProjectName) ? null : paratextProjectName);
        }

        /// <summary>
        /// Updates only the LastRuleAnalysis attribute of an existing project file
        /// </summary>
        public static void UpdateLastRuleAnalysis(string filePath, DateTime? lastRuleAnalysis)
        {
            UpdateProjectFile(filePath, store => store.LastRuleAnalysis = FormatLastRuleAnalysis(lastRuleAnalysis));
        }

        /// <summary>
        /// Updates only the DismissedRuleSuggestions of an existing project file
        /// </summary>
        public static void UpdateDismissedRuleSuggestions(string filePath, IEnumerable<string> keys)
        {
            var list = keys?.OrderBy(k => k).ToList();
            UpdateProjectFile(filePath, store => store.DismissedRuleSuggestions = ((list?.Count ?? 0) > 0) ? list : null);
        }

        private static void UpdateProjectFile(string filePath, Action<CscProjectStore> update)
        {
            if (!File.Exists(filePath))
                return;

            var store = LoadFromXml(filePath);
            if (store == null)
                return;

            update(store);
            store.SaveToXml(filePath);
        }

        public static bool CanWriteToFile(string filePath)
        {
            try
            {
                if (!File.Exists(filePath))
                    return true;

                using (FileStream fs = new FileStream(filePath, FileMode.Open, FileAccess.Write))
                {
                    // Successfully opened for write.
                    return true;
                }
            }
            catch (UnauthorizedAccessException)
            {
                return false;
            }
            catch (IOException)
            {
                return false;
            }
        }
    }

    public class MarkersToNotCheckData
    {
        [XmlElement("MarkersToNotCheck")]
        public HashSet<string> MarkersToNotCheck = ["\\id", "\\c", "\\mi", "\\r", "\\toc3", "\\tr"];
    }

    public class RuleNameData
    {
        [XmlElement("RuleName")]
        public List<string> RuleNames = [];
    }

    public class SerializableFont
    {
        [XmlAttribute("FontName")]
        public string Name { get; set; }

        [XmlAttribute("FontSize")]
        public float Size { get; set; }

        [XmlAttribute("FontStyle")]
        public FontStyle Style { get; set; }

        [XmlAttribute("FontUnit")]
        public GraphicsUnit Unit { get; set; }

        public SerializableFont() { }

        public SerializableFont(Font font)
        {
            Name = font.Name;
            Size = font.Size;
            Style = font.Style;
            Unit = font.Unit;
        }

        public Font ToFont()
        {
            return new Font(Name, Size, Style, Unit);
        }
    }

    public class SerializableCultureInfo
    {
        [XmlAttribute]
        public string Name { get; set; }

        public SerializableCultureInfo() { }

        public SerializableCultureInfo(CultureInfo culture)
        {
            Name = culture?.Name;
        }

        public CultureInfo ToCultureInfo()
        {
            return string.IsNullOrEmpty(Name) ? CultureInfo.InvariantCulture : new CultureInfo(Name);
        }
    }
}