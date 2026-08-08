using Food.Application.Abstractions;
using Food.Application.Ingredients;
using Food.Application.Recipes;
using Food.Domain.Common;
using Food.Domain.Logging;
using Food.Domain.Planning;
using MediatR;

namespace Food.Application.Planning.PlanMeal;

public sealed class PlanMealCommandHandler : IRequestHandler<PlanMealCommand, long>
{
    private readonly IPlannedMealRepository _plannedMeals;
    private readonly IRecipeRepository _recipes;
    private readonly IIngredientRepository _ingredients;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IClock _clock;

    public PlanMealCommandHandler(
        IPlannedMealRepository plannedMeals,
        IRecipeRepository recipes,
        IIngredientRepository ingredients,
        IUnitOfWork unitOfWork,
        IClock clock)
    {
        _plannedMeals = plannedMeals;
        _recipes = recipes;
        _ingredients = ingredients;
        _unitOfWork = unitOfWork;
        _clock = clock;
    }

    public async Task<long> Handle(PlanMealCommand request, CancellationToken cancellationToken)
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

        var plannedMeal = new PlannedMeal(request.UserId, request.PlanDate, request.MealSlot, item, _clock.UtcNow);

        await _plannedMeals.AddAsync(plannedMeal, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return plannedMeal.Id;
    }
}
