using Food.Domain.Users;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Food.Infrastructure.Persistence.Configurations;

public sealed class UserProfileConfiguration : IEntityTypeConfiguration<UserProfile>
{
    public void Configure(EntityTypeBuilder<UserProfile> builder)
    {
        builder.ToTable("user_profiles");

        builder.HasKey(p => p.Id);
        builder.Property(p => p.Id).ValueGeneratedOnAdd();

        builder.Property(p => p.UserId).IsRequired();
        builder.HasIndex(p => p.UserId).IsUnique();

        builder.Property(p => p.WeightKg).IsRequired();
        builder.Property(p => p.HeightCm).IsRequired();
        builder.Property(p => p.DateOfBirth).IsRequired();
        builder.Property(p => p.Sex).IsRequired().HasConversion<string>().HasMaxLength(20);
        builder.Property(p => p.ActivityLevel).IsRequired().HasConversion<string>().HasMaxLength(30);
        builder.Property(p => p.Goal).IsRequired().HasConversion<string>().HasMaxLength(20);
        builder.Property(p => p.UpdatedAt).IsRequired();
    }
}
