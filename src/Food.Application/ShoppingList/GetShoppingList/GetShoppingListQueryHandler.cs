using Food.Application.Planning;
using MediatR;

namespace Food.Application.ShoppingList.GetShoppingList;

public sealed class GetShoppingListQueryHandler : IRequestHandler<GetShoppingListQuery, ShoppingListDto>
{
    private readonly IPlannedMealRepository _plannedMeals;

    public GetShoppingListQueryHandler(IPlannedMealRepository plannedMeals) => _plannedMeals = plannedMeals;

    public async Task<ShoppingListDto> Handle(GetShoppingListQuery request, CancellationToken cancellationToken)
    {
        var plannedMeals = await _plannedMeals.ListForRangeAsync(
            request.UserId, request.StartDate, request.EndDate, cancellationToken);

        var totals = new Dictionary<long, (string Name, decimal Grams)>();

        foreach (var plannedMeal in plannedMeals)
        {
            if (plannedMeal.Item.Recipe is not null)
            {
                var recipe = plannedMeal.Item.Recipe;
                // RecipeIngredient.Quantity is the amount used for the whole recipe (all
                // Servings), same convention Recipe.MacrosPerServing() relies on - so a
                // planned ServingsCount must scale by ServingsCount / Servings, not a
                // flat multiply, or a >1-serving recipe would massively over-count.
                var scaleFactor = plannedMeal.Item.ServingsCount!.Value / recipe.Servings;
                foreach (var recipeIngredient in recipe.Ingredients)
                {
                    Accumulate(totals, recipeIngredient.IngredientId, recipeIngredient.Ingredient.Name,
                        recipeIngredient.Quantity.Grams * scaleFactor);
                }
            }
            else
            {
                var ingredient = plannedMeal.Item.Ingredient!;
                Accumulate(totals, ingredient.Id, ingredient.Name, plannedMeal.Item.Quantity!.Grams);
            }
        }

        var items = totals
            .Select(kvp => new ShoppingListItemDto(kvp.Key, kvp.Value.Name, kvp.Value.Grams))
            .OrderBy(i => i.IngredientName)
            .ToList();

        return new ShoppingListDto(request.StartDate, request.EndDate, items);
    }

    private static void Accumulate(Dictionary<long, (string Name, decimal Grams)> totals, long ingredientId, string name, decimal grams)
    {
        totals[ingredientId] = totals.TryGetValue(ingredientId, out var existing)
            ? (name, existing.Grams + grams)
            : (name, grams);
    }
}
