using Food.Domain.Ingredients;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Food.Infrastructure.Persistence.Configurations;

public sealed class IngredientConfiguration : IEntityTypeConfiguration<Ingredient>
{
    public void Configure(EntityTypeBuilder<Ingredient> builder)
    {
        builder.ToTable("ingredients");

        builder.HasKey(i => i.Id);
        builder.Property(i => i.Id).ValueGeneratedOnAdd();

        builder.Property(i => i.Name).IsRequired().HasMaxLength(200);
        builder.Property(i => i.CreatedByUserId).IsRequired();
        builder.Property(i => i.CreatedAt).IsRequired();

        builder.OwnsOne(i => i.MacrosPer100g, macros =>
        {
            macros.Property(m => m.Calories).HasColumnName("calories_per_100g");
            macros.Property(m => m.ProteinG).HasColumnName("protein_per_100g");
            macros.Property(m => m.CarbsG).HasColumnName("carbs_per_100g");
            macros.Property(m => m.FatG).HasColumnName("fat_per_100g");
            macros.Property(m => m.FiberG).HasColumnName("fiber_per_100g");
        });

        builder.Navigation(i => i.MacrosPer100g).IsRequired();

        builder.OwnsMany(i => i.Tags, tags =>
        {
            tags.ToTable("ingredient_tags");
            tags.WithOwner().HasForeignKey("IngredientId");
            tags.Property(t => t.Name).IsRequired().HasMaxLength(100).HasColumnName("name");
            tags.HasKey("IngredientId", "Name");
        });

        builder.Navigation(i => i.Tags).UsePropertyAccessMode(PropertyAccessMode.Field);
    }
}
