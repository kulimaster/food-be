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

    public Task<Ingredient?> GetByIdAsync(long id, CancellationToken cancellationToken) =>
        Task.FromResult(Added.SingleOrDefault(i => i.Id == id));

    public Task<IReadOnlyList<Ingredient>> ListAsync(string? search, string? tag, CancellationToken cancellationToken) =>
        Task.FromResult<IReadOnlyList<Ingredient>>(Added.AsReadOnly());
}
