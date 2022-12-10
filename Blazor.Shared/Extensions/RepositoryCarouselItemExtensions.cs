using Blazor.Shared.Entities.Models;

namespace Blazor.Shared.Implementations.Extensions;

public static class RepositoryCarouselItemExtensions
{
    public static IQueryable<CarouselItemEntity> FilterCarouselItems(
        this IQueryable<CarouselItemEntity> carouselItems, 
        int minId, 
        int maxId)
    {
        return carouselItems.Where(ci => ci.Id >= minId && ci.Id <= maxId);
    }

    public static IQueryable<CarouselItemEntity> SearchCarouselItems(this IQueryable<CarouselItemEntity> carouselItems, string searchTerm)
    {
        if (string.IsNullOrWhiteSpace(searchTerm))
        {
            return carouselItems; 
        }

        var lowerCaseTerm = searchTerm.Trim().ToLower(); 
        return carouselItems.Where(ci => ci.Alt.ToLower().Contains(lowerCaseTerm) || 
                                         ci.Caption.ToLower().Contains(lowerCaseTerm));
    }
}