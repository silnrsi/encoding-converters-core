using System;
using System.Diagnostics;
using System.IO;

using ECInterfaces;                     // for Util.DebugWriteLine

namespace SilEncConverters40
{
    /// <summary>
    /// The proactive half of the generic subprocess fallback (see Handover.md, "Generic subprocess
    /// fallback for host-process conflicts"): checks whether a named native DLL a converter depends on
    /// (see EncConverter.RiskyNativeDependencies) is already loaded into the current process from
    /// somewhere other than that converter's own directory -- Windows' DLL search order reuses an
    /// already-loaded module with the same filename regardless of directory, so a foreign copy loaded
    /// first (e.g. Word's own WebView2Loader.dll) means our own bundled copy never gets a chance to
    /// load, and initializing the converter in-process risks a native crash rather than a catchable
    /// .NET exception.
    /// This scan is deliberately cheap (a single pass over already-loaded modules, no I/O of its own)
    /// so it's safe to run on every instantiation, in every process -- see EncConverters.AddEx, which
    /// runs it before ever calling a risky converter's Initialize().
    /// </summary>
    internal static class NativeModuleConflictDetector
    {
        /// <summary>
        /// Returns true if a module named <paramref name="moduleFileName"/> (e.g. "WebView2Loader.dll",
        /// compared case-insensitively, ignoring any path) is already loaded into the current process
        /// from a directory other than <paramref name="ownDirectory"/>. Returns false both when the
        /// module isn't loaded at all (nothing to conflict with -- safe to load our own copy) and when
        /// it's already loaded from exactly <paramref name="ownDirectory"/> (a previous instantiation in
        /// this same process already loaded the correct copy -- also safe).
        /// </summary>
        public static bool IsLoadedFromElsewhere(string moduleFileName, string ownDirectory)
        {
            if (String.IsNullOrEmpty(moduleFileName))
                return false;

            var normalizedOwnDirectory = String.IsNullOrEmpty(ownDirectory)
                ? null
                : ownDirectory.TrimEnd(Path.DirectorySeparatorChar, Path.AltDirectorySeparatorChar);

            foreach (ProcessModule module in Process.GetCurrentProcess().Modules)
            {
                string modulePath;
                try
                {
                    // ProcessModule.FileName can throw (e.g. Win32Exception) for some system modules on
                    // some OS/permission combinations -- never let a single unreadable module entry
                    // abort the whole scan.
                    modulePath = module.FileName;
                }
                catch
                {
                    continue;
                }

                if (String.IsNullOrEmpty(modulePath)
                    || !String.Equals(Path.GetFileName(modulePath), moduleFileName, StringComparison.OrdinalIgnoreCase))
                {
                    continue;
                }

                var moduleDirectory = Path.GetDirectoryName(modulePath);
                var isElsewhere = normalizedOwnDirectory == null
                    || !String.Equals(moduleDirectory, normalizedOwnDirectory, StringComparison.OrdinalIgnoreCase);

                Util.DebugWriteLine(typeof(NativeModuleConflictDetector).Name,
                    $"'{moduleFileName}' already loaded from '{modulePath}' " +
                    $"({(isElsewhere ? "elsewhere -- conflict" : "our own directory -- fine")}).");

                return isElsewhere;
            }

            // not loaded at all -- nothing resident to lose the DLL-search-order race against.
            return false;
        }
    }
}
