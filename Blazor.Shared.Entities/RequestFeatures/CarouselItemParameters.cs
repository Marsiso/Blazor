using Blazor.Shared.Entities.Models;

namespace Blazor.Shared.Entities.RequestFeatures;

public sealed class CarouselItemParameters : RequestParameters, ICloneable
{
    public CarouselItemParameters()
    {
        OrderBy = "id";
    }
    public int MinId { get; set; } 
    public int MaxId { get; set; } = int.MaxValue;

    public bool ValidIdRange => MinId < MaxId;
    public object Clone()
    {
        return this.MemberwiseClone();
    }
}