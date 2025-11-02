namespace LineManagementSystem.Models;

public enum TelecomProvider
{
    Vodafone,
    Etisalat,
    We,
    Orange
}

public static class TelecomProviderExtensions
{
    public static string GetArabicName(this TelecomProvider provider)
    {
        return provider switch
        {
            TelecomProvider.Vodafone => "فودافون",
            TelecomProvider.Etisalat => "اتصالات",
            TelecomProvider.We => "وي",
            TelecomProvider.Orange => "أورانج",
            _ => provider.ToString()
        };
    }

    public static string GetColorHex(this TelecomProvider provider)
    {
        return provider switch
        {
            TelecomProvider.Vodafone => "#E60000",  // أحمر فودافون
            TelecomProvider.Etisalat => "#007E3A",  // أخضر اتصالات
            TelecomProvider.We => "#60269E", // بنفسجي وي
            TelecomProvider.Orange => "#FF7900", // برتقالي أورانج
            _ => "#000000"                          // افتراضي: أسود

        };
    }

    public static string GetLogoPath(this TelecomProvider provider)
    {
        return provider switch
        {
            TelecomProvider.Vodafone => "/Resources/Images/vodafone.png",
            TelecomProvider.Etisalat => "/Resources/Images/etisalat.png",
            TelecomProvider.We => "/Resources/Images/we.png",
            TelecomProvider.Orange => "/Resources/Images/orange.png",
            _ => ""
        };
    }
}