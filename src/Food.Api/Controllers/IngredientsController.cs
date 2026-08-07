using Food.Application.Ingredients.CreateIngredient;
using Food.Application.Ingredients.GetIngredientById;
using Food.Application.Ingredients.ListIngredients;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace Food.Api.Controllers;

[ApiController]
[Route("api/v1/ingredients")]
public sealed class IngredientsController : ControllerBase
{
    private readonly ISender _sender;

    public IngredientsController(ISender sender) => _sender = sender;

    [HttpPost]
    public async Task<IActionResult> Create(CreateIngredientRequest request, CancellationToken cancellationToken)
    {
        var command = new CreateIngredientCommand(
            request.Name,
            request.CaloriesPer100g,
            request.ProteinPer100g,
            request.CarbsPer100g,
            request.FatPer100g,
            request.FiberPer100g,
            request.CreatedByUserId,
            request.Tags);

        var id = await _sender.Send(command, cancellationToken);

        return Created($"/api/v1/ingredients/{id}", new { id });
    }

    [HttpGet]
    public async Task<IActionResult> List([FromQuery] string? search, [FromQuery] string? tag, CancellationToken cancellationToken)
    {
        var ingredients = await _sender.Send(new ListIngredientsQuery(search, tag), cancellationToken);

        return Ok(ingredients);
    }

    [HttpGet("{id:long}")]
    public async Task<IActionResult> GetById(long id, CancellationToken cancellationToken)
    {
        var ingredient = await _sender.Send(new GetIngredientByIdQuery(id), cancellationToken);

        return ingredient is null ? NotFound() : Ok(ingredient);
    }
}

public sealed record CreateIngredientRequest(
    string Name,
    decimal CaloriesPer100g,
    decimal ProteinPer100g,
    decimal CarbsPer100g,
    decimal FatPer100g,
    decimal FiberPer100g,
    long CreatedByUserId,
    IReadOnlyCollection<string> Tags);
