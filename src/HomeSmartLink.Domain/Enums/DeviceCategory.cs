namespace HomeSmartLink.Domain.Enums;

public enum DeviceCategory
{
    Unknown = 0,
    Thermostat = 1,      // FP11 - Fil Pilote
    Gateway = 2,          // BHSL32 - Box Home-SmartLink
    WaterHeaterRelay = 3, // RL10 - Relais Chauffe-eau
    Radiator = 4
}
