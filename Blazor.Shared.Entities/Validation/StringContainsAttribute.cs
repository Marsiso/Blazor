using System.ComponentModel.DataAnnotations;

namespace Blazor.Shared.Entities.Validation;

public sealed class StringContainsAttribute : ValidationAttribute
{
    private IList<string> SearchTerms { get; set; }

    public StringContainsAttribute(string searchTerms)
    {
        if (String.IsNullOrEmpty(searchTerms))
        {
            SearchTerms = new List<string>();
        }
        else
        {
            SearchTerms = searchTerms.Split(' ');
        }
    }
    
    protected override ValidationResult IsValid(object value, ValidationContext validationContext)
    {
        var payload = value as string;
        if (String.IsNullOrEmpty(payload))
        {
            return new ValidationResult("Payload can't be null or empty string");
        }

        foreach (var searchTerm in SearchTerms)
        {
            if (!payload.Contains(searchTerm))
            {
                return new ValidationResult($"Payload must contain {searchTerm} attribute at least once");
            }
        }

        return ValidationResult.Success;
    }
}