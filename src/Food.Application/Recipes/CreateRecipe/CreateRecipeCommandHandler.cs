using Food.Application.Abstractions;
using Food.Application.Ingredients;
using Food.Domain.Common;
using Food.Domain.Recipes;
using MediatR;

namespace Food.Application.Recipes.CreateRecipe;

public sealed class CreateRecipeCommandHandler : IRequestHandler<CreateRecipeCommand, long>
{
    private readonly IRecipeRepository _recipes;
    private readonly IIngredientRepository _ingredients;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IClock _clock;

    public CreateRecipeCommandHandler(
        IRecipeRepository recipes,
        IIngredientRepository ingredients,
        IUnitOfWork unitOfWork,
        IClock clock)
    {
        _recipes = recipes;
        _ingredients = ingredients;
        _unitOfWork = unitOfWork;
        _clock = clock;
    }

    public async Task<long> Handle(CreateRecipeCommand request, CancellationToken cancellationToken)
    {
        var recipe = new Recipe(request.Name, request.Servings, request.CreatedByUserId, _clock.UtcNow);

        foreach (var line in request.Ingredients)
        {
            var ingredient = await _ingredients.GetByIdAsync(line.IngredientId, cancellationToken)
                ?? throw new InvalidOperationException($"Ingredient {line.IngredientId} was not found.");

            recipe.AddIngredient(ingredient, new Quantity(line.QuantityGrams));
        }

        await _recipes.AddAsync(recipe, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return recipe.Id;
    }
}
