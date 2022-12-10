using Blazor.Shared.Entities.Models;

namespace Blazor.Shared.Entities.RequestFeatures;

public sealed class CarouselItemParameters : RequestParameters
{
    public CarouselItemParameters()
    {
        OrderBy = "id";
    }
    public int MinId { get; set; } 
    public int MaxId { get; set; } = int.MaxValue;

    public bool ValidIdRange => MinId < MaxId;
}