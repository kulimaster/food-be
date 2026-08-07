using MediatR;

namespace Food.Application.Recipes.GetRecipeById;

public sealed record GetRecipeByIdQuery(long Id) : IRequest<RecipeDto?>;
