using Food.Domain.Common;

namespace Food.Domain.Activities;

public sealed class ActivityLog : Entity
{
    public long UserId { get; private set; }
    public DateOnly LogDate { get; private set; }
    public string ActivityType { get; private set; }
    public int DurationMinutes { get; private set; }
    public int CaloriesBurned { get; private set; }
    public DateTimeOffset LoggedAt { get; private set; }

    private ActivityLog()
    {
        ActivityType = null!;
    }

    public ActivityLog(long userId, DateOnly logDate, string activityType, int durationMinutes, int caloriesBurned, DateTimeOffset loggedAt)
    {
        UserId = userId;
        LogDate = logDate;
        ActivityType = Guard.NotEmpty(activityType, nameof(activityType));
        DurationMinutes = Guard.Positive(durationMinutes, nameof(durationMinutes));
        CaloriesBurned = Guard.NonNegative(caloriesBurned, nameof(caloriesBurned));
        LoggedAt = loggedAt;
    }
}
