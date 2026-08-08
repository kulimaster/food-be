namespace Food.Application.Activities;

public sealed record ActivityLogDto(
    long Id,
    long UserId,
    DateOnly LogDate,
    string ActivityType,
    int DurationMinutes,
    int CaloriesBurned,
    DateTimeOffset LoggedAt);
