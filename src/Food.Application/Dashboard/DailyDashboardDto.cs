using Food.Application.Logging;
using Food.Domain.Common;

namespace Food.Application.Dashboard;

// Remaining macros are plain decimals, not a MacroBreakdown: MacroBreakdown enforces
// non-negative components, but "remaining" must be able to go negative to show the
// user they're over target.
public sealed record MacroRemainder(decimal Calories, decimal ProteinG, decimal CarbsG, decimal FatG, decimal FiberG);

public sealed record DailyDashboardDto(
    DateOnly Date,
    MacroBreakdown Target,
    MacroBreakdown Consumed,
    MacroRemainder Remaining,
    IReadOnlyCollection<MealLogDto> Meals);
