// 21-May-2013 JDK  Add link to help file in instructions.
// 24-Jun-2013 JDK  Loading Gecko libs may fail, so supply a macro to disable.
// 6-Sept-2021 BE	Added Edge browser WebView2 and moved them all to sub-classes

using System;
using System.Windows.Forms;
using Microsoft.Win32;
using ECInterfaces;     // for Util
using System.ComponentModel;
using System.Threading.Tasks;
using System.Threading;
using System.Reflection;
using System.IO;

namespace SilEncConverters40
{
	/// <summary>
	/// This class is an attempt at providing a very basic WebBrowser like control, but which can be used for GeckoFx (Mozilla) if it exists instead
	/// </summary>
	public abstract partial class WebBrowserAdaptor : UserControl
    {
        public enum WhichBrowser
        {
            Undefined = 0,
            InternetExplorer,
            GeckoFx,
            Instructions,   // instructions to install browser libs
			Edge
        }

        public WhichBrowser BrowserType { get; set; }

		protected WebBrowserAdaptor(WhichBrowser browserType)
		{
			BrowserType = browserType;
		}

		public static WebBrowserAdaptor CreateBrowser(WhichBrowser browserType = WhichBrowser.Undefined)
		{
			WebBrowserAdaptor webBrowserAdaptor;
			switch (browserType)
			{
				case WhichBrowser.InternetExplorer:
					webBrowserAdaptor = new WebBrowserIE();
					break;
				case WhichBrowser.GeckoFx:
					webBrowserAdaptor = new WebBrowserGecko();
					break;
				case WhichBrowser.Edge:
					webBrowserAdaptor = new WebBrowserEdge();
					break;

				default:
					// means for us to figure it out based on OS and reg settings
					// on linux, only Gecko works (on Windows, either Gecko or IE will work, but using IE saves us from having to redistribute too much)
					//  so on Linux, prefer Gecko, but on Windows, prefer IE.
					if (ShouldUseGecko)
					{
						try
						{
							webBrowserAdaptor = new WebBrowserGecko();
						}
						catch (Exception)
						{
							webBrowserAdaptor = new WebBrowserInstructions();
						}
					}
					else if (ShouldUseEdge)
					{
						try
						{
							webBrowserAdaptor = new WebBrowserEdge();
						}
						catch (Exception)
						{
							webBrowserAdaptor = new WebBrowserInstructions();
						}
					}
					else if (!Util.IsUnix)
					{
						webBrowserAdaptor = new WebBrowserIE();
					}
					else
					{
						Util.DebugWriteLine("WebBrowserAdaptor.CreateBrowser", "Could not use GeckoFx");
						webBrowserAdaptor = new WebBrowserInstructions();
					}
					break;
			}

			return webBrowserAdaptor;
		}

		private static bool ShouldUseGecko
        {
            get
            {
                if (Util.IsUnix)
                {
                    return true;
                }
                else
                {
                    return WindowsUserWantsToUseGecko;
                }
            }
        }

        /// <summary>
        /// this will return true if the user has set the 'UseGeckoFx' registry key to 'True' AND put the xulRunner folder
        /// in the target installation dir
        /// </summary>
        private static bool WindowsUserWantsToUseGecko
        {
            get
            {
                var regKeySecRoot = Registry.LocalMachine.OpenSubKey(EncConverters.SEC_ROOT_KEY);
                return (regKeySecRoot != null) &&
                       (regKeySecRoot.GetValue(EncConverters.CstrUseGeckoRegKey, "False") as string == "True");
            }
        }

		private static bool ShouldUseEdge
		{
			get
			{
				if (Util.IsUnix)
				{
					return false;
				}
				else
				{
					// prefer Edge to IE
					return WebBrowserEdge.IsWebView2RuntimeInstalled || WindowsUserWantsToUseEdge;
				}
			}
		}

		/// <summary>
		/// this will return true if the user has set the 'UseEdge' registry key to 'True'
		/// </summary>
		private static bool WindowsUserWantsToUseEdge
		{
			get
			{
				var regKeySecRoot = Registry.LocalMachine.OpenSubKey(EncConverters.SEC_ROOT_KEY);
				return (regKeySecRoot != null) &&
					   (regKeySecRoot.GetValue(EncConverters.CstrUseEdgeRegKey, "False") as string == "True");
			}
		}

		public virtual void Initialize()
		{
			Util.DebugWriteLine(this, "BEGIN");
			InitializeComponent();
			Util.DebugWriteLine(this, "END");
		}

		public abstract Task<string> GetInnerTextAsync(string htmlElementId);
		public abstract Task<string> SetInnerTextAsync(string htmlElementId, string value);
		public abstract Task<string> ExecuteScriptFunctionAsync(string functionName);

		/// <summary>
		/// navigate to a page and (if you've initialized 'DocumentCompleted' (e.g. _webBrowser.DocumentCompleted += <EventHandler function>),
		/// then you will get called back when it's finished loading. See aso Navigate, which waits until it is completely loaded before returning
		/// </summary>
		/// <param name="filePath"></param>
		/// <returns></returns>
		//	
		public abstract Task NavigateAsync(string filePath);
		/// <summary>
		/// navigate to a page and return when it's finished loading. This shouldn't interfere with user-interface while waiting
		/// </summary>
		/// <param name="filePath"></param>
		public async void Navigate(string filePath)
		{
			// if the using application doesn't set up the DocumentCompleted event, then we have to do it here
			if (listEventDelegates[documentCompletedEventKey] == null)
				waitForPageLoaded = new ManualResetEvent(false);

			// call the async part of it
			await NavigateAsync(filePath);

			// and now wait for it to finish (allowing events, since several of these browser types need msg loop processing)
			while (!waitForPageLoaded.WaitOne(200))
				Application.DoEvents();
		}

		protected readonly object documentCompletedEventKey = new();
		protected ManualResetEvent waitForPageLoaded;
		protected EventHandlerList listEventDelegates = new EventHandlerList();
		public event EventHandler DocumentCompleted
		{
			add
			{
				waitForPageLoaded = new ManualResetEvent(false);
				listEventDelegates.AddHandler(documentCompletedEventKey, value);
			}
			remove
			{
				listEventDelegates.RemoveHandler(documentCompletedEventKey, value);
			}
		}

		protected void OnDocumentCompleted(EventArgs e)
		{
			waitForPageLoaded.Set();
			((EventHandler)listEventDelegates[documentCompletedEventKey])?.Invoke(this, e);
		}

		public static string DirectoryOfTheApplicationExecutable
		{
			get
			{
				string path;
				bool unitTesting = Assembly.GetEntryAssembly() == null;
				if (unitTesting)
				{
					path = new Uri(Assembly.GetExecutingAssembly().CodeBase).AbsolutePath;
					path = Uri.UnescapeDataString(path);
				}
				else
				{
					path = Assembly.GetEntryAssembly().Location;
				}
				return Directory.GetParent(path).FullName;
			}
		}
	}
}
