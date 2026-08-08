using Food.Domain.Planning;

namespace Food.Application.Planning;

public static class PlannedMealMappingExtensions
{
    public static PlannedMealDto ToDto(this PlannedMeal plannedMeal) => new(
        plannedMeal.Id,
        plannedMeal.UserId,
        plannedMeal.PlanDate,
        plannedMeal.MealSlot,
        plannedMeal.Item.Recipe?.Id,
        plannedMeal.Item.Recipe?.Name,
        plannedMeal.Item.ServingsCount,
        plannedMeal.Item.Ingredient?.Id,
        plannedMeal.Item.Ingredient?.Name,
        plannedMeal.Item.Quantity?.Grams,
        plannedMeal.Macros(),
        plannedMeal.CreatedAt);
}
