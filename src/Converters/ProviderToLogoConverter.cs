using System;
using System.Globalization;
using System.Windows.Data;
using LineManagementSystem.Models;

namespace LineManagementSystem.Converters;

public class ProviderToLogoConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        if (value is TelecomProvider provider)
        {
            return provider switch
            {
                TelecomProvider.Vodafone => "pack://application:,,,/Resources/Images/vodafone.png",
                TelecomProvider.Orange => "pack://application:,,,/Resources/Images/orange.png",
                TelecomProvider.Etisalat => "pack://application:,,,/Resources/Images/etisalat.png",
                TelecomProvider.We => "pack://application:,,,/Resources/Images/we.png",
                _ => "pack://application:,,,/Resources/Images/vodafone.png"
            };
        }
        return "pack://application:,,,/Resources/Images/vodafone.png";
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}
