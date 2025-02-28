using System.ComponentModel.DataAnnotations;

namespace DAL.ViewModel;

public class UpdatePassword
{
    [Required]
    public string OldPassword {get; set;} = null!;

    [Required]
    [RegularExpression(@"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[^\da-zA-Z]).{8,15}$", 
     ErrorMessage = "Password must contain one uppercase letter, one lowercase letter, one digit and one special character.")]
    public string NewPassword { get; set; } = null!;

    [Required]
    [RegularExpression(@"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[^\da-zA-Z]).{8,15}$", 
     ErrorMessage = "Password must contain one uppercase letter, one lowercase letter, one digit and one special character.")]
    public string ConfirmNewPassword { get; set; } = null!;
}
