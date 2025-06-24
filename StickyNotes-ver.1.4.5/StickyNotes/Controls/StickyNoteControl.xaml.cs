using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Interop;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Threading;
using Color = System.Windows.Media.Color;
using KeyEventArgs = System.Windows.Input.KeyEventArgs;
using MessageBox = System.Windows.MessageBox;
using MouseEventArgs = System.Windows.Input.MouseEventArgs;
using Point = System.Windows.Point;

namespace StickyNotes.Controls
{
    public partial class StickyNoteControl : Window
    {
        public bool IsUserDeleted { get; private set; } = false;
        private IntPtr _targetWindowHandle = IntPtr.Zero;
        private DispatcherTimer _trackingTimer;
        private Point _offsetFromTargetWindow;
        private bool isDragging;
        private Point offset;

        public static readonly DependencyProperty NoteContentProperty =
            DependencyProperty.Register("NoteContent", typeof(string), typeof(StickyNoteControl));
        private DateTime _lastClickTime;
        private const int DoubleClickThreshold = 300; // 毫秒
        private Point _velocity;
        private const double Friction = 0.9;
        private IntPtr _lastValidTargetHandle = IntPtr.Zero;
        private IntPtr _preMenuTargetHandle = IntPtr.Zero;
        private IntPtr _preHoverTargetHandle = IntPtr.Zero;

        public static readonly DependencyProperty BackgroundColorProperty =
    DependencyProperty.Register(
        "BackgroundColor",
        typeof(Color), 
        typeof(StickyNoteControl),
        new PropertyMetadata(Colors.Yellow)
    );

        public static readonly DependencyProperty FontSizeProperty =
            DependencyProperty.Register("FontSize", typeof(double), typeof(StickyNoteControl),
                new PropertyMetadata(14.0));

        public Color BackgroundColor
        {
            get => (Color)GetValue(BackgroundColorProperty);
            set => SetValue(BackgroundColorProperty, value);
        }

        public double FontSize
        {
            get => (double)GetValue(FontSizeProperty);
            set => SetValue(FontSizeProperty, value);
        }
        public IntPtr TargetWindowHandle
        {
            get => _targetWindowHandle;
            set => _targetWindowHandle = value;
        }

        public Point OffsetFromTarget
        {
            get => _offsetFromTargetWindow;
            set => _offsetFromTargetWindow = value;
        }
        protected override void OnMouseEnter(MouseEventArgs e)
        {
            _preHoverTargetHandle = Win32ApiHelper.GetForegroundWindow();
            base.OnMouseEnter(e);
        }
        protected override void OnContextMenuOpening(ContextMenuEventArgs e)
        {
            Dispatcher.BeginInvoke(new Action(() =>
            {
                _lastValidTargetHandle = Win32ApiHelper.GetForegroundWindow();
                var wih = new WindowInteropHelper(this);
                if (_lastValidTargetHandle == wih.Handle)
                {
                    _lastValidTargetHandle = IntPtr.Zero;
                }
            }), DispatcherPriority.ApplicationIdle, null);

            base.OnContextMenuOpening(e);
        }
        protected override void OnMouseRightButtonDown(MouseButtonEventArgs e)
        {
            _preMenuTargetHandle = _preHoverTargetHandle;
            var wih = new WindowInteropHelper(this);
            if (_preMenuTargetHandle == wih.Handle)
            {
                _preMenuTargetHandle = IntPtr.Zero;
            }

            base.OnMouseRightButtonDown(e);
        }

        private void TextBlock_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.ChangedButton != MouseButton.Left)
                return;

            var currentTime = DateTime.Now;
            var elapsed = (currentTime - _lastClickTime).TotalMilliseconds;

            if (elapsed <= DoubleClickThreshold)
            {
                StartEditing();
                _lastClickTime = DateTime.MinValue;
            }
            else
            {
                _lastClickTime = currentTime;
            }
        }

        private void OnRendering(object sender, EventArgs e)
        {
            if (_velocity.X == 0 && _velocity.Y == 0)
            {
                CompositionTarget.Rendering -= OnRendering;
                return;
            }
            var x = Canvas.GetLeft(this) + _velocity.X;
            var y = Canvas.GetTop(this) + _velocity.Y;

            Canvas.SetLeft(this, x);
            Canvas.SetTop(this, y);

            _velocity.X *= Friction;
            _velocity.Y *= Friction;
            if (Math.Abs(_velocity.X) < 0.1 && Math.Abs(_velocity.Y) < 0.1)
            {
                _velocity = new Point(0, 0);
                CompositionTarget.Rendering -= OnRendering;
            }
        }


        private void StartEditing()
        {
            if (EditBox.Visibility == Visibility.Visible)
                return;

            EditBox.Opacity = 0;
            EditBox.Visibility = Visibility.Visible;
            DisplayTextBlock.Visibility = Visibility.Collapsed;

            var fadeIn = new DoubleAnimation
            {
                From = 0,
                To = 1,
                Duration = TimeSpan.FromMilliseconds(200)
            };
            EditBox.BeginAnimation(UIElement.OpacityProperty, fadeIn);

            EditBox.Focus();
            EditBox.SelectAll();
        }
        public string NoteContent
        {
            get => (string)GetValue(NoteContentProperty);
            set => SetValue(NoteContentProperty, value);
        }

        public StickyNoteControl()
        {
            InitializeComponent();
            this.DataContext = this;
            this.Topmost = true;
            SourceInitialized += (s, e) =>
            {
                var helper = new WindowInteropHelper(this);
                int exStyle = Win32ApiHelper.GetWindowLong(helper.Handle, Win32ApiHelper.GWL_EXSTYLE);
                exStyle |= (int)Win32ApiHelper.WS_EX_TOOLWINDOW;
                Win32ApiHelper.SetWindowLong(helper.Handle, Win32ApiHelper.GWL_EXSTYLE, exStyle);
            }; 
            RenderMarkdownToTextBlock(NoteContent ?? "", DisplayTextBlock);
        }

        private void Border_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.ChangedButton == MouseButton.Left)
            {
                this.DragMove();
            }
        }
        protected override void OnMouseMove(MouseEventArgs e)
        {
            if (isDragging && e.LeftButton == MouseButtonState.Pressed)
            {
                if (_targetWindowHandle != IntPtr.Zero)
                {
                    StopTracking();
                    PinMenuItem.Visibility = Visibility.Visible;
                    UnpinMenuItem.Visibility = Visibility.Collapsed;
                }
                var parent = Parent as Canvas;
                var currentPosition = e.GetPosition(parent);
                double newX = currentPosition.X - offset.X;
                double newY = currentPosition.Y - offset.Y;
                UpdatePosition(newX, newY);
            }
        }

        protected override void OnMouseUp(MouseButtonEventArgs e)
        {
            if (isDragging)
            {
                isDragging = false;
                ReleaseMouseCapture();
                StartInertia();
            }
        }

        private void StartInertia()
        {
            CompositionTarget.Rendering += OnRendering;
        }

        private void DeleteMenuItem_Click(object sender, RoutedEventArgs e)
        {
            IsUserDeleted = true;
            var fadeOut = new DoubleAnimation
            {
                From = 1,
                To = 0,
                Duration = TimeSpan.FromSeconds(0.8)
            };
            fadeOut.Completed += (s, _) => {
                Close();
                if (System.Windows.Application.Current.MainWindow is MainWindow mainWindow)
                {
                    mainWindow.SaveNotes();
                }
            };
            BeginAnimation(OpacityProperty, fadeOut);
        }

        private void EditBox_LostFocus(object sender, RoutedEventArgs e)
        {
            EndEditing();
        }

        private void EditBox_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Escape)
            {
                EndEditing();
            }
        }

        private void EndEditing()
        {
            if (string.IsNullOrWhiteSpace(EditBox.Text))
            {
                EditBox.Text = "新便签";
            }
            NoteContent = EditBox.Text;  // 更新属性

            // 渲染 Markdown
            RenderMarkdownToTextBlock(NoteContent, DisplayTextBlock);

            var fadeOut = new DoubleAnimation
            {
                From = 1,
                To = 0,
                Duration = TimeSpan.FromMilliseconds(200)
            };
            fadeOut.Completed += (s, _) =>
            {
                EditBox.Visibility = Visibility.Collapsed;
                DisplayTextBlock.Visibility = Visibility.Visible;
            };

            EditBox.BeginAnimation(UIElement.OpacityProperty, fadeOut);

            var mainWindow = System.Windows.Application.Current.MainWindow;
            if (mainWindow != null && !mainWindow.IsActive)
            {
                mainWindow.Activate();
            }
        }

        protected override void OnDeactivated(EventArgs e)
        {
            base.OnDeactivated(e);
            if (EditBox.Visibility == Visibility.Visible)
            {
                EndEditing();
                if (System.Windows.Application.Current.MainWindow is MainWindow mainWindow)
                {
                    mainWindow.SaveNotes();
                }
            }
        }

        private void UpdatePosition(double x, double y)
        {
            var parent = Parent as Canvas;
            if (parent == null) return;
            double padding = 10; 
            x = Math.Max(-padding, Math.Min(x, parent.ActualWidth - this.ActualWidth + padding));
            y = Math.Max(-padding, Math.Min(y, parent.ActualHeight - this.ActualHeight + padding));
            Canvas.SetLeft(this, x);
            Canvas.SetTop(this, y);
        }
        private void PinToWindow_Click(object sender, RoutedEventArgs e)
        {
            IntPtr targetHandle = _preMenuTargetHandle;
            if (targetHandle == IntPtr.Zero)
            {
                MessageBox.Show("请先选中目标窗口，再右键固定！", "错误", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }
            var wih = new WindowInteropHelper(this);
            if (targetHandle == wih.Handle)
            {
                MessageBox.Show("不能固定到自身窗口！", "错误", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }
            if (Win32ApiHelper.IsDesktopWindow(targetHandle))
            {
                PinToDesktop();
            }
            else
            {
                this.Topmost = false;
                PinToWindow(targetHandle);
            }
            PinMenuItem.Visibility = Visibility.Collapsed;
            UnpinMenuItem.Visibility = Visibility.Visible;
        }
        private void Unpin_Click(object sender, RoutedEventArgs e)
        {
            StopTracking();
            PinMenuItem.Visibility = Visibility.Visible;
            UnpinMenuItem.Visibility = Visibility.Collapsed;
            this.Topmost = true;
        }

        public void PinToDesktop()
        {
            this.Topmost = false;
            _targetWindowHandle = IntPtr.Zero;
            StopTracking(); 
        }

        private void PinToWindow(IntPtr targetHandle)
        {
            Win32ApiHelper.GetWindowRect(targetHandle, out Win32ApiHelper.RECT targetRect);
            Point screenTargetPos = new Point(targetRect.Left, targetRect.Top);
            Point wpfTargetPos = ConvertScreenToWpf(screenTargetPos);
            _offsetFromTargetWindow = new Point(
                this.Left - wpfTargetPos.X,
                this.Top - wpfTargetPos.Y
            );
            var selfHandle = new WindowInteropHelper(this).Handle;
            WindowInteropHelper helper = new WindowInteropHelper(this);
            helper.Owner = targetHandle;
            _targetWindowHandle = targetHandle;
            _trackingTimer = new DispatcherTimer { Interval = TimeSpan.FromMilliseconds(100) };
            _trackingTimer.Tick += TrackingTimer_Tick;
            _trackingTimer.Start();
        }

        private void TrackingTimer_Tick(object sender, EventArgs e)
        {
            if (_targetWindowHandle == IntPtr.Zero || isDragging) return;

            if (!Win32ApiHelper.IsWindowValid(_targetWindowHandle))
            {
                StopTracking();
                this.Topmost = true;
                return;
            }
            Win32ApiHelper.GetWindowRect(_targetWindowHandle, out Win32ApiHelper.RECT rect);
            Point screenPos = new Point(rect.Left, rect.Top);
            Point wpfPos = ConvertScreenToWpf(screenPos);
            this.Left = wpfPos.X + _offsetFromTargetWindow.X;
            this.Top = wpfPos.Y + _offsetFromTargetWindow.Y;
            this.UpdateLayout();
        }
        private Point ConvertScreenToWpf(Point screenPoint)
        {
            PresentationSource source = PresentationSource.FromVisual(this);
            if (source?.CompositionTarget != null)
            {
                Matrix transform = source.CompositionTarget.TransformFromDevice;
                return transform.Transform(screenPoint);
            }
            return screenPoint;
        }

        private void StopTracking()
        {
            if (_trackingTimer != null)
            {
                _trackingTimer.Stop();
                _trackingTimer = null;
            }
            _targetWindowHandle = IntPtr.Zero;
            PinMenuItem.Visibility = Visibility.Visible;
            UnpinMenuItem.Visibility = Visibility.Collapsed;
        }
        protected override void OnClosed(EventArgs e)
        {
            StopTracking();
            base.OnClosed(e);
        }
        public void PinToWindow(IntPtr targetHandle, double offsetX, double offsetY)
        {
            _targetWindowHandle = targetHandle;
            _offsetFromTargetWindow = new Point(offsetX, offsetY);

            if (_trackingTimer != null)
            {
                _trackingTimer.Stop();
                _trackingTimer = null;
            }

            _trackingTimer = new DispatcherTimer { Interval = TimeSpan.FromMilliseconds(100) };
            _trackingTimer.Tick += TrackingTimer_Tick;
            _trackingTimer.Start();

            PinMenuItem.Visibility = Visibility.Collapsed;
            UnpinMenuItem.Visibility = Visibility.Visible;
        }
        protected override void OnClosing(System.ComponentModel.CancelEventArgs e)
        {
            if (!App.IsExiting && !IsUserDeleted)
            {
                e.Cancel = true;
                this.Visibility = Visibility.Hidden;
                return;
            }
            base.OnClosing(e);
        }

        /// <summary>
        /// 简单语法渲染
        /// </summary>
        private void RenderMarkdownToTextBlock(string markdown, TextBlock target)
        {
            target.Inlines.Clear();
            int i = 0;
            while (i < markdown.Length)
            {
                // 加粗 **
                if (markdown.Substring(i).StartsWith("**"))
                {
                    int end = markdown.IndexOf("**", i + 2);
                    if (end > i + 1)
                    {
                        string boldText = markdown.Substring(i + 2, end - i - 2);
                        var bold = new Bold(new Run(boldText));
                        target.Inlines.Add(bold);
                        i = end + 2;
                        continue;
                    }
                }
                // 删除线 ~~
                if (markdown.Substring(i).StartsWith("~~"))
                {
                    int end = markdown.IndexOf("~~", i + 2);
                    if (end > i + 1)
                    {
                        string strikeText = markdown.Substring(i + 2, end - i - 2);
                        var run = new Run(strikeText);
                        run.TextDecorations = TextDecorations.Strikethrough;
                        target.Inlines.Add(run);
                        i = end + 2;
                        continue;
                    }
                }
                // 下划线 <u>...</u>
                if (markdown.Substring(i).StartsWith("<u>"))
                {
                    int end = markdown.IndexOf("</u>", i + 3);
                    if (end > i + 3)
                    {
                        string uText = markdown.Substring(i + 3, end - i - 3);
                        var run = new Run(uText);
                        run.TextDecorations = TextDecorations.Underline;
                        target.Inlines.Add(run);
                        i = end + 4 + 2; // 跳过</u>
                        continue;
                    }
                }
                // 普通文本
                int nextSpecial = FindNextSpecial(markdown, i);
                string plain = markdown.Substring(i, nextSpecial - i);
                target.Inlines.Add(new Run(plain));
                i = nextSpecial;
            }
        }

        private int FindNextSpecial(string text, int start)
        {
            int min = text.Length;
            int pos;
            foreach (var tag in new[] { "**", "~~", "<u>" })
            {
                pos = text.IndexOf(tag, start);
                if (pos >= 0 && pos < min) min = pos;
            }
            return min;
        }
        private void BoldMenuItem_Click(object sender, RoutedEventArgs e)
        {
            WrapSelection("**", "**"); // 加粗
        }

        private void UnderlineMenuItem_Click(object sender, RoutedEventArgs e)
        {
            WrapSelection("<u>", "</u>"); // 下划线
        }

        private void StrikethroughMenuItem_Click(object sender, RoutedEventArgs e)
        {
            WrapSelection("~~", "~~"); // 删除线
        }

        private void WrapSelection(string prefix, string suffix)
        {
            if (EditBox.Visibility != Visibility.Visible) return;

            int start = EditBox.SelectionStart;
            int length = EditBox.SelectionLength;
            if (length == 0) return;

            string text = EditBox.Text;
            string selected = text.Substring(start, length);


            string newText = text.Substring(0, start)
                            + prefix + selected + suffix
                            + text.Substring(start + length);

            EditBox.Text = newText;

            EditBox.Select(start + prefix.Length, length);
            EditBox.Focus();
        }
    }
}