using Food.Domain.Enums;
using Food.Domain.Users;

namespace Food.Domain.Tests.Users;

[TestFixture]
public class UserProfileTests
{
    private static UserProfile CreateProfile(DateOnly dateOfBirth) => new(
        userId: 1,
        weightKg: 80,
        heightCm: 180,
        dateOfBirth: dateOfBirth,
        sex: Sex.Male,
        activityLevel: ActivityLevel.Sedentary,
        goal: Goal.Maintain,
        updatedAt: DateTimeOffset.UtcNow);

    [Test]
    public void AgeInYears_WhenBirthdayAlreadyOccurredThisYear_ReturnsFullYears()
    {
        var profile = CreateProfile(new DateOnly(2000, 1, 1));

        var age = profile.AgeInYears(new DateOnly(2026, 6, 1));

        Assert.That(age, Is.EqualTo(26));
    }

    [Test]
    public void AgeInYears_WhenBirthdayNotYetOccurredThisYear_ReturnsOneLess()
    {
        var profile = CreateProfile(new DateOnly(2000, 12, 31));

        var age = profile.AgeInYears(new DateOnly(2026, 6, 1));

        Assert.That(age, Is.EqualTo(25));
    }

    [TestCase(0)]
    [TestCase(-5)]
    public void Constructor_Throws_WhenWeightIsNotPositive(decimal weightKg)
    {
        Assert.Throws<ArgumentOutOfRangeException>(() => new UserProfile(
            userId: 1,
            weightKg: weightKg,
            heightCm: 180,
            dateOfBirth: new DateOnly(2000, 1, 1),
            sex: Sex.Male,
            activityLevel: ActivityLevel.Sedentary,
            goal: Goal.Maintain,
            updatedAt: DateTimeOffset.UtcNow));
    }
}
