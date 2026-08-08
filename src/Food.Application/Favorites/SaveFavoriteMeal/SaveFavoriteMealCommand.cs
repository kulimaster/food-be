using MediatR;

namespace Food.Application.Favorites.SaveFavoriteMeal;

public sealed record SaveFavoriteMealCommand(
    long UserId,
    string DisplayName,
    long? RecipeId,
    decimal? ServingsCount,
    long? IngredientId,
    decimal? QuantityGrams) : IRequest<long>;
