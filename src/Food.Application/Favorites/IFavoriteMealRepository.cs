using Food.Domain.Logging;

namespace Food.Application.Favorites;

public interface IFavoriteMealRepository
{
    public Task AddAsync(FavoriteMeal favoriteMeal, CancellationToken cancellationToken);

    public Task<FavoriteMeal?> GetByIdAsync(long id, CancellationToken cancellationToken);

    public Task<IReadOnlyList<FavoriteMeal>> ListForUserAsync(long userId, CancellationToken cancellationToken);

    public Task RemoveAsync(FavoriteMeal favoriteMeal, CancellationToken cancellationToken);
}
