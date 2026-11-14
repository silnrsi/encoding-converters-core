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

        // list box display name -> project that's in a Paratext project's 'shared' folder, but not yet on this computer
        Dictionary<string, SharedCscProject> m_mapSharedProjects = new Dictionary<string, SharedCscProject>();

        /// <summary>
        /// Connects this computer to a project that's being shared in a Paratext project (by creating the
        /// junction to it) and replaces its list box entry with the plain project name.
        /// </summary>
        /// <returns>the project name, or null if it wasn't connected</returns>
        private string ConnectToSharedProject(string displayName)
        {
            var sharedProject = m_mapSharedProjects[displayName];
            if (ParatextProjectLink.LinkToParatext(sharedProject.CscProjectName, sharedProject.ParatextProjectName) == ParatextLinkResult.NotLinked)
                return null;

            var projectName = sharedProject.CscProjectName;
            CscProject.AddProjectKey(ParatextProjectLink.GetLocalProjectFolder(projectName), projectName);
            CscProjectStore.UpdateParatextProjectName(Path.Combine(ParatextProjectLink.GetLocalProjectFolder(projectName),
                                                                   $"{projectName}.{CscProject.ProjectFileExt}"),
                                                      sharedProject.ParatextProjectName);

            m_mapSharedProjects.Remove(displayName);
            var nIndex = checkedListBoxProjectNames.Items.IndexOf(displayName);
            if (nIndex >= 0)
            {
                var wasChecked = checkedListBoxProjectNames.GetItemChecked(nIndex);
                checkedListBoxProjectNames.Items[nIndex] = projectName;
                checkedListBoxProjectNames.SetItemChecked(nIndex, wasChecked);
            }
            return projectName;
        }

        public SelectProject()
        {
            InitializeComponent();

            openFileDialog.InitialDirectory = CscProject.ProjectsDefaultDirectory;

            var projectNames = CscProject.GetProjectNames;
            foreach (var projectName in projectNames)
                checkedListBoxProjectNames.Items.Add(projectName);

            // also list any projects being shared in a Paratext project that this computer doesn't have yet
            //  (e.g. another user of the Paratext project set it up). Choosing one connects to it.
            try
            {
                foreach (var sharedProject in ParatextProjectLink.FindSharedCscProjects())
                {
                    if (projectNames.Contains(sharedProject.CscProjectName, StringComparer.OrdinalIgnoreCase))
                        continue;
                    m_mapSharedProjects[sharedProject.DisplayName] = sharedProject;
                    checkedListBoxProjectNames.Items.Add(sharedProject.DisplayName);
                }
            }
            catch { }

            if (checkedListBoxProjectNames.Items.Count == 0)
                buttonOK.Enabled = false;

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
                        if (m_mapSharedProjects.ContainsKey(strProjectName))
                        {
                            strProjectName = ConnectToSharedProject(strProjectName);
                            if (strProjectName == null)
                                return null;
                        }

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
                if (m_mapSharedProjects.ContainsKey(strProjectName))
                {
                    MessageBox.Show("That project is only in a Paratext project's 'shared' folder (it isn't on this computer yet), so there's nothing to delete here.", SpellingFixer.cstrCaption);
                    return;
                }

                if( MessageBox.Show(String.Format("Are you sure you want to delete the '{0}' project?", strProjectName), SpellingFixer.cstrCaption, MessageBoxButtons.YesNoCancel) == DialogResult.Yes)
                {
                    try
                    {
                        CscProject.DeleteProject(strProjectName);
                        checkedListBoxProjectNames.Items.Remove(strProjectName);

						// if there are still projects left, select the next one in the list (or the previous one if we just deleted the last one)
						//	but only if no other projects are already selected.
						if ((checkedListBoxProjectNames.Items.Count > 0) && (checkedListBoxProjectNames.CheckedItems.Count == 0))
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
                {
                    if (!m_mapSharedProjects.ContainsKey(strProjectName))   // those aren't on this computer
                        CscProject.DeleteProject(strProjectName);
                }

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
				if (SelectedProject == null)
					checkedListBoxProjectNames.SetItemCheckState(nIndex, CheckState.Checked);
				System.Diagnostics.Debug.Assert(SelectedProject != null);
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
				if (SelectedProject == null)
					checkedListBoxProjectNames.SetItemCheckState(nIndex, CheckState.Checked);
				System.Diagnostics.Debug.Assert(SelectedProject != null);
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
				if (SelectedProject == null)
					checkedListBoxProjectNames.SetItemCheckState(nIndex, CheckState.Checked);
				System.Diagnostics.Debug.Assert(SelectedProject != null);
				try
				{
					SelectedProject.EditSpellingFixes(false);
				}
				catch (Exception ex)
				{
					MessageBox.Show(ex.Message, CscProject.ApplicationCaption);
				}
			}
		}

		private void validateSpellingFixesToolStripMenuItem_Click(object sender, EventArgs e)
		{
			int nIndex = checkedListBoxProjectNames.IndexFromPoint(m_ptRightClicked);
            if (nIndex >= 0)
            {
				if (SelectedProject == null)
					checkedListBoxProjectNames.SetItemCheckState(nIndex, CheckState.Checked);
				System.Diagnostics.Debug.Assert(SelectedProject != null);
				try
				{
                    SelectedProject.EditSpellingFixes(true);
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message, CscProject.ApplicationCaption);
                }
            }
        }
    }
}