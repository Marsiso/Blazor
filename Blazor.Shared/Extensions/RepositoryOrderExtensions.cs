using System.Globalization;
using System.Linq.Dynamic.Core;
using Blazor.Shared.Entities.Models;
using Blazor.Shared.Implementations.Extensions.Utility;

namespace Blazor.Shared.Implementations.Extensions;

public static class RepositoryOrderExtensions
{
    public static IQueryable<OrderEntity> FilterOrders
    (
        this IQueryable<OrderEntity> orders, 
        double minTotalPrice, 
        double maxTotalPrice)
    {
        return orders.Where(order => order.TotalPrice >= minTotalPrice && order.TotalPrice <= maxTotalPrice);
    }

    public static IQueryable<OrderEntity> SearchOrders(this IQueryable<OrderEntity> orders, string searchTerm)
    {
        if (string.IsNullOrWhiteSpace(searchTerm))
        {
            return orders; 
        }

        var lowerCaseTerm = searchTerm.Trim().ToLower(); 
        return orders.Where(order => order.TotalPrice.ToString().ToLower().Contains(lowerCaseTerm) ||
                                     order.OrderNumber.Contains(lowerCaseTerm) ||
                                     order.Id.ToString().Contains(lowerCaseTerm));
    }

    public static IQueryable<OrderEntity> SortOrders(this IQueryable<OrderEntity> orders,
        string orderByQueryString)
    {
        if (string.IsNullOrWhiteSpace(orderByQueryString))
        {
            return orders.OrderBy(order => order.Id);
        }

        var orderQuery = OrderQueryBuilder.CreateOrderQuery<OrderEntity>(orderByQueryString);
        
        return string.IsNullOrWhiteSpace(orderQuery) 
            ? orders.OrderBy(order => order.Id) 
            : orders.OrderBy(orderQuery);
    }
}