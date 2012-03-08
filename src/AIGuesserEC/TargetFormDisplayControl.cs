using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace SilEncConverters40
{
    public partial class TargetFormDisplayControl : UserControl
    {
        public Font TargetWordFont { get; set; }
        public RightToLeft TargetWordRightToLeft { get; set; }

        public TargetFormDisplayControl()
        {
            InitializeComponent();
        }

        private SourceWordElement SourceWordElement { get; set; }

        public delegate void DeleteSourceWord(string strSourceWord);
        public delegate void SetModified();

        internal void Initialize(SourceWordElement sourceWordElement,
            DeleteSourceWord functionToDeleteSourceWord)
        {
            System.Diagnostics.Debug.Assert((sourceWordElement != null)
                                            && (functionToDeleteSourceWord != null));

            SourceWordElement = sourceWordElement;
            CallToDeleteSourceWord = functionToDeleteSourceWord;
            // TODO: sourceWordElement.Descendants() can be null, which is bad
            foreach (TargetWordElement strTargetForm in sourceWordElement.Descendants())
                AddToPanel(strTargetForm);
            buttonAdd.Enabled = true;
        }

        public void Reset()
        {
            buttonAdd.Enabled = false;
            flowLayoutPanelTargetWords.Controls.Clear();
        }

        private Control AddToPanel(TargetWordElement targetWordElement)
        {
            var textBox = new TextBoxWithButtons(this)
            {
                TargetWordElement = targetWordElement,
                Font = TargetWordFont,
                RightToLeft = TargetWordRightToLeft
            };

            flowLayoutPanelTargetWords.Controls.Add(textBox);
            return textBox;
        }

        public SetModified CallToSetModified { get; set; }
        public DeleteSourceWord CallToDeleteSourceWord { get; set; }
        public void DeleteTextWithButtons(TextBoxWithButtons textBoxWithButtons)
        {
            flowLayoutPanelTargetWords.Controls.Remove(textBoxWithButtons);
            SourceWordElement.Remove(textBoxWithButtons.TargetWordElement.TargetWord);
            
            if (flowLayoutPanelTargetWords.Controls.Count == 0)
            {
                DialogResult res = MessageBox.Show(String.Format(Properties.Resources.IDS_QueryToDeleteSourceWord,
                                                                 SourceWordElement.SourceWord),
                                                   EncConverters.cstrCaption, MessageBoxButtons.YesNoCancel);
                if (res == DialogResult.Yes)
                    CallToDeleteSourceWord(SourceWordElement.SourceWord);
            }
            CallToSetModified();
        }

        public void MoveUpTextWithButtons(TextBoxWithButtons textBoxWithButtons)
        {
            int nIndex = flowLayoutPanelTargetWords.Controls.IndexOf(textBoxWithButtons);
            if (nIndex <= 0)
                return;

            int nNewIndex = nIndex - 1;
            SourceWordElement.ReorderTargetForms(nIndex, nNewIndex);
            flowLayoutPanelTargetWords.Controls.SetChildIndex(textBoxWithButtons, nNewIndex);
            CallToSetModified();
        }

        public void MoveDownTextWithButtons(TextBoxWithButtons textBoxWithButtons)
        {
            int nIndex = flowLayoutPanelTargetWords.Controls.IndexOf(textBoxWithButtons);
            if (nIndex >= flowLayoutPanelTargetWords.Controls.Count - 1)
                return;

            int nNewIndex = nIndex + 1;
            SourceWordElement.ReorderTargetForms(nIndex, nNewIndex);
            flowLayoutPanelTargetWords.Controls.SetChildIndex(textBoxWithButtons, nNewIndex);
            CallToSetModified();
        }

        public bool AreAllTargetFormsNonEmpty(char[] achTrim)
        {
            return (from TextBoxWithButtons control in flowLayoutPanelTargetWords.Controls 
                    select control.TargetWordElement.TargetWord.Trim(achTrim)).All(str => !String.IsNullOrEmpty(str));
        }

        public void TrimTargetWordForms(char[] achTrim)
        {
            System.Diagnostics.Debug.Assert(AreAllTargetFormsNonEmpty(achTrim)); // should be called first
            foreach (TextBoxWithButtons ctrl in flowLayoutPanelTargetWords.Controls)
                ctrl.TargetWordElement.TrimTargetWord(achTrim);
        }

        private void buttonAdd_Click(object sender, EventArgs e)
        {
            Control textBox = AddToPanel(SourceWordElement.GetNewTargetWord);
            textBox.Focus();
            CallToSetModified();
        }
    }
}
