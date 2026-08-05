using Food.Domain.Common;

namespace Food.Domain.Logging;

public sealed class FavoriteMeal : Entity
{
    public long UserId { get; private set; }
    public string DisplayName { get; private set; }
    public LoggableItem Item { get; private set; }
    public DateTimeOffset CreatedAt { get; private set; }

    private FavoriteMeal()
    {
        DisplayName = null!;
        Item = null!;
    }

    public FavoriteMeal(long userId, string displayName, LoggableItem item, DateTimeOffset createdAt)
    {
        UserId = userId;
        DisplayName = Guard.NotEmpty(displayName, nameof(displayName));
        Item = item ?? throw new ArgumentNullException(nameof(item));
        CreatedAt = createdAt;
    }

    public MacroBreakdown Macros() => Item.Macros();
}
