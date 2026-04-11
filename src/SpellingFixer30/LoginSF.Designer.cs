using System.Windows.Forms;

namespace SpellingFixer30
{
    partial class LoginSF
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(LoginSF));
            this.checkedListBoxProjects = new System.Windows.Forms.CheckedListBox();
            this.buttonOK = new System.Windows.Forms.Button();
            this.buttonCancel = new System.Windows.Forms.Button();
            this.toolTips = new System.Windows.Forms.ToolTip(this.components);
            this.contextMenu = new System.Windows.Forms.ContextMenu();
            this.menuItemAddNewProject = new System.Windows.Forms.MenuItem();
            this.menuItemClick = new System.Windows.Forms.MenuItem();
            this.menuItemDelete = new System.Windows.Forms.MenuItem();
            this.menuItemDeleteAll = new System.Windows.Forms.MenuItem();
            this.menuItemEdit = new System.Windows.Forms.MenuItem();
            this.tableLayoutPanel = new System.Windows.Forms.TableLayoutPanel();
            this.buttonAddNewProject = new System.Windows.Forms.Button();
            this.tableLayoutPanel.SuspendLayout();
            this.SuspendLayout();
            // 
            // checkedListBoxProjects
            // 
            this.tableLayoutPanel.SetColumnSpan(this.checkedListBoxProjects, 3);
            this.checkedListBoxProjects.Cursor = System.Windows.Forms.Cursors.Hand;
            this.checkedListBoxProjects.Dock = System.Windows.Forms.DockStyle.Fill;
            this.checkedListBoxProjects.Location = new System.Drawing.Point(3, 3);
            this.checkedListBoxProjects.Name = "checkedListBoxProjects";
            this.checkedListBoxProjects.Size = new System.Drawing.Size(485, 252);
            this.checkedListBoxProjects.TabIndex = 0;
            this.checkedListBoxProjects.ThreeDCheckBoxes = true;
            this.toolTips.SetToolTip(this.checkedListBoxProjects, "List of existing projects");
            this.checkedListBoxProjects.SelectedIndexChanged += new System.EventHandler(this.CheckedListBoxProjects_SelectedIndexChanged);
            this.checkedListBoxProjects.DoubleClick += new System.EventHandler(this.CheckedListBoxProjects_DoubleClick);
            this.checkedListBoxProjects.MouseUp += new System.Windows.Forms.MouseEventHandler(this.CheckedListBoxProjects_MouseUp);
            // 
            // buttonOK
            // 
            this.buttonOK.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.buttonOK.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.buttonOK.Location = new System.Drawing.Point(167, 261);
            this.buttonOK.Name = "buttonOK";
            this.buttonOK.Size = new System.Drawing.Size(75, 23);
            this.buttonOK.TabIndex = 11;
            this.buttonOK.Text = "OK";
            this.toolTips.SetToolTip(this.buttonOK, "Click to use the checked project");
            this.buttonOK.Click += new System.EventHandler(this.ButtonOK_Click);
            // 
            // buttonCancel
            // 
            this.buttonCancel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.buttonCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.buttonCancel.Location = new System.Drawing.Point(248, 261);
            this.buttonCancel.Name = "buttonCancel";
            this.buttonCancel.Size = new System.Drawing.Size(75, 23);
            this.buttonCancel.TabIndex = 12;
            this.buttonCancel.Text = "Cancel";
            this.toolTips.SetToolTip(this.buttonCancel, "Click to cancel this operation");
            this.buttonCancel.Click += new System.EventHandler(this.ButtonCancel_Click);
            // 
            // toolTips
            // 
            this.toolTips.AutoPopDelay = 30000;
            this.toolTips.InitialDelay = 500;
            this.toolTips.ReshowDelay = 100;
            // 
            // contextMenu
            // 
            this.contextMenu.MenuItems.AddRange(new System.Windows.Forms.MenuItem[] {
            this.menuItemAddNewProject,
            this.menuItemClick,
            this.menuItemDelete,
            this.menuItemDeleteAll,
            this.menuItemEdit});
            // 
            // menuItemAddNewProject
            // 
            this.menuItemAddNewProject.Index = 0;
            this.menuItemAddNewProject.Text = "&Add New Project";
            this.menuItemAddNewProject.Click += new System.EventHandler(this.MenuItemAddNewProject_Click);
            // 
            // menuItemClick
            // 
            this.menuItemClick.DefaultItem = true;
            this.menuItemClick.Index = 1;
            this.menuItemClick.Text = "&Check Project";
            this.menuItemClick.Click += new System.EventHandler(this.MenuItemClick_Click);
            // 
            // menuItemDelete
            // 
            this.menuItemDelete.Index = 2;
            this.menuItemDelete.Text = "&Delete Project";
            this.menuItemDelete.Click += new System.EventHandler(this.MenuItemDelete_Click);
            // 
            // menuItemDeleteAll
            // 
            this.menuItemDeleteAll.Index = 3;
            this.menuItemDeleteAll.Text = "Delete &All Projects";
            this.menuItemDeleteAll.Click += new System.EventHandler(this.MenuItemDeleteAll_Click);
            // 
            // menuItemEdit
            // 
            this.menuItemEdit.Index = 4;
            this.menuItemEdit.Text = "&Edit Project";
            this.menuItemEdit.Click += new System.EventHandler(this.MenuItemEdit_Click);
            // 
            // tableLayoutPanel
            // 
            this.tableLayoutPanel.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.tableLayoutPanel.ColumnCount = 3;
            this.tableLayoutPanel.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tableLayoutPanel.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 20F));
            this.tableLayoutPanel.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 30F));
            this.tableLayoutPanel.Controls.Add(this.checkedListBoxProjects, 0, 0);
            this.tableLayoutPanel.Controls.Add(this.buttonCancel, 1, 1);
            this.tableLayoutPanel.Controls.Add(this.buttonOK, 0, 1);
            this.tableLayoutPanel.Controls.Add(this.buttonAddNewProject, 2, 1);
            this.tableLayoutPanel.Location = new System.Drawing.Point(12, 12);
            this.tableLayoutPanel.Name = "tableLayoutPanel";
            this.tableLayoutPanel.RowCount = 2;
            this.tableLayoutPanel.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel.Size = new System.Drawing.Size(491, 287);
            this.tableLayoutPanel.TabIndex = 14;
            // 
            // buttonAddNewProject
            // 
            this.buttonAddNewProject.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.buttonAddNewProject.Location = new System.Drawing.Point(378, 261);
            this.buttonAddNewProject.Name = "buttonAddNewProject";
            this.buttonAddNewProject.Size = new System.Drawing.Size(110, 23);
            this.buttonAddNewProject.TabIndex = 13;
            this.buttonAddNewProject.Text = "&Add New Project";
            this.buttonAddNewProject.UseVisualStyleBackColor = true;
            this.buttonAddNewProject.Click += new System.EventHandler(this.MenuItemAddNewProject_Click);
            // 
            // LoginSF
            // 
            this.AcceptButton = this.buttonOK;
            this.AutoScaleBaseSize = new System.Drawing.Size(5, 13);
            this.CancelButton = this.buttonCancel;
            this.ClientSize = new System.Drawing.Size(515, 311);
            this.Controls.Add(this.tableLayoutPanel);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "LoginSF";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Select Find-Replace Project";
            this.Shown += new System.EventHandler(this.LoginSF_Shown);
            this.tableLayoutPanel.ResumeLayout(false);
            this.ResumeLayout(false);

        }
		#endregion

        private CheckedListBox checkedListBoxProjects;
        private Button buttonOK;
        private Button buttonCancel;
        private ToolTip toolTips;
        private MenuItem menuItemDelete;
        private MenuItem menuItemClick;
        private ContextMenu contextMenu;
        private MenuItem menuItemEdit;
        private MenuItem menuItemDeleteAll;
        private TableLayoutPanel tableLayoutPanel;
        private MenuItem menuItemAddNewProject;
        private Button buttonAddNewProject;
    }
}