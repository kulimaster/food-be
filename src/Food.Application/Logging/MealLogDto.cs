using Food.Domain.Common;
using Food.Domain.Enums;

namespace Food.Application.Logging;

public sealed record MealLogDto(
    long Id,
    long UserId,
    DateOnly LogDate,
    MealSlot MealSlot,
    long? RecipeId,
    string? RecipeName,
    decimal? ServingsCount,
    long? IngredientId,
    string? IngredientName,
    decimal? QuantityGrams,
    MacroBreakdown Macros,
    DateTimeOffset LoggedAt);
