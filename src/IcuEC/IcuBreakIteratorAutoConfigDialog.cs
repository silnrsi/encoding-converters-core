using System;
using System.Runtime.InteropServices;
using System.Windows.Forms;
using ECInterfaces;                     // for IEncConverter

namespace SilEncConverters40
{
	public class IcuBreakIteratorAutoConfigDialog : SilEncConverters40.AutoConfigDialog
    {
        public IcuBreakIteratorAutoConfigDialog(
            IEncConverters aECs,
            string strDisplayName,
            string strFriendlyName,
            string strConverterIdentifier,
            ConvType eConversionType,
            string strLhsEncodingId,
            string strRhsEncodingId,
            int lProcessTypeFlags,
            bool bIsInRepository,
			string strTestData)
        {
            Util.DebugWriteLine(this, "BEGIN");
            InitializeComponent();
            Util.DebugWriteLine(this, "Initialized component.");
            base.Initialize(
                aECs,
                IcuBreakIteratorEncConverter.CstrHtmlFilename,
                strDisplayName,
                strFriendlyName,
                strConverterIdentifier,
                eConversionType,
                strLhsEncodingId,
                strRhsEncodingId,
                lProcessTypeFlags,
                bIsInRepository);
            Util.DebugWriteLine(this, "Initialized base.");

			IsModified = false;	// there really is no editing

            m_bInitialized = true;
			InitialTestData = strTestData;
			Util.DebugWriteLine(this, "END");
        }

        public IcuBreakIteratorAutoConfigDialog(
            IEncConverters aECs,
            string strFriendlyName,
            string strConverterIdentifier,
            ConvType eConversionType,
            string strTestData)
        {
            InitializeComponent();
            base.Initialize(
                aECs,
                strFriendlyName,
                strConverterIdentifier,
                eConversionType,
                strTestData);
            m_bInitialized = true;
        }

        private System.Windows.Forms.Label labelInstructions2;

        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel1;

        // This code was NOT generated!
        // So feel free to modify it as needed.
        private void InitializeComponent()
        {
            this.tableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
            this.labelInstructions2 = new System.Windows.Forms.Label();
            this.tabControl.SuspendLayout();
            this.tabPageSetup.SuspendLayout();
            this.tableLayoutPanel1.SuspendLayout();
            this.SuspendLayout();
            //
            // tabPageSetup
            //
            this.tabPageSetup.Controls.Add(this.tableLayoutPanel1);
            //
            // buttonApply
            //
            this.helpProvider.SetHelpString(this.buttonApply, "Click this button to apply the configured values for this converter");
            this.helpProvider.SetShowHelp(this.buttonApply, true);
            //
            // buttonCancel
            //
            this.helpProvider.SetHelpString(this.buttonCancel, "Click this button to cancel this dialog");
            this.helpProvider.SetShowHelp(this.buttonCancel, true);
            //
            // buttonOK
            //
            this.helpProvider.SetHelpString(this.buttonOK, "Click this button to accept the configured values for this converter");
            this.helpProvider.SetShowHelp(this.buttonOK, true);
            //
            // buttonSaveInRepository
            //
            this.helpProvider.SetHelpString(this.buttonSaveInRepository, "\r\nClick to add this converter to the system repository permanently.\r\n    ");
            this.helpProvider.SetShowHelp(this.buttonSaveInRepository, true);
            //
            // tableLayoutPanel1
            //
            this.tableLayoutPanel1.ColumnCount = 1;
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.tableLayoutPanel1.Controls.Add(this.labelInstructions2, 0, 0);
            this.tableLayoutPanel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanel1.Location = new System.Drawing.Point(3, 3);
            this.tableLayoutPanel1.Name = "tableLayoutPanel1";
            this.tableLayoutPanel1.RowCount = 1;
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel1.Size = new System.Drawing.Size(596, 394);
            this.tableLayoutPanel1.TabIndex = 1;
			//
			// labelInstructions
			//
			this.labelInstructions2.Anchor = System.Windows.Forms.AnchorStyles.Right;
            this.labelInstructions2.AutoSize = true;
            this.labelInstructions2.Location = new System.Drawing.Point(3, 190);
            this.labelInstructions2.Name = "labelConvName";
            this.labelInstructions2.Size = new System.Drawing.Size(56, 13);
            this.labelInstructions2.TabIndex = 0;
            this.labelInstructions2.Text = "Click the 'Save in Repository' button to make this converter permanent, or 'OK' to use it as a temporary converter";
            //
            // IcuBreakIteratorAutoConfigDialog
            //
            this.ClientSize = new System.Drawing.Size(634, 479);
            this.Name = "IcuBreakIteratorAutoConfigDialog";
            this.Controls.SetChildIndex(this.tabControl, 0);
            this.Controls.SetChildIndex(this.buttonApply, 0);
            this.Controls.SetChildIndex(this.buttonCancel, 0);
            this.Controls.SetChildIndex(this.buttonOK, 0);
            this.Controls.SetChildIndex(this.buttonSaveInRepository, 0);
            this.tabControl.ResumeLayout(false);
            this.tabPageSetup.ResumeLayout(false);
            this.tableLayoutPanel1.ResumeLayout(false);
            this.tableLayoutPanel1.PerformLayout();
            this.ResumeLayout(false);

        }

        protected override string ProgID
        {
            get { return typeof(IcuBreakIteratorEncConverter).FullName; }
        }

        protected override string ImplType
        {
            get { return IcuBreakIteratorEncConverter.CstrImplementationType; }
        }

        protected override string DefaultFriendlyName
        {
            get { return IcuBreakIteratorEncConverter.CstrDisplayName; }
        }
    }
}
