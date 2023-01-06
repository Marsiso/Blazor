using Blazor.Shared.Entities.Models;
using Blazor.Shared.Entities.RequestFeatures;

namespace Blazor.Shared.Abstractions;

public interface IOrderRepository
{
    Task<PagedList<OrderEntity>> GetAllOrdersAsync(OrderParameters orderParameters, bool trackChanges);
    Task<OrderEntity> GetOrderAsync(int orderId, bool trackChanges);
    void CreateOrder(OrderEntity order);
    void UpdateOrder(OrderEntity order);
    void DeleteOrder(OrderEntity order);
}