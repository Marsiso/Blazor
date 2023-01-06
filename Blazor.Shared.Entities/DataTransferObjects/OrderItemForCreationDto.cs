using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Blazor.Shared.Entities.Models;

namespace Blazor.Shared.Entities.DataTransferObjects;

public sealed class OrderItemForCreationDto
{
    [Required(ErrorMessage = "Product item amount is a required field")]
    public int Amount { get; set; }
    
    [Required(ErrorMessage = "Product item price is a required field")]
    [Range(0, double.MaxValue, ErrorMessage = "Product item price must be ranging from {2} to {1}.")]
    public double Price { get; set; }
    
    [Required(ErrorMessage = "Product id is a required field")]
    public int ProductId { get; set; }
    
    [Required(ErrorMessage = "Order id is a required field")]
    public int OrderId { get; set; }
    
    public ProductForCreationDto Product { get; set; }
    public OrderForCreationDto Order { get; set; }
}