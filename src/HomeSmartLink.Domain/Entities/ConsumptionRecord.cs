namespace HomeSmartLink.Domain.Entities;

public class ConsumptionRecord : Entity<string>
{
    public string HomeId { get; private set; } = string.Empty;
    public string? RoomId { get; private set; }
    public string? DeviceId { get; private set; }
    public DateTime Date { get; private set; }
    public double ConsumptionKwh { get; private set; }
    public ConsumptionPeriod Period { get; private set; }

    private ConsumptionRecord() { }

    public static ConsumptionRecord Create(
        string id,
        string homeId,
        DateTime date,
        double consumptionKwh,
        ConsumptionPeriod period,
        string? roomId = null,
        string? deviceId = null)
    {
        return new ConsumptionRecord
        {
            Id = id,
            HomeId = homeId,
            Date = date,
            ConsumptionKwh = consumptionKwh,
            Period = period,
            RoomId = roomId,
            DeviceId = deviceId
        };
    }
}

public enum ConsumptionPeriod
{
    Hour,
    Day,
    Week,
    Month,
    Year
}
