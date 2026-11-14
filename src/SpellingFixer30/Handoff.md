# SpellingFixer30 — handoff notes

Status as of 2026-11-09. **None of this work is committed yet.** The changes are in the working tree on `master`, together with the earlier Paratext-sharing work: 18 modified files and 5 new ones, listed under [Files](#files).

Both x64 and Win32 Debug builds succeed. All testing was done on a sandbox copy of the Dogri project, in the Temp folder of the session that did this work (see [How it was tested](#how-it-was-tested)).

## Summary

1. **Paratext sharing.** A Consistent Spelling Fixer (CSF) project can live inside a Paratext project's `shared` folder, so Paratext Send/Receive syncs it. Locally it's reached through a junction under `ProgramData`. This shipped as NuGet 1.0.3.
2. **Bug fix: rules applied in the wrong order.** Inconsistency rules ran in hash-table order, which differs between 32-bit and 64-bit processes. They now run in the order they appear in the VSS file.
3. **New: rules that run before transliteration.** A rule can now work on the vernacular text instead of its Latin transliteration.
4. **New: Analyze dialog.** It checks the project's Inconsistency Settings against the corrections the user has actually made. It then suggests:
   - settings to change: turn off, turn back on, or move earlier;
   - new rules;
   - clean-ups of the data.
5. **New: Pause button** in the Bulk Picker, to stop searching for bundles.
6. **New: SFM markers are split off** words before they're added to WordsToCheck.
7. **Removed an unnecessary "A file exists… overwrite?" prompt** when a project is first opened through a junction.

## How SpellFixer decides two words are "the same"

The rest of this document assumes this background. A word's *key* is built in these steps, and words with identical keys go in the same bundle:

1. Normalize to NFKC (`TrimWhiteSpaceAndPunctuation`).
2. **Run the before-transliteration rules (new).**
3. Remove the "ignore before" characters (e.g. the nukta).
4. Transliterate with the project's EncConverter (ICU `Any-Latin`).
5. Remove the "ignore after" characters.
6. Fold case. (Despite its name, `CompareCase` means *ignore* case.)
7. `CollapseDoubleCharacters`, if it's turned on.
8. Run the after-transliteration rules, i.e. the ordinary Inconsistency Settings.

## 1. Paratext sharing (done earlier, uncommitted)

- **`ParatextProjectLink.cs`**:
  - `LinkToParatext` / `UnlinkFromParatext` copy the project folder into `<My Paratext Projects>\<PT project>\shared\ConsistentSpellingFixer\<name>`, and then replace the local folder with a junction to it.
  - `FindSharedCscProjects` finds projects that other users have shared, so they show up in project selection as "Dogri (shared in Paratext project 'Dog')".
- **`ParatextProjectListForm`**: lets the user pick the Paratext project.
- **`CscProject.EnsureEncConverterExists`** no longer asks "A file exists by the name … .cct. Overwrite?". The file is there because the junction was just created, so the code now keeps the project's own `.cct` and registers it.

## 2. Rule order and before-transliteration rules

- **`AmbiguityListMap.cs`** was rewritten.
  - `AmbiguityListMaps : Hashtable` was replaced by `AmbiguityRules`, which keeps two ordered lists, `BeforeTransliteration` and `AfterTransliteration`.
  - Each `AmbiguityRule` applies its longest forms first.
  - `AmbiguityRules.Build(vss, enabledNames, out missing)` builds the lists in VSS file order.
- **The VSS schema** (`VsvXmlModel.cs` and `SpellFixerVernScript.xsd`) has a new optional attribute: `NormalizeRule/@stage="BeforeTransliteration"`.
  - Before-transliteration forms are NFKC-normalized when they're loaded.
  - An empty `<NormalizedForm />` deletes whatever the rule matches.
- **`CscProject`**:
  - `ApplyVernScriptSystem()` contains the setup that used to be inline in `Init()`.
  - `ReinitializeRules()` reloads the VSS and recomputes the KnownGood and WordsToCheck keys without reloading them from disk. It's called after the project dialog closes with OK.
- **Words that simplify to nothing.** `TransliteratedWord` returns null when nothing is left of a word after the ignore characters are removed (e.g. a word that's only a nukta or a soft hyphen).
  - The old Hashtable code happened to throw on that null while loading. The new code let it through, so `SimplifiedValue` was stored as null, and SFMConv crashed in the Bulk Picker (`ArgumentNullException` in `FindMatchingSimplifiedFormsInHashTableValue`).
  - Fix: `SimplifiedWord` now returns `""` instead of null, and `FindMatchingSimplifiedFormsInHashTableValue` skips empty keys, so words like that are never bundled.
- **`VSVs/Kangri.xml`**: "Nasal Endings" now comes before "Nasal". Before this fix, hash order happened to apply them that way. In file order, "Nasal" consumed the letters first and "Nasal Endings" never fired.
  - **Only the copy in the repo has been fixed.** Copies already installed under ProgramData still have the old order. The analyzer will suggest the move, because it detects "move this rule earlier" situations in general.

## 3. Analyze dialog

### Where it appears

- An **Anal&yze...** button on the *Inconsistency Settings* tab of the project dialog (`SFProjectForm.buttonAnalyze_Click`).
- **A weekly offer.** `CscProject.QueryForSpellingCorrectionsBulk` calls `OfferRuleAnalysis()`.
  - The offer appears when there are at least 100 corrections and at least 7 days have passed since the last analysis.
  - The date is recorded whether the user says Yes or No, as `LastRuleAnalysis` in the `.csf`.
  - Yes opens the project dialog with the analysis already started (`EditProject(startWithRuleAnalysis: true)`).

### The analyzer — `CscRuleAnalyzer.cs` (new, no UI)

- **Input:**
  - the corrections the user has made (KnownGood words, and the forms they replaced);
  - the WordsToCheck list;
  - the VSS;
  - a batch transliterator, which joins words with `\n` in batches of 500 and falls back to one word at a time.
- **Measure.** "Found" means the corrected form and the original form end up in the same bundle. Each rule is measured by:
  - **corrections gained or lost** when the rule is added or dropped;
  - **words newly bundled**: words pulled into groups that weren't grouped before;
  - **precision**: the fraction of those words that are corrections;
  - **yield**: corrections found per word newly bundled.
- **Typical precision and yield** are the medians across the project's current rules. Each project's own settings set the bar for its suggestions.

**What it suggests**, by `SuggestionKind`:

| Kind | Suggested when |
|---|---|
| `MoveRule` | Moving an enabled rule to the front finds more corrections. Recommended if it loses ≤ 10% of what it gains and precision ≥ 0.75 × typical. |
| `TurnOffRule` | The rule has no effect at all. Otherwise only when the project has at least 200 corrections (`!TooFewCorrections`), and dropping the rule loses ≤ max(2, 0.2%) of corrections or it's mostly false alarms. |
| `TurnOnRule` | An unchecked VSS rule would now pass the "worth it" test. |
| `AddRule` | Candidates are found by diffing (LCS) the uncovered corrections, at both the Latin level and the vernacular level. Vernacular-level ones become before-transliteration rules. |
| `CleanUpWordLists` / `RemoveWords` | WordsToCheck entries contain SFM markup or untrimmed punctuation, or are Latin-script words in a project where more than 80% of words aren't Latin. |
| `PreprocessRegex` | `\qt-s/e` milestones, `\fig` attributes, or word-break characters inside words. These are added to the VSS `PreprocessDataViaRegex` list. |
| `IgnoreCharacter` / `PunctuationCharacter` | Format characters, the soft hyphen, curly quotes, or untrimmed characters at the edges of words. |

**"Worth it" criteria for new rules (`IsWorthItCriteria`).** A rule is recommended only if all of these hold:
- net gain ≥ max(5, 0.5% of corrections);
- lost ≤ 10% of gained;
- precision ≥ 0.9 × typical;
- yield ≥ 0.5 × typical.

**Rules are never deleted.** Turning one off only unchecks it. Every analysis re-tests the unchecked rules, so a rule turned off early in a project, when there wasn't much data, can come back.

**Deduplicating candidates.** Only recommended or "clean" candidates (loss ≤ 10%) can suppress an overlapping candidate (70% overlap). Without this rule, a lossy Latin candidate hid a better vernacular one.

**Plain-language text.** `DescribeInPlainLanguage` fills `Heading`, `Note`, `Question` and `Action`.
- Headings show the difference in the vernacular, e.g. "Spellings that differ by ◌े / ◌ै or ए / ऐ". `ShowPart` puts a dotted circle (◌) before combining marks.
- `Describe(char)` names invisible characters, e.g. "soft hyphen (U+00AD)".

**Combined estimate.** `EstimateCombined(suggestions, out newlyBundled)` estimates the effect of everything the user said Yes to, taken together.

### The dialog — `RuleAnalysisForm.cs` (new)

It's a step-through dialog aimed at mother-tongue translators: data first, one suggestion at a time.

- **Tabs:**
  1. **Overview**: progress, then a plain summary.
  2. **Your settings**: Move, Turn off and Turn on.
  3. **New settings**: new rules.
  4. **Clean up**: data clean-ups.
  5. **More ideas**: suggestions that aren't recommended, and previously dismissed ones.
  6. **Summary**: what Apply will do, plus the combined estimate.
- **Back / Next** step through suggestions, then on to the next tab.
- **Each suggestion page shows, from top to bottom:**
  1. heading;
  2. one-sentence note;
  3. *corrections you've made that this would have found*;
  4. *other words it would put together*;
  5. the question;
  6. **Yes / No / No, and don't suggest this again**;
  7. a "Show the details" link for the numbers and the technical rule.
- **No answer is pre-selected.** Only an explicit Yes is applied. Unanswered suggestions count as No this time and come back at the next analysis. The Summary tab says how many are unanswered.
- **"Don't suggest this again"** is stored in the `.csf` as `<DismissedRuleSuggestions><Suggestion>`, keyed by `RuleSuggestion.Key`, i.e. what the suggestion changes. Because the `.csf` is in the project folder, dismissals sync through Paratext along with it.
  - Dismissals are recorded even if the user cancels.
  - A checkbox on the Overview tab shows dismissed suggestions again. Answering Yes or No to one of them un-dismisses it.
- **`ShowForFolder(folder, vss, font)`** opens the dialog view-only on any project folder. It's for trying the analysis out (see the testing section).

### Applying — `SFProjectForm.ApplyRuleAnalysisSuggestions`

| Suggestion | What Apply does |
|---|---|
| Turn off / turn on | Toggles the rule's checkbox. |
| Move | Moves the `Distinction` and its checkbox to index 0. |
| Add | Inserts a new `Distinction`, with a unique name, at index 0. |
| Punctuation, ignore, regex | Adds to the in-memory VSS. |
| Word-list fixes | `CscProject.ApplyWordListFixes`, which merges counts and contexts. **These are saved immediately**; they don't wait for OK on the project dialog. |

When any setting changed, Apply then:
- sets `m_bSaveVSS`;
- suggests a new VSS name, "<VSS> (<project>)";
- sets `m_bSaveVssInProjectFolder`, so the save dialog opens in the project folder.

### VSS files in the project folder

- **Which VSS is used.** A VSS file in the project's own folder takes precedence over the global one with the same name (`CscProject.CreateVernacularScriptSystemFileSpec(name, projectName)`). The project folder can be the junction into Paratext's `shared` folder, so a project-specific VSS travels with the project.
- **How a VSS file is recognized.** The word-list data files in the project folder are also `.xml`, so `IsVernScriptSystemFile` reads only the root element (`VernScriptSystem`) with an `XmlReader`.
- **The project dialog's Vernacular Script Type combo** lists VSS files from the project folder too (`GetProjectVernacularScriptSystemNames`).

## 4. SFM markers in words

`CscProject.AddWordToCheckList` now splits SFM markers off a word before trimming it (`ContainsMarkup` / `SplitOutMarkup`). The regexes involved:
- `s_rxSfmMarker` matches `\w`, `\+w*`, `\*` and similar markers.
- `s_rxAttributeList` matches a `|attr="…"` list.

The analyzer's `CleanUpWordLists` suggestion uses the same functions to fix entries already in the list.

## 5. Bulk Picker: Pause button

`BulkAmbiguityPicker` has a new **Pause** button (`buttonPause`), in a new third column.
- A `ManualResetEvent` (`_bundleProcessingRunning`) controls the processing thread. While paused, the thread waits but still checks for cancellation.
- `StopBackgroundWorker` sets the event so the thread doesn't hang.
- The stats label says "(searching for more is paused)".

## 6. Bulk Picker: speed, right-click menu, examples pane, project settings

### Why Pause took a minute or more, and the fix

Two things were wrong, and both had been there before this work:

1. **The search thread interrupted the UI constantly.** It called `Control.Invoke` once or twice for *every word* it checked, mostly to update the progress bar. Windows always delivers those posted messages before mouse and keyboard input, so a click waited behind them.
2. **Adding each row re-measured the whole grid.** The text columns were set to auto-size to `AllCells`, so every new row re-measured every row already there. That's O(n²): on the Dogri copy, adding 646 rows of Devanagari took about 60 s of solid UI-thread time.

The fixes:

- **No calls into the UI from the search thread.** `StartBundleProcessing` no longer blocks `OnShown` with a `DoEvents` loop. The thread puts each bundle it finds into a `ConcurrentQueue`.
- **A timer fills the grid.** `BundleTimer_Tick` runs every 200 ms. It adds bundles for at most about 100 ms per tick and updates the progress bar.
- **An error on the search thread no longer kills SFMConv.** It's shown in a message box, and the bundles found so far stay in the grid.
- **Columns only grow.** `TurnOffColumnAutoSizing` and `WidenColumnsToFit` widen a column from the new row's text only.
- **The search no longer scans whole lists for each word.** `GetAmbiguousBundle` takes optional `SimplifiedFormIndex`es, which index the Known Good and Words to Check lists by normalized form (in `MyCollections.cs`). Before, each word meant a pass over both lists, which made the search O(n²).

**Result on the Dogri copy:** the whole search, including filling the grid, now takes under 2 s instead of about 62 s. A real mouse click on Pause during the search was handled in 0.7 s.

### Right-click menu on the middle ("Select") column

Right-clicking the middle column used to split the row (or undo the split). It now opens a menu:

- the right-hand word is correct (→);
- the left-hand word is correct (←);
- both are correct (↔);
- undecided;
- split into two rows;
- undo the split;
- edit the spellings (the same dialog as right-clicking the right-hand word);
- **Why are these words shown together?**

The Apps key and Shift+F10 open the same menu for the selected row. Every way of choosing now goes through `SetChoice`, so the "N inconsistencies left" count stays right when changing from one choice to another; before, it was off by one each time.

### "Why are these words shown together?" (`RuleTraceForm.cs`)

- **What it shows:** each word's steps from `CscProject.TraceSimplification`, i.e. the same steps as `TransliteratedWord` + `SimplifiedWord`, recording each change.
- **The checklist:** one checked item for each Inconsistency Setting that changed either word.
- **Unchecking** enables **Save changes and Re-analyze data from scratch**. That button:
  1. saves the choices made so far (`ProcessChosenRecords(true)`);
  2. calls `CscProject.DisableAmbiguityRules`, which removes the setting from the project's rule names, saves the `.csf` and calls `ReinitializeRules`;
  3. refills the list from the project's Words to Check (`RefillWordsToCheck`) and starts the search again.

### View menu additions

- **Show Examples Pane:** a toggle.
- **Edit Project Settings…**: stops the search and opens `SFProjectForm`. If the user clicks OK, it offers to save the choices made so far and start over from scratch; otherwise the search carries on.
- **Save and Look for Inconsistencies Again from Scratch.**

### Examples pane (`ExamplesPane.cs`)

- **Replaces the tooltip-like popup.** It's a resizable pane to the right of the grid, which shows the examples of the word under the mouse, or of the selected row.
- **The word is highlighted** in bold on a yellow background in each example.
- **Pin** keeps the current word's examples while the mouse moves on, e.g. to scroll through them.
- **Two new user settings:** `ShowExamplesPane` and `ExamplesPaneWidth`. When the pane is hidden, the old popup is used.

### "Normalized" column header

The "Transliteration" columns are headed **Normalized** when *Show Transliteration* is unchecked, because they then show the form that's actually compared. Toggling the setting now also refreshes the rows already shown.

When it's unchecked, only the left-hand Normalized column is shown: both words in a row always have the same normalized form, because that's why they're grouped. When it's checked, both columns are shown, because the two transliterations differ.

### Rows where both words are already known good start as ↔

`UpdateProgress` presets these rows to "both correct" and doesn't count them as inconsistencies left. Such rows only appear when a bundle also contains at least one unchecked word, because bundles are started only from words on the Words to Check list. On the Dogri sandbox, 151 of the 646 rows were preset, and the count left went from 646 to 495.

### No more "data left over from a previous session" prompt

`CscProject.QueryForSpellingCorrectionsBulk` now always builds the list from the project's Words to Check list. It deletes any `WordsToCheckBulk` file an earlier version left behind, and no longer saves one. Nothing is lost, because only words the user decides on are taken off the Words to Check list, so it still has every word left undecided.

## 7. Analysis: noisy suggestions are left out

Non-recommended setting changes, i.e. new rules and turning on unused ones, aren't suggested at all (`IsTooNoisy`) if either:
- they'd lose more than 25% of what they gain; or
- they'd find fewer than 0.3 × the typical number of corrections per word they bundle.

Users tend to say Yes to everything suggested, and rules like these bury the real inconsistencies in false alarms. The ones left out:
- are listed in the report (`RuleAnalysisResult.LeftOut`);
- are counted on the Overview tab;
- are re-tested at every analysis.

On the Dogri copy, the "More ideas" setting changes went from 16 to 2. The ones dropped include:

| Rule | Corrections found | Extra words to check |
|---|---|---|
| `'ā' ~ 'a'` | 108 | 1,731 |
| `'ा' ~ 'े'` | 26 | 2,129 |
| with or without 'a' | 374, losing 129 | 1,086 |

Notes on rule changes also now say e.g. "That's about 1 real mistake for every 4 words it would show you (your settings typically: about 1 in 2)."

## Results on the Dogri sandbox

| | Corrections found |
|---|---|
| Current settings (Kangri VSS) | 2,571 of 5,495 (46.8%) |
| After all recommended suggestions | 3,203 (58.3%), with 1,132 more words to check |

The analysis took about 6 s.

**Recommended:**
- Move "Nasal Endings" earlier.
- Turn off "Vowel Glides" and "Nukta".
- Add rules for:
  - ◌े / ◌ै (Latin `ai`~`ē`)
  - with or without `y`
  - `व` ~ `ब`
  - `yē` ~ `ai`
  - with or without `v`
- Clean up 924 WordsToCheck entries.
- Remove 246 Latin-script words.
- Add regexes for `\qt-s/e` milestones and `\fig` attributes, plus word breaks at `/` and `,`.
- Ignore the soft hyphen and `’`.

## How it was tested

There are no unit tests for any of this. All checks were done with PowerShell scripts that load the built DLLs by reflection and drive the forms.

- **Location.** The scripts and the sandbox are in the Temp folder of the session that did this work: `%LOCALAPPDATA%\Temp\claude\651fdfd1389dd317\680f6bec-28aa-453b-b2d6-49aaf8ba0c26\scratchpad\`.
  - The sandbox has a copy of the data in `sandbox\Dogri` and the VSS in `sandbox\Kangri.xml`.
  - Temp folders can be cleaned up, so copy these somewhere permanent if you want to keep them.
- **To try the dialog view-only on a fresh copy of the data:** `powershell.exe -STA -ExecutionPolicy Bypass -File <scratchpad>\try_analysis_dialog.ps1`
- **Other smoke scripts:**

  | Script | What it checks |
  |---|---|
  | `smoke_wizard.ps1` | Takes screenshots of the tabs. |
  | `smoke_dismissed.ps1` | Dismissed suggestions are hidden and counted. |
  | `smoke_undecided.ps1` | Clicking Next through everything leaves 0 Yes answers, and answering one gives 1. |

- **Windows PowerShell 5.1** misreads scripts that contain Devanagari unless they're saved as UTF-8 *with* a BOM.

## Building

- **MSBuild path:** `C:\Program Files\Microsoft Visual Studio\18\Insiders\MSBuild\Current\Bin\MSBuild.exe`
- **x86:** `"EncConverters 2019.sln" /restore /t:SpellingFixer30 /p:Platform=Win32`. Building the project alone fails with NETSDK1047, because EncConverterHostExe's assets have no win-x86.
- **Switching platforms.** x64 and Win32 share one restore file, so build with `/restore` again after switching.
- **RegAsm warnings** about administrator permissions are expected and harmless.

## Not done yet / next steps

- [ ] **The Bulk Picker paths that write to the project haven't been tried**: *Save changes and Re-analyze*, *Edit Project Settings…*, and *Look for Inconsistencies Again from Scratch*. The test (`smoke_picker.ps1`) builds the project in memory from the sandbox copy, and those paths would have saved to ProgramData.
- [ ] **Apply has not been tried end to end** in the real project editor or in SFM Converter. So far only the dialog itself has been driven in the sandbox, with Apply turned off.
- [ ] Pack NuGet 1.0.4 to `C:\btmp\LocalNuget` and update silconverters, so the whole flow can be tried in SFM Converter.
- [ ] Commit, once it has been tried.
- [ ] Fix the Kangri VSS on users' machines. Either ship the reordered `Kangri.xml` or rely on the analyzer's Move suggestion.
- [ ] Possibly handle more cases in the `\fig` regex. It keeps only the first field of USFM 2 figures, and skips figures that contain a nested marker.
- [ ] Consider unit tests for `AmbiguityRules` (order, before-transliteration, deletion) and for `SplitOutMarkup`. Both are pure functions and easy to test.

## Files

| File | Change |
|---|---|
| `AmbiguityListMap.cs` | Rewritten: `AmbiguityRule`, `AmbiguityRules` |
| `CscProject.cs` | Rule setup refactored, markup splitting, weekly offer, word-list fixes, project-folder VSS, dismissed keys |
| `CscProjectStore.cs` | `LastRuleAnalysis`, `DismissedRuleSuggestions`, `UpdateProjectFile` helper |
| `CscRuleAnalyzer.cs` | **New**: the analysis |
| `RuleAnalysisForm.cs` | **New**: step-through dialog |
| `SFProjectForm.cs` / `.Designer.cs` | Analyze button, applying suggestions, saving the VSS in the project folder, `DistinctionRuleCheckBox.ToString` fix (it chopped the last two characters) |
| `BulkAmbiguityPicker.cs` / `.Designer.cs` | Pause button, search without calls into the UI, columns that only grow, right-click menu, View menu items, examples pane, "Normalized" header |
| `RuleTraceForm.cs` | **New**: "Why are these words shown together?" |
| `ExamplesPane.cs` | **New**: the examples side pane |
| `Properties/Settings.*`, `app.config` | `ShowExamplesPane`, `ExamplesPaneWidth` |
| `MyCollections.cs` | Parameterless `SpellFixerWord()` constructor; `SimplifiedFormIndex` |
| `VSVs/VsvXmlModel.cs`, `SpellFixerVernScript.xsd` | `stage` attribute |
| `VSVs/Kangri.xml` | "Nasal Endings" moved before "Nasal" |
| `ParatextProjectLink.cs`, `ParatextProjectListForm.*` | **New** (earlier work): Paratext sharing |
| `SelectProject.*`, `DistinctionTreeEditor.*`, `../../Package.Release.nuspec` | Earlier work: shared projects in project selection, version 1.0.3 |
| `SpellingFixer30.csproj` | New files added |
