using Food.Domain.Common;
using Food.Domain.Ingredients;
using Food.Domain.Recipes;

namespace Food.Domain.Logging;

// Represents "either a recipe portion or a raw ingredient quantity, exactly one" -
// shared by MealLog, FavoriteMeal and PlannedMeal so the either/or rule (a DB CHECK
// constraint at the persistence layer) exists in exactly one place in the domain.
public sealed class LoggableItem
{
    public Recipe? Recipe { get; }
    public decimal? ServingsCount { get; }
    public Ingredient? Ingredient { get; }
    public Quantity? Quantity { get; }

    // Parameterless: EF Core materialization only. The 4-arg constructor below can't be
    // constructor-bound by EF because Recipe/Ingredient/Quantity are navigations, not
    // scalar properties - EF uses this ctor instead and sets backing fields via reflection.
    private LoggableItem()
    {
    }

    private LoggableItem(Recipe? recipe, decimal? servingsCount, Ingredient? ingredient, Quantity? quantity)
    {
        Recipe = recipe;
        ServingsCount = servingsCount;
        Ingredient = ingredient;
        Quantity = quantity;
    }

    public static LoggableItem FromRecipe(Recipe recipe, decimal servingsCount)
    {
        ArgumentNullException.ThrowIfNull(recipe);
        if (servingsCount <= 0)
        {
            throw new ArgumentOutOfRangeException(nameof(servingsCount), servingsCount, "Servings count must be positive.");
        }

        return new LoggableItem(recipe, servingsCount, null, null);
    }

    public static LoggableItem FromIngredient(Ingredient ingredient, Quantity quantity)
    {
        ArgumentNullException.ThrowIfNull(ingredient);
        ArgumentNullException.ThrowIfNull(quantity);
        return new LoggableItem(null, null, ingredient, quantity);
    }

    public MacroBreakdown Macros() => Recipe is not null
        ? Recipe.MacrosPerServing().Scale(ServingsCount!.Value)
        : Ingredient!.MacrosForQuantity(Quantity!);
}
