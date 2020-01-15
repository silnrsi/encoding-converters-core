//#define LoadGeckoLibs
// 21-May-2013 JDK  Add link to help file in instructions.
// 24-Jun-2013 JDK  Loading Gecko libs may fail, so supply a macro to disable.

using System;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Windows.Forms;
#if LoadGeckoLibs
using Gecko;
#endif
using Microsoft.Win32;
using ECInterfaces;     // for Util

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
            GeckoFx,
            Instructions    // instructions to install browser libs
        }

        private WhichBrowser     _whichBrowser   { get; set; }
#if LoadGeckoLibs
        private GeckoWebBrowser  GeckoWebBrowser { get; set; }
#endif
        private WebBrowser       IeWebBrowser    { get; set; }
        private TableLayoutPanel LabelsPanel     { get; set; }

        public WebBrowserAdaptor()
        {
            Util.DebugWriteLine(this, "BEGIN");
            InitializeComponent();

            // on linux, only Gecko works (on Windows, either Gecko or IE will work, but using IE saves us from having to redistribute too much)
            //  so on Linux, prefer Gecko, but on Windows, prefer IE.
            if (ShouldUseGecko)
            {
                // try to initialize 
                // if GeckoFx was successfully initialized, then use it
#if LoadGeckoLibs
                if (GeckoFxInitializer.SetUpXulRunner())
#else
                if (false)
#endif
                {
                    Util.DebugWriteLine(this, "Using GeckoFx");
                    _whichBrowser = WhichBrowser.GeckoFx;
#if LoadGeckoLibs
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
#endif
                }
                else
                {
                    Util.DebugWriteLine(this, "Could not use GeckoFx");
                    _whichBrowser = WhichBrowser.Instructions;
                    LabelsPanel = new TableLayoutPanel
                                                 {
                                                   Dock        = DockStyle.Fill,
                                                   ColumnCount = 1,
                                                   RowCount    = 2,
                                                 };
                    this.Controls.Add(LabelsPanel);
                    LabelsPanel.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 30F));
                    LabelsPanel.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 70F));
                    LabelsPanel.Controls.Add(GeckoFxInitializer.InstructionsLinkLabel, 0, 0);
                }
            }
            else
            {
                Util.DebugWriteLine(this, "Using Internet Explorer");
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
            Util.DebugWriteLine(this, "END");
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
        private  static bool WindowsUserWantsToUseGecko
        {
            get
            {
                var regKeySecRoot = Registry.LocalMachine.OpenSubKey(EncConverters.SEC_ROOT_KEY);
                return (regKeySecRoot != null) &&
                       (regKeySecRoot.GetValue(EncConverters.CstrUseGeckoRegKey, "False") as string == "True");
            }
        }
        private bool IsGecko
        {
            get { return (_whichBrowser == WhichBrowser.GeckoFx); }
        }
        private bool IsInstructions
        {
            get { return (_whichBrowser == WhichBrowser.Instructions); }
        }

        internal void Navigate(string strXmlFilePath)
        {
            if (IsGecko || IsInstructions)
            {
                if (Path.GetExtension(strXmlFilePath) == ".mht")
                {
                    var strHtm = Path.Combine(Path.GetDirectoryName(strXmlFilePath),
                                              Path.GetFileNameWithoutExtension(strXmlFilePath) + ".htm");
                    if (File.Exists(strHtm))
                        strXmlFilePath = strHtm;
                    else
                    {
                        const string cstrFileNameToEditSaveAsHtml = "CantReadMhtFiles.htm";
                        strXmlFilePath = Path.Combine(Path.GetDirectoryName(strXmlFilePath),
                                                      cstrFileNameToEditSaveAsHtml);
                        System.Diagnostics.Debug.Assert(File.Exists(strXmlFilePath));
                    }
                }
                if (IsGecko)
                {
#if LoadGeckoLibs
                    GeckoWebBrowser.Navigate("file://" + strXmlFilePath);
#endif
                }
                else
                {
                    string cstrLinkPrefix = "Help file for this converter: " + Environment.NewLine;
                    LinkLabel labelHelpLink = new LinkLabel
                                                  {
                                                      Text = cstrLinkPrefix + strXmlFilePath,
                                                      Dock = DockStyle.Fill,
                                                  };
                    labelHelpLink.Links.Add(cstrLinkPrefix.Length, strXmlFilePath.Length, strXmlFilePath);
                    labelHelpLink.LinkClicked += (sender, args) =>
                                                     {
                                                         if (args.Link.LinkData != null)
                                                            Process.Start("file://" + args.Link.LinkData as string);
                                                     };
                    LabelsPanel.Controls.Add(labelHelpLink, 0, 1);
                }
            }
            else if (IeWebBrowser != null)
                IeWebBrowser.Url = new Uri(strXmlFilePath);
        }
    }
}
