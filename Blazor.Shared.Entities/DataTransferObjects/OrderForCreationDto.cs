using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Blazor.Shared.Entities.DataTransferObjects;

public sealed class OrderForCreationDto
{
    [Required(ErrorMessage = "Order number is a required field")]
    public string OrderNumber { get; set; }

    [Required(ErrorMessage = "Order total price is a required field")]
    public double TotalPrice { get; set; }

    [Required(ErrorMessage = "User id a required field")]
    public int UserId { get; set; }
    
    public ICollection<OrderItemForCreationDto> OrderItems { get; set; }
}