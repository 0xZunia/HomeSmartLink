namespace HomeSmartLink.Domain.ValueObjects;

public sealed record Temperature
{
    public double Value { get; }
    public TemperatureUnit Unit { get; }

    private Temperature(double value, TemperatureUnit unit)
    {
        Value = value;
        Unit = unit;
    }

    public static Temperature FromCelsius(double celsius) => new(celsius, TemperatureUnit.Celsius);
    public static Temperature FromFahrenheit(double fahrenheit) => new(fahrenheit, TemperatureUnit.Fahrenheit);

    public double ToCelsius() => Unit switch
    {
        TemperatureUnit.Celsius => Value,
        TemperatureUnit.Fahrenheit => (Value - 32) * 5 / 9,
        _ => throw new InvalidOperationException("Unknown temperature unit")
    };

    public double ToFahrenheit() => Unit switch
    {
        TemperatureUnit.Fahrenheit => Value,
        TemperatureUnit.Celsius => Value * 9 / 5 + 32,
        _ => throw new InvalidOperationException("Unknown temperature unit")
    };

    public override string ToString() => Unit switch
    {
        TemperatureUnit.Celsius => $"{Value:F1}°C",
        TemperatureUnit.Fahrenheit => $"{Value:F1}°F",
        _ => $"{Value:F1}"
    };
}

public enum TemperatureUnit
{
    Celsius,
    Fahrenheit
}
