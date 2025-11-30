using HomeSmartLink.Domain.Enums;
using HomeSmartLink.Domain.ValueObjects;

namespace HomeSmartLink.Domain.Entities;

public class Room : Entity<string>
{
    public string HomeId { get; private set; } = string.Empty;
    public string Name { get; private set; } = string.Empty;
    public int Position { get; private set; }
    public double? AreaM2 { get; private set; }

    // Temperature settings for Program mode
    public Temperature ComfortSetpoint { get; private set; } = Temperature.FromCelsius(20);
    public Temperature EcoSetpoint { get; private set; } = Temperature.FromCelsius(17);
    public Temperature? MaxTemperatureLimit { get; private set; }

    // Current state (aggregated from devices)
    public DeviceMode CurrentMode { get; private set; } = DeviceMode.Program;
    public Temperature? CurrentTemperature { get; private set; }
    public Temperature? CurrentSetpoint { get; private set; }
    public bool IsBoostActive { get; private set; }
    public DateTime? BoostEndTime { get; private set; }

    private readonly List<Device> _devices = [];
    public IReadOnlyCollection<Device> Devices => _devices.AsReadOnly();

    private Room() { }

    internal static Room Create(string id, string homeId, string name, int position)
    {
        return new Room
        {
            Id = id,
            HomeId = homeId,
            Name = name,
            Position = position
        };
    }

    public void UpdateName(string name)
    {
        Name = name;
        UpdatedAt = DateTime.UtcNow;
    }

    public void SetArea(double areaM2)
    {
        if (areaM2 <= 0)
            throw new ArgumentException("Area must be positive", nameof(areaM2));

        AreaM2 = areaM2;
        UpdatedAt = DateTime.UtcNow;
    }

    public void SetComfortSetpoint(Temperature temperature)
    {
        var celsius = temperature.ToCelsius();
        if (celsius is < 7 or > 30)
            throw new ArgumentException("Temperature must be between 7°C and 30°C");

        if (EcoSetpoint.ToCelsius() >= celsius)
            throw new ArgumentException("Comfort setpoint must be higher than Eco setpoint");

        ComfortSetpoint = temperature;
        UpdatedAt = DateTime.UtcNow;
    }

    public void SetEcoSetpoint(Temperature temperature)
    {
        var celsius = temperature.ToCelsius();
        if (celsius is < 7 or > 30)
            throw new ArgumentException("Temperature must be between 7°C and 30°C");

        if (celsius >= ComfortSetpoint.ToCelsius())
            throw new ArgumentException("Eco setpoint must be lower than Comfort setpoint");

        EcoSetpoint = temperature;
        UpdatedAt = DateTime.UtcNow;
    }

    public void SetMaxTemperatureLimit(Temperature? temperature)
    {
        MaxTemperatureLimit = temperature;
        UpdatedAt = DateTime.UtcNow;
    }

    public void SetMode(DeviceMode mode)
    {
        CurrentMode = mode;

        if (mode != DeviceMode.Boost)
        {
            IsBoostActive = false;
            BoostEndTime = null;
        }

        UpdatedAt = DateTime.UtcNow;
    }

    public void ActivateBoost(TimeSpan duration)
    {
        if (duration.TotalMinutes is < 15 or > 120)
            throw new ArgumentException("Boost duration must be between 15 minutes and 2 hours");

        IsBoostActive = true;
        BoostEndTime = DateTime.UtcNow.Add(duration);
        CurrentMode = DeviceMode.Boost;
        UpdatedAt = DateTime.UtcNow;
    }

    public void DeactivateBoost()
    {
        IsBoostActive = false;
        BoostEndTime = null;
        CurrentMode = DeviceMode.Program;
        UpdatedAt = DateTime.UtcNow;
    }

    public void UpdateCurrentTemperature(Temperature temperature)
    {
        CurrentTemperature = temperature;
        UpdatedAt = DateTime.UtcNow;
    }

    public void UpdateCurrentSetpoint(Temperature temperature)
    {
        CurrentSetpoint = temperature;
        UpdatedAt = DateTime.UtcNow;
    }

    internal void AddDevice(Device device)
    {
        _devices.Add(device);
    }

    internal void RemoveDevice(Device device)
    {
        _devices.Remove(device);
    }
}
