using ECInterfaces;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace SilEncConverters40
{
	public class WebBrowserIE : WebBrowserAdaptor
	{
		private WebBrowser _webBrowser { get; set; }

		public WebBrowserIE()
			: base(WhichBrowser.InternetExplorer)
		{
			Util.DebugWriteLine(this, "Using Internet Explorer");
			_webBrowser = new WebBrowser
			{
				Dock = DockStyle.Fill,
				Location = new Point(3, 3),
				MinimumSize = new Size(20, 20),
				Name = "ieWebBrowser",
				Size = new Size(596, 394),
				TabIndex = 0,
				Url = new Uri("", UriKind.Relative)
			};
			Controls.Add(_webBrowser);

			_webBrowser.DocumentCompleted += WebBrowserDocumentCompleted;
		}

		private void WebBrowserDocumentCompleted(object sender, WebBrowserDocumentCompletedEventArgs e)
		{
			OnDocumentCompleted(e);
		}

		public async override Task<string> GetInnerTextAsync(string htmlElementId)
		{
			string strOutput = null;
			var elem = _webBrowser?.Document?.GetElementById(htmlElementId);
			if (elem != null)
			{
				strOutput = elem.InnerText;
			}
			return await Task.FromResult(strOutput);
		}

		public async override Task<string> SetInnerTextAsync(string htmlElementId, string value)
		{
			var elem = _webBrowser?.Document?.GetElementById(htmlElementId);
			if (elem != null)
			{
				elem.InnerText = value;
			}
			return await GetInnerTextAsync(htmlElementId);
		}

		public async override Task<string> ExecuteScriptFunctionAsync(string functionName)
		{
			string retVal = null;
			var docHtml = _webBrowser.Document;
			if (docHtml != null)
				retVal = docHtml.InvokeScript(functionName)?.ToString();
			return await Task.FromResult(retVal);
		}

		public override Task NavigateAsync(string filePath)
		{
			_webBrowser.Url = new Uri(filePath);
			return Task.Delay(0);
		}
	}
}
