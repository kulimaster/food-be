using Food.Domain.Common;

namespace Food.Domain.Users;

public sealed class User : Entity
{
    public string Email { get; private set; }
    public string DisplayName { get; private set; }
    public string Timezone { get; private set; }

    private User()
    {
        Email = null!;
        DisplayName = null!;
        Timezone = null!;
    }

    public User(string email, string displayName, string timezone)
    {
        Email = Guard.NotEmpty(email, nameof(email));
        DisplayName = Guard.NotEmpty(displayName, nameof(displayName));
        Timezone = Guard.NotEmpty(timezone, nameof(timezone));
    }

    public void Rename(string displayName) => DisplayName = Guard.NotEmpty(displayName, nameof(displayName));

    public void ChangeTimezone(string timezone) => Timezone = Guard.NotEmpty(timezone, nameof(timezone));
}
