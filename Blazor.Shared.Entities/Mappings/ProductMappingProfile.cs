using AutoMapper;
using Blazor.Shared.Entities.DataTransferObjects;
using Blazor.Shared.Entities.Models;

namespace Blazor.Shared.Entities.Mappings;

public sealed class ProductMappingProfile : Profile
{
    public ProductMappingProfile()
    {
        CreateMap<ProductEntity, ProductDto>().ReverseMap();
        CreateMap<ProductForCreationDto, ProductEntity>().ReverseMap();
        CreateMap<ProductForUpdateDto, ProductEntity>().ReverseMap();
    }
}