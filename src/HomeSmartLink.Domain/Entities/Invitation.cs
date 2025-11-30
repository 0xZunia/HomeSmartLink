using HomeSmartLink.Domain.Enums;

namespace HomeSmartLink.Domain.Entities;

public class Invitation : Entity<string>
{
    public string HomeId { get; private set; } = string.Empty;
    public string Email { get; private set; } = string.Empty;
    public UserRole Role { get; private set; }
    public DateTime? ExpirationDate { get; private set; }
    public string? Code { get; private set; }
    public bool IsAccepted { get; private set; }
    public DateTime? AcceptedAt { get; private set; }

    private Invitation() { }

    public static Invitation Create(string id, string homeId, string email, UserRole role, DateTime? expirationDate = null)
    {
        return new Invitation
        {
            Id = id,
            HomeId = homeId,
            Email = email,
            Role = role,
            ExpirationDate = expirationDate,
            Code = GenerateCode()
        };
    }

    public bool IsExpired => ExpirationDate.HasValue && ExpirationDate.Value < DateTime.UtcNow;

    public void Accept()
    {
        if (IsExpired)
            throw new InvalidOperationException("Invitation has expired");

        if (IsAccepted)
            throw new InvalidOperationException("Invitation has already been accepted");

        IsAccepted = true;
        AcceptedAt = DateTime.UtcNow;
        UpdatedAt = DateTime.UtcNow;
    }

    private static string GenerateCode()
    {
        return Guid.NewGuid().ToString("N")[..8].ToUpperInvariant();
    }
}
