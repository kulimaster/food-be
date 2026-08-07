using MediatR;

namespace Food.Application.Ingredients.ListIngredients;

public sealed record ListIngredientsQuery(string? Search, string? Tag) : IRequest<IReadOnlyList<IngredientDto>>;
