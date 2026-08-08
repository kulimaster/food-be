using Food.Domain.Activities;

namespace Food.Application.Activities;

public static class ActivityLogMappingExtensions
{
    public static ActivityLogDto ToDto(this ActivityLog activityLog) => new(
        activityLog.Id,
        activityLog.UserId,
        activityLog.LogDate,
        activityLog.ActivityType,
        activityLog.DurationMinutes,
        activityLog.CaloriesBurned,
        activityLog.LoggedAt);
}
