using Food.Domain.Enums;
using MediatR;

namespace Food.Application.Planning.PlanMeal;

public sealed record PlanMealCommand(
    long UserId,
    DateOnly PlanDate,
    MealSlot MealSlot,
    long? RecipeId,
    decimal? ServingsCount,
    long? IngredientId,
    decimal? QuantityGrams) : IRequest<long>;
