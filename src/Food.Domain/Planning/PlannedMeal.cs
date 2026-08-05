using Food.Domain.Common;
using Food.Domain.Enums;
using Food.Domain.Logging;

namespace Food.Domain.Planning;

public sealed class PlannedMeal : Entity
{
    public long UserId { get; private set; }
    public DateOnly PlanDate { get; private set; }
    public MealSlot MealSlot { get; private set; }
    public LoggableItem Item { get; private set; }
    public DateTimeOffset CreatedAt { get; private set; }

    private PlannedMeal()
    {
        Item = null!;
    }

    public PlannedMeal(long userId, DateOnly planDate, MealSlot mealSlot, LoggableItem item, DateTimeOffset createdAt)
    {
        UserId = userId;
        PlanDate = planDate;
        MealSlot = mealSlot;
        Item = item ?? throw new ArgumentNullException(nameof(item));
        CreatedAt = createdAt;
    }

    public MacroBreakdown Macros() => Item.Macros();
}
