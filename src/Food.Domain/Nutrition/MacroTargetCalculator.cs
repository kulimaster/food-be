using Food.Domain.Common;
using Food.Domain.Enums;
using Food.Domain.Users;

namespace Food.Domain.Nutrition;

// Macro-split ratios below are placeholders pending confirmation
// (see docs/business-description.md open questions).
public static class MacroTargetCalculator
{
    private const decimal ProteinGramsPerKg = 2.0m;
    private const decimal FatCalorieRatio = 0.27m;
    private const decimal FiberGramsPer1000Kcal = 14m;

    public static MacroBreakdown Calculate(UserProfile profile, DateOnly asOf)
    {
        ArgumentNullException.ThrowIfNull(profile);

        var bmr = CalculateBmr(profile, asOf);
        var tdee = bmr * ActivityMultiplier(profile.ActivityLevel);
        var calories = ApplyGoal(tdee, profile.Goal);

        var proteinG = profile.WeightKg * ProteinGramsPerKg;
        var fatG = calories * FatCalorieRatio / 9m;
        var fiberG = calories / 1000m * FiberGramsPer1000Kcal;
        var carbsCalories = Math.Max(calories - proteinG * 4m - fatG * 9m, 0m);
        var carbsG = carbsCalories / 4m;

        return new MacroBreakdown(
            Math.Round(calories, 0),
            Math.Round(proteinG, 1),
            Math.Round(carbsG, 1),
            Math.Round(fatG, 1),
            Math.Round(fiberG, 1));
    }

    private static decimal CalculateBmr(UserProfile profile, DateOnly asOf)
    {
        var age = profile.AgeInYears(asOf);
        var baseBmr = 10m * profile.WeightKg + 6.25m * profile.HeightCm - 5m * age;

        return profile.Sex == Sex.Male ? baseBmr + 5m : baseBmr - 161m;
    }

    private static decimal ActivityMultiplier(ActivityLevel level) => level switch
    {
        ActivityLevel.Sedentary => 1.2m,
        ActivityLevel.LightlyActive => 1.375m,
        ActivityLevel.ModeratelyActive => 1.55m,
        ActivityLevel.VeryActive => 1.725m,
        ActivityLevel.ExtraActive => 1.9m,
        _ => throw new ArgumentOutOfRangeException(nameof(level), level, null)
    };

    private static decimal ApplyGoal(decimal tdee, Goal goal) => goal switch
    {
        Goal.Lose => tdee * 0.8m,
        Goal.Maintain => tdee,
        Goal.Gain => tdee * 1.1m,
        _ => throw new ArgumentOutOfRangeException(nameof(goal), goal, null)
    };
}
