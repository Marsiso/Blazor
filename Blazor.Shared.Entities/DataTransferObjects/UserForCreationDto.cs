using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Blazor.Shared.Entities.Validation;

namespace Blazor.Shared.Entities.DataTransferObjects;

public sealed class UserForCreationDto
{
    [Required(ErrorMessage = "Email is a required field")]
    [MaxLength(50, ErrorMessage = "Maximum length for the email is 50 characters")]
    [DataType(DataType.EmailAddress, ErrorMessage = "Invalid email address format")]
    public string Email { get; set; }

    [Required(ErrorMessage = "First name is a required field")]
    [MaxLength(50, ErrorMessage = "Maximum length for the first name is 50 characters")]
    [DataType(DataType.Text, ErrorMessage = "First name must be text value type")]
    public string FirstName { get; set; }

    [Required(ErrorMessage = "Last name is a required field")]
    [MaxLength(50, ErrorMessage = "Maximum length for the last name is 50 characters")]
    [DataType(DataType.Text, ErrorMessage = "Last name must be text value type")]
    public string LastName { get; set; }

    [Required(ErrorMessage = " Password is a required field")]
    [StringLength(50, ErrorMessage = "Password must be between 5 and 50 characters", MinimumLength = 5)]
    [DataType(DataType.Password, ErrorMessage = "Invalid password format")]
    [RegularExpression(@"^.*(?=.*[A-Z])(?=.*[0-9])(?=.*[!@#$%^&*\(\)_\-+=]).*$", ErrorMessage = "Password must meet requirements")]
    public string Password { get; set; }
    
    [StringMatch(ComparisonProperty = nameof(Password), ErrorMessage = "Passwords doesn't match")]
    //[Compare(nameof(Password), ErrorMessage = "Passwords doesn't match")]
    public string RepeatedPassword { get; set; }

    [Required(ErrorMessage = "Address is a required field")]
    [MaxLength(100, ErrorMessage = "Maximum length for the address is 100 characters")]
    [DataType(DataType.Text, ErrorMessage = "Address must be text value type")]
    public string Address { get; set; }
    
    [Required(ErrorMessage = "Country is a required field")]
    [MaxLength(50, ErrorMessage = "Maximum length for the country is 50 characters")]
    [DataType(DataType.Text, ErrorMessage = "Country must be text value type")]
    public string Country { get; set; }
}