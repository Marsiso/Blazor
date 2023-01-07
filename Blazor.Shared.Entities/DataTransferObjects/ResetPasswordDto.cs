using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Blazor.Shared.Entities.Validation;

namespace Blazor.Shared.Entities.DataTransferObjects;

public sealed class ResetPasswordDto
{
    [Required(ErrorMessage = "Email is a required field")]
    [MaxLength(50, ErrorMessage = "Maximum length for the email is 50 characters")]
    [DataType(DataType.EmailAddress, ErrorMessage = "Invalid email address format")]
    public string Email { get; set; }
    
    [Required(ErrorMessage = "Password reset code is a required field")]
    [DataType(DataType.Text, ErrorMessage = "Invalid password reset code type")]
    public string Code { get; set; }

    [Required(ErrorMessage = "Image alternative name is a required field")]
    [MaxLength(50, ErrorMessage = "Maximum length for the image alternative name is 50 characters")]
    [DataType(DataType.Password, ErrorMessage = "Invalid password format")]
    public string Password { get; set; }
    
    [PasswordMatch(ComparisonProperty = nameof(Password), ErrorMessage = "Passwords do not match")]
    public string RepeatedPassword { get; set; }
}