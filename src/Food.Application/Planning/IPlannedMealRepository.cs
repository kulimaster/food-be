using Food.Domain.Planning;

namespace Food.Application.Planning;

public interface IPlannedMealRepository
{
    public Task AddAsync(PlannedMeal plannedMeal, CancellationToken cancellationToken);

    public Task<IReadOnlyList<PlannedMeal>> ListForDayAsync(long userId, DateOnly planDate, CancellationToken cancellationToken);

    public Task<IReadOnlyList<PlannedMeal>> ListForRangeAsync(
        long userId,
        DateOnly startDate,
        DateOnly endDate,
        CancellationToken cancellationToken);
}
