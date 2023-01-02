namespace Blazor.Shared.Abstractions;

public interface IRepositoryManager
{
    ICarouselItemRepository CarouselItem { get; }
    IImageRepository Image { get; }
    IUserRepository User { get; }
    Task SaveAsync();
}
