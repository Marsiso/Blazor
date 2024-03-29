﻿using Blazor.Shared.Entities.Models;
using Blazor.Shared.Entities.RequestFeatures;

namespace Blazor.Shared.Abstractions;

public interface ICarouselItemRepository
{
    Task<PagedList<CarouselItemEntity>> GetAllCarouselItemsAsync(CarouselItemParameters carouselItemParameters, bool trackChanges);
    Task<CarouselItemEntity> GetCarouselItemAsync(int carouselItemId, bool trackChanges);
    Task<IEnumerable<CarouselItemEntity>> GetCarouselItemsByIds(IEnumerable<int> ids, bool trackChanges);
    void CreateCarouselItem(CarouselItemEntity carouselItem);
    void UpdateCarouselItem(CarouselItemEntity carouselItem);
    void DeleteCarouselItem(CarouselItemEntity carouselItem);
}
