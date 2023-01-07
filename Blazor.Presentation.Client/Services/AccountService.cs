using System.ComponentModel.DataAnnotations;
using System.Net.Http.Json;
using Blazor.Shared.Entities.Constants;
using Blazor.Shared.Entities.DataTransferObjects;
using Polly;

namespace Blazor.Presentation.Client.Services;

public class AccountService
{
    private readonly IHttpClientFactory _httpClientFactory;

    public AccountService(IHttpClientFactory httpClientFactory)
    {
        _httpClientFactory = httpClientFactory;
    }

    public async Task<bool> TryCreateUserAsync(UserForCreationDto user)
    {
        if (user is null)
        {
            return false;
        }

        var httpClient = _httpClientFactory.CreateClient("Anonymous");
        var retryPolicy = Policy<HttpResponseMessage>
            .Handle<HttpRequestException>()
            .WaitAndRetryAsync(Constants.MaxHttpRequestRetries,times => TimeSpan.FromMilliseconds(times * 100));
        
        var policyResult = await retryPolicy.ExecuteAndCaptureAsync(async () => await httpClient.PostAsJsonAsync(
            $"Account/Create/", user));
        
        return policyResult.Outcome == OutcomeType.Successful && policyResult.Result.IsSuccessStatusCode;
    }

    public async Task<bool> TryResetPasswordAsync(ResetPasswordDto resetPasswordRequest)
    {
        if (resetPasswordRequest is null)
        {
            return false;
        }

        var httpClient = _httpClientFactory.CreateClient("Anonymous");
        var retryPolicy = Policy<HttpResponseMessage>
            .Handle<HttpRequestException>()
            .WaitAndRetryAsync(Constants.MaxHttpRequestRetries,times => TimeSpan.FromMilliseconds(times * 100));
        
        var policyResult = await retryPolicy.ExecuteAndCaptureAsync(async () => await httpClient.PutAsJsonAsync(
            $"Account/Password/Reset", resetPasswordRequest));
        
        return policyResult.Outcome == OutcomeType.Successful && policyResult.Result.IsSuccessStatusCode;
    }
}