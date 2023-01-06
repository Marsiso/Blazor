using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Blazor.Shared.Entities.DataTransferObjects;

public sealed class UserForUpdateDto
{
    [Required(ErrorMessage = "First name is a required field")]
    [MaxLength(50, ErrorMessage = "Maximum length for the first name is 50 characters")]
    [DataType(DataType.Text, ErrorMessage = "First name must be text value type")]
    public string FirstName { get; set; }

    [Required(ErrorMessage = "Last name is a required field")]
    [MaxLength(50, ErrorMessage = "Maximum length for the last name is 50 characters")]
    [DataType(DataType.Text, ErrorMessage = "Last name must be text value type")]
    public string LastName { get; set; }

    [Required(ErrorMessage = "Address is a required field")]
    [MaxLength(100, ErrorMessage = "Maximum length for the address is 100 characters")]
    [DataType(DataType.Text, ErrorMessage = "Address must be text value type")]
    public string Address { get; set; }
    
    [Required(ErrorMessage = "Country is a required field")]
    [MaxLength(50, ErrorMessage = "Maximum length for the country is 50 characters")]
    [DataType(DataType.Text, ErrorMessage = "Country must be text value type")]
    public string Country { get; set; }

    public string Password { get; set; }
}