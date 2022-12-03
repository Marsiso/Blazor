namespace Blazor.Shared.Entities.DataTransferObjects;

public sealed class ImageDto
{
    public int Id { get; set; }
    public string UnsafeName { get; set; }
    public string Src { get; set; }
    public int CarouselItemId { get; set; }
}
