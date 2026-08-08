using Food.Application.Logging;
using Food.Domain.Logging;
using Microsoft.EntityFrameworkCore;

namespace Food.Infrastructure.Persistence.Repositories;

public sealed class MealLogRepository : IMealLogRepository
{
    private readonly FoodDbContext _dbContext;

    public MealLogRepository(FoodDbContext dbContext) => _dbContext = dbContext;

    public async Task AddAsync(MealLog mealLog, CancellationToken cancellationToken) =>
        await _dbContext.MealLogs.AddAsync(mealLog, cancellationToken);

    public async Task<IReadOnlyList<MealLog>> ListForDayAsync(long userId, DateOnly logDate, CancellationToken cancellationToken) =>
        await _dbContext.MealLogs
            .Include(m => m.Item.Recipe!)
            .ThenInclude(r => r.Ingredients)
            .ThenInclude(ri => ri.Ingredient)
            .Include(m => m.Item.Ingredient)
            .Where(m => m.UserId == userId && m.LogDate == logDate)
            .OrderBy(m => m.LoggedAt)
            .ToListAsync(cancellationToken);
}
