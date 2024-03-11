using System.Diagnostics;
using System.Windows.Forms;

namespace SilEncConverters40.EcTranslators.VertexAi
{
	public partial class QueryForGoogleCredentials : Form
	{
		private const string InstructionsFormat = "see {0} for instructions on creating your own Google Cloud/Vertex Resource";

		public QueryForGoogleCredentials(string googleTranslatorCredentials)
		{
			InitializeComponent();

			textBoxTranslatorKey.Text = googleTranslatorCredentials;
			var translatorKeyLocationDialogInstructionUrl = Properties.Resources.GoogleCloudVertexCredentialsDialogInstructionUrl;
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
			Process.Start(Properties.Resources.GoogleCloudVertexCredentialsDialogInstructionUrl);
		}

		private void buttonOK_Click(object sender, System.EventArgs e)
		{
			DialogResult = DialogResult.OK;
			Close();
		}
	}
}
