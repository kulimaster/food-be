using Food.Domain.Common;

namespace Food.Domain.Nutrition;

public sealed class NutritionTarget : Entity
{
    public long UserId { get; private set; }
    public DateOnly EffectiveFrom { get; private set; }
    public MacroBreakdown Macros { get; private set; }
    public bool IsManualOverride { get; private set; }
    public DateTimeOffset CreatedAt { get; private set; }

    private NutritionTarget()
    {
        Macros = null!;
    }

    public NutritionTarget(long userId, DateOnly effectiveFrom, MacroBreakdown macros, bool isManualOverride, DateTimeOffset createdAt)
    {
        UserId = userId;
        EffectiveFrom = effectiveFrom;
        Macros = macros ?? throw new ArgumentNullException(nameof(macros));
        IsManualOverride = isManualOverride;
        CreatedAt = createdAt;
    }
}
