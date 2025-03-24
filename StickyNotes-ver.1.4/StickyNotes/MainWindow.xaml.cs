using Newtonsoft.Json;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using Application = System.Windows.Application;
using Color = System.Windows.Media.Color;
using MessageBox = System.Windows.MessageBox;
using Point = System.Windows.Point;
using StickyNoteControl = StickyNotes.Controls.StickyNoteControl;


namespace StickyNotes
{

    public partial class MainWindow : Window
    {
        public Color SelectedColor { get; set; } = Colors.Yellow;
        private void ColorPickerButton_Click(object sender, RoutedEventArgs e)
        {
            var colorPickerDialog = new ColorPickerDialog();
            if (colorPickerDialog.ShowDialog() == true)
            {
                SelectedColor = colorPickerDialog.SelectedColor;
            }
        }

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
                Height = 150,
                BackgroundColor = SelectedColor,
                FontSize = double.Parse(
                    ((ComboBoxItem)FontSizeCombo.SelectedItem).Content.ToString())
            };
            note.Show();
            InputTextBox.Clear();
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            if (!App.IsExiting)
            {
                e.Cancel = true;
                this.Hide();
                return;
            }
            SaveNotes();
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
                    NoteContent = "这是一条示例便签：\n1.双击便签可以编辑内容\n2.右键可以删除便签或者将便签固定到窗口\n3.关闭主窗口后会自动最小化至托盘\n4.欢迎联系我的个人博客satone1008.cn",
                    Width = 200,
                    Height = 150,
                    Left = 100,
                    Top = 100
                };
                exampleNote.Show();
            }
        }

        private void SetVersionNumber()
        {
            var version = System.Reflection.Assembly.GetExecutingAssembly().GetName().Version;

            VersionTextBlock.Text = $"v{version.Major}.{version.Minor}.{version.Build}";
        }

        private void SaveNotes()
        {
            try 
            { 
                var notes = Application.Current.Windows.OfType<StickyNoteControl>().ToList();
                if (notes.Count == 0)
                {
                    File.Delete("notes.json"); 
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
                if (targetHandle == IntPtr.Zero)
                {
                    var desktopNote = new StickyNoteControl
                    {
                        NoteContent = data.Content,
                        Left = data.X,
                        Top = data.Y
                    };
                    desktopNote.PinToDesktop();
                    desktopNote.Show();
                    continue;
                }
                if (!Win32ApiHelper.IsWindowValid(targetHandle))
                {
                    targetHandle = Win32ApiHelper.FindWindowByTitleAndClass(
                        data.TargetWindowTitle,
                        data.TargetWindowClass
                    );
                }
                if (targetHandle == IntPtr.Zero)
                {
                    var floatingNote = new StickyNoteControl
                    {
                        NoteContent = data.Content,
                        Left = data.X,
                        Top = data.Y
                    };
                    floatingNote.Topmost = true; 
                    floatingNote.Show();
                    continue;
                }
                var note = new StickyNoteControl
                {
                    NoteContent = data.Content,
                    Left = data.X,
                    Top = data.Y,
                    TargetWindowHandle = targetHandle,
                    OffsetFromTarget = new Point(data.OffsetX, data.OffsetY)
                };
                note.PinToWindow(targetHandle, data.OffsetX, data.OffsetY);
                note.Show();
            }
        }
    }
}