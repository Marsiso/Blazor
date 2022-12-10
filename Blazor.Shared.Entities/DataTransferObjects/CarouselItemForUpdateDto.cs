using System.ComponentModel.DataAnnotations;
using Blazor.Shared.Entities.Models;

namespace Blazor.Shared.Entities.DataTransferObjects;

public sealed class CarouselItemForUpdateDto
{
    [Required(ErrorMessage = "Image alternative name is a required field")]
    [MaxLength(50, ErrorMessage = "Maximum length for the image alternative name is 50 characters")]
    [DataType(DataType.Text, ErrorMessage = "Image alternative name must be text value type")]
    public string Alt { get; set; }

    [MaxLength(50, ErrorMessage = "Maximum length for the image caption is 50 characters")]
    [DataType(DataType.Text, ErrorMessage = "Image caption must be text value type")]
    public string Caption { get; set; }
    
    public ImageForCreationDto Image { get; set; }
}