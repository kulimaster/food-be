using FluentValidation;

namespace Food.Application.Logging.LogMeal;

public sealed class LogMealCommandValidator : AbstractValidator<LogMealCommand>
{
    public LogMealCommandValidator()
    {
        RuleFor(x => x.UserId).GreaterThan(0);
        RuleFor(x => x.LogDate).NotEqual(default(DateOnly));
        RuleFor(x => x.MealSlot).IsInEnum();

        RuleFor(x => x)
            .Must(x => (x.RecipeId is not null) ^ (x.IngredientId is not null))
            .WithMessage("Exactly one of RecipeId or IngredientId must be provided.");

        When(x => x.RecipeId is not null, () =>
        {
            RuleFor(x => x.RecipeId!.Value).GreaterThan(0);
            RuleFor(x => x.ServingsCount).NotNull().GreaterThan(0);
        });

        When(x => x.IngredientId is not null, () =>
        {
            RuleFor(x => x.IngredientId!.Value).GreaterThan(0);
            RuleFor(x => x.QuantityGrams).NotNull().GreaterThan(0);
        });
    }
}
