using Food.Application.Favorites.ListFavoriteMeals;
using Food.Application.Favorites.RemoveFavoriteMeal;
using Food.Application.Favorites.SaveFavoriteMeal;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace Food.Api.Controllers;

[ApiController]
[Route("api/v1/users/{userId:long}/favorite-meals")]
public sealed class FavoriteMealsController : ControllerBase
{
    private readonly ISender _sender;

    public FavoriteMealsController(ISender sender) => _sender = sender;

    [HttpPost]
    public async Task<IActionResult> Create(long userId, SaveFavoriteMealRequest request, CancellationToken cancellationToken)
    {
        var command = new SaveFavoriteMealCommand(
            userId,
            request.DisplayName,
            request.RecipeId,
            request.ServingsCount,
            request.IngredientId,
            request.QuantityGrams);

        var id = await _sender.Send(command, cancellationToken);

        return Created($"/api/v1/users/{userId}/favorite-meals/{id}", new { id });
    }

    [HttpGet]
    public async Task<IActionResult> List(long userId, CancellationToken cancellationToken)
    {
        var favorites = await _sender.Send(new ListFavoriteMealsQuery(userId), cancellationToken);

        return Ok(favorites);
    }

    [HttpDelete("{id:long}")]
    public async Task<IActionResult> Remove(long userId, long id, CancellationToken cancellationToken)
    {
        var removed = await _sender.Send(new RemoveFavoriteMealCommand(userId, id), cancellationToken);

        return removed ? NoContent() : NotFound();
    }
}

public sealed record SaveFavoriteMealRequest(
    string DisplayName,
    long? RecipeId,
    decimal? ServingsCount,
    long? IngredientId,
    decimal? QuantityGrams);
