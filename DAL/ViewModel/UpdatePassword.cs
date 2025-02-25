using System.ComponentModel.DataAnnotations;

namespace DAL.ViewModel;

public class UpdatePassword
{
    [Required]
    public string OldPassword {get; set;} = null!;

    [Required]
    public string NewPassword { get; set; } = null!;

    [Required]
    public string ConfirmNewPassword { get; set; } = null!;
}
