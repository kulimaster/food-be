using Food.Domain.Activities;

namespace Food.Application.Activities;

public interface IActivityLogRepository
{
    public Task AddAsync(ActivityLog activityLog, CancellationToken cancellationToken);

    public Task<IReadOnlyList<ActivityLog>> ListForDayAsync(long userId, DateOnly logDate, CancellationToken cancellationToken);
}
