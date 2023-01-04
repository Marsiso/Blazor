using System.Net.Http.Headers;
using System.Net.Http.Json;
using Blazor.Shared.Entities.Constants;
using Blazor.Shared.Entities.DataTransferObjects;
using Polly;

namespace Blazor.Presentation.Client.Services;

public sealed class ImageService
{
    private readonly IHttpClientFactory _httpClientFactory;

    public ImageService(IHttpClientFactory httpClientFactory)
    {
        _httpClientFactory = httpClientFactory;
    }

    public async Task<PolicyResult<ImageDto>> GetImageAsync(int carouselItemId)
    {
        var httpClient = _httpClientFactory.CreateClient("Default");
        httpClient.DefaultRequestHeaders.Accept.Clear();
        httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
        var retryPolicy = Policy<ImageDto>
            .Handle<HttpRequestException>()
            .WaitAndRetryAsync(Constants.MaxHttpRequestRetries,times => TimeSpan.FromMilliseconds(times * 100));
        
        return await retryPolicy.ExecuteAndCaptureAsync(async () => await httpClient.GetFromJsonAsync<ImageDto>(
            $"CarouselItem/{carouselItemId}/Image"));
    }
}