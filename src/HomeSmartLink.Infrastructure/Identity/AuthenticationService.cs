using HomeSmartLink.Application.Common.Interfaces;
using HomeSmartLink.Application.Common.Models;

namespace HomeSmartLink.Infrastructure.Identity;

public class AuthenticationService : IAuthenticationService
{
    private readonly ISmartLinkApiClient _apiClient;
    private readonly ISessionStorage _sessionStorage;

    private const string EmailKey = "user_email";
    private const string HomeIdKey = "current_home_id";

    public AuthenticationService(ISmartLinkApiClient apiClient, ISessionStorage sessionStorage)
    {
        _apiClient = apiClient;
        _sessionStorage = sessionStorage;
    }

    public async Task<ApiResult<AuthResponse>> SignInAsync(string email, string password, CancellationToken ct = default)
    {
        var result = await _apiClient.SignInAsync(email, password, ct);

        if (result.IsSuccess && result.Data != null)
        {
            await _sessionStorage.SetTokenAsync(result.Data.Token, ct);
            await _sessionStorage.SetAsync(EmailKey, email, ct);

            // Store home ID from invitation if present
            if (result.Data.Invitation != null && !string.IsNullOrEmpty(result.Data.Invitation.HomeId))
            {
                await _sessionStorage.SetAsync(HomeIdKey, result.Data.Invitation.HomeId, ct);
            }
        }

        return result;
    }

    public async Task<ApiResult<AuthResponse>> SignUpAsync(string email, string password, string firstName, string lastName, CancellationToken ct = default)
    {
        var result = await _apiClient.SignUpAsync(email, password, firstName, lastName, ct);

        if (result.IsSuccess && result.Data != null)
        {
            await _sessionStorage.SetTokenAsync(result.Data.Token, ct);
            await _sessionStorage.SetAsync(EmailKey, email, ct);
        }

        return result;
    }

    public Task<ApiResult> ForgotPasswordAsync(string email, CancellationToken ct = default) =>
        _apiClient.SendPasswordResetCodeAsync(email, ct);

    public async Task<ApiResult> ChangePasswordAsync(string code, string newPassword, CancellationToken ct = default)
    {
        var result = await _apiClient.ResetPasswordAsync(code, newPassword, ct);
        return result.IsSuccess ? ApiResult.Success() : ApiResult.Failure(result.ErrorMessage ?? "Password reset failed");
    }

    public async Task SignOutAsync(CancellationToken ct = default)
    {
        _apiClient.Logout();
        await _sessionStorage.ClearTokenAsync(ct);
        await _sessionStorage.RemoveAsync(EmailKey, ct);
        await _sessionStorage.RemoveAsync(HomeIdKey, ct);
    }

    public async Task<bool> IsAuthenticatedAsync(CancellationToken ct = default)
    {
        var token = await _sessionStorage.GetTokenAsync(ct);
        if (string.IsNullOrEmpty(token))
            return false;

        // Optionally verify the session is still valid
        var result = await _apiClient.CheckSessionAsync(ct);
        return result.IsSuccess;
    }

    public Task<string?> GetCurrentUserEmailAsync(CancellationToken ct = default) =>
        _sessionStorage.GetAsync<string>(EmailKey, ct);
}
