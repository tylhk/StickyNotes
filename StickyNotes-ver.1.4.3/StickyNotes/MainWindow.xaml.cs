using Newtonsoft.Json;
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
        }
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
                    ((ComboBoxItem)FontSizeCombo.SelectedItem).Content.ToString()),
                Topmost = true
            };
            note.Show();
            InputTextBox.Clear();
            SaveNotes();
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            if (!App.IsExiting)
            {
                e.Cancel = true;
                this.Hide();
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
                    NoteContent = "这是一条示例便签：\n1.双击便签可以编辑内容\n2.右键可以删除便签或者将便签固定到窗口\n3.关闭主窗口后会自动最小化至托盘\n4.欢迎联系我的个人博客satone1008.cn",
                    Width = 200,
                    Height = 150,
                    Left = 100,
                    Top = 100,
                    BackgroundColor = Colors.Yellow
                };
                exampleNote.Show();
            }
        }

        private void SetVersionNumber()
        {
            var version = System.Reflection.Assembly.GetExecutingAssembly().GetName().Version;

            VersionTextBlock.Text = $"v{version.Major}.{version.Minor}.{version.Build}";
        }

        public void SaveNotes()
        {
            string appDataPath = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
            string directoryPath = Path.Combine(appDataPath, "StickyNotes");
            string filePath = Path.Combine(directoryPath, "notes.json");

            try
            {
                Directory.CreateDirectory(directoryPath);

                var notes = Application.Current.Windows
                    .OfType<StickyNoteControl>()
                    .Where(n => n.IsVisible && !string.IsNullOrWhiteSpace(n.NoteContent))
                    .ToList();

                File.AppendAllText(
                    Path.Combine(directoryPath, "debug.log"),
                    $"[{DateTime.Now}] 保存便签数量: {notes.Count}\n"
                );

                if (File.Exists(filePath))
                {
                    File.Delete(filePath);
                }

                if (notes.Count == 0) return;

                var notesData = notes.Select(n => new NoteData
                {
                    Content = n.NoteContent,
                    X = n.Left,
                    Y = n.Top,
                    TargetWindowHandle = n.TargetWindowHandle.ToInt64(),
                    TargetWindowTitle = Win32ApiHelper.GetWindowTitle(n.TargetWindowHandle),
                    TargetWindowClass = Win32ApiHelper.GetWindowClassName(n.TargetWindowHandle),
                    OffsetX = n.OffsetFromTarget.X,
                    OffsetY = n.OffsetFromTarget.Y,
                    Color = n.BackgroundColor.ToString()
                }).ToList();

                File.WriteAllText(
                    filePath,
                    JsonConvert.SerializeObject(notesData, Formatting.Indented),
                    Encoding.UTF8
                );
            }
            catch (Exception ex)
            {
                MessageBox.Show($"保存失败: {ex.Message}\n堆栈跟踪:\n{ex.StackTrace}");
            }
        }

        public void LoadNotes()
        {
            foreach (Window window in Application.Current.Windows.OfType<StickyNoteControl>().ToList())
            {
                window.Close();
            }
            string appDataPath = Environment.GetFolderPath(
                Environment.SpecialFolder.ApplicationData
            );
            string directoryPath = Path.Combine(appDataPath, "StickyNotes");
            string filePath = Path.Combine(directoryPath, "notes.json");
            string debugLogPath = Path.Combine(directoryPath, "debug.log");
            Directory.CreateDirectory(directoryPath);

            File.AppendAllText(debugLogPath, $"[{DateTime.Now}] 正在尝试加载路径: {filePath}\n");
            if (!File.Exists(filePath))
            {
                File.AppendAllText(
                Path.Combine(appDataPath, "StickyNotes", "debug.log"),
                $"[{DateTime.Now}] 未找到 notes.json\n"
                );
                return;
            }

            var notesData = JsonConvert.DeserializeObject<List<NoteData>>(File.ReadAllText(filePath));
            foreach (var data in notesData)
            {
                Color color;
                try
                {
                    if (!string.IsNullOrEmpty(data.Color))
                    {
                        color = (Color)ColorConverter.ConvertFromString(data.Color);
                    }
                    else
                    {
                        throw new FormatException("Color string is empty");
                    }
                }
                catch (Exception ex)
                {
                    File.AppendAllText(debugLogPath, $"颜色解析失败\n");
                    File.AppendAllText(debugLogPath, $"[{DateTime.Now}] 颜色解析失败: {data.Color} | 错误: {ex.Message}\n");
                    color = Colors.Yellow;
                }
                IntPtr targetHandle = new IntPtr(data.TargetWindowHandle);
                
                var note = new StickyNoteControl
                {
                    NoteContent = data.Content,
                    Left = data.X,
                    Top = data.Y,
                    TargetWindowHandle = targetHandle,
                    OffsetFromTarget = new Point(data.OffsetX, data.OffsetY),
                    BackgroundColor = color
                };
                note.Show();
            }
        }
    }
}