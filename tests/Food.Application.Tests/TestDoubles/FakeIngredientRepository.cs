using Food.Application.Ingredients;
using Food.Domain.Ingredients;

namespace Food.Application.Tests.TestDoubles;

public sealed class FakeIngredientRepository : IIngredientRepository
{
    public List<Ingredient> Added { get; } = new();

    public Task AddAsync(Ingredient ingredient, CancellationToken cancellationToken)
    {
        Added.Add(ingredient);
        return Task.CompletedTask;
    }
}
