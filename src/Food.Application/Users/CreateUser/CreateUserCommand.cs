using MediatR;

namespace Food.Application.Users.CreateUser;

public sealed record CreateUserCommand(string Email, string DisplayName, string Timezone) : IRequest<long>;
