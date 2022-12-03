namespace Blazor.Shared.Abstractions;

public interface IRepositoryManager
{
    ICarouselItemRepository CarouselItem { get; }
    IImageRepository Image { get; }
    Task SaveAsync();
}
