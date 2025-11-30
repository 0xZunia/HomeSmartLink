namespace HomeSmartLink.Domain.Enums;

public enum ConnectionStatus
{
    Offline = 0,
    Connecting = 1,
    Online = 2,
    Scanning = 3,
    Configuring = 4,
    Peering = 5,
    Failed = 6
}
