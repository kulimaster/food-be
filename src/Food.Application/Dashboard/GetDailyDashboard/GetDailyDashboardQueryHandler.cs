using Food.Application.Activities;
using Food.Application.Logging;
using Food.Application.Users;
using Food.Domain.Common;
using MediatR;

namespace Food.Application.Dashboard.GetDailyDashboard;

public sealed class GetDailyDashboardQueryHandler : IRequestHandler<GetDailyDashboardQuery, DailyDashboardDto?>
{
    private readonly INutritionTargetRepository _targets;
    private readonly IMealLogRepository _mealLogs;
    private readonly IActivityLogRepository _activityLogs;

    public GetDailyDashboardQueryHandler(
        INutritionTargetRepository targets,
        IMealLogRepository mealLogs,
        IActivityLogRepository activityLogs)
    {
        _targets = targets;
        _mealLogs = mealLogs;
        _activityLogs = activityLogs;
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

        var activityLogs = await _activityLogs.ListForDayAsync(request.UserId, request.Date, cancellationToken);
        var caloriesBurned = activityLogs.Sum(a => a.CaloriesBurned);

        // "Eat back" burned calories: the displayed target includes the day's activity
        // adjustment, per business-description.md ("Dashboard"). Only calories are
        // adjusted - protein/carbs/fat/fiber targets are unaffected.
        var adjustedTarget = new MacroBreakdown(
            target.Macros.Calories + caloriesBurned,
            target.Macros.ProteinG,
            target.Macros.CarbsG,
            target.Macros.FatG,
            target.Macros.FiberG);

        var remaining = new MacroRemainder(
            adjustedTarget.Calories - consumed.Calories,
            adjustedTarget.ProteinG - consumed.ProteinG,
            adjustedTarget.CarbsG - consumed.CarbsG,
            adjustedTarget.FatG - consumed.FatG,
            adjustedTarget.FiberG - consumed.FiberG);

        return new DailyDashboardDto(
            request.Date,
            adjustedTarget,
            caloriesBurned,
            consumed,
            remaining,
            mealLogs.Select(m => m.ToDto()).ToList(),
            activityLogs.Select(a => a.ToDto()).ToList());
    }
}
