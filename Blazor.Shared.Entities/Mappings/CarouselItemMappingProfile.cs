using AutoMapper;
using Blazor.Shared.Entities.DataTransferObjects;
using Blazor.Shared.Entities.Models;

namespace Blazor.Shared.Entities.Mappings;

public sealed class CarouselItemMappingProfile : Profile
{
    public CarouselItemMappingProfile()
    {
        CreateMap<CarouselItemEntity, CarouselItemDto>().ReverseMap();
        CreateMap<CarouselItemForCreationDto, CarouselItemEntity>().ReverseMap();
    }
}
