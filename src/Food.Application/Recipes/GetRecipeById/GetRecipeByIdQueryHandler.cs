using MediatR;

namespace Food.Application.Recipes.GetRecipeById;

public sealed class GetRecipeByIdQueryHandler : IRequestHandler<GetRecipeByIdQuery, RecipeDto?>
{
    private readonly IRecipeRepository _recipes;

    public GetRecipeByIdQueryHandler(IRecipeRepository recipes) => _recipes = recipes;

    public async Task<RecipeDto?> Handle(GetRecipeByIdQuery request, CancellationToken cancellationToken)
    {
        var recipe = await _recipes.GetByIdAsync(request.Id, cancellationToken);
        return recipe?.ToDto();
    }
}
