using Food.Application.Abstractions;

namespace Food.Infrastructure.Persistence;

public sealed class UnitOfWork : IUnitOfWork
{
    private readonly FoodDbContext _dbContext;

    public UnitOfWork(FoodDbContext dbContext) => _dbContext = dbContext;

    public Task<int> SaveChangesAsync(CancellationToken cancellationToken) =>
        _dbContext.SaveChangesAsync(cancellationToken);
}
