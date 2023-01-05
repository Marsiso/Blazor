using System.Net.Http.Headers;
using Blazor.Shared.Entities.DataTransferObjects;
using System.Net.Http.Json;
using Blazor.Shared.Entities.Constants;
using Blazor.Shared.Entities.Models;
using Blazor.Shared.Entities.RequestFeatures;
using Polly;
using Polly.Retry;
using Syncfusion.Blazor.Navigations;

namespace Blazor.Presentation.Client.Services;

public sealed class CarouselItemService
{
    private readonly IHttpClientFactory _httpClientFactory;

    public CarouselItemService(IHttpClientFactory httpClientFactory)
    {
        _httpClientFactory = httpClientFactory;
    }

    public async Task<PolicyResult<List<Entity>>> GetAllAsync(CarouselItemParameters carouselItemParameters)
    {
        var httpClient = _httpClientFactory.CreateClient("Default");
        httpClient.DefaultRequestHeaders.Accept.Clear();
        httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
        AsyncRetryPolicy<List<Entity>> retryPolicy = Policy<List<Entity>>
            .Handle<HttpRequestException>()
            .WaitAndRetryAsync(Constants.MaxHttpRequestRetries,times => TimeSpan.FromMilliseconds(times * 100));

        return await retryPolicy.ExecuteAndCaptureAsync(async () =>
        {
            var requestUri = String.Join('&', "CarouselItem?",
                $"pageNumber={carouselItemParameters.PageNumber}",
                $"pageSize={carouselItemParameters.PageSize}",
                $"searchTerm={carouselItemParameters.SearchTerm}",
                $"minId={carouselItemParameters.MinId}",
                $"maxId={carouselItemParameters.MaxId}",
                $"orderBy={carouselItemParameters.OrderBy}");
            
            var httpResponse = await httpClient.GetAsync(requestUri);

            return httpResponse.IsSuccessStatusCode 
                ? await httpResponse.Content.ReadFromJsonAsync<List<Entity>>() 
                : new List<Entity>();
        });
    }
    
    public async Task<PolicyResult<Entity>> GetAsync(int carouselItemId, CarouselItemParameters carouselItemParameters = null)
    {
        var httpClient = _httpClientFactory.CreateClient("Default");
        httpClient.DefaultRequestHeaders.Accept.Clear();
        httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
        AsyncRetryPolicy<Entity> retryPolicy = Policy<Entity>
            .Handle<HttpRequestException>()
            .WaitAndRetryAsync(Constants.MaxHttpRequestRetries,times => TimeSpan.FromMilliseconds(times * 100));

        return await retryPolicy.ExecuteAndCaptureAsync(async () =>
        {
            var requestUri = $"CarouselItem/{carouselItemId}";
            
            var httpResponse = await httpClient.GetAsync(requestUri);

            if (!httpResponse.IsSuccessStatusCode) return new Entity();
            
            var responseResult = await httpResponse.Content.ReadFromJsonAsync<List<Entity>>();
            return responseResult.FirstOrDefault() ?? new Entity();
        });
    }

    public async Task<PolicyResult<HttpResponseMessage>> DeleteAsync(int carouselItemId)
    {
        var httpClient = _httpClientFactory.CreateClient("Default");
        
        AsyncRetryPolicy retryPolicy = Policy.Handle<HttpRequestException>()
            .WaitAndRetryAsync(Constants.MaxHttpRequestRetries,times => TimeSpan.FromMilliseconds(times * 100));

        return await retryPolicy.ExecuteAndCaptureAsync(async () =>
            await httpClient.DeleteAsync($"CarouselItem/{carouselItemId}"));
    }
    
    public async Task<CarouselItemDto> CreateAsync(CarouselItemForCreationDto carouselItemForCreationDto)
    {
        var httpClient = _httpClientFactory.CreateClient("Default");
        httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
        
        AsyncRetryPolicy retryPolicy = Policy.Handle<HttpRequestException>()
            .WaitAndRetryAsync(Constants.MaxHttpRequestRetries,times => TimeSpan.FromMilliseconds(times * 100));

        var policyResult = await retryPolicy.ExecuteAndCaptureAsync(async () =>
            await httpClient.PostAsJsonAsync("CarouselItem/", carouselItemForCreationDto));

        CarouselItemDto carouselItemDto = null;
        if (policyResult.Outcome == OutcomeType.Successful && policyResult.Result.IsSuccessStatusCode)
        {
            carouselItemDto = await policyResult.Result.Content.ReadFromJsonAsync<CarouselItemDto>();
        }

        return carouselItemDto;
    }
    
    public async Task<PolicyResult<HttpResponseMessage>> UpdateAsync(int carouselItemId, CarouselItemForUpdateDto carouselItemForCreationDto)
    {
        var httpClient = _httpClientFactory.CreateClient("Default");
        AsyncRetryPolicy retryPolicy = Policy.Handle<HttpRequestException>()
            .WaitAndRetryAsync(Constants.MaxHttpRequestRetries,times => TimeSpan.FromMilliseconds(times * 100));

        return await retryPolicy.ExecuteAndCaptureAsync(async () =>
            await httpClient.PutAsJsonAsync($"CarouselItem/{carouselItemId}", carouselItemForCreationDto));
    }
}
