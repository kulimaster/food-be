using Food.Application.Favorites;
using Food.Domain.Logging;
using Microsoft.EntityFrameworkCore;

namespace Food.Infrastructure.Persistence.Repositories;

public sealed class FavoriteMealRepository : IFavoriteMealRepository
{
    private readonly FoodDbContext _dbContext;

    public FavoriteMealRepository(FoodDbContext dbContext) => _dbContext = dbContext;

    public async Task AddAsync(FavoriteMeal favoriteMeal, CancellationToken cancellationToken) =>
        await _dbContext.FavoriteMeals.AddAsync(favoriteMeal, cancellationToken);

    public Task<FavoriteMeal?> GetByIdAsync(long id, CancellationToken cancellationToken) =>
        _dbContext.FavoriteMeals
            .Include(f => f.Item.Recipe!)
            .ThenInclude(r => r.Ingredients)
            .ThenInclude(ri => ri.Ingredient)
            .Include(f => f.Item.Ingredient)
            .SingleOrDefaultAsync(f => f.Id == id, cancellationToken);

    public async Task<IReadOnlyList<FavoriteMeal>> ListForUserAsync(long userId, CancellationToken cancellationToken) =>
        await _dbContext.FavoriteMeals
            .Include(f => f.Item.Recipe!)
            .ThenInclude(r => r.Ingredients)
            .ThenInclude(ri => ri.Ingredient)
            .Include(f => f.Item.Ingredient)
            .Where(f => f.UserId == userId)
            .OrderBy(f => f.DisplayName)
            .ToListAsync(cancellationToken);

    public Task RemoveAsync(FavoriteMeal favoriteMeal, CancellationToken cancellationToken)
    {
        _dbContext.FavoriteMeals.Remove(favoriteMeal);
        return Task.CompletedTask;
    }
}
