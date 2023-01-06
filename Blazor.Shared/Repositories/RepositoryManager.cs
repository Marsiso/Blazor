using Blazor.Shared.Abstractions;
using Blazor.Shared.Entities.DbContexts;

namespace Blazor.Shared.Implementations.Repositories;

public sealed class RepositoryManager : IRepositoryManager
{
    private readonly SqlContext _context;
    private ICarouselItemRepository _carouselItem;
    private IImageRepository _image;
    private IUserRepository _user;
    private IProductRepository _product;
    private IOrderItemRepository _orderItem;
    private IOrderRepository _order;

    public RepositoryManager(SqlContext context)
    {
        _context = context;
    }

    public ICarouselItemRepository CarouselItem => _carouselItem ??= new CarouselItemRepository(_context);

    public IImageRepository Image => _image ??= new ImageRepository(_context);
    public IUserRepository User => _user ??= new UserRepository(_context);
    public IProductRepository Product => _product ??= new ProductRepository(_context);
    public IOrderItemRepository OrderItem => _orderItem ??= new OrderItemRepository(_context);
    public IOrderRepository Order => _order ??= new OrderRepository(_context);

    public async Task SaveAsync() => await _context.SaveChangesAsync();
}
