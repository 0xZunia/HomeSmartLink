using HomeSmartLink.Application.Common.Interfaces;

namespace HomeSmartLink.Infrastructure.Services;

public class CurrentUserService : ICurrentUserService
{
    private readonly ISessionStorage _sessionStorage;

    public CurrentUserService(ISessionStorage sessionStorage)
    {
        _sessionStorage = sessionStorage;
    }

    public string? UserId => GetValue<string>("user_id");
    public string? Email => GetValue<string>("user_email");
    public string? CurrentHomeId => GetValue<string>("current_home_id");
    public bool IsAuthenticated => !string.IsNullOrEmpty(GetToken());

    private T? GetValue<T>(string key)
    {
        try
        {
            return _sessionStorage.GetAsync<T>(key).GetAwaiter().GetResult();
        }
        catch
        {
            return default;
        }
    }

    private string? GetToken()
    {
        try
        {
            return _sessionStorage.GetTokenAsync().GetAwaiter().GetResult();
        }
        catch
        {
            return null;
        }
    }
}
