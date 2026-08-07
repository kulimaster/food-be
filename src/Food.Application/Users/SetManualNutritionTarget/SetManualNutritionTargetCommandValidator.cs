using FluentValidation;

namespace Food.Application.Users.SetManualNutritionTarget;

public sealed class SetManualNutritionTargetCommandValidator : AbstractValidator<SetManualNutritionTargetCommand>
{
    public SetManualNutritionTargetCommandValidator()
    {
        RuleFor(x => x.UserId).GreaterThan(0);
        RuleFor(x => x.Calories).GreaterThanOrEqualTo(0);
        RuleFor(x => x.ProteinG).GreaterThanOrEqualTo(0);
        RuleFor(x => x.CarbsG).GreaterThanOrEqualTo(0);
        RuleFor(x => x.FatG).GreaterThanOrEqualTo(0);
        RuleFor(x => x.FiberG).GreaterThanOrEqualTo(0);
    }
}
