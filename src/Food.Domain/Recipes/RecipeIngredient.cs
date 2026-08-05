using Food.Domain.Common;
using Food.Domain.Ingredients;

namespace Food.Domain.Recipes;

public sealed class RecipeIngredient : Entity
{
    public Ingredient Ingredient { get; private set; }
    public long IngredientId { get; private set; }
    public Quantity Quantity { get; private set; }

    private RecipeIngredient()
    {
        Ingredient = null!;
        Quantity = null!;
    }

    // Internal: only Recipe.AddIngredient should create these, to keep the recipe as the aggregate root.
    internal RecipeIngredient(Ingredient ingredient, Quantity quantity)
    {
        Ingredient = ingredient ?? throw new ArgumentNullException(nameof(ingredient));
        IngredientId = ingredient.Id;
        Quantity = quantity ?? throw new ArgumentNullException(nameof(quantity));
    }

    public MacroBreakdown Macros() => Ingredient.MacrosForQuantity(Quantity);
}
