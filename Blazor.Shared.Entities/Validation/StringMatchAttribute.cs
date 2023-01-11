using System.ComponentModel.DataAnnotations;

namespace Blazor.Shared.Entities.Validation;

public class StringMatchAttribute : ValidationAttribute
{
    public string ComparisonProperty { get; set; }

    protected override ValidationResult IsValid(object value, ValidationContext validationContext)
    {
        if (value is not string givenString) return new ValidationResult("Given object type must be a string");
        
        var property = validationContext.ObjectType.GetProperty(ComparisonProperty);
        
        if (property is null) return new ValidationResult("Object does not contain comparison property");
        
        if (property.GetValue(validationContext.ObjectInstance) is string comparisonString)
        {
            return givenString.Equals(comparisonString) ? ValidationResult.Success : new ValidationResult("Given string and comparison string do not match");
        }
                
        return new ValidationResult("Comparison object type must be a string");
    }
}