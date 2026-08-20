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

		// generous enough for a legitimate (if slow) cold WebView2/Evergreen-runtime startup, but
		// bounded so a genuinely stuck native COM call surfaces as a catchable failure instead of
		// hanging Initialize() (and thus this converter's very first Convert() call - see
		// TechHindiSiteEncConverter.PreConvert) forever. Confirmed necessary, not theoretical: a live
		// dotnet-dump capture of an actual hang showed a thread permanently inside
		// ICoreWebView2Environment.CreateCoreWebView2Controller, whose completion callback simply never
		// fired - no fault, no event, nothing else to ever observe. Once either wait below times out
		// and throws, EncConverter.Convert()'s own catch-and-retry (see Handover.md, "Generic
		// subprocess fallback for host-process conflicts") takes over exactly as it would for any
		// other exception from this same call path.
		private const int WebView2InitializationTimeoutSeconds = 20;

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

			// bounded, not the bare ".Result" this used to be: that blocks forever on whatever thread
			// calls Initialize() if CreateAsync's underlying native call never completes, exactly the
			// same failure shape confirmed below for EnsureCoreWebView2Async - see the timeout constant's
			// doc comment.
			var createEnvTask = CoreWebView2Environment.CreateAsync(userDataFolder: path);
			if (!createEnvTask.Wait(TimeSpan.FromSeconds(WebView2InitializationTimeoutSeconds)))
			{
				// nothing WebView2-specific pending yet at this point (no controller was ever
				// requested), so there's nothing to Dispose() to cancel - just abandon this task.
				throw new ApplicationException(BuildTimeoutMessage("creating the WebView2 environment"));
			}
			var env = createEnvTask.Result;

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

			// bounded, not infinite: confirmed via a live dotnet-dump capture that the underlying native
			// COM call (ICoreWebView2Environment.CreateCoreWebView2Controller) can simply never complete -
			// no fault, no event, nothing to observe, ever - so this loop used to hang forever in that
			// case. A deadline turns that into a catchable failure instead.
			var deadline = DateTime.UtcNow.AddSeconds(WebView2InitializationTimeoutSeconds);
			while (!waitForCoreWebView2Loaded.WaitOne(200))
			{
				if (DateTime.UtcNow >= deadline)
				{
					// per WebView2's own documented cancellation contract: Dispose() (not just walking
					// away and letting GC/finalization deal with it later) is the supported way to
					// abandon a pending CoreWebView2 initialization - it tears down the underlying
					// controller and suppresses CoreWebView2InitializationCompleted from firing late.
					// Confirmed necessary, not just tidy: without this, a live repro of this exact
					// timeout crashed the whole process later during window cleanup ("Failed to
					// unregister class Chrome_WidgetWin_0") instead of just failing this one call.
					DisposeAbandonedWebBrowser();
					throw new ApplicationException(BuildTimeoutMessage("waiting for CoreWebView2InitializationCompleted"));
				}
				Application.DoEvents();
			}

			if (_webBrowser.CoreWebView2 == null)
				throw new ApplicationException(BuildCoreWebView2FailureMessage(), _coreWebView2InitializationException);
		}

		private string BuildCoreWebView2FailureMessage()
		{
			var reason = _coreWebView2InitializationException?.Message;
			return "the WebView2 runtime appears to not match the version of the Edge controller we're using"
				+ (String.IsNullOrEmpty(reason) ? "" : $" - reason reported by WebView2: {reason}");
		}

		private static string BuildTimeoutMessage(string whatTimedOut)
		{
			return $"Timed out after {WebView2InitializationTimeoutSeconds}s {whatTimedOut} - this can "
				+ "happen when a native WebView2 COM call never completes (confirmed via a live process "
				+ "dump: no fault, no event raised, nothing else to observe), e.g. a runtime/version "
				+ "conflict or a stuck shared browser process this code has no way to detect on its own.";
		}

		/// <summary>
		/// Per WebView2's own documented cancellation contract, Dispose() -- not just abandoning the
		/// control and letting GC/finalization deal with it whenever that eventually happens -- is the
		/// supported way to cancel a pending CoreWebView2 initialization: it tears down the underlying
		/// controller and suppresses CoreWebView2InitializationCompleted from firing late against an
		/// object nothing is listening to anymore. Confirmed necessary, not just tidy, by reproducing a
		/// timeout without this call: the process later crashed during window cleanup ("Failed to
		/// unregister class Chrome_WidgetWin_0") instead of just failing the one call that timed out.
		/// Best-effort: this already runs while reporting one failure, so a second one here (e.g.
		/// Dispose() itself throwing for some unrelated reason) must not mask the original.
		/// </summary>
		private void DisposeAbandonedWebBrowser()
		{
			try
			{
				_webBrowser.CoreWebView2InitializationCompleted -= WebView_CoreWebView2InitializationCompleted;
				_webBrowser.Dispose();
			}
			catch
			{
			}
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
