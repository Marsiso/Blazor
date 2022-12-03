using Blazor.Shared.Entities.Models;

namespace Blazor.Shared.Abstractions;

public interface IImageRepository
{
    Task<ImageEntity> GetImageByCarouselItemAsync(CarouselItemEntity carouselItem, bool trackChanges);
}
