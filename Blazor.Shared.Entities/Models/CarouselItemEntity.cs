using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Blazor.Shared.Entities.Models;

[Table("CarouselItems")]
public sealed class CarouselItemEntity
{
    [Column("pk_carousel_item"), Key]
    public int Id { get; set; }

    [Column("image_alt"), Required(ErrorMessage = "Image alternative name is a required field")]
    [MaxLength(50, ErrorMessage = "Maximum length for the image alternative name is 50 characters"), DataType(DataType.Text, ErrorMessage = "Image alternative name must be text value type")]
    public string Alt { get; set; }

    [Column("image_caption")]
    [MaxLength(50, ErrorMessage = "Maximum length for the image caption is 50 characters"), DataType(DataType.Text, ErrorMessage = "Image caption must be text value type")]
    public string Caption { get; set; }

    public ImageEntity Image { get; set; }
}
