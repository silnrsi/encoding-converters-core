using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace SpellingFixer30
{
    internal partial class QueryDistinctionName : Form
    {
        public QueryDistinctionName()
        {
            InitializeComponent();
        }

        public string DistinctionName
        {
            get { return textBoxName.Text; }
        }

        private void buttonOK_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.OK;
            if (String.IsNullOrEmpty(DistinctionName))
                MessageBox.Show("The distinction name cannot be empty", CscProject.ApplicationCaption);
            else
                this.Close();
        }

        private void buttonCancel_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.Cancel;
            this.Close();
        }
    }
}