using MediatR;

namespace Food.Application.Favorites.RemoveFavoriteMeal;

public sealed record RemoveFavoriteMealCommand(long UserId, long FavoriteMealId) : IRequest<bool>;
