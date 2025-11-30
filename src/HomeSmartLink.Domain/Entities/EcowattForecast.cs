using HomeSmartLink.Domain.Enums;

namespace HomeSmartLink.Domain.Entities;

public class EcowattForecast : Entity<string>
{
    public DateTime Date { get; private set; }
    public EcowattLevel Level { get; private set; }
    public string Message { get; private set; } = string.Empty;
    public string Description { get; private set; } = string.Empty;

    private EcowattForecast() { }

    public static EcowattForecast Create(string id, DateTime date, EcowattLevel level, string message, string description)
    {
        return new EcowattForecast
        {
            Id = id,
            Date = date,
            Level = level,
            Message = message,
            Description = description
        };
    }

    public static string GetMessageForLevel(EcowattLevel level) => level switch
    {
        EcowattLevel.Normal => "Production décarbonée",
        EcowattLevel.NoAlert => "Pas d'alerte ECOWATT",
        EcowattLevel.Orange => "Le système électrique est tendu. Les écogestes sont les bienvenus.",
        EcowattLevel.Red => "Le système électrique est très tendu. Des coupures sont inévitables si nous ne baissons pas notre consommation.",
        _ => string.Empty
    };

    public static string GetDescriptionForLevel(EcowattLevel level) => level switch
    {
        EcowattLevel.Normal => "Production décarbonée",
        EcowattLevel.NoAlert => string.Empty,
        EcowattLevel.Orange => "Le système électrique est tendu",
        EcowattLevel.Red => "Le système électrique est très tendu",
        _ => string.Empty
    };
}
