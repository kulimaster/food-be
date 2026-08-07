using Food.Application.Abstractions;
using Food.Domain.Common;
using Food.Domain.Nutrition;
using MediatR;

namespace Food.Application.Users.SetManualNutritionTarget;

public sealed class SetManualNutritionTargetCommandHandler
    : IRequestHandler<SetManualNutritionTargetCommand, MacroBreakdown>
{
    private readonly INutritionTargetRepository _targets;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IClock _clock;

    public SetManualNutritionTargetCommandHandler(
        INutritionTargetRepository targets,
        IUnitOfWork unitOfWork,
        IClock clock)
    {
        _targets = targets;
        _unitOfWork = unitOfWork;
        _clock = clock;
    }

    public async Task<MacroBreakdown> Handle(SetManualNutritionTargetCommand request, CancellationToken cancellationToken)
    {
        var now = _clock.UtcNow;
        var today = DateOnly.FromDateTime(now.UtcDateTime);

        var macros = new MacroBreakdown(request.Calories, request.ProteinG, request.CarbsG, request.FatG, request.FiberG);
        var target = new NutritionTarget(request.UserId, today, macros, isManualOverride: true, now);

        await _targets.AddAsync(target, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return macros;
    }
}
