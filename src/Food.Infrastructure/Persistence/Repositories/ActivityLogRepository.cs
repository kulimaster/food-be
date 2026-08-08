using Food.Application.Activities;
using Food.Domain.Activities;
using Microsoft.EntityFrameworkCore;

namespace Food.Infrastructure.Persistence.Repositories;

public sealed class ActivityLogRepository : IActivityLogRepository
{
    private readonly FoodDbContext _dbContext;

    public ActivityLogRepository(FoodDbContext dbContext) => _dbContext = dbContext;

    public async Task AddAsync(ActivityLog activityLog, CancellationToken cancellationToken) =>
        await _dbContext.ActivityLogs.AddAsync(activityLog, cancellationToken);

    public async Task<IReadOnlyList<ActivityLog>> ListForDayAsync(long userId, DateOnly logDate, CancellationToken cancellationToken) =>
        await _dbContext.ActivityLogs
            .Where(a => a.UserId == userId && a.LogDate == logDate)
            .OrderBy(a => a.LoggedAt)
            .ToListAsync(cancellationToken);
}
