using FluentValidation;

namespace Food.Application.Activities.LogActivity;

public sealed class LogActivityCommandValidator : AbstractValidator<LogActivityCommand>
{
    public LogActivityCommandValidator()
    {
        RuleFor(x => x.UserId).GreaterThan(0);
        RuleFor(x => x.LogDate).NotEqual(default(DateOnly));
        RuleFor(x => x.ActivityType).NotEmpty().MaximumLength(100);
        RuleFor(x => x.DurationMinutes).GreaterThan(0);
        RuleFor(x => x.CaloriesBurned).GreaterThanOrEqualTo(0);
    }
}
