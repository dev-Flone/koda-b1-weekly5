namespace EWallet.Core.DTOs.Responses;

public class TransactionResponse
{
    public int Id { get; set; }
    public int? SenderWalletId { get; set; }
    public int ReceiverWalletId { get; set; }
    public string Type { get; set; } = string.Empty;
    public decimal Amount { get; set; }
    public string? Description { get; set; }
    public DateTime CreatedAt { get; set; }
}