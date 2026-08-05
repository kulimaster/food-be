using Food.Domain.Ingredients;

namespace Food.Application.Ingredients;

public interface IIngredientRepository
{
    public Task AddAsync(Ingredient ingredient, CancellationToken cancellationToken);
}
