using System.Globalization;
using System.Windows.Data;
using System.Windows.Media;

namespace LineManagementSystem.Converters;

public class HexToColorConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        if (value is string hexColor && hexColor.StartsWith("#"))
        {
            try
            {
                return (Color)System.Windows.Media.ColorConverter.ConvertFromString(hexColor);
            }
            catch
            {
                return Colors.Gray;
            }
        }
        return Colors.Gray;
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}
