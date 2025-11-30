namespace HomeSmartLink.Domain.ValueObjects;

public sealed record Address
{
    public string Street { get; init; } = string.Empty;
    public string City { get; init; } = string.Empty;
    public string ZipCode { get; init; } = string.Empty;
    public string? Country { get; init; }

    public override string ToString() => $"{Street}, {ZipCode} {City}";
}
