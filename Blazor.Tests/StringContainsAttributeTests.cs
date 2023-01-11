using AutoMapper;
using Blazor.Shared.Entities.Validation;
using ValidationContext = System.ComponentModel.DataAnnotations.ValidationContext;

namespace Blazor.Tests;

public class StringContainsAttributeTests
{
    [Fact]
    public void GivenAStringProperty_WhenDoesContainValues_ThenTheAttributeReturnsExpectedResponse()
    {
        // Arrange
        var attribute = new StringContainsAttribute("STRING_0 STRING_1 STRING_2 STRING_3");
        var context = new ValidationContext(new { });
        
        // Act
        var validationResult = attribute.GetValidationResult("STRING_0 STRING_1 STRING_2 STRING_3", context);
        Assert.Null(validationResult?.ErrorMessage);
        
        validationResult = attribute.GetValidationResult("STRING_0STRING_1STRING_2STRING_3", context);
        Assert.Null(validationResult?.ErrorMessage);
        
        validationResult = attribute.GetValidationResult("STRING_0 STRING_1 STRING_2 STRING_3 STRING_4", context);
        Assert.Null(validationResult?.ErrorMessage);
    }
    
    [Fact]
    public void GivenAStringProperty_WhenDoesNotContainValues_ThenTheAttributeReturnsExpectedResponse()
    {
        // Arrange
        var attribute = new StringContainsAttribute("STRING_0 STRING_1 STRING_2 STRING_3 STRING_4");
        var context = new ValidationContext(new { });
        const string expectedErrorMessage = "Given object must contain string value STRING_4 at least once";
        
        // Act
        var validationResult = attribute.GetValidationResult("STRING_0 STRING_1 STRING_2 STRING_3", context);
        Assert.NotNull(validationResult?.ErrorMessage);
        Assert.Equal(expectedErrorMessage,validationResult?.ErrorMessage);
    }
    
    [Fact]
    public void GivenAProperty_WhenPropertyTypeIsNotString_ThenTheAttributeReturnsExpectedResponse()
    {
        // Arrange
        var attribute = new StringContainsAttribute("STRING_0 STRING_1 STRING_2 STRING_3 STRING_4");
        var context = new ValidationContext(new { });
        const string expectedErrorMessage = "Given object type must be a string";
        
        // Act
        var validationResult = attribute.GetValidationResult(1_000_000, context);     
        
        // Assert
        Assert.NotNull(validationResult?.ErrorMessage);
        Assert.Equal(expectedErrorMessage,validationResult.ErrorMessage);
    }
    
    [Fact]
    public void GivenAValuesProperty_WhenValuesPropertyIsAnEmptyString_ThenTheAttributeReturnsExpectedResponse()
    {
        // Arrange
        var attribute = new StringContainsAttribute(string.Empty);
        var context = new ValidationContext(new { });
        const string expectedErrorMessage = "Values object can not be an empty or null string";
        
        // Act
        void Action() => attribute.GetValidationResult("STRING_0 STRING_1 STRING_2 STRING_3", context);
        var exception = Record.Exception(Action);
        
        Assert.NotNull(exception);
        Assert.Equal(expectedErrorMessage, exception.Message);
    }
}