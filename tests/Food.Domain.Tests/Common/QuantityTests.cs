using Food.Domain.Common;

namespace Food.Domain.Tests.Common;

[TestFixture]
public class QuantityTests
{
    [Test]
    public void Constructor_AcceptsPositiveGrams()
    {
        var quantity = new Quantity(65);

        Assert.That(quantity.Grams, Is.EqualTo(65));
    }

    [TestCase(0)]
    [TestCase(-10)]
    public void Constructor_Throws_WhenGramsIsNotPositive(decimal grams)
    {
        Assert.Throws<ArgumentOutOfRangeException>(() => new Quantity(grams));
    }
}
