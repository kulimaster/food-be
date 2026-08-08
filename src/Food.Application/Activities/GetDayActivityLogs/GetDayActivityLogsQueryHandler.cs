using MediatR;

namespace Food.Application.Activities.GetDayActivityLogs;

public sealed class GetDayActivityLogsQueryHandler : IRequestHandler<GetDayActivityLogsQuery, IReadOnlyList<ActivityLogDto>>
{
    private readonly IActivityLogRepository _activityLogs;

    public GetDayActivityLogsQueryHandler(IActivityLogRepository activityLogs) => _activityLogs = activityLogs;

    public async Task<IReadOnlyList<ActivityLogDto>> Handle(GetDayActivityLogsQuery request, CancellationToken cancellationToken)
    {
        var activityLogs = await _activityLogs.ListForDayAsync(request.UserId, request.LogDate, cancellationToken);
        return activityLogs.Select(a => a.ToDto()).ToList();
    }
}
