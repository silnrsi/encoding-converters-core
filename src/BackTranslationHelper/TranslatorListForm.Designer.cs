
using System;
using System.Windows.Forms;

namespace BackTranslationHelper
{
	partial class TranslatorListForm
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(TranslatorListForm));
            this.listBoxTranslatorNames = new System.Windows.Forms.ListBox();
            this.labelStatic = new System.Windows.Forms.Label();
            this.buttonRemove = new System.Windows.Forms.Button();
            this.buttonCancel = new System.Windows.Forms.Button();
            this.helpProvider = new System.Windows.Forms.HelpProvider();
            this.tableLayoutPanel = new System.Windows.Forms.TableLayoutPanel();
            this.buttonMoveTranslatorUp = new System.Windows.Forms.Button();
            this.buttonMoveTranslatorDown = new System.Windows.Forms.Button();
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
            this.helpProvider.SetHelpString(this.listBoxTranslatorNames, "This list displays all of the currently added Translator/EncConverters");
            this.listBoxTranslatorNames.Location = new System.Drawing.Point(3, 26);
            this.listBoxTranslatorNames.Name = "listBoxTranslatorNames";
            this.tableLayoutPanel.SetRowSpan(this.listBoxTranslatorNames, 2);
            this.helpProvider.SetShowHelp(this.listBoxTranslatorNames, true);
            this.listBoxTranslatorNames.Size = new System.Drawing.Size(316, 212);
            this.listBoxTranslatorNames.TabIndex = 0;
            this.listBoxTranslatorNames.SelectedIndexChanged += new System.EventHandler(this.listBoxTranslatorNames_SelectedIndexChanged);
            this.listBoxTranslatorNames.MouseMove += new System.Windows.Forms.MouseEventHandler(this.listBoxTranslatorNames_MouseMove);
            // 
            // labelStatic
            // 
            this.labelStatic.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.tableLayoutPanel.SetColumnSpan(this.labelStatic, 2);
            this.labelStatic.Location = new System.Drawing.Point(3, 0);
            this.labelStatic.Name = "labelStatic";
            this.labelStatic.Size = new System.Drawing.Size(316, 23);
            this.labelStatic.TabIndex = 1;
            this.labelStatic.Text = "Choose the Translator/EncConverter to remove or reorder";
            // 
            // buttonRemove
            // 
            this.buttonRemove.Anchor = System.Windows.Forms.AnchorStyles.Right;
            this.helpProvider.SetHelpString(this.buttonRemove, "Click this button to remove the selected translator/EncConverter from this projec" +
        "t");
            this.buttonRemove.Location = new System.Drawing.Point(83, 256);
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
            this.buttonCancel.Location = new System.Drawing.Point(164, 256);
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
            this.tableLayoutPanel.ColumnCount = 3;
            this.tableLayoutPanel.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tableLayoutPanel.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tableLayoutPanel.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.tableLayoutPanel.Controls.Add(this.labelStatic, 0, 0);
            this.tableLayoutPanel.Controls.Add(this.buttonCancel, 1, 3);
            this.tableLayoutPanel.Controls.Add(this.listBoxTranslatorNames, 0, 1);
            this.tableLayoutPanel.Controls.Add(this.buttonRemove, 0, 3);
            this.tableLayoutPanel.Controls.Add(this.buttonMoveTranslatorUp, 2, 1);
            this.tableLayoutPanel.Controls.Add(this.buttonMoveTranslatorDown, 2, 2);
            this.tableLayoutPanel.Location = new System.Drawing.Point(12, 12);
            this.tableLayoutPanel.Name = "tableLayoutPanel";
            this.tableLayoutPanel.RowCount = 4;
            this.tableLayoutPanel.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tableLayoutPanel.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tableLayoutPanel.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel.Size = new System.Drawing.Size(351, 283);
            this.tableLayoutPanel.TabIndex = 4;
            // 
            // buttonMoveTranslatorUp
            // 
            this.buttonMoveTranslatorUp.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.buttonMoveTranslatorUp.Image = global::BackTranslationHelper.Properties.Resources.FillUpHS;
            this.buttonMoveTranslatorUp.Location = new System.Drawing.Point(325, 112);
            this.buttonMoveTranslatorUp.Name = "buttonMoveTranslatorUp";
            this.buttonMoveTranslatorUp.Size = new System.Drawing.Size(23, 23);
            this.buttonMoveTranslatorUp.TabIndex = 9;
            this.toolTip.SetToolTip(this.buttonMoveTranslatorUp, "Click on this button to move the selected Translator/EncConverter up");
            this.buttonMoveTranslatorUp.UseVisualStyleBackColor = true;
            this.buttonMoveTranslatorUp.Click += new System.EventHandler(this.buttonMoveTranslatorUp_Click);
            // 
            // buttonMoveTranslatorDown
            // 
            this.buttonMoveTranslatorDown.Image = global::BackTranslationHelper.Properties.Resources.FillDownHS;
            this.buttonMoveTranslatorDown.Location = new System.Drawing.Point(325, 141);
            this.buttonMoveTranslatorDown.Name = "buttonMoveTranslatorDown";
            this.buttonMoveTranslatorDown.Size = new System.Drawing.Size(23, 23);
            this.buttonMoveTranslatorDown.TabIndex = 9;
            this.toolTip.SetToolTip(this.buttonMoveTranslatorDown, "Click on this button to move the selected Translator/EncConverter down");
            this.buttonMoveTranslatorDown.UseVisualStyleBackColor = true;
            this.buttonMoveTranslatorDown.Click += new System.EventHandler(this.buttonMoveTranslatorDown_Click);
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
            this.ClientSize = new System.Drawing.Size(375, 307);
            this.Controls.Add(this.tableLayoutPanel);
            this.HelpButton = true;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.MinimumSize = new System.Drawing.Size(270, 133);
            this.Name = "TranslatorListForm";
            this.Text = "Reorder/Remove Translator";
            this.tableLayoutPanel.ResumeLayout(false);
            this.ResumeLayout(false);
        }

		private ListBox listBoxTranslatorNames;
		private Label labelStatic;
		private Button buttonCancel;
		private Button buttonRemove;
		private HelpProvider helpProvider;
		private TableLayoutPanel tableLayoutPanel;
		private ToolTip toolTip;
		private Timer timerTooltip;
		private Button buttonMoveTranslatorUp;
		private Button buttonMoveTranslatorDown;
		#endregion

	}
}
