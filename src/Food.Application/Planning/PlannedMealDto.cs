using Food.Domain.Common;
using Food.Domain.Enums;

namespace Food.Application.Planning;

public sealed record PlannedMealDto(
    long Id,
    long UserId,
    DateOnly PlanDate,
    MealSlot MealSlot,
    long? RecipeId,
    string? RecipeName,
    decimal? ServingsCount,
    long? IngredientId,
    string? IngredientName,
    decimal? QuantityGrams,
    MacroBreakdown Macros,
    DateTimeOffset CreatedAt);
