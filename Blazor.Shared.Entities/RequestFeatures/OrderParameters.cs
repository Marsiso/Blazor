namespace Blazor.Shared.Entities.RequestFeatures;

public sealed class OrderParameters : RequestParameters
{
    public double MinTotalPrice { get; set; } = 0;
    public double MaxTotalPrice { get; set; } = double.MaxValue;

    public bool ValidPriceRange => MinTotalPrice < MaxTotalPrice;
}