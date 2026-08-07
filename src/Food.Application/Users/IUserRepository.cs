using Food.Domain.Users;

namespace Food.Application.Users;

public interface IUserRepository
{
    public Task AddAsync(User user, CancellationToken cancellationToken);
}
