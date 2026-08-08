using Food.Application.Planning.GetDayPlannedMeals;
using Food.Application.Planning.GetPlannedDashboard;
using Food.Application.Planning.PlanMeal;
using Food.Domain.Enums;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace Food.Api.Controllers;

[ApiController]
[Route("api/v1/users/{userId:long}/planned-meals")]
public sealed class PlannedMealsController : ControllerBase
{
    private readonly ISender _sender;

    public PlannedMealsController(ISender sender) => _sender = sender;

    [HttpPost]
    public async Task<IActionResult> Create(long userId, PlanMealRequest request, CancellationToken cancellationToken)
    {
        var command = new PlanMealCommand(
            userId,
            request.PlanDate,
            request.MealSlot,
            request.RecipeId,
            request.ServingsCount,
            request.IngredientId,
            request.QuantityGrams);

        var id = await _sender.Send(command, cancellationToken);

        return Created($"/api/v1/users/{userId}/planned-meals/{id}", new { id });
    }

    [HttpGet]
    public async Task<IActionResult> ListForDay(long userId, [FromQuery] DateOnly? date, CancellationToken cancellationToken)
    {
        var query = new GetDayPlannedMealsQuery(userId, date ?? DateOnly.FromDateTime(DateTime.UtcNow));
        var plannedMeals = await _sender.Send(query, cancellationToken);

        return Ok(plannedMeals);
    }
}

public sealed record PlanMealRequest(
    DateOnly PlanDate,
    MealSlot MealSlot,
    long? RecipeId,
    decimal? ServingsCount,
    long? IngredientId,
    decimal? QuantityGrams);
