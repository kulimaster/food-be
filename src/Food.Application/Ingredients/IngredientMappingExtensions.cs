using Food.Domain.Ingredients;

namespace Food.Application.Ingredients;

public static class IngredientMappingExtensions
{
    public static IngredientDto ToDto(this Ingredient ingredient) => new(
        ingredient.Id,
        ingredient.Name,
        ingredient.MacrosPer100g.Calories,
        ingredient.MacrosPer100g.ProteinG,
        ingredient.MacrosPer100g.CarbsG,
        ingredient.MacrosPer100g.FatG,
        ingredient.MacrosPer100g.FiberG,
        ingredient.Tags.Select(t => t.Name).ToList());
}
