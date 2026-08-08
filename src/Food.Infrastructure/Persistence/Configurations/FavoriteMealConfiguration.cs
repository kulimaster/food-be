using Food.Domain.Logging;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Food.Infrastructure.Persistence.Configurations;

public sealed class FavoriteMealConfiguration : IEntityTypeConfiguration<FavoriteMeal>
{
    public void Configure(EntityTypeBuilder<FavoriteMeal> builder)
    {
        builder.ToTable("favorite_meals", t => t.HasCheckConstraint(
            "CK_favorite_meals_exactly_one_source",
            "(\"RecipeId\" IS NOT NULL) <> (\"IngredientId\" IS NOT NULL)"));

        builder.HasKey(f => f.Id);
        builder.Property(f => f.Id).ValueGeneratedOnAdd();

        builder.Property(f => f.UserId).IsRequired();
        builder.Property(f => f.DisplayName).IsRequired().HasMaxLength(200);
        builder.Property(f => f.CreatedAt).IsRequired();

        builder.HasIndex(f => f.UserId);

        builder.OwnsOne(f => f.Item, item =>
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

        builder.Navigation(f => f.Item).IsRequired();
    }
}
