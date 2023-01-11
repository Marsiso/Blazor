using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Blazor.Shared.Entities.Validation;

namespace Blazor.Shared.Entities.DataTransferObjects;

public sealed class ResetPasswordForCreationDto
{
    [Required(ErrorMessage = "Password reset code is a required field")]
    [DataType(DataType.Text, ErrorMessage = "Invalid password reset code type")]
    public string Code { get; set; }

    [Required(ErrorMessage = " Password is a required field")]
    [StringLength(50, ErrorMessage = "Password must be between 5 and 50 characters", MinimumLength = 5)]
    [DataType(DataType.Password, ErrorMessage = "Invalid password format")]
    [RegularExpression(@"^.*(?=.*[A-Z])(?=.*[0-9])(?=.*[!@#$%^&*\(\)_\-+=]).*$", ErrorMessage = "Password must meet requirements")]
    public string Password { get; set; }
    
    [StringMatch(ComparisonProperty = nameof(Password), ErrorMessage = "Passwords doesn't match")]
    public string RepeatedPassword { get; set; }
}