using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;

namespace SpellingFixer30
{
    /// <summary>
    /// A side pane in the Bulk picker that shows the contexts (examples from the text) of the word under the mouse (or
    /// of the selected row), with the word highlighted. It can be pinned, so it keeps showing the same word's examples
    /// while the mouse moves on (e.g. to scroll thru them or copy one). This replaces the old tooltip-like popup.
    /// </summary>
    internal class ExamplesPane : UserControl
    {
        private readonly Label _labelTitle = new Label();
        private readonly CheckBox _checkPin = new CheckBox();
        private readonly Button _buttonClose = new Button();
        private readonly RichTextBox _textExamples = new RichTextBox();
        private readonly ToolTip _toolTip = new ToolTip();
        private readonly Font _vernacularFont;
        private Font _highlightFont;

        public event EventHandler CloseRequested;

        /// <summary>
        /// The word whose examples are being shown (null if none)
        /// </summary>
        public string Word { get; private set; }

        public bool IsPinned => _checkPin.Checked;

        public ExamplesPane(Font vernacularFont)
        {
            _vernacularFont = vernacularFont;
            _highlightFont = new Font(vernacularFont, FontStyle.Bold);

            var header = new TableLayoutPanel { Dock = DockStyle.Top, AutoSize = true, ColumnCount = 3, RowCount = 1, BackColor = SystemColors.ControlLight };
            header.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100));
            header.ColumnStyles.Add(new ColumnStyle(SizeType.AutoSize));
            header.ColumnStyles.Add(new ColumnStyle(SizeType.AutoSize));

            _labelTitle.AutoSize = true;
            _labelTitle.Anchor = AnchorStyles.Left;
            _labelTitle.Font = new Font(SystemFonts.MessageBoxFont, FontStyle.Bold);
            _labelTitle.Text = "Examples";

            _checkPin.Appearance = Appearance.Button;
            _checkPin.AutoSize = true;
            _checkPin.Text = "Pin";
            _checkPin.FlatStyle = FlatStyle.System;
            _checkPin.CheckedChanged += (s, e) => UpdateTitle();
            _toolTip.SetToolTip(_checkPin, "Keep showing these examples (instead of the examples of whatever word the mouse is over)");

            _buttonClose.Text = "×";
            _buttonClose.AutoSize = true;
            _buttonClose.AutoSizeMode = AutoSizeMode.GrowAndShrink;
            _buttonClose.FlatStyle = FlatStyle.Flat;
            _buttonClose.FlatAppearance.BorderSize = 0;
            _buttonClose.Click += (s, e) => CloseRequested?.Invoke(this, EventArgs.Empty);
            _toolTip.SetToolTip(_buttonClose, "Hide the examples (View menu, 'Show Examples Pane', shows them again)");

            header.Controls.Add(_labelTitle, 0, 0);
            header.Controls.Add(_checkPin, 1, 0);
            header.Controls.Add(_buttonClose, 2, 0);

            _textExamples.Dock = DockStyle.Fill;
            _textExamples.ReadOnly = true;
            _textExamples.BackColor = SystemColors.Window;
            _textExamples.BorderStyle = BorderStyle.None;
            _textExamples.DetectUrls = false;
            _textExamples.Font = vernacularFont;

            var border = new Panel { Dock = DockStyle.Fill, BorderStyle = BorderStyle.FixedSingle, Padding = new Padding(4) };
            border.Controls.Add(_textExamples);

            Controls.Add(border);
            Controls.Add(header);
            ShowExamples(null, null);
        }

        /// <summary>
        /// Shows the examples of the given word (unless the pane is pinned to another word)
        /// </summary>
        /// <param name="force">show it even if pinned (e.g. the user explicitly asked for this word)</param>
        public void ShowExamples(string word, IEnumerable<string> contexts, bool force = false)
        {
            if (IsPinned && !force)
                return;
            if (force)
                _checkPin.Checked = false;
            if ((word == Word) && (word != null))
                return;

            Word = word;
            UpdateTitle();

            _textExamples.SuspendLayout();
            _textExamples.Clear();
            var list = contexts?.Where(c => !String.IsNullOrWhiteSpace(c)).ToList() ?? new List<string>();
            if (word == null)
            {
                _textExamples.SelectionFont = SystemFonts.MessageBoxFont;
                _textExamples.SelectionColor = SystemColors.GrayText;
                _textExamples.AppendText("Move the mouse over a word to see where it occurs in the text.");
            }
            else if (list.Count == 0)
            {
                _textExamples.SelectionFont = SystemFonts.MessageBoxFont;
                _textExamples.SelectionColor = SystemColors.GrayText;
                _textExamples.AppendText("(No examples were saved for this word.)");
            }
            else
            {
                foreach (var context in list)
                {
                    AppendHighlighted(context.Trim(), word);
                    _textExamples.AppendText(Environment.NewLine + Environment.NewLine);
                }
            }
            _textExamples.Select(0, 0);
            _textExamples.ResumeLayout();
        }

        private void UpdateTitle()
        {
            _labelTitle.Text = (Word == null) ? "Examples" : $"Examples of “{Word}”{(IsPinned ? " (pinned)" : null)}";
        }

        private void AppendHighlighted(string context, string word)
        {
            int start = 0;
            while (start < context.Length)
            {
                var index = String.IsNullOrEmpty(word) ? -1 : context.IndexOf(word, start, StringComparison.Ordinal);
                var end = (index < 0) ? context.Length : index;
                _textExamples.SelectionFont = _vernacularFont;
                _textExamples.SelectionBackColor = SystemColors.Window;
                _textExamples.AppendText(context.Substring(start, end - start));
                if (index < 0)
                    break;
                _textExamples.SelectionFont = _highlightFont;
                _textExamples.SelectionBackColor = Color.LightGoldenrodYellow;
                _textExamples.AppendText(word);
                start = index + word.Length;
            }
            _textExamples.SelectionBackColor = SystemColors.Window;
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                _highlightFont?.Dispose();
                _highlightFont = null;
                _toolTip.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}
