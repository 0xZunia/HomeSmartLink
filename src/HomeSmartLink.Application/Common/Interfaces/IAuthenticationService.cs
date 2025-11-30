using HomeSmartLink.Application.Common.Models;

namespace HomeSmartLink.Application.Common.Interfaces;

public interface IAuthenticationService
{
    Task<ApiResult<AuthResponse>> SignInAsync(string email, string password, CancellationToken ct = default);
    Task<ApiResult<AuthResponse>> SignUpAsync(string email, string password, string firstName, string lastName, CancellationToken ct = default);
    Task<ApiResult> ForgotPasswordAsync(string email, CancellationToken ct = default);
    Task<ApiResult> ChangePasswordAsync(string code, string newPassword, CancellationToken ct = default);
    Task SignOutAsync(CancellationToken ct = default);
    Task<bool> IsAuthenticatedAsync(CancellationToken ct = default);
    Task<string?> GetCurrentUserEmailAsync(CancellationToken ct = default);
}

public interface ISessionStorage
{
    Task<string?> GetTokenAsync(CancellationToken ct = default);
    Task SetTokenAsync(string token, CancellationToken ct = default);
    Task ClearTokenAsync(CancellationToken ct = default);
    Task<T?> GetAsync<T>(string key, CancellationToken ct = default);
    Task SetAsync<T>(string key, T value, CancellationToken ct = default);
    Task RemoveAsync(string key, CancellationToken ct = default);
}

public interface ICurrentUserService
{
    string? UserId { get; }
    string? Email { get; }
    string? CurrentHomeId { get; }
    bool IsAuthenticated { get; }
}
