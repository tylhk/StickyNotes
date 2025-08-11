using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace StickyNotes
{
    /// <summary>
    /// SettingPanel.xaml 的交互逻辑
    /// </summary>
    public partial class SettingPanel : Window
    {
        public SettingPanel()
        {
            InitializeComponent();
            SetVersionNumber();
        }

        private void OpenGitHubLink(object sender, RoutedEventArgs e)
        {
            OpenUrl("https://github.com/tylhk/Stickynotes/");
        }

        private void OpenBlogLink(object sender, RoutedEventArgs e)
        {
            OpenUrl("https://satone1008.cn/");
        }

        // 通用打开 URL 方法
        private void OpenUrl(string url)
        {
            try
            {
                System.Diagnostics.Process.Start(new System.Diagnostics.ProcessStartInfo(url)
                {
                    UseShellExecute = true  // 用系统默认浏览器打开
                });
            }
            catch (Exception ex)
            {
                // 在某些系统上可能失败，比如没有默认浏览器
                System.Windows.MessageBox.Show($"无法打开链接: {ex.Message}", "错误",
                    System.Windows.MessageBoxButton.OK, System.Windows.MessageBoxImage.Error);
            }
        }
        private void SetVersionNumber()
        {
            var version = System.Reflection.Assembly.GetExecutingAssembly().GetName().Version;

            labelVersion.Content = $"Version: V{version.Major}.{version.Minor}.{version.Build}";
        }
    }
}
