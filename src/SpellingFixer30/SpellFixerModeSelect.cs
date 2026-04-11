using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace SpellingFixer30
{
    public partial class SpellFixerModeSelect : Form
    {
        public SpellFixerModeSelect()
        {
            InitializeComponent();
        }

        public SpellFixerMode Mode
        {
            get
            {
                SpellFixerMode eMode = SpellFixerMode.eConsistentSpellingChecker;
                if (radioButtonSpellFixerLegacy.Checked)
                    eMode = SpellFixerMode.eSpellFixerLegacy;
                else
                    System.Diagnostics.Debug.Assert(radioButtonCsc30.Checked);
                return eMode;
            }
        }

        private void buttonOK_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.OK;
            Close();
        }

        private void buttonCancel_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.Cancel;
            Close();
        }
    }
}