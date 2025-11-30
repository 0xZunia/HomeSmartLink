using System.Text.Json;
using HomeSmartLink.Application.Common.Interfaces;
using Microsoft.JSInterop;

namespace HomeSmartLink.Infrastructure.Services;

public class BrowserSessionStorage : ISessionStorage
{
    private readonly IJSRuntime _jsRuntime;
    private readonly JsonSerializerOptions _jsonOptions;

    private const string TokenKey = "smartlink_session_token";

    public BrowserSessionStorage(IJSRuntime jsRuntime)
    {
        _jsRuntime = jsRuntime;
        _jsonOptions = new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true,
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase
        };
    }

    public async Task<string?> GetTokenAsync(CancellationToken ct = default)
    {
        try
        {
            return await _jsRuntime.InvokeAsync<string?>("localStorage.getItem", ct, TokenKey);
        }
        catch
        {
            return null;
        }
    }

    public async Task SetTokenAsync(string token, CancellationToken ct = default)
    {
        try
        {
            await _jsRuntime.InvokeVoidAsync("localStorage.setItem", ct, TokenKey, token);
        }
        catch
        {
            // Ignore errors during SSR
        }
    }

    public async Task ClearTokenAsync(CancellationToken ct = default)
    {
        try
        {
            await _jsRuntime.InvokeVoidAsync("localStorage.removeItem", ct, TokenKey);
        }
        catch
        {
            // Ignore errors during SSR
        }
    }

    public async Task<T?> GetAsync<T>(string key, CancellationToken ct = default)
    {
        try
        {
            var json = await _jsRuntime.InvokeAsync<string?>("localStorage.getItem", ct, $"smartlink_{key}");

            if (string.IsNullOrEmpty(json))
                return default;

            return JsonSerializer.Deserialize<T>(json, _jsonOptions);
        }
        catch
        {
            return default;
        }
    }

    public async Task SetAsync<T>(string key, T value, CancellationToken ct = default)
    {
        try
        {
            var json = JsonSerializer.Serialize(value, _jsonOptions);
            await _jsRuntime.InvokeVoidAsync("localStorage.setItem", ct, $"smartlink_{key}", json);
        }
        catch
        {
            // Ignore errors during SSR
        }
    }

    public async Task RemoveAsync(string key, CancellationToken ct = default)
    {
        try
        {
            await _jsRuntime.InvokeVoidAsync("localStorage.removeItem", ct, $"smartlink_{key}");
        }
        catch
        {
            // Ignore errors during SSR
        }
    }
}
