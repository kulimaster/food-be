using Food.Domain.Ingredients;

namespace Food.Application.Ingredients;

public interface IIngredientRepository
{
    public Task AddAsync(Ingredient ingredient, CancellationToken cancellationToken);

    public Task<Ingredient?> GetByIdAsync(long id, CancellationToken cancellationToken);

    public Task<IReadOnlyList<Ingredient>> ListAsync(string? search, string? tag, CancellationToken cancellationToken);
}
