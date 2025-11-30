namespace HomeSmartLink.Domain.ValueObjects;

public sealed record GeoLocation
{
    public double Latitude { get; init; }
    public double Longitude { get; init; }
    public double? RadiusKm { get; init; }

    public GeoLocation(double latitude, double longitude, double? radiusKm = null)
    {
        if (latitude is < -90 or > 90)
            throw new ArgumentOutOfRangeException(nameof(latitude), "Latitude must be between -90 and 90");

        if (longitude is < -180 or > 180)
            throw new ArgumentOutOfRangeException(nameof(longitude), "Longitude must be between -180 and 180");

        Latitude = latitude;
        Longitude = longitude;
        RadiusKm = radiusKm;
    }

    public double DistanceTo(GeoLocation other)
    {
        const double earthRadiusKm = 6371;

        var dLat = ToRadians(other.Latitude - Latitude);
        var dLon = ToRadians(other.Longitude - Longitude);

        var a = Math.Sin(dLat / 2) * Math.Sin(dLat / 2) +
                Math.Cos(ToRadians(Latitude)) * Math.Cos(ToRadians(other.Latitude)) *
                Math.Sin(dLon / 2) * Math.Sin(dLon / 2);

        var c = 2 * Math.Atan2(Math.Sqrt(a), Math.Sqrt(1 - a));

        return earthRadiusKm * c;
    }

    private static double ToRadians(double degrees) => degrees * Math.PI / 180;
}
