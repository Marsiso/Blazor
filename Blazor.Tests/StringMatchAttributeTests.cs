using System.ComponentModel.DataAnnotations;
using Blazor.Shared.Entities.DataTransferObjects;
using Blazor.Shared.Entities.Validation;

namespace Blazor.Tests;

public class StringMatchAttributeTests
{
    private record struct TestStringModel(string GivenString, string ComparisonString);
    private record struct TestInvalidTypeModel(string StringProperty, int IntegerProperty);
    
    [Fact]
    public void GivenAStringProperty_WhenDoesNotMatchComparisonProperty_ThenTheAttributeReturnsExpectedResponse()
    {
        // Arrange
        var attribute = new StringMatchAttribute { ComparisonProperty = nameof(TestStringModel.ComparisonString) };
        var model = new TestStringModel { GivenString = "STRING", ComparisonString = "STRING_STRING" };
        var context = new ValidationContext(model);
        const string expectedErrorMessage = "Given string and comparison string do not match";
        
        // Act
        var validationResult = attribute.GetValidationResult(model.GivenString, context);     
        
        // Assert
        Assert.NotNull(validationResult?.ErrorMessage);
        Assert.Equal(expectedErrorMessage, validationResult.ErrorMessage);
    }
    
    [Fact]
    public void GivenAStringProperty_WhenDoesMatchComparisonProperty_ThenTheAttributeReturnsExpectedResponse()
    {
        // Arrange
        var attribute = new StringMatchAttribute { ComparisonProperty = nameof(TestStringModel.ComparisonString) };
        var model = new TestStringModel{ GivenString = "STRING", ComparisonString = "STRING" };
        var context = new ValidationContext(model);
        
        // Act
        var validationResult = attribute.GetValidationResult(model.GivenString, context);     
        
        // Assert
        Assert.Null(validationResult?.ErrorMessage);
    }
    
    [Fact]
    public void GivenAComparisonProperty_WhenComparisonPropertyDoesNotExist_ThenTheAttributeReturnsExpectedResponse()
    {
        // Arrange
        var attribute = new StringMatchAttribute { ComparisonProperty = "OtherString" };
        var model = new TestStringModel{ GivenString = "STRING", ComparisonString = "STRING" };
        var context = new ValidationContext(model);
        const string expectedErrorMessage = "Object does not contain comparison property";
        
        // Act
        var validationResult = attribute.GetValidationResult(model.GivenString, context);     
        
        // Assert
        Assert.NotNull(validationResult?.ErrorMessage);
        Assert.Equal(expectedErrorMessage,validationResult.ErrorMessage);
    }
    
    [Fact]
    public void GivenAComparisonProperty_WhenComparisonPropertyTypeIsNotString_ThenTheAttributeReturnsExpectedResponse()
    {
        // Arrange
        var attribute = new StringMatchAttribute { ComparisonProperty = nameof(TestInvalidTypeModel.IntegerProperty) };
        var model = new TestInvalidTypeModel { StringProperty = "STRING", IntegerProperty = 0 };
        var context = new ValidationContext(model);
        const string expectedErrorMessage = "Comparison object type must be a string";
        
        // Act
        var validationResult = attribute.GetValidationResult(model.StringProperty, context);     
        
        // Assert
        Assert.NotNull(validationResult?.ErrorMessage);
        Assert.Equal(expectedErrorMessage,validationResult.ErrorMessage);
    }
    
    [Fact]
    public void GivenAProperty_WhenPropertyTypeIsNotString_ThenTheAttributeReturnsExpectedResponse()
    {
        // Arrange
        var attribute = new StringMatchAttribute { ComparisonProperty = nameof(TestInvalidTypeModel.StringProperty) };
        var model = new TestInvalidTypeModel { StringProperty = "STRING", IntegerProperty = 0 };
        var context = new ValidationContext(model);
        const string expectedErrorMessage = "Given object type must be a string";
        
        // Act
        var validationResult = attribute.GetValidationResult(model.IntegerProperty, context);     
        
        // Assert
        Assert.NotNull(validationResult?.ErrorMessage);
        Assert.Equal(expectedErrorMessage,validationResult.ErrorMessage);
    }
}