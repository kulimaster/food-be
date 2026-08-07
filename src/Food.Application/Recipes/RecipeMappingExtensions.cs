using Food.Domain.Recipes;

namespace Food.Application.Recipes;

public static class RecipeMappingExtensions
{
    public static RecipeDto ToDto(this Recipe recipe) => new(
        recipe.Id,
        recipe.Name,
        recipe.Servings,
        recipe.CreatedByUserId,
        recipe.Ingredients
            .Select(ri => new RecipeIngredientDto(ri.IngredientId, ri.Ingredient.Name, ri.Quantity.Grams))
            .ToList(),
        recipe.TotalMacros(),
        recipe.MacrosPerServing());
}
