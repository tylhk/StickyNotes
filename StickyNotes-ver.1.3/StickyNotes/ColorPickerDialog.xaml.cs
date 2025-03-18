using System.Windows;
using System.Windows.Media;

namespace StickyNotes
{
    public partial class ColorPickerDialog : Window
    {
        public Color SelectedColor { get; set; } = Colors.Yellow;

        public ColorPickerDialog()
        {
            InitializeComponent();
            DataContext = this;
            RedSlider.Value = 255;
            GreenSlider.Value = 255;
            BlueSlider.Value = 0;
            UpdatePreviewColor();
        }

        public SolidColorBrush PreviewColor
        {
            get => (SolidColorBrush)GetValue(PreviewColorProperty);
            set => SetValue(PreviewColorProperty, value);
        }

        public static readonly DependencyProperty PreviewColorProperty =
            DependencyProperty.Register("PreviewColor", typeof(SolidColorBrush), typeof(ColorPickerDialog));

        private void UpdatePreviewColor()
        {
            var color = Color.FromRgb(
                (byte)RedSlider.Value,
                (byte)GreenSlider.Value,
                (byte)BlueSlider.Value
            );
            PreviewColor = new SolidColorBrush(color);
        }

        private void OkButton_Click(object sender, RoutedEventArgs e)
        {
            SelectedColor = PreviewColor.Color;
            DialogResult = true;
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
        }

        // 滑动条值改变时更新预览
        private void Slider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            UpdatePreviewColor();
        }
    }
}