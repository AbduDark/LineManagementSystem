using System.ComponentModel.DataAnnotations;

namespace LineManagementSystem.Models;

public class PhoneLine
{
    [Key]
    public int Id { get; set; }

    [Required]
    [MaxLength(100)]
    public string Name { get; set; } = string.Empty;

    [Required]
    [MaxLength(14)]
    public string NationalId { get; set; } = string.Empty;

    [Required]
    [MaxLength(20)]
    public string PhoneNumber { get; set; } = string.Empty;

    [MaxLength(50)]
    public string InternalId { get; set; } = string.Empty;

    public bool HasCashWallet { get; set; }

    [MaxLength(50)]
    public string? CashWalletNumber { get; set; }

    public int GroupId { get; set; }
    public LineGroup? Group { get; set; }

    public DateTime CreatedAt { get; set; } = DateTime.Now;
    public DateTime UpdatedAt { get; set; } = DateTime.Now;
}
