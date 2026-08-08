using MediatR;

namespace Food.Application.Dashboard.GetDailyDashboard;

public sealed record GetDailyDashboardQuery(long UserId, DateOnly Date) : IRequest<DailyDashboardDto?>;
