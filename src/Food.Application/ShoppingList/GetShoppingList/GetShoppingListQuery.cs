using MediatR;

namespace Food.Application.ShoppingList.GetShoppingList;

public sealed record GetShoppingListQuery(long UserId, DateOnly StartDate, DateOnly EndDate) : IRequest<ShoppingListDto>;
