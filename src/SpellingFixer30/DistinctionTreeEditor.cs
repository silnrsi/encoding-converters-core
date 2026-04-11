using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using SilEncConverters40;

namespace SpellingFixer30
{
    internal partial class DistinctionTreeEditor : Form
    {
        internal VernScriptSystem m_sfssp = null;
        internal Distinction m_aDistinctionRow = null;
        internal DirectableEncConverter m_aECTransliterator = null;
        
        protected const string cstrResultFormPrefix = "Change into: ";
        protected const string cstrSourceFormPrefix = "From: ";
        protected const string cstrAddNewFromForm = "[Add new 'From' value]";
        protected const string cstrAddNewToForm = "[Add new 'Into' value]";

        public DistinctionTreeEditor(VernScriptSystem sfssp, ref Distinction aDistinctionRow,
            DirectableEncConverter aECTransliterator, Font fontTransliterator)
        {
            InitializeComponent();

            treeViewDistinctions.Font = fontTransliterator;
            ecTextBoxFrom.Font = fontTransliterator;
            ecTextBoxTo.Font = fontTransliterator;

            textBoxRuleName.Text = aDistinctionRow.Name;
            textBoxDescription.Text = aDistinctionRow.Description;

            m_aECTransliterator = aECTransliterator;

            m_aDistinctionRow = aDistinctionRow;
            m_sfssp = sfssp;

            helpProvider.SetHelpString(treeViewDistinctions, Properties.Resources.DistinctionTreeHelp);

            var aNRRows = aDistinctionRow.NormalizeRules;
            foreach (var aNRRow in aNRRows)
            {
                string strResultingForm = aNRRow.NormalizedForm;
                TreeNode nodeNF = treeViewDistinctions.Nodes.Add(cstrResultFormPrefix + strResultingForm);
                nodeNF.Checked = true;
                nodeNF.Expand();

                // now add the rest
                foreach (var aCTNRow in aNRRow.CharactersToNormalize.CharacterToNormalize)
                {
                    var strOriginalForm = aCTNRow.Value;
                    TreeNode nodeOrigForm = nodeNF.Nodes.Add(cstrSourceFormPrefix + strOriginalForm);
                    UpdateOrigExtras(nodeOrigForm, strOriginalForm, strResultingForm);
                    nodeOrigForm.Tag = strResultingForm;
                }
                /*
                // add a final one that can be clicked in order to add a new string.
                nodeNF.Nodes.Add(cstrAddNewFromForm).Tag = strResultingForm;
                */
            }

            treeViewDistinctions.Nodes.Add(cstrAddNewToForm).Tag = cstrAddNewToForm;
        }

        protected void UpdateOrigExtras(TreeNode nodeOrigForm, string strOriginalForm, string strResultingForm)
        {
            nodeOrigForm.Text = cstrSourceFormPrefix + strOriginalForm;
            nodeOrigForm.Checked = true;
            nodeOrigForm.ToolTipText = String.Format("In the process of determining potentially misspelled words, this string{0}(i.e. '{1}') will be changed to the string above{0}(i.e. '{2}') in order to find words that differ between these two strings.{0}{0}Click this node to edit the string or uncheck the box to exclude this simplification rule",
                Environment.NewLine, strOriginalForm, strResultingForm);
        }

        protected bool m_bAddingNewFrom = false;
        protected bool m_bEditingNewFrom = false;
        protected TreeNode m_lastSelected = null;
        protected bool IsEditingNewFrom
        {
            get { return m_bEditingNewFrom; }
            set { m_bEditingNewFrom = value; }
        }

        protected bool IsEditingNewTo
        {
            get { return (m_lastSelected.Text == cstrAddNewToForm); }
        }

        protected bool IsAddingNewFrom
        {
            get { return m_bAddingNewFrom; }
            set { m_bAddingNewFrom = value; }
        }

        protected void SaveEditedValue()
        {
            // if editing both (i.e. it's a new empty node)
            string strOriginalForm = ecTextBoxFrom.Text;
            if (!ecTextBoxTo.ReadOnly && !ecTextBoxFrom.ReadOnly)
            {
                string strResultingForm = ecTextBoxTo.Text;
                if (m_lastSelected == null)
                {
                    // add a final one that can be clicked in order to add a new string.
                    System.Diagnostics.Debug.Assert(treeViewDistinctions.Nodes.Count == 0);
                    m_lastSelected = treeViewDistinctions.Nodes.Add(cstrAddNewToForm);
                    m_lastSelected.Tag = cstrAddNewToForm;
                }

                // add a final one that can be clicked in order to add a new string.
                m_lastSelected = treeViewDistinctions.Nodes.Insert(m_lastSelected.Index, cstrResultFormPrefix + strResultingForm);
                m_lastSelected.Checked = true;
                IsAddingNewFrom = true; // so we pick up the addition below

                // add details to the vss file
                var aNRRow = new NormalizeRule 
                { 
                    NormalizedForm = strResultingForm, 
                    CharactersToNormalize = new CharactersToNormalize
                    {
                        CharacterToNormalize = new List<CharacterToNormalize> 
                        { 
                            new CharacterToNormalize { Value = strOriginalForm } 
                        }
                    }
                };
                m_aDistinctionRow.NormalizeRules.Add(aNRRow);
            }

            if (IsEditingNewFrom)
            {
                string strResultingForm = (string)m_lastSelected.Tag;
                TreeNode nodeNew = m_lastSelected.Parent.Nodes.Insert(m_lastSelected.Index, cstrSourceFormPrefix + strOriginalForm);
                UpdateOrigExtras(nodeNew, strOriginalForm, strResultingForm);
                nodeNew.Tag = strResultingForm;
                m_lastSelected.Parent.Expand();
                IsEditingNewFrom = false;
            }
            else if (IsAddingNewFrom)
            {
                string strResultingForm = m_lastSelected.Text.Substring(cstrResultFormPrefix.Length);
                TreeNode nodeNew = m_lastSelected.Nodes.Add(cstrSourceFormPrefix + strOriginalForm);
                UpdateOrigExtras(nodeNew, strOriginalForm, strResultingForm);
                nodeNew.Tag = strResultingForm;
                IsAddingNewFrom = false;
            }
            else
                m_lastSelected.Text = cstrSourceFormPrefix + ecTextBoxFrom.Text;
        }

        private void treeViewDistinctions_NodeMouseClick(object sender, TreeNodeMouseClickEventArgs e)
        {
            // don't work with the parent nodes
            if (e.Button == MouseButtons.Right)
            {
                if (e.Node.Tag == null)
                    m_lastSelected = e.Node;
                else if (e.Node.Parent != null)
                    m_lastSelected = e.Node.Parent;
                else
                    m_lastSelected = null;
            }
            else
            {
                m_lastSelected = e.Node;
                treeViewDistinctions.SelectedNode = m_lastSelected;
                string strTag = (string)m_lastSelected.Tag;
                if (cstrAddNewToForm == strTag)
                {
                    // except the 'add new' parent node
                    ecTextBoxFrom.ReadOnly = ecTextBoxTo.ReadOnly = false;
                    ecTextBoxFrom.Select();
                    return;
                }
                else if (strTag == null)
                    return;

                if (!ecTextBoxFrom.ReadOnly)
                    // this means we're already editing it, so save changes before moving on to the next one
                    SaveEditedValue();

                string strResultingForm = (string)m_lastSelected.Tag;
                ecTextBoxTo.Text = strResultingForm;

                if (IsEditingNewFrom)
                {
                    ecTextBoxFrom.Text = strResultingForm;
                }
                else
                {
                    string strOriginalForm = m_lastSelected.Text.Substring(cstrSourceFormPrefix.Length);
                    ecTextBoxFrom.Text = strOriginalForm;
                }

                // enabling it means we're editing
                ecTextBoxTo.ReadOnly = true;
                ecTextBoxFrom.ReadOnly = false;
                ecTextBoxFrom.Select();
            }
        }

        private void buttonTransliterate_Click(object sender, EventArgs e)
        {
            if (m_aECTransliterator != null)
            {
                if (!ecTextBoxFrom.ReadOnly && !String.IsNullOrEmpty(ecTextBoxFrom.Text))
                    ecTextBoxFrom.Text = m_aECTransliterator.Convert(ecTextBoxFrom.Text);
                if (!ecTextBoxTo.ReadOnly && !String.IsNullOrEmpty(ecTextBoxTo.Text))
                    ecTextBoxTo.Text = m_aECTransliterator.Convert(ecTextBoxTo.Text);
            }
        }

        private void buttonSave_Click(object sender, EventArgs e)
        {
            // if the editing box is not read-only, it means we're editing and then this means save the edit 
            //  (otherwise, it means save all changes and dismiss dialog box)
            if (!ecTextBoxFrom.ReadOnly || !ecTextBoxFrom.ReadOnly)
            {
                SaveEditedValue();
                ecTextBoxFrom.Text = ecTextBoxTo.Text = null;
                ecTextBoxFrom.ReadOnly = ecTextBoxTo.ReadOnly = true;
            }
            else
            {
                SaveVSSRows();
                DialogResult = DialogResult.OK;
                this.Close();
            }
        }

        protected void SaveVSSRows()
        {
            m_aDistinctionRow.Description = textBoxDescription.Text;
            var aNRRows = m_aDistinctionRow.NormalizeRules;
            int nNumDistinctions = treeViewDistinctions.Nodes.Count - 1;
            System.Diagnostics.Debug.Assert(aNRRows.Count == nNumDistinctions);
            for (int i = 0; i < nNumDistinctions; i++)
            {
                TreeNode node = treeViewDistinctions.Nodes[i];
                var aNRRow = aNRRows[i];
                System.Diagnostics.Debug.Assert(node.Text.Substring(cstrResultFormPrefix.Length) == aNRRow.NormalizedForm);
                if (!node.Checked)
                {
                    m_aDistinctionRow.NormalizeRules.Remove(aNRRow);
                }
                else
                {
                    aNRRow.CharactersToNormalize.CharacterToNormalize.Clear();
                    foreach (TreeNode nodeSource in node.Nodes)
                    {
                        string strText = nodeSource.Text;
                        if (strText != cstrAddNewFromForm)
                        {
                            var strToNormalize = strText.Substring(cstrSourceFormPrefix.Length);
                            aNRRow.CharactersToNormalize.CharacterToNormalize.Add(new CharacterToNormalize { Value = strToNormalize });
                        }
                    }
                }
            }
        }

        private void buttonCancel_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.Cancel;
            this.Close();
        }

        private void DistinctionTreeEditor_FormClosing(object sender, FormClosingEventArgs e)
        {
            // if the editing box is not read-only, it means we're editing and then this means cancel the edit 
            //  (otherwise, it means throw away all changes and dismiss dialog box)
            if (!ecTextBoxFrom.ReadOnly || !ecTextBoxTo.ReadOnly)
            {
                ecTextBoxFrom.Text = ecTextBoxTo.Text = null;
                ecTextBoxTo.ReadOnly = ecTextBoxFrom.ReadOnly = true;
                e.Cancel = true;
            }
        }

        private void addFromValueToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (m_lastSelected != null)
            {
                string strResultingForm = m_lastSelected.Text.Substring(cstrResultFormPrefix.Length);
                ecTextBoxTo.Text = strResultingForm;
                ecTextBoxTo.ReadOnly = true;
                ecTextBoxFrom.ReadOnly = false;
                ecTextBoxFrom.Focus();
                IsAddingNewFrom = true;
            }
        }
    }
}