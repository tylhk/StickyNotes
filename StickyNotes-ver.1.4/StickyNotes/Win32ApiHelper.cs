using System;
using System.Runtime.InteropServices;
using System.Text;

public static class Win32ApiHelper
{
    public const int GWL_EXSTYLE = -20;
    public const int WS_EX_TOOLWINDOW = 0x00000080;

    [DllImport("user32.dll")]
    public static extern int GetWindowLong(IntPtr hWnd, int nIndex);
    public const IntPtr HWND_TOP = (IntPtr)0;
    public const uint SWP_NOMOVE = 0x0002;
    public const uint SWP_NOSIZE = 0x0001;
    public const uint SWP_NOACTIVATE = 0x0010;

    [DllImport("user32.dll")]
    [return: MarshalAs(UnmanagedType.Bool)]
    public static extern bool SetWindowPos(IntPtr hWnd, IntPtr hWndInsertAfter, int X, int Y, int cx, int cy, uint uFlags);

    [DllImport("user32.dll")]
    public static extern int SetWindowLong(IntPtr hWnd, int nIndex, int dwNewLong);
    [DllImport("user32.dll", CharSet = CharSet.Auto)]
    public static extern int GetWindowText(IntPtr hWnd, StringBuilder lpString, int nMaxCount);
    [DllImport("user32.dll", CharSet = CharSet.Auto)]
    public static extern int GetClassName(IntPtr hWnd, StringBuilder lpClassName, int nMaxCount);
    [DllImport("user32.dll")]
    public static extern IntPtr GetForegroundWindow();
    [DllImport("user32.dll")]
    [return: MarshalAs(UnmanagedType.Bool)]
    public static extern bool GetWindowRect(IntPtr hWnd, out RECT lpRect);
    [DllImport("user32.dll")]
    [return: MarshalAs(UnmanagedType.Bool)]
    public static extern bool IsWindow(IntPtr hWnd);
    [DllImport("user32.dll")]
    [return: MarshalAs(UnmanagedType.Bool)]
    public static extern bool IsWindowVisible(IntPtr hWnd);
    [DllImport("user32.dll")]
    [return: MarshalAs(UnmanagedType.Bool)]
    public static extern bool EnumWindows(EnumWindowsProc lpEnumFunc, IntPtr lParam);
    public delegate bool EnumWindowsProc(IntPtr hWnd, IntPtr lParam);
    public static IntPtr FindWindowByTitleAndClass(string title, string className)
    {
        IntPtr foundHandle = IntPtr.Zero;
        EnumWindows((hWnd, lParam) =>
        {
            if (!IsWindowVisible(hWnd)) return true;

            string currentTitle = GetWindowTitle(hWnd);
            string currentClass = GetWindowClassName(hWnd);
            bool isMatch =
                (currentTitle.Contains(title) || title.Contains(currentTitle)) &&
                currentClass.Equals(className, StringComparison.OrdinalIgnoreCase);

            if (isMatch)
            {
                foundHandle = hWnd;
                return false;
            }
            return true;
        }, IntPtr.Zero);

        return foundHandle;
    }
    [StructLayout(LayoutKind.Sequential)]
    public struct RECT
    {
        public int Left;
        public int Top;
        public int Right;
        public int Bottom;
    }
    public static bool IsWindowValid(IntPtr hWnd)
    {
        return hWnd != IntPtr.Zero &&
               IsWindow(hWnd) &&
               IsWindowVisible(hWnd);
    }
    public static bool IsDesktopWindow(IntPtr hWnd)
    {
        StringBuilder className = new StringBuilder(256);
        GetClassName(hWnd, className, className.Capacity);
        return className.ToString() == "Progman" || className.ToString() == "WorkerW";
    }
    public static string GetWindowTitle(IntPtr hWnd)
    {
        StringBuilder title = new StringBuilder(256);
        GetWindowText(hWnd, title, title.Capacity);
        return title.ToString();
    }
    public static string GetWindowClassName(IntPtr hWnd)
    {
        StringBuilder className = new StringBuilder(256);
        GetClassName(hWnd, className, className.Capacity);
        return className.ToString();
    }


}