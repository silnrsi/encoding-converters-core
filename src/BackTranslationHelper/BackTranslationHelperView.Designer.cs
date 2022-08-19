
namespace BackTranslationHelper
{
    partial class BackTranslationHelperView
    {
        /// <summary> 
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary> 
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Component Designer generated code

        /// <summary> 
        /// Required method for Designer support - do not modify 
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            this.menuStrip = new System.Windows.Forms.MenuStrip();
            this.settingsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.changeEncConverterToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.addEncConverterToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.fontsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.sourceTextToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.targetTextToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.autoSaveToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolTip = new System.Windows.Forms.ToolTip(this.components);
            this.xWebBrowser = new SIL.Windows.Forms.HtmlBrowser.XWebBrowser();
            this.menuStrip.SuspendLayout();
            this.SuspendLayout();
            // 
            // menuStrip
            // 
            this.menuStrip.Dock = System.Windows.Forms.DockStyle.None;
            this.menuStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.settingsToolStripMenuItem});
            this.menuStrip.Location = new System.Drawing.Point(0, 0);
            this.menuStrip.Name = "menuStrip";
            this.menuStrip.Size = new System.Drawing.Size(69, 24);
            this.menuStrip.TabIndex = 1;
            this.menuStrip.Text = "menuStrip1";
            // 
            // settingsToolStripMenuItem
            // 
            this.settingsToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.changeEncConverterToolStripMenuItem,
            this.addEncConverterToolStripMenuItem,
            this.fontsToolStripMenuItem,
            this.autoSaveToolStripMenuItem});
            this.settingsToolStripMenuItem.Name = "settingsToolStripMenuItem";
            this.settingsToolStripMenuItem.Size = new System.Drawing.Size(61, 20);
            this.settingsToolStripMenuItem.Text = "&Settings";
            // 
            // changeEncConverterToolStripMenuItem
            // 
            this.changeEncConverterToolStripMenuItem.Name = "changeEncConverterToolStripMenuItem";
            this.changeEncConverterToolStripMenuItem.Size = new System.Drawing.Size(245, 22);
            this.changeEncConverterToolStripMenuItem.Text = "Change &Translator/EncConverter";
            this.changeEncConverterToolStripMenuItem.ToolTipText = "Click to switch to a different Translator/EncConverter used to generate the trans" +
    "lated draft of the source text (e.g. Bing Translator)";
            // 
            // addEncConverterToolStripMenuItem
            // 
            this.addEncConverterToolStripMenuItem.Name = "addEncConverterToolStripMenuItem";
            this.addEncConverterToolStripMenuItem.Size = new System.Drawing.Size(245, 22);
            this.addEncConverterToolStripMenuItem.Text = "&Add Translator/EncConverter";
            this.addEncConverterToolStripMenuItem.ToolTipText = "Click to add an additional Translator/EncConverter to give multiple options for t" +
    "he translated draft of the source text (e.g. DeepL Translator)";
            // 
            // fontsToolStripMenuItem
            // 
            this.fontsToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.sourceTextToolStripMenuItem,
            this.targetTextToolStripMenuItem});
            this.fontsToolStripMenuItem.Name = "fontsToolStripMenuItem";
            this.fontsToolStripMenuItem.Size = new System.Drawing.Size(245, 22);
            this.fontsToolStripMenuItem.Text = "&Fonts";
            // 
            // sourceTextToolStripMenuItem
            // 
            this.sourceTextToolStripMenuItem.Name = "sourceTextToolStripMenuItem";
            this.sourceTextToolStripMenuItem.Size = new System.Drawing.Size(133, 22);
            this.sourceTextToolStripMenuItem.Text = "&Source text";
            // 
            // targetTextToolStripMenuItem
            // 
            this.targetTextToolStripMenuItem.Name = "targetTextToolStripMenuItem";
            this.targetTextToolStripMenuItem.Size = new System.Drawing.Size(133, 22);
            this.targetTextToolStripMenuItem.Text = "&Target text";
            // 
            // autoSaveToolStripMenuItem
            // 
            this.autoSaveToolStripMenuItem.Checked = true;
            this.autoSaveToolStripMenuItem.CheckOnClick = true;
            this.autoSaveToolStripMenuItem.CheckState = System.Windows.Forms.CheckState.Checked;
            this.autoSaveToolStripMenuItem.Name = "autoSaveToolStripMenuItem";
            this.autoSaveToolStripMenuItem.Size = new System.Drawing.Size(245, 22);
            this.autoSaveToolStripMenuItem.Text = "&Auto-Save";
            this.autoSaveToolStripMenuItem.ToolTipText = "If this is checked, then the translated text changes will be saved when clicking " +
    "the \'Next\' button";
            // 
            // xWebBrowser
            // 
            this.xWebBrowser.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.xWebBrowser.Location = new System.Drawing.Point(4, 28);
            this.xWebBrowser.Name = "xWebBrowser";
            this.xWebBrowser.Size = new System.Drawing.Size(982, 474);
            this.xWebBrowser.TabIndex = 2;
            this.xWebBrowser.Url = null;
            // 
            // BackTranslationHelperView
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.xWebBrowser);
            this.Controls.Add(this.menuStrip);
            this.Name = "BackTranslationHelperView";
            this.Size = new System.Drawing.Size(986, 502);
            this.menuStrip.ResumeLayout(false);
            this.menuStrip.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.MenuStrip menuStrip;
        private System.Windows.Forms.ToolStripMenuItem settingsToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem changeEncConverterToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem addEncConverterToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem fontsToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem sourceTextToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem targetTextToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem autoSaveToolStripMenuItem;
        private System.Windows.Forms.ToolTip toolTip;
        private SIL.Windows.Forms.HtmlBrowser.XWebBrowser xWebBrowser;
    }
}
