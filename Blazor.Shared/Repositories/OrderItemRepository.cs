using Blazor.Shared.Abstractions;
using Blazor.Shared.Entities.DbContexts;
using Blazor.Shared.Entities.Models;
using Blazor.Shared.Entities.RequestFeatures;
using Blazor.Shared.Implementations.Extensions;
using Microsoft.EntityFrameworkCore;

namespace Blazor.Shared.Implementations.Repositories;

public sealed class OrderItemRepository : RepositoryBase<OrderItemEntity>, IOrderItemRepository
{
    public OrderItemRepository(SqlContext context) : base(context)
    {
    }

    public async Task<PagedList<OrderItemEntity>> GetAllOrderItemsAsync(OrderItemParameters orderItemParameters, bool trackChanges)
    {
        var orderItems = await FindAll(trackChanges)
            .SearchOrderItems(orderItemParameters.SearchTerm)
            .SortOrderItems(orderItemParameters.OrderBy)
            .ToListAsync();
        
        return PagedList<OrderItemEntity>.ToPagedList(orderItems, orderItemParameters.PageNumber, orderItemParameters.PageSize);
    }

    public async Task<OrderItemEntity> GetOrderItemAsync(int orderItemId, bool trackChanges) => 
        await FindByCondition(orderItem => orderItem.Id == orderItemId, trackChanges)
            .SingleOrDefaultAsync();

    public void CreateOrderItem(OrderItemEntity orderItem) => Create(orderItem);

    public void UpdateOrderItem(OrderItemEntity orderItem) => Update(orderItem);

    public void DeleteOrderItem(OrderItemEntity orderItem) => Delete(orderItem);
}