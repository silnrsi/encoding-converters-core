using ECInterfaces;
using System.Diagnostics;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace SilEncConverters40
{
    public class WebBrowserInstructions : WebBrowserAdaptor
    {
        public static TableLayoutPanel LabelsPanel { get; set; }

        public WebBrowserInstructions()
            : base(WhichBrowser.Instructions)
        {
            // if the initialization of the browser fails, then prepare an 'instruction' pane for the caller
            //    this is assuming that the caller is putting what it was going to get in a Form, but not all
            //    callers do that... So it's up to them to do this if they get back 'null':
            // this.Controls.Add(WebBrowserAdaptor.LabelsPanel);
            Util.DebugWriteLine("WebBrowserInstructions", "Could not use Edge");
            LabelsPanel = new TableLayoutPanel
            {
                Dock = DockStyle.Fill,
                ColumnCount = 1,
                RowCount = 1,
            };
            LabelsPanel.RowStyles.Add(new RowStyle(SizeType.Percent, 100F));
            LabelsPanel.Controls.Add(InstructionsLinkLabelEdge, 0, 0);
            Controls.Add(LabelsPanel);
        }

        public static LinkLabel InstructionsLinkLabelEdge
        {
            get
            {
                const string cstrLinkPrefix = "To use Microsoft Edge to display the help files, download and install its runtime from: ";
                const string cstrEvergreenEdgeLink = "https://go.microsoft.com/fwlink/p/?LinkId=2124703";

                var runDirectory = DirectoryOfTheApplicationExecutable;

                var labelInstructions = new LinkLabel
                {
                    Text = cstrLinkPrefix + cstrEvergreenEdgeLink + string.Format(@". Otherwise, change the registry key 'HKLM\{0}\{1}' to False (currently, CoreWebView2Environment.GetAvailableBrowserVersionString() = '{2}'",
#if X64
                                                     EncConverters.SEC_ROOT_KEY,
#else
                                                     EncConverters.SEC_ROOT_KEY.Replace("SOFTWARE", @"SOFTWARE\WOW6432Node"),
#endif
                                                     EncConverters.CstrUseEdgeRegKey,
                                                     WebBrowserEdgeInfo.EdgeAvailableBrowserVersion),
                    Dock = DockStyle.Fill
                };

                labelInstructions.Links.Add(cstrLinkPrefix.Length, cstrEvergreenEdgeLink.Length, cstrEvergreenEdgeLink);

                labelInstructions.LinkClicked += (sender, args) =>
                {
                    if (args.Link.LinkData != null)
                        Process.Start(args.Link.LinkData as string);
                };

                return labelInstructions;
            }
        }

        public override Task<string> GetInnerTextAsync(string htmlElementId)
        {
            throw new System.NotImplementedException();
        }

        public override Task<string> SetInnerTextAsync(string htmlElementId, string value)
        {
            throw new System.NotImplementedException();
        }

        public override Task<string> ExecuteScriptFunctionAsync(string functionName)
        {
            throw new System.NotImplementedException();
        }

        public override Task NavigateAsync(string filePath)
        {
            OnDocumentCompleted(new System.EventArgs());
            return Task.CompletedTask;
        }
    }
}
