using System;
using System.Drawing;
using System.Windows.Forms;
using Gecko;

namespace SilEncConverters40
{
    /// <summary>
    /// This class is an attempt at providing a very basic WebBrowser like control, but which can be used for GeckoFx (Mozilla) if it exists instead
    /// </summary>
    public partial class WebBrowserAdaptor : UserControl
    {
        public enum WhichBrowser
        {
            Undefined = 0,
            InternetExplorer,
            GeckoFx
        }

        private WhichBrowser _whichBrowser { get; set; }

        private GeckoWebBrowser GeckoWebBrowser { get; set; }
        private WebBrowser IeWebBrowser { get; set; }

        public WebBrowserAdaptor()
        {
            InitializeComponent();

            // on linux, only Gecko works (on Windows, either Gecko or IE will work, but using IE saves us from having to redistribute too much)
            //  so on Linux, prefer Gecko, but on Windows, prefer IE.
            if (ECNormalizeData.IsUnix)
            {
                // try to initialize GeckoFx 
                GeckoFxInitializer.SetUpXulRunner();

                // if that was successful, then stick with it
                if (GeckoFxInitializer.IsGeckoInitialized)
                {
                    _whichBrowser = WhichBrowser.GeckoFx;
                    GeckoWebBrowser = new GeckoWebBrowser
                    {
                        Dock = DockStyle.Fill,
                        Location = new Point(3, 3),
                        MinimumSize = new Size(20, 20),
                        Name = "geckoWebBrowser",
                        Size = new Size(596, 394),
                        TabIndex = 0
                    };

                    Controls.Add(GeckoWebBrowser);
                }
            }
            else
            {
                _whichBrowser = WhichBrowser.InternetExplorer;
                IeWebBrowser = new WebBrowser
                                   {
                                       Dock = DockStyle.Fill,
                                       Location = new Point(3, 3),
                                       MinimumSize = new Size(20, 20),
                                       Name = "ieWebBrowser",
                                       Size = new Size(596, 394),
                                       TabIndex = 0,
                                       Url = new Uri("", UriKind.Relative)
                                   };
                Controls.Add(IeWebBrowser);
            }
        }

        private bool IsGecko
        {
            get { return (_whichBrowser == WhichBrowser.GeckoFx); }
        }

        internal void Navigate(string strXmlFilePath)
        {
            if (IsGecko)
            {
                System.Diagnostics.Debug.Assert(strXmlFilePath.Substring(strXmlFilePath.Length - 3, 3) != "mht", "Oops, if you're going to use Mozilla to view the help file, then you need to use *.htm for the help file");
                GeckoWebBrowser.Navigate("file://" + strXmlFilePath);
            }
            else
                IeWebBrowser.Url = new Uri(strXmlFilePath);
            Application.DoEvents();
        }
    }
}
