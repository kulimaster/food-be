using Food.Domain.Common;
using Food.Domain.Enums;
using Food.Domain.Nutrition;
using Food.Domain.Users;

namespace Food.Domain.Tests.Nutrition;

[TestFixture]
public class MacroTargetCalculatorTests
{
    private static readonly DateOnly AsOf = new(2026, 1, 1);

    [Test]
    public void Calculate_MaleSedentaryMaintain_MatchesExpectedTarget()
    {
        // weight=80kg, height=180cm, age=30 (DOB same day as AsOf), sedentary (x1.2), maintain.
        var profile = new UserProfile(
            userId: 1,
            weightKg: 80,
            heightCm: 180,
            dateOfBirth: new DateOnly(1996, 1, 1),
            sex: Sex.Male,
            activityLevel: ActivityLevel.Sedentary,
            goal: Goal.Maintain,
            updatedAt: DateTimeOffset.UtcNow);

        var result = MacroTargetCalculator.Calculate(profile, AsOf);

        Assert.That(result, Is.EqualTo(new MacroBreakdown(2136, 160.0m, 229.8m, 64.1m, 29.9m)));
    }

    [Test]
    public void Calculate_FemaleLightlyActiveLose_MatchesExpectedTarget()
    {
        // weight=60kg, height=165cm, age=25, lightly active (x1.375), lose (x0.8).
        var profile = new UserProfile(
            userId: 1,
            weightKg: 60,
            heightCm: 165,
            dateOfBirth: new DateOnly(2001, 1, 1),
            sex: Sex.Female,
            activityLevel: ActivityLevel.LightlyActive,
            goal: Goal.Lose,
            updatedAt: DateTimeOffset.UtcNow);

        var result = MacroTargetCalculator.Calculate(profile, AsOf);

        Assert.That(result, Is.EqualTo(new MacroBreakdown(1480, 120.0m, 150.1m, 44.4m, 20.7m)));
    }

    [Test]
    public void Calculate_Throws_WhenProfileIsNull()
    {
        Assert.Throws<ArgumentNullException>(() => MacroTargetCalculator.Calculate(null!, AsOf));
    }
}
