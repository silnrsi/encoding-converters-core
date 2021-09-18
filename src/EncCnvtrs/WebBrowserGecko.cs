using ECInterfaces;
using Gecko;
using Gecko.DOM;
using Gecko.Events;
using System;
using System.Drawing;
using System.IO;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace SilEncConverters40
{
	public class WebBrowserGecko : WebBrowserAdaptor
	{
		private GeckoWebBrowser _webBrowser { get; set; }

		public WebBrowserGecko()
			: base(WhichBrowser.GeckoFx)
		{
			// try to initialize
			// if GeckoFx was successfully initialized, then use it
			if (SetUpXulRunner())
			{
				Util.DebugWriteLine(this, "Using GeckoFx");
				_webBrowser = new GeckoWebBrowser
				{
					Dock = DockStyle.Fill,
					Location = new Point(3, 3),
					MinimumSize = new Size(20, 20),
					Name = "geckoWebBrowser",
					Size = new Size(596, 394),
					TabIndex = 0,
					FrameEventsPropagateToMainWindow = false,
					UseHttpActivityObserver = false
				};
				Controls.Add(_webBrowser);
				_webBrowser.DocumentCompleted += GeckoDocumentCompleted;
			}
			else
				throw new ApplicationException($"Unable to initialize Gecko installation!");
		}

		private void GeckoDocumentCompleted(object sender, GeckoDocumentCompletedEventArgs e)
		{
			OnDocumentCompleted(e);
		}

		public async override Task<string> GetInnerTextAsync(string htmlElementId)
		{
			string strOutput = null;
			var elem = _webBrowser?.Document?.GetHtmlElementById(htmlElementId) as GeckoTextAreaElement;
			if (elem != null)
			{
				strOutput = elem.Value;
			}
			return await Task.FromResult(strOutput);
		}

		public async override Task<string> SetInnerTextAsync(string htmlElementId, string value)
		{
			var elem = _webBrowser?.Document?.GetHtmlElementById(htmlElementId) as GeckoTextAreaElement;
			if (elem != null)
			{
				elem.Value = value;
			}
			return await GetInnerTextAsync(htmlElementId);
		}

		public async override Task<string> ExecuteScriptFunctionAsync(string functionName)
		{
			using AutoJSContext js = new(_webBrowser.Window);
			js.EvaluateScript($"{functionName}();", out string retVal);
			return await Task.FromResult(retVal);
		}

		public override Task NavigateAsync(string filePath)
		{
			if (Path.GetExtension(filePath) == ".mht")
			{
				var strHtm = Path.Combine(Path.GetDirectoryName(filePath),
										  Path.GetFileNameWithoutExtension(filePath) + ".htm");
				if (File.Exists(strHtm))
				{
					filePath = strHtm;
				}
				else
				{
					const string cstrFileNameToEditSaveAsHtml = "CantReadMhtFiles.htm";
					filePath = Path.Combine(Path.GetDirectoryName(filePath),
											cstrFileNameToEditSaveAsHtml);
					System.Diagnostics.Debug.Assert(File.Exists(filePath));
				}
			}

			// GeckoWebBrowser.Navigate("file://" + filePath);
			_webBrowser.Navigate(filePath);
			return Task.Delay(0);
		}

		#region FirefoxInitialization
		public static bool SetUpXulRunner()
		{
			var className = typeof(WebBrowserGecko).Name;
			Util.DebugWriteLine(className, "BEGIN");
			if (Xpcom.IsInitialized)
				return true;

			string xulRunnerPath;

			// Use XULRUNNER environment variable if set
			var xulrunnerLocation = Environment.GetEnvironmentVariable("XULRUNNER");
			if (xulrunnerLocation != null)
			{
				xulRunnerPath = xulrunnerLocation;
			}
			else
			{
				if (!XulRunnerDirectoryOfApplicationOrLib(out xulRunnerPath))
					return false;
			}
			Util.DebugWriteLine(className, "xulRunnerPath=" + xulRunnerPath);

#if _MSC_VER
            //Review: an early tester found that wrong xpcom was being loaded. The following solution is from http://www.geckofx.org/viewtopic.php?id=74&action=new
            SetDllDirectory(xulRunnerPath);
#endif

			Xpcom.Initialize(xulRunnerPath);
			Util.DebugWriteLine(className, "END");
			return true;
		}

		/// <summary>
		/// Gives an xulrunner subdirectory of either solutiondir/lib (if running from visual studio), or
		/// the installation folder.  Helpful for finding templates and things; by using this,
		/// you don't have to copy those files into the build directory during development.
		/// It assumes your build directory has "output" as part of its path.
		/// </summary>
		/// <returns></returns>
		public static bool XulRunnerDirectoryOfApplicationOrLib(out string xulRunnerPath)
		{
			if (Util.IsUnix)
			{
				xulRunnerPath = "/usr/lib/xulrunner-geckofx";
				return Directory.Exists(xulRunnerPath);	// gecko is the only option on linux
			}

			string path = DirectoryOfTheApplicationExecutable;
#if X64
			xulRunnerPath = Path.Combine(path, "FireFox64");
#else
			xulRunnerPath = Path.Combine(path, "FireFox");
#endif
			if (Directory.Exists(xulRunnerPath))
				return true;

			// this is also a possibility if they download it per the instructions page
			xulRunnerPath = Path.Combine(path, "xulrunner");
			if (Directory.Exists(xulRunnerPath))
				return true;

			//if this is a programmer, go look in the lib directory
			char sep = Path.DirectorySeparatorChar;
			int i = path.ToLower().LastIndexOf(sep + "output" + sep);
			if (i > -1)
			{
				path = path.Substring(0, i + 1);
			}
			xulRunnerPath = Path.Combine(path, Path.Combine("lib", "xulrunner"));
			return (Directory.Exists(xulRunnerPath));
		}
		#endregion
	}
}
