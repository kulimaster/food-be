using Food.Domain.Common;
using Food.Domain.Ingredients;

namespace Food.Domain.Tests.Ingredients;

[TestFixture]
public class IngredientTests
{
    private static Ingredient CreateBanana() => new(
        name: "Banana",
        macrosPer100g: new MacroBreakdown(89, 1.1m, 23m, 0.3m, 2.6m),
        createdByUserId: 1,
        createdAt: DateTimeOffset.UtcNow);

    [Test]
    public void MacrosForQuantity_ScalesPer100gValuesByQuantity()
    {
        var banana = CreateBanana();

        var macros = banana.MacrosForQuantity(new Quantity(65));

        Assert.That(macros, Is.EqualTo(new MacroBreakdown(57.85m, 0.715m, 14.95m, 0.195m, 1.69m)));
    }

    [Test]
    public void AddTag_IgnoresCaseInsensitiveDuplicates()
    {
        var banana = CreateBanana();

        banana.AddTag("Fruit");
        banana.AddTag("fruit");

        Assert.That(banana.Tags, Has.Count.EqualTo(1));
    }

    [Test]
    public void RemoveTag_RemovesRegardlessOfCase()
    {
        var banana = CreateBanana();
        banana.AddTag("Fruit");

        banana.RemoveTag("FRUIT");

        Assert.That(banana.Tags, Is.Empty);
    }
}
