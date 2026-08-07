using Food.Domain.Common;

namespace Food.Application.Recipes;

public sealed record RecipeIngredientDto(long IngredientId, string IngredientName, decimal QuantityGrams);

public sealed record RecipeDto(
    long Id,
    string Name,
    int Servings,
    long CreatedByUserId,
    IReadOnlyCollection<RecipeIngredientDto> Ingredients,
    MacroBreakdown TotalMacros,
    MacroBreakdown MacrosPerServing);
