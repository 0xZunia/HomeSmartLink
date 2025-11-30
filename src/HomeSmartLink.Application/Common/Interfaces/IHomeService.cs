using HomeSmartLink.Application.Common.Models;
using HomeSmartLink.Domain.Entities;
using HomeSmartLink.Domain.Enums;

namespace HomeSmartLink.Application.Common.Interfaces;

public interface IHomeService
{
    Task<ApiResult<List<Home>>> GetUserHomesAsync(CancellationToken ct = default);
    Task<ApiResult<Home>> GetHomeAsync(string homeId, CancellationToken ct = default);
    Task<ApiResult<Home>> CreateHomeAsync(string name, string? notes = null, CancellationToken ct = default);
    Task<ApiResult> UpdateHomeAsync(Home home, CancellationToken ct = default);
    Task<ApiResult> DeleteHomeAsync(string homeId, CancellationToken ct = default);

    // Rooms
    Task<ApiResult<Room>> AddRoomAsync(string homeId, string name, CancellationToken ct = default);
    Task<ApiResult> UpdateRoomAsync(Room room, CancellationToken ct = default);
    Task<ApiResult> DeleteRoomAsync(string homeId, string roomId, CancellationToken ct = default);
    Task<ApiResult> SetRoomModeAsync(string roomId, DeviceMode mode, CancellationToken ct = default);
    Task<ApiResult> SetRoomSetpointsAsync(string roomId, double comfortCelsius, double ecoCelsius, CancellationToken ct = default);

    // Vacation Mode
    Task<ApiResult> EnableVacationModeAsync(string homeId, DateTime returnDate, CancellationToken ct = default);
    Task<ApiResult> DisableVacationModeAsync(string homeId, CancellationToken ct = default);

    // Geofencing
    Task<ApiResult> SendGeofenceActionAsync(string homeId, ProximityAction action, string? description = null, int roomMask = 255, CancellationToken ct = default);
}

public interface IDeviceService
{
    Task<ApiResult<List<Device>>> GetDevicesAsync(string homeId, CancellationToken ct = default);
    Task<ApiResult<Device>> GetDeviceAsync(string deviceId, CancellationToken ct = default);
    Task<ApiResult> SendCommandAsync(string deviceId, DeviceMode mode, double? setpoint = null, CancellationToken ct = default);
    Task<ApiResult> ActivateBoostAsync(string deviceId, int durationMinutes, CancellationToken ct = default);
    Task<ApiResult> BlinkDeviceAsync(string deviceId, CancellationToken ct = default);
    Task<ApiResult<DiagnosysResponseDto>> RunDiagnosticsAsync(string homeId, CancellationToken ct = default);
}

public interface IConsumptionService
{
    Task<ApiResult<int>> GetThermalEvaluationReportAsync(int consumption, string zipCode, int area, double ecoSetpoint, double comfortSetpoint, CancellationToken ct = default);
}

public interface IEcowattService
{
    Task<ApiResult<EcowattDto>> GetCurrentStatusAsync(CancellationToken ct = default);
    Task<ApiResult<List<EcowattDailyForecastDto>>> GetForecastAsync(CancellationToken ct = default);
}
