using MediatR;

namespace Food.Application.Logging.GetDayMealLogs;

public sealed class GetDayMealLogsQueryHandler : IRequestHandler<GetDayMealLogsQuery, IReadOnlyList<MealLogDto>>
{
    private readonly IMealLogRepository _mealLogs;

    public GetDayMealLogsQueryHandler(IMealLogRepository mealLogs) => _mealLogs = mealLogs;

    public async Task<IReadOnlyList<MealLogDto>> Handle(GetDayMealLogsQuery request, CancellationToken cancellationToken)
    {
        var mealLogs = await _mealLogs.ListForDayAsync(request.UserId, request.LogDate, cancellationToken);
        return mealLogs.Select(m => m.ToDto()).ToList();
    }
}
