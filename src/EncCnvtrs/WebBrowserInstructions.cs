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
			LabelsPanel.Controls.Add(InstructionsLinkLabel, 0, 1);
			if (!Util.IsUnix)
			{
				LabelsPanel.RowCount = 3;
				LabelsPanel.RowStyles.Add(new RowStyle(SizeType.Percent, 35F));
				LabelsPanel.Controls.Add(InstructionsLinkLabelEdge, 0, 2);
			}
			Controls.Add(LabelsPanel);
		}


		public static LinkLabel InstructionsLinkLabel
		{
			get
			{
				if (Util.IsUnix)
				{
					return new LinkLabel
					{
						Text = "To use Mozilla to display the help file, install the firefox-geckofx package.",
						Dock = DockStyle.Fill
					};
				}

				const string cstrLinkPrefix = "To use Firefox/Mozilla to display the help file, download xulRunner from ";
				const string cstrXulRunnerLink = "https://archive.mozilla.org/pub/xulrunner/releases/latest/runtimes/";

				var runDirectory = DirectoryOfTheApplicationExecutable;

				var labelInstructions = new LinkLabel
				{
					Text =
						cstrLinkPrefix + cstrXulRunnerLink + " and put the xulrunner folder as a subfolder in the " +
						runDirectory + string.Format(@" folder. Otherwise, change the registry key 'HKLM\{0}\{1}' to False",
#if X64
													 EncConverters.SEC_ROOT_KEY,
#else
													 EncConverters.SEC_ROOT_KEY.Replace("SOFTWARE", @"SOFTWARE\WOW6432Node"),
#endif
													 EncConverters.CstrUseGeckoRegKey),
					Dock = DockStyle.Fill
				};

				labelInstructions.Links.Add(cstrLinkPrefix.Length, cstrXulRunnerLink.Length, cstrXulRunnerLink);
				labelInstructions.Links.Add(labelInstructions.Text.IndexOf(runDirectory),
											runDirectory.Length,
											runDirectory);

				labelInstructions.LinkClicked += (sender, args) =>
				{
					if (args.Link.LinkData != null)
						Process.Start(args.Link.LinkData as string);
				};

				return labelInstructions;
			}
		}


		public static LinkLabel InstructionsLinkLabelEdge
		{
			get
			{
				const string cstrLinkPrefix = "To use Microsoft Edge to display the help file, download and install its runtime from: ";
				const string cstrEvergreenEdgeLink = "https://go.microsoft.com/fwlink/p/?LinkId=2124703";

				var runDirectory = DirectoryOfTheApplicationExecutable;

				var labelInstructions = new LinkLabel
				{
					Text = cstrLinkPrefix + cstrEvergreenEdgeLink + string.Format(@". Otherwise, change the registry key 'HKLM\{0}\{1}' to False",
#if X64
													 EncConverters.SEC_ROOT_KEY,
#else
													 EncConverters.SEC_ROOT_KEY.Replace("SOFTWARE", @"SOFTWARE\WOW6432Node"),
#endif
													 EncConverters.CstrUseEdgeRegKey),
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
