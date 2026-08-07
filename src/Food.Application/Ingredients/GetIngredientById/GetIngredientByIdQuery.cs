using MediatR;

namespace Food.Application.Ingredients.GetIngredientById;

public sealed record GetIngredientByIdQuery(long Id) : IRequest<IngredientDto?>;
