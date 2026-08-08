using MediatR;

namespace Food.Application.Logging.GetDayMealLogs;

public sealed record GetDayMealLogsQuery(long UserId, DateOnly LogDate) : IRequest<IReadOnlyList<MealLogDto>>;
