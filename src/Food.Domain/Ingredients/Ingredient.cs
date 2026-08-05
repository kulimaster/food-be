using Food.Domain.Common;

namespace Food.Domain.Ingredients;

public sealed class Ingredient : Entity
{
    private readonly List<IngredientTag> _tags = new();

    public string Name { get; private set; }
    public MacroBreakdown MacrosPer100g { get; private set; }
    public long CreatedByUserId { get; private set; }
    public DateTimeOffset CreatedAt { get; private set; }
    public IReadOnlyCollection<IngredientTag> Tags => _tags.AsReadOnly();

    private Ingredient()
    {
        Name = null!;
        MacrosPer100g = null!;
    }

    public Ingredient(string name, MacroBreakdown macrosPer100g, long createdByUserId, DateTimeOffset createdAt)
    {
        Name = Guard.NotEmpty(name, nameof(name));
        MacrosPer100g = macrosPer100g ?? throw new ArgumentNullException(nameof(macrosPer100g));
        CreatedByUserId = createdByUserId;
        CreatedAt = createdAt;
    }

    public void AddTag(string tagName)
    {
        var tag = new IngredientTag(tagName);
        if (!_tags.Any(t => string.Equals(t.Name, tag.Name, StringComparison.OrdinalIgnoreCase)))
        {
            _tags.Add(tag);
        }
    }

    public void RemoveTag(string tagName) =>
        _tags.RemoveAll(t => string.Equals(t.Name, tagName, StringComparison.OrdinalIgnoreCase));

    public MacroBreakdown MacrosForQuantity(Quantity quantity)
    {
        ArgumentNullException.ThrowIfNull(quantity);
        return MacrosPer100g.Scale(quantity.Grams / 100m);
    }
}
