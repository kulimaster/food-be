using Food.Domain.Ingredients;
using Microsoft.EntityFrameworkCore;

namespace Food.Infrastructure.Persistence;

public sealed class FoodDbContext : DbContext
{
    public FoodDbContext(DbContextOptions<FoodDbContext> options) : base(options)
    {
    }

    public DbSet<Ingredient> Ingredients => Set<Ingredient>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(FoodDbContext).Assembly);
    }
}
