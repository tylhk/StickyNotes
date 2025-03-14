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
            };

            Canvas.SetLeft(note, 50 + NotesCanvas.Children.Count * 30);
            Canvas.SetTop(note, 50 + NotesCanvas.Children.Count * 30);

            NotesCanvas.Children.Add(note);
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
            var notesData = NotesCanvas.Children.OfType<StickyNoteControl>()
                .Select(n => new
                {
                    Content = n.NoteContent,
                    X = Canvas.GetLeft(n),
                    Y = Canvas.GetTop(n)
                });
            File.WriteAllText("notes.json", JsonConvert.SerializeObject(notesData));
        }

        private void LoadNotes()
        {
            if (!File.Exists("notes.json")) return;

            var json = File.ReadAllText("notes.json");
            var data = JsonConvert.DeserializeObject<dynamic>(json);

            foreach (var item in data)
            {
                var note = new StickyNoteControl { NoteContent = item.Content };
                Canvas.SetLeft(note, (double)item.X);
                Canvas.SetTop(note, (double)item.Y);
                NotesCanvas.Children.Add(note);
            }
        }
    }
}