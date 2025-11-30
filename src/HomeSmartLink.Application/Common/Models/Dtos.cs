using System.Text.Json.Serialization;
using System.Text.RegularExpressions;

namespace HomeSmartLink.Application.Common.Models;

// ==========================================
// Authentication DTOs (from ThrottledWebClient.AuthenticationResponse)
// ==========================================

public class AuthResponse
{
    [JsonPropertyName("token")]
    public string Token { get; set; } = "";

    [JsonPropertyName("invitation")]
    public InvitationDto? Invitation { get; set; }
}

// ==========================================
// Invitation DTOs (from ThrottledWebClient.WInvitation)
// ==========================================

public class InvitationDto
{
    [JsonPropertyName("_id")]
    public string Id { get; set; } = "";

    [JsonPropertyName("email")]
    public string? Email { get; set; }

    [JsonPropertyName("role")]
    public int Role { get; set; }

    [JsonPropertyName("expirationDate")]
    public DateTime? ExpirationDate { get; set; }

    [JsonPropertyName("name")]
    public string? HomeName { get; set; }

    [JsonPropertyName("homeid")]
    public string HomeId { get; set; } = "";

    [JsonPropertyName("invitedBy")]
    public string? InvitedBy { get; set; }

    [JsonPropertyName("notes")]
    public string? Notes { get; set; }
}

// ==========================================
// Home DTOs (from SmartLink.Core.Model.Home)
// ==========================================

public class HomeDto
{
    [JsonPropertyName("_id")]
    public string Id { get; set; } = "";

    [JsonPropertyName("meshCryptogram")]
    public string? MeshData { get; set; }

    [JsonPropertyName("client")]
    public string? ProIdentifier { get; set; }

    [JsonPropertyName("location")]
    public LocationDto? Location { get; set; }

    [JsonPropertyName("rooms")]
    public List<RoomDto> Rooms { get; set; } = [];

    [JsonPropertyName("vacationReturnDate")]
    public DateTime? VacationReturnDate { get; set; }

    [JsonPropertyName("hasGateway")]
    public bool HasGateway { get; set; }
}

public class LocationDto
{
    [JsonPropertyName("latitude")]
    public double Latitude { get; set; }

    [JsonPropertyName("longitude")]
    public double Longitude { get; set; }

    [JsonPropertyName("locality")]
    public string? Locality { get; set; }

    [JsonPropertyName("postalCode")]
    public string? ZipCode { get; set; }
}

// ==========================================
// Room DTOs (from SmartLink.Core.Model.Room)
// ==========================================

public record RoomDto
{
    [JsonPropertyName("roomNumber")]
    public int Position { get; set; }

    [JsonPropertyName("name")]
    public string Name { get; set; } = "";

    [JsonPropertyName("area")]
    public int Area { get; set; }

    [JsonPropertyName("accessories")]
    public List<AccessoryDto> Accessories { get; set; } = [];

    [JsonPropertyName("events")]
    public List<ProgramRangeDto> Programs { get; set; } = [];

    [JsonPropertyName("ecopilotEnabled")]
    public bool UsesEcopilot { get; set; } = true;

    [JsonPropertyName("heaterSettings")]
    public HeaterSettingsDto Settings { get; set; } = new();

    [JsonPropertyName("openWindowsDetectionEnabled")]
    public bool OpenWindowsDetectionEnabled { get; set; }

    // Calculated properties for MAUI/Blazor UI
    [JsonIgnore]
    public int RoomId => Position;

    [JsonIgnore]
    public List<DeviceDataDto> Devices { get; set; } = [];

    [JsonIgnore]
    public double AverageTemperature => Devices.Count > 0 ? Math.Round(Devices.Average(d => d.Ambient), 1) : 0;

    [JsonIgnore]
    public double AverageSetpoint => Devices.Count > 0 ? Math.Round(Devices.Average(d => d.Setpoint), 1) : 0;

    [JsonIgnore]
    public int TotalDailyHours => Devices.Sum(d => d.DailyHours);
}

public class HeaterSettingsDto
{
    [JsonPropertyName("manualValue")]
    public double ManualValue { get; set; } = 18.5;

    [JsonPropertyName("economyValue")]
    public double EconomyValue { get; set; } = 15.5;

    [JsonPropertyName("standardValue")]
    public double StandardValue { get; set; } = 19.0;

    [JsonPropertyName("currentMode")]
    public int Mode { get; set; } = 3; // Manual

    [JsonPropertyName("limitationSetPoint")]
    public double LimitationSetPoint { get; set; } = 30.0;
}

public class ProgramRangeDto
{
    [JsonPropertyName("eventID")]
    public int Tag { get; set; }

    [JsonPropertyName("mode")]
    public int Mode { get; set; } = 1; // Standard

    [JsonPropertyName("startHour")]
    public int StartHour { get; set; }

    [JsonPropertyName("startMinute")]
    public int StartMinute { get; set; }

    [JsonPropertyName("endHour")]
    public int EndHour { get; set; }

    [JsonPropertyName("endMinute")]
    public int EndMinute { get; set; }

    [JsonPropertyName("reccurency")]
    public int ActiveDays { get; set; } // Bitwise enum for Days
}

// ==========================================
// Accessory/Device DTOs (from SmartLink.Core.Model.Accessory)
// ==========================================

public class AccessoryDto
{
    [JsonPropertyName("header")]
    public string Identifier { get; set; } = "";

    [JsonPropertyName("retailer")]
    public string? Retailer { get; set; }

    [JsonPropertyName("category")]
    public int Category { get; set; }

    [JsonPropertyName("color")]
    public int? Color { get; set; }

    [JsonPropertyName("powerRange")]
    public int Power { get; set; }

    [JsonPropertyName("reference")]
    public string? Reference { get; set; }

    [JsonPropertyName("softwareVersion")]
    public string? FirmwareVersion { get; set; }

    [JsonPropertyName("creationDate")]
    public DateTime? CreationDate { get; set; }

    [JsonPropertyName("capabilities")]
    public int Capabilities { get; set; }
}

// ==========================================
// Device Status DTOs (from ThrottledWebClient.DeviceStatusResponse)
// ==========================================

public class DeviceStatusResponseDto
{
    [JsonPropertyName("homeId")]
    public string HomeId { get; set; } = "";

    [JsonPropertyName("status")]
    public List<string> DevicesStatus { get; set; } = [];
}

/// <summary>
/// Parsed device data from status string format: R7D1M3S4C44A41J10W50Y100
/// R=Room, D=Device position, M=Mode, S=State, C=Setpoint*2, A=Ambient*2, J=DailyHours, W=WeeklyHours, Y=YearlyHours
/// Note: Some formats may have T prefix (TC for temp consigne, TA for temp ambient)
/// </summary>
public class DeviceDataDto
{
    public int RoomId { get; set; }
    public int Position { get; set; }
    public int Mode { get; set; }
    public int State { get; set; }
    public double Setpoint { get; set; }
    public double Ambient { get; set; }
    public int DailyHours { get; set; }
    public int WeeklyHours { get; set; }
    public int MonthlyHours { get; set; }
    public int YearlyHours { get; set; }

    /// <summary>
    /// Parses device status string format: R7D1M3S4C44A41J10W50Y100
    /// Also handles format with T prefix: R7D1M3S4TC44TA41
    /// </summary>
    public static DeviceDataDto Parse(string dataString)
    {
        try
        {
            // Remove T prefix if present (TC -> C, TA -> A)
            var cleaned = dataString.Replace("TC", "C").Replace("TA", "A");

            // Replace markers with spaces and split
            var replaced = Regex.Replace(cleaned, "(R|D|M|S|C|A|J|W|Y)", " ").Trim();
            var parts = replaced.Split(' ', StringSplitOptions.RemoveEmptyEntries);

            // Prepare default values array
            var values = new string[10];
            Array.Fill(values, "0");

            // Copy parsed values
            for (int i = 0; i < Math.Min(parts.Length, 10); i++)
            {
                // Filter out any non-numeric values
                if (int.TryParse(parts[i], out _) || double.TryParse(parts[i], out _))
                {
                    values[i] = parts[i];
                }
            }

            return new DeviceDataDto
            {
                RoomId = int.TryParse(values[0], out var r) ? r : 0,
                Position = int.TryParse(values[1], out var p) ? p : 0,
                Mode = int.TryParse(values[2], out var m) ? m : 0,
                State = int.TryParse(values[3], out var s) ? s : 0,
                Setpoint = double.TryParse(values[4], out var sp) ? sp / 2.0 : 0,
                Ambient = double.TryParse(values[5], out var a) ? a / 2.0 : 0,
                DailyHours = int.TryParse(values[6], out var d) ? d : 0,
                WeeklyHours = int.TryParse(values[7], out var w) ? w : 0,
                MonthlyHours = int.TryParse(values[8], out var mo) ? mo : 0,
                YearlyHours = int.TryParse(values[9], out var y) ? y : 0
            };
        }
        catch
        {
            // Return default if parsing fails
            return new DeviceDataDto();
        }
    }
}

// ==========================================
// Gateway DTOs (from ThrottledWebClient.FetchGateway)
// ==========================================

public class GatewayDto
{
    [JsonPropertyName("homeId")]
    public string HomeId { get; set; } = "";

    [JsonPropertyName("hasGateway")]
    public bool HasGateway { get; set; }

    [JsonPropertyName("identifier")]
    public string Identifier { get; set; } = "";

    [JsonPropertyName("installation")]
    public DateTime? Installation { get; set; }

    [JsonPropertyName("lastConnection")]
    public DateTime? LastConnection { get; set; }

    public string Version { get; set; } = "1.15";
}

// ==========================================
// Ecowatt DTOs (from ThrottledWebClient.EcowattForcastResponse)
// ==========================================

public class EcowattDto
{
    [JsonPropertyName("forecast")]
    public List<EcowattDailyForecastDto> Forecast { get; set; } = [];

    [JsonPropertyName("todayHourly")]
    public List<EcowattHourlyForecastDto> TodayHourly { get; set; } = [];

    public int CurrentHour { get; set; }
    public int Today { get; set; }
    public int Current { get; set; }
}

public class EcowattDailyForecastDto
{
    [JsonPropertyName("date")]
    public string Date { get; set; } = "";

    [JsonPropertyName("alertLevel")]
    public int AlertLevel { get; set; }

    [JsonPropertyName("hourlyData")]
    public List<EcowattHourlyForecastDto> HourlyData { get; set; } = [];
}

public class EcowattHourlyForecastDto
{
    [JsonPropertyName("hour")]
    public int Hour { get; set; }

    [JsonPropertyName("alertLevel")]
    public int AlertLevel { get; set; }
}

// ==========================================
// Registration DTOs (from ThrottledWebClient.Registration)
// ==========================================

public class RegistrationDto
{
    [JsonPropertyName("isRegistered")]
    public bool IsRegistered { get; set; }

    [JsonPropertyName("homeId")]
    public string HomeId { get; set; } = "";

    [JsonPropertyName("firstName")]
    public string? FirstName { get; set; }

    [JsonPropertyName("lastName")]
    public string? LastName { get; set; }

    [JsonPropertyName("address")]
    public string? Address { get; set; }

    [JsonPropertyName("zipCode")]
    public string? ZipCode { get; set; }

    [JsonPropertyName("city")]
    public string? City { get; set; }

    [JsonPropertyName("prmId")]
    public string? PrmId { get; set; }

    [JsonPropertyName("_created_at")]
    public string? Date { get; set; }
}

// ==========================================
// Linky DTOs (from ThrottledWebClient.LinkyResponse)
// ==========================================

public class LinkyResponseDto
{
    [JsonPropertyName("success")]
    public bool Success { get; set; }

    [JsonPropertyName("reason")]
    public string? Reason { get; set; }

    [JsonPropertyName("details")]
    public string? Details { get; set; }

    [JsonPropertyName("prmId")]
    public string? PrmId { get; set; }
}

// ==========================================
// Diagnostics DTOs (from ThrottledWebClient.DiagnosysResponse)
// ==========================================

public class DiagnosysResponseDto
{
    [JsonPropertyName("_id")]
    public string Id { get; set; } = "";

    [JsonPropertyName("outdated")]
    public bool IsOutdated { get; set; }

    [JsonPropertyName("acknowledge")]
    public bool IsAcknowledge { get; set; }

    [JsonPropertyName("deviceStatus")]
    public List<DeviceDataDto> DeviceStatus { get; set; } = [];
}

// ==========================================
// Consumption DTOs (from ThrottledWebClient.GetThermalEvaluationReport)
// ==========================================

public class ConsumptionRequestDto
{
    [JsonPropertyName("zipCode")]
    public string ZipCode { get; set; } = "";

    [JsonPropertyName("ecoSP")]
    public double EcoSetpoint { get; set; }

    [JsonPropertyName("confSP")]
    public double ComfortSetpoint { get; set; }

    [JsonPropertyName("area")]
    public int Area { get; set; }

    [JsonPropertyName("conso")]
    public int Consumption { get; set; }
}

public class ConsumptionResponseDto
{
    [JsonPropertyName("nbStars")]
    public int Stars { get; set; }
}

// ==========================================
// Geofencing DTOs (from ThrottledWebClient.SendGeofenceAction)
// ==========================================

public class GeofencingRequestDto
{
    [JsonPropertyName("action")]
    public int Action { get; set; }

    [JsonPropertyName("homeId")]
    public string HomeId { get; set; } = "";

    [JsonPropertyName("description")]
    public string? Description { get; set; }

    [JsonPropertyName("rooms")]
    public int RoomMask { get; set; } = 255;
}

// ==========================================
// Direct Action DTOs (from ThrottledWebClient.SendDirectAction)
// ==========================================

public class DirectActionRequestDto
{
    [JsonPropertyName("action")]
    public string? ActionPayload { get; set; }

    [JsonPropertyName("homeId")]
    public string HomeId { get; set; } = "";

    [JsonPropertyName("description")]
    public string? Description { get; set; }
}

// ==========================================
// Enums (from SmartLink.Core.Model)
// ==========================================

public enum HeaterMode
{
    Stop = 1,
    Antifreeze = 2,
    Manual = 3,
    Prog = 4
}

public enum DeviceCategory
{
    Heater = 1,
    TowelDryer = 2,
    Fireplace = 3,
    Gateway = 4,
    Module = 5,
    Furnace = 6
}

public enum ProximityAction
{
    Nothing = 0,
    Stop = 1,
    Antifreeze = 2,
    Manual = 3,
    Prog = 4
}

public enum ProgramMode
{
    Standard = 1,
    Boost = 2,
    Fan = 3
}

[Flags]
public enum DaysOfWeek
{
    Monday = 1,
    Tuesday = 2,
    Wednesday = 4,
    Thursday = 8,
    Friday = 16,
    Saturday = 32,
    Sunday = 64
}
