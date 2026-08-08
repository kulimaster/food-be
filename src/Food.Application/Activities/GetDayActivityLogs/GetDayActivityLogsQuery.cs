using MediatR;

namespace Food.Application.Activities.GetDayActivityLogs;

public sealed record GetDayActivityLogsQuery(long UserId, DateOnly LogDate) : IRequest<IReadOnlyList<ActivityLogDto>>;
