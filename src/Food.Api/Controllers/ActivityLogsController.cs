using Food.Application.Activities.GetDayActivityLogs;
using Food.Application.Activities.LogActivity;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace Food.Api.Controllers;

[ApiController]
[Route("api/v1/users/{userId:long}/activity-logs")]
public sealed class ActivityLogsController : ControllerBase
{
    private readonly ISender _sender;

    public ActivityLogsController(ISender sender) => _sender = sender;

    [HttpPost]
    public async Task<IActionResult> Create(long userId, LogActivityRequest request, CancellationToken cancellationToken)
    {
        var command = new LogActivityCommand(
            userId,
            request.LogDate,
            request.ActivityType,
            request.DurationMinutes,
            request.CaloriesBurned);

        var id = await _sender.Send(command, cancellationToken);

        return Created($"/api/v1/users/{userId}/activity-logs/{id}", new { id });
    }

    [HttpGet]
    public async Task<IActionResult> ListForDay(long userId, [FromQuery] DateOnly? date, CancellationToken cancellationToken)
    {
        var query = new GetDayActivityLogsQuery(userId, date ?? DateOnly.FromDateTime(DateTime.UtcNow));
        var activityLogs = await _sender.Send(query, cancellationToken);

        return Ok(activityLogs);
    }
}

public sealed record LogActivityRequest(
    DateOnly LogDate,
    string ActivityType,
    int DurationMinutes,
    int CaloriesBurned);
