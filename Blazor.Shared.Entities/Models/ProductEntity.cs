using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Blazor.Shared.Entities.Models;

[Table("Products")]
public sealed class ProductEntity
{
    [Column("pk_product"), Key] 
    public int Id { get; set; }

    [Column("product_name")]
    [Required(ErrorMessage = "Product name is a required field")]
    [MaxLength(100, ErrorMessage = "Maximum length for the product name is {1} characters")]
    [DataType(DataType.Text, ErrorMessage = "Product name must be text value type")]
    public string Name { get; set; }
    
    [Column("product_price"), Required(ErrorMessage = "Product price is a required field")]
    [Range(0, double.MaxValue, ErrorMessage = "Product price must be ranging from {2} to {1}.")]
    public double Price { get; set; }

    [Column("fk_carousel_item")]
    [ForeignKey(nameof(CarouselItemEntity))]
    public int CarouselItemId { get; set; }

    public CarouselItemEntity CarouselItem { get; set; }
    public ICollection<OrderItemEntity> OrderItems { get; set; }
}