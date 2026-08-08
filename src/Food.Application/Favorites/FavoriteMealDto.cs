using Food.Domain.Common;

namespace Food.Application.Favorites;

public sealed record FavoriteMealDto(
    long Id,
    long UserId,
    string DisplayName,
    long? RecipeId,
    string? RecipeName,
    decimal? ServingsCount,
    long? IngredientId,
    string? IngredientName,
    decimal? QuantityGrams,
    MacroBreakdown Macros,
    DateTimeOffset CreatedAt);
