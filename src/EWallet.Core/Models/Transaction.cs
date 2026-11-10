using System.ComponentModel.DataAnnotations;
using EWallet.Core.Enums;

namespace EWallet.Core.Models;

public class Transaction
{
    public int Id { get; set; }
    public int? SenderWalletId { get; set; }
    public int ReceiverWalletId { get; set; }
    public TransactionType Type { get; set; }
    public decimal Amount { get; set; }

    [MaxLength(250)]
    public string? Description { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public Wallet? SenderWallet { get; set; }
    public Wallet ReceiverWallet { get; set; } = null!;
}