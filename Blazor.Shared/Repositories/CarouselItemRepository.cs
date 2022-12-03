using Blazor.Shared.Abstractions;
using Blazor.Shared.Entities.DbContexts;
using Blazor.Shared.Entities.Models;
using Microsoft.EntityFrameworkCore;

namespace Blazor.Shared.Implementations.Repositories;

public sealed class CarouselItemRepository : RepositoryBase<CarouselItemEntity>, ICarouselItemRepository
{
    public CarouselItemRepository(SqlContext context) : base(context)
    {
    }

    public async Task<IEnumerable<CarouselItemEntity>> GetAllCarouselItemsAsync(bool trackChanges) =>
        await FindAll(trackChanges)
        .OrderBy(ci => ci.Id)
        .ToListAsync();

    public async Task<CarouselItemEntity> GetCarouselItemAsync(int carouselItemId, bool trackChanges) =>
        await FindByCondition(ci => ci.Id == carouselItemId, trackChanges)
        .SingleOrDefaultAsync();
}
