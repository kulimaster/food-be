using Food.Domain.Nutrition;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Food.Infrastructure.Persistence.Configurations;

public sealed class NutritionTargetConfiguration : IEntityTypeConfiguration<NutritionTarget>
{
    public void Configure(EntityTypeBuilder<NutritionTarget> builder)
    {
        builder.ToTable("nutrition_targets");

        builder.HasKey(t => t.Id);
        builder.Property(t => t.Id).ValueGeneratedOnAdd();

        builder.Property(t => t.UserId).IsRequired();
        builder.HasIndex(t => new { t.UserId, t.EffectiveFrom });

        builder.Property(t => t.EffectiveFrom).IsRequired();
        builder.Property(t => t.IsManualOverride).IsRequired();
        builder.Property(t => t.CreatedAt).IsRequired();

        builder.OwnsOne(t => t.Macros, macros =>
        {
            macros.Property(m => m.Calories).HasColumnName("calories_kcal");
            macros.Property(m => m.ProteinG).HasColumnName("protein_g");
            macros.Property(m => m.CarbsG).HasColumnName("carbs_g");
            macros.Property(m => m.FatG).HasColumnName("fat_g");
            macros.Property(m => m.FiberG).HasColumnName("fiber_g");
        });

        builder.Navigation(t => t.Macros).IsRequired();
    }
}
