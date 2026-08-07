using FluentValidation;

namespace Food.Application.Users.GetCurrentNutritionTarget;

public sealed class GetCurrentNutritionTargetQueryValidator : AbstractValidator<GetCurrentNutritionTargetQuery>
{
    public GetCurrentNutritionTargetQueryValidator()
    {
        RuleFor(x => x.UserId).GreaterThan(0);
    }
}
