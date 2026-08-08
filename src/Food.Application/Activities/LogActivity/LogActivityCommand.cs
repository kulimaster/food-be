using MediatR;

namespace Food.Application.Activities.LogActivity;

public sealed record LogActivityCommand(
    long UserId,
    DateOnly LogDate,
    string ActivityType,
    int DurationMinutes,
    int CaloriesBurned) : IRequest<long>;
