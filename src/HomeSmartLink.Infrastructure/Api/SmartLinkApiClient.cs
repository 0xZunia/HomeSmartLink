using System.Net.Http.Json;
using System.Text;
using System.Text.Json;
using HomeSmartLink.Application.Common.Interfaces;
using HomeSmartLink.Application.Common.Models;

namespace HomeSmartLink.Infrastructure.Api;

/// <summary>
/// SmartLink API Client implementation matching the decompiled ThrottledWebClient from SmartLink.Core.Services.Common
/// </summary>
public class SmartLinkApiClient : ISmartLinkApiClient
{
    private readonly HttpClient _httpClient;
    private readonly ISessionStorage _sessionStorage;
    private readonly JsonSerializerOptions _jsonOptions;

    private const string BaseUrl = "https://jcqzjjt6q5.execute-api.eu-west-3.amazonaws.com/Prod/";
    private const int CurrentAppVersion = 103;
    private const string Agent = "blazor-web";

    public string? Session { get; set; }

    public SmartLinkApiClient(HttpClient httpClient, ISessionStorage sessionStorage)
    {
        _httpClient = httpClient;
        _httpClient.BaseAddress = new Uri(BaseUrl);
        _httpClient.DefaultRequestHeaders.Add("Accept", "application/json");
        _sessionStorage = sessionStorage;
        _jsonOptions = new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true,
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
            DefaultIgnoreCondition = System.Text.Json.Serialization.JsonIgnoreCondition.WhenWritingNull
        };
    }

    private async Task<string?> GetSessionAsync(CancellationToken ct)
    {
        // Always read from storage to ensure we have the latest token
        // This is important because the client may be a new instance (transient)
        if (!string.IsNullOrEmpty(Session))
        {
            return Session;
        }

        return await _sessionStorage.GetTokenAsync(ct);
    }

    private async Task<ApiResult<T>> SendRequestAsync<T>(HttpMethod method, string route, object? body = null, CancellationToken ct = default)
    {
        try
        {
            var session = await GetSessionAsync(ct);

            var request = new HttpRequestMessage(method, route);

            if (body != null)
            {
                var json = JsonSerializer.Serialize(body, _jsonOptions);
                request.Content = new StringContent(json, Encoding.UTF8, "application/json");
            }

            if (!string.IsNullOrEmpty(session))
            {
                request.Headers.TryAddWithoutValidation("x-session", session);
            }

            var response = await _httpClient.SendAsync(request, ct);

            if (!response.IsSuccessStatusCode)
            {
                var errorContent = await response.Content.ReadAsStringAsync(ct);
                return ApiResult<T>.Failure(
                    string.IsNullOrEmpty(errorContent) ? $"Error: {response.StatusCode}" : errorContent,
                    (int)response.StatusCode);
            }

            var content = await response.Content.ReadAsStringAsync(ct);
            if (string.IsNullOrEmpty(content))
            {
                return ApiResult<T>.Failure("Empty response");
            }

            var data = JsonSerializer.Deserialize<T>(content, _jsonOptions);
            return data != null
                ? ApiResult<T>.Success(data)
                : ApiResult<T>.Failure("Failed to deserialize response");
        }
        catch (Exception ex)
        {
            return ApiResult<T>.Failure(ex.Message);
        }
    }

    private async Task<ApiResult> SendRequestAsync(HttpMethod method, string route, object? body = null, CancellationToken ct = default)
    {
        try
        {
            var session = await GetSessionAsync(ct);

            var request = new HttpRequestMessage(method, route);

            if (body != null)
            {
                var json = JsonSerializer.Serialize(body, _jsonOptions);
                request.Content = new StringContent(json, Encoding.UTF8, "application/json");
            }

            if (!string.IsNullOrEmpty(session))
            {
                request.Headers.TryAddWithoutValidation("x-session", session);
            }

            var response = await _httpClient.SendAsync(request, ct);

            if (!response.IsSuccessStatusCode)
            {
                var errorContent = await response.Content.ReadAsStringAsync(ct);
                return ApiResult.Failure(
                    string.IsNullOrEmpty(errorContent) ? $"Error: {response.StatusCode}" : errorContent,
                    (int)response.StatusCode);
            }

            return ApiResult.Success();
        }
        catch (Exception ex)
        {
            return ApiResult.Failure(ex.Message);
        }
    }

    // ==========================================
    // Authentication
    // ==========================================

    public async Task<ApiResult<AuthResponse>> SignInAsync(string email, string password, CancellationToken ct = default)
    {
        var body = new Dictionary<string, object>
        {
            { "email", email },
            { "password", password },
            { "agent", Agent },
            { "version", CurrentAppVersion }
        };

        var result = await SendRequestAsync<AuthResponse>(HttpMethod.Post, "api/1/signin", body, ct);
        if (result.IsSuccess && result.Data?.Token != null)
        {
            Session = result.Data.Token;
            await _sessionStorage.SetTokenAsync(result.Data.Token, ct);
        }
        return result;
    }

    public async Task<ApiResult<AuthResponse>> SignUpAsync(string email, string password, string firstName, string lastName, CancellationToken ct = default)
    {
        var body = new Dictionary<string, object>
        {
            { "email", email },
            { "password", password },
            { "agent", Agent },
            { "version", CurrentAppVersion },
            { "firstName", firstName },
            { "lastName", lastName }
        };

        var result = await SendRequestAsync<AuthResponse>(HttpMethod.Post, "api/1/signup", body, ct);
        if (result.IsSuccess && result.Data?.Token != null)
        {
            Session = result.Data.Token;
            await _sessionStorage.SetTokenAsync(result.Data.Token, ct);
        }
        return result;
    }

    public async Task<ApiResult<AuthResponse>> CheckSessionAsync(CancellationToken ct = default)
    {
        return await SendRequestAsync<AuthResponse>(HttpMethod.Get, $"api/1/auth?version={CurrentAppVersion}", ct: ct);
    }

    public void Logout()
    {
        Session = null;
        _ = _sessionStorage.ClearTokenAsync(default);
    }

    public async Task<ApiResult> UnsubscribeAsync(CancellationToken ct = default)
    {
        var result = await SendRequestAsync(HttpMethod.Delete, "api/1/user", ct: ct);
        if (result.IsSuccess)
        {
            Logout();
        }
        return result;
    }

    public async Task<ApiResult> SendPasswordResetCodeAsync(string email, CancellationToken ct = default)
    {
        var body = new Dictionary<string, object>
        {
            { "email", email.ToLower() }
        };
        return await SendRequestAsync(HttpMethod.Post, "api/1/iforgot", body, ct);
    }

    public async Task<ApiResult<AuthResponse>> ResetPasswordAsync(string resetCode, string newPassword, CancellationToken ct = default)
    {
        var body = new Dictionary<string, object>
        {
            { "code", resetCode.ToLower() },
            { "password", newPassword }
        };
        return await SendRequestAsync<AuthResponse>(HttpMethod.Post, "api/1/ichange", body, ct);
    }

    // ==========================================
    // Home Management
    // ==========================================

    public async Task<ApiResult<InvitationDto>> CreateHomeAsync(string name, string? notes = null, CancellationToken ct = default)
    {
        var body = new Dictionary<string, object>
        {
            { "name", name ?? "" },
            { "notes", notes ?? "" }
        };
        return await SendRequestAsync<InvitationDto>(HttpMethod.Post, "api/1/homes/", body, ct);
    }

    public async Task<ApiResult<HomeDto>> FetchHomeAsync(string homeId, CancellationToken ct = default)
    {
        return await SendRequestAsync<HomeDto>(HttpMethod.Get, $"api/1/homes/{homeId}", ct: ct);
    }

    public async Task<ApiResult> SaveHomeAsync(HomeDto home, CancellationToken ct = default)
    {
        return await SendRequestAsync(HttpMethod.Put, "api/1/homes/", home, ct);
    }

    // ==========================================
    // Invitations
    // ==========================================

    public async Task<ApiResult<List<InvitationDto>>> FetchInvitationsAsync(string? homeId = null, CancellationToken ct = default)
    {
        var route = string.IsNullOrEmpty(homeId) ? "api/1/invitations" : $"api/1/invitations/{homeId}";
        return await SendRequestAsync<List<InvitationDto>>(HttpMethod.Get, route, ct: ct);
    }

    public async Task<ApiResult<List<InvitationDto>>> FetchMyInvitationsAsync(CancellationToken ct = default)
    {
        return await SendRequestAsync<List<InvitationDto>>(HttpMethod.Get, "api/1/invitations", ct: ct);
    }

    public async Task<ApiResult> SendInvitationAsync(string homeId, string toEmail, int role, string? expirationIsoDate = null, CancellationToken ct = default)
    {
        var body = new Dictionary<string, object>
        {
            { "homeid", homeId.ToLower() },
            { "role", role },
            { "to", toEmail.ToLower() }
        };

        if (expirationIsoDate != null)
        {
            body.Add("expirationDate", expirationIsoDate);
        }

        return await SendRequestAsync(HttpMethod.Post, "api/1/invitations", body, ct);
    }

    public async Task<ApiResult> RevokeInvitationAsync(string invitationId, CancellationToken ct = default)
    {
        return await SendRequestAsync(HttpMethod.Delete, $"api/1/invitations/{invitationId}", ct: ct);
    }

    public async Task<ApiResult<InvitationDto>> UpdateInvitationAsync(string invitationId, string? homeName, string? notes, CancellationToken ct = default)
    {
        var body = new Dictionary<string, object>
        {
            { "name", homeName ?? "" },
            { "notes", notes ?? "" }
        };
        return await SendRequestAsync<InvitationDto>(HttpMethod.Put, $"api/1/invitations/{invitationId}", body, ct);
    }

    // ==========================================
    // Gateway/Device
    // ==========================================

    public async Task<ApiResult> RegisterGatewayAsync(string homeId, string gatewayId, byte[] secret, CancellationToken ct = default)
    {
        var body = new Dictionary<string, object>
        {
            { "uuid", gatewayId },
            { "homeId", homeId },
            { "secret", Convert.ToBase64String(secret) }
        };
        return await SendRequestAsync(HttpMethod.Post, "api/1/relai", body, ct);
    }

    public async Task<ApiResult<GatewayDto>> FetchGatewayAsync(string homeId, CancellationToken ct = default)
    {
        // Use GatewayDto directly since hasGateway is a boolean in JSON
        return await SendRequestAsync<GatewayDto>(HttpMethod.Get, $"api/1/device/{homeId}", ct: ct);
    }

    public async Task<ApiResult<List<DeviceDataDto>>> FetchDeviceDataAsync(string homeId, CancellationToken ct = default)
    {
        var result = await SendRequestAsync<DeviceStatusResponseDto>(HttpMethod.Get, $"api/1/deviceStatus/{homeId}", ct: ct);

        if (!result.IsSuccess || result.Data == null)
        {
            return ApiResult<List<DeviceDataDto>>.Failure(result.ErrorMessage ?? "Failed to fetch device data");
        }

        var deviceData = result.Data.DevicesStatus
            .Select(DeviceDataDto.Parse)
            .ToList();

        return ApiResult<List<DeviceDataDto>>.Success(deviceData);
    }

    public async Task<ApiResult> SendDirectActionAsync(string homeId, byte[] payload, string payloadTag = "action", string? description = null, CancellationToken ct = default)
    {
        var dict = new Dictionary<string, string>
        {
            { payloadTag, Convert.ToBase64String(payload) },
            { "homeId", homeId },
            { "description", description ?? "" }
        };
        return await SendDirectActionAsync(dict, ct);
    }

    public async Task<ApiResult> SendDirectActionAsync(Dictionary<string, string> payload, CancellationToken ct = default)
    {
        return await SendRequestAsync(HttpMethod.Post, "api/1/device/notify", payload, ct);
    }

    // ==========================================
    // Geofencing
    // ==========================================

    public async Task<ApiResult> SendGeofenceActionAsync(string homeId, ProximityAction action, string? description = null, int roomMask = 255, CancellationToken ct = default)
    {
        if (action == ProximityAction.Nothing)
        {
            return ApiResult.Success();
        }

        var body = new Dictionary<string, object>
        {
            { "action", (int)action },
            { "homeId", homeId },
            { "description", description ?? "" },
            { "rooms", roomMask }
        };

        return await SendRequestAsync(HttpMethod.Post, "api/1/geofencing", body, ct);
    }

    // ==========================================
    // Ecowatt
    // ==========================================

    public async Task<ApiResult<EcowattDto>> FetchEcowattReportsAsync(CancellationToken ct = default)
    {
        return await SendRequestAsync<EcowattDto>(HttpMethod.Get, "api/1/ecowatt", ct: ct);
    }

    // ==========================================
    // Consumption
    // ==========================================

    public async Task<ApiResult<int>> GetThermalEvaluationReportAsync(int consumption, string zipCode, int area, double ecoSetpoint, double comfortSetpoint, CancellationToken ct = default)
    {
        var body = new Dictionary<string, object>
        {
            { "zipCode", zipCode },
            { "ecoSP", ecoSetpoint },
            { "confSP", comfortSetpoint },
            { "area", area },
            { "conso", consumption }
        };

        var result = await SendRequestAsync<Dictionary<string, object>>(HttpMethod.Post, "api/1/consumption", body, ct);

        if (!result.IsSuccess || result.Data == null)
        {
            return ApiResult<int>.Failure(result.ErrorMessage ?? "Failed to get thermal evaluation");
        }

        if (result.Data.TryGetValue("nbStars", out var starsObj) && starsObj is JsonElement element)
        {
            return ApiResult<int>.Success(element.GetInt32());
        }

        return ApiResult<int>.Failure("Invalid response format");
    }

    // ==========================================
    // Linky/Registration
    // ==========================================

    public async Task<ApiResult<RegistrationDto>> GetRegistrationAsync(string homeId, CancellationToken ct = default)
    {
        return await SendRequestAsync<RegistrationDto>(HttpMethod.Get, $"api/1/register/{homeId}", ct: ct);
    }

    public async Task<ApiResult> RegisterAsync(string homeId, string address, string zipCode, string city, string prmId, CancellationToken ct = default)
    {
        var body = new Dictionary<string, object>
        {
            { "prmId", prmId },
            { "zipCode", zipCode },
            { "city", city },
            { "street", address }
        };

        var result = await SendRequestAsync<Dictionary<string, string>>(HttpMethod.Put, $"api/1/register/{homeId}", body, ct);

        if (!result.IsSuccess)
        {
            return ApiResult.Failure(result.ErrorMessage ?? "Registration failed");
        }

        if (result.Data?.GetValueOrDefault("status") == "OK")
        {
            return ApiResult.Success();
        }

        return ApiResult.Failure("Registration failed");
    }

    public async Task<ApiResult<LinkyResponseDto>> SearchLinkyAsync(string address, string zipCode, string city, CancellationToken ct = default)
    {
        var body = new Dictionary<string, object>
        {
            { "zipCode", zipCode },
            { "city", city },
            { "street", address }
        };
        return await SendRequestAsync<LinkyResponseDto>(HttpMethod.Post, "api/1/linky", body, ct);
    }

    // ==========================================
    // Diagnostics
    // ==========================================

    public async Task<ApiResult<string>> SendDiagnosysRequestAsync(string homeId, CancellationToken ct = default)
    {
        var body = new Dictionary<string, object>
        {
            { "homeId", homeId }
        };

        var result = await SendRequestAsync<Dictionary<string, string>>(HttpMethod.Post, "api/1/diagnosys", body, ct);

        if (!result.IsSuccess || result.Data == null)
        {
            return ApiResult<string>.Failure(result.ErrorMessage ?? "Failed to request diagnostics");
        }

        if (result.Data.TryGetValue("id", out var diagId))
        {
            return ApiResult<string>.Success(diagId);
        }

        return ApiResult<string>.Failure("No diagnostic ID returned");
    }

    public async Task<ApiResult<DiagnosysResponseDto>> FetchDiagnosysAsync(string diagnosysId, CancellationToken ct = default)
    {
        return await SendRequestAsync<DiagnosysResponseDto>(HttpMethod.Get, $"api/1/diagnosys/{diagnosysId}", ct: ct);
    }

    // ==========================================
    // Help/Documentation
    // ==========================================

    public async Task<ApiResult<List<HelpItemDto>>> GetHelpAsync(string lang, CancellationToken ct = default)
    {
        return await SendRequestAsync<List<HelpItemDto>>(HttpMethod.Get, $"api/1/assistance/{lang}", ct: ct);
    }

    public async Task<ApiResult<byte[]>> GetNoticeAsync(string reference, CancellationToken ct = default)
    {
        try
        {
            var session = await GetSessionAsync(ct);

            var request = new HttpRequestMessage(HttpMethod.Get, $"api/1/usermanual/{reference}");

            if (!string.IsNullOrEmpty(session))
            {
                request.Headers.TryAddWithoutValidation("x-session", session);
            }

            var response = await _httpClient.SendAsync(request, ct);

            if (!response.IsSuccessStatusCode)
            {
                return ApiResult<byte[]>.Failure($"Error: {response.StatusCode}", (int)response.StatusCode);
            }

            var bytes = await response.Content.ReadAsByteArrayAsync(ct);
            return ApiResult<byte[]>.Success(bytes);
        }
        catch (Exception ex)
        {
            return ApiResult<byte[]>.Failure(ex.Message);
        }
    }
}
