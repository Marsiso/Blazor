using System.ComponentModel.DataAnnotations;
using Blazor.Shared.Entities.Validation;

namespace Blazor.Shared.Entities.DataTransferObjects;

public sealed class ResetPasswordEmailTemplateDto
{
    [Required(ErrorMessage = "Property {0} is required")]
    [StringLength(1000, ErrorMessage = "Minimal char lenght for {0} property is {2} and maximal {1}")]
    [StringContains("[RECIPIENT] [LINK]",
        ErrorMessage = "Template must contain at at least once following attributes: [LINK] and [RECIPIENT]")]
    [DataType(DataType.Text, ErrorMessage = "Property's {0} type must be text")]
    public string Payload { get; set; }
}