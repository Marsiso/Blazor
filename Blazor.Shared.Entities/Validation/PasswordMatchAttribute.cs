using System.ComponentModel.DataAnnotations;

namespace Blazor.Shared.Entities.Validation;

public class PasswordMatchAttribute : ValidationAttribute
{
    public string ComparisonProperty { get; set; }

    protected override ValidationResult IsValid(object value, ValidationContext validationContext)
    {
        var property = validationContext.ObjectType.GetProperty(ComparisonProperty);
        if (property is null)
        {
            return new ValidationResult("Matching property not found for proper comparison");
        }

        return (string)property.GetValue(validationContext.ObjectInstance) == value?.ToString() 
            ? ValidationResult.Success 
            : new ValidationResult("Passwords does not match");
    }
}