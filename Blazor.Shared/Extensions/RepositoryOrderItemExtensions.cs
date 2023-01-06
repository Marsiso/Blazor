using System.Globalization;
using System.Linq.Dynamic.Core;
using Blazor.Shared.Entities.Models;
using Blazor.Shared.Implementations.Extensions.Utility;

namespace Blazor.Shared.Implementations.Extensions;

public static class RepositoryOrderItemExtensions
{
    public static IQueryable<OrderItemEntity> SearchOrderItems(this IQueryable<OrderItemEntity> orderItems, string searchTerm)
    {
        if (string.IsNullOrWhiteSpace(searchTerm))
        {
            return orderItems; 
        }

        var lowerCaseTerm = searchTerm.Trim().ToLower(); 
        return orderItems.Where(orderItem => orderItem.Price.ToString().Contains(lowerCaseTerm) || 
                                             orderItem.Amount.ToString().Contains(lowerCaseTerm) || 
                                             orderItem.Id.ToString().Contains(lowerCaseTerm));
    }

    public static IQueryable<OrderItemEntity> SortOrderItems(this IQueryable<OrderItemEntity> orderItems,
        string orderByQueryString)
    {
        if (string.IsNullOrWhiteSpace(orderByQueryString))
        {
            return orderItems.OrderBy(orderItem => orderItem.Id);
        }

        var orderQuery = OrderQueryBuilder.CreateOrderQuery<OrderItemEntity>(orderByQueryString);
        
        return string.IsNullOrWhiteSpace(orderQuery) 
            ? orderItems.OrderBy(orderItem => orderItem.Id) 
            : orderItems.OrderBy(orderQuery);
    }
}