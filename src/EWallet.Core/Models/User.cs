using System.ComponentModel.DataAnnotations;

namespace EWallet.Core.Models;

public class User
{
    public int Id { get; set; }

    [Required]
    [MaxLength(100)]
    public string Name { get; set; } = string.Empty;

    [Required]
    [EmailAddress]
    [MaxLength(100)]
    public string Email { get; set; } = string.Empty;

    [Required]
    [MaxLength(15)]
    public string Phone { get; set; } = string.Empty;

    public Wallet? Wallet { get; set; }
}