using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Blazor.Shared.Entities.Models;

[Table("OrderItems")]
public sealed class OrderItemEntity
{
    [Column("pk_order_item"), Key] 
    public int Id { get; set; }

    [Column("product_item_amount")]
    [Required(ErrorMessage = "Product item amount is a required field")]
    public int Amount { get; set; }
    
    [Column("product_item_price")]
    [Required(ErrorMessage = "Product item price is a required field")]
    [Range(0, double.MaxValue, ErrorMessage = "Product item price must be ranging from {2} to {1}.")]
    public double Price { get; set; }

    [ForeignKey(nameof(ProductEntity))]
    public int ProductId { get; set; }
    
    [ForeignKey(nameof(OrderEntity))]
    public int OrderId { get; set; }

    public ProductEntity Product { get; set; }
    public OrderEntity Order { get; set; }
}