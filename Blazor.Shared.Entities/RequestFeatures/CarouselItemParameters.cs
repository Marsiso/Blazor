namespace Blazor.Shared.Entities.RequestFeatures;

public sealed class CarouselItemParameters : RequestParameters
{
    public int MinId { get; set; } 
    public int MaxId { get; set; } = int.MaxValue;

    public bool ValidIdRange => MinId < MaxId;
}