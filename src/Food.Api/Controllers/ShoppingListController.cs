using Food.Application.ShoppingList.GetShoppingList;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace Food.Api.Controllers;

[ApiController]
[Route("api/v1/users/{userId:long}/shopping-list")]
public sealed class ShoppingListController : ControllerBase
{
    private readonly ISender _sender;

    public ShoppingListController(ISender sender) => _sender = sender;

    [HttpGet]
    public async Task<IActionResult> Get(
        long userId,
        [FromQuery] DateOnly startDate,
        [FromQuery] DateOnly endDate,
        CancellationToken cancellationToken)
    {
        var query = new GetShoppingListQuery(userId, startDate, endDate);
        var shoppingList = await _sender.Send(query, cancellationToken);

        return Ok(shoppingList);
    }
}
