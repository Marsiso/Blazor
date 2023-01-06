using Blazor.Shared.Entities.Models;
using Blazor.Shared.Entities.RequestFeatures;

namespace Blazor.Shared.Abstractions;

public interface IOrderItemRepository
{
    Task<PagedList<OrderItemEntity>> GetAllOrderItemsAsync(OrderItemParameters orderItemParameters, bool trackChanges);
    Task<OrderItemEntity> GetOrderItemAsync(int orderItemId, bool trackChanges);
    void CreateOrderItem(OrderItemEntity orderItem);
    void UpdateOrderItem(OrderItemEntity orderItem);
    void DeleteOrderItem(OrderItemEntity orderItem);
}