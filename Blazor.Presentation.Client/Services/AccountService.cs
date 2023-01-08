using System.ComponentModel.DataAnnotations;
using System.Net.Http.Json;
using Blazor.Shared.Entities.Constants;
using Blazor.Shared.Entities.DataTransferObjects;
using Blazor.Shared.Entities.RequestFeatures;
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

    public async Task<bool> TryResetPasswordAsync(ResetPasswordForCreationDto resetPasswordForCreationRequest)
    {
        if (resetPasswordForCreationRequest is null)
        {
            return false;
        }

        var httpClient = _httpClientFactory.CreateClient("Anonymous");
        var retryPolicy = Policy<HttpResponseMessage>
            .Handle<HttpRequestException>()
            .WaitAndRetryAsync(Constants.MaxHttpRequestRetries,times => TimeSpan.FromMilliseconds(times * 100));
        
        var policyResult = await retryPolicy.ExecuteAndCaptureAsync(async () => await httpClient.PutAsJsonAsync(
            $"Account/Password/Reset", resetPasswordForCreationRequest));
        
        return policyResult.Outcome == OutcomeType.Successful && policyResult.Result.IsSuccessStatusCode;
    }
    
    public async Task<List<ResetPasswordRequestDto>> GetAllResetPasswordRequestsAsync(ResetPasswordRequestParameters parameters)
    {
        if (parameters is null)
        {
            return new List<ResetPasswordRequestDto>();
        }

        var httpClient = _httpClientFactory.CreateClient("Anonymous");
        var retryPolicy = Policy<IEnumerable<ResetPasswordRequestDto>>
            .Handle<HttpRequestException>()
            .WaitAndRetryAsync(Constants.MaxHttpRequestRetries,times => TimeSpan.FromMilliseconds(times * 100));

        var uri = String.Format("Account/Password/Reset?{0}={1}&{2}={3}&{4}={5}&{6}={7}&{8}={9}&{10}={11}&{12}={13}&{14}={15}",
            nameof(parameters.MinIssueDate), parameters.MinIssueDate,
            nameof(parameters.MaxIssueDate), parameters.MaxIssueDate,
            nameof(parameters.MinExpirationDate), parameters.MinExpirationDate,
            nameof(parameters.MaxExpirationDate), parameters.MaxExpirationDate,
            nameof(parameters.PageSize), parameters.PageSize,
            nameof(parameters.PageNumber), parameters.PageNumber,
            nameof(parameters.SearchTerm), parameters.SearchTerm,
            nameof(parameters.OrderBy), parameters.OrderBy);
        
        var policyResult = await retryPolicy.ExecuteAndCaptureAsync(async () => 
            await httpClient.GetFromJsonAsync<IEnumerable<ResetPasswordRequestDto>>(uri));
        
        return policyResult.Outcome == OutcomeType.Successful && policyResult.Result is not null 
            ? policyResult.Result.ToList()
            : new List<ResetPasswordRequestDto>();
    }

    public async Task<bool> TryCreateResetPasswordEmailTemplate(ResetPasswordEmailTemplateDto template)
    {
        if (template is null) return false;

        var httpClient = _httpClientFactory.CreateClient("Default");
        var retryPolicy = Policy<HttpResponseMessage>
            .Handle<HttpRequestException>()
            .WaitAndRetryAsync(Constants.MaxHttpRequestRetries,times => TimeSpan.FromMilliseconds(times * 100));
        
        var policyResult = await retryPolicy.ExecuteAndCaptureAsync(async () => 
            await httpClient.PostAsJsonAsync("Account/Password/Reset/Email/Template", template));
        
        return policyResult.Outcome == OutcomeType.Successful && policyResult.Result.IsSuccessStatusCode;
    }
    
    public async Task<ResetPasswordEmailTemplateDto> GetResetPasswordEmailTemplate()
    {
        var httpClient = _httpClientFactory.CreateClient("Default");
        var retryPolicy = Policy<ResetPasswordEmailTemplateDto>
            .Handle<HttpRequestException>()
            .WaitAndRetryAsync(Constants.MaxHttpRequestRetries,times => TimeSpan.FromMilliseconds(times * 100));
        
        var policyResult = await retryPolicy.ExecuteAndCaptureAsync(async () => 
            await httpClient.GetFromJsonAsync<ResetPasswordEmailTemplateDto>("Account/Password/Reset/Email/Template"));

        return policyResult.Outcome == OutcomeType.Successful 
            ? policyResult.Result 
            : new ResetPasswordEmailTemplateDto { Payload = "<p><strong>Failed to load data ...</strong></p>" };
    }
}