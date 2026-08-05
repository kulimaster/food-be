namespace Food.Domain.Common;

public sealed record Quantity(decimal Grams)
{
    public decimal Grams { get; init; } = Guard.Positive(Grams, nameof(Grams));
}
