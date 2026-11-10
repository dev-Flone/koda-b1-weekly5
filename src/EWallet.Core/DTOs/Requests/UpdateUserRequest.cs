using System.ComponentModel.DataAnnotations;

namespace EWallet.Core.DTOs.Requests;

public class UpdateUserRequest
{
    [Required(ErrorMessage = "Name is requred.")]
    [StringLength(100, MinimumLength = 2)]
    public string Name { get; set; } = string.Empty;

    [Required(ErrorMessage = "Email is required.")]
    [EmailAddress(ErrorMessage = "Invalid email format.")]
    [StringLength(150)]
    public string Email { get; set; } = string.Empty;

    [Required(ErrorMessage = "Phone number is required.")]
    [StringLength(20)]
    public string Phone { get; set; } = string.Empty;
}
