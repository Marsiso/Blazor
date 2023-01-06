using System.ComponentModel.DataAnnotations;

namespace Blazor.Shared.Entities.DataTransferObjects;

public sealed class ProductForCreationDto
{
    [Required(ErrorMessage = "Product name is a required field")]
    [MaxLength(100, ErrorMessage = "Maximum length for the product name is {1} characters"), DataType(DataType.Text, ErrorMessage = "Product name must be text value type")]
    public string Name { get; set; }
    
    [Required(ErrorMessage = "Product price is a required field")]
    [Range(0, double.MaxValue, ErrorMessage = "Product price must be ranging from {2} to {1}.")]
    public double Price { get; set; }

    public CarouselItemForCreationDto CarouselItem { get; set; }
}