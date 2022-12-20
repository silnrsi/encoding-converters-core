using System.Diagnostics;
using System.Windows.Forms;

namespace SilEncConverters40.EcTranslators.DeepLTranslator
{
	public partial class QueryForDeepLKey : Form
	{
		private const string InstructionsFormat = "see {0} for instructions on creating your own DeepL or DeepL Pro Translator Resource";
		public QueryForDeepLKey(string deepLTranslatorKey)
		{
			InitializeComponent();

			textBoxTranslatorKey.Text = deepLTranslatorKey;
			var azureTranslatorKeyLocationDialogInstructionUrl = Properties.Settings.Default.DeepLTranslatorKeyDialogInstructionUrl;
			var azureTranslatorKeyLocationDialogInstruction = string.Format(InstructionsFormat, azureTranslatorKeyLocationDialogInstructionUrl);
			linkLabelInstructions.Text = azureTranslatorKeyLocationDialogInstruction;
			linkLabelInstructions.Links.Add(4, azureTranslatorKeyLocationDialogInstructionUrl.Length,
				azureTranslatorKeyLocationDialogInstructionUrl);
		}

		public string TranslatorKey
		{
			get
			{
				return textBoxTranslatorKey.Text;
			}
		}

		private void linkLabelInstructions_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
		{
			Process.Start(Properties.Settings.Default.DeepLTranslatorKeyDialogInstructionUrl);
		}

		private void buttonOK_Click(object sender, System.EventArgs e)
		{
			DialogResult = DialogResult.OK;
			Close();
		}
	}
}
