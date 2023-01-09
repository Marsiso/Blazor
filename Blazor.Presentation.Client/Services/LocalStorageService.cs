using Microsoft.JSInterop;
using Newtonsoft.Json;

namespace Blazor.Presentation.Client.Services;

public sealed class LocalStorageService
{
    private readonly IJSRuntime _jsRuntime;

    public LocalStorageService(IJSRuntime jsRuntime)
    {
        _jsRuntime = jsRuntime;
    }

    public async Task SetAsync<TValue>(string key, TValue value)
    {
        string jsVal = null;
        if (value != null)
            jsVal = System.Text.Json.JsonSerializer.Serialize(value);
        await _jsRuntime.InvokeVoidAsync("localStorage.setItem", 
            new object[] { key, jsVal });
    }
    public async Task<TValue> GetAsync<TValue>(string key)
    {
        string val = await _jsRuntime.InvokeAsync<string>("localStorage.getItem", key);
        if (val == null) return default;
        TValue result = System.Text.Json.JsonSerializer.Deserialize<TValue>(val);
        return result;
    }
    public async Task RemoveAsync(string key)
    {
        await _jsRuntime.InvokeVoidAsync("localStorage.removeItem", key);
    }
    public async Task ClearAsync()
    {
        await _jsRuntime.InvokeVoidAsync("localStorage.clear");
    }
}