using Food.Application.Dashboard;
using Food.Application.Users;
using Food.Domain.Common;
using MediatR;

namespace Food.Application.Planning.GetPlannedDashboard;

public sealed class GetPlannedDashboardQueryHandler : IRequestHandler<GetPlannedDashboardQuery, PlannedDashboardDto?>
{
    private readonly INutritionTargetRepository _targets;
    private readonly IPlannedMealRepository _plannedMeals;

    public GetPlannedDashboardQueryHandler(INutritionTargetRepository targets, IPlannedMealRepository plannedMeals)
    {
        _targets = targets;
        _plannedMeals = plannedMeals;
    }

    public async Task<PlannedDashboardDto?> Handle(GetPlannedDashboardQuery request, CancellationToken cancellationToken)
    {
        var target = await _targets.GetCurrentAsync(request.UserId, request.PlanDate, cancellationToken);
        if (target is null)
        {
            return null;
        }

        var plannedMeals = await _plannedMeals.ListForDayAsync(request.UserId, request.PlanDate, cancellationToken);
        var planned = plannedMeals.Aggregate(MacroBreakdown.Zero, (sum, m) => sum + m.Macros());

        var remaining = new MacroRemainder(
            target.Macros.Calories - planned.Calories,
            target.Macros.ProteinG - planned.ProteinG,
            target.Macros.CarbsG - planned.CarbsG,
            target.Macros.FatG - planned.FatG,
            target.Macros.FiberG - planned.FiberG);

        return new PlannedDashboardDto(
            request.PlanDate,
            target.Macros,
            planned,
            remaining,
            plannedMeals.Select(m => m.ToDto()).ToList());
    }
}
