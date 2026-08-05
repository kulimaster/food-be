using Food.Domain.Common;
using Food.Domain.Ingredients;
using Food.Domain.Recipes;

namespace Food.Domain.Tests.Recipes;

[TestFixture]
public class RecipeTests
{
    private static Ingredient CreateIngredient(string name, MacroBreakdown macrosPer100g) => new(
        name, macrosPer100g, createdByUserId: 1, createdAt: DateTimeOffset.UtcNow);

    [Test]
    public void TotalMacros_SumsMacrosAcrossAllIngredients()
    {
        var recipe = new Recipe("Protein Bowl", servings: 2, createdByUserId: 1, createdAt: DateTimeOffset.UtcNow);
        var chicken = CreateIngredient("Chicken breast", new MacroBreakdown(165, 31, 0, 3.6m, 0));
        var rice = CreateIngredient("Rice", new MacroBreakdown(130, 2.7m, 28m, 0.3m, 0.4m));

        recipe.AddIngredient(chicken, new Quantity(200)); // x2
        recipe.AddIngredient(rice, new Quantity(150)); // x1.5

        var total = recipe.TotalMacros();

        Assert.That(total, Is.EqualTo(new MacroBreakdown(525, 66.05m, 42m, 7.65m, 0.6m)));
    }

    [Test]
    public void MacrosPerServing_DividesTotalMacrosByServings()
    {
        var recipe = new Recipe("Protein Bowl", servings: 2, createdByUserId: 1, createdAt: DateTimeOffset.UtcNow);
        var chicken = CreateIngredient("Chicken breast", new MacroBreakdown(165, 31, 0, 3.6m, 0));
        recipe.AddIngredient(chicken, new Quantity(200)); // total: 330 kcal / 62g protein / 0 / 7.2 / 0

        var perServing = recipe.MacrosPerServing();

        Assert.That(perServing, Is.EqualTo(new MacroBreakdown(165, 31, 0, 3.6m, 0)));
    }

    [TestCase(0)]
    [TestCase(-1)]
    public void Constructor_Throws_WhenServingsIsNotPositive(int servings)
    {
        Assert.Throws<ArgumentOutOfRangeException>(() =>
            new Recipe("Bad recipe", servings, createdByUserId: 1, createdAt: DateTimeOffset.UtcNow));
    }
}
