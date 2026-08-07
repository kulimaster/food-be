using Food.Application.Users.CreateUser;
using Food.Application.Users.GetCurrentNutritionTarget;
using Food.Application.Users.SetUserProfile;
using Food.Domain.Enums;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace Food.Api.Controllers;

[ApiController]
[Route("api/v1/users")]
public sealed class UsersController : ControllerBase
{
    private readonly ISender _sender;

    public UsersController(ISender sender) => _sender = sender;

    [HttpPost]
    public async Task<IActionResult> Create(CreateUserRequest request, CancellationToken cancellationToken)
    {
        var command = new CreateUserCommand(request.Email, request.DisplayName, request.Timezone);
        var id = await _sender.Send(command, cancellationToken);

        return Created($"/api/v1/users/{id}", new { id });
    }

    [HttpPut("{userId:long}/profile")]
    public async Task<IActionResult> SetProfile(long userId, SetUserProfileRequest request, CancellationToken cancellationToken)
    {
        var command = new SetUserProfileCommand(
            userId,
            request.WeightKg,
            request.HeightCm,
            request.DateOfBirth,
            request.Sex,
            request.ActivityLevel,
            request.Goal);

        var macros = await _sender.Send(command, cancellationToken);

        return Ok(macros);
    }

    [HttpGet("{userId:long}/nutrition-target")]
    public async Task<IActionResult> GetNutritionTarget(long userId, [FromQuery] DateOnly? asOf, CancellationToken cancellationToken)
    {
        var query = new GetCurrentNutritionTargetQuery(userId, asOf ?? DateOnly.FromDateTime(DateTime.UtcNow));
        var macros = await _sender.Send(query, cancellationToken);

        return macros is null ? NotFound() : Ok(macros);
    }
}

public sealed record CreateUserRequest(string Email, string DisplayName, string Timezone);

public sealed record SetUserProfileRequest(
    decimal WeightKg,
    decimal HeightCm,
    DateOnly DateOfBirth,
    Sex Sex,
    ActivityLevel ActivityLevel,
    Goal Goal);
