using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;

namespace SpellingFixer30
{
    /// <summary>
    /// Lists the words whose spelling status in Paratext's SpellingStatus.xml disagrees with the SpellFixer project's, so
    /// the user can tick the ones where SpellFixer's should replace Paratext's (the others are left as Paratext has them).
    /// </summary>
    internal class ParatextConflictsForm : Form
    {
        private const string ColumnReplace = "Replace";

        private readonly List<ParatextSpellingConflict> _conflicts;
        private readonly DataGridView _grid = new DataGridView();
        private readonly Label _labelCount = new Label();

        public ParatextConflictsForm(List<ParatextSpellingConflict> conflicts, string paratextFilespec, Font vernacularFont)
        {
            _conflicts = conflicts;

            Text = "Spelling Differences between SpellFixer and Paratext";
            Size = new Size(860, 600);
            MinimumSize = new Size(600, 400);
            StartPosition = FormStartPosition.CenterParent;
            ShowInTaskbar = false;
            MinimizeBox = false;
            Font = new Font(SystemFonts.MessageBoxFont.FontFamily, 10f);

            var layout = new TableLayoutPanel { Dock = DockStyle.Fill, ColumnCount = 1, RowCount = 3, Padding = new Padding(8) };
            layout.RowStyles.Add(new RowStyle(SizeType.AutoSize));
            layout.RowStyles.Add(new RowStyle(SizeType.Percent, 100));
            layout.RowStyles.Add(new RowStyle(SizeType.AutoSize));
            Controls.Add(layout);

            var labelIntro = new Label
            {
                AutoSize = true,
                MaximumSize = new Size(820, 0),
                Margin = new Padding(3, 3, 3, 8),
                Text = $"These words already have a different spelling status in Paratext ('{paratextFilespec}') than they do in SpellFixer." +
                       $"{Environment.NewLine}{Environment.NewLine}Tick the ones where SpellFixer's spelling status should replace Paratext's. " +
                       "The ones left unticked will stay the way Paratext has them. (Clicking Cancel exports nothing at all.)"
            };
            layout.Controls.Add(labelIntro, 0, 0);

            _grid.Dock = DockStyle.Fill;
            _grid.AllowUserToAddRows = false;
            _grid.AllowUserToDeleteRows = false;
            _grid.AllowUserToResizeRows = false;
            _grid.RowHeadersVisible = false;
            _grid.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            _grid.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            _grid.AutoSizeRowsMode = DataGridViewAutoSizeRowsMode.AllCells;
            _grid.Columns.Add(new DataGridViewCheckBoxColumn { Name = ColumnReplace, HeaderText = "Replace", FillWeight = 12, SortMode = DataGridViewColumnSortMode.NotSortable });
            var vernacularStyle = new DataGridViewCellStyle { Font = vernacularFont };
            _grid.Columns.Add(new DataGridViewTextBoxColumn { HeaderText = "Word", FillWeight = 25, ReadOnly = true, DefaultCellStyle = vernacularStyle });
            _grid.Columns.Add(new DataGridViewTextBoxColumn { HeaderText = "Paratext has it as", FillWeight = 32, ReadOnly = true, DefaultCellStyle = vernacularStyle });
            _grid.Columns.Add(new DataGridViewTextBoxColumn { HeaderText = "SpellFixer has it as", FillWeight = 32, ReadOnly = true, DefaultCellStyle = vernacularStyle });
            foreach (var conflict in _conflicts)
                _grid.Rows[_grid.Rows.Add(false, conflict.Word, conflict.ParatextDescription, conflict.SpellFixerDescription)].Tag = conflict;

            // tick/untick right away (rather than when the cell loses focus), so the count stays right
            _grid.CurrentCellDirtyStateChanged += (s, e) =>
            {
                if (_grid.IsCurrentCellDirty)
                    _grid.CommitEdit(DataGridViewDataErrorContexts.Commit);
            };
            _grid.CellValueChanged += (s, e) => UpdateCount();
            layout.Controls.Add(_grid, 0, 1);

            var buttons = new TableLayoutPanel { Dock = DockStyle.Fill, AutoSize = true, ColumnCount = 6, RowCount = 1 };
            buttons.ColumnStyles.Add(new ColumnStyle(SizeType.AutoSize));
            buttons.ColumnStyles.Add(new ColumnStyle(SizeType.AutoSize));
            buttons.ColumnStyles.Add(new ColumnStyle(SizeType.AutoSize));
            buttons.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100));
            buttons.ColumnStyles.Add(new ColumnStyle(SizeType.AutoSize));
            buttons.ColumnStyles.Add(new ColumnStyle(SizeType.AutoSize));
            layout.Controls.Add(buttons, 0, 2);

            var buttonCheckAll = new Button { Text = "Tick &all", AutoSize = true };
            buttonCheckAll.Click += (s, e) => SetAll(true);
            var buttonUncheckAll = new Button { Text = "&Untick all", AutoSize = true };
            buttonUncheckAll.Click += (s, e) => SetAll(false);
            _labelCount.AutoSize = true;
            _labelCount.Anchor = AnchorStyles.Left;
            var buttonOk = new Button { Text = "&Export", AutoSize = true, DialogResult = DialogResult.OK };
            var buttonCancel = new Button { Text = "Cancel", AutoSize = true, DialogResult = DialogResult.Cancel };
            AcceptButton = buttonOk;
            CancelButton = buttonCancel;

            buttons.Controls.Add(buttonCheckAll, 0, 0);
            buttons.Controls.Add(buttonUncheckAll, 1, 0);
            buttons.Controls.Add(_labelCount, 2, 0);
            buttons.Controls.Add(buttonOk, 4, 0);
            buttons.Controls.Add(buttonCancel, 5, 0);

            UpdateCount();
        }

        /// <summary>
        /// The conflicts the user ticked (i.e. where SpellFixer's spelling status should replace Paratext's)
        /// </summary>
        public List<ParatextSpellingConflict> ConflictsToReplace =>
            _grid.Rows.Cast<DataGridViewRow>()
                 .Where(r => (r.Cells[ColumnReplace].Value as bool?) == true)
                 .Select(r => (ParatextSpellingConflict)r.Tag)
                 .ToList();

        private void SetAll(bool value)
        {
            _grid.EndEdit();
            foreach (DataGridViewRow row in _grid.Rows)
                row.Cells[ColumnReplace].Value = value;
            UpdateCount();
        }

        private void UpdateCount()
        {
            var numTicked = _grid.Rows.Cast<DataGridViewRow>().Count(r => (r.Cells[ColumnReplace].Value as bool?) == true);
            _labelCount.Text = $"   {numTicked} of {_conflicts.Count} will be replaced";
        }
    }
}
