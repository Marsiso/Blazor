using System.ComponentModel.DataAnnotations;

namespace Blazor.Shared.Entities.DataTransferObjects;

public sealed class OrderItemForUpdateDto
{
    [Required(ErrorMessage = "Product item amount is a required field")]
    public int Amount { get; set; }
    
    [Required(ErrorMessage = "Product item price is a required field")]
    [Range(0, double.MaxValue, ErrorMessage = "Product item price must be ranging from {2} to {1}.")]
    public double Price { get; set; }
    
    public ProductForUpdateDto Product { get; set; }
    public OrderForUpdateDto Order { get; set; }
}