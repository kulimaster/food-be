using Food.Domain.Common;
using MediatR;

namespace Food.Application.Users.GetCurrentNutritionTarget;

public sealed class GetCurrentNutritionTargetQueryHandler
    : IRequestHandler<GetCurrentNutritionTargetQuery, MacroBreakdown?>
{
    private readonly INutritionTargetRepository _targets;

    public GetCurrentNutritionTargetQueryHandler(INutritionTargetRepository targets) => _targets = targets;

    public async Task<MacroBreakdown?> Handle(GetCurrentNutritionTargetQuery request, CancellationToken cancellationToken)
    {
        var target = await _targets.GetCurrentAsync(request.UserId, request.AsOf, cancellationToken);
        return target?.Macros;
    }
}
