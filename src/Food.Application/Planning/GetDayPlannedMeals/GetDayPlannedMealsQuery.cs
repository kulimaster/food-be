using MediatR;

namespace Food.Application.Planning.GetDayPlannedMeals;

public sealed record GetDayPlannedMealsQuery(long UserId, DateOnly PlanDate) : IRequest<IReadOnlyList<PlannedMealDto>>;
