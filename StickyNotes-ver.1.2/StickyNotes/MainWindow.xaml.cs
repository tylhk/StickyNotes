using Newtonsoft.Json;
using System.IO;
using System.Windows;
using StickyNoteControl = StickyNotes.Controls.StickyNoteControl;

namespace StickyNotes
{

    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private void AddNoteButton_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(InputTextBox.Text)) return;

            var note = new StickyNoteControl
            {
                NoteContent = InputTextBox.Text,
                Width = 200,
                Height = 150
            };
            note.Show(); // 显示为独立窗口
            InputTextBox.Clear();
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            // 先保存数据
            SaveNotes();

            // 再关闭所有便签窗口
            foreach (Window window in Application.Current.Windows.OfType<StickyNoteControl>().ToList())
            {
                window.Close();
            }
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            LoadNotes();
            SetVersionNumber();
        }

        private void SetVersionNumber()
        {
            // 获取程序集版本
            var version = System.Reflection.Assembly.GetExecutingAssembly().GetName().Version;

            // 格式化版本号（例如：v1.0.0）
            VersionTextBlock.Text = $"v{version.Major}.{version.Minor}.{version.Build}";
        }

        private void SaveNotes()
        {
            try 
            { 
                // 显式获取所有便签窗口实例
                var notes = Application.Current.Windows.OfType<StickyNoteControl>().ToList();
                if (notes.Count == 0)
                {
                    File.Delete("notes.json"); // 无便签时删除旧文件
                    return;
                }

                var notesData = notes.Select(n => new NoteData
                {
                    Content = n.NoteContent,
                    X = n.Left,
                    Y = n.Top,
                    TargetWindowHandle = n.TargetWindowHandle,
                    TargetWindowTitle = Win32ApiHelper.GetWindowTitle(n.TargetWindowHandle),
                    TargetWindowClass = Win32ApiHelper.GetWindowClassName(n.TargetWindowHandle),
                    OffsetX = n.OffsetFromTarget.X,
                    OffsetY = n.OffsetFromTarget.Y
                }).ToList();

                File.WriteAllText("notes.json", JsonConvert.SerializeObject(notesData));
            }
            catch (Exception ex)
            {
                MessageBox.Show($"保存便签失败: {ex.Message}", "错误", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void LoadNotes()
        {
            if (!File.Exists("notes.json")) return;

            var notesData = JsonConvert.DeserializeObject<List<NoteData>>(File.ReadAllText("notes.json"));
            foreach (var data in notesData)
            {
                IntPtr targetHandle = data.TargetWindowHandle;

                // 如果原句柄无效，尝试根据标题和类名查找新窗口
                if (!Win32ApiHelper.IsWindowValid(targetHandle))
                {
                    targetHandle = Win32ApiHelper.FindWindowByTitleAndClass(
                        data.TargetWindowTitle,
                        data.TargetWindowClass
                    );
                }

                // 如果目标窗口无效，跳过此便签
                if (targetHandle == IntPtr.Zero) continue;

                var note = new StickyNoteControl
                {
                    NoteContent = data.Content,
                    Left = data.X,
                    Top = data.Y,
                    TargetWindowHandle = targetHandle,
                    OffsetFromTarget = new Point(data.OffsetX, data.OffsetY)
                };

                // 显式调用 PinToWindow 启动跟踪定时器
                note.PinToWindow(targetHandle, data.OffsetX, data.OffsetY);
                note.Show();
            }
        }
    }
}