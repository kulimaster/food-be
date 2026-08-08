using Food.Application.Logging.GetDayMealLogs;
using Food.Application.Logging.LogMeal;
using Food.Domain.Enums;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace Food.Api.Controllers;

[ApiController]
[Route("api/v1/users/{userId:long}/meal-logs")]
public sealed class MealLogsController : ControllerBase
{
    private readonly ISender _sender;

    public MealLogsController(ISender sender) => _sender = sender;

    [HttpPost]
    public async Task<IActionResult> Create(long userId, LogMealRequest request, CancellationToken cancellationToken)
    {
        var command = new LogMealCommand(
            userId,
            request.LogDate,
            request.MealSlot,
            request.RecipeId,
            request.ServingsCount,
            request.IngredientId,
            request.QuantityGrams);

        var id = await _sender.Send(command, cancellationToken);

        return Created($"/api/v1/users/{userId}/meal-logs/{id}", new { id });
    }

    [HttpGet]
    public async Task<IActionResult> ListForDay(long userId, [FromQuery] DateOnly? date, CancellationToken cancellationToken)
    {
        var query = new GetDayMealLogsQuery(userId, date ?? DateOnly.FromDateTime(DateTime.UtcNow));
        var mealLogs = await _sender.Send(query, cancellationToken);

        return Ok(mealLogs);
    }
}

public sealed record LogMealRequest(
    DateOnly LogDate,
    MealSlot MealSlot,
    long? RecipeId,
    decimal? ServingsCount,
    long? IngredientId,
    decimal? QuantityGrams);
