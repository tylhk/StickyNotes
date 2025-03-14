using Newtonsoft.Json;
using StickyNotes.Controls;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
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
            SaveNotes();
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
            var notesData = Application.Current.Windows
                .OfType<StickyNoteControl>()
                .Select(n => new
                {
                    Content = n.NoteContent,
                    X = n.Left,
                    Y = n.Top
                });
            File.WriteAllText("notes.json", JsonConvert.SerializeObject(notesData));
        }

        private void LoadNotes()
        {
            if (File.Exists("notes.json"))
            {
                // 原有加载逻辑
            }
            else
            {
                // 首次运行创建示例便签
                var note = new StickyNoteControl { NoteContent = "示例便签条" };
                note.Show();
            }
        }
    }
}