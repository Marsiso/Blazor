namespace Blazor.Shared.Abstractions;

public interface IRepositoryManager
{
    ICarouselItemRepository CarouselItem { get; }
    IImageRepository Image { get; }
    IUserRepository User { get; }
    IProductRepository Product { get; }
    IOrderItemRepository OrderItem { get; }
    IOrderRepository Order { get; }
    IResetPasswordRequestRepository ResetPasswordRequest { get; }
    Task SaveAsync();
}
