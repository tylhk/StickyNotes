using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Interop;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Threading;

namespace StickyNotes.Controls
{
    public partial class StickyNoteControl : Window
    {
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

        // 添加依赖属性
        public static readonly DependencyProperty BackgroundColorProperty =
    DependencyProperty.Register(
        "BackgroundColor",
        typeof(Color),  // 注意：类型是 Color，不是 Brush
        typeof(StickyNoteControl),
        new PropertyMetadata(Colors.Yellow)
    );

        public static readonly DependencyProperty FontSizeProperty =
            DependencyProperty.Register("FontSize", typeof(double), typeof(StickyNoteControl),
                new PropertyMetadata(14.0));

        // 属性包装器
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
        // 公共属性（暴露给外部访问）
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
        // 当鼠标进入便签区域时记录前景窗口
        protected override void OnMouseEnter(MouseEventArgs e)
        {
            _preHoverTargetHandle = Win32ApiHelper.GetForegroundWindow();
            base.OnMouseEnter(e);
        }
        protected override void OnContextMenuOpening(ContextMenuEventArgs e)
        {
            // 延长延迟时间至 200ms
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
        // 覆盖鼠标右键按下事件
        protected override void OnMouseRightButtonDown(MouseButtonEventArgs e)
        {
            // 使用鼠标悬停时捕获的句柄
            _preMenuTargetHandle = _preHoverTargetHandle;

            // 排除自身窗口
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
                // 双击事件处理
                StartEditing();
                _lastClickTime = DateTime.MinValue; // 重置时间
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

            // 应用惯性
            var x = Canvas.GetLeft(this) + _velocity.X;
            var y = Canvas.GetTop(this) + _velocity.Y;

            Canvas.SetLeft(this, x);
            Canvas.SetTop(this, y);

            // 应用摩擦力
            _velocity.X *= Friction;
            _velocity.Y *= Friction;

            // 当速度足够小时停止
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

            // 添加动画效果
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
            this.Owner = Application.Current.MainWindow; // 设置主窗口为Owner
            this.DataContext = this;
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
                // 如果已固定，立即解除
                if (_targetWindowHandle != IntPtr.Zero)
                {
                    StopTracking();
                    PinMenuItem.Visibility = Visibility.Visible;
                    UnpinMenuItem.Visibility = Visibility.Collapsed;
                }

                // 继续拖动逻辑
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
            var fadeOut = new DoubleAnimation
            {
                From = 1,
                To = 0,
                Duration = TimeSpan.FromSeconds(0.3)
            };
            fadeOut.Completed += (s, _) => Close();
            BeginAnimation(OpacityProperty, fadeOut);
        }

        private void EditBox_LostFocus(object sender, RoutedEventArgs e)
        {
            // 结束编辑
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

            // 添加淡出动画
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

            // 强制转移焦点到主窗口
            var mainWindow = Application.Current.MainWindow;
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
            }
        }

        private void UpdatePosition(double x, double y)
        {
            var parent = Parent as Canvas;
            if (parent == null) return;

            // 弹性边界
            double padding = 10; // 弹性距离
            x = Math.Max(-padding, Math.Min(x, parent.ActualWidth - this.ActualWidth + padding));
            y = Math.Max(-padding, Math.Min(y, parent.ActualHeight - this.ActualHeight + padding));

            // 更新位置
            Canvas.SetLeft(this, x);
            Canvas.SetTop(this, y);
        }
        private void PinToWindow_Click(object sender, RoutedEventArgs e)
        {
            // 使用预存的句柄
            IntPtr targetHandle = _preMenuTargetHandle;


            // 基础验证
            if (targetHandle == IntPtr.Zero)
            {
                MessageBox.Show("请先激活目标窗口，再右键固定！", "错误", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            // 排除自身窗口
            var wih = new WindowInteropHelper(this);
            if (targetHandle == wih.Handle)
            {
                MessageBox.Show("不能固定到自身窗口！", "错误", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            // 执行固定逻辑
            if (Win32ApiHelper.IsDesktopWindow(targetHandle))
            {
                PinToDesktop();
            }
            else
            {
                PinToWindow(targetHandle);
            }

            // 更新菜单状态
            PinMenuItem.Visibility = Visibility.Collapsed;
            UnpinMenuItem.Visibility = Visibility.Visible;
        }
        private void Unpin_Click(object sender, RoutedEventArgs e)
        {
            StopTracking();
            PinMenuItem.Visibility = Visibility.Visible;
            UnpinMenuItem.Visibility = Visibility.Collapsed;
            this.Topmost = true; // 恢复置顶
        }

        public void PinToDesktop()
        {
            this.Topmost = false;
            _targetWindowHandle = IntPtr.Zero;
            StopTracking(); // 确保定时器停止
        }

        private void PinToWindow(IntPtr targetHandle)
        {
            // 获取目标窗口的屏幕坐标（物理像素）
            Win32ApiHelper.GetWindowRect(targetHandle, out Win32ApiHelper.RECT targetRect);
            Point screenTargetPos = new Point(targetRect.Left, targetRect.Top);

            // 将目标窗口的屏幕坐标转换为当前窗口的WPF逻辑坐标（考虑DPI）
            Point wpfTargetPos = ConvertScreenToWpf(screenTargetPos);

            // 计算偏移量（使用当前窗口的Left/Top，已经是WPF逻辑坐标）
            _offsetFromTargetWindow = new Point(
                this.Left - wpfTargetPos.X,
                this.Top - wpfTargetPos.Y
            );

            _targetWindowHandle = targetHandle;

            // 初始化定时器
            _trackingTimer = new DispatcherTimer { Interval = TimeSpan.FromMilliseconds(100) };
            _trackingTimer.Tick += TrackingTimer_Tick;
            _trackingTimer.Start();
        }

        private void TrackingTimer_Tick(object sender, EventArgs e)
        {
            if (_targetWindowHandle == IntPtr.Zero || isDragging) return;

            // 检查目标窗口是否仍然有效
            if (!Win32ApiHelper.IsWindowValid(_targetWindowHandle))
            {
                this.Close(); // 关闭便签
                return;
            }

            // 原有位置更新逻辑
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

        // 在窗口关闭时停止定时器
        protected override void OnClosed(EventArgs e)
        {
            StopTracking();
            base.OnClosed(e);
        }
        public void PinToWindow(IntPtr targetHandle, double offsetX, double offsetY)
        {
            _targetWindowHandle = targetHandle;
            _offsetFromTargetWindow = new Point(offsetX, offsetY);

            // 确保定时器已停止并重新初始化
            if (_trackingTimer != null)
            {
                _trackingTimer.Stop();
                _trackingTimer = null;
            }

            _trackingTimer = new DispatcherTimer { Interval = TimeSpan.FromMilliseconds(100) };
            _trackingTimer.Tick += TrackingTimer_Tick;
            _trackingTimer.Start();

            // 更新菜单状态
            PinMenuItem.Visibility = Visibility.Collapsed;
            UnpinMenuItem.Visibility = Visibility.Visible;
        }

    }
}