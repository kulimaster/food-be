using Food.Application.Abstractions;
using Food.Application.Ingredients;
using Food.Application.Recipes;
using Food.Domain.Common;
using Food.Domain.Logging;
using MediatR;

namespace Food.Application.Favorites.SaveFavoriteMeal;

public sealed class SaveFavoriteMealCommandHandler : IRequestHandler<SaveFavoriteMealCommand, long>
{
    private readonly IFavoriteMealRepository _favoriteMeals;
    private readonly IRecipeRepository _recipes;
    private readonly IIngredientRepository _ingredients;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IClock _clock;

    public SaveFavoriteMealCommandHandler(
        IFavoriteMealRepository favoriteMeals,
        IRecipeRepository recipes,
        IIngredientRepository ingredients,
        IUnitOfWork unitOfWork,
        IClock clock)
    {
        _favoriteMeals = favoriteMeals;
        _recipes = recipes;
        _ingredients = ingredients;
        _unitOfWork = unitOfWork;
        _clock = clock;
    }

    public async Task<long> Handle(SaveFavoriteMealCommand request, CancellationToken cancellationToken)
    {
        LoggableItem item;

        if (request.RecipeId is not null)
        {
            var recipe = await _recipes.GetByIdAsync(request.RecipeId.Value, cancellationToken)
                ?? throw new InvalidOperationException($"Recipe {request.RecipeId} was not found.");

            item = LoggableItem.FromRecipe(recipe, request.ServingsCount!.Value);
        }
        else
        {
            var ingredient = await _ingredients.GetByIdAsync(request.IngredientId!.Value, cancellationToken)
                ?? throw new InvalidOperationException($"Ingredient {request.IngredientId} was not found.");

            item = LoggableItem.FromIngredient(ingredient, new Quantity(request.QuantityGrams!.Value));
        }

        var favoriteMeal = new FavoriteMeal(request.UserId, request.DisplayName, item, _clock.UtcNow);

        await _favoriteMeals.AddAsync(favoriteMeal, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return favoriteMeal.Id;
    }
}
