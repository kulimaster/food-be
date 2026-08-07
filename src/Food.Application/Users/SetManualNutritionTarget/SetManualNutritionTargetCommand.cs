using Food.Domain.Common;
using MediatR;

namespace Food.Application.Users.SetManualNutritionTarget;

public sealed record SetManualNutritionTargetCommand(
    long UserId,
    decimal Calories,
    decimal ProteinG,
    decimal CarbsG,
    decimal FatG,
    decimal FiberG) : IRequest<MacroBreakdown>;
