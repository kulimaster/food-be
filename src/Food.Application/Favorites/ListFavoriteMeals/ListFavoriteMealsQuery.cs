using MediatR;

namespace Food.Application.Favorites.ListFavoriteMeals;

public sealed record ListFavoriteMealsQuery(long UserId) : IRequest<IReadOnlyList<FavoriteMealDto>>;
