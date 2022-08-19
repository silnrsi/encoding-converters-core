using System;
using System.Collections;
using System.ComponentModel;
using System.Windows.Forms;
using System.Diagnostics;               // for Debug.Assert
using System.Collections.Generic;
using ECInterfaces;

namespace BackTranslationHelper
{
	/// <summary>
	/// Summary description for TranslatorListForm.
	/// </summary>
	public class TranslatorListForm : Form
	{
		private ListBox listBoxTranslatorNames;
		private Label labelStatic;
		private Button buttonCancel;
		private Button buttonRemove;
        private HelpProvider helpProvider;
        private TableLayoutPanel tableLayoutPanel;
		private ToolTip toolTip;
		private IContainer components;
        private Timer timerTooltip;
        protected Dictionary<string, string> m_mapLbItems2Tooltips = new Dictionary<string, string>();

        public TranslatorListForm(List<IEncConverter> translators)
        {
            InitializeComponent();

            // put the display names in a list box for the user to choose.
			foreach(var translator in translators)
			{
                listBoxTranslatorNames.Items.Add(translator.Name);
                m_mapLbItems2Tooltips[translator.Name] = translator.ToString();
            }

            // disable the add button (until an implementation type is selected)
            buttonRemove.Enabled = false;

            labelStatic.Text = $"Choose the Translator/EncConverter to remove";
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(TranslatorListForm));
            this.listBoxTranslatorNames = new System.Windows.Forms.ListBox();
            this.labelStatic = new System.Windows.Forms.Label();
            this.buttonRemove = new System.Windows.Forms.Button();
            this.buttonCancel = new System.Windows.Forms.Button();
            this.helpProvider = new System.Windows.Forms.HelpProvider();
            this.tableLayoutPanel = new System.Windows.Forms.TableLayoutPanel();
            this.toolTip = new System.Windows.Forms.ToolTip(this.components);
            this.timerTooltip = new System.Windows.Forms.Timer(this.components);
            this.tableLayoutPanel.SuspendLayout();
            this.SuspendLayout();
            // 
            // listBoxTranslatorNames
            // 
            this.listBoxTranslatorNames.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.tableLayoutPanel.SetColumnSpan(this.listBoxTranslatorNames, 2);
            this.helpProvider.SetHelpString(this.listBoxTranslatorNames, "This list displays all of the available transduction engine types currently insta" +
        "lled");
            this.listBoxTranslatorNames.Location = new System.Drawing.Point(3, 26);
            this.listBoxTranslatorNames.Name = "listBoxTranslatorNames";
            this.helpProvider.SetShowHelp(this.listBoxTranslatorNames, true);
            this.listBoxTranslatorNames.Size = new System.Drawing.Size(262, 225);
            this.listBoxTranslatorNames.Sorted = true;
            this.listBoxTranslatorNames.TabIndex = 0;
            this.listBoxTranslatorNames.SelectedIndexChanged += new System.EventHandler(this.listBoxTranslatorNames_SelectedIndexChanged);
            this.listBoxTranslatorNames.DoubleClick += new System.EventHandler(this.listBoxTranslatorNames_DoubleClick);
            this.listBoxTranslatorNames.MouseMove += new System.Windows.Forms.MouseEventHandler(this.listBoxTranslatorNames_MouseMove);
            // 
            // labelStatic
            // 
            this.tableLayoutPanel.SetColumnSpan(this.labelStatic, 2);
            this.labelStatic.Location = new System.Drawing.Point(3, 0);
            this.labelStatic.Name = "labelStatic";
            this.labelStatic.Size = new System.Drawing.Size(256, 23);
            this.labelStatic.TabIndex = 1;
            this.labelStatic.Text = "Choose the Translator/EncConverter to remove";
            // 
            // buttonRemove
            // 
            this.buttonRemove.Anchor = System.Windows.Forms.AnchorStyles.Right;
            this.helpProvider.SetHelpString(this.buttonRemove, "Click this button to remove the selected translator/EncConverter from this project");
            this.buttonRemove.Location = new System.Drawing.Point(56, 257);
            this.buttonRemove.Name = "buttonRemove";
            this.helpProvider.SetShowHelp(this.buttonRemove, true);
            this.buttonRemove.Size = new System.Drawing.Size(75, 23);
            this.buttonRemove.TabIndex = 2;
            this.buttonRemove.Text = "&Remove";
            this.buttonRemove.Click += new System.EventHandler(this.buttonRemove_Click);
            // 
            // buttonCancel
            // 
            this.buttonCancel.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.buttonCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.helpProvider.SetHelpString(this.buttonCancel, "Click this button to cancel this dialog");
            this.buttonCancel.Location = new System.Drawing.Point(137, 257);
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
            this.tableLayoutPanel.Controls.Add(this.labelStatic, 0, 0);
            this.tableLayoutPanel.Controls.Add(this.buttonCancel, 1, 2);
            this.tableLayoutPanel.Controls.Add(this.listBoxTranslatorNames, 0, 1);
            this.tableLayoutPanel.Controls.Add(this.buttonRemove, 0, 2);
            this.tableLayoutPanel.Location = new System.Drawing.Point(12, 12);
            this.tableLayoutPanel.Name = "tableLayoutPanel";
            this.tableLayoutPanel.RowCount = 3;
            this.tableLayoutPanel.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel.Size = new System.Drawing.Size(268, 283);
            this.tableLayoutPanel.TabIndex = 4;
            // 
            // timerTooltip
            // 
            this.timerTooltip.Interval = 500;
            this.timerTooltip.Tick += new System.EventHandler(this.timerTooltip_Tick);
            // 
            // TranslatorListForm
            // 
            this.AcceptButton = this.buttonRemove;
            this.AutoScaleBaseSize = new System.Drawing.Size(5, 13);
            this.CancelButton = this.buttonCancel;
            this.ClientSize = new System.Drawing.Size(292, 307);
            this.Controls.Add(this.tableLayoutPanel);
            this.HelpButton = true;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.MinimumSize = new System.Drawing.Size(270, 133);
            this.Name = "TranslatorListForm";
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

		private void buttonRemove_Click(object sender, System.EventArgs e)
		{
			Debug.Assert(this.listBoxTranslatorNames.SelectedItem != null);
			m_strDisplayNameChosen = (string)this.listBoxTranslatorNames.SelectedItem;
			this.DialogResult = DialogResult.OK;
			this.Close();
		}

		private void listBoxTranslatorNames_SelectedIndexChanged(object sender, EventArgs e)
		{
			this.buttonRemove.Enabled = (this.listBoxTranslatorNames.SelectedItem != null);
		}

		private void buttonCancel_Click(object sender, System.EventArgs e)
		{
            m_strDisplayNameChosen = null;
			this.DialogResult = DialogResult.Cancel;
			this.Close();
        }

        private void listBoxTranslatorNames_DoubleClick(object sender, EventArgs e)
        {
            if (this.listBoxTranslatorNames.SelectedIndex >= 0)
            {
                m_strDisplayNameChosen = (string)this.listBoxTranslatorNames.SelectedItem;
                this.DialogResult = DialogResult.OK;
                this.Close();
            }
        }

        protected int m_nLastTooltipDisplayedIndex = ListBox.NoMatches;
        private void listBoxTranslatorNames_MouseMove(object sender, MouseEventArgs e)
        {
            int nIndex = this.listBoxTranslatorNames.IndexFromPoint(e.X, e.Y);
            if (nIndex != m_nLastTooltipDisplayedIndex)
            {
                m_nLastTooltipDisplayedIndex = nIndex;
                toolTip.Hide(listBoxTranslatorNames);
                if (nIndex != ListBox.NoMatches)
                {
                    timerTooltip.Stop();
                    timerTooltip.Start();
                }
            }
        }

        private void timerTooltip_Tick(object sender, EventArgs e)
        {
            try
            {
                if (m_nLastTooltipDisplayedIndex != ListBox.NoMatches)
                {
                    string strKey = (string)this.listBoxTranslatorNames.Items[m_nLastTooltipDisplayedIndex];

                    string strDescription;
                    if (m_mapLbItems2Tooltips.TryGetValue(strKey, out strDescription))
                        toolTip.SetToolTip(listBoxTranslatorNames, strDescription);
                }
            }
            finally
            {
                timerTooltip.Stop();
            }
        }
    }
}
