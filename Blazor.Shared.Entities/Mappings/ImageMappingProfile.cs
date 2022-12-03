using AutoMapper;
using Blazor.Shared.Entities.DataTransferObjects;
using Blazor.Shared.Entities.Models;

namespace Blazor.Shared.Entities.Mappings;

public sealed class ImageMappingProfile : Profile
{
    public ImageMappingProfile()
    {
        CreateMap<ImageEntity, ImageDto>().ReverseMap();
    }
}
