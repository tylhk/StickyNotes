using Newtonsoft.Json;
using StickyNotes.Controls;
using StickyNotes.Utils;
using System.ComponentModel;
using System.IO;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using Application = System.Windows.Application;
using Color = System.Windows.Media.Color;
using ColorConverter = System.Windows.Media.ColorConverter;
using MessageBox = System.Windows.MessageBox;
using Point = System.Windows.Point;
using StickyNoteControl = StickyNotes.Controls.StickyNoteControl;

namespace StickyNotes
{

    public partial class MainWindow : Window
    {
        public class NoteData
        {
            public string Content { get; set; }
            public double X { get; set; }
            public double Y { get; set; }
            public long TargetWindowHandle { get; set; }
            public string TargetWindowTitle { get; set; }
            public string TargetWindowClass { get; set; }
            public double OffsetX { get; set; }
            public double OffsetY { get; set; }
            public string Color { get; set; }
            public string FontColor { get; set; } // Added to persist font color per note

        }
        //对象管理器
        private List<StickyNoteControl> stickyNoteControls = new List<StickyNoteControl>();
        public Color SelectedColor { get; set; } = Colors.Yellow;
        // SelectedFontColor stores the font color chosen in the color picker dialog (added)
        public Color SelectedFontColor { get; set; } = Colors.Black;
        private void ColorPickerButton_Click(object sender, RoutedEventArgs e)
        {
            var colorPickerDialog = new ColorPickerDialog();
            if (colorPickerDialog.ShowDialog() == true)
            {
                // Save both background color and font color selected in the dialog
                SelectedColor = colorPickerDialog.SelectedColor;
                SelectedFontColor = colorPickerDialog.SelectedFontColor;
            }
        }

n        public MainWindow()
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
                Height = 150,
                BackgroundColor = SelectedColor,
                FontColor = SelectedFontColor, // Apply chosen font color to the new note (added)
                FontSize = double.Parse(
                    ((ComboBoxItem)FontSizeCombo.SelectedItem).Content.ToString()),
                Topmost = true
            };
            stickyNoteControls.Add(note);
            //MessageBox.Show(stickyNoteControls.Count.ToString());
            note.Show();
            //note.Show();

            //for 
            InputTextBox.Clear();
            SaveNotes();
        }
        //对象被创建后需要调一下showNotes
        public void showNotes()
        {
            foreach (var note in stickyNoteControls)
            {
                if (!note.IsVisible)  // 或 note.Visibility == Visibility.Visible
                {
                    note.Show();
                }

n            }
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            if (!App.IsExiting)
            {
                e.Cancel = true;
                this.Hide();
                Utils.ToastUtil.ShowWindowsToastNotification("通知", "StickyNotes已最小化至任务栏");
                return;
            }
            foreach (Window window in Application.Current.Windows.OfType<StickyNoteControl>().ToList())
            {
                window.Close();
            }
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            LoadNotes();
            SetVersionNumber();
            if (!File.Exists("notes.json") && Application.Current.Windows.OfType<StickyNoteControl>().Count() == 0)
            {
                var exampleNote = new StickyNoteControl
                {
                    NoteContent = "这是一条示例便签：\n1.双击便签可以编辑内容 选中文字后右键可以添加文字格式\n2.右键可以删除便签或者将便签固定到窗口\n3.关闭主窗口后会自动最小化至托盘\n4.欢迎联系我的个人博客satone1008.cn",
                    Width = 200,
                    Height = 150,
                    Left = 100,
                    Top = 100,
                    BackgroundColor = Colors.Yellow,
                    FontColor = Colors.Black
                };
                stickyNoteControls.Add(exampleNote);
                showNotes();
            }
        }

        private void SetVersionNumber()
        {
            var version = System.Reflection.Assembly.GetExecutingAssembly().GetName().Version;

            VersionTextBlock.Text = $"v{version.Major}.{version.Minor}.{version.Build}";
        }