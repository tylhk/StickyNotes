using System;
using System.Windows.Forms;

namespace StickyNotes.Utils
{
    public static class ToastUtil
    {
        public static void ShowWindowsToastNotification(string title, string content)
        {
            // 创建托盘图标
            NotifyIcon notifyIcon = new NotifyIcon
            {
                Icon = SystemIcons.Information, // 图标,
                Visible = true
            };

            // 显示气泡通知
            notifyIcon.ShowBalloonTip(3, title, content, ToolTipIcon.Info);

            // 可选：通知结束后隐藏图标
            notifyIcon.Visible = false;
        }
    }
}
