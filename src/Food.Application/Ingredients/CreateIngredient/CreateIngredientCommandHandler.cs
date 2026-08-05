using Food.Application.Abstractions;
using Food.Domain.Common;
using Food.Domain.Ingredients;
using MediatR;

namespace Food.Application.Ingredients.CreateIngredient;

public sealed class CreateIngredientCommandHandler : IRequestHandler<CreateIngredientCommand, long>
{
    private readonly IIngredientRepository _ingredients;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IClock _clock;

    public CreateIngredientCommandHandler(IIngredientRepository ingredients, IUnitOfWork unitOfWork, IClock clock)
    {
        _ingredients = ingredients;
        _unitOfWork = unitOfWork;
        _clock = clock;
    }

    public async Task<long> Handle(CreateIngredientCommand request, CancellationToken cancellationToken)
    {
        var macros = new MacroBreakdown(
            request.CaloriesPer100g,
            request.ProteinPer100g,
            request.CarbsPer100g,
            request.FatPer100g,
            request.FiberPer100g);

        var ingredient = new Ingredient(request.Name, macros, request.CreatedByUserId, _clock.UtcNow);

        foreach (var tag in request.Tags)
        {
            ingredient.AddTag(tag);
        }

        await _ingredients.AddAsync(ingredient, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return ingredient.Id;
    }
}
