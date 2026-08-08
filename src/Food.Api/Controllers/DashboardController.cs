using Food.Application.Dashboard.GetDailyDashboard;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace Food.Api.Controllers;

[ApiController]
[Route("api/v1/users/{userId:long}/dashboard")]
public sealed class DashboardController : ControllerBase
{
    private readonly ISender _sender;

    public DashboardController(ISender sender) => _sender = sender;

    [HttpGet]
    public async Task<IActionResult> GetDaily(long userId, [FromQuery] DateOnly? date, CancellationToken cancellationToken)
    {
        var query = new GetDailyDashboardQuery(userId, date ?? DateOnly.FromDateTime(DateTime.UtcNow));
        var dashboard = await _sender.Send(query, cancellationToken);

        return dashboard is null ? NotFound() : Ok(dashboard);
    }
}
