using MediatR;

namespace Food.Application.Recipes.ListRecipes;

public sealed record ListRecipesQuery(string? Search) : IRequest<IReadOnlyList<RecipeDto>>;
