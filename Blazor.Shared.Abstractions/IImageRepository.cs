using Blazor.Shared.Entities.Models;

namespace Blazor.Shared.Abstractions;

public interface IImageRepository
{
    Task<ImageEntity> GetImageByCarouselItemAsync(CarouselItemEntity carouselItem, bool trackChanges);
    void CreateImage(ImageEntity image);
    void UpdateImage(ImageEntity image);
    void DeleteImage(ImageEntity image);
}
