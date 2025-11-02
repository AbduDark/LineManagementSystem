using System.ComponentModel.DataAnnotations;

namespace LineManagementSystem.Models;

public class LineGroup
{
    [Key]
    public int Id { get; set; }

    [Required]
    [MaxLength(100)]
    public string Name { get; set; } = string.Empty;

    [Required]
    public TelecomProvider Provider { get; set; }

    [Required]
    public GroupStatus Status { get; set; } = GroupStatus.Active;

    public bool RequiresCashWallet { get; set; }

    public DateTime? LastRenewedOn { get; set; }
    
    public DateTime? NextRenewalDue { get; set; }

    [MaxLength(100)]
    public string? AssignedToEmployee { get; set; }

    [MaxLength(100)]
    public string? AssignedCustomer { get; set; }

    public DateTime? ExpectedHandoverDate { get; set; }

    public bool IsHandedOver { get; set; }

    public DateTime? ActualHandoverDate { get; set; }

    [MaxLength(1000)]
    public string? AdditionalDetails { get; set; }

    public bool IsBusinessGroup { get; set; } = false;

    public int MaxLines { get; set; } = 50;

    public List<PhoneLine> Lines { get; set; } = new();

    public DateTime CreatedAt { get; set; } = DateTime.Now;
    public DateTime UpdatedAt { get; set; } = DateTime.Now;

    public int GetLineCount => Lines?.Count ?? 0;

    public bool CanAddMoreLines => GetLineCount < MaxLines;

    public int GetRemainingDaysForRenewal
    {
        get
        {
            if (NextRenewalDue == null) return -1;
            return (NextRenewalDue.Value - DateTime.Now).Days;
        }
    }

    public bool NeedsRenewalAlert
    {
        get
        {
            var daysRemaining = GetRemainingDaysForRenewal;
            return daysRemaining >= 0 && daysRemaining <= 7;
        }
    }

    public bool IsRenewalExpired => GetRemainingDaysForRenewal < 0;

    public int GetDaysUntilHandover
    {
        get
        {
            if (ExpectedHandoverDate == null) return -1;
            return (ExpectedHandoverDate.Value - DateTime.Now).Days;
        }
    }

    public bool NeedsHandoverAlert
    {
        get
        {
            if (IsHandedOver || ExpectedHandoverDate == null) return false;
            var daysUntil = GetDaysUntilHandover;
            return daysUntil >= 0 && daysUntil <= 3;
        }
    }

    public bool IsHandoverOverdue
    {
        get
        {
            if (IsHandedOver || ExpectedHandoverDate == null) return false;
            return DateTime.Now > ExpectedHandoverDate.Value;
        }
    }

    public void RenewLicense()
    {
        LastRenewedOn = DateTime.Now;
        NextRenewalDue = DateTime.Now.AddDays(60);
        UpdatedAt = DateTime.Now;
    }
}
