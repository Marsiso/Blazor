using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Blazor.Shared.Entities.Models;

[Table("Orders")]
public sealed class OrderEntity
{
    [Column("pk_order"), Key] 
    public int Id { get; set; }
    
    [DatabaseGenerated(DatabaseGeneratedOption.Computed)]
    public DateTime DateTimeCreated { get; set; }

    [StringLength(25, ErrorMessage = "Total length of order number cannot exceed {1} characters.")]
    [Required(ErrorMessage = "Order number is a required field")]
    public string OrderNumber { get; set; }

    [Required(ErrorMessage = "Order total price is a required field")]
    public double TotalPrice { get; set; }

    [ForeignKey(nameof(User))]
    public int UserId { get; set; }
    public UserEntity User { get; set; }

    public ICollection<OrderItemEntity> OrderItems { get; set; }
}