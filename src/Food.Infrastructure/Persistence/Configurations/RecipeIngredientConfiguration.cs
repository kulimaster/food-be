using Food.Domain.Recipes;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Food.Infrastructure.Persistence.Configurations;

public sealed class RecipeIngredientConfiguration : IEntityTypeConfiguration<RecipeIngredient>
{
    public void Configure(EntityTypeBuilder<RecipeIngredient> builder)
    {
        builder.ToTable("recipe_ingredients");

        builder.HasKey(ri => ri.Id);
        builder.Property(ri => ri.Id).ValueGeneratedOnAdd();

        builder.Property(ri => ri.IngredientId).IsRequired();

        builder.HasOne(ri => ri.Ingredient)
            .WithMany()
            .HasForeignKey(ri => ri.IngredientId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.OwnsOne(ri => ri.Quantity, quantity =>
        {
            quantity.Property(q => q.Grams).HasColumnName("quantity_g");
        });

        builder.Navigation(ri => ri.Quantity).IsRequired();
    }
}
