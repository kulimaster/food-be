using Food.Application.Users;
using Food.Domain.Users;
using Microsoft.EntityFrameworkCore;

namespace Food.Infrastructure.Persistence.Repositories;

public sealed class UserProfileRepository : IUserProfileRepository
{
    private readonly FoodDbContext _dbContext;

    public UserProfileRepository(FoodDbContext dbContext) => _dbContext = dbContext;

    public Task<UserProfile?> GetByUserIdAsync(long userId, CancellationToken cancellationToken) =>
        _dbContext.UserProfiles.SingleOrDefaultAsync(p => p.UserId == userId, cancellationToken);

    public async Task AddAsync(UserProfile profile, CancellationToken cancellationToken) =>
        await _dbContext.UserProfiles.AddAsync(profile, cancellationToken);
}
