using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Blazor.Shared.Entities.Models;

[Table("Images")]
public sealed class ImageEntity
{
    [Column("pk_image"), Key]
    public int Id { get; set; }

    [Column("image_unsafe_name"), Required(ErrorMessage = "Untrusted image name is a required field")]
    [DataType(DataType.Text, ErrorMessage = "Untrusted image name must be text value type")]
    public string UnsafeName { get; set; }

    [Column("image_safe_name"), Required(ErrorMessage = "Trusted image name is a required field")]
    [DataType(DataType.Text, ErrorMessage = "Trusted image name must be text value type")]
    public string SafeName { get; set; }

    [Column("fk_carousel_item"), ForeignKey(nameof(CarouselItemEntity))]
    public int CarouselItemId { get; set; }

    public CarouselItemEntity CarouselItem { get; set; }
}
