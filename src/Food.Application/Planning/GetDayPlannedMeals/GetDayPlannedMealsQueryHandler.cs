using MediatR;

namespace Food.Application.Planning.GetDayPlannedMeals;

public sealed class GetDayPlannedMealsQueryHandler : IRequestHandler<GetDayPlannedMealsQuery, IReadOnlyList<PlannedMealDto>>
{
    private readonly IPlannedMealRepository _plannedMeals;

    public GetDayPlannedMealsQueryHandler(IPlannedMealRepository plannedMeals) => _plannedMeals = plannedMeals;

    public async Task<IReadOnlyList<PlannedMealDto>> Handle(GetDayPlannedMealsQuery request, CancellationToken cancellationToken)
    {
        var plannedMeals = await _plannedMeals.ListForDayAsync(request.UserId, request.PlanDate, cancellationToken);
        return plannedMeals.Select(m => m.ToDto()).ToList();
    }
}
