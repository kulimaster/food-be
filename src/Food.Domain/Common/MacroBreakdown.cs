namespace Food.Domain.Common;

public sealed record MacroBreakdown(decimal Calories, decimal ProteinG, decimal CarbsG, decimal FatG, decimal FiberG)
{
    public decimal Calories { get; init; } = Guard.NonNegative(Calories, nameof(Calories));
    public decimal ProteinG { get; init; } = Guard.NonNegative(ProteinG, nameof(ProteinG));
    public decimal CarbsG { get; init; } = Guard.NonNegative(CarbsG, nameof(CarbsG));
    public decimal FatG { get; init; } = Guard.NonNegative(FatG, nameof(FatG));
    public decimal FiberG { get; init; } = Guard.NonNegative(FiberG, nameof(FiberG));

    public static readonly MacroBreakdown Zero = new(0, 0, 0, 0, 0);

    public MacroBreakdown Scale(decimal factor)
    {
        if (factor < 0)
        {
            throw new ArgumentOutOfRangeException(nameof(factor), factor, "Scale factor cannot be negative.");
        }

        return new MacroBreakdown(Calories * factor, ProteinG * factor, CarbsG * factor, FatG * factor, FiberG * factor);
    }

    public static MacroBreakdown operator +(MacroBreakdown left, MacroBreakdown right) => new(
        left.Calories + right.Calories,
        left.ProteinG + right.ProteinG,
        left.CarbsG + right.CarbsG,
        left.FatG + right.FatG,
        left.FiberG + right.FiberG);
}
