namespace HomeSmartLink.Application.Common.Interfaces;

public interface ISessionService
{
    Task<string?> GetSessionTokenAsync();
    Task SaveSessionAsync(string sessionToken, string email, string homeId);
    Task<string?> GetEmailAsync();
    Task<string?> GetHomeIdAsync();
    void ClearSession();
    Task<bool> HasValidSessionAsync();
}
