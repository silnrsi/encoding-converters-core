using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace BackTranslationHelper
{
	public partial class ExampleEditorDialog : Form
	{
		public string ExampleInput {  get; set; }
		public string ExampleOutput { get; set; }

		public List<string> TranslatorNames { get; set; } = new List<string>();

		public ExampleEditorDialog(string input, Font fontInput, string output, Font fontOutput, List<string> chatPromptTranslatorNames)
		{
			InitializeComponent();

			textBoxExampleInput.Font = fontInput;
			textBoxExampleInput.Text = input;
			textBoxExampleOutput.Font = fontOutput;
			textBoxExampleOutput.Text = output;

			if (chatPromptTranslatorNames.Count <= 1)	// if there's only 1, then no need to specify which ones you want the example sent to
				return;

			var i = 3;
			foreach(var translatorConfigName in chatPromptTranslatorNames)
			{
				var checkBox = new CheckBox
				{
					AutoSize = true,
					Name = translatorConfigName,
					Text = translatorConfigName,
					TabIndex = i++,
					UseVisualStyleBackColor = true,
					Checked = true,
				};
				flowLayoutPanel.Controls.Add(checkBox);
			}
		}

		private void ButtonCancel_Click(object sender, EventArgs e)
		{
			DialogResult = DialogResult.Cancel;
			Close();
		}

		private void ButtonOk_Click(object sender, EventArgs e)
		{
			ExampleInput = textBoxExampleInput.Text;
			ExampleOutput = textBoxExampleOutput.Text;

			foreach (var checkbox in flowLayoutPanel.Controls?.Cast<CheckBox>()?.Where(cb => cb.Checked))
			{
				TranslatorNames.Add(checkbox.Name);
			}

			DialogResult = DialogResult.OK;
			Close();
		}
	}
}
