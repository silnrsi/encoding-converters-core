using System.Diagnostics;
using System.Windows.Forms;

namespace SilEncConverters40.EcTranslators.BingTranslator
{
	public partial class QueryForAzureKeyAndLocation : Form
	{
		private const string InstructionsFormat = "see {0} for instructions on creating your own Azure Translator Resource";
		public QueryForAzureKeyAndLocation(string azureTranslatorKey, string azureTranslatorRegion)
		{
			InitializeComponent();

			textBoxAzureTranslatorKey.Text = azureTranslatorKey;
			textBoxAzureTranslatorLocation.Text = azureTranslatorRegion;
			var azureTranslatorKeyLocationDialogInstructionUrl = Properties.Settings.Default.AzureTranslatorKeyLocationDialogInstructionUrl;
			var azureTranslatorKeyLocationDialogInstruction = string.Format(InstructionsFormat, azureTranslatorKeyLocationDialogInstructionUrl);
			linkLabelInstructions.Text = azureTranslatorKeyLocationDialogInstruction;
			linkLabelInstructions.Links.Add(4, azureTranslatorKeyLocationDialogInstructionUrl.Length,
				azureTranslatorKeyLocationDialogInstructionUrl);
		}

		public string AzureTranslatorKey
		{
			get
			{
				return textBoxAzureTranslatorKey.Text;
			}
		}

		public string AzureTranslatorLocation
		{
			get
			{
				return textBoxAzureTranslatorLocation.Text;
			}
		}

		private void linkLabelInstructions_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
		{
			Process.Start("https://docs.microsoft.com/en-us/azure/cognitive-services/translator/quickstart-translator");
		}

		private void buttonOK_Click(object sender, System.EventArgs e)
		{
			DialogResult = DialogResult.OK;
			Close();
		}
	}
}
