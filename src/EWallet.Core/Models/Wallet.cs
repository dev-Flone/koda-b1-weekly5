using System.ComponentModel.DataAnnotations;
using System.Transactions;

namespace EWallet.Core.Models;

public class Wallet
{
    public int Id { get; set; }

    // FK
    public int UserId { get; set; }

    [Required]
    [MaxLength(20)]
    public string WalletNumber { get; set; } = string.Empty;

    public decimal Balance { get; set; } = 0m;

    // One-to-One relationship
    public User User { get; set; } = null!;

    // One-to-Many relationship
    public ICollection<Transaction> SentTransaction { get; set; } = [];

    public ICollection<Transaction> ReceiveTransaction { get; set; } = [];
}