using AutoMapper;
using Blazor.Shared.Entities.DataTransferObjects;
using Blazor.Shared.Entities.Models;

namespace Blazor.Shared.Entities.Mappings;

public sealed class OrderMappingProfile : Profile
{
    public OrderMappingProfile()
    {
        CreateMap<OrderEntity, OrderDto>().ReverseMap();
        CreateMap<OrderForCreationDto, OrderEntity>().ReverseMap();
        CreateMap<OrderForUpdateDto, OrderEntity>().ReverseMap();
    }
}