using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;

namespace SpellingFixer30
{
    /// <summary>
    /// Shows why two words were put in the same bundle: the steps that turned each of them into the form that's compared
    /// (transliteration, ignored characters, and the Inconsistency Settings), with a checkbox for each Inconsistency Setting
    /// involved, so the user can turn off one that's causing false alarms (e.g. one they accepted in an analysis by mistake)
    /// </summary>
    internal class RuleTraceForm : Form
    {
        private readonly CheckedListBox _listSettings = new CheckedListBox();
        private readonly Button _buttonReanalyze = new Button();
        private readonly Button _buttonClose = new Button();
        private readonly List<string> _distinctionNames = new List<string>();

        /// <summary>
        /// The Inconsistency Settings the user unchecked (when the dialog result is OK)
        /// </summary>
        public List<string> UncheckedDistinctionNames =>
            _distinctionNames.Where((name, i) => !_listSettings.GetItemChecked(i)).ToList();

        public RuleTraceForm(CscProject project, string wordLhs, string wordRhs)
        {
            Text = "Why are these words shown together?";
            Size = new Size(760, 560);
            MinimumSize = new Size(520, 400);
            StartPosition = FormStartPosition.CenterParent;
            ShowInTaskbar = false;
            MinimizeBox = false;
            MaximizeBox = false;
            Font = new Font(SystemFonts.MessageBoxFont.FontFamily, 10f);

            var stepsLhs = project.TraceSimplification(wordLhs);
            var stepsRhs = String.IsNullOrEmpty(wordRhs) ? new List<CscProject.SimplificationStep>() : project.TraceSimplification(wordRhs);

            var layout = new TableLayoutPanel { Dock = DockStyle.Fill, ColumnCount = 1, RowCount = 5, Padding = new Padding(10) };
            layout.RowStyles.Add(new RowStyle(SizeType.AutoSize));
            layout.RowStyles.Add(new RowStyle(SizeType.Percent, 70));
            layout.RowStyles.Add(new RowStyle(SizeType.AutoSize));
            layout.RowStyles.Add(new RowStyle(SizeType.Percent, 30));
            layout.RowStyles.Add(new RowStyle(SizeType.AutoSize));
            Controls.Add(layout);

            var intro = new Label
            {
                AutoSize = true,
                MaximumSize = new Size(720, 0),
                Text = "Before comparing words, the Spelling Fixer changes them in these steps. Words that end up the same are shown " +
                       "together as possible inconsistencies:"
            };
            layout.Controls.Add(intro, 0, 0);

            var trace = new RichTextBox
            {
                Dock = DockStyle.Fill,
                ReadOnly = true,
                BackColor = SystemColors.Window,
                DetectUrls = false,
                WordWrap = false,
                Font = new Font(project.Font.FontFamily, Math.Max(11f, project.Font.Size * 0.8f))
            };
            AppendTrace(trace, wordLhs, stepsLhs);
            if (!String.IsNullOrEmpty(wordRhs))
            {
                trace.AppendText(Environment.NewLine);
                AppendTrace(trace, wordRhs, stepsRhs);
            }
            trace.Select(0, 0);
            layout.Controls.Add(trace, 0, 1);

            var settingsLabel = new Label
            {
                AutoSize = true,
                MaximumSize = new Size(720, 0),
                Margin = new Padding(3, 12, 3, 3),
                Text = "These Inconsistency Settings changed these words. If one of them is causing words to be shown together that " +
                       "aren't really inconsistent, uncheck it and the project will stop using it. (It stays in the Vernacular Script " +
                       "System, so you can turn it back on in the project settings, and the next analysis will check it again.)"
            };
            layout.Controls.Add(settingsLabel, 0, 2);

            // one item per setting involved (in the order applied), saying what it did to which word
            var involved = stepsLhs.Concat(stepsRhs).Where(s => s.DistinctionName != null).Select(s => s.DistinctionName).Distinct().ToList();
            foreach (var name in involved)
            {
                var what = new List<string>();
                foreach (var step in stepsLhs.Where(s => s.DistinctionName == name))
                    what.Add($"{step.Description} in {wordLhs}");
                foreach (var step in stepsRhs.Where(s => s.DistinctionName == name))
                    what.Add($"{step.Description} in {wordRhs}");
                _distinctionNames.Add(name);
                _listSettings.Items.Add($"{name}:   {String.Join(";  ", what)}", true);
            }
            _listSettings.Dock = DockStyle.Fill;
            _listSettings.CheckOnClick = true;
            _listSettings.HorizontalScrollbar = true;
            _listSettings.IntegralHeight = false;
            _listSettings.Font = trace.Font;
            _listSettings.ItemCheck += (s, e) => BeginInvoke((Action)UpdateButtons);
            if (involved.Count == 0)
            {
                settingsLabel.Text = "None of the Inconsistency Settings changed these words: they look alike just because of the steps above " +
                                     "(e.g. the transliteration, or characters that are ignored).";
                _listSettings.Visible = false;
            }
            layout.Controls.Add(_listSettings, 0, 3);

            var buttons = new FlowLayoutPanel { AutoSize = true, Dock = DockStyle.Fill, FlowDirection = FlowDirection.RightToLeft, WrapContents = false };
            _buttonClose.Text = "Close";
            _buttonClose.AutoSize = true;
            _buttonClose.DialogResult = DialogResult.Cancel;
            _buttonReanalyze.Text = "&Save changes and Re-analyze data from scratch";
            _buttonReanalyze.AutoSize = true;
            _buttonReanalyze.Enabled = false;
            _buttonReanalyze.DialogResult = DialogResult.OK;
            buttons.Controls.Add(_buttonClose);
            buttons.Controls.Add(_buttonReanalyze);
            layout.Controls.Add(buttons, 0, 4);
            CancelButton = _buttonClose;
        }

        private void UpdateButtons()
        {
            _buttonReanalyze.Enabled = UncheckedDistinctionNames.Count > 0;
        }

        private static void AppendTrace(RichTextBox box, string word, List<CscProject.SimplificationStep> steps)
        {
            var bold = new Font(box.Font, FontStyle.Bold);
            box.SelectionFont = bold;
            box.AppendText(word + Environment.NewLine);
            box.SelectionFont = box.Font;
            if (steps.Count == 0)
                box.AppendText("    (unchanged)" + Environment.NewLine);
            foreach (var step in steps)
            {
                box.SelectionFont = box.Font;
                box.SelectionColor = SystemColors.WindowText;
                box.AppendText($"    → {step.After}");
                box.SelectionColor = (step.DistinctionName != null) ? Color.DarkBlue : SystemColors.GrayText;
                box.AppendText((step.DistinctionName != null)
                                    ? $"      ({step.DistinctionName}: {step.Description})"
                                    : $"      ({step.Description})");
                box.AppendText(Environment.NewLine);
            }
            box.SelectionColor = SystemColors.WindowText;
        }
    }
}
