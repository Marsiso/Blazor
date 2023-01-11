using System.ComponentModel.DataAnnotations;

namespace Blazor.Shared.Entities.Validation;

public sealed class StringContainsAttribute : ValidationAttribute
{
    public IList<string> Values { get; set; }

    public StringContainsAttribute(string values)
    {
        if (string.IsNullOrEmpty(values))
        {
            throw new Exception("Values object can not be an empty or null string");
        }

        Values = values.Split(' ');
    }
    
    protected override ValidationResult IsValid(object value, ValidationContext validationContext)
    {
        if (value is not string givenString) return new ValidationResult("Given object type must be a string");
        if (string.IsNullOrEmpty(givenString)) return new ValidationResult("Given string object can not be null or empty string");
        foreach (var val in Values)
            if (givenString.Contains(val) is false)
            {
                return new ValidationResult(string.Format("Given object must contain string value {0} at least once", val));
            }

        return ValidationResult.Success;
    }
}