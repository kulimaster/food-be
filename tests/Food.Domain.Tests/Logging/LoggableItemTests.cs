using Food.Domain.Common;
using Food.Domain.Ingredients;
using Food.Domain.Logging;
using Food.Domain.Recipes;

namespace Food.Domain.Tests.Logging;

[TestFixture]
public class LoggableItemTests
{
    private static Ingredient CreateBanana() => new(
        "Banana", new MacroBreakdown(89, 1.1m, 23m, 0.3m, 2.6m), createdByUserId: 1, createdAt: DateTimeOffset.UtcNow);

    private static Recipe CreateRecipeWithBanana(Ingredient banana)
    {
        var recipe = new Recipe("Banana smoothie", servings: 2, createdByUserId: 1, createdAt: DateTimeOffset.UtcNow);
        recipe.AddIngredient(banana, new Quantity(200)); // total per recipe: x2 of per-100g
        return recipe;
    }

    [Test]
    public void FromIngredient_ComputesMacrosFromQuantity()
    {
        var banana = CreateBanana();

        var item = LoggableItem.FromIngredient(banana, new Quantity(65));

        Assert.That(item.Macros(), Is.EqualTo(banana.MacrosForQuantity(new Quantity(65))));
    }

    [Test]
    public void FromRecipe_ComputesMacrosFromServingsCount()
    {
        var banana = CreateBanana();
        var recipe = CreateRecipeWithBanana(banana);

        var item = LoggableItem.FromRecipe(recipe, servingsCount: 1.5m);

        Assert.That(item.Macros(), Is.EqualTo(recipe.MacrosPerServing().Scale(1.5m)));
    }

    [TestCase(0)]
    [TestCase(-1)]
    public void FromRecipe_Throws_WhenServingsCountIsNotPositive(decimal servingsCount)
    {
        var recipe = CreateRecipeWithBanana(CreateBanana());

        Assert.Throws<ArgumentOutOfRangeException>(() => LoggableItem.FromRecipe(recipe, servingsCount));
    }

    [Test]
    public void FromIngredient_LeavesRecipeSideNull()
    {
        var item = LoggableItem.FromIngredient(CreateBanana(), new Quantity(65));

        Assert.That(item.Recipe, Is.Null);
        Assert.That(item.ServingsCount, Is.Null);
    }

    [Test]
    public void FromRecipe_LeavesIngredientSideNull()
    {
        var item = LoggableItem.FromRecipe(CreateRecipeWithBanana(CreateBanana()), servingsCount: 1);

        Assert.That(item.Ingredient, Is.Null);
        Assert.That(item.Quantity, Is.Null);
    }
}
