using Food.Application.Ingredients.CreateIngredient;
using FluentValidation.TestHelper;

namespace Food.Application.Tests.Ingredients.CreateIngredient;

[TestFixture]
public class CreateIngredientCommandValidatorTests
{
    private CreateIngredientCommandValidator _validator = null!;

    [SetUp]
    public void SetUp() => _validator = new CreateIngredientCommandValidator();

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
    public void Validate_PassesForAValidCommand()
    {
        var result = _validator.TestValidate(ValidCommand());

        result.ShouldNotHaveAnyValidationErrors();
    }

    [Test]
    public void Validate_Fails_WhenNameIsEmpty()
    {
        var command = ValidCommand() with { Name = "" };

        var result = _validator.TestValidate(command);

        result.ShouldHaveValidationErrorFor(x => x.Name);
    }

    [TestCase(-1, 0, 0, 0, 0)]
    [TestCase(0, -1, 0, 0, 0)]
    [TestCase(0, 0, -1, 0, 0)]
    [TestCase(0, 0, 0, -1, 0)]
    [TestCase(0, 0, 0, 0, -1)]
    public void Validate_Fails_WhenAnyMacroIsNegative(
        decimal calories, decimal protein, decimal carbs, decimal fat, decimal fiber)
    {
        var command = ValidCommand() with
        {
            CaloriesPer100g = calories,
            ProteinPer100g = protein,
            CarbsPer100g = carbs,
            FatPer100g = fat,
            FiberPer100g = fiber,
        };

        var result = _validator.TestValidate(command);

        Assert.That(result.IsValid, Is.False);
    }

    [Test]
    public void Validate_Fails_WhenCreatedByUserIdIsNotPositive()
    {
        var command = ValidCommand() with { CreatedByUserId = 0 };

        var result = _validator.TestValidate(command);

        result.ShouldHaveValidationErrorFor(x => x.CreatedByUserId);
    }

    [Test]
    public void Validate_Fails_WhenAnyTagIsEmpty()
    {
        var command = ValidCommand(new[] { "Fruit", "" });

        var result = _validator.TestValidate(command);

        result.ShouldHaveValidationErrorFor("Tags[1]");
    }
}
