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
			// if the initialization of Gecko fails, then prepare an 'instruction' pane for the caller
			//	this is assuming that the caller is putting what it was going to get in a Form, but not all
			//	callers do that... So it's up to them to do this if they get back 'null':
			// this.Controls.Add(WebBrowserAdaptor.LabelsPanel);
			Util.DebugWriteLine("WebBrowserInstructions", "Could not use GeckoFx");
			LabelsPanel = new TableLayoutPanel
			{
				Dock = DockStyle.Fill,
				ColumnCount = 1,
				RowCount = (Util.IsUnix) ? 2 : 3,
			};
			LabelsPanel.RowStyles.Add(new RowStyle(SizeType.Percent, 30F));
			LabelsPanel.RowStyles.Add(new RowStyle(SizeType.Percent, 35F));
			LabelsPanel.Controls.Add(InstructionsForFireFox, 0, 1);
			if (!Util.IsUnix)
			{
				LabelsPanel.RowCount = 3;
				LabelsPanel.RowStyles.Add(new RowStyle(SizeType.Percent, 35F));
				LabelsPanel.Controls.Add(InstructionsLinkLabelEdge, 0, 2);
			}
			Controls.Add(LabelsPanel);
		}


		public static Label InstructionsForFireFox
		{
			get
			{
				string strInstructions;
				if (Util.IsUnix)
				{
					strInstructions = "To use Mozilla to display the help file, install the firefox-geckofx package.";
				}
				else
				{
					strInstructions = "You seem to be missing the Firefox native DLLs that are usually in the \"<Install Directory>\\Firefox\" folder. You should re-run the installer and make sure Firefox is selected to be installed,"
										+ string.Format(@" or change the registry key 'HKLM\{0}\{1}' to False",
#if X64
													 EncConverters.SEC_ROOT_KEY,
#else
													 EncConverters.SEC_ROOT_KEY.Replace("SOFTWARE", @"SOFTWARE\WOW6432Node"),
#endif
													 EncConverters.CstrUseGeckoRegKey);
				}

				var labelInstructions = new Label
				{
					Text = strInstructions,						
					Dock = DockStyle.Fill
				};

				return labelInstructions;
			}
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
													 WebBrowserEdge.EdgeAvailableBrowserVersion),
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
