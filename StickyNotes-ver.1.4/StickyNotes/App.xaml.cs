using System.Windows;
using System.Windows.Forms; // 引入Windows Forms
using System.Drawing;
using Application = System.Windows.Application; // 明确指定别名

namespace StickyNotes
{
    public partial class App : Application
    {
        private NotifyIcon _notifyIcon;
        public static bool IsExiting { get; set; }

        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);

            // 确保 MainWindow 实例化
            if (MainWindow == null)
            {
                MainWindow = new MainWindow();
            }

            string iconPath = System.IO.Path.Combine(
                AppDomain.CurrentDomain.BaseDirectory,
                "icon",
                "r7ats-wjgih-001.ico"
            );

            _notifyIcon = new System.Windows.Forms.NotifyIcon();
            _notifyIcon.Icon = new Icon(iconPath);
            _notifyIcon.Text = "Sticky Notes";
            _notifyIcon.Visible = true;

            var contextMenu = new ContextMenuStrip();
            contextMenu.Items.Add("显示主窗口", null, (s, args) => ShowMainWindow());
            contextMenu.Items.Add("退出", null, (s, args) => ExitApplication());
            _notifyIcon.ContextMenuStrip = contextMenu;

            _notifyIcon.DoubleClick += (s, args) => ShowMainWindow();

            // 现在 MainWindow 已初始化，可以安全订阅事件
            MainWindow.Closing += MainWindow_Closing;

            // 如果启动时不显示主窗口，可在此隐藏
            MainWindow.Hide();
        }

        private void MainWindow_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            if (!IsExiting)
            {
                e.Cancel = true;
                MainWindow.Hide();
            }
        }

        private void ShowMainWindow()
        {
            MainWindow.Show();
            MainWindow.WindowState = WindowState.Normal;
            MainWindow.Activate();
        }

        private void ExitApplication()
        {
            IsExiting = true;
            MainWindow.Close(); // 触发保存逻辑
            _notifyIcon.Visible = false;
            Current.Shutdown();
        }

        protected override void OnExit(ExitEventArgs e)
        {
            _notifyIcon?.Dispose();
            base.OnExit(e);
        }
    }
}