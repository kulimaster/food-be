using MediatR;

namespace Food.Application.Planning.GetPlannedDashboard;

public sealed record GetPlannedDashboardQuery(long UserId, DateOnly PlanDate) : IRequest<PlannedDashboardDto?>;
