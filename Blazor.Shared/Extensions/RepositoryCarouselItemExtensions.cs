using System.Linq.Dynamic.Core;
using System.Reflection;
using System.Text;
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

    public static IQueryable<CarouselItemEntity> SortCarouselItems(this IQueryable<CarouselItemEntity> carouselItems,
        string orderByQueryString)
    {
        if (string.IsNullOrWhiteSpace(orderByQueryString))
        {
            return carouselItems.OrderBy(ci => ci.Id);
        }

        var orderParams = orderByQueryString.Trim().Split(',');
        var propertyInfos = typeof(CarouselItemEntity).GetProperties(BindingFlags.Public | BindingFlags.Instance);
        var orderQueryBuilder = new StringBuilder();
        foreach (var param in orderParams)
        {
            if (string.IsNullOrWhiteSpace(param))
            {
                continue;
            }

            var propertyFromQueryName = param.Split(" ")[0];
            var objectProperty = propertyInfos.FirstOrDefault(pi =>
                pi.Name.Equals(propertyFromQueryName, StringComparison.InvariantCultureIgnoreCase));
            if (objectProperty == null)
            {
                continue;
            }

            var direction = param.EndsWith(" desc") ? "descending" : "ascending";
            orderQueryBuilder.Append($"{objectProperty.Name} {direction}, ");
        }

        var orderQuery = orderQueryBuilder.ToString().TrimEnd(',', ' ');
        
        return string.IsNullOrWhiteSpace(orderQuery) 
            ? carouselItems.OrderBy(ci => ci.Id) 
            : carouselItems.OrderBy(orderQuery);
    }
}