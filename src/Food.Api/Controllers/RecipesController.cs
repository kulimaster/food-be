using Food.Application.Recipes.CreateRecipe;
using Food.Application.Recipes.GetRecipeById;
using Food.Application.Recipes.ListRecipes;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace Food.Api.Controllers;

[ApiController]
[Route("api/v1/recipes")]
public sealed class RecipesController : ControllerBase
{
    private readonly ISender _sender;

    public RecipesController(ISender sender) => _sender = sender;

    [HttpPost]
    public async Task<IActionResult> Create(CreateRecipeRequest request, CancellationToken cancellationToken)
    {
        var command = new CreateRecipeCommand(
            request.Name,
            request.Servings,
            request.CreatedByUserId,
            request.Ingredients
                .Select(i => new CreateRecipeIngredientLine(i.IngredientId, i.QuantityGrams))
                .ToList());

        var id = await _sender.Send(command, cancellationToken);

        return Created($"/api/v1/recipes/{id}", new { id });
    }

    [HttpGet]
    public async Task<IActionResult> List([FromQuery] string? search, CancellationToken cancellationToken)
    {
        var recipes = await _sender.Send(new ListRecipesQuery(search), cancellationToken);

        return Ok(recipes);
    }

    [HttpGet("{id:long}")]
    public async Task<IActionResult> GetById(long id, CancellationToken cancellationToken)
    {
        var recipe = await _sender.Send(new GetRecipeByIdQuery(id), cancellationToken);

        return recipe is null ? NotFound() : Ok(recipe);
    }
}

public sealed record CreateRecipeIngredientLineRequest(long IngredientId, decimal QuantityGrams);

public sealed record CreateRecipeRequest(
    string Name,
    int Servings,
    long CreatedByUserId,
    IReadOnlyCollection<CreateRecipeIngredientLineRequest> Ingredients);
