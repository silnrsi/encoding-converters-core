using System;
using System.Runtime.InteropServices;
using System.Windows.Forms;

namespace SpellingFixer30
{
    /// <summary>
    /// For our dialogs that are shown without an owner (e.g. CscProject.FindReplacementRule): if they're opened from
    /// a window that's "always on top" (e.g. a client's dialog owned by a pinned window), they'd otherwise end up
    /// behind it, so use that window as the owner, and be on top as well.
    /// </summary>
    internal static class DialogOwner
    {
        private const int GWL_EXSTYLE = -20;
        private const int WS_EX_TOPMOST = 0x00000008;

        [DllImport("user32.dll")]
        private static extern IntPtr GetActiveWindow();

        [DllImport("user32.dll", EntryPoint = "GetWindowLong")]
        private static extern int GetWindowLong32(IntPtr hWnd, int nIndex);

        [DllImport("user32.dll", EntryPoint = "GetWindowLongPtr")]
        private static extern IntPtr GetWindowLongPtr64(IntPtr hWnd, int nIndex);

        private const uint GW_OWNER = 4;

        [DllImport("user32.dll")]
        private static extern IntPtr GetWindow(IntPtr hWnd, uint uCmd);

        private static long GetExStyle(IntPtr hWnd)
        {
            return (IntPtr.Size == 8) ? GetWindowLongPtr64(hWnd, GWL_EXSTYLE).ToInt64() : GetWindowLong32(hWnd, GWL_EXSTYLE);
        }

        // a window owned by a topmost window stays above it without being topmost itself, so check its owners too
        private static bool IsOnTop(IntPtr hWnd)
        {
            for (var i = 0; (hWnd != IntPtr.Zero) && (i < 20); i++, hWnd = GetWindow(hWnd, GW_OWNER))
            {
                if ((GetExStyle(hWnd) & WS_EX_TOPMOST) != 0)
                    return true;
            }
            return false;
        }

        private class Win32Window : IWin32Window
        {
            public Win32Window(IntPtr handle)
            {
                Handle = handle;
            }

            public IntPtr Handle { get; }
        }

        /// <summary>
        /// Shows the dialog modally, owned by the currently active window (as ShowDialog() would), but on top if
        /// that window is
        /// </summary>
        public static DialogResult ShowDialog(Form dialog)
        {
            IntPtr hWndActive = IntPtr.Zero;
            try
            {
                hWndActive = GetActiveWindow();
                if ((hWndActive != IntPtr.Zero) && IsOnTop(hWndActive))
                    dialog.TopMost = true;
            }
            catch
            {
                // not worth failing over; just show it as before
                hWndActive = IntPtr.Zero;
            }

            return (hWndActive == IntPtr.Zero)
                    ? dialog.ShowDialog()
                    : dialog.ShowDialog(new Win32Window(hWndActive));
        }
    }
}
