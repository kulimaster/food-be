using Food.Domain.Logging;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Food.Infrastructure.Persistence.Configurations;

public sealed class MealLogConfiguration : IEntityTypeConfiguration<MealLog>
{
    public void Configure(EntityTypeBuilder<MealLog> builder)
    {
        builder.ToTable("meal_logs", t => t.HasCheckConstraint(
            "CK_meal_logs_exactly_one_source",
            "(\"RecipeId\" IS NOT NULL) <> (\"IngredientId\" IS NOT NULL)"));

        builder.HasKey(m => m.Id);
        builder.Property(m => m.Id).ValueGeneratedOnAdd();

        builder.Property(m => m.UserId).IsRequired();
        builder.Property(m => m.LogDate).IsRequired();
        builder.Property(m => m.MealSlot).IsRequired().HasConversion<string>().HasMaxLength(20);
        builder.Property(m => m.LoggedAt).IsRequired();

        builder.HasIndex(m => new { m.UserId, m.LogDate });

        builder.OwnsOne(m => m.Item, item =>
        {
            item.HasOne(i => i.Recipe)
                .WithMany()
                .HasForeignKey("RecipeId")
                .OnDelete(DeleteBehavior.Restrict);
            item.Property<long?>("RecipeId").HasColumnName("RecipeId");

            item.Property(i => i.ServingsCount).HasColumnName("ServingsCount");

            item.HasOne(i => i.Ingredient)
                .WithMany()
                .HasForeignKey("IngredientId")
                .OnDelete(DeleteBehavior.Restrict);
            item.Property<long?>("IngredientId").HasColumnName("IngredientId");

            item.OwnsOne(i => i.Quantity, quantity =>
            {
                quantity.Property(q => q.Grams).HasColumnName("quantity_g");
            });

            item.Navigation(i => i.Quantity).IsRequired(false);
        });

        builder.Navigation(m => m.Item).IsRequired();
    }
}
