using System.Net.Http.Headers;
using Blazor.Shared.Entities.DataTransferObjects;
using System.Net.Http.Json;
using System.Text.Json.Serialization;
using Blazor.Presentation.Client.Utility;
using Blazor.Shared.Entities.Constants;
using Blazor.Shared.Entities.LinkModels;
using Blazor.Shared.Entities.Models;
using Blazor.Shared.Entities.RequestFeatures;
using Blazor.Shared.Entities.Responses;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Newtonsoft.Json;
using Polly;
using Polly.Retry;

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

        /*return await retryPolicy.ExecuteAndCaptureAsync(async () => await httpClient.GetFromJsonAsync<List<Entity>>(
            $"CarouselItem?pageNumber={carouselItemParameters.PageNumber}&pageSize={carouselItemParameters.PageSize}"));*/
        
        return await retryPolicy.ExecuteAndCaptureAsync(async () =>
        {
            var httpResponse = await httpClient.GetAsync(
                $"CarouselItem?pageNumber={carouselItemParameters.PageNumber}&pageSize={carouselItemParameters.PageSize}");

            return httpResponse.IsSuccessStatusCode 
                ? await httpResponse.Content.ReadFromJsonAsync<List<Entity>>() 
                : new List<Entity>();
        });
    }

    [Obsolete]
    public async ValueTask<ResponseDetails<LinkCollectionWrapper<Entity>>> GetAllWithLinksAsync(
        CarouselItemParameters carouselItemParameters)
    {
        var responseDetails = new ResponseDetails<LinkCollectionWrapper<Entity>> { Content = new LinkCollectionWrapper<Entity>() };
        
        var httpClient = _httpClientFactory.CreateClient("Default");
        httpClient.DefaultRequestHeaders.Accept.Clear();
        httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/vnd.utb.hateoas+json"));
        
        var retryPolicy = Policy<ResponseDetails<LinkCollectionWrapper<Entity>>>
            .Handle<HttpRequestException>()
            .WaitAndRetryAsync(Constants.MaxHttpRequestRetries,times => TimeSpan.FromMilliseconds(times * 100));

        return await retryPolicy.ExecuteAsync(async () =>
        {
            try
            {
                var response = await httpClient.GetAsync(
                    $"CarouselItem?pageNumber={carouselItemParameters.PageNumber}&pageSize={carouselItemParameters.PageSize}",
                    HttpCompletionOption.ResponseHeadersRead);

                responseDetails.IsResponse = true;
                if (response.IsSuccessStatusCode)
                {
                    responseDetails.IsSuccess = true;
                }
                else
                {
                    responseDetails.Message = "Http request has not been successful";
                    return responseDetails;
                }

                if (response.Content is object &&
                    response.Content.Headers.ContentType?.MediaType == "application/vnd.utb.hateoas+json")
                {
                    var contentStream = await response.Content.ReadAsStreamAsync();

                    responseDetails.Content =
                        await System.Text.Json.JsonSerializer.DeserializeAsync<LinkCollectionWrapper<Entity>>(
                            contentStream,
                            new System.Text.Json.JsonSerializerOptions
                            {
                                PropertyNameCaseInsensitive = true,
                                DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull
                            });
                }
                else
                {
                    responseDetails.Message = "Invalid content media type";
                }
            }
            catch (TaskCanceledException )
            {
                responseDetails.Message = "Http client connection timeout failure";
            }
            catch (JsonReaderException)
            {
                responseDetails.Message = "Json deserialization failure";
            }

            return responseDetails;
        });
    }
    
    [Obsolete]
    public async ValueTask<ResponseDetails<LinkCollectionWrapper<Entity>>> GetWithLinksAsync(int carouselItemId,
        CarouselItemParameters carouselItemParameters)
    {
        var responseDetails = new ResponseDetails<LinkCollectionWrapper<Entity>> { IsResponse = true, Content = new LinkCollectionWrapper<Entity>() };
        
        var httpClient = _httpClientFactory.CreateClient("Default");
        httpClient.DefaultRequestHeaders.Accept.Clear();
        httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/vnd.utb.hateoas+json"));
        
        var retryPolicy = Policy<ResponseDetails<LinkCollectionWrapper<Entity>>>
            .Handle<HttpRequestException>()
            .WaitAndRetryAsync(Constants.MaxHttpRequestRetries,times => TimeSpan.FromMilliseconds(times * 100));

        return await retryPolicy.ExecuteAsync(async () =>
        {
            try
            {
                var response = await httpClient.GetAsync(
                    $"CarouselItem/{carouselItemId}?pageNumber={carouselItemParameters.PageNumber}&pageSize={carouselItemParameters.PageSize}",
                    HttpCompletionOption.ResponseHeadersRead);

                responseDetails.IsResponse = true;
                if (response.IsSuccessStatusCode)
                {
                    responseDetails.IsSuccess = true;
                }
                else
                {
                    responseDetails.Message = "Http request has not been successful";
                    return responseDetails;
                }

                if (response.Content is object &&
                    response.Content.Headers.ContentType?.MediaType == "application/vnd.utb.hateoas+json")
                {
                    var contentStream = await response.Content.ReadAsStreamAsync();

                    responseDetails.Content =
                        await System.Text.Json.JsonSerializer.DeserializeAsync<LinkCollectionWrapper<Entity>>(
                            contentStream,
                            new System.Text.Json.JsonSerializerOptions
                            {
                                PropertyNameCaseInsensitive = true,
                                DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull
                            });
                }
                else
                {
                    responseDetails.Message = "Invalid content media type";
                    return responseDetails;
                }
            }
            catch (TaskCanceledException )
            {
                responseDetails.Message = "Http client connection timeout failure";
                responseDetails.IsResponse = false;
            }
            catch (JsonReaderException)
            {
                responseDetails.Message = "Json deserialization failure";
            }
            
            return responseDetails;
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
    
    public async Task<PolicyResult<HttpResponseMessage>> CreateAsync(CarouselItemForCreationDto carouselItemForCreationDto)
    {
        var httpClient = _httpClientFactory.CreateClient("Default");
        AsyncRetryPolicy retryPolicy = Policy.Handle<HttpRequestException>()
            .WaitAndRetryAsync(Constants.MaxHttpRequestRetries,times => TimeSpan.FromMilliseconds(times * 100));

        return await retryPolicy.ExecuteAndCaptureAsync(async () =>
            await httpClient.PostAsJsonAsync("CarouselItem/", carouselItemForCreationDto));
    }
    
    public async Task<PolicyResult<HttpResponseMessage>> UpdateAsync(int carouselItemId, CarouselItemForCreationDto carouselItemForCreationDto)
    {
        var httpClient = _httpClientFactory.CreateClient("Default");
        AsyncRetryPolicy retryPolicy = Policy.Handle<HttpRequestException>()
            .WaitAndRetryAsync(Constants.MaxHttpRequestRetries,times => TimeSpan.FromMilliseconds(times * 100));

        return await retryPolicy.ExecuteAndCaptureAsync(async () =>
            await httpClient.PutAsJsonAsync($"CarouselItem/{carouselItemId}", carouselItemForCreationDto));
    }
}
