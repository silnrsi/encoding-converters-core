using System.Diagnostics;
using System.Windows.Forms;

namespace SilEncConverters40.EcTranslators.GoogleTranslator
{
	public partial class QueryForGoogleCredentials : Form
	{
		private const string InstructionsFormat = "see {0} for instructions on creating your own Google Translate Resource";

		public QueryForGoogleCredentials(string googleTranslatorCredentials)
		{
			InitializeComponent();

			textBoxTranslatorKey.Text = googleTranslatorCredentials;
			var translatorKeyLocationDialogInstructionUrl = Properties.Resources.GoogleTranslatorKeyDialogInstructionUrl;
			var translatorKeyLocationDialogInstruction = string.Format(InstructionsFormat, translatorKeyLocationDialogInstructionUrl);
			linkLabelInstructions.Text = translatorKeyLocationDialogInstruction;
			linkLabelInstructions.Links.Add(4, translatorKeyLocationDialogInstructionUrl.Length,
				translatorKeyLocationDialogInstructionUrl);
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
			Process.Start(Properties.Resources.GoogleTranslatorKeyDialogInstructionUrl);
		}

		private void buttonOK_Click(object sender, System.EventArgs e)
		{
			DialogResult = DialogResult.OK;
			Close();
		}
	}
}
