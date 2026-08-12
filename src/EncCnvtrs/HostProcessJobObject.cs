using System;
using System.Diagnostics;
using System.Runtime.InteropServices;

using ECInterfaces;                     // for Util.DebugWriteLine

namespace SilEncConverters40
{
    /// <summary>
    /// Ties every EncConverterHostExe.exe child this process launches (via
    /// PersistentEncConverterHostSession) to this process's own lifetime, using a Windows Job Object
    /// with JOB_OBJECT_LIMIT_KILL_ON_JOB_CLOSE -- defense-in-depth on top of the two mechanisms already
    /// in place: the child's own idle-timeout self-termination (Program.cs), and the graceful Shutdown
    /// message PersistentEncConverterHostSession.Dispose() sends. Neither of those helps if the *host*
    /// process (Word, Paratext, whichever embeds SilEncConverters40.dll) terminates uncleanly -- a crash
    /// or a forced kill runs no managed code at all, so no Dispose()/finalizer/AppDomain.ProcessExit
    /// handler can be relied on to send that Shutdown message, and the child would otherwise sit around
    /// for up to its full idle timeout as an orphan.
    ///
    /// A Job Object sidesteps that because it isn't tied to any .NET object's lifetime at all -- it's a
    /// kernel object tied to the *process handle table*. The sequence:
    ///   1. CreateJobObject() makes the job; SetInformationJobObject() sets KILL_ON_JOB_CLOSE on it.
    ///   2. AssignProcessToJobObject() puts the freshly-launched child into that job.
    ///   3. The job handle is kept open for the rest of this process's life (never explicitly closed).
    ///   4. When this process terminates for ANY reason -- clean exit, crash, TerminateProcess -- Windows
    ///      itself tears down its entire handle table, closing the job handle as a side effect.
    ///   5. Because of KILL_ON_JOB_CLOSE, the instant that (last) handle closes, Windows automatically
    ///      terminates every process still assigned to the job -- no managed code has to run for this.
    ///
    /// One job object is created lazily and shared across every session in this process (Windows allows
    /// multiple processes in the same job) -- there's no reason for each PersistentEncConverterHostSession
    /// to have its own.
    /// </summary>
    internal static class HostProcessJobObject
    {
        private const uint JOB_OBJECT_LIMIT_KILL_ON_JOB_CLOSE = 0x2000;
        private const int JobObjectExtendedLimitInformation = 9;

        [StructLayout(LayoutKind.Sequential)]
        private struct JOBOBJECT_BASIC_LIMIT_INFORMATION
        {
            public long PerProcessUserTimeLimit;
            public long PerJobUserTimeLimit;
            public uint LimitFlags;
            public UIntPtr MinimumWorkingSetSize;
            public UIntPtr MaximumWorkingSetSize;
            public uint ActiveProcessLimit;
            public UIntPtr Affinity;
            public uint PriorityClass;
            public uint SchedulingClass;
        }

        [StructLayout(LayoutKind.Sequential)]
        private struct IO_COUNTERS
        {
            public ulong ReadOperationCount;
            public ulong WriteOperationCount;
            public ulong OtherOperationCount;
            public ulong ReadTransferCount;
            public ulong WriteTransferCount;
            public ulong OtherTransferCount;
        }

        [StructLayout(LayoutKind.Sequential)]
        private struct JOBOBJECT_EXTENDED_LIMIT_INFORMATION
        {
            public JOBOBJECT_BASIC_LIMIT_INFORMATION BasicLimitInformation;
            public IO_COUNTERS IoInfo;
            public UIntPtr ProcessMemoryLimit;
            public UIntPtr JobMemoryLimit;
            public UIntPtr PeakProcessMemoryUsed;
            public UIntPtr PeakJobMemoryUsed;
        }

        [DllImport("kernel32.dll", CharSet = CharSet.Unicode, SetLastError = true)]
        private static extern IntPtr CreateJobObject(IntPtr lpJobAttributes, string lpName);

        [DllImport("kernel32.dll", SetLastError = true)]
        private static extern bool SetInformationJobObject(IntPtr hJob, int infoType, ref JOBOBJECT_EXTENDED_LIMIT_INFORMATION lpJobObjectInfo, uint cbJobObjectInfoLength);

        [DllImport("kernel32.dll", SetLastError = true)]
        private static extern bool AssignProcessToJobObject(IntPtr hJob, IntPtr hProcess);

        // deliberately not disposed/closed anywhere: kept open for the life of this process on purpose
        // (see class doc above) -- lazily created, thread-safe via the null-check-then-lock pattern
        // (cheap enough here; this only runs once per process, not once per session).
        private static IntPtr s_jobHandle = IntPtr.Zero;
        private static readonly object s_lock = new object();

        /// <summary>
        /// Assigns <paramref name="childProcess"/> to this process's shared kill-on-close job, creating
        /// the job on first use. Best-effort: any failure (e.g. a sandboxed/restricted environment that
        /// denies CreateJobObject) is swallowed and logged rather than thrown -- this is defense-in-depth
        /// on top of the idle-timeout and graceful-Shutdown mechanisms, not the only line of defense, so
        /// losing it shouldn't take down the subprocess fallback feature entirely.
        /// </summary>
        public static void AssignToKillOnCloseJob(Process childProcess)
        {
            try
            {
                var jobHandle = GetOrCreateJobHandle();
                if (jobHandle == IntPtr.Zero)
                    return;

                if (!AssignProcessToJobObject(jobHandle, childProcess.Handle))
                {
                    Util.DebugWriteLine(typeof(HostProcessJobObject).Name,
                        $"AssignProcessToJobObject failed (Win32 error {Marshal.GetLastWin32Error()}) -- "
                        + "the child won't be tied to this process's lifetime via the job object; the "
                        + "idle-timeout and graceful-Shutdown mechanisms still apply.");
                }
            }
            catch (Exception ex)
            {
                Util.DebugWriteLine(typeof(HostProcessJobObject).Name, $"AssignToKillOnCloseJob failed: {ex.Message}");
            }
        }

        private static IntPtr GetOrCreateJobHandle()
        {
            if (s_jobHandle != IntPtr.Zero)
                return s_jobHandle;

            lock (s_lock)
            {
                if (s_jobHandle != IntPtr.Zero)
                    return s_jobHandle;

                var handle = CreateJobObject(IntPtr.Zero, null);
                if (handle == IntPtr.Zero)
                {
                    Util.DebugWriteLine(typeof(HostProcessJobObject).Name,
                        $"CreateJobObject failed (Win32 error {Marshal.GetLastWin32Error()}).");
                    return IntPtr.Zero;
                }

                var info = new JOBOBJECT_EXTENDED_LIMIT_INFORMATION
                {
                    BasicLimitInformation = new JOBOBJECT_BASIC_LIMIT_INFORMATION
                    {
                        LimitFlags = JOB_OBJECT_LIMIT_KILL_ON_JOB_CLOSE,
                    },
                };

                if (!SetInformationJobObject(handle, JobObjectExtendedLimitInformation, ref info,
                    (uint)Marshal.SizeOf(typeof(JOBOBJECT_EXTENDED_LIMIT_INFORMATION))))
                {
                    Util.DebugWriteLine(typeof(HostProcessJobObject).Name,
                        $"SetInformationJobObject failed (Win32 error {Marshal.GetLastWin32Error()}) -- "
                        + "not using this job (it wouldn't actually kill children on close).");
                    return IntPtr.Zero;
                }

                s_jobHandle = handle;
                return s_jobHandle;
            }
        }
    }
}
