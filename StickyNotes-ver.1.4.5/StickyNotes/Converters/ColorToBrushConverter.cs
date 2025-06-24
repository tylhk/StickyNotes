using System.Windows.Data;
using System.Windows.Media;
using Color = System.Windows.Media.Color;

namespace StickyNotes.Converters
{
    public class ColorToBrushConverter : IValueConverter  // 必须为 public
    {
        public object Convert(object value, System.Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            return new SolidColorBrush((Color)value);
        }

        public object ConvertBack(object value, System.Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            return ((SolidColorBrush)value).Color;
        }
    }
}