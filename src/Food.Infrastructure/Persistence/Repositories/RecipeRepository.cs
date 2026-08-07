using Food.Application.Recipes;
using Food.Domain.Recipes;
using Microsoft.EntityFrameworkCore;

namespace Food.Infrastructure.Persistence.Repositories;

public sealed class RecipeRepository : IRecipeRepository
{
    private readonly FoodDbContext _dbContext;

    public RecipeRepository(FoodDbContext dbContext) => _dbContext = dbContext;

    public async Task AddAsync(Recipe recipe, CancellationToken cancellationToken) =>
        await _dbContext.Recipes.AddAsync(recipe, cancellationToken);

    public Task<Recipe?> GetByIdAsync(long id, CancellationToken cancellationToken) =>
        _dbContext.Recipes
            .Include(r => r.Ingredients)
            .ThenInclude(ri => ri.Ingredient)
            .SingleOrDefaultAsync(r => r.Id == id, cancellationToken);

    public async Task<IReadOnlyList<Recipe>> ListAsync(string? search, CancellationToken cancellationToken)
    {
        var query = _dbContext.Recipes
            .Include(r => r.Ingredients)
            .ThenInclude(ri => ri.Ingredient)
            .AsQueryable();

        if (!string.IsNullOrWhiteSpace(search))
        {
            query = query.Where(r => EF.Functions.ILike(r.Name, $"%{search}%"));
        }

        return await query.OrderBy(r => r.Name).ToListAsync(cancellationToken);
    }
}
