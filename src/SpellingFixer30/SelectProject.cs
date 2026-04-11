using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.Collections;
using System.IO;
using Microsoft.Win32;
using System.Linq;                  // for RegistryKey

namespace SpellingFixer30
{
    internal partial class SelectProject : Form
    {
        Dictionary<string, CscProject> m_mapProjects = new Dictionary<string, CscProject>();

        public SelectProject()
        {
            InitializeComponent();

            openFileDialog.InitialDirectory = CscProject.ProjectsDefaultDirectory;

            var projectNames = CscProject.GetProjectNames;
            if (!projectNames.Any())
                buttonOK.Enabled = false;
            else
            {
                foreach (var projectName in projectNames)
                    checkedListBoxProjectNames.Items.Add(projectName);
            }

            Properties.Settings.Default.Reload();
            if (checkedListBoxProjectNames.Items.Contains(Properties.Settings.Default.LastProjectSelected))
            {
                int nIndex = checkedListBoxProjectNames.FindStringExact(Properties.Settings.Default.LastProjectSelected);
                if (nIndex != -1)
                {
                    checkedListBoxProjectNames.SetItemChecked(nIndex, true);
                    checkedListBoxProjectNames.SelectedIndex = nIndex;
                }
            }
        }

        public CscProject SelectedProject
        {
            get
            {
                CscProject proj = null;
                if (this.checkedListBoxProjectNames.CheckedItems.Count > 0)
                {
                    try
                    {
                        Cursor = Cursors.WaitCursor;
                        System.Diagnostics.Debug.Assert(checkedListBoxProjectNames.CheckedItems.Count == 1);
                        string strProjectName = (string)checkedListBoxProjectNames.CheckedItems[0];
                        Properties.Settings.Default.LastProjectSelected = strProjectName;
                        Properties.Settings.Default.Save();

                        if (m_mapProjects.ContainsKey(strProjectName))
                            proj = m_mapProjects[strProjectName];
                        else
                        {
                            proj = CscProject.LoadProject(strProjectName);
                            m_mapProjects.Add(strProjectName, proj);
                        }
                    }
                    catch
                    {
                        throw;
                    }
                    finally
                    {
                        Cursor = Cursors.Default;
                    }
                }
                return proj;
            }
        }

        private void buttonOK_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.OK;
            this.Close();
        }

        private void buttonAddNewProject_Click(object sender, EventArgs e)
        {
            SFProjectForm dlg = new SFProjectForm(null);
            if (dlg.ShowDialog() == DialogResult.OK)
            {
                CscProject proj = dlg.CscProject;
                System.Diagnostics.Debug.Assert(proj != null);
				int nIndex = -1;
                if( !this.checkedListBoxProjectNames.Items.Contains(proj.Name) )
                    nIndex = this.checkedListBoxProjectNames.Items.Add(proj.Name);
				else
					nIndex = this.checkedListBoxProjectNames.Items.IndexOf(proj.Name);
				checkedListBoxProjectNames.SetItemChecked(nIndex, true);
				checkedListBoxProjectNames.SelectedIndex = nIndex;
            }
        }

		private void buttonCancel_Click(object sender, EventArgs e)
		{
			DialogResult = DialogResult.Cancel;
			this.Close();
		}

		private void deleteToolStripMenuItem_Click(object sender, EventArgs e)
		{
            int nIndex = checkedListBoxProjectNames.IndexFromPoint(m_ptRightClicked);
            if( nIndex >= 0 )
            {
                string strProjectName = checkedListBoxProjectNames.Items[nIndex].ToString();
                if( MessageBox.Show(String.Format("Are you sure you want to delete the '{0}' project?", strProjectName), SpellingFixer.cstrCaption, MessageBoxButtons.YesNoCancel) == DialogResult.Yes)
                {
                    try
                    {
                        CscProject.DeleteProject(strProjectName);
                        checkedListBoxProjectNames.Items.Remove(strProjectName);
                        if (checkedListBoxProjectNames.Items.Count > 0)
                            checkedListBoxProjectNames.SetItemCheckState(Math.Max(0, nIndex - 1), CheckState.Checked);
                    }
                    catch { }
                }
            }
		}

        private void deleteAllToolStripMenuItem_Click(object sender, EventArgs e)
        {
            // verify first!
            if (MessageBox.Show("Are you sure you want to delete all the existing projects?", SpellingFixer.cstrCaption, MessageBoxButtons.YesNoCancel) == DialogResult.Yes)
            {
                foreach (string strProjectName in checkedListBoxProjectNames.Items)
                    CscProject.DeleteProject(strProjectName);

                // now remove them from the checkbox list
                while (checkedListBoxProjectNames.Items.Count > 0)
                    checkedListBoxProjectNames.Items.RemoveAt(0);

                this.buttonOK.Enabled = false;
            }
        }

		private void ClearClickedItems()
		{
			foreach (int indexChecked in checkedListBoxProjectNames.CheckedIndices)
				checkedListBoxProjectNames.SetItemCheckState(indexChecked, CheckState.Unchecked);
		}

		private void checkedListBoxProjectNames_SelectedIndexChanged(object sender, EventArgs e)
		{
			int nIndex = checkedListBoxProjectNames.SelectedIndex;
			ClearClickedItems();
			if (nIndex != -1)
				checkedListBoxProjectNames.SetItemCheckState(nIndex, CheckState.Checked);

			buttonOK.Enabled = (nIndex != -1);
		}

		private Point m_ptRightClicked;
		private void checkedListBoxProjectNames_MouseUp(object sender, MouseEventArgs e)
		{
			m_ptRightClicked = new Point(e.X, e.Y);
		}

		private void checkedListBoxProjectNames_KeyPress(object sender, KeyPressEventArgs e)
		{
			buttonOK.Enabled = (checkedListBoxProjectNames.CheckedIndices.Count > 0);
		}

		private void editToolStripMenuItem_Click(object sender, EventArgs e)
		{
            int nIndex = checkedListBoxProjectNames.IndexFromPoint(m_ptRightClicked);
            if( nIndex >= 0 )
            {
				// select the project under the mouse and call SelectedProject to load the project
				ClearClickedItems();
				checkedListBoxProjectNames.SetItemCheckState(nIndex, CheckState.Checked); 
				
				SFProjectForm dlg = new SFProjectForm(SelectedProject);
				dlg.ShowDialog();
			}
		}

        private void buttonBrowseForProject_Click(object sender, EventArgs e)
        {
            if (openFileDialog.ShowDialog() == DialogResult.OK)
            {
                try
                {
                    string strFilename = openFileDialog.FileName;
                    string strProjectName = Path.GetFileNameWithoutExtension(strFilename);
                    CscProject.AddProjectKey(Path.GetDirectoryName(strFilename), strProjectName);

                    int nIndex = -1;
                    if (!checkedListBoxProjectNames.Items.Contains(strProjectName))
                        nIndex = checkedListBoxProjectNames.Items.Add(strProjectName);
                    else
                        nIndex = checkedListBoxProjectNames.Items.IndexOf(strProjectName);

                    checkedListBoxProjectNames.SetItemChecked(nIndex, true);
                    checkedListBoxProjectNames.SelectedIndex = nIndex;
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message, CscProject.ApplicationCaption);
                }
            }
        }

        private void resetWordsToCheckListToolStripMenuItem_Click(object sender, EventArgs e)
        {
            int nIndex = checkedListBoxProjectNames.IndexFromPoint(m_ptRightClicked);
            if (nIndex >= 0)
            {
                try
                {
                    SelectedProject.ResetCheckList();
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message, CscProject.ApplicationCaption);
                }
            }
        }

        private void editDictionaryToolStripMenuItem_Click(object sender, EventArgs e)
        {
            int nIndex = checkedListBoxProjectNames.IndexFromPoint(m_ptRightClicked);
            if (nIndex >= 0)
            {
                try
                {
                    SelectedProject.EditDictionary();
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message, CscProject.ApplicationCaption);
                }
            }
        }

        private void editListOfSpellingFixToolStripMenuItem_Click(object sender, EventArgs e)
        {
            int nIndex = checkedListBoxProjectNames.IndexFromPoint(m_ptRightClicked);
            if (nIndex >= 0)
            {
                try
                {
                    SelectedProject.EditSpellingFixes();
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message, CscProject.ApplicationCaption);
                }
            }
        }
    }
}