using System.ComponentModel.DataAnnotations;

namespace EWallet.Mvc.ViewModel;

public class TopUpViewModel
{
    public int UserId { get; set; }

    [Required]
    [Range(typeof(decimal), "10000.00", "9999999999999.99", ParseLimitsInInvariantCulture = true, ErrorMessage = "Minimum Top up Rp. 10.000")]
    public decimal Amount { get; set; }

    [StringLength(255)]
    public string? Description { get; set; }

}