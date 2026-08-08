using Food.Domain.Logging;

namespace Food.Application.Logging;

public static class MealLogMappingExtensions
{
    public static MealLogDto ToDto(this MealLog mealLog) => new(
        mealLog.Id,
        mealLog.UserId,
        mealLog.LogDate,
        mealLog.MealSlot,
        mealLog.Item.Recipe?.Id,
        mealLog.Item.Recipe?.Name,
        mealLog.Item.ServingsCount,
        mealLog.Item.Ingredient?.Id,
        mealLog.Item.Ingredient?.Name,
        mealLog.Item.Quantity?.Grams,
        mealLog.Macros(),
        mealLog.LoggedAt);
}
