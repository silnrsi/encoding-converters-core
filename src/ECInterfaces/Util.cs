// Copyright (c) 2011, SIL International. All Rights Reserved.
// <copyright from='2011' to='2011' company='SIL International'>
//  Copyright (c) 2011, SIL International. All Rights Reserved.
//  Distributable under the terms of either the Common Public License or the
//  GNU Lesser General Public License, as specified in the LICENSING.txt file.
// </copyright>
//
// 21-Jun-13 JDK  Moved debugging methods from EncConverter.cs to here.
//                Use /var/lib/encConverters unless otherwise specified by build system.

using System;
using System.Diagnostics;
using System.Linq;          // for Contains
using System.Text;          // for StringBuilder

namespace ECInterfaces
{
    public static class Util
    {
        private static string s_CommonAppDataFolder;

        // The following classes and methods will not be shown in debugging output.
        // Comment out or add as desired.
        private static readonly string[] DEBUG_EXCLUDE_CLASSES = {
            //"EncConverters",
        };
        private static readonly string[] DEBUG_EXCLUDE_METHODS = {
            "EncConverters.AddEx",
            "EncConverters.AddToCollection",
            "EncConverters.GetConversionEnginesSupported",
            //"EncConverters.GetRepositoryFileName",
            "EncConverters.GetEncodingFontDetails",
            "EncConverters.InitializeConverter",
            "EncConverters.InstantiateIEncConverter",

            "EncConverter.Initialize",
            "IcuTranslitEncConverter.Initialize",
            "PerlExpressionEncConverter.Initialize",
            "PyScriptEncConverter.Initialize",
            "TecEncConverter.Initialize",
        };

        /// <summary>
        /// Gets the path for storing common application data that might be shared between
        /// multiple applications and multiple users on the same machine.
        ///
        /// On Windows this returns Environment.SpecialFolder.CommonApplicationData
        /// (C:\ProgramData).
        /// On Linux, CommonApplicationData (/usr/share) is not writeable, so we
        /// return /var/lib/[appname] instead.
        /// </summary>
        private static string CommonApplicationData
        {
            get
            {
                if (s_CommonAppDataFolder == null)
                {
                    if (Util.IsUnix)
                    {
                        // COMMON_DATA_FW can be defined during compile time.
                        // This folder should match REGROOT in the main Makefile.in.
#if (COMMON_DATA_FW)
                        s_CommonAppDataFolder = "/var/lib/fieldworks";
#else
                        s_CommonAppDataFolder = "/var/lib/encConverters";
#endif
                        DebugWriteLine(typeof(Util).ToString(), "CommonAppDataFolder = " + s_CommonAppDataFolder);
                    }
                    else
                    {
                        s_CommonAppDataFolder =
                            Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData);
                    }
                }
                return s_CommonAppDataFolder;
            }
        }

        /// <summary>
        /// Gets a special folder, similar to Environment.GetFolderPath. The main
        /// difference is that this method works cross-platform and does some translations.
        /// </summary>
        public static string CommonAppDataPath()
        {
            return CommonApplicationData;
        }

        // Currently we are using the following routines rather than log4net
        // in order to avoid a dependency on another library.
        // It sure would be nice if C# came with something more powerful
        // than System.Diagnostics.TraceSwitch.

        private static void DebugWriteLine(StackFrame sf, string className, string strMsg)
        {
            // this makes it unbearably slow on Windows...
#if DEBUG
            if (!Util.IsUnix)
            {
                return;
            }
            System.Reflection.MethodBase mb = sf.GetMethod();
            string methodName = mb != null ? mb.Name : "";
            string fullMethodName = className + "." + methodName;
            if (DEBUG_EXCLUDE_CLASSES.Contains(className))
                return;
            if (DEBUG_EXCLUDE_METHODS.Contains(fullMethodName))
                return;
            string output = String.Format("{0,-40} {1}", fullMethodName + ":", strMsg);

            // Should we use System.Diagnostics.Debug or Console.Error here?
            // Using Debug is nice because it can be turned on or off with an environment variable.
            // However Debug.Flush() doesn't have any effect, which means that the messages can
            // get out of order relative to C++ output.

            //System.Diagnostics.Debug.WriteLine(output);
            Console.WriteLine(output);
            //Console.Error.WriteLine(output);
#endif
        }

        // Call this method from static methods.
        public static void DebugWriteLine(string className, string strMsg)
        {
#if DEBUG
            if (!Util.IsUnix)
            {
                return;
            }
            StackFrame sf = new StackFrame(1, true); // get frame of calling method
            DebugWriteLine(sf, className, strMsg);
#endif
        }

        // @param caller: "this" pointer of the calling class.
        public static void DebugWriteLine(Object caller, string strMsg)
        {
#if DEBUG
            if (!Util.IsUnix)
            {
                return;
            }
            StackFrame sf = new StackFrame(1, true); // get frame of calling method
            string className = caller.GetType().Name;
            DebugWriteLine(sf, className, strMsg);
#endif
        }

        // Get a string suitable for displaying.
        static public string getDisplayBytes(string desc, byte[] ba)
        {
#if DEBUG
            if (!Util.IsUnix)
            {
                return "";
            }
            StringBuilder builder = new StringBuilder();
            builder.Append(desc).Append(": ");
            for (int i = 0; i < ba.Length; i++)
            {
                builder.Append(ba[i].ToString("x2")).Append(" ");
            }
            return builder.ToString();
#else
            return "";
#endif
        }

        /// <summary>
        /// Test whether we're running on a Unix variant (such as Linux).
        /// </summary>
        static public bool IsUnix
        {
            get { return Environment.OSVersion.Platform == PlatformID.Unix; }
        }
    }
}
