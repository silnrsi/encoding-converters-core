using System;
using System.ComponentModel;
using System.Windows.Forms;
using System.Diagnostics;               // for Debug.Assert
using System.Collections.Generic;
using Paratext.PluginInterfaces;

namespace SilEncConverters40.PtxConverters
{
	/// <summary>
	/// Summary description for ProjectListForm.
	/// </summary>
	public class ProjectListForm : Form
	{
		private ListBox listBoxProjectNames;
		private Button buttonCancel;
		private Button buttonAdd;
        private HelpProvider helpProvider;
        private TableLayoutPanel tableLayoutPanel;
		private ToolTip toolTip;
		private IContainer components;
        private Timer timerTooltip;

        public ProjectListForm(List<string> projectNames)
        {
            InitializeComponent();

            // put the display names in a list box for the user to choose.
			foreach(var projectName in projectNames)
			{
                this.listBoxProjectNames.Items.Add(projectName);
            }

            // disable the add button (until an implementation type is selected)
            this.buttonAdd.Enabled = false;
        }

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        protected override void Dispose( bool disposing )
		{
			if( disposing )
			{
				if(components != null)
				{
					components.Dispose();
				}
			}
			base.Dispose( disposing );
		}

		#region Windows Form Designer generated code
		/// <summary>
		/// Required method for Designer support - do not modify
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(ProjectListForm));
            this.listBoxProjectNames = new System.Windows.Forms.ListBox();
            this.buttonAdd = new System.Windows.Forms.Button();
            this.buttonCancel = new System.Windows.Forms.Button();
            this.helpProvider = new System.Windows.Forms.HelpProvider();
            this.tableLayoutPanel = new System.Windows.Forms.TableLayoutPanel();
            this.toolTip = new System.Windows.Forms.ToolTip(this.components);
            this.timerTooltip = new System.Windows.Forms.Timer(this.components);
            this.tableLayoutPanel.SuspendLayout();
            this.SuspendLayout();
            // 
            // listBoxProjectNames
            // 
            this.listBoxProjectNames.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.tableLayoutPanel.SetColumnSpan(this.listBoxProjectNames, 2);
            this.helpProvider.SetHelpString(this.listBoxProjectNames, "This list displays all of the available transduction engine types currently insta" +
        "lled");
            this.listBoxProjectNames.Location = new System.Drawing.Point(3, 3);
            this.listBoxProjectNames.Name = "listBoxProjectNames";
            this.helpProvider.SetShowHelp(this.listBoxProjectNames, true);
            this.listBoxProjectNames.Size = new System.Drawing.Size(408, 303);
            this.listBoxProjectNames.Sorted = true;
            this.listBoxProjectNames.TabIndex = 0;
            this.listBoxProjectNames.SelectedIndexChanged += new System.EventHandler(this.listBoxProjectNames_SelectedIndexChanged);
            this.listBoxProjectNames.DoubleClick += new System.EventHandler(this.listBoxProjectNames_DoubleClick);
            // 
            // buttonAdd
            // 
            this.buttonAdd.Anchor = System.Windows.Forms.AnchorStyles.Right;
            this.helpProvider.SetHelpString(this.buttonAdd, "Click this button to add an existing map or create a new converter based on the s" +
        "elected transduction type");
            this.buttonAdd.Location = new System.Drawing.Point(129, 321);
            this.buttonAdd.Name = "buttonAdd";
            this.helpProvider.SetShowHelp(this.buttonAdd, true);
            this.buttonAdd.Size = new System.Drawing.Size(75, 23);
            this.buttonAdd.TabIndex = 2;
            this.buttonAdd.Text = "&Add";
            this.buttonAdd.Click += new System.EventHandler(this.buttonAdd_Click);
            // 
            // buttonCancel
            // 
            this.buttonCancel.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.buttonCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.helpProvider.SetHelpString(this.buttonCancel, "Click this button to cancel this dialog");
            this.buttonCancel.Location = new System.Drawing.Point(210, 321);
            this.buttonCancel.Name = "buttonCancel";
            this.helpProvider.SetShowHelp(this.buttonCancel, true);
            this.buttonCancel.Size = new System.Drawing.Size(75, 23);
            this.buttonCancel.TabIndex = 3;
            this.buttonCancel.Text = "&Cancel";
            this.buttonCancel.Click += new System.EventHandler(this.buttonCancel_Click);
            // 
            // tableLayoutPanel
            // 
            this.tableLayoutPanel.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.tableLayoutPanel.ColumnCount = 2;
            this.tableLayoutPanel.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tableLayoutPanel.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tableLayoutPanel.Controls.Add(this.buttonCancel, 1, 1);
            this.tableLayoutPanel.Controls.Add(this.listBoxProjectNames, 0, 0);
            this.tableLayoutPanel.Controls.Add(this.buttonAdd, 0, 1);
            this.tableLayoutPanel.Location = new System.Drawing.Point(12, 12);
            this.tableLayoutPanel.Name = "tableLayoutPanel";
            this.tableLayoutPanel.RowCount = 2;
            this.tableLayoutPanel.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 20F));
            this.tableLayoutPanel.Size = new System.Drawing.Size(414, 347);
            this.tableLayoutPanel.TabIndex = 4;
            // 
            // ProjectListForm
            // 
            this.AcceptButton = this.buttonAdd;
            this.AutoScaleBaseSize = new System.Drawing.Size(5, 13);
            this.CancelButton = this.buttonCancel;
            this.ClientSize = new System.Drawing.Size(438, 371);
            this.Controls.Add(this.tableLayoutPanel);
            this.HelpButton = true;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.MinimumSize = new System.Drawing.Size(270, 133);
            this.Name = "ProjectListForm";
            this.Text = "Paratext Project Picker";
            this.tableLayoutPanel.ResumeLayout(false);
            this.ResumeLayout(false);

        }
		#endregion

		private string	m_strDisplayNameChosen;

		public string	SelectedDisplayName
		{
			get	{ return m_strDisplayNameChosen; }
		}

		private void buttonAdd_Click(object sender, System.EventArgs e)
		{
			Debug.Assert(this.listBoxProjectNames.SelectedItem != null);
			m_strDisplayNameChosen = (string)this.listBoxProjectNames.SelectedItem;
			this.DialogResult = DialogResult.OK;
			this.Close();
		}

		private void listBoxProjectNames_SelectedIndexChanged(object sender, EventArgs e)
		{
			this.buttonAdd.Enabled = (this.listBoxProjectNames.SelectedItem != null);
		}

		private void buttonCancel_Click(object sender, System.EventArgs e)
		{
            m_strDisplayNameChosen = null;
			this.DialogResult = DialogResult.Cancel;
			this.Close();
        }

        private void listBoxProjectNames_DoubleClick(object sender, EventArgs e)
        {
            if (this.listBoxProjectNames.SelectedIndex >= 0)
            {
                m_strDisplayNameChosen = (string)this.listBoxProjectNames.SelectedItem;
                this.DialogResult = DialogResult.OK;
                this.Close();
            }
        }
    }
}
