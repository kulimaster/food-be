using MediatR;

namespace Food.Application.Ingredients.GetIngredientById;

public sealed class GetIngredientByIdQueryHandler : IRequestHandler<GetIngredientByIdQuery, IngredientDto?>
{
    private readonly IIngredientRepository _ingredients;

    public GetIngredientByIdQueryHandler(IIngredientRepository ingredients) => _ingredients = ingredients;

    public async Task<IngredientDto?> Handle(GetIngredientByIdQuery request, CancellationToken cancellationToken)
    {
        var ingredient = await _ingredients.GetByIdAsync(request.Id, cancellationToken);
        return ingredient?.ToDto();
    }
}
