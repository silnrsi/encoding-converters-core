// 21-May-2013 JDK  Check for xulrunner folder in DirectoryOfApplication method.

using System;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Windows.Forms;
using ECInterfaces;     // for Util

namespace SilEncConverters40
{
    public class GeckoFxInitializer
    {
#if _MSC_VER
        [DllImport("kernel32.dll", CharSet = CharSet.Unicode, SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        static extern bool SetDllDirectory(string lpPathName);
#endif
        private static string className = typeof(GeckoFxInitializer).Name;

        public static bool SetUpXulRunner()
        {
            Util.DebugWriteLine(className, "BEGIN");
	        if (Gecko.Xpcom.IsInitialized)
		        return true;

            string xulRunnerPath;
            if (!XulRunnerDirectoryOfApplicationOrLib(out xulRunnerPath))
                return false;
            Util.DebugWriteLine(className, "xulRunnerPath=" + xulRunnerPath);

#if _MSC_VER
            //Review: an early tester found that wrong xpcom was being loaded. The following solution is from http://www.geckofx.org/viewtopic.php?id=74&action=new
            SetDllDirectory(xulRunnerPath);
#endif

            Gecko.Xpcom.Initialize(xulRunnerPath);
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
            string path = DirectoryOfTheApplicationExecutable;
            xulRunnerPath = Path.Combine(path, "xulrunner");
            if (Directory.Exists(xulRunnerPath))
                return true;

#if __MonoCS__
            xulRunnerPath = "/usr/lib/xulrunner-geckofx";
            if (Directory.Exists(xulRunnerPath))
                return true;
#endif

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

        public static LinkLabel InstructionsLinkLabel
        {
            get
            {
#if __MonoCS__
                return new LinkLabel
                    {
                        Text = "To use Mozilla to display the help file, install the firefox-geckofx package.",
                        Dock = DockStyle.Fill
                    };
#else
                const string cstrLinkPrefix = "To use Mozilla to display the help file, download xulRunner from ";
                const string cstrXulRunnerLink = "http://ftp.mozilla.org/pub/mozilla.org/xulrunner/releases/14.0.1/runtimes";
                var labelInstructions = new LinkLabel
                    {
                        Text = " and put the xulrunner folder as a subfolder in the ";
                               cstrLinkPrefix + cstrXulRunnerLink + cstrFolderPrefix +
                               DirectoryOfTheApplicationExecutable +
                               String.Format(@" folder. Otherwise, change the registry key 'HKLM\{0}\{1}' to 'False'",
                               EncConverters.SEC_ROOT_KEY, EncConverters.CstrUseGeckoRegKey);
                        Dock = DockStyle.Fill
                    };
                labelInstructions.Links.Add(cstrLinkPrefix.Length, cstrXulRunnerLink.Length, cstrXulRunnerLink);
                labelInstructions.Links.Add(labelInstructions.Text.IndexOf(DirectoryOfTheApplicationExecutable),
                                            DirectoryOfTheApplicationExecutable.Length,
                                            DirectoryOfTheApplicationExecutable);
                labelInstructions.LinkClicked += (sender, args) =>
                                                     {
                                                         if (args.Link.LinkData != null);
                                                            Process.Start(args.Link.LinkData as string);
                                                     };
                return labelInstructions;
#endif
            }
        }
    }  
}
