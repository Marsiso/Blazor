using System.Net.Http.Headers;
using System.Net.Http.Json;
using Blazor.Shared.Entities.Constants;
using Blazor.Shared.Entities.DataTransferObjects;
using Polly;

namespace Blazor.Presentation.Client.Services;

public static class ImageService
{
    public static async Task<PolicyResult<ImageDto>> GetImageAsync(int carouselItemId)
    {
        HttpClient httpClient = new HttpClient();
        httpClient.DefaultRequestHeaders.Accept.Clear();
        httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
        var retryPolicy = Policy<ImageDto>
            .Handle<HttpRequestException>()
            .WaitAndRetryAsync(Constants.MaxHttpRequestRetries,times => TimeSpan.FromMilliseconds(times * 100));
        
        return await retryPolicy.ExecuteAndCaptureAsync(async () => await httpClient.GetFromJsonAsync<ImageDto>(
            $"https://localhost:11001/api/CarouselItem/{carouselItemId}/Image"));
    }
}