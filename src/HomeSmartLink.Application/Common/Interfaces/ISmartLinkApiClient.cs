using HomeSmartLink.Application.Common.Models;

namespace HomeSmartLink.Application.Common.Interfaces;

/// <summary>
/// SmartLink API Client interface matching the decompiled IWebClient from SmartLink.Core.Services
/// Base URL: https://jcqzjjt6q5.execute-api.eu-west-3.amazonaws.com/Prod/
/// </summary>
public interface ISmartLinkApiClient
{
    /// <summary>
    /// Current session token (x-session header)
    /// </summary>
    string? Session { get; set; }

    // ==========================================
    // Authentication (api/1/signin, api/1/signup, api/1/auth)
    // ==========================================

    /// <summary>
    /// Sign in with email and password
    /// POST api/1/signin
    /// Body: { email, password, agent, version }
    /// </summary>
    Task<ApiResult<AuthResponse>> SignInAsync(string email, string password, CancellationToken ct = default);

    /// <summary>
    /// Create new account
    /// POST api/1/signup
    /// Body: { email, password, agent, version, firstName, lastName }
    /// </summary>
    Task<ApiResult<AuthResponse>> SignUpAsync(string email, string password, string firstName, string lastName, CancellationToken ct = default);

    /// <summary>
    /// Check current session validity
    /// GET api/1/auth?version={version}
    /// </summary>
    Task<ApiResult<AuthResponse>> CheckSessionAsync(CancellationToken ct = default);

    /// <summary>
    /// Logout (clears session)
    /// </summary>
    void Logout();

    /// <summary>
    /// Unsubscribe/delete user account
    /// DELETE api/1/user
    /// </summary>
    Task<ApiResult> UnsubscribeAsync(CancellationToken ct = default);

    /// <summary>
    /// Request password reset code
    /// POST api/1/iforgot
    /// Body: { email }
    /// </summary>
    Task<ApiResult> SendPasswordResetCodeAsync(string email, CancellationToken ct = default);

    /// <summary>
    /// Reset password with code
    /// POST api/1/ichange
    /// Body: { code, password }
    /// </summary>
    Task<ApiResult<AuthResponse>> ResetPasswordAsync(string resetCode, string newPassword, CancellationToken ct = default);

    // ==========================================
    // Home Management (api/1/homes/)
    // ==========================================

    /// <summary>
    /// Create a new home
    /// POST api/1/homes/
    /// Body: { name, notes }
    /// </summary>
    Task<ApiResult<InvitationDto>> CreateHomeAsync(string name, string? notes = null, CancellationToken ct = default);

    /// <summary>
    /// Fetch home details
    /// GET api/1/homes/{homeId}
    /// </summary>
    Task<ApiResult<HomeDto>> FetchHomeAsync(string homeId, CancellationToken ct = default);

    /// <summary>
    /// Save/update home data
    /// PUT api/1/homes/
    /// Body: serialized home object
    /// </summary>
    Task<ApiResult> SaveHomeAsync(HomeDto home, CancellationToken ct = default);

    // ==========================================
    // Invitations (api/1/invitations)
    // ==========================================

    /// <summary>
    /// Get invitations for a specific home
    /// GET api/1/invitations/{homeId}
    /// </summary>
    Task<ApiResult<List<InvitationDto>>> FetchInvitationsAsync(string? homeId = null, CancellationToken ct = default);

    /// <summary>
    /// Get my invitations
    /// GET api/1/invitations
    /// </summary>
    Task<ApiResult<List<InvitationDto>>> FetchMyInvitationsAsync(CancellationToken ct = default);

    /// <summary>
    /// Send invitation to user
    /// POST api/1/invitations
    /// Body: { homeid, role, to, expirationDate? }
    /// </summary>
    Task<ApiResult> SendInvitationAsync(string homeId, string toEmail, int role, string? expirationIsoDate = null, CancellationToken ct = default);

    /// <summary>
    /// Revoke/delete invitation
    /// DELETE api/1/invitations/{invitationId}
    /// </summary>
    Task<ApiResult> RevokeInvitationAsync(string invitationId, CancellationToken ct = default);

    /// <summary>
    /// Update invitation metadata
    /// PUT api/1/invitations/{invitationId}
    /// Body: { name, notes }
    /// </summary>
    Task<ApiResult<InvitationDto>> UpdateInvitationAsync(string invitationId, string? homeName, string? notes, CancellationToken ct = default);

    // ==========================================
    // Gateway/Device (api/1/relai, api/1/device)
    // ==========================================

    /// <summary>
    /// Register gateway
    /// POST api/1/relai
    /// Body: { uuid, homeId, secret }
    /// </summary>
    Task<ApiResult> RegisterGatewayAsync(string homeId, string gatewayId, byte[] secret, CancellationToken ct = default);

    /// <summary>
    /// Fetch gateway info
    /// GET api/1/device/{homeId}
    /// </summary>
    Task<ApiResult<GatewayDto>> FetchGatewayAsync(string homeId, CancellationToken ct = default);

    /// <summary>
    /// Fetch device status for all devices in a home
    /// GET api/1/deviceStatus/{homeId}
    /// Returns status strings in format: R7D1M3S4C44A41J10W50Y100
    /// </summary>
    Task<ApiResult<List<DeviceDataDto>>> FetchDeviceDataAsync(string homeId, CancellationToken ct = default);

    /// <summary>
    /// Send direct MQTT action to device
    /// POST api/1/device/notify
    /// Body: { action, homeId, description }
    /// </summary>
    Task<ApiResult> SendDirectActionAsync(string homeId, byte[] payload, string payloadTag = "action", string? description = null, CancellationToken ct = default);

    /// <summary>
    /// Send direct action with key-value payload
    /// POST api/1/device/notify
    /// </summary>
    Task<ApiResult> SendDirectActionAsync(Dictionary<string, string> payload, CancellationToken ct = default);

    // ==========================================
    // Geofencing (api/1/geofencing)
    // ==========================================

    /// <summary>
    /// Send geofence action (enter/exit zone)
    /// POST api/1/geofencing
    /// Body: { action, homeId, description, rooms }
    /// </summary>
    Task<ApiResult> SendGeofenceActionAsync(string homeId, ProximityAction action, string? description = null, int roomMask = 255, CancellationToken ct = default);

    // ==========================================
    // Ecowatt (api/1/ecowatt)
    // ==========================================

    /// <summary>
    /// Fetch Ecowatt forecast data
    /// GET api/1/ecowatt
    /// </summary>
    Task<ApiResult<EcowattDto>> FetchEcowattReportsAsync(CancellationToken ct = default);

    // ==========================================
    // Consumption (api/1/consumption)
    // ==========================================

    /// <summary>
    /// Get thermal evaluation report (energy rating stars)
    /// POST api/1/consumption
    /// Body: { zipCode, ecoSP, confSP, area, conso }
    /// </summary>
    Task<ApiResult<int>> GetThermalEvaluationReportAsync(int consumption, string zipCode, int area, double ecoSetpoint, double comfortSetpoint, CancellationToken ct = default);

    // ==========================================
    // Linky/Registration (api/1/linky, api/1/register)
    // ==========================================

    /// <summary>
    /// Get registration info
    /// GET api/1/register/{homeId}
    /// </summary>
    Task<ApiResult<RegistrationDto>> GetRegistrationAsync(string homeId, CancellationToken ct = default);

    /// <summary>
    /// Register home with Linky
    /// PUT api/1/register/{homeId}
    /// Body: { prmId, zipCode, city, street }
    /// </summary>
    Task<ApiResult> RegisterAsync(string homeId, string address, string zipCode, string city, string prmId, CancellationToken ct = default);

    /// <summary>
    /// Search Linky meter
    /// POST api/1/linky
    /// Body: { zipCode, city, street }
    /// </summary>
    Task<ApiResult<LinkyResponseDto>> SearchLinkyAsync(string address, string zipCode, string city, CancellationToken ct = default);

    // ==========================================
    // Diagnostics (api/1/diagnosys)
    // ==========================================

    /// <summary>
    /// Request new diagnostics
    /// POST api/1/diagnosys
    /// Body: { homeId }
    /// Returns diagnostic ID
    /// </summary>
    Task<ApiResult<string>> SendDiagnosysRequestAsync(string homeId, CancellationToken ct = default);

    /// <summary>
    /// Fetch diagnostics result
    /// GET api/1/diagnosys/{diagnosysId}
    /// </summary>
    Task<ApiResult<DiagnosysResponseDto>> FetchDiagnosysAsync(string diagnosysId, CancellationToken ct = default);

    // ==========================================
    // Help/Documentation (api/1/assistance, api/1/usermanual)
    // ==========================================

    /// <summary>
    /// Get help articles
    /// GET api/1/assistance/{lang}
    /// </summary>
    Task<ApiResult<List<HelpItemDto>>> GetHelpAsync(string lang, CancellationToken ct = default);

    /// <summary>
    /// Download user manual PDF
    /// GET api/1/usermanual/{reference}
    /// Returns raw bytes
    /// </summary>
    Task<ApiResult<byte[]>> GetNoticeAsync(string reference, CancellationToken ct = default);
}

/// <summary>
/// Help item DTO for assistance articles
/// </summary>
public class HelpItemDto
{
    public string Title { get; set; } = "";
    public string Content { get; set; } = "";
    public string? Category { get; set; }
}
