using Blazor.Shared.Abstractions;
using Blazor.Shared.Entities.DbContexts;
using Blazor.Shared.Entities.Models;
using Blazor.Shared.Entities.RequestFeatures;
using Blazor.Shared.Implementations.Extensions;
using Microsoft.EntityFrameworkCore;

namespace Blazor.Shared.Implementations.Repositories;

public sealed class OrderRepository : RepositoryBase<OrderEntity>, IOrderRepository
{
    public OrderRepository(SqlContext context) : base(context)
    {
    }

    public async Task<PagedList<OrderEntity>> GetAllOrdersAsync(OrderParameters orderParameters, bool trackChanges)
    {
        var products = await FindAll(trackChanges)
            .FilterOrders(orderParameters.MinTotalPrice, orderParameters.MaxTotalPrice)
            .SearchOrders(orderParameters.SearchTerm)
            .SortOrders(orderParameters.OrderBy)
            .ToListAsync();
        
        return PagedList<OrderEntity>.ToPagedList(products, orderParameters.PageNumber, orderParameters.PageSize);
    }

    public async Task<OrderEntity> GetOrderAsync(int orderId, bool trackChanges)  =>
        await FindByCondition(order => order.Id == orderId, trackChanges)
            .SingleOrDefaultAsync();

    public void CreateOrder(OrderEntity order) => Create(order);

    public void UpdateOrder(OrderEntity order) => Update(order);

    public void DeleteOrder(OrderEntity order) => Delete(order);
}