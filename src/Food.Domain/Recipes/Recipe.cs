using Food.Domain.Common;
using Food.Domain.Ingredients;

namespace Food.Domain.Recipes;

public sealed class Recipe : Entity
{
    private readonly List<RecipeIngredient> _ingredients = new();

    public string Name { get; private set; }
    public int Servings { get; private set; }
    public long CreatedByUserId { get; private set; }
    public DateTimeOffset CreatedAt { get; private set; }
    public IReadOnlyCollection<RecipeIngredient> Ingredients => _ingredients.AsReadOnly();

    private Recipe()
    {
        Name = null!;
    }

    public Recipe(string name, int servings, long createdByUserId, DateTimeOffset createdAt)
    {
        Name = Guard.NotEmpty(name, nameof(name));
        Servings = Guard.Positive(servings, nameof(servings));
        CreatedByUserId = createdByUserId;
        CreatedAt = createdAt;
    }

    public void AddIngredient(Ingredient ingredient, Quantity quantity) =>
        _ingredients.Add(new RecipeIngredient(ingredient, quantity));

    public void RemoveIngredient(long ingredientId) =>
        _ingredients.RemoveAll(ri => ri.IngredientId == ingredientId);

    public MacroBreakdown TotalMacros() =>
        _ingredients.Aggregate(MacroBreakdown.Zero, (sum, ri) => sum + ri.Macros());

    public MacroBreakdown MacrosPerServing() => TotalMacros().Scale(1m / Servings);
}
