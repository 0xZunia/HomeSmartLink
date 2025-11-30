using HomeSmartLink.Domain.Enums;
using HomeSmartLink.Domain.ValueObjects;

namespace HomeSmartLink.Domain.Entities;

public class Home : Entity<string>
{
    public string Name { get; private set; } = string.Empty;
    public string? Notes { get; private set; }
    public Address? Address { get; private set; }
    public GeoLocation? Location { get; private set; }
    public bool HasGateway { get; private set; }
    public bool VacationMode { get; private set; }
    public DateTime? VacationReturnDate { get; private set; }

    private readonly List<Room> _rooms = [];
    public IReadOnlyCollection<Room> Rooms => _rooms.AsReadOnly();

    private readonly List<Device> _devices = [];
    public IReadOnlyCollection<Device> Devices => _devices.AsReadOnly();

    public Device? Gateway => _devices.FirstOrDefault(d => d.Category == DeviceCategory.Gateway);

    private Home() { }

    public static Home Create(string id, string name)
    {
        return new Home
        {
            Id = id,
            Name = name
        };
    }

    public void UpdateName(string name)
    {
        Name = name;
        UpdatedAt = DateTime.UtcNow;
    }

    public void UpdateNotes(string? notes)
    {
        Notes = notes;
        UpdatedAt = DateTime.UtcNow;
    }

    public void SetAddress(Address address)
    {
        Address = address;
        UpdatedAt = DateTime.UtcNow;
    }

    public void SetLocation(GeoLocation location)
    {
        Location = location;
        UpdatedAt = DateTime.UtcNow;
    }

    public void EnableVacationMode(DateTime returnDate)
    {
        VacationMode = true;
        VacationReturnDate = returnDate;
        UpdatedAt = DateTime.UtcNow;
    }

    public void DisableVacationMode()
    {
        VacationMode = false;
        VacationReturnDate = null;
        UpdatedAt = DateTime.UtcNow;
    }

    public Room AddRoom(string name)
    {
        if (_rooms.Count >= 8)
            throw new InvalidOperationException("Maximum number of rooms (8) has been reached");

        var room = Room.Create(Guid.NewGuid().ToString(), Id, name, _rooms.Count);
        _rooms.Add(room);
        UpdatedAt = DateTime.UtcNow;
        return room;
    }

    public void RemoveRoom(string roomId)
    {
        var room = _rooms.FirstOrDefault(r => r.Id == roomId);
        if (room is null) return;

        if (_devices.Any(d => d.RoomId == roomId))
            throw new InvalidOperationException("Cannot remove a room that contains devices");

        _rooms.Remove(room);
        UpdatedAt = DateTime.UtcNow;
    }

    public void AddDevice(Device device)
    {
        if (_devices.Count >= 8)
            throw new InvalidOperationException("Maximum number of devices (8) has been reached");

        if (device.Category == DeviceCategory.Gateway)
            HasGateway = true;

        _devices.Add(device);
        UpdatedAt = DateTime.UtcNow;
    }

    public void RemoveDevice(string deviceId)
    {
        var device = _devices.FirstOrDefault(d => d.Id == deviceId);
        if (device is null) return;

        if (device.Category == DeviceCategory.Gateway)
            HasGateway = false;

        _devices.Remove(device);
        UpdatedAt = DateTime.UtcNow;
    }
}
