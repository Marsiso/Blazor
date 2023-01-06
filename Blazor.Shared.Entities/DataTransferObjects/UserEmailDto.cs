using System.ComponentModel.DataAnnotations;

namespace Blazor.Shared.Entities.DataTransferObjects;

public class UserEmailDto
{
    [Required(ErrorMessage = "Email is a required field")]
    [MaxLength(50, ErrorMessage = "Maximum length for the email is 50 characters")]
    [DataType(DataType.EmailAddress, ErrorMessage = "Invalid email address format")]
    public string Email { get; set; }
}