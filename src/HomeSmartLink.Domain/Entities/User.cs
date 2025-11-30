using HomeSmartLink.Domain.Enums;

namespace HomeSmartLink.Domain.Entities;

public class User : Entity<string>
{
    public string Email { get; private set; } = string.Empty;
    public string FirstName { get; private set; } = string.Empty;
    public string LastName { get; private set; } = string.Empty;
    public string FullName => $"{FirstName} {LastName}".Trim();

    private readonly List<HomeAccess> _homeAccesses = [];
    public IReadOnlyCollection<HomeAccess> HomeAccesses => _homeAccesses.AsReadOnly();

    private User() { }

    public static User Create(string id, string email, string firstName, string lastName)
    {
        return new User
        {
            Id = id,
            Email = email,
            FirstName = firstName,
            LastName = lastName
        };
    }

    public void UpdateProfile(string firstName, string lastName)
    {
        FirstName = firstName;
        LastName = lastName;
        UpdatedAt = DateTime.UtcNow;
    }

    public void AddHomeAccess(Home home, UserRole role)
    {
        if (_homeAccesses.Any(ha => ha.HomeId == home.Id))
            return;

        _homeAccesses.Add(new HomeAccess(Id, home.Id, role));
    }
}

public class HomeAccess
{
    public string UserId { get; private set; }
    public string HomeId { get; private set; }
    public UserRole Role { get; private set; }
    public DateTime? ExpiresAt { get; private set; }

    public HomeAccess(string userId, string homeId, UserRole role, DateTime? expiresAt = null)
    {
        UserId = userId;
        HomeId = homeId;
        Role = role;
        ExpiresAt = expiresAt;
    }

    public bool IsExpired => ExpiresAt.HasValue && ExpiresAt.Value < DateTime.UtcNow;
}
