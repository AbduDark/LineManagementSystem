namespace LineManagementSystem.Models;

public enum GroupStatus
{
    Active,
    Suspended,
    Barred,
    Cashless,
    CashWallet
}

public static class GroupStatusExtensions
{
    public static string GetArabicName(this GroupStatus status)
    {
        return status switch
        {
            GroupStatus.Active => "نشط",
            GroupStatus.Suspended => "موقوف",
            GroupStatus.Barred => "محظور",
            GroupStatus.Cashless => "بدون محفظة كاش",
            GroupStatus.CashWallet => "محفظة كاش",
            _ => status.ToString()
        };
    }

    public static string GetColorHex(this GroupStatus status)
    {
        return status switch
        {
            GroupStatus.Active => "#4CAF50",
            GroupStatus.Suspended => "#FFC107",
            GroupStatus.Barred => "#F44336",
            GroupStatus.Cashless => "#9E9E9E",
            GroupStatus.CashWallet => "#2196F3",
            _ => "#000000"
        };
    }
}
