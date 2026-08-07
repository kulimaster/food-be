using Food.Application.Users;
using Food.Domain.Nutrition;
using Microsoft.EntityFrameworkCore;

namespace Food.Infrastructure.Persistence.Repositories;

public sealed class NutritionTargetRepository : INutritionTargetRepository
{
    private readonly FoodDbContext _dbContext;

    public NutritionTargetRepository(FoodDbContext dbContext) => _dbContext = dbContext;

    public async Task AddAsync(NutritionTarget target, CancellationToken cancellationToken) =>
        await _dbContext.NutritionTargets.AddAsync(target, cancellationToken);

    public Task<NutritionTarget?> GetCurrentAsync(long userId, DateOnly asOf, CancellationToken cancellationToken) =>
        _dbContext.NutritionTargets
            .Where(t => t.UserId == userId && t.EffectiveFrom <= asOf)
            .OrderByDescending(t => t.EffectiveFrom)
            .FirstOrDefaultAsync(cancellationToken);
}
