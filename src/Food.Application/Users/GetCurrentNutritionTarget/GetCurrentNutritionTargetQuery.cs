using Food.Domain.Common;
using MediatR;

namespace Food.Application.Users.GetCurrentNutritionTarget;

public sealed record GetCurrentNutritionTargetQuery(long UserId, DateOnly AsOf) : IRequest<MacroBreakdown?>;
