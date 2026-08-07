using Food.Domain.Common;
using Food.Domain.Enums;
using MediatR;

namespace Food.Application.Users.SetUserProfile;

public sealed record SetUserProfileCommand(
    long UserId,
    decimal WeightKg,
    decimal HeightCm,
    DateOnly DateOfBirth,
    Sex Sex,
    ActivityLevel ActivityLevel,
    Goal Goal) : IRequest<MacroBreakdown>;
