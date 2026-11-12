using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml;
using System.Xml.Linq;

namespace SpellingFixer30
{
    /// <summary>
    /// A word whose spelling status in a Paratext project's SpellingStatus.xml disagrees with the SpellFixer project's
    /// (i.e. one that the user has to decide about, rather than one that can just be added)
    /// </summary>
    internal class ParatextSpellingConflict
    {
        public string Word { get; set; }

        /// <summary>"R" (correct) or "W" (wrong)</summary>
        public string ParatextState { get; set; }

        /// <summary>Paratext's correction for the word (if it's "W" and has one)</summary>
        public string ParatextCorrection { get; set; }

        /// <summary>SpellFixer's correction for the word, or null if SpellFixer has it as a correct (good) word</summary>
        public string SpellFixerCorrection { get; set; }

        internal XElement Status { get; set; }

        public string ParatextDescription => (ParatextState == ParatextSpellingStatusExport.StateRight)
            ? "correct"
            : String.IsNullOrEmpty(ParatextCorrection) ? "misspelled" : $"misspelled → {ParatextCorrection}";

        public string SpellFixerDescription => (SpellFixerCorrection == null)
            ? "correct"
            : $"misspelled → {SpellFixerCorrection}";
    }

    /// <summary>
    /// Works out how to bring a SpellFixer project's good words and spelling corrections into a Paratext project's
    /// SpellingStatus.xml file:
    ///   <SpellingStatus>
    ///     <Status Word="the" State="R" />
    ///     <Status Word="teh" State="W"><Correction>the</Correction></Status>
    ///   </SpellingStatus>
    /// Words that Paratext doesn't have yet (or has as wrong, but w/o a correction) are just added (to the in-memory
    /// document); words that Paratext has differently are collected as Conflicts for the user to decide about (see Apply).
    /// Nothing is written until Save is called.
    /// </summary>
    internal class ParatextSpellingStatusExport
    {
        public const string StateRight = "R";
        public const string StateWrong = "W";
        private const string StatusName = "Status";
        private const string WordName = "Word";
        private const string StateName = "State";
        private const string CorrectionName = "Correction";

        public XDocument Document { get; private set; }
        public List<ParatextSpellingConflict> Conflicts { get; } = new List<ParatextSpellingConflict>();

        public int NumApprovedWordsAdded { get; private set; }
        public int NumCorrectionsAdded { get; private set; }
        public int NumConflictsReplaced { get; private set; }

        /// <summary>the number of good words or corrections that aren't single words (e.g. phrases), which Paratext can't hold</summary>
        public int NumSkipped { get; private set; }

        public bool HasChanges => (NumApprovedWordsAdded + NumCorrectionsAdded + NumConflictsReplaced) > 0;

        public static ParatextSpellingStatusExport Plan(string filespec, IEnumerable<string> goodWords,
                                                        IEnumerable<KeyValuePair<string, string>> corrections)
        {
            var export = new ParatextSpellingStatusExport { Document = XDocument.Load(filespec) };
            if (export.Document.Root == null)
                throw new ApplicationException($"The file '{filespec}' isn't a Paratext SpellingStatus file");
            export.Plan(goodWords, corrections.ToList());
            return export;
        }

        private void Plan(IEnumerable<string> goodWords, List<KeyValuePair<string, string>> corrections)
        {
            var root = Document.Root;
            var statusByWord = new Dictionary<string, XElement>(StringComparer.Ordinal);
            foreach (var status in root.Elements(StatusName))
            {
                var word = (string)status.Attribute(WordName);
                if (!String.IsNullOrEmpty(word) && !statusByWord.ContainsKey(word))
                    statusByWord.Add(word, status);
            }

            // the good words (incl. the corrected spellings, which should already be good words, but just in case)
            var goodSet = new HashSet<string>(goodWords, StringComparer.Ordinal);
            goodSet.UnionWith(corrections.Select(c => c.Value).Where(v => !String.IsNullOrEmpty(v)));
            foreach (var good in goodSet.OrderBy(w => w, StringComparer.Ordinal))
            {
                if (!IsSingleWord(good))
                {
                    NumSkipped++;
                    continue;
                }

                if (!statusByWord.TryGetValue(good, out var status))
                {
                    statusByWord.Add(good, AddStatus(root, good, StateRight, null));
                    NumApprovedWordsAdded++;
                }
                else if (StateOf(status) == StateWrong)
                {
                    Conflicts.Add(new ParatextSpellingConflict
                    {
                        Word = good,
                        ParatextState = StateWrong,
                        ParatextCorrection = CorrectionOf(status),
                        SpellFixerCorrection = null,
                        Status = status
                    });
                }
                else if (StateOf(status) != StateRight)
                {
                    // (some state we don't know about; treat it as not having a status)
                    SetStatus(status, StateRight, null);
                    NumApprovedWordsAdded++;
                }
            }

            foreach (var correction in corrections.OrderBy(c => c.Key, StringComparer.Ordinal))
            {
                var bad = correction.Key;
                var good = correction.Value;
                if (!IsSingleWord(bad) || !IsSingleWord(good))
                {
                    NumSkipped++;
                    continue;
                }

                // (if SpellFixer has it as both good and bad, which shouldn't happen, the good word was dealt with above)
                if (goodSet.Contains(bad))
                    continue;

                if (!statusByWord.TryGetValue(bad, out var status))
                {
                    statusByWord.Add(bad, AddStatus(root, bad, StateWrong, good));
                    NumCorrectionsAdded++;
                    continue;
                }

                var state = StateOf(status);
                var paratextCorrection = CorrectionOf(status);
                if ((state == StateWrong) && (paratextCorrection == good))
                    continue;   // already the same

                if ((state == StateWrong) && String.IsNullOrEmpty(paratextCorrection))
                {
                    // Paratext knows it's wrong, but doesn't have a correction for it, so just add ours
                    SetStatus(status, StateWrong, good);
                    NumCorrectionsAdded++;
                }
                else if ((state == StateWrong) || (state == StateRight))
                {
                    Conflicts.Add(new ParatextSpellingConflict
                    {
                        Word = bad,
                        ParatextState = state,
                        ParatextCorrection = paratextCorrection,
                        SpellFixerCorrection = good,
                        Status = status
                    });
                }
                else
                {
                    SetStatus(status, StateWrong, good);
                    NumCorrectionsAdded++;
                }
            }
        }

        /// <summary>
        /// Replaces Paratext's status with SpellFixer's for the given conflicts (the others are left as Paratext has them)
        /// </summary>
        public void Apply(IEnumerable<ParatextSpellingConflict> conflictsToReplace)
        {
            foreach (var conflict in conflictsToReplace)
            {
                if (conflict.SpellFixerCorrection == null)
                    SetStatus(conflict.Status, StateRight, null);
                else
                    SetStatus(conflict.Status, StateWrong, conflict.SpellFixerCorrection);
                NumConflictsReplaced++;
            }
        }

        /// <summary>
        /// Saves the way Paratext does (UTF-8 w/ BOM, 2-space indent, CRLF), by writing a temporary file and then
        /// replacing the original with it (so a failure part way through can't leave a broken file behind).
        /// </summary>
        public void Save(string filespec)
        {
            var settings = new XmlWriterSettings
            {
                Encoding = new UTF8Encoding(true),
                Indent = true,
                IndentChars = "  ",
                NewLineChars = "\r\n",
                NewLineHandling = NewLineHandling.Replace,
            };

            var tempPath = filespec + ".tmp";
            using (var writer = XmlWriter.Create(tempPath, settings))
                Document.Save(writer);

            if (File.Exists(filespec))
                File.Replace(tempPath, filespec, null);
            else
                File.Move(tempPath, filespec);
        }

        /// <summary>
        /// Paratext must not be running when its SpellingStatus.xml file is changed (it would overwrite the changes
        /// with what it has in memory)
        /// </summary>
        public static bool IsParatextRunning()
        {
            foreach (var process in Process.GetProcesses())
            {
                try
                {
                    if (process.ProcessName.StartsWith("Paratext", StringComparison.OrdinalIgnoreCase))
                        return true;
                }
                catch
                {
                    // (e.g. it exited while we were looking at it)
                }
                finally
                {
                    process.Dispose();
                }
            }
            return false;
        }

        private static bool IsSingleWord(string word)
        {
            return !String.IsNullOrEmpty(word) && !word.Any(Char.IsWhiteSpace);
        }

        private static string StateOf(XElement status)
        {
            return (string)status.Attribute(StateName);
        }

        private static string CorrectionOf(XElement status)
        {
            return status.Element(CorrectionName)?.Value;
        }

        private static XElement AddStatus(XElement root, string word, string state, string correction)
        {
            var status = new XElement(StatusName, new XAttribute(WordName, word));
            root.Add(status);
            SetStatus(status, state, correction);
            return status;
        }

        private static void SetStatus(XElement status, string state, string correction)
        {
            status.SetAttributeValue(StateName, state);
            status.Elements(CorrectionName).Remove();
            if (correction != null)
                status.Add(new XElement(CorrectionName, correction));
        }
    }
}
