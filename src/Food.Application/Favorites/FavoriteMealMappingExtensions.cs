using Food.Domain.Logging;

namespace Food.Application.Favorites;

public static class FavoriteMealMappingExtensions
{
    public static FavoriteMealDto ToDto(this FavoriteMeal favoriteMeal) => new(
        favoriteMeal.Id,
        favoriteMeal.UserId,
        favoriteMeal.DisplayName,
        favoriteMeal.Item.Recipe?.Id,
        favoriteMeal.Item.Recipe?.Name,
        favoriteMeal.Item.ServingsCount,
        favoriteMeal.Item.Ingredient?.Id,
        favoriteMeal.Item.Ingredient?.Name,
        favoriteMeal.Item.Quantity?.Grams,
        favoriteMeal.Macros(),
        favoriteMeal.CreatedAt);
}
