using Food.Domain.Common;

namespace Food.Domain.Tests.Common;

[TestFixture]
public class MacroBreakdownTests
{
    [Test]
    public void Zero_HasAllComponentsAtZero()
    {
        var zero = MacroBreakdown.Zero;

        Assert.That(zero, Is.EqualTo(new MacroBreakdown(0, 0, 0, 0, 0)));
    }

    [TestCase(-1, 0, 0, 0, 0)]
    [TestCase(0, -1, 0, 0, 0)]
    [TestCase(0, 0, -1, 0, 0)]
    [TestCase(0, 0, 0, -1, 0)]
    [TestCase(0, 0, 0, 0, -1)]
    public void Constructor_Throws_WhenAnyComponentIsNegative(
        decimal calories, decimal protein, decimal carbs, decimal fat, decimal fiber)
    {
        Assert.Throws<ArgumentOutOfRangeException>(() =>
            new MacroBreakdown(calories, protein, carbs, fat, fiber));
    }

    [Test]
    public void Scale_MultipliesEveryComponentByFactor()
    {
        var macros = new MacroBreakdown(100, 10, 20, 5, 2);

        var scaled = macros.Scale(0.5m);

        Assert.That(scaled, Is.EqualTo(new MacroBreakdown(50, 5, 10, 2.5m, 1)));
    }

    [Test]
    public void Scale_Throws_WhenFactorIsNegative()
    {
        var macros = new MacroBreakdown(100, 10, 20, 5, 2);

        Assert.Throws<ArgumentOutOfRangeException>(() => macros.Scale(-1m));
    }

    [Test]
    public void Addition_SumsEachComponent()
    {
        var a = new MacroBreakdown(100, 10, 20, 5, 2);
        var b = new MacroBreakdown(50, 5, 10, 2, 1);

        var sum = a + b;

        Assert.That(sum, Is.EqualTo(new MacroBreakdown(150, 15, 30, 7, 3)));
    }
}
