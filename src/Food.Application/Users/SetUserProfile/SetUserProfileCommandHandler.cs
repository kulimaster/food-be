using Food.Application.Abstractions;
using Food.Domain.Common;
using Food.Domain.Nutrition;
using Food.Domain.Users;
using MediatR;

namespace Food.Application.Users.SetUserProfile;

public sealed class SetUserProfileCommandHandler : IRequestHandler<SetUserProfileCommand, MacroBreakdown>
{
    private readonly IUserProfileRepository _profiles;
    private readonly INutritionTargetRepository _targets;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IClock _clock;

    public SetUserProfileCommandHandler(
        IUserProfileRepository profiles,
        INutritionTargetRepository targets,
        IUnitOfWork unitOfWork,
        IClock clock)
    {
        _profiles = profiles;
        _targets = targets;
        _unitOfWork = unitOfWork;
        _clock = clock;
    }

    public async Task<MacroBreakdown> Handle(SetUserProfileCommand request, CancellationToken cancellationToken)
    {
        var now = _clock.UtcNow;
        var today = DateOnly.FromDateTime(now.UtcDateTime);

        var profile = await _profiles.GetByUserIdAsync(request.UserId, cancellationToken);

        if (profile is null)
        {
            profile = new UserProfile(
                request.UserId,
                request.WeightKg,
                request.HeightCm,
                request.DateOfBirth,
                request.Sex,
                request.ActivityLevel,
                request.Goal,
                now);

            await _profiles.AddAsync(profile, cancellationToken);
        }
        else
        {
            profile.Update(request.WeightKg, request.HeightCm, request.ActivityLevel, request.Goal, now);
        }

        var macros = MacroTargetCalculator.Calculate(profile, today);
        var target = new NutritionTarget(request.UserId, today, macros, isManualOverride: false, now);
        await _targets.AddAsync(target, cancellationToken);

        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return macros;
    }
}
