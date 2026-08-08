using Food.Application.Planning.GetPlannedDashboard;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace Food.Api.Controllers;

[ApiController]
[Route("api/v1/users/{userId:long}/planned-dashboard")]
public sealed class PlannedDashboardController : ControllerBase
{
    private readonly ISender _sender;

    public PlannedDashboardController(ISender sender) => _sender = sender;

    [HttpGet]
    public async Task<IActionResult> GetForDay(long userId, [FromQuery] DateOnly? date, CancellationToken cancellationToken)
    {
        var query = new GetPlannedDashboardQuery(userId, date ?? DateOnly.FromDateTime(DateTime.UtcNow));
        var dashboard = await _sender.Send(query, cancellationToken);

        return dashboard is null ? NotFound() : Ok(dashboard);
    }
}
