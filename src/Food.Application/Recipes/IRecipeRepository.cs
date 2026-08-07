using Food.Domain.Recipes;

namespace Food.Application.Recipes;

public interface IRecipeRepository
{
    public Task AddAsync(Recipe recipe, CancellationToken cancellationToken);

    public Task<Recipe?> GetByIdAsync(long id, CancellationToken cancellationToken);

    public Task<IReadOnlyList<Recipe>> ListAsync(string? search, CancellationToken cancellationToken);
}
