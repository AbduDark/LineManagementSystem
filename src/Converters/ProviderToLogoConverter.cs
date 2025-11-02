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
                TelecomProvider.Vodafone => "/Resources/Images/vodafone.png",
                TelecomProvider.Orange => "/Resources/Images/orange.png",
                TelecomProvider.Etisalat => "/Resources/Images/etisalat.png",
                TelecomProvider.We => "/Resources/Images/we.png",
                _ => "/Resources/Images/vodafone.png"
            };
        }
        return "/Resources/Images/vodafone.png";
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}
