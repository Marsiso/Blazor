using Microsoft.AspNetCore.Http;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Blazor.Shared.Entities.DataTransferObjects;

public sealed class ImageForCreationDto
{
    [Column("image_safe_name"), Required(ErrorMessage = "Image path is a required field")]
    [DataType(DataType.Text, ErrorMessage = "Image path name must be text value type")]
    public string FileName { get; set; }

    [Required(ErrorMessage = "Please select file.")]
    public IFormFile ImageFile { get; set; }
}
