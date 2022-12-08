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

    public void CreateImage(ImageEntity image) => Create(image);

    public void DeleteImage(ImageEntity image) => Delete(image);

    public void UpdateImage(ImageEntity image) => Update(image);

    async Task<ImageEntity> IImageRepository.GetImageByCarouselItemAsync(CarouselItemEntity carouselItem, bool trackChanges) =>
        await FindByCondition(image => image.CarouselItemId == carouselItem.Id, trackChanges)
        .SingleOrDefaultAsync();
}
