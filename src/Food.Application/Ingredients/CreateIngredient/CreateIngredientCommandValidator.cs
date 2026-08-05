using FluentValidation;

namespace Food.Application.Ingredients.CreateIngredient;

public sealed class CreateIngredientCommandValidator : AbstractValidator<CreateIngredientCommand>
{
    public CreateIngredientCommandValidator()
    {
        RuleFor(x => x.Name).NotEmpty().MaximumLength(200);
        RuleFor(x => x.CaloriesPer100g).GreaterThanOrEqualTo(0);
        RuleFor(x => x.ProteinPer100g).GreaterThanOrEqualTo(0);
        RuleFor(x => x.CarbsPer100g).GreaterThanOrEqualTo(0);
        RuleFor(x => x.FatPer100g).GreaterThanOrEqualTo(0);
        RuleFor(x => x.FiberPer100g).GreaterThanOrEqualTo(0);
        RuleFor(x => x.CreatedByUserId).GreaterThan(0);
        RuleForEach(x => x.Tags).NotEmpty();
    }
}
