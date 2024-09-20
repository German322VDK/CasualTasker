using System.Globalization;
using System.Windows.Data;
using System.Windows.Media;

namespace CasualTasker.Infrastructure.WPFExtension
{
    public class ColorToHexConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is string hex && !string.IsNullOrEmpty(hex))
            {
                try
                {
                    return (Color)ColorConverter.ConvertFromString(hex);
                }
                catch
                {
                    return Colors.Transparent;
                }
            }

            return Colors.Transparent;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is Color color)
            {
                string result = $"#{color.R:X2}{color.G:X2}{color.B:X2}";
                return result;
            }

            return null;
        }
    }
}
