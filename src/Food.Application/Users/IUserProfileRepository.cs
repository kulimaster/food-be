using Food.Domain.Users;

namespace Food.Application.Users;

public interface IUserProfileRepository
{
    public Task<UserProfile?> GetByUserIdAsync(long userId, CancellationToken cancellationToken);

    public Task AddAsync(UserProfile profile, CancellationToken cancellationToken);
}
