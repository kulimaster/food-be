using MediatR;

namespace Food.Application.Ingredients.ListIngredients;

public sealed class ListIngredientsQueryHandler : IRequestHandler<ListIngredientsQuery, IReadOnlyList<IngredientDto>>
{
    private readonly IIngredientRepository _ingredients;

    public ListIngredientsQueryHandler(IIngredientRepository ingredients) => _ingredients = ingredients;

    public async Task<IReadOnlyList<IngredientDto>> Handle(ListIngredientsQuery request, CancellationToken cancellationToken)
    {
        var ingredients = await _ingredients.ListAsync(request.Search, request.Tag, cancellationToken);
        return ingredients.Select(i => i.ToDto()).ToList();
    }
}
