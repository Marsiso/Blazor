using System.ComponentModel.DataAnnotations;

namespace Blazor.Shared.Entities.DataTransferObjects;

public sealed class OrderForUpdateDto
{
    [Required(ErrorMessage = "Order number is a required field")]
    public string OrderNumber { get; set; }

    [Required(ErrorMessage = "Order total price is a required field")]
    public double TotalPrice { get; set; }
    
    public ICollection<OrderItemForUpdateDto> OrderItems { get; set; }
}