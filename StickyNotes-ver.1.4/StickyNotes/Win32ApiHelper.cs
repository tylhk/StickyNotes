using System;
using System.Runtime.InteropServices;
using System.Text;

public static class Win32ApiHelper
{
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