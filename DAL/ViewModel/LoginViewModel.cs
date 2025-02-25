using System.ComponentModel.DataAnnotations;

namespace DAL.ViewModel;

public class LoginViewModel
{

    [Required]
    [EmailAddress(ErrorMessage = "Invalid email address.")]
    [RegularExpression(@"^[a-zA-Z0-9._%+-]+@[a-zA-Z0-9.-]+\.[a-zA-Z]{2,}$", ErrorMessage = "Please enter a valid email address.")]
    public string Email { get; set; } = null!;

    [Required]
    public string Password { get; set; } = null!;

    [Required]
    public bool RememberMe {get; set;}
}
