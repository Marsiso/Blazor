using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Blazor.Shared.Entities.Models;

[Table("CarouselItems")]
public sealed class CarouselItemEntity
{
    [Column("pk_carousel_item"), Key]
    public int Id { get; set; }

    [Column("image_alt"), Required(ErrorMessage = "Image alternative name is a required field")]
    [MaxLength(100, ErrorMessage = "Maximum length for the image alternative name is {1} characters"), DataType(DataType.Text, ErrorMessage = "Image alternative name must be text value type")]
    public string Alt { get; set; }

    [Column("image_caption")]
    [MaxLength(600, ErrorMessage = "Maximum length for the image caption is {1} characters"), DataType(DataType.Text, ErrorMessage = "Image caption must be text value type")]
    public string Caption { get; set; }

    public ImageEntity Image { get; set; }
    public ProductEntity Product { get; set; }
}
