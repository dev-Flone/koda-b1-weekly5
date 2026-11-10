namespace EWallet.Core.DTOs.Responses;

public class WalletResponse
{
    public int Id { get; set; }
    public int UserId { get; set; }
    public string WalletNumber { get; set; } = string.Empty;
    public decimal Balance { get; set; }
}