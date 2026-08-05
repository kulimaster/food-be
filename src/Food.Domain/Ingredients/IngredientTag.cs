using Food.Domain.Common;

namespace Food.Domain.Ingredients;

public sealed record IngredientTag(string Name)
{
    public string Name { get; init; } = Guard.NotEmpty(Name, nameof(Name)).Trim();
}
