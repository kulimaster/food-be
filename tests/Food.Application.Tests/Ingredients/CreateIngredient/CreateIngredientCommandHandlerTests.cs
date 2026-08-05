using Food.Application.Ingredients.CreateIngredient;
using Food.Application.Tests.TestDoubles;

namespace Food.Application.Tests.Ingredients.CreateIngredient;

[TestFixture]
public class CreateIngredientCommandHandlerTests
{
    private static readonly DateTimeOffset Now = new(2026, 1, 1, 0, 0, 0, TimeSpan.Zero);

    private FakeIngredientRepository _repository = null!;
    private FakeUnitOfWork _unitOfWork = null!;
    private CreateIngredientCommandHandler _handler = null!;

    [SetUp]
    public void SetUp()
    {
        _repository = new FakeIngredientRepository();
        _unitOfWork = new FakeUnitOfWork();
        _handler = new CreateIngredientCommandHandler(_repository, _unitOfWork, new FakeClock(Now));
    }

    private static CreateIngredientCommand ValidCommand(IReadOnlyCollection<string>? tags = null) => new(
        Name: "Banana",
        CaloriesPer100g: 89,
        ProteinPer100g: 1.1m,
        CarbsPer100g: 23m,
        FatPer100g: 0.3m,
        FiberPer100g: 2.6m,
        CreatedByUserId: 1,
        Tags: tags ?? Array.Empty<string>());

    [Test]
    public async Task Handle_AddsIngredientWithCommandDataToRepository()
    {
        await _handler.Handle(ValidCommand(), CancellationToken.None);

        Assert.That(_repository.Added, Has.Count.EqualTo(1));
        var ingredient = _repository.Added[0];
        Assert.That(ingredient.Name, Is.EqualTo("Banana"));
        Assert.That(ingredient.MacrosPer100g.Calories, Is.EqualTo(89));
        Assert.That(ingredient.CreatedByUserId, Is.EqualTo(1));
        Assert.That(ingredient.CreatedAt, Is.EqualTo(Now));
    }

    [Test]
    public async Task Handle_AddsAllRequestedTags()
    {
        await _handler.Handle(ValidCommand(new[] { "Fruit", "Vegan" }), CancellationToken.None);

        var ingredient = _repository.Added[0];
        Assert.That(ingredient.Tags.Select(t => t.Name), Is.EquivalentTo(new[] { "Fruit", "Vegan" }));
    }

    [Test]
    public async Task Handle_SavesChangesExactlyOnce()
    {
        await _handler.Handle(ValidCommand(), CancellationToken.None);

        Assert.That(_unitOfWork.SaveChangesCallCount, Is.EqualTo(1));
    }

    [Test]
    public async Task Handle_ReturnsTheCreatedIngredientId()
    {
        var result = await _handler.Handle(ValidCommand(), CancellationToken.None);

        Assert.That(result, Is.EqualTo(_repository.Added[0].Id));
    }
}
