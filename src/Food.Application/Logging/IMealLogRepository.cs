using Food.Domain.Logging;

namespace Food.Application.Logging;

public interface IMealLogRepository
{
    public Task AddAsync(MealLog mealLog, CancellationToken cancellationToken);

    public Task<IReadOnlyList<MealLog>> ListForDayAsync(long userId, DateOnly logDate, CancellationToken cancellationToken);
}
