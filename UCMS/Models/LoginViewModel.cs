// LoginViewModel.cs
using System.ComponentModel.DataAnnotations;

public class LoginViewModel
{
    [Required]
    [EmailAddress]
    public string Email { get; set; }

    [Required]
    [Display(Name = "Password")]
    [DataType(DataType.Password)]
    [RegularExpression(@"^(?=.*\d)(?=.*\W).+$", ErrorMessage = "Password must contain at least one digit and one special character.")]
    [StringLength(100, ErrorMessage = "The {0} must be at least {2} and at most {1} characters long.", MinimumLength = 8)]
    public string Password { get; set; }

    [Display(Name = "Remember me")]
    public bool RememberMe { get; set; }
}
