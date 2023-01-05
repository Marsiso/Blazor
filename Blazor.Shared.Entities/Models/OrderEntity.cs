using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Blazor.Shared.Entities.Models;

[Table("Orders")]
public sealed class OrderEntity
{
    [Column("pk_order"), Key] 
    public int Id { get; set; }
    
    [DataType(DataType.Date, ErrorMessage = "Order date created is not a valid date")]
    public DateTime DateTimeCreated { get; set; }

    [Required(ErrorMessage = "Order number is a required field")]
    public string OrderNumber { get; set; }

    [Required(ErrorMessage = "Order total price is a required field")]
    public double TotalPrice { get; set; }

    [ForeignKey(nameof(User))]
    public int UserId { get; set; }
    public UserEntity User { get; set; }

    public ICollection<OrderItemEntity> OrderItems { get; set; }
}