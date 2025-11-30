using HomeSmartLink.Application.Common.Interfaces;

namespace HomeSmartLink.Mobile.Services;

public class SessionService : ISessionService
{
    private const string SessionKey = "smartlink_session";
    private const string EmailKey = "smartlink_email";
    private const string HomeIdKey = "smartlink_home_id";

    private readonly ISecureStorageService _secureStorage;

    public SessionService(ISecureStorageService secureStorage)
    {
        _secureStorage = secureStorage;
    }

    public async Task<string?> GetSessionTokenAsync()
    {
        return await _secureStorage.GetAsync(SessionKey);
    }

    public async Task SaveSessionAsync(string sessionToken, string email, string homeId)
    {
        await _secureStorage.SetAsync(SessionKey, sessionToken);
        await _secureStorage.SetAsync(EmailKey, email);
        await _secureStorage.SetAsync(HomeIdKey, homeId);
    }

    public async Task<string?> GetEmailAsync()
    {
        return await _secureStorage.GetAsync(EmailKey);
    }

    public async Task<string?> GetHomeIdAsync()
    {
        return await _secureStorage.GetAsync(HomeIdKey);
    }

    public void ClearSession()
    {
        _secureStorage.Remove(SessionKey);
        _secureStorage.Remove(EmailKey);
        _secureStorage.Remove(HomeIdKey);
    }

    public async Task<bool> HasValidSessionAsync()
    {
        var token = await GetSessionTokenAsync();
        return !string.IsNullOrEmpty(token);
    }
}
