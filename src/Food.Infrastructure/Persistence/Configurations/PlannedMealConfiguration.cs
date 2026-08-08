using Food.Domain.Planning;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Food.Infrastructure.Persistence.Configurations;

public sealed class PlannedMealConfiguration : IEntityTypeConfiguration<PlannedMeal>
{
    public void Configure(EntityTypeBuilder<PlannedMeal> builder)
    {
        builder.ToTable("planned_meals", t => t.HasCheckConstraint(
            "CK_planned_meals_exactly_one_source",
            "(\"RecipeId\" IS NOT NULL) <> (\"IngredientId\" IS NOT NULL)"));

        builder.HasKey(p => p.Id);
        builder.Property(p => p.Id).ValueGeneratedOnAdd();

        builder.Property(p => p.UserId).IsRequired();
        builder.Property(p => p.PlanDate).IsRequired();
        builder.Property(p => p.MealSlot).IsRequired().HasConversion<string>().HasMaxLength(20);
        builder.Property(p => p.CreatedAt).IsRequired();

        builder.HasIndex(p => new { p.UserId, p.PlanDate });

        builder.OwnsOne(p => p.Item, item =>
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

        builder.Navigation(p => p.Item).IsRequired();
    }
}
