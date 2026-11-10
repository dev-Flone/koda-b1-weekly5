using System.ComponentModel.DataAnnotations;

namespace EWallet.Core.DTOs.Requests;

public class TransferRequest
{
    [Required]
    [Range(1, int.MaxValue, ErrorMessage = "Sender user ID must be valid.")]
    public int SenderUserId { get; set; }

    [Required]
    [Range(1, int.MaxValue, ErrorMessage = "Receiver user ID must be valid.")]
    public int ReceiverUserId { get; set; }

    [Required]
    [Range(typeof(decimal), "0.01", "9999999999999.99", ParseLimitsInInvariantCulture = true, ErrorMessage = "Amount must be greater than 0.")]
    public decimal Amount { get; set; }

    [StringLength(250)]
    public string? Description { get; set; }
}