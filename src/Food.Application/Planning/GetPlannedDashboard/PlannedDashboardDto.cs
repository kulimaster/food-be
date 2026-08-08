using Food.Application.Dashboard;
using Food.Domain.Common;

namespace Food.Application.Planning.GetPlannedDashboard;

// No activity-calorie adjustment here (unlike DailyDashboardDto): the planner looks at
// future days that have no logged activity yet, so it checks planned macros against
// the plain NutritionTarget rather than an activity-adjusted one.
public sealed record PlannedDashboardDto(
    DateOnly Date,
    MacroBreakdown Target,
    MacroBreakdown Planned,
    MacroRemainder Remaining,
    IReadOnlyCollection<PlannedMealDto> Meals);
