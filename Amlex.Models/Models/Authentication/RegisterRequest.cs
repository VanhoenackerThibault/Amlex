using System.ComponentModel.DataAnnotations;

namespace Amlex.Models.Models.Authentication;

public class RegisterRequest
{
    [Required(ErrorMessage = "Please enter your email address.")]
    [EmailAddress(ErrorMessage = "Please enter a valid email address.")]
    [StringLength(300, ErrorMessage = "Your email address cannot be longer than 300 characters.")]
    [Display(Name = "Email address")]
    public required string Email { get; set; }

    [Required(ErrorMessage = "Please enter a password.")]
    [StringLength(100,MinimumLength = 8,ErrorMessage = "Your password must be between 8 and 100 characters long.")]
    [RegularExpression(@"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[^\w\s]).+$", ErrorMessage = "Your password must contain at least one uppercase letter, one lowercase letter, one number, and one special character.")]
    [DataType(DataType.Password)]
    [Display(Name = "Password")]
    public required string Password { get; set; }

    [Required(ErrorMessage = "Please confirm your password.")]
    [DataType(DataType.Password)]
    [Compare(nameof(Password), ErrorMessage = "The password and confirmation password do not match.")]
    [Display(Name = "Confirm password")]
    public required string ConfirmPassword { get; set; }
}