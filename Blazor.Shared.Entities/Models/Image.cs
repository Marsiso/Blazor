using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Blazor.Shared.Entities.Models;

[Table("Images")]
public class Image
{
    [Column("pk_image"), Key]
    public int Id { get; set; }

    [Column("image_unsafe_name"), Required(ErrorMessage = "Image name is a required field")]
    [DataType(DataType.Text, ErrorMessage = "Image name must be text value type")]
    public string UnsafeName { get; set; }

    [Column("image_safe_name"), Required(ErrorMessage = "Image path is a required field")]
    [DataType(DataType.Text, ErrorMessage = "Image path name must be text value type")]
    public string SafeName { get; set; }

    [Column("fk_carousel_item"), ForeignKey(nameof(CarouselItem))]
    public int CarouselItemId { get; set; }

    public CarouselItem CarouselItem { get; set; }
}
