using Blazor.Shared.Entities.Models;

namespace Blazor.Shared.Abstractions;

public interface ICarouselItemRepository
{
    Task<IEnumerable<CarouselItemEntity>> GetAllCarouselItemsAsync(bool trackChanges);
    Task<CarouselItemEntity> GetCarouselItemAsync(int carouselItemId, bool trackChanges);
    void CreateCarouselItem(CarouselItemEntity carouselItem);
    void UpdateCarouselItem(CarouselItemEntity carouselItem);
    void DeleteCarouselItem(CarouselItemEntity carouselItem);
}
