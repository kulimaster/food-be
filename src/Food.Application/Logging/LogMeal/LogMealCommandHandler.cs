using Food.Application.Abstractions;
using Food.Application.Ingredients;
using Food.Application.Recipes;
using Food.Domain.Common;
using Food.Domain.Logging;
using MediatR;

namespace Food.Application.Logging.LogMeal;

public sealed class LogMealCommandHandler : IRequestHandler<LogMealCommand, long>
{
    private readonly IMealLogRepository _mealLogs;
    private readonly IRecipeRepository _recipes;
    private readonly IIngredientRepository _ingredients;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IClock _clock;

    public LogMealCommandHandler(
        IMealLogRepository mealLogs,
        IRecipeRepository recipes,
        IIngredientRepository ingredients,
        IUnitOfWork unitOfWork,
        IClock clock)
    {
        _mealLogs = mealLogs;
        _recipes = recipes;
        _ingredients = ingredients;
        _unitOfWork = unitOfWork;
        _clock = clock;
    }

    public async Task<long> Handle(LogMealCommand request, CancellationToken cancellationToken)
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

        var mealLog = new MealLog(request.UserId, request.LogDate, request.MealSlot, item, _clock.UtcNow);

        await _mealLogs.AddAsync(mealLog, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return mealLog.Id;
    }
}
