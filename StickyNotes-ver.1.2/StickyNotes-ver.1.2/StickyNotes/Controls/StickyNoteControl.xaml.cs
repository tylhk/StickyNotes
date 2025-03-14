using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;

namespace StickyNotes.Controls
{
    public partial class StickyNoteControl : Window
    {
        private bool isDragging;
        private Point offset;

        public static readonly DependencyProperty NoteContentProperty =
            DependencyProperty.Register("NoteContent", typeof(string), typeof(StickyNoteControl));
        private DateTime _lastClickTime;
        private const int DoubleClickThreshold = 300; // 毫秒
        private Point _velocity;
        private DateTime _lastUpdateTime;
        private const double Friction = 0.9;


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
            this.DataContext = this;
        }
        private void OnSizeChanged(object sender, SizeChangedEventArgs e)
        {
            // 获取当前位置
            double currentX = Canvas.GetLeft(this);
            double currentY = Canvas.GetTop(this);

            // 更新位置以确保在边界内
            UpdatePosition(currentX, currentY);
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
                var parent = Parent as Canvas;
                var currentPosition = e.GetPosition(parent);

                // 计算新位置
                double newX = currentPosition.X - offset.X;
                double newY = currentPosition.Y - offset.Y;

                // 使用UpdatePosition更新位置
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
            this.Close(); // 直接关闭窗口
        }

        private void TextBlock_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            // 切换到编辑模式
            EditBox.Visibility = Visibility.Visible;
            ((TextBlock)sender).Visibility = Visibility.Collapsed;
            EditBox.Focus();
            EditBox.SelectAll();
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
    }
}