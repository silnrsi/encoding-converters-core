using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SpellingFixer30
{
    /// <summary>
    /// One 'NormalizeRule' of a Distinction (aka Inconsistency Setting) of the Vernacular Script System, ready to apply:
    /// each of the Forms is replaced by the NormalizedForm (which may be empty, meaning the Forms are just removed).
    /// </summary>
    [Serializable()]
    internal class AmbiguityRule
    {
        public string DistinctionName { get; set; }
        public bool BeforeTransliteration { get; set; }
        public string NormalizedForm { get; set; }
        public string[] Forms { get; set; }

        public string Apply(string str)
        {
            if (String.IsNullOrEmpty(str))
                return str;

            foreach (var form in Forms)
                str = str.Replace(form, NormalizedForm);
            return str;
        }

        public override string ToString()
        {
            return $"{DistinctionName}: [{String.Join(", ", Forms)}] -> '{NormalizedForm}'{(BeforeTransliteration ? " (before transliteration)" : null)}";
        }

        /// <summary>
        /// Creates the rule from the VSS file's NormalizeRule. Rules applied before transliteration are applied to words
        /// that have been NFKC normalized (see CscProject.TrimWhiteSpaceAndPunctuation), so normalize them the same way
        /// </summary>
        public static AmbiguityRule Create(string distinctionName, NormalizeRule normalizeRule)
        {
            var isBefore = normalizeRule.IsBeforeTransliteration;
            string Prep(string s) => (isBefore && !String.IsNullOrEmpty(s)) ? s.Normalize(NormalizationForm.FormKC) : s;

            var forms = (normalizeRule.CharactersToNormalize?.CharacterToNormalize ?? new List<CharacterToNormalize>())
                            .Select(c => Prep(c.Value))
                            .Where(f => !String.IsNullOrEmpty(f))
                            .Distinct()
                            .OrderByDescending(f => f.Length)   // so e.g. 'aa' gets replaced before 'a' can break it up
                            .ToArray();

            return new AmbiguityRule
            {
                DistinctionName = distinctionName,
                BeforeTransliteration = isBefore,
                NormalizedForm = Prep(normalizeRule.NormalizedForm) ?? String.Empty,
                Forms = forms
            };
        }
    }

    /// <summary>
    /// The ambiguity rules a project uses, in the order they're applied: the order of the Distinctions in the
    /// Vernacular Script System file (and of the NormalizeRules within each Distinction).
    /// (This used to be a Hashtable keyed by the NormalizedForm, which meant the rules were applied in hash order,
    /// which differs between 32-bit and 64-bit processes, so the same data could be bundled differently in each.)
    /// </summary>
    [Serializable()]
    internal class AmbiguityRules
    {
        public List<AmbiguityRule> BeforeTransliteration { get; } = new List<AmbiguityRule>();
        public List<AmbiguityRule> AfterTransliteration { get; } = new List<AmbiguityRule>();

        public void Add(AmbiguityRule rule)
        {
            if ((rule == null) || (rule.Forms.Length == 0))
                return;
            (rule.BeforeTransliteration ? BeforeTransliteration : AfterTransliteration).Add(rule);
        }

        public string ApplyBeforeTransliteration(string vernacularWord)
        {
            foreach (var rule in BeforeTransliteration)
                vernacularWord = rule.Apply(vernacularWord);
            return vernacularWord;
        }

        public string ApplyAfterTransliteration(string transliteratedWord)
        {
            foreach (var rule in AfterTransliteration)
                transliteratedWord = rule.Apply(transliteratedWord);
            return transliteratedWord;
        }

        public bool IsEmpty => (BeforeTransliteration.Count == 0) && (AfterTransliteration.Count == 0);

        /// <summary>
        /// Builds the rules for the given (enabled) distinctions, in the order they occur in the VSS file
        /// </summary>
        /// <param name="sfssp">the Vernacular Script System</param>
        /// <param name="enabledDistinctionNames">the names of the distinctions the project uses</param>
        /// <param name="missingDistinctionNames">any of the enabled names that aren't in the VSS file</param>
        public static AmbiguityRules Build(VernScriptSystem sfssp, IEnumerable<string> enabledDistinctionNames,
                                           out List<string> missingDistinctionNames)
        {
            var enabled = new HashSet<string>(enabledDistinctionNames ?? Enumerable.Empty<string>());
            var distinctions = sfssp?.Distinctions?.DistinctionList ?? new List<Distinction>();
            missingDistinctionNames = enabled.Where(n => !distinctions.Any(d => d.Name == n)).ToList();
            return Build(distinctions.Where(d => enabled.Contains(d.Name)));
        }

        public static AmbiguityRules Build(IEnumerable<Distinction> distinctionsInOrder)
        {
            var rules = new AmbiguityRules();
            foreach (var distinction in distinctionsInOrder)
                foreach (var normalizeRule in distinction.NormalizeRules ?? new List<NormalizeRule>())
                    rules.Add(AmbiguityRule.Create(distinction.Name, normalizeRule));
            return rules;
        }
    }
}
