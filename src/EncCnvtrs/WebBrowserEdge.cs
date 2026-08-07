using Microsoft.Web.WebView2.Core;
using Microsoft.Web.WebView2.WinForms;
using System;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO;
using System.Linq;
using ECInterfaces;
using Microsoft.Win32;

namespace SilEncConverters40
{
	// ReSharper disable once UnusedMember.Global
	/// <remarks>This class is instantiated only by reflection to avoid runtime errors on Mono</remarks>
	public class WebBrowserEdge : WebBrowserAdaptor
	{
		private WebView2 _webBrowser { get; set; }

		public ManualResetEvent waitForCoreWebView2Loaded;

		// captured from whichever of the two channels below actually reports the failure - either
		// can be the one that fires, depending on WebView2 SDK/runtime version (see Initialize() and
		// WebView_CoreWebView2InitializationCompleted)
		private Exception _coreWebView2InitializationException;

		public WebBrowserEdge()
			: base(WhichBrowser.Edge)
		{
			InitializeComponent();
		}

		private void InitializeComponent()
		{
			_webBrowser = new WebView2
			{
				CreationProperties = null,
				DefaultBackgroundColor = System.Drawing.Color.White,
				Location = new System.Drawing.Point(12, 12),
				Name = "EcWebView2",
				Size = new System.Drawing.Size(776, 381),
				TabIndex = 0,
				ZoomFactor = 1D
			};

			_webBrowser.NavigationCompleted += WebView_NavigationCompleted;

			((System.ComponentModel.ISupportInitialize)_webBrowser).BeginInit();
			SuspendLayout();
			Controls.Add(_webBrowser);
			((System.ComponentModel.ISupportInitialize)(_webBrowser)).EndInit();
			ResumeLayout(false);
		}

		private void WebView_NavigationCompleted(object sender, CoreWebView2NavigationCompletedEventArgs e)
		{
			OnDocumentCompleted(e);
		}

		public override void Initialize()
		{
			base.Initialize();
			var path = Path.Combine(Path.GetTempPath(), "EncConverters_WebView2Browser");
			var env = CoreWebView2Environment.CreateAsync(userDataFolder: path).Result;

			_webBrowser.CoreWebView2InitializationCompleted += WebView_CoreWebView2InitializationCompleted;
			waitForCoreWebView2Loaded = new ManualResetEvent(false);
			// calling w/o await (and letting DoEvents happen until CoreWebView2InitializationCompleted) -
			// but EnsureCoreWebView2Async can fault directly instead of (or in addition to) raising that
			// event with IsSuccess=false, depending on the failure and the installed WebView2 SDK/runtime
			// version. Since nothing else awaits this task, capture a fault here too, or it's silently lost
			// as an unobserved task exception and all NavigateAsync/callers get to see is "CoreWebView2 is
			// null", with no idea why.
			InitializeAsync(env).ContinueWith(t =>
			{
				if (t.IsFaulted)
					_coreWebView2InitializationException = t.Exception?.Flatten().InnerException ?? t.Exception;
				waitForCoreWebView2Loaded.Set();
			}, TaskScheduler.Default);
			while (!waitForCoreWebView2Loaded.WaitOne(200))
				Application.DoEvents();

			if (_webBrowser.CoreWebView2 == null)
				throw new ApplicationException(BuildCoreWebView2FailureMessage(), _coreWebView2InitializationException);
		}

		private string BuildCoreWebView2FailureMessage()
		{
			var reason = _coreWebView2InitializationException?.Message;
			return "the WebView2 runtime appears to not match the version of the Edge controller we're using"
				+ (String.IsNullOrEmpty(reason) ? "" : $" - reason reported by WebView2: {reason}");
		}

		private async Task InitializeAsync(CoreWebView2Environment env)
		{
			await Task.Run(async delegate
			{
				await _webBrowser.EnsureCoreWebView2Async(env);
			}).ConfigureAwait(false);			
		}

		private void WebView_CoreWebView2InitializationCompleted(object sender, CoreWebView2InitializationCompletedEventArgs e)
		{
			// e.IsSuccess/e.InitializationException are exactly how WebView2 reports *why* this failed
			// (e.g. a genuine loader/runtime version mismatch) without needing to catch an exception -
			// capture it here so the ApplicationException thrown below (or from NavigateAsync) can
			// actually say why instead of just guessing.
			if (!e.IsSuccess)
				_coreWebView2InitializationException = e.InitializationException;

			// this event, which should indicate that CoreWebView2 is non-null, seems to not work in some versions...
			//  workaround: if it's still null, then keep waiting (but note, we won't get this event again, so wait in
			//	the line above...
			waitForCoreWebView2Loaded.Set();
		}

		private string _returnedValue;

		public async Task ExecJavaScript(string userScript)
		{
			_returnedValue = null;
			_returnedValue = await _webBrowser.ExecuteScriptAsync(userScript);
		}

		public string CallWebView(string userScript)
		{
			var waitForFunctionComplete = new ManualResetEvent(false);
			var awaiter = ExecJavaScript(userScript).GetAwaiter();
			while (!waitForFunctionComplete.WaitOne(200))
			{
				if (awaiter.IsCompleted)
					waitForFunctionComplete.Set();
				else
					Application.DoEvents();
			}
			return _returnedValue;
		}

#pragma warning disable CS1998 // Async method lacks 'await' operators and will run synchronously (intentionally)
		public async override Task<string> GetInnerTextAsync(string htmlElementId)
#pragma warning restore CS1998 // Async method lacks 'await' operators and will run synchronously
		{
			var str = CallWebView($"document.getElementById(\"{htmlElementId}\").value");
			if (!String.IsNullOrEmpty(str) && (str.Length > 2) && (str.First() == '"') && (str.Last() == '"'))
				str = str.Substring(1, str.Length - 2);
			return str;
		}

		public async override Task<string> SetInnerTextAsync(string htmlElementId, string value)
		{
			CallWebView($"document.getElementById(\"{htmlElementId}\").value = '{value}';");

			return await GetInnerTextAsync(htmlElementId);
		}

#pragma warning disable CS1998 // Async method lacks 'await' operators and will run synchronously (intentionally)
		public async override Task<string> ExecuteScriptFunctionAsync(string functionName)
#pragma warning restore CS1998 // Async method lacks 'await' operators and will run synchronously
		{
			return CallWebView($"{functionName}()");
		}

		public override Task NavigateAsync(string filePath)
		{
			// if the test fails at this line (bkz CoreWebView2 is null), chances are you don't have
			//	the webview2 runtime installed or up to date.
			// To install or update the WebView2 Runtime: Go to page https://developer.microsoft.com/en-us/microsoft-edge/webview2/
			// (this shouldn't normally be reachable - Initialize() already throws with the same,
			// better-informed message if CoreWebView2 never came up - but kept as a defensive check
			// in case CoreWebView2 becomes null some other way, e.g. disposal, after Initialize() ran)
			if (_webBrowser.CoreWebView2 == null)
				throw new ApplicationException(BuildCoreWebView2FailureMessage(), _coreWebView2InitializationException);

			_webBrowser.CoreWebView2.Navigate(filePath);
			return Task.Delay(0);
		}
	}

	/// <summary>
	/// This class holds static methods extracted from WebBrowserEdge so we don't need reflective calls to avoid Mono runtime problems.
	/// </summary>
	public static class WebBrowserEdgeInfo
	{
		// prefer Edge to IE
		public static bool ShouldUseBrowser => !Util.IsUnix && WindowsUserWantsToUseEdge;

		/// <summary>
		/// this will return true if the user has set the 'UseEdge' registry key to 'True'
		/// </summary>
		private static bool WindowsUserWantsToUseEdge
		{
			get
			{
				var regKeySecRoot = Registry.LocalMachine.OpenSubKey(EncConverters.SEC_ROOT_KEY);
				return regKeySecRoot != null &&
					regKeySecRoot.GetValue(EncConverters.CstrUseEdgeRegKey, "False") as string == "True";
			}
		}

		public static bool IsWebView2RuntimeInstalled => !string.IsNullOrEmpty(EdgeAvailableBrowserVersion);

		public static string EdgeAvailableBrowserVersion
		{
			get
			{
				try
				{
					return CoreWebView2Environment.GetAvailableBrowserVersionString();
				}
				catch (Exception ex)
				{
					LogExceptionMessage("EdgeAvailableBrowserVersion", ex);
					return null;
				}
			}
		}

		public static string LogExceptionMessage(string className, Exception ex)
		{
			string msg = "Error occurred: " + ex.Message;
			while (ex.InnerException != null)
			{
				ex = ex.InnerException;
				msg += $"{Environment.NewLine}because: (InnerException): {ex.Message}";
			}

			Util.DebugWriteLine(className, msg);
			return msg;
		}
	}
}
