using Food.Application.Ingredients;
using Food.Domain.Ingredients;
using Microsoft.EntityFrameworkCore;

namespace Food.Infrastructure.Persistence.Repositories;

public sealed class IngredientRepository : IIngredientRepository
{
    private readonly FoodDbContext _dbContext;

    public IngredientRepository(FoodDbContext dbContext) => _dbContext = dbContext;

    public async Task AddAsync(Ingredient ingredient, CancellationToken cancellationToken) =>
        await _dbContext.Ingredients.AddAsync(ingredient, cancellationToken);

    public Task<Ingredient?> GetByIdAsync(long id, CancellationToken cancellationToken) =>
        _dbContext.Ingredients.SingleOrDefaultAsync(i => i.Id == id, cancellationToken);

    public async Task<IReadOnlyList<Ingredient>> ListAsync(string? search, string? tag, CancellationToken cancellationToken)
    {
        var query = _dbContext.Ingredients.AsQueryable();

        if (!string.IsNullOrWhiteSpace(search))
        {
            query = query.Where(i => EF.Functions.ILike(i.Name, $"%{search}%"));
        }

        if (!string.IsNullOrWhiteSpace(tag))
        {
            query = query.Where(i => i.Tags.Any(t => t.Name == tag));
        }

        return await query.OrderBy(i => i.Name).ToListAsync(cancellationToken);
    }
}
