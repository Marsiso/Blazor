using AutoMapper;
using Blazor.Shared.Entities.DataTransferObjects;
using Blazor.Shared.Entities.Models;

namespace Blazor.Shared.Entities.Mappings;

public class ResetPasswordRequestMappingProfile : Profile
{
    public ResetPasswordRequestMappingProfile()
    {
        CreateMap<ResetPasswordRequestEntity, ResetPasswordRequestDto>().ReverseMap();
    }
}