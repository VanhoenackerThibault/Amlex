using System.ComponentModel.DataAnnotations;

namespace Amlex.Models.Models.Authentication;

public class SignInRequest
{
    [Required(ErrorMessage = "Please enter your email address.")]
    [EmailAddress(ErrorMessage = "Please enter a valid email address.")]
    [StringLength(300, ErrorMessage = "Your email address cannot be longer than 300 characters.")]
    [Display(Name = "Email address")]
    public required string Email { get; set; }

    [Required(ErrorMessage = "Please enter your password.")]
    [DataType(DataType.Password)]
    [StringLength(100, ErrorMessage = "Your password cannot be longer than 100 characters.")]
    [Display(Name = "Password")]
    public required string Password { get; set; }
}