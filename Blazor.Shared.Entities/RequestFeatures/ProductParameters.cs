namespace Blazor.Shared.Entities.RequestFeatures;

public sealed class ProductParameters : RequestParameters
{
    public double MinPrice { get; set; } = 0;
    public double MaxPrice { get; set; } = double.MaxValue;

    public bool ValidPriceRange => MinPrice < MaxPrice;
}