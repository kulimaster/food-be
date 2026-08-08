using Food.Application.Planning;
using Food.Domain.Planning;
using Microsoft.EntityFrameworkCore;

namespace Food.Infrastructure.Persistence.Repositories;

public sealed class PlannedMealRepository : IPlannedMealRepository
{
    private readonly FoodDbContext _dbContext;

    public PlannedMealRepository(FoodDbContext dbContext) => _dbContext = dbContext;

    public async Task AddAsync(PlannedMeal plannedMeal, CancellationToken cancellationToken) =>
        await _dbContext.PlannedMeals.AddAsync(plannedMeal, cancellationToken);

    public async Task<IReadOnlyList<PlannedMeal>> ListForDayAsync(long userId, DateOnly planDate, CancellationToken cancellationToken) =>
        await Query()
            .Where(p => p.UserId == userId && p.PlanDate == planDate)
            .OrderBy(p => p.CreatedAt)
            .ToListAsync(cancellationToken);

    public async Task<IReadOnlyList<PlannedMeal>> ListForRangeAsync(
        long userId,
        DateOnly startDate,
        DateOnly endDate,
        CancellationToken cancellationToken) =>
        await Query()
            .Where(p => p.UserId == userId && p.PlanDate >= startDate && p.PlanDate <= endDate)
            .OrderBy(p => p.PlanDate)
            .ToListAsync(cancellationToken);

    private IQueryable<PlannedMeal> Query() =>
        _dbContext.PlannedMeals
            .Include(p => p.Item.Recipe!)
            .ThenInclude(r => r.Ingredients)
            .ThenInclude(ri => ri.Ingredient)
            .Include(p => p.Item.Ingredient);
}
