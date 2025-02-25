using System.ComponentModel.DataAnnotations;

namespace DAL.ViewModel;

public class ResetPasswordModel
{

    [Required]
    [EmailAddress]
    [RegularExpression(@"^[a-zA-Z0-9._%+-]+@[a-zA-Z0-9.-]+\.[a-zA-Z]{2,}$", ErrorMessage = "Please enter a valid email address.")]
    public string Email { get; set; } = null!;

    [Required]
    public string NewPassword { get; set; } = null!;

    [Required]
    public string ConfirmNewPassword { get; set; } = null!;

}
