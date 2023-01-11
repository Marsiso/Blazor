namespace Blazor.Shared.Entities.DataTransferObjects;

public sealed class ProductModelDto
{
    public int ProductIdentifier { get; set; }
        
    public string ProductName { get; set; }
        
    public double ProductPrice { get; set; }
        
    public string ImageAlt { get; set; }
        
    public string ImageCaption { get; set; }
        
    public string ImageSource { get; set; }
}