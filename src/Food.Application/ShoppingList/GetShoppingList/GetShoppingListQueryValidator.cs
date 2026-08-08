using FluentValidation;

namespace Food.Application.ShoppingList.GetShoppingList;

public sealed class GetShoppingListQueryValidator : AbstractValidator<GetShoppingListQuery>
{
    public GetShoppingListQueryValidator()
    {
        RuleFor(x => x.UserId).GreaterThan(0);
        RuleFor(x => x.EndDate).GreaterThanOrEqualTo(x => x.StartDate);
    }
}
