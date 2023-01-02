using Blazor.Shared.Entities.DataTransferObjects;
using System.Net.Http.Json;
using Blazor.Shared.Entities.RequestFeatures;

namespace Blazor.Presentation.Client.Services;

public sealed class CarouselItemService
{
    readonly IHttpClientFactory _httpClientFactory;

    public CarouselItemService(IHttpClientFactory httpClientFactory)
    {
        _httpClientFactory = httpClientFactory;
    }

    public async ValueTask<IEnumerable<CarouselItemDto>> GetAllAsync(CarouselItemParameters _carouselItemParameters)
    {
        var httpClient = _httpClientFactory.CreateClient(nameof(CarouselItemService));
        var result = await httpClient.GetFromJsonAsync<IEnumerable<CarouselItemDto>>("api/CarouselItem");
        return result;
    }
}
