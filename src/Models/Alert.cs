namespace LineManagementSystem.Models;

public enum AlertType
{
    RenewalNeeded,
    RenewalExpired,
    HandoverDue,
    HandoverOverdue
}

public class Alert
{
    public int Id { get; set; }
    public AlertType Type { get; set; }
    public string Message { get; set; } = string.Empty;
    public int GroupId { get; set; }
    public LineGroup? Group { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.Now;
    public bool IsRead { get; set; }

    public string GetArabicMessage()
    {
        if (Group == null) return Message;

        return Type switch
        {
            AlertType.RenewalNeeded => $"المجموعة '{Group.Name}' تحتاج تجديد خلال {Group.GetRemainingDaysForRenewal()} يوم",
            AlertType.RenewalExpired => $"صلاحية المجموعة '{Group.Name}' منتهية! يرجى التجديد فوراً",
            AlertType.HandoverDue => $"موعد تسليم المجموعة '{Group.Name}' للعميل '{Group.AssignedCustomer}' خلال {Group.GetDaysUntilHandover()} يوم",
            AlertType.HandoverOverdue => $"تأخر تسليم المجموعة '{Group.Name}' للعميل '{Group.AssignedCustomer}'!",
            _ => Message
        };
    }

    public string GetColorHex()
    {
        return Type switch
        {
            AlertType.RenewalNeeded => "#FFC107",
            AlertType.RenewalExpired => "#F44336",
            AlertType.HandoverDue => "#2196F3",
            AlertType.HandoverOverdue => "#F44336",
            _ => "#9E9E9E"
        };
    }

    public System.Windows.Media.Brush GetColorBrush()
    {
        var color = GetColorHex();
        return new System.Windows.Media.SolidColorBrush(
            (System.Windows.Media.Color)System.Windows.Media.ColorConverter.ConvertFromString(color)
        );
    }
}
