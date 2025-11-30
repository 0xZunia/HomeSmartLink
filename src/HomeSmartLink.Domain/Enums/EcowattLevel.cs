namespace HomeSmartLink.Domain.Enums;

public enum EcowattLevel
{
    Normal = 0,         // Pas d'alerte - Production décarbonée
    NoAlert = 1,        // Pas d'alerte
    Orange = 2,         // Système électrique tendu
    Red = 3             // Système électrique très tendu
}
