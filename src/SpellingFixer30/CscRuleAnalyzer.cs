using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using SilEncConverters40;

namespace SpellingFixer30
{
    internal enum SuggestionKind
    {
        TurnOffRule,        // an Inconsistency Setting that rarely (or never) helps find the kinds of inconsistencies the user fixes
        TurnOnRule,         // an Inconsistency Setting in the VSS file the project doesn't use, which looks like it would now help
        MoveRule,           // an Inconsistency Setting that would help more if it were applied before the others
        AddRule,            // a new Inconsistency Setting, learned from the user's corrections
        CleanUpWordLists,   // entries in the word lists with SFM markup or untrimmed punctuation in them
        RemoveWords,        // entries in the Words to Check list in a different script (e.g. English from milestone attributes)
        PreprocessRegex,    // a regular expression to remove markup (e.g. attributes) before the text is split into words
        IgnoreCharacter,    // a character inside words that should be ignored (e.g. soft hyphen)
        PunctuationCharacter// a character at the edges of words that isn't being trimmed
    }

    /// <summary>
    /// A change to a word list entry: replace 'Word' with the (to-be-trimmed) Replacements (or remove it if there are none)
    /// </summary>
    internal class WordListFix
    {
        public string Word { get; set; }
        public bool InKnownGoodList { get; set; }
        public List<string> Replacements { get; set; } = new List<string>();
    }

    internal class RuleSuggestion
    {
        public SuggestionKind Kind { get; set; }
        public string Title { get; set; }
        public string Explanation { get; set; }
        public bool Recommended { get; set; }

        // for the rule suggestions (-1 if not applicable)
        public int CorrectionsGained { get; set; } = -1;
        public int CorrectionsLost { get; set; } = -1;
        public int WordsNewlyBundled { get; set; } = -1;
        public double FractionCorrected { get; set; } = -1;

        public string DistinctionName { get; set; }             // TurnOffRule, TurnOnRule
        public Distinction NewDistinction { get; set; }         // AddRule
        public string Character { get; set; }                   // IgnoreCharacter, PunctuationCharacter
        public RegexChange Regex { get; set; }                  // PreprocessRegex
        public List<WordListFix> WordListFixes { get; set; }    // CleanUpWordLists, RemoveWords

        public List<string> ExampleCorrections { get; set; } = new List<string>();
        public List<string> PossibleInconsistencies { get; set; } = new List<string>();
        public List<string> ExampleWords { get; set; } = new List<string>();

        internal HashSet<int> CaughtCorrections { get; set; } = new HashSet<int>();

        public bool IsRuleChange => (Kind == SuggestionKind.TurnOffRule) || (Kind == SuggestionKind.TurnOnRule) ||
                                    (Kind == SuggestionKind.MoveRule) || (Kind == SuggestionKind.AddRule);

        /// <summary>
        /// A short, plain-language heading (e.g. "Spellings that differ by ◌ै / ◌े")
        /// </summary>
        public string Heading { get; set; }

        /// <summary>
        /// The plain-language yes/no question to ask the user
        /// </summary>
        public string Question { get; set; }

        /// <summary>
        /// A sentence or two, in plain language, of why (e.g. "It would have found 210 more of your corrections...")
        /// </summary>
        public string Note { get; set; }

        /// <summary>
        /// What will be done if the user says yes (e.g. "Turn off the 'Nukta' setting"), for the summary
        /// </summary>
        public string Action { get; set; }

        /// <summary>
        /// For a rule change: the most common difference(s) in the vernacular spelling of the corrections it finds
        /// (e.g. "◌ै / ◌े"), since the rule itself may be in terms of the transliteration (e.g. 'ai' ~ 'ē')
        /// </summary>
        public string VernacularDifference { get; set; }

        /// <summary>
        /// Identifies the suggestion across analyses (e.g. to not show it again if the user asked not to)
        /// </summary>
        public string Key
        {
            get
            {
                switch (Kind)
                {
                    case SuggestionKind.AddRule:
                        var rule = NewDistinction?.NormalizeRules?.FirstOrDefault();
                        var forms = String.Join("|", rule?.CharactersToNormalize?.CharacterToNormalize?.Select(c => c.Value) ?? Enumerable.Empty<string>());
                        return $"{Kind}:{(rule?.IsBeforeTransliteration == true ? "before" : "after")}:{forms}>{rule?.NormalizedForm}";
                    case SuggestionKind.TurnOffRule:
                    case SuggestionKind.TurnOnRule:
                    case SuggestionKind.MoveRule:
                        return $"{Kind}:{DistinctionName}";
                    case SuggestionKind.PreprocessRegex:
                        return $"{Kind}:{Regex?.Pattern}";
                    case SuggestionKind.IgnoreCharacter:
                    case SuggestionKind.PunctuationCharacter:
                        return $"{Kind}:U+{(int)(Character ?? " ")[0]:X4}";
                    default:
                        return Kind.ToString();
                }
            }
        }

        public string ExamplesCaption
        {
            get
            {
                switch (Kind)
                {
                    case SuggestionKind.TurnOffRule:
                        return "Corrections you've made that this setting found (they wouldn't be found without it):";
                    case SuggestionKind.TurnOnRule:
                    case SuggestionKind.MoveRule:
                    case SuggestionKind.AddRule:
                        return "Corrections you've made that this would have found:";
                    default:
                        return "For example:";
                }
            }
        }

        public string GroupsCaption =>
            (Kind == SuggestionKind.TurnOffRule)
                ? "Groups of words this setting puts together in which nothing has been corrected (if these really are different words, they're false alarms):"
                : "It would also put these words together, though none of them have been corrected (some may be real inconsistencies you haven't found yet; others will be false alarms):";

        /// <summary>
        /// The examples to show first (the corrections or the words affected)
        /// </summary>
        public List<string> Examples => IsRuleChange ? ExampleCorrections : ExampleWords;

        public string Details
        {
            get
            {
                // (data first: that's what most users can evaluate best)
                var sb = new StringBuilder();
                sb.AppendLine(Heading ?? Title);
                if (Examples.Any())
                {
                    sb.AppendLine();
                    sb.AppendLine(ExamplesCaption);
                    foreach (var ex in Examples)
                        sb.AppendLine("    " + ex);
                }
                if (PossibleInconsistencies.Any())
                {
                    sb.AppendLine();
                    sb.AppendLine(GroupsCaption);
                    foreach (var ex in PossibleInconsistencies)
                        sb.AppendLine("    " + ex);
                }
                sb.AppendLine();
                sb.AppendLine(Title);
                if (!String.IsNullOrEmpty(Explanation))
                    sb.AppendLine(Explanation);
                return sb.ToString();
            }
        }
    }

    internal class RuleAnalysisResult
    {
        public string ProjectName { get; set; }
        public string ScriptSystemName { get; set; }
        public int NumCorrections { get; set; }
        public int NumCorrectionsBundledNow { get; set; }
        public int NumCorpusWords { get; set; }
        public double TypicalFractionCorrected { get; set; }

        /// <summary>the typical number of corrections a current setting finds per word it adds to bundles</summary>
        public double TypicalYield { get; set; }
        public bool TooFewCorrections { get; set; }
        public List<RuleSuggestion> Suggestions { get; } = new List<RuleSuggestion>();

        /// <summary>
        /// Possible setting changes that weren't suggested at all, because they'd show the user far too many words that
        /// are fine for each mistake they'd find (or would lose too many of the corrections found now). They're only in
        /// the report (and they're re-tried each time the analysis is run, in case the data changes the picture)
        /// </summary>
        public List<RuleSuggestion> LeftOut { get; } = new List<RuleSuggestion>();
        public List<RuleSuggestion> RuleAudit { get; } = new List<RuleSuggestion>();   // every enabled rule (for the report)

        public string Summary
        {
            get
            {
                var sb = new StringBuilder();
                sb.Append($"Of the {NumCorrections} spelling corrections you've made in the '{ProjectName}' project, the current Inconsistency Settings " +
                          $"would have bundled {NumCorrectionsBundledNow} ({Percent(NumCorrectionsBundledNow, NumCorrections)}) together with their correct spelling. ");
                if (TooFewCorrections)
                    sb.Append("There are too few corrections yet to tell whether a setting is unhelpful, so none are suggested to be turned off (but they'll be re-checked each time you analyze). ");
                sb.Append($"Typically, {TypicalFractionCorrected:P0} of the words a setting adds to bundles are ones you've ended up correcting.");
                return sb.ToString();
            }
        }

        internal static string Percent(int n, int d) => (d == 0) ? "0%" : ((double)n / d).ToString("P1", CultureInfo.CurrentCulture);

        public string ToReport()
        {
            var sb = new StringBuilder();
            sb.AppendLine($"Inconsistency Settings analysis of '{ProjectName}' (Vernacular Script System: '{ScriptSystemName}') on {DateTime.Now:yyyy-MM-dd}");
            sb.AppendLine(Summary);
            sb.AppendLine($"(Words in the Known Good and Words to Check lists: {NumCorpusWords})");
            sb.AppendLine();
            sb.AppendLine("Current settings (corrections that need it / words it adds to bundles / % of those corrected):");
            foreach (var rule in RuleAudit)
                sb.AppendLine($"    {rule.DistinctionName,-30} {rule.CorrectionsLost,6} {rule.WordsNewlyBundled,7} {FormatFraction(rule.FractionCorrected),6}");
            foreach (var group in Suggestions.GroupBy(s => s.Kind))
            {
                sb.AppendLine();
                sb.AppendLine($"=== {group.Key} ===");
                foreach (var s in group)
                {
                    sb.AppendLine($"[{(s.Recommended ? "x" : " ")}] {s.Title}");
                    if (s.IsRuleChange)
                        sb.AppendLine($"      corrections: +{Math.Max(0, s.CorrectionsGained)} / -{Math.Max(0, s.CorrectionsLost)}; words newly bundled: {s.WordsNewlyBundled}; corrected: {FormatFraction(s.FractionCorrected)}");
                    sb.AppendLine($"      {s.Heading}");
                    foreach (var line in s.Details.Split(new[] { Environment.NewLine }, StringSplitOptions.None).Skip(1))
                        if (!String.IsNullOrWhiteSpace(line))
                            sb.AppendLine("      " + line.Trim());
                }
            }
            if (LeftOut.Count > 0)
            {
                sb.AppendLine();
                sb.AppendLine("=== Not suggested (too many words shown for each mistake found, or too many corrections lost) ===");
                foreach (var s in LeftOut)
                    sb.AppendLine($"    {s.Title}: corrections +{Math.Max(0, s.CorrectionsGained)} / -{Math.Max(0, s.CorrectionsLost)}; words newly bundled: {s.WordsNewlyBundled}");
            }
            return sb.ToString();
        }

        internal static string FormatFraction(double f) => (f < 0) ? "-" : f.ToString("P0", CultureInfo.CurrentCulture);
    }

    /// <summary>
    /// Analyzes a project's Inconsistency Settings (the Distinctions in its Vernacular Script System) against the
    /// spelling corrections the user has made (the bad -> good pairs) and the words in the Known Good and Words to
    /// Check lists, to suggest settings to turn off or on, new settings to add, and data to clean up.
    ///
    /// The basic measure is: a correction is "bundled" if the bad and good spellings simplify to the same form (so the
    /// Bulk picker would have shown them together). A setting's value is how many corrections need it; its cost is how
    /// many words it puts into bundles (that wouldn't otherwise be in one), and of those, the fraction the user has
    /// actually corrected (or chosen as a correction) is a rough measure of its precision.
    /// </summary>
    internal class CscRuleAnalyzer
    {
        private const char Zwj = '\u200D';
        private const char Zwnj = '\u200C';
        private const char SoftHyphen = '\u00AD';   // (a hyphenation hint; it's to be ignored, not treated as a word break)

        // configuration
        private readonly VernScriptSystem _vss;
        private readonly List<Distinction> _distinctions;
        private readonly HashSet<string> _enabled;
        private readonly bool _compareDoubleChars;
        private readonly bool _ignoreCase;
        private readonly CultureInfo _locale;
        private readonly string[] _ignoreBefore;
        private readonly string[] _ignoreAfter;
        private readonly Func<IList<string>, IList<string>> _transliterateBatch;
        private readonly Func<string, string> _trim;

        // data
        private readonly string _projectName;
        private readonly List<string[]> _corrections = new List<string[]>();          // [bad, good]
        private readonly Dictionary<string, int> _corpus = new Dictionary<string, int>(StringComparer.Ordinal);   // word -> count
        private readonly HashSet<string> _knownGood = new HashSet<string>(StringComparer.Ordinal);
        private readonly Dictionary<string, List<string>> _contexts = new Dictionary<string, List<string>>(StringComparer.Ordinal);
        private readonly HashSet<string> _correctedWords = new HashSet<string>(StringComparer.Ordinal);
        private readonly List<string> _allWords;

        // caches
        private readonly Dictionary<string, string> _transliterations = new Dictionary<string, string>(StringComparer.Ordinal);
        private readonly Dictionary<string, Dictionary<string, string>> _baseStringsByPreRules = new Dictionary<string, Dictionary<string, string>>();

        internal CscRuleAnalyzer(string projectName, VernScriptSystem vss, IEnumerable<string> enabledDistinctionNames,
                                 bool compareDoubleChars, bool ignoreCase, CultureInfo locale,
                                 Func<IList<string>, IList<string>> transliterateBatch, Func<string, string> trim,
                                 IEnumerable<KeyValuePair<string, string>> corrections,
                                 IEnumerable<SpellFixerWord> knownGoodWords, IEnumerable<SpellFixerWord> wordsToCheck)
        {
            _projectName = projectName;
            _vss = vss;
            _distinctions = vss?.Distinctions?.DistinctionList ?? new List<Distinction>();
            _enabled = new HashSet<string>(enabledDistinctionNames ?? Enumerable.Empty<string>());
            _compareDoubleChars = compareDoubleChars;
            _ignoreCase = ignoreCase;
            _locale = locale;
            _ignoreBefore = CscProject.GetIgnoreStrings(vss?.IgnoreCharactersBeforeTransliteration?.Characters);
            _ignoreAfter = CscProject.GetIgnoreStrings(vss?.IgnoreCharactersAfterTransliteration?.Characters);
            _transliterateBatch = transliterateBatch;
            _trim = trim;

            foreach (var kvp in corrections ?? Enumerable.Empty<KeyValuePair<string, string>>())
            {
                if (String.IsNullOrEmpty(kvp.Key) || String.IsNullOrEmpty(kvp.Value) || (kvp.Key == kvp.Value))
                    continue;
                _corrections.Add(new[] { kvp.Key, kvp.Value });
                _correctedWords.Add(kvp.Key);
                _correctedWords.Add(kvp.Value);
            }

            void AddWords(IEnumerable<SpellFixerWord> words, bool isKnownGood)
            {
                foreach (var sfw in words ?? Enumerable.Empty<SpellFixerWord>())
                {
                    if (String.IsNullOrEmpty(sfw?.Value))
                        continue;
                    _corpus[sfw.Value] = (_corpus.TryGetValue(sfw.Value, out var n) ? n : 0) + Math.Max(1, sfw.Count);
                    if (isKnownGood)
                        _knownGood.Add(sfw.Value);
                    if ((sfw.ContextStrings != null) && !_contexts.ContainsKey(sfw.Value))
                        _contexts[sfw.Value] = sfw.ContextStrings;
                }
            }
            AddWords(knownGoodWords, true);
            AddWords(wordsToCheck, false);

            _allWords = _corpus.Keys.Union(_correctedWords).ToList();
        }

        /// <summary>
        /// Creates an analyzer for the project, using the (possibly edited, not yet saved) settings from the project editor
        /// </summary>
        internal static CscRuleAnalyzer Create(CscProject project, VernScriptSystem vss, IEnumerable<string> enabledDistinctionNames,
                                               bool compareDoubleChars, bool ignoreCase, DirectableEncConverter transliterator)
        {
            if ((project.m_mapS2SBad2GoodWords == null) || (project.m_mapS2SfwKnownGoodWords == null) || (project.m_mapS2SfwWordsToCheck == null))
                throw new ApplicationException("The project's data hasn't been loaded");

            return new CscRuleAnalyzer(project.Name, vss, enabledDistinctionNames, compareDoubleChars, ignoreCase, project.Locale,
                                       CreateBatchTransliterator(transliterator ?? project.Transliterator),
                                       project.TrimWhiteSpaceAndPunctuation,
                                       project.m_mapS2SBad2GoodWords.ToList(),
                                       project.m_mapS2SfwKnownGoodWords.Values.ToList(),
                                       project.m_mapS2SfwWordsToCheck.Values.ToList());
        }

        /// <summary>
        /// Creates an analyzer from a (copy of a) project folder, without loading the project (e.g. for trying it out
        /// on a sandbox copy of a project's data)
        /// </summary>
        internal static CscRuleAnalyzer CreateForFolder(string projectFolder, string vssFilePath)
        {
            var csf = Directory.GetFiles(projectFolder, $"*.{CscProject.ProjectFileExt}").FirstOrDefault()
                      ?? throw new ApplicationException($"No project (.{CscProject.ProjectFileExt}) file in {projectFolder}");
            var store = CscProjectStore.LoadFromXml(csf);
            if (vssFilePath == null)
            {
                // (a project-specific VSS file in the project folder takes precedence over the global one)
                var projectSpecific = Path.Combine(projectFolder, $"{store.ScriptSystemName}.{CscProject.VernacularScriptSystemFileExt}");
                vssFilePath = CscProject.IsVernScriptSystemFile(projectSpecific)
                                ? projectSpecific
                                : CscProject.CreateVernacularScriptSystemFileSpec(store.ScriptSystemName);
            }
            var vss = VernScriptSystem.LoadVsvXml(vssFilePath);

            var cct = Directory.GetFiles(projectFolder, "*.cct").FirstOrDefault();
            var corrections = (cct != null) ? Bad2GoodMap.LoadTable(cct).ToList() : new List<KeyValuePair<string, string>>();

            List<SpellFixerWord> LoadWords(string filename)
            {
                var path = Path.Combine(projectFolder, filename);
                if (!File.Exists(path))
                    return new List<SpellFixerWord>();
                return SpellFixerHashtableStore.LoadFromXml(path).Items
                            .Select(i => new SpellFixerWord { Value = i.Key, Count = i.Count, ContextStrings = i.ContextList?.Contexts })
                            .ToList();
            }

            var trimChars = (vss.ExtraPunctuation?.IgnorePunctuations ?? new List<IgnorePunctuation>())
                                .Where(p => !String.IsNullOrEmpty(p.Value)).Select(p => p.Value[0]).ToList();

            return new CscRuleAnalyzer(store.ProjectName, vss, store.RuleNames?.RuleNames, store.CompareDoubleChars, store.CompareCase,
                                       store.Locale, CreateBatchTransliterator(CscProject.GetTransliterator(vss)),
                                       w => CscProject.TrimWhiteSpaceAndPunctuation(w, trimChars),
                                       corrections, LoadWords(KnownGoodWordList.MyFilename), LoadWords(WordsToCheck.MyFilename));
        }

        internal static Func<IList<string>, IList<string>> CreateBatchTransliterator(DirectableEncConverter transliterator)
        {
            if (transliterator == null)
                return words => words.ToList();

            return words =>
            {
                // transliterating a batch of words (one per line) at once is much faster than one at a time
                var results = new List<string>(words.Count);
                const int batchSize = 500;
                for (int i = 0; i < words.Count; i += batchSize)
                {
                    var batch = words.Skip(i).Take(batchSize).ToList();
                    string[] converted = null;
                    if (!batch.Any(w => w.Contains("\n")))
                    {
                        try
                        {
                            converted = (transliterator.Convert(String.Join("\n", batch)) ?? String.Empty).Split('\n');
                        }
                        catch { }
                    }
                    if ((converted == null) || (converted.Length != batch.Count))
                        converted = batch.Select(w => SafeConvert(transliterator, w)).ToArray();
                    results.AddRange(converted.Select(c => c?.TrimEnd('\r')));
                }
                return results;
            };
        }

        private static string SafeConvert(DirectableEncConverter transliterator, string word)
        {
            try
            {
                return transliterator.Convert(word);
            }
            catch
            {
                return word;
            }
        }

        #region Computing the simplified forms

        private List<Distinction> CurrentDistinctions => _distinctions.Where(d => _enabled.Contains(d.Name)).ToList();

        private AmbiguityRules Rules(IEnumerable<Distinction> distinctionsInOrder) => AmbiguityRules.Build(distinctionsInOrder);

        private static string Signature(IEnumerable<AmbiguityRule> rules) => String.Join("\u0001", rules.Select(r => r.ToString()));

        /// <summary>
        /// For each word: the form after the pre-transliteration rules, ignore strings, transliteration, case and double
        /// character simplification--i.e. what the post-transliteration rules are applied to (cached by pre-transliteration rules)
        /// </summary>
        private Dictionary<string, string> BaseStrings(AmbiguityRules rules)
        {
            var signature = Signature(rules.BeforeTransliteration);
            if (_baseStringsByPreRules.TryGetValue(signature, out var bases))
                return bases;

            var prepared = _allWords.ToDictionary(w => w, w => CscProject.RemoveStrings(rules.ApplyBeforeTransliteration(w), _ignoreBefore), StringComparer.Ordinal);
            EnsureTransliterated(prepared.Values);

            bases = new Dictionary<string, string>(StringComparer.Ordinal);
            foreach (var kvp in prepared)
                bases[kvp.Key] = Simplify(CscProject.RemoveStrings(_transliterations[kvp.Value], _ignoreAfter));

            _baseStringsByPreRules[signature] = bases;
            return bases;
        }

        private void EnsureTransliterated(IEnumerable<string> preparedWords)
        {
            var needed = preparedWords.Where(w => !_transliterations.ContainsKey(w)).Distinct().ToList();
            if (needed.Count == 0)
                return;
            var results = _transliterateBatch(needed);
            for (int i = 0; i < needed.Count; i++)
                _transliterations[needed[i]] = results[i] ?? String.Empty;
        }

        private string Simplify(string transliterated)
        {
            var str = transliterated ?? String.Empty;
            if (_ignoreCase)
                str = (_locale != null) ? str.ToLower(_locale) : str.ToLowerInvariant();
            if (_compareDoubleChars)
                str = CscProject.CollapseDoubleCharacters(str);
            return str;
        }

        /// <summary>
        /// The result of applying a set of rules to all the words
        /// </summary>
        private class Evaluation
        {
            public Dictionary<string, string> Keys;
            public Dictionary<string, int> GroupSizes;     // (of corpus words only)
            public bool[] Bundled;                          // per correction
            public int NumBundled;
        }

        private Evaluation Evaluate(IEnumerable<Distinction> distinctionsInOrder)
        {
            var rules = Rules(distinctionsInOrder);
            var bases = BaseStrings(rules);
            var keys = new Dictionary<string, string>(bases.Count, StringComparer.Ordinal);
            foreach (var kvp in bases)
                keys[kvp.Key] = rules.ApplyAfterTransliteration(kvp.Value);

            var groupSizes = new Dictionary<string, int>(StringComparer.Ordinal);
            foreach (var word in _corpus.Keys)
            {
                var key = keys[word];
                if (!String.IsNullOrEmpty(key))
                    groupSizes[key] = (groupSizes.TryGetValue(key, out var n) ? n : 0) + 1;
            }

            var eval = new Evaluation { Keys = keys, GroupSizes = groupSizes, Bundled = new bool[_corrections.Count] };
            for (int i = 0; i < _corrections.Count; i++)
            {
                var kb = keys[_corrections[i][0]];
                if (!String.IsNullOrEmpty(kb) && (kb == keys[_corrections[i][1]]))
                {
                    eval.Bundled[i] = true;
                    eval.NumBundled++;
                }
            }
            return eval;
        }

        private int GroupSize(Evaluation eval, string word)
        {
            var key = eval.Keys[word];
            return (!String.IsNullOrEmpty(key) && eval.GroupSizes.TryGetValue(key, out var n)) ? n : 0;
        }

        /// <summary>
        /// Fills in the suggestion's measures, comparing the evaluation with the rule ('with') to the one without it.
        /// For turning a rule off, the change is from 'with' to 'without' (so what the rule finds is what's lost);
        /// otherwise, it's from 'without' to 'with'.
        /// </summary>
        private void Measure(RuleSuggestion suggestion, Evaluation with, Evaluation without)
        {
            var ruleFinds = new List<int>();
            var rulePrevents = new List<int>();
            for (int i = 0; i < _corrections.Count; i++)
            {
                if (with.Bundled[i] && !without.Bundled[i])
                    ruleFinds.Add(i);
                else if (!with.Bundled[i] && without.Bundled[i])
                    rulePrevents.Add(i);
            }

            var isTurnOff = (suggestion.Kind == SuggestionKind.TurnOffRule);
            suggestion.CorrectionsGained = isTurnOff ? rulePrevents.Count : ruleFinds.Count;
            suggestion.CorrectionsLost = isTurnOff ? ruleFinds.Count : rulePrevents.Count;
            suggestion.CaughtCorrections = new HashSet<int>(ruleFinds);
            suggestion.ExampleCorrections = ruleFinds.Take(8).Select(i => $"{_corrections[i][0]}  →  {_corrections[i][1]}").ToList();

            // the words this rule puts into bundles, which wouldn't otherwise be in one
            var pulled = _corpus.Keys.Where(w => (GroupSize(with, w) > 1) && (GroupSize(without, w) <= 1)).ToList();
            suggestion.WordsNewlyBundled = pulled.Count;
            suggestion.FractionCorrected = (pulled.Count > 0) ? (double)pulled.Count(w => _correctedWords.Contains(w)) / pulled.Count : -1;

            // the bundles (with the rule) that those words are in, in which nothing has been corrected
            var keysOfPulled = new HashSet<string>(pulled.Select(w => with.Keys[w]), StringComparer.Ordinal);
            var members = new Dictionary<string, List<string>>(StringComparer.Ordinal);
            foreach (var word in _corpus.Keys)
            {
                var key = with.Keys[word];
                if (!String.IsNullOrEmpty(key) && keysOfPulled.Contains(key))
                {
                    if (!members.TryGetValue(key, out var list))
                        members[key] = list = new List<string>();
                    list.Add(word);
                }
            }
            suggestion.PossibleInconsistencies = members.Values
                .Where(list => !list.Any(w => _correctedWords.Contains(w)))
                .OrderByDescending(list => list.Sum(w => _corpus[w]))
                .Take(8)
                .Select(list => String.Join("  ~  ", list.OrderByDescending(w => _corpus[w]).Take(4)))
                .ToList();
        }

        #endregion

        #region Finding the differences in the corrections the current rules don't bundle

        /// <summary>
        /// Aligns the two strings (longest common subsequence) and returns the runs of differences
        /// (e.g. "apabicai" / "apabicē" -> [ ("ai","ē") ])
        /// </summary>
        internal static List<string[]> Differences(string a, string b, bool keepCombiningMarksWithBase)
        {
            int n = a.Length, m = b.Length;
            var lcs = new int[n + 1, m + 1];
            for (int i = n - 1; i >= 0; i--)
                for (int j = m - 1; j >= 0; j--)
                    lcs[i, j] = (a[i] == b[j]) ? lcs[i + 1, j + 1] + 1 : Math.Max(lcs[i + 1, j], lcs[i, j + 1]);

            var result = new List<string[]>();
            int ia = 0, ib = 0, startA = 0, startB = 0;
            bool inDiff = false;

            void Flush(int endA, int endB)
            {
                if (!inDiff)
                    return;
                // keep a combining mark together with its base character (e.g. 'm' + U+0310)
                if (keepCombiningMarksWithBase && (startA > 0) && (startB > 0) &&
                    (IsCombining(a, startA, endA) || IsCombining(b, startB, endB)))
                {
                    startA--;
                    startB--;
                }
                result.Add(new[] { a.Substring(startA, endA - startA), b.Substring(startB, endB - startB) });
                inDiff = false;
            }

            while ((ia < n) && (ib < m))
            {
                if (a[ia] == b[ib])
                {
                    Flush(ia, ib);
                    ia++;
                    ib++;
                    continue;
                }
                if (!inDiff)
                {
                    inDiff = true;
                    startA = ia;
                    startB = ib;
                }
                if (lcs[ia + 1, ib] >= lcs[ia, ib + 1])
                    ia++;
                else
                    ib++;
            }
            if ((ia < n) || (ib < m))
            {
                if (!inDiff)
                {
                    inDiff = true;
                    startA = ia;
                    startB = ib;
                }
                ia = n;
                ib = m;
            }
            Flush(ia, ib);
            return result;
        }

        private static bool IsCombining(string s, int start, int end)
        {
            if (start >= end)
                return false;
            var cat = CharUnicodeInfo.GetUnicodeCategory(s[start]);
            return (cat == UnicodeCategory.NonSpacingMark) || (cat == UnicodeCategory.EnclosingMark);
        }

        private class DifferenceStats
        {
            public string From;     // (ordinal-ordered pair)
            public string To;
            public HashSet<int> Corrections = new HashSet<int>();
            public int FromOnGoodSide;
            public int ToOnGoodSide;
        }

        private static string VernacularForDiff(string word)
        {
            return word.Normalize(NormalizationForm.FormKC).Replace(Zwj.ToString(), null).Replace(Zwnj.ToString(), null);
        }

        private List<DifferenceStats> CollectDifferences(IEnumerable<int> correctionIndices, Func<string, string> view,
                                                        bool keepCombiningMarksWithBase, Func<string, string, bool> isAlreadyEquivalent)
        {
            var stats = new Dictionary<string, DifferenceStats>(StringComparer.Ordinal);
            foreach (var i in correctionIndices)
            {
                var bad = view(_corrections[i][0]);
                var good = view(_corrections[i][1]);
                if ((bad == null) || (good == null) || (bad == good))
                    continue;

                var diffs = Differences(bad, good, keepCombiningMarksWithBase);
                if ((diffs.Count == 0) || (diffs.Count > 2))
                    continue;   // too different to learn a simple rule from

                foreach (var diff in diffs)
                {
                    string badPart = diff[0], goodPart = diff[1];
                    if (badPart.Any(Char.IsWhiteSpace) || goodPart.Any(Char.IsWhiteSpace))
                        continue;   // (joined/split words aren't something a rule can catch)
                    if (isAlreadyEquivalent(badPart, goodPart))
                        continue;

                    var ordered = String.CompareOrdinal(badPart, goodPart) <= 0;
                    var from = ordered ? badPart : goodPart;
                    var to = ordered ? goodPart : badPart;
                    var key = from + "\u0001" + to;
                    if (!stats.TryGetValue(key, out var stat))
                        stats[key] = stat = new DifferenceStats { From = from, To = to };
                    stat.Corrections.Add(i);
                    if (goodPart == from)
                        stat.FromOnGoodSide++;
                    else
                        stat.ToOnGoodSide++;
                }
            }
            return stats.Values.OrderByDescending(s => s.Corrections.Count).ToList();
        }

        private Distinction CreateDistinction(DifferenceStats stat, bool beforeTransliteration, int support, int numCorrections)
        {
            // normalize to the form that's more often in the correct spelling (so the simplified forms look like it)
            string form, normalizedForm;
            if (String.IsNullOrEmpty(stat.From) || String.IsNullOrEmpty(stat.To))
            {
                form = String.IsNullOrEmpty(stat.From) ? stat.To : stat.From;
                normalizedForm = String.Empty;
            }
            else if (stat.FromOnGoodSide >= stat.ToOnGoodSide)
            {
                form = stat.To;
                normalizedForm = stat.From;
            }
            else
            {
                form = stat.From;
                normalizedForm = stat.To;
            }

            var name = String.IsNullOrEmpty(normalizedForm)
                        ? $"with or without '{form}'"
                        : $"'{form}' ~ '{normalizedForm}'";
            if (beforeTransliteration)
                name += " (vernacular)";

            return new Distinction
            {
                Name = name,
                Description = $"Suggested by the Inconsistency Settings analysis on {DateTime.Today:yyyy-MM-dd}: this difference was in {support} of the " +
                              $"{numCorrections} spelling corrections that the other settings didn't find. ",
                NormalizeRules = new List<NormalizeRule>
                {
                    new NormalizeRule
                    {
                        Stage = beforeTransliteration ? NormalizeRule.StageBeforeTransliteration : null,
                        NormalizedForm = normalizedForm,
                        CharactersToNormalize = new CharactersToNormalize
                        {
                            CharacterToNormalize = new List<CharacterToNormalize> { new CharacterToNormalize { Value = form } }
                        }
                    }
                }
            };
        }

        #endregion

        /// <summary>
        /// Does the analysis
        /// </summary>
        /// <param name="progress">reports what's being done and the approximate percent complete</param>
        public RuleAnalysisResult Analyze(Action<string, int> progress, CancellationToken cancel)
        {
            progress ??= (s, n) => { };
            var result = new RuleAnalysisResult
            {
                ProjectName = _projectName,
                ScriptSystemName = _vss?.Properties?.Name,
                NumCorrections = _corrections.Count,
                NumCorpusWords = _corpus.Count,
                TooFewCorrections = _corrections.Count < 200
            };

            progress("Transliterating the words...", 2);
            var current = CurrentDistinctions;
            var evalCurrent = Evaluate(current);
            result.NumCorrectionsBundledNow = evalCurrent.NumBundled;
            cancel.ThrowIfCancellationRequested();

            // 1. how much does each of the current settings help (i.e. what happens if it's turned off)
            var audits = new List<RuleSuggestion>();
            int step = 0;
            foreach (var distinction in current)
            {
                progress($"Checking the '{distinction.Name}' setting...", 5 + (25 * step++ / Math.Max(1, current.Count)));
                var without = Evaluate(current.Where(d => d != distinction));
                var audit = new RuleSuggestion
                {
                    Kind = SuggestionKind.TurnOffRule,
                    DistinctionName = distinction.Name,
                    Title = $"Turn off '{distinction.Name}'",
                };
                Measure(audit, evalCurrent, without);
                audit.Explanation = $"Without this setting, {audit.CorrectionsLost} of your corrections wouldn't have been found " +
                                    $"(and {Math.Max(0, audit.CorrectionsGained)} would be found that currently aren't). " +
                                    $"It puts {audit.WordsNewlyBundled} words into bundles that they wouldn't otherwise be in, " +
                                    $"of which {RuleAnalysisResult.FormatFraction(audit.FractionCorrected)} have been part of a correction.";
                audits.Add(audit);
                result.RuleAudit.Add(audit);
                cancel.ThrowIfCancellationRequested();
            }

            // the typical precision (fraction of the words a setting bundles that get corrected) and yield (corrections
            //  found per word bundled) of the current settings are the bar the suggestions have to meet
            var measured = audits.Where(a => a.WordsNewlyBundled >= 10).ToList();
            var precisions = measured.Select(a => a.FractionCorrected).OrderBy(p => p).ToList();
            var typical = (precisions.Count > 0) ? precisions[precisions.Count / 2] : 0.2;
            var yields = measured.Select(a => (double)a.CorrectionsLost / a.WordsNewlyBundled).OrderBy(y => y).ToList();
            var typicalYield = (yields.Count > 0) ? yields[yields.Count / 2] : 0.2;
            result.TypicalFractionCorrected = typical;
            result.TypicalYield = typicalYield;
            var minUsefulGain = Math.Max(5, (int)Math.Ceiling(0.005 * _corrections.Count));
            var rarelyHelps = Math.Max(2, (int)(0.002 * _corrections.Count));
            bool IsWorthIt(RuleSuggestion s) => IsWorthItCriteria(s, minUsefulGain, typical, typicalYield);

            // (not recommended) new settings that would show the user far more words that are fine for each mistake they'd
            //  find than the current settings do, or that would lose a lot of the corrections found now, aren't suggested at
            //  all: users tend to trust the suggestions (and say 'Yes' to them), and settings like that bury the real
            //  inconsistencies in false alarms. (They're re-tried each time, so if the data changes, they may come back)
            bool IsTooNoisy(RuleSuggestion s) => !s.Recommended &&
                                                 ((s.CorrectionsLost > 0.25 * s.CorrectionsGained) ||
                                                  ((s.WordsNewlyBundled > 0) && ((double)s.CorrectionsGained / s.WordsNewlyBundled < 0.3 * typicalYield)));

            // since settings are applied in order, one that comes after another that changes the same characters may not
            //  get a chance to do anything (e.g. 'aiṁ' -> 'ē' after 'ṁ' -> 'n'): would it help more if it were applied first?
            var moved = new HashSet<string>();
            foreach (var distinction in current.Skip(1))
            {
                var withMoved = Evaluate(new[] { distinction }.Concat(current.Where(d => d != distinction)));
                if (withMoved.NumBundled - evalCurrent.NumBundled < 3)
                    continue;
                var suggestion = new RuleSuggestion
                {
                    Kind = SuggestionKind.MoveRule,
                    DistinctionName = distinction.Name,
                    Title = $"Move '{distinction.Name}' to the top (so it's applied before the other settings)",
                };
                Measure(suggestion, withMoved, evalCurrent);
                if (suggestion.CorrectionsGained - suggestion.CorrectionsLost < 3)
                    continue;

                // (moving an existing setting just lets it do what it was meant to, so the bar is lower than for a new one)
                suggestion.Recommended = (suggestion.CorrectionsLost <= 0.1 * suggestion.CorrectionsGained) &&
                                         ((suggestion.FractionCorrected < 0) || (suggestion.FractionCorrected >= 0.75 * typical));
                suggestion.Explanation = "Settings are applied in the order they're listed, so a setting can be prevented from working by one listed " +
                                         "before it that changes the same characters first. " +
                                         $"If '{distinction.Name}' were applied first, {suggestion.CorrectionsGained} more of your corrections would have been found" +
                                         (suggestion.CorrectionsLost > 0 ? $" (and {suggestion.CorrectionsLost} fewer)" : null) +
                                         $", and {suggestion.WordsNewlyBundled} more words would be put into bundles, of which " +
                                         $"{RuleAnalysisResult.FormatFraction(suggestion.FractionCorrected)} have been part of a correction.";
                result.Suggestions.Add(suggestion);
                moved.Add(distinction.Name);
            }

            foreach (var audit in audits.Where(a => !moved.Contains(a.DistinctionName)))
            {
                bool noEffect = (audit.CorrectionsLost == 0) && (audit.CorrectionsGained <= 0) && (audit.WordsNewlyBundled == 0);
                if (noEffect)
                {
                    audit.Recommended = true;
                    audit.Explanation = "This setting makes no difference to how any of the words in your data are compared " +
                                        "(e.g. because the characters it changes never occur at the point it's applied). " + audit.Explanation;
                }
                else if (result.TooFewCorrections)
                {
                    continue;
                }
                else if (audit.CorrectionsLost <= rarelyHelps)
                {
                    audit.Recommended = true;
                    audit.Explanation = "This setting rarely helps find the kinds of inconsistencies you've been correcting. " + audit.Explanation;
                }
                else if ((audit.CorrectionsLost < 0.01 * _corrections.Count) &&
                         (((audit.FractionCorrected >= 0) && (audit.FractionCorrected < typical / 2)) ||
                          ((audit.WordsNewlyBundled >= 10) && ((double)audit.CorrectionsLost / audit.WordsNewlyBundled < typicalYield / 4))))
                {
                    audit.Recommended = false;
                    audit.Explanation = "This setting mostly creates bundles of words you haven't needed to correct (false alarms). " + audit.Explanation;
                }
                else
                {
                    continue;
                }

                audit.Explanation += " Turning it off doesn't remove it from the Vernacular Script System file, so later analyses will " +
                                     "re-check whether newer data shows that it's worth turning back on.";
                result.Suggestions.Add(audit);
            }

            // 2. settings in the VSS file the project doesn't use: would they help now?
            var unused = _distinctions.Where(d => !_enabled.Contains(d.Name)).ToList();
            step = 0;
            foreach (var distinction in unused)
            {
                progress($"Checking the (unused) '{distinction.Name}' setting...", 30 + (10 * step++ / Math.Max(1, unused.Count)));
                var with = Evaluate(_distinctions.Where(d => _enabled.Contains(d.Name) || (d == distinction)));
                var suggestion = new RuleSuggestion
                {
                    Kind = SuggestionKind.TurnOnRule,
                    DistinctionName = distinction.Name,
                    Title = $"Turn on '{distinction.Name}'",
                };
                Measure(suggestion, with, evalCurrent);
                if (suggestion.CorrectionsGained - suggestion.CorrectionsLost < 1)
                    continue;
                suggestion.Recommended = IsWorthIt(suggestion);
                if (IsTooNoisy(suggestion))
                {
                    result.LeftOut.Add(suggestion);
                    continue;
                }
                suggestion.Explanation = $"This setting is in the '{result.ScriptSystemName}' Vernacular Script System, but this project doesn't use it. " +
                                         $"With it, {suggestion.CorrectionsGained} more of your corrections would have been found" +
                                         (suggestion.CorrectionsLost > 0 ? $" (and {suggestion.CorrectionsLost} fewer)" : null) +
                                         $". It would put {suggestion.WordsNewlyBundled} more words into bundles, of which " +
                                         $"{RuleAnalysisResult.FormatFraction(suggestion.FractionCorrected)} have been part of a correction.";
                result.Suggestions.Add(suggestion);
                cancel.ThrowIfCancellationRequested();
            }

            // 3. new settings, learned from the differences in the corrections the current settings don't bundle
            progress("Looking for patterns in the corrections the current settings don't find...", 40);
            var uncovered = Enumerable.Range(0, _corrections.Count).Where(i => !evalCurrent.Bundled[i]).ToList();
            var currentRules = Rules(current);
            var currentBases = BaseStrings(currentRules);
            var minSupport = Math.Max(3, (int)(0.002 * uncovered.Count));

            // a) in the transliterated form (as the existing settings are)
            var latinDiffs = CollectDifferences(uncovered, w => currentBases[w], true,
                                                (x, y) => currentRules.ApplyAfterTransliteration(x) == currentRules.ApplyAfterTransliteration(y))
                                .Where(s => s.Corrections.Count >= minSupport).Take(30).ToList();

            // b) in the vernacular form (i.e. a rule to apply before transliteration). Differences that are only in
            //  characters that are ignored before transliteration (e.g. a nukta) are already being ignored
            var vernDiffs = CollectDifferences(uncovered, VernacularForDiff, false,
                                               (x, y) => CscProject.RemoveStrings(x, _ignoreBefore) == CscProject.RemoveStrings(y, _ignoreBefore))
                                .Where(s => s.Corrections.Count >= minSupport).Take(20).ToList();

            var candidates = new List<RuleSuggestion>();
            var allDiffs = latinDiffs.Select(d => new { Diff = d, Before = false })
                                     .Concat(vernDiffs.Select(d => new { Diff = d, Before = true })).ToList();
            step = 0;
            foreach (var item in allDiffs)
            {
                progress("Trying out possible new settings...", 45 + (45 * step++ / Math.Max(1, allDiffs.Count)));
                var distinction = CreateDistinction(item.Diff, item.Before, item.Diff.Corrections.Count, uncovered.Count);

                // (put new settings first, so they apply before the existing settings change the characters involved)
                var with = Evaluate(new[] { distinction }.Concat(current));
                var suggestion = new RuleSuggestion
                {
                    Kind = SuggestionKind.AddRule,
                    NewDistinction = distinction,
                };
                Measure(suggestion, with, evalCurrent);
                if (suggestion.CorrectionsGained - suggestion.CorrectionsLost < 3)
                    continue;

                var rule = distinction.NormalizeRules[0];
                var form = rule.CharactersToNormalize.CharacterToNormalize[0].Value;
                var what = String.IsNullOrEmpty(rule.NormalizedForm)
                            ? $"words with and without '{form}'"
                            : $"words that differ only by '{form}' vs. '{rule.NormalizedForm}'";
                var example = (suggestion.ExampleCorrections.Count > 0) ? $" (e.g. {suggestion.ExampleCorrections[0]})" : null;
                suggestion.Title = $"Add '{distinction.Name}'{example}";
                suggestion.Explanation = $"A new setting to compare {what}" +
                                         (item.Before ? ", in the vernacular form of the word (i.e. before it's transliterated)" : ", in the transliterated form of the word") +
                                         $". With it, {suggestion.CorrectionsGained} more of your corrections would have been found" +
                                         (suggestion.CorrectionsLost > 0 ? $" (but {suggestion.CorrectionsLost} that are found now wouldn't be)" : null) +
                                         $". It would put {suggestion.WordsNewlyBundled} more words into bundles, of which " +
                                         $"{RuleAnalysisResult.FormatFraction(suggestion.FractionCorrected)} have been part of a correction " +
                                         $"(the current settings typically have {typical:P0}).";
                distinction.Description += $"With it, {suggestion.CorrectionsGained} more corrections would have been found.";
                candidates.Add(suggestion);
                cancel.ThrowIfCancellationRequested();
            }

            // drop candidates that mostly find the same corrections as a better (recommended) one, e.g. the same
            //  difference seen in both the vernacular and the transliterated form, or a setting that would be fixed by
            //  moving or turning on an existing one. (A broad, not recommended candidate shouldn't hide a precise one.)
            var kept = new List<RuleSuggestion>();
            var foundByRecommended = new HashSet<int>(result.Suggestions.Where(s => s.Recommended && (s.Kind != SuggestionKind.TurnOffRule))
                                                                        .SelectMany(s => s.CaughtCorrections));
            foreach (var candidate in candidates.Select(c => { c.Recommended = IsWorthIt(c); return c; }))
                if (IsTooNoisy(candidate))
                    result.LeftOut.Add(candidate);
            foreach (var candidate in candidates.Where(c => !result.LeftOut.Contains(c))
                                                .OrderByDescending(c => c.Recommended)
                                                .ThenByDescending(c => c.CorrectionsGained - c.CorrectionsLost)
                                                .ThenByDescending(c => c.FractionCorrected))
            {
                var overlap = candidate.CaughtCorrections.Count(foundByRecommended.Contains);
                if (overlap >= 0.7 * candidate.CaughtCorrections.Count)
                    continue;
                kept.Add(candidate);
                if (candidate.Recommended || (candidate.CorrectionsLost <= 0.1 * candidate.CorrectionsGained))
                    foundByRecommended.UnionWith(candidate.CaughtCorrections);
                if (kept.Count >= 20)
                    break;
            }
            result.Suggestions.AddRange(kept);

            // 4. data and settings to clean up
            progress("Checking the word lists for things to clean up...", 92);
            AddDataSuggestions(result);

            foreach (var suggestion in result.Suggestions)
                DescribeInPlainLanguage(suggestion, typicalYield);

            progress("Done", 100);
            return result;
        }

        /// <summary>
        /// Shows a vernacular difference (e.g. a vowel sign) so it's visible on its own, e.g. "ै" -> "◌ै", "" -> "(nothing)"
        /// </summary>
        private static string ShowPart(string part)
        {
            if (String.IsNullOrEmpty(part))
                return "(nothing)";
            var cat = CharUnicodeInfo.GetUnicodeCategory(part[0]);
            var isMark = (cat == UnicodeCategory.NonSpacingMark) || (cat == UnicodeCategory.SpacingCombiningMark) || (cat == UnicodeCategory.EnclosingMark);
            return isMark ? "◌" + part : part;
        }

        /// <summary>
        /// The most common difference(s) between the vernacular spellings of the given corrections, e.g. "◌ै / ◌े"
        /// </summary>
        private string DescribeVernacularDifference(IEnumerable<int> correctionIndices)
        {
            var counts = new Dictionary<string, int>(StringComparer.Ordinal);
            foreach (var i in correctionIndices)
            {
                var diffs = Differences(VernacularForDiff(_corrections[i][0]), VernacularForDiff(_corrections[i][1]), false);
                if (diffs.Count != 1)
                    continue;
                var parts = new[] { diffs[0][0], diffs[0][1] }.OrderBy(p => p.Length).ThenBy(p => p, StringComparer.Ordinal).ToArray();
                var key = $"{ShowPart(parts[0])} / {ShowPart(parts[1])}";
                counts[key] = (counts.TryGetValue(key, out var n) ? n : 0) + 1;
            }
            var top = counts.OrderByDescending(kvp => kvp.Value).Take(2).ToList();
            if (top.Count == 0)
                return null;
            // (only mention the 2nd if it's nearly as common)
            return ((top.Count > 1) && (top[1].Value >= top[0].Value / 2))
                    ? $"{top[0].Key}  or  {top[1].Key}"
                    : top[0].Key;
        }

        /// <summary>
        /// e.g. " That's about 1 real mistake for every 4 words it would show you (your current settings: about 1 in 2)."
        /// </summary>
        private static string DescribeYield(int found, int words, double typicalYield, string verb)
        {
            if ((found <= 0) || (words <= 0))
                return null;
            var n = Math.Max(1, (int)Math.Round((double)words / found));
            var typicalN = (typicalYield > 0) ? Math.Max(1, (int)Math.Round(1 / typicalYield)) : 0;
            return $" That's about 1 real mistake for every {n} words it {verb}" +
                   ((typicalN > 0) ? $" (your settings typically: about 1 in {typicalN})." : ".");
        }

        private void DescribeInPlainLanguage(RuleSuggestion s, double typicalYield)
        {
            if (s.IsRuleChange)
                s.VernacularDifference = DescribeVernacularDifference(s.CaughtCorrections);
            var difference = (s.VernacularDifference != null) ? $" (mostly: {s.VernacularDifference})" : null;
            var lost = (s.CorrectionsLost > 0) ? $" (but {s.CorrectionsLost} that are found now wouldn't be)" : null;
            var numWords = s.WordListFixes?.Count ?? 0;
            var yieldGained = DescribeYield(s.CorrectionsGained, s.WordsNewlyBundled, typicalYield, "would show you");
            var yieldLost = DescribeYield(s.CorrectionsLost, s.WordsNewlyBundled, typicalYield, "shows you");

            switch (s.Kind)
            {
                case SuggestionKind.AddRule:
                    s.Heading = (s.VernacularDifference != null)
                                    ? $"Spellings that differ by  {s.VernacularDifference}"
                                    : $"Spellings that differ like this: {s.NewDistinction.Name}";
                    s.Question = "Should the Spelling Fixer show you words that differ only like this, as possible inconsistencies?";
                    s.Note = $"Of your {_corrections.Count} corrections, this would have found {s.CorrectionsGained} more{lost}. " +
                             $"It would also show you {s.WordsNewlyBundled} more words to check.{yieldGained}";
                    s.Action = (s.VernacularDifference != null)
                                    ? $"Look for spellings that differ by  {s.VernacularDifference}"
                                    : $"Add the setting {s.NewDistinction.Name}";
                    break;
                case SuggestionKind.TurnOffRule:
                    s.Heading = (s.CorrectionsLost == 0)
                                    ? $"The '{s.DistinctionName}' setting makes no difference for your data"
                                    : $"The '{s.DistinctionName}' setting finds few of the kinds of mistakes you correct{difference}";
                    s.Question = $"Turn off the '{s.DistinctionName}' setting? (You can turn it back on at any time, and the next analysis will check it again.)";
                    s.Note = (s.CorrectionsLost == 0)
                                ? "None of your corrections needed it, and it doesn't put any words together, so turning it off changes nothing (it just makes the list of settings shorter)."
                                : $"It found only {s.CorrectionsLost} of your {_corrections.Count} corrections, but it shows you {s.WordsNewlyBundled} words to check that you wouldn't otherwise have to.{yieldLost}";
                    s.Action = $"Turn off the '{s.DistinctionName}' setting";
                    break;
                case SuggestionKind.TurnOnRule:
                    s.Heading = $"The '{s.DistinctionName}' setting (which you aren't using) would find more of your mistakes{difference}";
                    s.Question = $"Turn on the '{s.DistinctionName}' setting?";
                    s.Note = $"It would have found {s.CorrectionsGained} more of your {_corrections.Count} corrections{lost}, and would show you {s.WordsNewlyBundled} more words to check.{yieldGained}";
                    s.Action = $"Turn on the '{s.DistinctionName}' setting";
                    break;
                case SuggestionKind.MoveRule:
                    s.Heading = $"The '{s.DistinctionName}' setting isn't getting a chance to work{difference}";
                    s.Question = $"Apply the '{s.DistinctionName}' setting before the other settings, so it can work?";
                    s.Note = "The settings are applied in order, and one before it changes the same letters first. " +
                             $"Applied first, it would have found {s.CorrectionsGained} more of your corrections{lost}.";
                    s.Action = $"Apply the '{s.DistinctionName}' setting first";
                    break;
                case SuggestionKind.CleanUpWordLists:
                    s.Heading = "Words with formatting codes or punctuation stuck to them";
                    s.Question = "Clean up these words (in the Known Good and Words to Check lists)?";
                    s.Note = $"{numWords} words in your lists have things like '\\f' or '\\qt2-e' stuck to them, so they aren't compared with the same words without them.";
                    s.Action = $"Clean up {numWords} words with formatting codes or punctuation stuck to them";
                    break;
                case SuggestionKind.RemoveWords:
                    s.Heading = "Words written in a different script (e.g. English)";
                    s.Question = "Remove these words from the Words to Check list?";
                    s.Note = $"{numWords} words in the Words to Check list aren't in the script the rest of your words are in (they probably come from formatting codes).";
                    s.Action = $"Remove {numWords} words written in a different script";
                    break;
                case SuggestionKind.PreprocessRegex:
                    s.Heading = s.Title;
                    s.Question = "Add this step, so this doesn't get into the word lists again?";
                    s.Note = "This is a step that's done to the text before it's split into words. The examples are from your text.";
                    s.Action = s.Title;
                    break;
                case SuggestionKind.IgnoreCharacter:
                    s.Heading = $"Words with {Describe(s.Character[0])} inside them";
                    s.Question = $"Ignore the {Describe(s.Character[0])} when comparing words?";
                    s.Note = $"These words have {Describe(s.Character[0])} in them, so they aren't compared with the same words without it.";
                    s.Action = $"Ignore the {Describe(s.Character[0])} inside words";
                    break;
                case SuggestionKind.PunctuationCharacter:
                    s.Heading = $"Words that begin or end with {Describe(s.Character[0])}";
                    s.Question = $"Treat {Describe(s.Character[0])} as punctuation (i.e. not part of words)?";
                    s.Action = $"Treat {Describe(s.Character[0])} as punctuation";
                    break;
            }
        }

        /// <summary>
        /// Is a setting change worth recommending? It has to find enough more corrections, hardly lose any, and be about as
        /// precise as the current settings typically are--both in the fraction of the words it bundles that get corrected,
        /// and in the corrections it finds per word it bundles (so a setting that finds a lot by bundling a huge number of
        /// words isn't recommended just because of its volume)
        /// </summary>
        private static bool IsWorthItCriteria(RuleSuggestion suggestion, int minUsefulGain, double typicalPrecision, double typicalYield)
        {
            var net = suggestion.CorrectionsGained - suggestion.CorrectionsLost;
            return (net >= minUsefulGain) &&
                   (suggestion.CorrectionsLost <= 0.1 * suggestion.CorrectionsGained) &&
                   ((suggestion.FractionCorrected < 0) || (suggestion.FractionCorrected >= 0.9 * typicalPrecision)) &&
                   ((suggestion.WordsNewlyBundled <= 0) || ((double)suggestion.CorrectionsGained / suggestion.WordsNewlyBundled >= 0.5 * typicalYield));
        }

        /// <summary>
        /// Estimates the effect of applying all the given (rule) suggestions together
        /// </summary>
        /// <returns>the number of corrections that would be bundled</returns>
        public int EstimateCombined(IEnumerable<RuleSuggestion> suggestions, out int wordsNewlyBundled)
        {
            var list = suggestions.Where(s => s.IsRuleChange).ToList();
            var turnOff = new HashSet<string>(list.Where(s => s.Kind == SuggestionKind.TurnOffRule).Select(s => s.DistinctionName));
            var turnOn = new HashSet<string>(list.Where(s => s.Kind == SuggestionKind.TurnOnRule).Select(s => s.DistinctionName));
            var moveFirst = new HashSet<string>(list.Where(s => s.Kind == SuggestionKind.MoveRule).Select(s => s.DistinctionName));
            var added = list.Where(s => s.Kind == SuggestionKind.AddRule).Select(s => s.NewDistinction);

            // (the same order the project dialog ends up with: added settings first, then moved ones, then the rest)
            var used = _distinctions.Where(d => (_enabled.Contains(d.Name) && !turnOff.Contains(d.Name)) || turnOn.Contains(d.Name)).ToList();
            var distinctions = added.Concat(used.Where(d => moveFirst.Contains(d.Name)))
                                    .Concat(used.Where(d => !moveFirst.Contains(d.Name)));
            var evalCurrent = Evaluate(CurrentDistinctions);
            var with = Evaluate(distinctions);
            wordsNewlyBundled = _corpus.Keys.Count(w => (GroupSize(with, w) > 1) && (GroupSize(evalCurrent, w) <= 1));
            return with.NumBundled;
        }

        #region Data clean up

        private static bool IsWordForming(char ch)
        {
            if ((ch == Zwj) || (ch == Zwnj))
                return true;
            var cat = CharUnicodeInfo.GetUnicodeCategory(ch);
            return Char.IsLetter(ch) || (cat == UnicodeCategory.NonSpacingMark) || (cat == UnicodeCategory.SpacingCombiningMark) ||
                   (cat == UnicodeCategory.EnclosingMark);
        }

        private static bool IsLatinLetter(char ch) => Char.IsLetter(ch) && ((ch <= '\u024F') || ((ch >= '\u1E00') && (ch <= '\u1EFF')));

        private static readonly Dictionary<char, string> s_invisibleCharacterNames = new Dictionary<char, string>
        {
            { '­', "soft hyphen" },
            { '​', "zero width space" },
            { '‌', "zero width non-joiner" },
            { '‍', "zero width joiner" },
            { '⁠', "word joiner" },
            { '﻿', "zero width no-break space" },
        };

        private static string Describe(char ch)
        {
            // (invisible characters need a name, or they'd show as '')
            if (s_invisibleCharacterNames.TryGetValue(ch, out var name) || (CharUnicodeInfo.GetUnicodeCategory(ch) == UnicodeCategory.Format))
                return $"{name ?? "invisible character"} (U+{(int)ch:X4})";
            return $"'{ch}' (U+{(int)ch:X4})";
        }

        private void AddDataSuggestions(RuleAnalysisResult result)
        {
            // a) entries with SFM markers or attributes in them, or punctuation that should have been trimmed
            var fixes = new List<WordListFix>();
            foreach (var word in _corpus.Keys)
            {
                var pieces = CscProject.ContainsMarkup(word) ? CscProject.SplitOutMarkup(word) : new List<string> { word };
                var trimmed = pieces.Select(_trim).Where(p => !String.IsNullOrEmpty(p)).ToList();
                if ((trimmed.Count == 1) && (trimmed[0] == word))
                    continue;
                fixes.Add(new WordListFix { Word = word, InKnownGoodList = _knownGood.Contains(word), Replacements = trimmed });
            }
            if (fixes.Count > 0)
            {
                result.Suggestions.Add(new RuleSuggestion
                {
                    Kind = SuggestionKind.CleanUpWordLists,
                    Recommended = true,
                    Title = $"Clean up {fixes.Count} word list entries that have SFM markers or punctuation attached",
                    Explanation = "These entries have SFM markers (e.g. '\\f', '\\qt2-e'), USFM attributes (e.g. src=\"...\"), or punctuation still " +
                                  "attached to them, so they can't be bundled with the same word without them. Each will be replaced by the word(s) " +
                                  "without the markup (markers are now split off automatically as words are added, so this shouldn't happen again). " +
                                  "This is saved as soon as you apply it.",
                    WordListFixes = fixes,
                    ExampleWords = fixes.Take(12).Select(f => $"{f.Word}  →  {(f.Replacements.Any() ? String.Join(" + ", f.Replacements) : "(removed)")}").ToList()
                });
            }
            var fixedWords = new HashSet<string>(fixes.Select(f => f.Word));
            var cleanWords = _corpus.Keys.Where(w => !fixedWords.Contains(w)).ToList();

            // b) words in the Words to Check list in a different script (e.g. English from attributes or milestones)
            int nonLatin = cleanWords.Count(w => w.Any(ch => Char.IsLetter(ch) && !IsLatinLetter(ch)));
            if (nonLatin > 0.8 * cleanWords.Count)
            {
                var foreign = cleanWords.Where(w => !_knownGood.Contains(w) && w.Any(Char.IsLetter) && w.Where(Char.IsLetter).All(IsLatinLetter)).ToList();
                if (foreign.Count > 0)
                {
                    result.Suggestions.Add(new RuleSuggestion
                    {
                        Kind = SuggestionKind.RemoveWords,
                        Recommended = true,
                        Title = $"Remove {foreign.Count} words written in Latin script from the Words to Check list",
                        Explanation = "Most of the words in this project aren't written in Latin script, but these are (e.g. English words from figure or " +
                                      "milestone attributes, or footnote markers). Removing them keeps them out of the bundles. (If they come back, " +
                                      "a preprocessing regular expression to remove the text they come from may be suggested below.)",
                        WordListFixes = foreign.Select(w => new WordListFix { Word = w }).ToList(),
                        ExampleWords = foreign.OrderByDescending(w => _corpus[w]).Take(20).ToList()
                    });
                }
            }

            // c) preprocessing regexs for markup whose (non-vernacular) contents end up as words
            var existingPatterns = new HashSet<string>((_vss?.PreprocessDataViaRegex?.RegexChangeList ?? new List<RegexChange>()).Select(r => r.Pattern));
            var allContexts = _contexts.Values.SelectMany(c => c ?? new List<string>()).ToList();
            void SuggestRegex(string pattern, string replaceWith, string description, string title)
            {
                if (existingPatterns.Contains(pattern))
                    return;
                var rx = new Regex(pattern);
                var examples = allContexts.Where(c => rx.IsMatch(c)).Distinct().Take(6).ToList();
                if (examples.Count == 0)
                    return;
                result.Suggestions.Add(new RuleSuggestion
                {
                    Kind = SuggestionKind.PreprocessRegex,
                    Recommended = true,
                    Title = title,
                    Explanation = description + $" (pattern: {pattern}, replaced with: '{replaceWith}')",
                    Regex = new RegexChange { Description = description, Pattern = pattern, ReplaceWith = replaceWith },
                    ExampleWords = examples
                });
            }
            SuggestRegex(@"\\qt\d*-[se]\b[^\\]*\\\*", " ",
                         "Remove USFM quotation milestones, including their attributes (e.g. '\\qt2-s |Lamech\\*' and '\\qt2-e\\*')",
                         "Add a preprocessing step to remove quotation milestones (\\qt-s ... \\*)");
            SuggestRegex(@"\\fig\s+([^|\\]*)(\|[^\\]*)?\\fig\*", " $1 ",
                         "Remove USFM figure markup and attributes (keeping the caption of USFM 3 style figures, e.g. '\\fig caption|src=\"x.jpg\" ref=\"9:6\"\\fig*')",
                         "Add a preprocessing step to remove figure attributes (\\fig caption|src=... \\fig*)");

            // d) characters inside words that should probably be ignored (e.g. soft hyphen, or a curly apostrophe
            //  when the straight one is already ignored)
            var alreadyIgnored = new HashSet<string>((_ignoreBefore ?? new string[0]).Concat(_ignoreAfter ?? new string[0]));
            var quoteLike = "\u2019\u2018\u02BC\u02BB\u00B4`";
            var innerChars = new Dictionary<char, List<string>>();
            foreach (var word in cleanWords)
                for (int i = 1; i < word.Length - 1; i++)
                {
                    var ch = word[i];
                    var cat = CharUnicodeInfo.GetUnicodeCategory(ch);
                    if (((cat == UnicodeCategory.Format) || (ch == SoftHyphen) || (quoteLike.IndexOf(ch) >= 0)) && !alreadyIgnored.Contains(ch.ToString()))
                    {
                        if (!innerChars.TryGetValue(ch, out var list))
                            innerChars[ch] = list = new List<string>();
                        if (!list.Contains(word))
                            list.Add(word);
                    }
                }
            foreach (var kvp in innerChars.Where(k => k.Value.Count >= 2))
            {
                // only if it survives transliteration (otherwise it's already effectively ignored)
                var sample = kvp.Value.First();
                var prepared = CscProject.RemoveStrings(sample, _ignoreBefore);
                EnsureTransliterated(new[] { prepared });
                if (_transliterations[prepared].IndexOf(kvp.Key) < 0)
                    continue;

                result.Suggestions.Add(new RuleSuggestion
                {
                    Kind = SuggestionKind.IgnoreCharacter,
                    Recommended = true,
                    Character = kvp.Key.ToString(),
                    Title = $"Ignore {Describe(kvp.Key)} inside words ({kvp.Value.Count} words have it)",
                    Explanation = $"The character {Describe(kvp.Key)} occurs inside {kvp.Value.Count} words, so they aren't compared with the same words " +
                                  "without it. This adds it to the 'Ignore Characters After Transliteration' list.",
                    ExampleWords = kvp.Value.Take(12).ToList()
                });
            }

            // e) a punctuation character between two words with no space (e.g. 'चेलें/रसूलें'): treat it as a word break
            var breakChars = new Dictionary<char, List<string>>();
            foreach (var word in cleanWords)
                for (int i = 1; i < word.Length - 1; i++)
                {
                    var ch = word[i];
                    if (!Char.IsPunctuation(ch) || (ch == '-') || (ch == '\\') || (ch == SoftHyphen) || (quoteLike.IndexOf(ch) >= 0) || (ch == '\'') ||
                        (CharUnicodeInfo.GetUnicodeCategory(ch) == UnicodeCategory.DashPunctuation) || !IsWordForming(word[i - 1]) || !IsWordForming(word[i + 1]))
                        continue;
                    var parts = word.Split(ch);
                    if (parts.All(p => _corpus.ContainsKey(p)))
                    {
                        if (!breakChars.TryGetValue(ch, out var list))
                            breakChars[ch] = list = new List<string>();
                        if (!list.Contains(word))
                            list.Add(word);
                    }
                }
            foreach (var kvp in breakChars.Where(k => k.Value.Count >= 2))
            {
                var pattern = $@"(?<=[\p{{L}}\p{{M}}]){Regex.Escape(kvp.Key.ToString())}(?=\p{{L}})";
                if (existingPatterns.Contains(pattern))
                    continue;
                result.Suggestions.Add(new RuleSuggestion
                {
                    Kind = SuggestionKind.PreprocessRegex,
                    Recommended = true,
                    Title = $"Treat {Describe(kvp.Key)} between two words as a word break ({kvp.Value.Count} words)",
                    Explanation = $"These entries are two words joined by {Describe(kvp.Key)} with no space, and each part is already a word in your lists. " +
                                  "This adds a preprocessing step to replace it with a space when it's between letters (punctuation is otherwise only " +
                                  "trimmed from the ends of words). After applying it, clean up or re-check the existing entries.",
                    Regex = new RegexChange
                    {
                        Description = $"Treat {Describe(kvp.Key)} between letters as a word break",
                        Pattern = pattern,
                        ReplaceWith = " "
                    },
                    ExampleWords = kvp.Value.Take(12).ToList()
                });
            }

            // f) characters at the edges of words that aren't being trimmed
            var edgeChars = new Dictionary<char, List<string>>();
            foreach (var word in cleanWords)
                foreach (var ch in new[] { word[0], word[word.Length - 1] })
                {
                    if (IsWordForming(ch) || Char.IsDigit(ch) || Char.IsWhiteSpace(ch) || Char.IsPunctuation(ch) || (ch == '\\'))
                        continue;   // (those are trimmed already)
                    if (!edgeChars.TryGetValue(ch, out var list))
                        edgeChars[ch] = list = new List<string>();
                    if (!list.Contains(word))
                        list.Add(word);
                }
            foreach (var kvp in edgeChars.Where(k => k.Value.Count >= 3))
            {
                result.Suggestions.Add(new RuleSuggestion
                {
                    Kind = SuggestionKind.PunctuationCharacter,
                    Recommended = true,
                    Character = kvp.Key.ToString(),
                    Title = $"Add {Describe(kvp.Key)} to the punctuation list ({kvp.Value.Count} words begin or end with it)",
                    Explanation = $"{kvp.Value.Count} words begin or end with {Describe(kvp.Key)}, which isn't being trimmed off as punctuation. " +
                                  "This adds it to the extra punctuation list (after which, the existing entries can be cleaned up by analyzing again).",
                    ExampleWords = kvp.Value.Take(12).ToList()
                });
            }
        }

        #endregion
    }
}
