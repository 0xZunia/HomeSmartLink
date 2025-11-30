using System.Text.Json;
using HomeSmartLink.Application.Common.Interfaces;

namespace HomeSmartLink.Mobile.Services;

public class MauiSessionStorage : ISessionStorage
{
    private const string TokenKey = "smartlink_session_token";
    private readonly JsonSerializerOptions _jsonOptions;

    public MauiSessionStorage()
    {
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
            return await SecureStorage.Default.GetAsync(TokenKey);
        }
        catch
        {
            return null;
        }
    }

    public async Task SetTokenAsync(string token, CancellationToken ct = default)
    {
        await SecureStorage.Default.SetAsync(TokenKey, token);
    }

    public Task ClearTokenAsync(CancellationToken ct = default)
    {
        SecureStorage.Default.Remove(TokenKey);
        return Task.CompletedTask;
    }

    public async Task<T?> GetAsync<T>(string key, CancellationToken ct = default)
    {
        try
        {
            var json = await SecureStorage.Default.GetAsync($"smartlink_{key}");

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
        var json = JsonSerializer.Serialize(value, _jsonOptions);
        await SecureStorage.Default.SetAsync($"smartlink_{key}", json);
    }

    public Task RemoveAsync(string key, CancellationToken ct = default)
    {
        SecureStorage.Default.Remove($"smartlink_{key}");
        return Task.CompletedTask;
    }
}
