using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using Color = System.Windows.Media.Color;

namespace StickyNotes
{
    public partial class ColorPickerDialog : Window
    {
        public Color SelectedColor { get; set; } = Colors.Yellow;
        public Color SelectedFontColor { get; set; } = Colors.Black;

        public ColorPickerDialog()
        {
            InitializeComponent();
            DataContext = this;
            RedSlider.Value = 255;
            GreenSlider.Value = 255;
            BlueSlider.Value = 0;
            AlphaSlider.Value = 255;
            FontRedSlider.Value = 0;
            FontGreenSlider.Value = 0;
            FontBlueSlider.Value = 0;
            UpdatePreviewColor();
        }

        public SolidColorBrush PreviewColor
        {
            get => (SolidColorBrush)GetValue(PreviewColorProperty);
            set => SetValue(PreviewColorProperty, value);
        }

        public static readonly DependencyProperty PreviewColorProperty =
            DependencyProperty.Register("PreviewColor", typeof(SolidColorBrush), typeof(ColorPickerDialog));

        public SolidColorBrush PreviewTextBrush
        {
            get => (SolidColorBrush)GetValue(PreviewTextBrushProperty);
            set => SetValue(PreviewTextBrushProperty, value);
        }

        public static readonly DependencyProperty PreviewTextBrushProperty =
            DependencyProperty.Register("PreviewTextBrush", typeof(SolidColorBrush), typeof(ColorPickerDialog));

        private void UpdatePreviewColor()
        {
            var bg = Color.FromArgb(
                (byte)AlphaSlider.Value,
                (byte)RedSlider.Value,
                (byte)GreenSlider.Value,
                (byte)BlueSlider.Value
            );
            PreviewColor = new SolidColorBrush(bg);

            var textColor = Color.FromRgb(
                (byte)FontRedSlider.Value,
                (byte)FontGreenSlider.Value,
                (byte)FontBlueSlider.Value
            );
            PreviewTextBrush = new SolidColorBrush(textColor);
        }

        private void OkButton_Click(object sender, RoutedEventArgs e)
        {
            SelectedColor = PreviewColor.Color;
            SelectedFontColor = PreviewTextBrush.Color;
            DialogResult = true;
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
        }
        private void Slider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            UpdatePreviewColor();
        }
        private void ColorBlock_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            var border = (Border)sender;
            var colorParts = border.Tag.ToString().Split(',');
            byte r = byte.Parse(colorParts[0]);
            byte g = byte.Parse(colorParts[1]);
            byte b = byte.Parse(colorParts[2]);

            AnimateSlider(RedSlider, r);
            AnimateSlider(GreenSlider, g);
            AnimateSlider(BlueSlider, b);
            AnimateSlider(AlphaSlider, 255);
        }

        private void AnimateSlider(Slider slider, double targetValue)
        {
            DoubleAnimation animation = new DoubleAnimation
            {
                To = targetValue,
                Duration = TimeSpan.FromMilliseconds(800),
                EasingFunction = new CubicEase { EasingMode = EasingMode.EaseOut }
            };
            slider.BeginAnimation(Slider.ValueProperty, animation);
        }
    }
}
