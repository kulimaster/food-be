using Food.Application.Ingredients;
using Food.Domain.Ingredients;

namespace Food.Infrastructure.Persistence.Repositories;

public sealed class IngredientRepository : IIngredientRepository
{
    private readonly FoodDbContext _dbContext;

    public IngredientRepository(FoodDbContext dbContext) => _dbContext = dbContext;

    public async Task AddAsync(Ingredient ingredient, CancellationToken cancellationToken) =>
        await _dbContext.Ingredients.AddAsync(ingredient, cancellationToken);
}
