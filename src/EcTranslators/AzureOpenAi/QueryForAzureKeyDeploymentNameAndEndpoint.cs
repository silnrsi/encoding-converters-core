using System;
using System.Diagnostics;
using System.Windows.Forms;

namespace SilEncConverters40.EcTranslators.AzureOpenAI
{
    public partial class QueryForAzureKeyDeploymentNameAndEndpoint : Form
    {
        private const string InstructionsFormat = "see {0} for instructions on creating your own Azure Open AI Resource";

        // the ending that a real Azure OpenAI resource endpoint needs (so that it can be used with the OpenAI client library --
        //  see AzureOpenAiExe's Program.cs). Generic OpenAI-compatible services (e.g. Ollama) typically already expose their
        //  endpoint in this "/v1" form, so this dialog only adds/removes it based on the "Azure OpenAI?" checkbox below.
        private const string AzureOpenAiV1EndpointSuffix = "openai/v1/";

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

            // if we're editing an existing endpoint that's already in the Azure "/openai/v1/" form, flag it as such;
            //  this also fires checkBoxIsAzureOpenAi_CheckedChanged, but that's a no-op since the suffix is already there
            checkBoxIsAzureOpenAi.Checked = EndpointHasAzureOpenAiV1Suffix(azureOpenAiEndpoint);
            linkLabelInstructions.Visible = checkBoxIsAzureOpenAi.Checked;
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

        private void checkBoxIsAzureOpenAi_CheckedChanged(object sender, System.EventArgs e)
        {
            // the Azure-specific "how to create a resource" instructions aren't relevant when pointing at some other,
            //  generic OpenAI-compatible service (e.g. Ollama)
            linkLabelInstructions.Visible = checkBoxIsAzureOpenAi.Checked;

            var endpoint = textBoxAzureOpenAiEndpoint.Text?.Trim();
            if (string.IsNullOrEmpty(endpoint))
                return;

            if (checkBoxIsAzureOpenAi.Checked)
            {
                // add the ending Azure OpenAI needs, if it isn't already there
                if (!EndpointHasAzureOpenAiV1Suffix(endpoint))
                    textBoxAzureOpenAiEndpoint.Text = $"{endpoint.TrimEnd('/')}/{AzureOpenAiV1EndpointSuffix}";
            }
            else
            {
                // remove it again, so we're back to the bare endpoint the user originally entered
                if (EndpointHasAzureOpenAiV1Suffix(endpoint))
                {
                    var trimmedEndpoint = endpoint.TrimEnd('/');
                    var suffixIndex = trimmedEndpoint.LastIndexOf("openai/v1", StringComparison.OrdinalIgnoreCase);
                    textBoxAzureOpenAiEndpoint.Text = trimmedEndpoint.Substring(0, suffixIndex).TrimEnd('/') + "/";
                }
            }
        }

        private static bool EndpointHasAzureOpenAiV1Suffix(string endpoint)
        {
            return !string.IsNullOrEmpty(endpoint) &&
                   endpoint.TrimEnd('/').EndsWith("openai/v1", StringComparison.OrdinalIgnoreCase);
        }
    }
}
