namespace Food.Application.ShoppingList;

public sealed record ShoppingListItemDto(long IngredientId, string IngredientName, decimal QuantityGrams);

public sealed record ShoppingListDto(
    DateOnly StartDate,
    DateOnly EndDate,
    IReadOnlyCollection<ShoppingListItemDto> Items);
