using Food.Domain.Common;
using Food.Domain.Enums;

namespace Food.Domain.Users;

public sealed class UserProfile : Entity
{
    public long UserId { get; private set; }
    public decimal WeightKg { get; private set; }
    public decimal HeightCm { get; private set; }
    public DateOnly DateOfBirth { get; private set; }
    public Sex Sex { get; private set; }
    public ActivityLevel ActivityLevel { get; private set; }
    public Goal Goal { get; private set; }
    public DateTimeOffset UpdatedAt { get; private set; }

    private UserProfile()
    {
    }

    public UserProfile(
        long userId,
        decimal weightKg,
        decimal heightCm,
        DateOnly dateOfBirth,
        Sex sex,
        ActivityLevel activityLevel,
        Goal goal,
        DateTimeOffset updatedAt)
    {
        UserId = userId;
        WeightKg = Guard.Positive(weightKg, nameof(weightKg));
        HeightCm = Guard.Positive(heightCm, nameof(heightCm));
        DateOfBirth = dateOfBirth;
        Sex = sex;
        ActivityLevel = activityLevel;
        Goal = goal;
        UpdatedAt = updatedAt;
    }

    public void Update(decimal weightKg, decimal heightCm, ActivityLevel activityLevel, Goal goal, DateTimeOffset updatedAt)
    {
        WeightKg = Guard.Positive(weightKg, nameof(weightKg));
        HeightCm = Guard.Positive(heightCm, nameof(heightCm));
        ActivityLevel = activityLevel;
        Goal = goal;
        UpdatedAt = updatedAt;
    }

    public int AgeInYears(DateOnly asOf)
    {
        var age = asOf.Year - DateOfBirth.Year;
        if (asOf < DateOfBirth.AddYears(age))
        {
            age--;
        }

        return age;
    }
}
