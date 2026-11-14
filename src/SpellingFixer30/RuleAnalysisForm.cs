using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace SpellingFixer30
{
    internal enum SuggestionChoice
    {
        Undecided,      // (the user hasn't answered yet: treated like 'No' for this time, but it'll be suggested again)
        Yes,
        No,
        NoAndDontSuggestAgain
    }

    /// <summary>
    /// Runs the Inconsistency Settings analysis (CscRuleAnalyzer) and steps the user thru its suggestions, one at a time,
    /// in tabs by kind (showing examples from their own data first, then asking yes/no/don't-ask-again). The caller applies
    /// the accepted suggestions (see SFProjectForm.ApplyRuleAnalysisSuggestions) and records the dismissed ones.
    /// </summary>
    internal class RuleAnalysisForm : Form
    {
        private readonly CscRuleAnalyzer _analyzer;
        private readonly bool _canApply;
        private readonly Font _vernacularFont;
        private readonly HashSet<string> _previouslyDismissed;
        private readonly CancellationTokenSource _cancel = new CancellationTokenSource();
        private RuleAnalysisResult _result;
        private readonly Dictionary<RuleSuggestion, SuggestionChoice> _choices = new Dictionary<RuleSuggestion, SuggestionChoice>();

        // the step-thru order: one entry per tab (the Overview and Summary tabs have no suggestions)
        private readonly List<TabPage> _pages = new List<TabPage>();
        private readonly Dictionary<TabPage, List<RuleSuggestion>> _pageSuggestions = new Dictionary<TabPage, List<RuleSuggestion>>();
        private readonly Dictionary<TabPage, SuggestionView> _pageViews = new Dictionary<TabPage, SuggestionView>();
        private readonly Dictionary<TabPage, int> _pageIndex = new Dictionary<TabPage, int>();

        private readonly TabControl _tabs = new TabControl();
        private readonly TabPage _pageOverview = new TabPage("Overview");
        private readonly TabPage _pageSummary = new TabPage("Summary");
        private readonly Label _labelOverview = new Label();
        private readonly Label _labelProgress = new Label();
        private readonly ProgressBar _progressBar = new ProgressBar();
        private readonly CheckBox _checkShowDismissed = new CheckBox();
        private readonly Label _labelSummary = new Label();
        private readonly Label _labelEstimate = new Label();
        private readonly Button _buttonBack = new Button();
        private readonly Button _buttonNext = new Button();
        private readonly Button _buttonApply = new Button();
        private readonly Button _buttonCancel = new Button();
        private readonly Button _buttonCopy = new Button();
        private readonly ToolTip _toolTip = new ToolTip();

        public bool AnalysisCompleted { get; private set; }

        /// <summary>
        /// The suggestions the user said 'Yes' to (when the dialog result is OK)
        /// </summary>
        public List<RuleSuggestion> CheckedSuggestions => _choices.Where(kvp => kvp.Value == SuggestionChoice.Yes).Select(kvp => kvp.Key).ToList();

        /// <summary>
        /// The keys of the suggestions not to show again: the ones dismissed before (unless the user changed their mind
        /// about one that was shown) plus the ones the user just dismissed
        /// </summary>
        public HashSet<string> DismissedKeys
        {
            get
            {
                var keys = new HashSet<string>(_previouslyDismissed);
                foreach (var kvp in _choices)
                {
                    if (kvp.Value == SuggestionChoice.NoAndDontSuggestAgain)
                        keys.Add(kvp.Key.Key);
                    else if (_checkShowDismissed.Checked)
                        keys.Remove(kvp.Key.Key);   // (it was shown again and the user didn't dismiss it this time)
                }
                return keys;
            }
        }

        public RuleAnalysisForm(CscRuleAnalyzer analyzer, Font vernacularFont, bool canApply = true, IEnumerable<string> dismissedKeys = null)
        {
            _analyzer = analyzer;
            _canApply = canApply;
            _vernacularFont = vernacularFont;
            _previouslyDismissed = new HashSet<string>(dismissedKeys ?? Enumerable.Empty<string>());
            InitializeComponents();
        }

        private void InitializeComponents()
        {
            Text = "Analyze Inconsistency Settings";
            Size = new Size(940, 720);
            MinimumSize = new Size(720, 560);
            StartPosition = FormStartPosition.CenterParent;
            ShowInTaskbar = false;
            MinimizeBox = false;
            Font = new Font(SystemFonts.MessageBoxFont.FontFamily, 10f);

            var layout = new TableLayoutPanel { Dock = DockStyle.Fill, ColumnCount = 1, RowCount = 2, Padding = new Padding(8) };
            layout.RowStyles.Add(new RowStyle(SizeType.Percent, 100));
            layout.RowStyles.Add(new RowStyle(SizeType.AutoSize));
            Controls.Add(layout);

            _tabs.Dock = DockStyle.Fill;
            _tabs.SelectedIndexChanged += (s, e) => OnPageChanged();
            layout.Controls.Add(_tabs, 0, 0);

            // Overview
            var overview = new TableLayoutPanel { Dock = DockStyle.Fill, ColumnCount = 1, RowCount = 4, Padding = new Padding(12) };
            overview.RowStyles.Add(new RowStyle(SizeType.AutoSize));
            overview.RowStyles.Add(new RowStyle(SizeType.AutoSize));
            overview.RowStyles.Add(new RowStyle(SizeType.AutoSize));
            overview.RowStyles.Add(new RowStyle(SizeType.Percent, 100));
            _labelOverview.AutoSize = true;
            _labelOverview.MaximumSize = new Size(860, 0);
            _labelOverview.Text = "The Spelling Fixer is comparing its Inconsistency Settings with the spelling corrections you've made...";
            _labelProgress.AutoSize = true;
            _labelProgress.Margin = new Padding(3, 16, 3, 3);
            _labelProgress.Text = "Starting...";
            _progressBar.Width = 600;
            _checkShowDismissed.AutoSize = true;
            _checkShowDismissed.Visible = false;
            _checkShowDismissed.Margin = new Padding(3, 16, 3, 3);
            _checkShowDismissed.CheckedChanged += (s, e) => BuildPages();
            overview.Controls.Add(_labelOverview, 0, 0);
            overview.Controls.Add(_labelProgress, 0, 1);
            overview.Controls.Add(_progressBar, 0, 2);
            overview.Controls.Add(_checkShowDismissed, 0, 3);
            _pageOverview.Controls.Add(overview);
            _tabs.TabPages.Add(_pageOverview);

            // Summary
            var summary = new TableLayoutPanel { Dock = DockStyle.Fill, ColumnCount = 1, RowCount = 2, Padding = new Padding(12), AutoScroll = true };
            summary.RowStyles.Add(new RowStyle(SizeType.AutoSize));
            summary.RowStyles.Add(new RowStyle(SizeType.AutoSize));
            _labelSummary.AutoSize = true;
            _labelSummary.MaximumSize = new Size(860, 0);
            _labelSummary.Font = new Font(_vernacularFont.FontFamily, 11f);
            _labelEstimate.AutoSize = true;
            _labelEstimate.MaximumSize = new Size(860, 0);
            _labelEstimate.Margin = new Padding(3, 16, 3, 3);
            summary.Controls.Add(_labelSummary, 0, 0);
            summary.Controls.Add(_labelEstimate, 0, 1);
            _pageSummary.Controls.Add(summary);

            // buttons
            var buttons = new TableLayoutPanel { Dock = DockStyle.Fill, AutoSize = true, ColumnCount = 6, RowCount = 1 };
            buttons.ColumnStyles.Add(new ColumnStyle(SizeType.AutoSize));
            buttons.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100));
            for (int i = 0; i < 4; i++)
                buttons.ColumnStyles.Add(new ColumnStyle(SizeType.AutoSize));
            layout.Controls.Add(buttons, 0, 1);

            _buttonCopy.Text = "Cop&y report";
            _buttonCopy.AutoSize = true;
            _buttonCopy.Enabled = false;
            _buttonCopy.Click += (s, e) => { if (_result != null) Clipboard.SetText(_result.ToReport()); };
            _toolTip.SetToolTip(_buttonCopy, "Copy the full analysis (with all the numbers) to the clipboard, e.g. to send to a consultant");

            _buttonBack.Text = "< &Back";
            _buttonBack.AutoSize = true;
            _buttonBack.Enabled = false;
            _buttonBack.Click += (s, e) => Step(-1);

            _buttonNext.Text = "&Next >";
            _buttonNext.AutoSize = true;
            _buttonNext.Enabled = false;
            _buttonNext.Click += (s, e) => Step(+1);

            _buttonApply.Text = _canApply ? "&Apply" : "&Close";
            _buttonApply.AutoSize = true;
            _buttonApply.Enabled = false;
            _buttonApply.Click += (s, e) => { DialogResult = _canApply ? DialogResult.OK : DialogResult.Cancel; Close(); };
            _toolTip.SetToolTip(_buttonApply, "Make the changes you said 'Yes' to (see the Summary tab)");

            _buttonCancel.Text = "Cancel";
            _buttonCancel.AutoSize = true;
            _buttonCancel.Click += (s, e) => { _cancel.Cancel(); DialogResult = DialogResult.Cancel; Close(); };
            CancelButton = _buttonCancel;

            buttons.Controls.Add(_buttonCopy, 0, 0);
            buttons.Controls.Add(_buttonBack, 2, 0);
            buttons.Controls.Add(_buttonNext, 3, 0);
            buttons.Controls.Add(_buttonApply, 4, 0);
            buttons.Controls.Add(_buttonCancel, 5, 0);
        }

        protected override async void OnShown(EventArgs e)
        {
            base.OnShown(e);

            IProgress<Tuple<string, int>> reporter = new Progress<Tuple<string, int>>(p =>
            {
                _labelProgress.Text = p.Item1;
                _progressBar.Value = Math.Max(0, Math.Min(100, p.Item2));
            });

            try
            {
                UseWaitCursor = true;
                _result = await Task.Run(() => _analyzer.Analyze((s, n) => reporter.Report(Tuple.Create(s, n)), _cancel.Token));
                AnalysisCompleted = true;
            }
            catch (OperationCanceledException)
            {
                return;
            }
            catch (Exception ex)
            {
                if (!IsDisposed)
                    MessageBox.Show(this, $"The analysis failed: {ex.Message}", CscProject.ApplicationCaption);
                return;
            }
            finally
            {
                UseWaitCursor = false;
            }

            if (IsDisposed)
                return;

            // nothing is pre-selected (even the recommended ones), so only what the user explicitly says 'Yes' to gets applied
            foreach (var suggestion in _result.Suggestions)
                _choices[suggestion] = _previouslyDismissed.Contains(suggestion.Key)
                                        ? SuggestionChoice.NoAndDontSuggestAgain
                                        : SuggestionChoice.Undecided;

            _labelProgress.Visible = false;
            _progressBar.Visible = false;
            var numDismissed = _result.Suggestions.Count(s => _previouslyDismissed.Contains(s.Key));
            _checkShowDismissed.Text = $"Also show the {numDismissed} suggestion(s) you asked not to be shown again";
            _checkShowDismissed.Visible = numDismissed > 0;
            _buttonApply.Enabled = true;
            _buttonCopy.Enabled = true;
            BuildPages();
        }

        private bool IsShown(RuleSuggestion s) => _checkShowDismissed.Checked || !_previouslyDismissed.Contains(s.Key);

        /// <summary>
        /// (Re)builds the tabs: the recommended suggestions by kind, then all the others in 'More ideas'
        /// </summary>
        private void BuildPages()
        {
            if (_result == null)
                return;

            _tabs.SuspendLayout();
            foreach (var page in _pages.Where(p => (p != _pageOverview) && (p != _pageSummary)))
            {
                _tabs.TabPages.Remove(page);
                page.Dispose();
            }
            _tabs.TabPages.Remove(_pageSummary);
            _pages.Clear();
            _pageSuggestions.Clear();
            _pageViews.Clear();
            _pageIndex.Clear();
            _pages.Add(_pageOverview);

            var shown = _result.Suggestions.Where(IsShown).ToList();
            bool IsMain(RuleSuggestion s) => s.Recommended && !_previouslyDismissed.Contains(s.Key);
            var settings = shown.Where(s => IsMain(s) && ((s.Kind == SuggestionKind.MoveRule) || (s.Kind == SuggestionKind.TurnOffRule) || (s.Kind == SuggestionKind.TurnOnRule))).ToList();
            var added = shown.Where(s => IsMain(s) && (s.Kind == SuggestionKind.AddRule)).ToList();
            var cleanUp = shown.Where(s => IsMain(s) && !s.IsRuleChange).ToList();
            var others = shown.Where(s => !IsMain(s)).ToList();

            AddPage("Your settings", settings, "Changes to the settings you have now.");
            AddPage("New settings", added, "New settings, learned from the corrections you've made that the current settings didn't find.");
            AddPage("Clean up", cleanUp, "Things in your data that keep words from being compared properly.");
            AddPage("More ideas", others, "Less certain ideas: each would find some more of the kinds of mistakes you correct, but would " +
                                         "probably also show you more words that are fine (or lose some that are found now). Look at the examples " +
                                         "to decide whether they're worth it for your language.");

            _pages.Add(_pageSummary);
            _tabs.TabPages.Add(_pageSummary);
            _tabs.ResumeLayout();

            var sb = new StringBuilder();
            sb.AppendLine($"The Spelling Fixer compared its Inconsistency Settings with the {_result.NumCorrections} spelling corrections you've made in the " +
                          $"'{_result.ProjectName}' project. With the current settings, it would have found {RuleAnalysisResult.Percent(_result.NumCorrectionsBundledNow, _result.NumCorrections)} of them.");
            sb.AppendLine();
            sb.AppendLine("Here's what it suggests:");
            sb.AppendLine($"    •  {settings.Count} change(s) to your current settings");
            sb.AppendLine($"    •  {added.Count} new setting(s)");
            sb.AppendLine($"    •  {cleanUp.Count} thing(s) to clean up in your data");
            sb.AppendLine($"    •  {others.Count} other, less certain idea(s)");
            if (_result.LeftOut.Count > 0)
                sb.AppendLine($"({_result.LeftOut.Count} other possible setting(s) aren't shown, because they would show you far too many words that are " +
                              "fine for each mistake they'd find. They're checked again each time you analyze.)");
            sb.AppendLine();
            sb.AppendLine("Click 'Next' to go thru them one at a time. Each one first shows examples from your own data, and then asks whether " +
                          "to make the change. Only the ones you answer 'Yes' to will be changed, and nothing is changed until you click 'Apply' " +
                          "(you can see everything you've said 'Yes' to on the Summary tab).");
            if (_result.TooFewCorrections)
            {
                sb.AppendLine();
                sb.AppendLine("(There aren't many corrections yet, so none of your settings are suggested to be turned off. Analyze again once you've made more corrections.)");
            }
            _labelOverview.Text = sb.ToString();

            _tabs.SelectedTab = _pageOverview;
            OnPageChanged();
        }

        private void AddPage(string title, List<RuleSuggestion> suggestions, string introduction)
        {
            if (suggestions.Count == 0)
                return;
            var page = new TabPage($"{title} ({suggestions.Count})");
            var view = new SuggestionView(_vernacularFont, introduction) { Dock = DockStyle.Fill };
            view.ChoiceChanged += (s, choice) => _choices[view.Suggestion] = choice;
            page.Controls.Add(view);
            _pages.Add(page);
            _pageSuggestions[page] = suggestions;
            _pageViews[page] = view;
            _pageIndex[page] = 0;
            _tabs.TabPages.Add(page);
        }

        private void ShowSuggestion(TabPage page)
        {
            var list = _pageSuggestions[page];
            var index = _pageIndex[page];
            var suggestion = list[index];
            _pageViews[page].Show(suggestion, _choices[suggestion], $"{index + 1} of {list.Count}");
        }

        private void OnPageChanged()
        {
            var page = _tabs.SelectedTab;
            if (page == null)
                return;
            if (_pageSuggestions.ContainsKey(page))
                ShowSuggestion(page);
            else if (page == _pageSummary)
                UpdateSummary();
            UpdateButtons();
        }

        private void UpdateButtons()
        {
            var page = _tabs.SelectedTab;
            var pagePos = _pages.IndexOf(page);
            var hasData = _result != null;
            _buttonBack.Enabled = hasData && ((pagePos > 0) || (_pageSuggestions.ContainsKey(page) && (_pageIndex[page] > 0)));
            _buttonNext.Enabled = hasData && (page != _pageSummary);
        }

        /// <summary>
        /// Goes to the next (or previous) suggestion, moving on to the next (or previous) tab at the end of one
        /// </summary>
        private void Step(int direction)
        {
            var page = _tabs.SelectedTab;
            if (_pageSuggestions.TryGetValue(page, out var list))
            {
                var index = _pageIndex[page] + direction;
                if ((index >= 0) && (index < list.Count))
                {
                    _pageIndex[page] = index;
                    ShowSuggestion(page);
                    UpdateButtons();
                    return;
                }
            }

            var pagePos = _pages.IndexOf(page) + direction;
            if ((pagePos < 0) || (pagePos >= _pages.Count))
                return;
            var newPage = _pages[pagePos];
            if (_pageSuggestions.TryGetValue(newPage, out var newList))
                _pageIndex[newPage] = (direction > 0) ? 0 : newList.Count - 1;
            _tabs.SelectedTab = newPage;    // (OnPageChanged shows it)
        }

        private void UpdateSummary()
        {
            var yes = _result.Suggestions.Where(s => _choices[s] == SuggestionChoice.Yes).ToList();
            var dismiss = _result.Suggestions.Where(s => (_choices[s] == SuggestionChoice.NoAndDontSuggestAgain) && !_previouslyDismissed.Contains(s.Key)).ToList();
            var unanswered = _result.Suggestions.Count(s => IsShown(s) && (_choices[s] == SuggestionChoice.Undecided));

            var sb = new StringBuilder();
            if (yes.Count == 0)
                sb.AppendLine("You haven't said 'Yes' to any of the suggestions, so nothing will be changed.");
            else
            {
                sb.AppendLine(_canApply ? "When you click 'Apply', these changes will be made:" : "You said 'Yes' to these:");
                foreach (var s in yes)
                    sb.AppendLine("    •  " + (s.Action ?? s.Heading ?? s.Title));
            }
            if (dismiss.Count > 0)
            {
                sb.AppendLine();
                sb.AppendLine("These won't be suggested again:");
                foreach (var s in dismiss)
                    sb.AppendLine("    •  " + (s.Action ?? s.Heading ?? s.Title));
            }
            if (unanswered > 0)
            {
                sb.AppendLine();
                sb.AppendLine($"You haven't answered {unanswered} of the suggestions. They won't be changed now, but they'll be suggested again the next time.");
            }
            if (yes.Any(s => s.IsRuleChange))
            {
                sb.AppendLine();
                sb.AppendLine("The setting changes take effect when you click OK on the project dialog. Clean-ups of the word lists are saved right away.");
            }
            _labelSummary.Text = sb.ToString();
            _ = EstimateAsync(yes);
        }

        private async Task EstimateAsync(List<RuleSuggestion> yes)
        {
            if (!yes.Any(s => s.IsRuleChange))
            {
                _labelEstimate.Text = null;
                return;
            }
            _labelEstimate.Text = "Estimating what the setting changes would do together...";
            try
            {
                int newlyBundled = 0;
                var found = await Task.Run(() => _analyzer.EstimateCombined(yes, out newlyBundled));
                _labelEstimate.Text = $"Together, these setting changes would find {RuleAnalysisResult.Percent(found, _result.NumCorrections)} of your corrections " +
                                      $"(now: {RuleAnalysisResult.Percent(_result.NumCorrectionsBundledNow, _result.NumCorrections)}), and would show you " +
                                      $"{newlyBundled} more words to check.";
            }
            catch (Exception ex)
            {
                _labelEstimate.Text = $"Unable to estimate: {ex.Message}";
            }
        }

        protected override void OnFormClosing(FormClosingEventArgs e)
        {
            _cancel.Cancel();
            base.OnFormClosing(e);
        }

        /// <summary>
        /// For trying out the analysis on a (sandbox copy of a) project folder, without loading the project or being able
        /// to apply anything (e.g. from PowerShell: [SpellingFixer30.RuleAnalysisForm]::ShowForFolder(folder, vssFile))
        /// </summary>
        internal static void ShowForFolder(string projectFolder, string vssFilePath, Font vernacularFont = null)
        {
            var analyzer = CscRuleAnalyzer.CreateForFolder(projectFolder, vssFilePath);
            using (var dlg = new RuleAnalysisForm(analyzer, vernacularFont ?? new Font("Arial Unicode MS", 14f), canApply: false))
                dlg.ShowDialog();
        }
    }

    /// <summary>
    /// Shows one suggestion: a heading, examples from the user's data, the question (Yes / No / don't suggest again), and
    /// (on request) the details
    /// </summary>
    internal class SuggestionView : UserControl
    {
        private readonly Label _labelIntro = new Label();
        private readonly Label _labelCounter = new Label();
        private readonly Label _labelHeading = new Label();
        private readonly Label _labelNote = new Label();
        private readonly Label _labelExamples = new Label();
        private readonly RichTextBox _textExamples = new RichTextBox();
        private readonly Label _labelGroups = new Label();
        private readonly RichTextBox _textGroups = new RichTextBox();
        private readonly Label _labelQuestion = new Label();
        private readonly RadioButton _radioYes = new RadioButton();
        private readonly RadioButton _radioNo = new RadioButton();
        private readonly RadioButton _radioNever = new RadioButton();
        private readonly LinkLabel _linkDetails = new LinkLabel();
        private readonly RichTextBox _textDetails = new RichTextBox();
        private readonly TableLayoutPanel _layout = new TableLayoutPanel();
        private bool _settingChoice;

        public RuleSuggestion Suggestion { get; private set; }
        public event EventHandler<SuggestionChoice> ChoiceChanged;

        public SuggestionView(Font vernacularFont, string introduction)
        {
            _layout.Dock = DockStyle.Fill;
            _layout.ColumnCount = 2;
            _layout.Padding = new Padding(8);
            _layout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100));
            _layout.ColumnStyles.Add(new ColumnStyle(SizeType.AutoSize));
            Controls.Add(_layout);

            _labelIntro.AutoSize = true;
            _labelIntro.MaximumSize = new Size(760, 0);
            _labelIntro.ForeColor = SystemColors.GrayText;
            _labelIntro.Text = introduction;
            _labelCounter.AutoSize = true;
            _labelCounter.ForeColor = SystemColors.GrayText;
            _labelCounter.Anchor = AnchorStyles.Right;

            _labelHeading.AutoSize = true;
            _labelHeading.MaximumSize = new Size(860, 0);
            _labelHeading.Font = new Font(vernacularFont.FontFamily, 14f, FontStyle.Bold);
            _labelHeading.Margin = new Padding(3, 10, 3, 6);

            _labelNote.AutoSize = true;
            _labelNote.MaximumSize = new Size(860, 0);
            _labelNote.Margin = new Padding(3, 0, 3, 4);

            foreach (var label in new[] { _labelExamples, _labelGroups })
            {
                label.AutoSize = true;
                label.MaximumSize = new Size(860, 0);
                label.Margin = new Padding(3, 8, 3, 3);
            }
            foreach (var text in new[] { _textExamples, _textGroups })
            {
                text.Dock = DockStyle.Fill;
                text.ReadOnly = true;
                text.BackColor = SystemColors.Window;
                text.Font = vernacularFont;
                text.DetectUrls = false;
                text.BorderStyle = BorderStyle.FixedSingle;
            }

            _labelQuestion.AutoSize = true;
            _labelQuestion.MaximumSize = new Size(860, 0);
            _labelQuestion.Font = new Font(Font, FontStyle.Bold);
            _labelQuestion.Margin = new Padding(3, 12, 3, 3);

            var radios = new FlowLayoutPanel { AutoSize = true, Dock = DockStyle.Fill, WrapContents = false };
            _radioYes.Text = "&Yes";
            _radioNo.Text = "N&o";
            _radioNever.Text = "No, and &don't suggest this again";
            foreach (var radio in new[] { _radioYes, _radioNo, _radioNever })
            {
                radio.AutoSize = true;
                radio.Margin = new Padding(3, 3, 24, 3);
                radio.CheckedChanged += (s, e) =>
                {
                    if (!_settingChoice && ((RadioButton)s).Checked)
                        ChoiceChanged?.Invoke(this, CurrentChoice);
                };
                radios.Controls.Add(radio);
            }

            _linkDetails.AutoSize = true;
            _linkDetails.Text = "Show the details";
            _linkDetails.Margin = new Padding(3, 8, 3, 3);
            _linkDetails.LinkClicked += (s, e) => ShowDetails(!_textDetails.Visible);

            _textDetails.Dock = DockStyle.Fill;
            _textDetails.ReadOnly = true;
            _textDetails.BackColor = SystemColors.Control;
            _textDetails.BorderStyle = BorderStyle.None;
            _textDetails.Visible = false;

            // rows: intro+counter, heading, note, examples caption, examples, groups caption, groups, question, radios, details link, details
            _layout.RowCount = 11;
            for (int i = 0; i < 11; i++)
                _layout.RowStyles.Add(new RowStyle(SizeType.AutoSize));
            _layout.RowStyles[RowExamples] = new RowStyle(SizeType.Percent, 55);
            _layout.RowStyles[RowGroups] = new RowStyle(SizeType.Percent, 45);
            _layout.RowStyles[RowDetails] = new RowStyle(SizeType.Absolute, 0);

            _layout.Controls.Add(_labelIntro, 0, 0);
            _layout.Controls.Add(_labelCounter, 1, 0);
            AddRow(_labelHeading, 1);
            AddRow(_labelNote, 2);
            AddRow(_labelExamples, 3);
            AddRow(_textExamples, RowExamples);
            AddRow(_labelGroups, 5);
            AddRow(_textGroups, RowGroups);
            AddRow(_labelQuestion, 7);
            AddRow(radios, 8);
            AddRow(_linkDetails, 9);
            AddRow(_textDetails, RowDetails);
        }

        private const int RowExamples = 4;
        private const int RowGroups = 6;
        private const int RowDetails = 10;

        private void AddRow(Control control, int row)
        {
            _layout.Controls.Add(control, 0, row);
            _layout.SetColumnSpan(control, 2);
        }

        private SuggestionChoice CurrentChoice => _radioYes.Checked ? SuggestionChoice.Yes
                                                : _radioNever.Checked ? SuggestionChoice.NoAndDontSuggestAgain
                                                : SuggestionChoice.No;

        public void Show(RuleSuggestion suggestion, SuggestionChoice choice, string counter)
        {
            Suggestion = suggestion;
            _labelCounter.Text = counter;
            _labelHeading.Text = suggestion.Heading ?? suggestion.Title;
            _labelNote.Text = suggestion.Note;
            _labelNote.Visible = !String.IsNullOrEmpty(suggestion.Note);

            _labelExamples.Text = suggestion.ExamplesCaption;
            _textExamples.Text = String.Join(Environment.NewLine, suggestion.Examples);
            var hasExamples = suggestion.Examples.Any();
            _labelExamples.Visible = _textExamples.Visible = hasExamples;

            var hasGroups = suggestion.PossibleInconsistencies.Any();
            _labelGroups.Text = suggestion.GroupsCaption;
            _textGroups.Text = String.Join(Environment.NewLine, suggestion.PossibleInconsistencies);
            _labelGroups.Visible = _textGroups.Visible = hasGroups;
            _layout.RowStyles[RowExamples] = hasExamples ? new RowStyle(SizeType.Percent, hasGroups ? 55 : 100) : new RowStyle(SizeType.Absolute, 0);
            _layout.RowStyles[RowGroups] = hasGroups ? new RowStyle(SizeType.Percent, hasExamples ? 45 : 100) : new RowStyle(SizeType.Absolute, 0);

            _labelQuestion.Text = suggestion.Question ?? "Make this change?";

            // (all three are unchecked while the user hasn't answered this one yet)
            _settingChoice = true;
            _radioYes.Checked = (choice == SuggestionChoice.Yes);
            _radioNo.Checked = (choice == SuggestionChoice.No);
            _radioNever.Checked = (choice == SuggestionChoice.NoAndDontSuggestAgain);
            _settingChoice = false;

            var details = new StringBuilder();
            details.AppendLine(suggestion.Title);
            if (suggestion.IsRuleChange)
                details.AppendLine($"Corrections found: +{suggestion.CorrectionsGained} / −{suggestion.CorrectionsLost};  words newly put into groups: {suggestion.WordsNewlyBundled};  " +
                                   $"of those, corrected: {RuleAnalysisResult.FormatFraction(suggestion.FractionCorrected)}" +
                                   (suggestion.Recommended ? "  (recommended)" : "  (not recommended)"));
            details.AppendLine(suggestion.Explanation);
            _textDetails.Text = details.ToString();
            ShowDetails(false);
        }

        private void ShowDetails(bool show)
        {
            _textDetails.Visible = show;
            _layout.RowStyles[RowDetails] = show ? new RowStyle(SizeType.Absolute, 110) : new RowStyle(SizeType.Absolute, 0);
            _linkDetails.Text = show ? "Hide the details" : "Show the details";
        }
    }
}
