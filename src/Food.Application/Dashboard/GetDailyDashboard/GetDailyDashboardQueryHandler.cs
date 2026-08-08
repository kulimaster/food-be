using Food.Application.Logging;
using Food.Application.Users;
using Food.Domain.Common;
using MediatR;

namespace Food.Application.Dashboard.GetDailyDashboard;

public sealed class GetDailyDashboardQueryHandler : IRequestHandler<GetDailyDashboardQuery, DailyDashboardDto?>
{
    private readonly INutritionTargetRepository _targets;
    private readonly IMealLogRepository _mealLogs;

    public GetDailyDashboardQueryHandler(INutritionTargetRepository targets, IMealLogRepository mealLogs)
    {
        _targets = targets;
        _mealLogs = mealLogs;
    }

    public async Task<DailyDashboardDto?> Handle(GetDailyDashboardQuery request, CancellationToken cancellationToken)
    {
        var target = await _targets.GetCurrentAsync(request.UserId, request.Date, cancellationToken);
        if (target is null)
        {
            return null;
        }

        var mealLogs = await _mealLogs.ListForDayAsync(request.UserId, request.Date, cancellationToken);
        var consumed = mealLogs.Aggregate(MacroBreakdown.Zero, (sum, log) => sum + log.Macros());

        var remaining = new MacroRemainder(
            target.Macros.Calories - consumed.Calories,
            target.Macros.ProteinG - consumed.ProteinG,
            target.Macros.CarbsG - consumed.CarbsG,
            target.Macros.FatG - consumed.FatG,
            target.Macros.FiberG - consumed.FiberG);

        return new DailyDashboardDto(
            request.Date,
            target.Macros,
            consumed,
            remaining,
            mealLogs.Select(m => m.ToDto()).ToList());
    }
}
