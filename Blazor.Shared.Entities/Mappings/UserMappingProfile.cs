using AutoMapper;
using Blazor.Shared.Entities.DataTransferObjects;
using Blazor.Shared.Entities.Models;

namespace Blazor.Shared.Entities.Mappings;

public sealed class UserMappingProfile : Profile
{
    public UserMappingProfile()
    {
        CreateMap<UserEntity, UserDto>().ReverseMap();
        CreateMap<UserForCreationDto, UserEntity>()
            .ForMember(dest => dest.Name, opt => opt.MapFrom(src => String.Join(' ', src.FirstName, src.LastName)));
        CreateMap<UserForUpdateDto, UserEntity>()
            .ForMember(dest => dest.Name, opt => opt.MapFrom(src => String.Join(' ', src.FirstName, src.LastName)));
    }
}