using HomeSmartLink.Domain.Enums;
using HomeSmartLink.Domain.ValueObjects;

namespace HomeSmartLink.Domain.Entities;

public class Device : Entity<string>
{
    public string HomeId { get; private set; } = string.Empty;
    public string? RoomId { get; private set; }
    public string Identifier { get; private set; } = string.Empty;
    public string? Reference { get; private set; }
    public DeviceCategory Category { get; private set; }
    public DeviceBrand Brand { get; private set; }
    public string? Color { get; private set; }
    public int? PowerWatts { get; private set; }
    public string? FirmwareVersion { get; private set; }
    public DateTime? InstallationDate { get; private set; }
    public DateTime? LastConnection { get; private set; }

    // Current state
    public ConnectionStatus ConnectionStatus { get; private set; } = ConnectionStatus.Offline;
    public DeviceMode Mode { get; private set; } = DeviceMode.Program;
    public Temperature? AmbientTemperature { get; private set; }
    public Temperature? Setpoint { get; private set; }
    public bool IsBoostActive { get; private set; }
    public bool IsOpenWindowDetected { get; private set; }
    public bool DidRespond { get; private set; }

    private Device() { }

    public static Device Create(
        string id,
        string homeId,
        string identifier,
        DeviceCategory category,
        DeviceBrand brand = DeviceBrand.Unknown)
    {
        return new Device
        {
            Id = id,
            HomeId = homeId,
            Identifier = identifier,
            Category = category,
            Brand = brand,
            InstallationDate = DateTime.UtcNow
        };
    }

    public void AssignToRoom(string roomId)
    {
        RoomId = roomId;
        UpdatedAt = DateTime.UtcNow;
    }

    public void RemoveFromRoom()
    {
        RoomId = null;
        UpdatedAt = DateTime.UtcNow;
    }

    public void UpdateReference(string reference)
    {
        Reference = reference;
        UpdatedAt = DateTime.UtcNow;
    }

    public void UpdatePower(int powerWatts)
    {
        if (powerWatts < 0)
            throw new ArgumentException("Power must be positive", nameof(powerWatts));

        PowerWatts = powerWatts;
        UpdatedAt = DateTime.UtcNow;
    }

    public void UpdateFirmwareVersion(string version)
    {
        FirmwareVersion = version;
        UpdatedAt = DateTime.UtcNow;
    }

    public void UpdateConnectionStatus(ConnectionStatus status)
    {
        ConnectionStatus = status;

        if (status == ConnectionStatus.Online)
            LastConnection = DateTime.UtcNow;

        DidRespond = status == ConnectionStatus.Online;
        UpdatedAt = DateTime.UtcNow;
    }

    public void UpdateMode(DeviceMode mode)
    {
        Mode = mode;
        UpdatedAt = DateTime.UtcNow;
    }

    public void UpdateTemperatures(Temperature? ambient, Temperature? setpoint)
    {
        AmbientTemperature = ambient;
        Setpoint = setpoint;
        UpdatedAt = DateTime.UtcNow;
    }

    public void SetBoostActive(bool active)
    {
        IsBoostActive = active;
        UpdatedAt = DateTime.UtcNow;
    }

    public void SetOpenWindowDetected(bool detected)
    {
        IsOpenWindowDetected = detected;
        UpdatedAt = DateTime.UtcNow;
    }

    public string GetManualUrl() => Category switch
    {
        DeviceCategory.Thermostat => "https://man.wf/FP111S0",
        DeviceCategory.Gateway => "https://man.wf/BHSL32",
        DeviceCategory.WaterHeaterRelay => "https://man.wf/RL1011S0",
        _ => string.Empty
    };
}
