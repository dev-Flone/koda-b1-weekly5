using System.ComponentModel.DataAnnotations;

namespace EWallet.Mvc.ViewModel;

public class TransferViewModel
{
    public int SenderUserId { get; set; }

    [Range(1, int.MaxValue, ErrorMessage = "Please select a recipient.")]
    public int ReceiverUserId { get; set; }

    [Range(typeof(decimal), "10000.00", "9999999999999.99", ParseLimitsInInvariantCulture = true, ErrorMessage = "Minimun Transfer Rp. 10.000")]
    public decimal Amount { get; set; }

    [StringLength(250)]
    public string? Description { get; set; }
}