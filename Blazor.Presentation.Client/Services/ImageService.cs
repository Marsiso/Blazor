using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Runtime.InteropServices;
using System.Runtime.Serialization.Formatters.Binary;
using Blazor.Shared.Entities.Constants;
using Blazor.Shared.Entities.DataTransferObjects;
using Microsoft.AspNetCore.Components.Forms;
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
    
    public async Task<string> UploadImageAsync(int carouselItemId, IBrowserFile browserFile)
    {
        if (browserFile.Size is >= Constants.MaximalImageSize or <= Constants.MinimalImageSize)
        {
            return String.Empty;
        }

        var httpClient = _httpClientFactory.CreateClient("Default");
        var retryPolicy = Policy<HttpResponseMessage>
            .Handle<HttpRequestException>()
            .WaitAndRetryAsync(Constants.MaxHttpRequestRetries,times => TimeSpan.FromMilliseconds(times * 100));
        
        var buffers = new byte[browserFile.Size];
        _ = await browserFile.OpenReadStream(browserFile.Size).ReadAsync(buffers);
        var imageSource = $"data:{browserFile.ContentType};base64,{Convert.ToBase64String(buffers)}";

        httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

        var byteArrayContent = new ByteArrayContent(buffers);
        byteArrayContent.Headers.ContentType = MediaTypeHeaderValue.Parse(browserFile.ContentType);

        var policyResult = await retryPolicy.ExecuteAndCaptureAsync(async () => await httpClient.PostAsync(
            $"CarouselItem/{carouselItemId}/Image", new MultipartFormDataContent
        {
            {
                new StringContent(Path.GetFileNameWithoutExtension(browserFile.Name)), 
                "\"fileName\""
            },
            {
                byteArrayContent, 
                "\"imageFile\"", 
                $"\"{browserFile.Name}\""
            }
        }));

        if (policyResult.Outcome == OutcomeType.Successful && policyResult.Result.IsSuccessStatusCode)
        {
            return imageSource;
        }
        
        return String.Empty;
    }
}