using Food.Domain.Activities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Food.Infrastructure.Persistence.Configurations;

public sealed class ActivityLogConfiguration : IEntityTypeConfiguration<ActivityLog>
{
    public void Configure(EntityTypeBuilder<ActivityLog> builder)
    {
        builder.ToTable("activity_logs");

        builder.HasKey(a => a.Id);
        builder.Property(a => a.Id).ValueGeneratedOnAdd();

        builder.Property(a => a.UserId).IsRequired();
        builder.Property(a => a.LogDate).IsRequired();
        builder.Property(a => a.ActivityType).IsRequired().HasMaxLength(100);
        builder.Property(a => a.DurationMinutes).IsRequired();
        builder.Property(a => a.CaloriesBurned).IsRequired();
        builder.Property(a => a.LoggedAt).IsRequired();

        builder.HasIndex(a => new { a.UserId, a.LogDate });
    }
}
