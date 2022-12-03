using Blazor.Shared.Abstractions;
using Blazor.Shared.Entities.DbContexts;
using Blazor.Shared.Entities.Models;
using Microsoft.EntityFrameworkCore;

namespace Blazor.Shared.Implementations.Repositories;

public class ImageRepository : RepositoryBase<ImageEntity>, IImageRepository
{
    public ImageRepository(SqlContext context) : base(context)
    {
    }

    async Task<ImageEntity> IImageRepository.GetImageByCarouselItemAsync(CarouselItemEntity carouselItem, bool trackChanges) =>
        await FindByCondition(image => image.CarouselItemId == carouselItem.Id, trackChanges)
        .SingleOrDefaultAsync();
}
