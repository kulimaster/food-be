using MediatR;

namespace Food.Application.Recipes.ListRecipes;

public sealed class ListRecipesQueryHandler : IRequestHandler<ListRecipesQuery, IReadOnlyList<RecipeDto>>
{
    private readonly IRecipeRepository _recipes;

    public ListRecipesQueryHandler(IRecipeRepository recipes) => _recipes = recipes;

    public async Task<IReadOnlyList<RecipeDto>> Handle(ListRecipesQuery request, CancellationToken cancellationToken)
    {
        var recipes = await _recipes.ListAsync(request.Search, cancellationToken);
        return recipes.Select(r => r.ToDto()).ToList();
    }
}
