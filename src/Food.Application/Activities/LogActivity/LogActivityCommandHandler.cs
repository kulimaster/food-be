using Food.Application.Abstractions;
using Food.Domain.Activities;
using MediatR;

namespace Food.Application.Activities.LogActivity;

public sealed class LogActivityCommandHandler : IRequestHandler<LogActivityCommand, long>
{
    private readonly IActivityLogRepository _activityLogs;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IClock _clock;

    public LogActivityCommandHandler(IActivityLogRepository activityLogs, IUnitOfWork unitOfWork, IClock clock)
    {
        _activityLogs = activityLogs;
        _unitOfWork = unitOfWork;
        _clock = clock;
    }

    public async Task<long> Handle(LogActivityCommand request, CancellationToken cancellationToken)
    {
        var activityLog = new ActivityLog(
            request.UserId,
            request.LogDate,
            request.ActivityType,
            request.DurationMinutes,
            request.CaloriesBurned,
            _clock.UtcNow);

        await _activityLogs.AddAsync(activityLog, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return activityLog.Id;
    }
}
