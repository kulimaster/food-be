using MediatR;

namespace Food.Application.Favorites.ListFavoriteMeals;

public sealed class ListFavoriteMealsQueryHandler : IRequestHandler<ListFavoriteMealsQuery, IReadOnlyList<FavoriteMealDto>>
{
    private readonly IFavoriteMealRepository _favoriteMeals;

    public ListFavoriteMealsQueryHandler(IFavoriteMealRepository favoriteMeals) => _favoriteMeals = favoriteMeals;

    public async Task<IReadOnlyList<FavoriteMealDto>> Handle(ListFavoriteMealsQuery request, CancellationToken cancellationToken)
    {
        var favoriteMeals = await _favoriteMeals.ListForUserAsync(request.UserId, cancellationToken);
        return favoriteMeals.Select(f => f.ToDto()).ToList();
    }
}
