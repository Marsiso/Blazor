using AutoMapper;
using Blazor.Shared.Entities.DataTransferObjects;
using Blazor.Shared.Entities.Models;

namespace Blazor.Shared.Entities.Mappings;

public sealed class OrderItemMappingProfile : Profile
{
    public OrderItemMappingProfile()
    {
        CreateMap<OrderItemEntity, OrderItemDto>().ReverseMap();
        CreateMap<OrderItemForCreationDto, OrderItemEntity>().ReverseMap();
        CreateMap<OrderItemForUpdateDto, OrderItemEntity>().ReverseMap();
    }
}