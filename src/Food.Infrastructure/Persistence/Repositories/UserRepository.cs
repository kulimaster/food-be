using Food.Application.Users;
using Food.Domain.Users;

namespace Food.Infrastructure.Persistence.Repositories;

public sealed class UserRepository : IUserRepository
{
    private readonly FoodDbContext _dbContext;

    public UserRepository(FoodDbContext dbContext) => _dbContext = dbContext;

    public async Task AddAsync(User user, CancellationToken cancellationToken) =>
        await _dbContext.Users.AddAsync(user, cancellationToken);
}
