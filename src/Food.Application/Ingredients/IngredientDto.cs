namespace Food.Application.Ingredients;

public sealed record IngredientDto(
    long Id,
    string Name,
    decimal CaloriesPer100g,
    decimal ProteinPer100g,
    decimal CarbsPer100g,
    decimal FatPer100g,
    decimal FiberPer100g,
    IReadOnlyCollection<string> Tags);
