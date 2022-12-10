using Blazor.Shared.Abstractions;
using Blazor.Shared.Entities.DbContexts;
using Blazor.Shared.Entities.Models;
using Blazor.Shared.Entities.RequestFeatures;
using Microsoft.EntityFrameworkCore;

namespace Blazor.Shared.Implementations.Repositories;

public sealed class CarouselItemRepository : RepositoryBase<CarouselItemEntity>, ICarouselItemRepository
{
    public CarouselItemRepository(SqlContext context) : base(context)
    {
    }

    public void CreateCarouselItem(CarouselItemEntity carouselItem) => Create(carouselItem);

    public void DeleteCarouselItem(CarouselItemEntity carouselItem) => Delete(carouselItem);

    public async Task<PagedList<CarouselItemEntity>> GetAllCarouselItemsAsync(CarouselItemParameters carouselItemParameters, bool trackChanges)
    {
        var carouselItems = await FindAll(trackChanges)
            .OrderBy(ci => ci.Id)
            .ToListAsync();
        
        return PagedList<CarouselItemEntity>.ToPagedList(carouselItems, carouselItemParameters.PageNumber, carouselItemParameters.PageSize);
    }

    public async Task<CarouselItemEntity> GetCarouselItemAsync(int carouselItemId, bool trackChanges) =>
        await FindByCondition(ci => ci.Id == carouselItemId, trackChanges)
        .SingleOrDefaultAsync();

    public async Task<IEnumerable<CarouselItemEntity>> GetCarouselItemsByIds(IEnumerable<int> ids, bool trackChanges) =>
        await FindByCondition(ci => ids.Contains(ci.Id), trackChanges)
        .ToListAsync();

    public void UpdateCarouselItem(CarouselItemEntity carouselItem) => Update(carouselItem);
}
