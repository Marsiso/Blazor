using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Http;

namespace Blazor.Shared.Entities.DataTransferObjects;

public sealed class ProductModelForCreationDto
{
    [Required(ErrorMessage = "Product name is a required field")]
    [MaxLength(100, ErrorMessage = "Maximum length for the product name is {1} characters")]
    [DataType(DataType.Text, ErrorMessage = "Product name must be text value type")]
    public string ProductName { get; set; }
        
    [Required(ErrorMessage = "Product price is a required field")]
    [Range(0, 1_000_000, ErrorMessage = "Product price must be ranging from {1} to {2}.")]
    public double ProductPrice { get; set; }
        
    [Required(ErrorMessage = "Image alternative name is a required field")]
    [MaxLength(100, ErrorMessage = "Maximum length for the image alternative name is {1} characters")]
    [DataType(DataType.Text, ErrorMessage = "Image alternative name must be text value type")]
    public string ImageAlt { get; set; }
        
    [MaxLength(600, ErrorMessage = "Maximum length for the image caption is {1} characters")]
    [DataType(DataType.Text, ErrorMessage = "Image caption must be text value type")]
    public string ImageCaption { get; set; }

    [Required(ErrorMessage = "Image file is a required field")]
    public IFormFile Image { get; set; }
}