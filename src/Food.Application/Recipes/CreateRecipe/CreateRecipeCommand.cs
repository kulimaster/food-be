using MediatR;

namespace Food.Application.Recipes.CreateRecipe;

public sealed record CreateRecipeIngredientLine(long IngredientId, decimal QuantityGrams);

public sealed record CreateRecipeCommand(
    string Name,
    int Servings,
    long CreatedByUserId,
    IReadOnlyCollection<CreateRecipeIngredientLine> Ingredients) : IRequest<long>;
