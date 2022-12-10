using Blazor.Shared.Abstractions;
using Blazor.Shared.Entities.DbContexts;

namespace Blazor.Shared.Implementations.Repositories;

public sealed class RepositoryManager : IRepositoryManager
{
    private readonly SqlContext _context;
    private ICarouselItemRepository _carouselItem;
    private IImageRepository _image;

    public RepositoryManager(SqlContext context)
    {
        _context = context;
    }

    public ICarouselItemRepository CarouselItem => _carouselItem ??= new CarouselItemRepository(_context);

    public IImageRepository Image => _image ??= new ImageRepository(_context);

    public async Task SaveAsync() => await _context.SaveChangesAsync();
}
