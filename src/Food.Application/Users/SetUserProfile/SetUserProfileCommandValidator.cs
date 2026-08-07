using FluentValidation;

namespace Food.Application.Users.SetUserProfile;

public sealed class SetUserProfileCommandValidator : AbstractValidator<SetUserProfileCommand>
{
    public SetUserProfileCommandValidator()
    {
        RuleFor(x => x.UserId).GreaterThan(0);
        RuleFor(x => x.WeightKg).GreaterThan(0);
        RuleFor(x => x.HeightCm).GreaterThan(0);
        RuleFor(x => x.DateOfBirth).LessThan(_ => DateOnly.FromDateTime(DateTime.UtcNow));
        RuleFor(x => x.Sex).IsInEnum();
        RuleFor(x => x.ActivityLevel).IsInEnum();
        RuleFor(x => x.Goal).IsInEnum();
    }
}
