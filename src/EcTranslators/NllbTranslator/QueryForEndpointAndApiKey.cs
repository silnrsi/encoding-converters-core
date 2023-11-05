using System.Diagnostics;
using System.Windows.Forms;

namespace SilEncConverters40.EcTranslators.NllbTranslator
{
    public partial class QueryForEndpointAndApiKey : Form
    {
        public QueryForEndpointAndApiKey(string apiKey, string endpoint)
        {
            InitializeComponent();

            textBoxNllbApiKey.Text = apiKey;
            textBoxNllbEndpoint.Text = endpoint;
        }

        public string TranslatorKey
        {
            get
            {
                return textBoxNllbApiKey.Text;
            }
        }

        public string Endpoint
        {
            get
            {
                return textBoxNllbEndpoint.Text;
            }
        }

        private void buttonOK_Click(object sender, System.EventArgs e)
        {
            DialogResult = DialogResult.OK;
            Close();
        }
    }
}
