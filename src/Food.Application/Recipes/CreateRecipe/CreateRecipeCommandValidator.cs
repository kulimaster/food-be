using FluentValidation;

namespace Food.Application.Recipes.CreateRecipe;

public sealed class CreateRecipeCommandValidator : AbstractValidator<CreateRecipeCommand>
{
    public CreateRecipeCommandValidator()
    {
        RuleFor(x => x.Name).NotEmpty().MaximumLength(200);
        RuleFor(x => x.Servings).GreaterThan(0);
        RuleFor(x => x.CreatedByUserId).GreaterThan(0);
        RuleFor(x => x.Ingredients).NotEmpty();

        RuleForEach(x => x.Ingredients).ChildRules(line =>
        {
            line.RuleFor(l => l.IngredientId).GreaterThan(0);
            line.RuleFor(l => l.QuantityGrams).GreaterThan(0);
        });
    }
}
