using Food.Application.Abstractions;
using MediatR;

namespace Food.Application.Favorites.RemoveFavoriteMeal;

public sealed class RemoveFavoriteMealCommandHandler : IRequestHandler<RemoveFavoriteMealCommand, bool>
{
    private readonly IFavoriteMealRepository _favoriteMeals;
    private readonly IUnitOfWork _unitOfWork;

    public RemoveFavoriteMealCommandHandler(IFavoriteMealRepository favoriteMeals, IUnitOfWork unitOfWork)
    {
        _favoriteMeals = favoriteMeals;
        _unitOfWork = unitOfWork;
    }

    public async Task<bool> Handle(RemoveFavoriteMealCommand request, CancellationToken cancellationToken)
    {
        var favoriteMeal = await _favoriteMeals.GetByIdAsync(request.FavoriteMealId, cancellationToken);
        if (favoriteMeal is null || favoriteMeal.UserId != request.UserId)
        {
            return false;
        }

        await _favoriteMeals.RemoveAsync(favoriteMeal, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return true;
    }
}
