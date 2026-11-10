using System.ComponentModel.DataAnnotations;

namespace EWallet.Core.DTOs.Requests;

public class TopUpRequest
{
    [Required]
    [Range(typeof(decimal), "0.01", "999999999999.99", ParseLimitsInInvariantCulture = true, ErrorMessage = "Amount must be greater than 0.")]
    public decimal Amount { get; set; }

    [StringLength(250)]
    public string? Description { get; set; }
}