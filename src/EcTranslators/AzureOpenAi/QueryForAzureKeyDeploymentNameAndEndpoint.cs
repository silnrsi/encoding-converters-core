using System.Diagnostics;
using System.Windows.Forms;

namespace SilEncConverters40.EcTranslators.AzureOpenAI
{
    public partial class QueryForAzureKeyDeploymentNameAndEndpoint : Form
    {
        private const string InstructionsFormat = "see {0} for instructions on creating your own Azure Open AI Resource";
        public QueryForAzureKeyDeploymentNameAndEndpoint(string azureOpenAiKey, string azureOpenAiDeploymentName, string azureOpenAiEndpoint)
        {
            InitializeComponent();

            textBoxAzureOpenAiKey.Text = azureOpenAiKey;
            textBoxAzureOpenAiDeploymentName.Text = azureOpenAiDeploymentName;
            textBoxAzureOpenAiEndpoint.Text = azureOpenAiEndpoint;
            var azureOpenAiKeyLocationDialogInstructionUrl = Properties.Settings.Default.AzureOpenAiKeyLocationDialogInstructionUrl;
            var azureOpenAiKeyLocationDialogInstruction = string.Format(InstructionsFormat, azureOpenAiKeyLocationDialogInstructionUrl);
            linkLabelInstructions.Text = azureOpenAiKeyLocationDialogInstruction;
            linkLabelInstructions.Links.Add(4, azureOpenAiKeyLocationDialogInstructionUrl.Length,
                azureOpenAiKeyLocationDialogInstructionUrl);
        }

        public string AzureOpenAiKeyOverride
        {
            get
            {
                return textBoxAzureOpenAiKey.Text;
            }
        }

        public string AzureOpenAiDeploymentName
        {
            get
            {
                return textBoxAzureOpenAiDeploymentName.Text;
            }
        }

        public string AzureOpenAiEndpoint
        {
            get
            {
                return textBoxAzureOpenAiEndpoint.Text;
            }
        }

        private void linkLabelInstructions_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            Process.Start("https://learn.microsoft.com/en-us/azure/ai-services/openai/how-to/create-resource");
        }

        private void buttonOK_Click(object sender, System.EventArgs e)
        {
            DialogResult = DialogResult.OK;
            Close();
        }
    }
}
