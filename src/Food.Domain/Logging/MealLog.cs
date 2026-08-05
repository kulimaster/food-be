using Food.Domain.Common;
using Food.Domain.Enums;

namespace Food.Domain.Logging;

public sealed class MealLog : Entity
{
    public long UserId { get; private set; }
    public DateOnly LogDate { get; private set; }
    public MealSlot MealSlot { get; private set; }
    public LoggableItem Item { get; private set; }
    public DateTimeOffset LoggedAt { get; private set; }

    private MealLog()
    {
        Item = null!;
    }

    public MealLog(long userId, DateOnly logDate, MealSlot mealSlot, LoggableItem item, DateTimeOffset loggedAt)
    {
        UserId = userId;
        LogDate = logDate;
        MealSlot = mealSlot;
        Item = item ?? throw new ArgumentNullException(nameof(item));
        LoggedAt = loggedAt;
    }

    public MacroBreakdown Macros() => Item.Macros();
}
