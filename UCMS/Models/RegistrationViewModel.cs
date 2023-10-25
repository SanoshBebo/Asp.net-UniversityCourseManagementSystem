// RegistrationViewModel.cs
using System.ComponentModel.DataAnnotations;

public class RegistrationViewModel
{
    [Required]
    public string Name { get; set; }

    [Required]
    [EmailAddress]
    public string Email { get; set; }

    [Required]
    [DataType(DataType.Password)]
    public string Password { get; set; }

    [Required]
    public string Role { get; set; } // "professor" or "student"
}
