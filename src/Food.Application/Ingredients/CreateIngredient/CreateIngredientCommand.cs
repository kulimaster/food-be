using MediatR;

namespace Food.Application.Ingredients.CreateIngredient;

public sealed record CreateIngredientCommand(
    string Name,
    decimal CaloriesPer100g,
    decimal ProteinPer100g,
    decimal CarbsPer100g,
    decimal FatPer100g,
    decimal FiberPer100g,
    long CreatedByUserId,
    IReadOnlyCollection<string> Tags) : IRequest<long>;
