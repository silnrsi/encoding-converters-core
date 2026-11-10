using Microsoft.Win32;
using Microsoft.Win32.SafeHandles;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Windows.Forms;

namespace SpellingFixer30
{
    /// <summary>
    /// The result of trying to link a CscProject folder to a Paratext project's 'shared' folder
    /// </summary>
    internal enum ParatextLinkResult
    {
        /// <summary>the user cancelled or something failed (an error message has already been shown)</summary>
        NotLinked,

        /// <summary>linked, and the project's data is what was already in this project's local folder (or there was none)</summary>
        LinkedUsingLocalData,

        /// <summary>linked, and the project's data is what was already in the Paratext 'shared' folder, so the caller should reload the project</summary>
        LinkedUsingSharedData
    }

    /// <summary>
    /// A CscProject found in some Paratext project's 'shared\ConsistentSpellingFixer' folder
    /// </summary>
    internal class SharedCscProject
    {
        public string CscProjectName { get; set; }
        public string ParatextProjectName { get; set; }
        public string FolderPath { get; set; }

        public string DisplayName => $"{CscProjectName} (shared in Paratext project '{ParatextProjectName}')";

        public override string ToString() => DisplayName;
    }

    /// <summary>
    /// Supports synchronizing a CscProject's data via a Paratext project's 'shared' folder (which Paratext
    /// Send/Receive syncs between all the users of that Paratext project). The CscProject's normal folder
    /// (e.g. C:\ProgramData\SIL\ConsistentSpellingFixer\Projects\Dogri) is replaced by an NTFS junction that
    /// points to <My Paratext Projects>\<PT project>\shared\ConsistentSpellingFixer\<CscProject name>, so the
    /// rest of CscProject doesn't need to know that its files actually live somewhere else.
    /// Junctions (unlike symbolic links) don't require any special privilege to create.
    /// </summary>
    internal static class ParatextProjectLink
    {
        public const string DefaultPathToMyParatextProjects = @"C:\My Paratext 9 Projects";
        public const string SharedFolderName = "shared";
        public const string SharedCscFolderName = "ConsistentSpellingFixer";

        private const string RegistryParatextKey = @"SOFTWARE\Paratext\8";
        private const string RegistryParatextSettingsDirectory = "Settings_Directory";
        private const string ParatextProjectSettingsFile = "Settings.xml";

        private static string _myParatextProjectsFolder;

        #region Paratext folder discovery

        /// <summary>
        /// Returns the 'My Paratext Projects' folder: from the registry where Paratext records it, or the
        /// default location, or (if allowBrowse) by asking the user. Returns null if it can't be found.
        /// </summary>
        public static string GetMyParatextProjectsFolder(bool allowBrowse)
        {
            if (!String.IsNullOrEmpty(_myParatextProjectsFolder) && Directory.Exists(_myParatextProjectsFolder))
                return _myParatextProjectsFolder;

            var folder = GetParatextSettingsDirectoryFromRegistry();
            if (String.IsNullOrEmpty(folder) || !Directory.Exists(folder))
                folder = Directory.Exists(DefaultPathToMyParatextProjects) ? DefaultPathToMyParatextProjects : null;

            if ((folder == null) && allowBrowse)
            {
                using (var dlg = new FolderBrowserDialog
                {
                    Description = $"Browse for your 'My Paratext 9 Projects' folder (default: '{DefaultPathToMyParatextProjects}')"
                })
                {
                    if (dlg.ShowDialog() == DialogResult.OK)
                        folder = dlg.SelectedPath;
                }
            }

            if (folder != null)
                _myParatextProjectsFolder = folder.TrimEnd('\\');

            return _myParatextProjectsFolder;
        }

        private static string GetParatextSettingsDirectoryFromRegistry()
        {
            // Paratext 9 (which still uses the 'Paratext\8' key) is a 32-bit app on some machines and 64-bit on
            //  others, so look in both registry views.
            foreach (var view in new[] { RegistryView.Registry32, RegistryView.Registry64 })
            {
                try
                {
                    using (var baseKey = RegistryKey.OpenBaseKey(RegistryHive.LocalMachine, view))
                    using (var key = baseKey.OpenSubKey(RegistryParatextKey))
                    {
                        var value = key?.GetValue(RegistryParatextSettingsDirectory) as string;
                        if (!String.IsNullOrEmpty(value))
                            return value;
                    }
                }
                catch
                {
                    // try the next view
                }
            }
            return null;
        }

        /// <summary>
        /// Returns the (short) names of the Paratext projects in the given 'My Paratext Projects' folder
        /// </summary>
        public static List<string> GetParatextProjectNames(string myParatextProjectsFolder)
        {
            try
            {
                return Directory.GetDirectories(myParatextProjectsFolder)
                                .Where(p => File.Exists(Path.Combine(p, ParatextProjectSettingsFile)))
                                .Select(Path.GetFileName)
                                .OrderBy(n => n, StringComparer.OrdinalIgnoreCase)
                                .ToList();
            }
            catch
            {
                return new List<string>();
            }
        }

        /// <summary>
        /// Asks the user which Paratext project to synchronize the given CscProject with. Returns null if cancelled.
        /// </summary>
        public static string QueryParatextProjectName(string cscProjectName)
        {
            var myParatextProjectsFolder = GetMyParatextProjectsFolder(allowBrowse: true);
            if (myParatextProjectsFolder == null)
                return null;

            var projectNames = GetParatextProjectNames(myParatextProjectsFolder);
            if (!projectNames.Any())
            {
                MessageBox.Show($"There are no Paratext projects in the '{myParatextProjectsFolder}' folder.",
                                CscProject.ApplicationCaption);
                return null;
            }

            using (var dlg = new ParatextProjectListForm(projectNames, $"Choose the Paratext project to synchronize '{cscProjectName}' with"))
            {
                return (dlg.ShowDialog() == DialogResult.OK) ? dlg.SelectedDisplayName : null;
            }
        }

        /// <summary>
        /// Returns the folder where the given CscProject's data goes in the given Paratext project
        /// (e.g. C:\My Paratext 9 Projects\Dog\shared\ConsistentSpellingFixer\Dogri)
        /// </summary>
        public static string GetSharedCscProjectFolder(string myParatextProjectsFolder, string paratextProjectName, string cscProjectName)
        {
            return Path.Combine(myParatextProjectsFolder, paratextProjectName, SharedFolderName, SharedCscFolderName, cscProjectName);
        }

        /// <summary>
        /// Returns all the CscProjects found in any Paratext project's 'shared\ConsistentSpellingFixer' folder.
        /// Doesn't prompt the user (returns an empty list if the Paratext projects folder can't be found).
        /// </summary>
        public static List<SharedCscProject> FindSharedCscProjects()
        {
            var list = new List<SharedCscProject>();
            var myParatextProjectsFolder = GetMyParatextProjectsFolder(allowBrowse: false);
            if (myParatextProjectsFolder == null)
                return list;

            foreach (var paratextProjectName in GetParatextProjectNames(myParatextProjectsFolder))
            {
                var sharedCscRoot = Path.Combine(myParatextProjectsFolder, paratextProjectName, SharedFolderName, SharedCscFolderName);
                if (!Directory.Exists(sharedCscRoot))
                    continue;

                try
                {
                    foreach (var folder in Directory.GetDirectories(sharedCscRoot))
                    {
                        var cscProjectName = Path.GetFileName(folder);
                        if (File.Exists(Path.Combine(folder, $"{cscProjectName}.{CscProject.ProjectFileExt}")))
                        {
                            list.Add(new SharedCscProject
                            {
                                CscProjectName = cscProjectName,
                                ParatextProjectName = paratextProjectName,
                                FolderPath = folder
                            });
                        }
                    }
                }
                catch
                {
                    // skip any folder we can't read
                }
            }
            return list;
        }

        #endregion

        #region Link/Unlink

        /// <summary>
        /// The local folder for the given CscProject (e.g. C:\ProgramData\SIL\ConsistentSpellingFixer\Projects\Dogri),
        /// which is either a real folder or a junction to a Paratext project's 'shared' folder
        /// </summary>
        public static string GetLocalProjectFolder(string cscProjectName)
        {
            return Path.Combine(CscProject.GetProjectsFolder, cscProjectName);
        }

        /// <summary>
        /// Returns true if the CscProject's local folder is a junction (i.e. is synchronized via a Paratext project)
        /// </summary>
        public static bool IsLinked(string cscProjectName)
        {
            return IsJunction(GetLocalProjectFolder(cscProjectName));
        }

        /// <summary>
        /// Returns the name of the Paratext project that the CscProject's local folder is linked to (or null)
        /// </summary>
        public static string GetLinkedParatextProjectName(string cscProjectName)
        {
            var target = GetJunctionTarget(GetLocalProjectFolder(cscProjectName));
            if (target == null)
                return null;

            // target = <My Paratext Projects>\<PT project>\shared\ConsistentSpellingFixer\<cscProjectName>
            var sharedCscRoot = Path.GetDirectoryName(target.TrimEnd('\\'));
            var sharedFolder = Path.GetDirectoryName(sharedCscRoot);
            var paratextProjectFolder = Path.GetDirectoryName(sharedFolder);
            return Path.GetFileName(paratextProjectFolder);
        }

        /// <summary>
        /// Moves the CscProject's local data into the given Paratext project's 'shared' folder (dealing with the
        /// case where both already have data) and replaces the local folder with a junction to it.
        /// </summary>
        public static ParatextLinkResult LinkToParatext(string cscProjectName, string paratextProjectName)
        {
            try
            {
                var myParatextProjectsFolder = GetMyParatextProjectsFolder(allowBrowse: true);
                if (myParatextProjectsFolder == null)
                    return ParatextLinkResult.NotLinked;

                var localFolder = GetLocalProjectFolder(cscProjectName);
                var sharedFolder = GetSharedCscProjectFolder(myParatextProjectsFolder, paratextProjectName, cscProjectName);

                // if it's already a junction, then the 'local' data is wherever it currently points to. In that
                //  case, we copy (rather than move) it, since other users may still be using it there.
                string sourceFolder = localFolder;
                bool sourceIsJunction = IsJunction(localFolder);
                if (sourceIsJunction)
                {
                    sourceFolder = GetJunctionTarget(localFolder);
                    if (PathsAreEqual(sourceFolder, sharedFolder))
                        return ParatextLinkResult.LinkedUsingLocalData;  // already linked there
                }

                var sourceHasData = HasFiles(sourceFolder);
                var sharedHasData = HasFiles(sharedFolder);
                var result = ParatextLinkResult.LinkedUsingLocalData;

                if (sourceHasData && sharedHasData)
                {
                    var res = MessageBox.Show(String.Format("Both this computer and the Paratext project '{1}' already have data for the '{0}' project:{2}{2}" +
                                                            "    On this computer: {3}{2}    In Paratext project '{1}': {4}{2}{2}" +
                                                            "Click 'Yes' to use the version in the Paratext project, or 'No' to use the version on this computer " +
                                                            "(which will replace the version in the Paratext project for all users after the next Send/Receive). " +
                                                            "Either way, the version not used will be backed up in the '{5}' folder.",
                                                            cscProjectName, paratextProjectName, Environment.NewLine,
                                                            DescribeProjectData(sourceFolder, cscProjectName),
                                                            DescribeProjectData(sharedFolder, cscProjectName),
                                                            Path.Combine(CscProject.ConsistentSpellingFixerDefaultRoot, "Backups")),
                                              CscProject.ApplicationCaption, MessageBoxButtons.YesNoCancel, MessageBoxIcon.Question);
                    if (res == DialogResult.Cancel)
                        return ParatextLinkResult.NotLinked;

                    if (res == DialogResult.Yes)
                    {
                        // use the shared version; back up the local one
                        CopyDirectory(sourceFolder, GetBackupFolder(cscProjectName, "local"));
                        result = ParatextLinkResult.LinkedUsingSharedData;
                    }
                    else
                    {
                        // use the local version; back up the shared one and replace it
                        CopyDirectory(sharedFolder, GetBackupFolder(cscProjectName, paratextProjectName));
                        DeleteDirectoryContents(sharedFolder);
                        CopyDirectory(sourceFolder, sharedFolder);
                    }
                }
                else if (sourceHasData)
                {
                    CopyDirectory(sourceFolder, sharedFolder);
                }
                else
                {
                    Directory.CreateDirectory(sharedFolder);
                    if (sharedHasData)
                        result = ParatextLinkResult.LinkedUsingSharedData;
                }

                // now replace the local folder with a junction to the shared one
                if (sourceIsJunction)
                    RemoveJunction(localFolder);
                else if (Directory.Exists(localFolder))
                    Directory.Delete(localFolder, true);    // it's a real folder whose files have been copied to sharedFolder

                CreateJunction(localFolder, sharedFolder);
                return result;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Unable to synchronize the '{cscProjectName}' project with the Paratext project '{paratextProjectName}': {ex.Message}",
                                CscProject.ApplicationCaption);
                return ParatextLinkResult.NotLinked;
            }
        }

        /// <summary>
        /// Removes the junction to the Paratext project's 'shared' folder and replaces it with a local copy of the
        /// data. The data in the Paratext project is left as is (since other users may still be using it).
        /// </summary>
        public static bool UnlinkFromParatext(string cscProjectName)
        {
            try
            {
                var localFolder = GetLocalProjectFolder(cscProjectName);
                if (!IsJunction(localFolder))
                    return true;

                var sharedFolder = GetJunctionTarget(localFolder);
                RemoveJunction(localFolder);
                Directory.CreateDirectory(localFolder);
                if (Directory.Exists(sharedFolder))
                    CopyDirectory(sharedFolder, localFolder);
                return true;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Unable to stop synchronizing the '{cscProjectName}' project with Paratext: {ex.Message}",
                                CscProject.ApplicationCaption);
                return false;
            }
        }

        private static string DescribeProjectData(string folder, string cscProjectName)
        {
            var files = Directory.GetFiles(folder, "*.*", SearchOption.AllDirectories)
                                 .Select(f => new FileInfo(f))
                                 .ToList();
            var totalBytes = files.Sum(f => f.Length);
            var lastModified = files.Max(f => f.LastWriteTime);
            var csf = new FileInfo(Path.Combine(folder, $"{cscProjectName}.{CscProject.ProjectFileExt}"));
            var csfInfo = csf.Exists ? $", project file saved {csf.LastWriteTime:g}" : ", no project file";
            return $"{files.Count} files, {totalBytes / 1024.0:N1} KB, last changed {lastModified:g}{csfInfo}";
        }

        private static string GetBackupFolder(string cscProjectName, string sourceDescription)
        {
            // keep backups locally (i.e. not in the 'shared' folder, so they don't get synchronized), and not in
            //  the Projects folder either (or they'd show up as projects)
            return Path.Combine(CscProject.ConsistentSpellingFixerDefaultRoot, "Backups",
                                $"{cscProjectName} ({sourceDescription}) {DateTime.Now:yyyy-MM-dd HHmmss}");
        }

        #endregion

        #region File system helpers

        private static bool PathsAreEqual(string path1, string path2)
        {
            if ((path1 == null) || (path2 == null))
                return false;
            return String.Equals(Path.GetFullPath(path1).TrimEnd('\\'), Path.GetFullPath(path2).TrimEnd('\\'),
                                 StringComparison.OrdinalIgnoreCase);
        }

        private static bool HasFiles(string folder)
        {
            return !String.IsNullOrEmpty(folder) && Directory.Exists(folder) &&
                   Directory.EnumerateFiles(folder, "*.*", SearchOption.AllDirectories).Any();
        }

        /// <summary>
        /// Copies the files (and sub-folders) of one folder to another (which may be on a different volume),
        /// overwriting any existing files of the same name
        /// </summary>
        private static void CopyDirectory(string sourceFolder, string targetFolder)
        {
            Directory.CreateDirectory(targetFolder);
            foreach (var file in Directory.GetFiles(sourceFolder))
            {
                var targetFile = Path.Combine(targetFolder, Path.GetFileName(file));
                if (File.Exists(targetFile))
                    File.SetAttributes(targetFile, FileAttributes.Normal);
                File.Copy(file, targetFile, true);
            }
            foreach (var subFolder in Directory.GetDirectories(sourceFolder))
                CopyDirectory(subFolder, Path.Combine(targetFolder, Path.GetFileName(subFolder)));
        }

        private static void DeleteDirectoryContents(string folder)
        {
            foreach (var file in Directory.GetFiles(folder))
            {
                File.SetAttributes(file, FileAttributes.Normal);
                File.Delete(file);
            }
            foreach (var subFolder in Directory.GetDirectories(folder))
                Directory.Delete(subFolder, true);
        }

        #endregion

        #region Junctions

        // see https://learn.microsoft.com/en-us/windows-hardware/drivers/ddi/ntifs/ns-ntifs-_reparse_data_buffer
        private const uint IO_REPARSE_TAG_MOUNT_POINT = 0xA0000003;
        private const uint FSCTL_SET_REPARSE_POINT = 0x000900A4;
        private const uint FSCTL_GET_REPARSE_POINT = 0x000900A8;
        private const uint GENERIC_READ = 0x80000000;
        private const uint GENERIC_WRITE = 0x40000000;
        private const uint FILE_SHARE_READ_WRITE_DELETE = 0x00000007;
        private const uint OPEN_EXISTING = 3;
        private const uint FILE_FLAG_BACKUP_SEMANTICS = 0x02000000;
        private const uint FILE_FLAG_OPEN_REPARSE_POINT = 0x00200000;
        private const string NonInterpretedPathPrefix = @"\??\";
        private const int MaximumReparseDataBufferSize = 16 * 1024;

        [DllImport("kernel32.dll", CharSet = CharSet.Unicode, SetLastError = true)]
        private static extern SafeFileHandle CreateFile(string lpFileName, uint dwDesiredAccess, uint dwShareMode,
            IntPtr lpSecurityAttributes, uint dwCreationDisposition, uint dwFlagsAndAttributes, IntPtr hTemplateFile);

        [DllImport("kernel32.dll", SetLastError = true)]
        private static extern bool DeviceIoControl(SafeFileHandle hDevice, uint dwIoControlCode,
            byte[] lpInBuffer, int nInBufferSize, byte[] lpOutBuffer, int nOutBufferSize,
            out int lpBytesReturned, IntPtr lpOverlapped);

        /// <summary>
        /// Returns true if the given path is a junction (or other reparse point) folder
        /// </summary>
        public static bool IsJunction(string path)
        {
            try
            {
                if (String.IsNullOrEmpty(path))
                    return false;
                var attributes = File.GetAttributes(path);
                return attributes.HasFlag(FileAttributes.Directory) && attributes.HasFlag(FileAttributes.ReparsePoint);
            }
            catch
            {
                return false;   // doesn't exist
            }
        }

        /// <summary>
        /// Creates a junction at junctionPath (which must not exist, or be an empty folder) that points to targetFolder
        /// </summary>
        public static void CreateJunction(string junctionPath, string targetFolder)
        {
            targetFolder = Path.GetFullPath(targetFolder).TrimEnd('\\');
            if (!Directory.Exists(targetFolder))
                throw new DirectoryNotFoundException($"The folder '{targetFolder}' doesn't exist");

            Directory.CreateDirectory(junctionPath);
            try
            {
                var substituteName = Encoding.Unicode.GetBytes(NonInterpretedPathPrefix + targetFolder);
                var printName = Encoding.Unicode.GetBytes(targetFolder);

                // PathBuffer = substituteName + null + printName + null
                var pathBufferLength = substituteName.Length + 2 + printName.Length + 2;
                var buffer = new byte[8 + 8 + pathBufferLength];
                using (var writer = new BinaryWriter(new MemoryStream(buffer)))
                {
                    writer.Write(IO_REPARSE_TAG_MOUNT_POINT);                   // ReparseTag
                    writer.Write((ushort)(8 + pathBufferLength));               // ReparseDataLength
                    writer.Write((ushort)0);                                    // Reserved
                    writer.Write((ushort)0);                                    // SubstituteNameOffset
                    writer.Write((ushort)substituteName.Length);                // SubstituteNameLength
                    writer.Write((ushort)(substituteName.Length + 2));          // PrintNameOffset
                    writer.Write((ushort)printName.Length);                     // PrintNameLength
                    writer.Write(substituteName);
                    writer.Write((ushort)0);
                    writer.Write(printName);
                    writer.Write((ushort)0);
                }

                using (var handle = OpenReparsePoint(junctionPath, GENERIC_WRITE))
                {
                    if (!DeviceIoControl(handle, FSCTL_SET_REPARSE_POINT, buffer, buffer.Length, null, 0, out _, IntPtr.Zero))
                        throw new Win32Exception(Marshal.GetLastWin32Error());
                }
            }
            catch
            {
                // don't leave an empty (non-junction) folder behind
                try { Directory.Delete(junctionPath); } catch { }
                throw;
            }
        }

        /// <summary>
        /// Returns the folder that the junction at junctionPath points to (or null if it isn't a junction)
        /// </summary>
        public static string GetJunctionTarget(string junctionPath)
        {
            if (!IsJunction(junctionPath))
                return null;

            using (var handle = OpenReparsePoint(junctionPath, GENERIC_READ))
            {
                var buffer = new byte[MaximumReparseDataBufferSize];
                if (!DeviceIoControl(handle, FSCTL_GET_REPARSE_POINT, null, 0, buffer, buffer.Length, out _, IntPtr.Zero))
                    throw new Win32Exception(Marshal.GetLastWin32Error());

                var tag = BitConverter.ToUInt32(buffer, 0);
                if (tag != IO_REPARSE_TAG_MOUNT_POINT)
                    return null;    // e.g. a symbolic link (which we don't create)

                var substituteNameOffset = BitConverter.ToUInt16(buffer, 8);
                var substituteNameLength = BitConverter.ToUInt16(buffer, 10);
                var target = Encoding.Unicode.GetString(buffer, 16 + substituteNameOffset, substituteNameLength);
                if (target.StartsWith(NonInterpretedPathPrefix))
                    target = target.Substring(NonInterpretedPathPrefix.Length);
                return target;
            }
        }

        /// <summary>
        /// Removes the junction itself (NOT the files in the folder it points to)
        /// </summary>
        public static void RemoveJunction(string junctionPath)
        {
            if (IsJunction(junctionPath))
                Directory.Delete(junctionPath, false);  // non-recursive: removes only the reparse point
        }

        private static SafeFileHandle OpenReparsePoint(string path, uint access)
        {
            var handle = CreateFile(path, access, FILE_SHARE_READ_WRITE_DELETE, IntPtr.Zero, OPEN_EXISTING,
                                    FILE_FLAG_BACKUP_SEMANTICS | FILE_FLAG_OPEN_REPARSE_POINT, IntPtr.Zero);
            if (handle.IsInvalid)
                throw new Win32Exception(Marshal.GetLastWin32Error());
            return handle;
        }

        #endregion
    }
}
