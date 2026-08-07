using Food.Domain.Nutrition;

namespace Food.Application.Users;

public interface INutritionTargetRepository
{
    public Task AddAsync(NutritionTarget target, CancellationToken cancellationToken);

    public Task<NutritionTarget?> GetCurrentAsync(long userId, DateOnly asOf, CancellationToken cancellationToken);
}
