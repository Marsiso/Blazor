using System.Linq.Dynamic.Core;
using Blazor.Shared.Entities.Models;
using Blazor.Shared.Implementations.Extensions.Utility;

namespace Blazor.Shared.Implementations.Extensions;

public static class RepositoryProductExtensions
{
    public static IQueryable<ProductEntity> FilterProducts
    (
        this IQueryable<ProductEntity> products, 
        double minPrice, 
        double maxPrice)
    {
        return products.Where(product => product.Price >= minPrice && product.Price <= maxPrice);
    }

    public static IQueryable<ProductEntity> SearchProducts(this IQueryable<ProductEntity> products, string searchTerm)
    {
        if (string.IsNullOrWhiteSpace(searchTerm))
        {
            return products; 
        }

        var lowerCaseTerm = searchTerm.Trim().ToLower(); 
        return products.Where(product => product.Name.ToLower().Contains(lowerCaseTerm) || 
                                         product.Id.ToString().Contains(lowerCaseTerm));
    }

    public static IQueryable<ProductEntity> SortProducts(this IQueryable<ProductEntity> products,
        string orderByQueryString)
    {
        if (string.IsNullOrWhiteSpace(orderByQueryString))
        {
            return products.OrderBy(product => product.Id);
        }

        var orderQuery = OrderQueryBuilder.CreateOrderQuery<ProductEntity>(orderByQueryString);
        
        return string.IsNullOrWhiteSpace(orderQuery) 
            ? products.OrderBy(product => product.Id) 
            : products.OrderBy(orderQuery);
    }
}