using Food.Domain.Enums;
using MediatR;

namespace Food.Application.Logging.LogMeal;

public sealed record LogMealCommand(
    long UserId,
    DateOnly LogDate,
    MealSlot MealSlot,
    long? RecipeId,
    decimal? ServingsCount,
    long? IngredientId,
    decimal? QuantityGrams) : IRequest<long>;
