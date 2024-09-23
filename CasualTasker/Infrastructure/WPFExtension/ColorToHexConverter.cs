using System.Globalization;
using System.Windows.Data;
using System.Windows.Media;

namespace CasualTasker.Infrastructure.WPFExtension
{
    /// <summary>
    /// Converts a color in hex format to a Color object and vice versa.
    /// </summary>
    public sealed class ColorToHexConverter : IValueConverter
    {
        /// <summary>
        /// Converts a hex string to a Color object.
        /// </summary>
        /// <param name="value">The hex string to convert.</param>
        /// <param name="targetType">The target type (not used in this conversion).</param>
        /// <param name="parameter">Additional parameters (not used).</param>
        /// <param name="culture">The culture information.</param>
        /// <returns>A Color object or Transparent if conversion fails.</returns>
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

        /// <summary>
        /// Converts a Color object back to a hex string.
        /// </summary>
        /// <param name="value">The Color object to convert.</param>
        /// <param name="targetType">The target type (not used in this conversion).</param>
        /// <param name="parameter">Additional parameters (not used).</param>
        /// <param name="culture">The culture information.</param>
        /// <returns>A hex string representing the Color or null if value is not a Color.</returns>
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
