namespace Blazor.Shared.Entities.DataTransferObjects;

public sealed class ProductDto
{
    public int Id { get; set; }
    
    public string Name { get; set; }
    
    public double Price { get; set; }
    
    public int CarouselItemId { get; set; }
}