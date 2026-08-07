using Food.Domain.Ingredients;
using Food.Domain.Nutrition;
using Food.Domain.Users;
using Microsoft.EntityFrameworkCore;

namespace Food.Infrastructure.Persistence;

public sealed class FoodDbContext : DbContext
{
    public FoodDbContext(DbContextOptions<FoodDbContext> options) : base(options)
    {
    }

    public DbSet<Ingredient> Ingredients => Set<Ingredient>();
    public DbSet<User> Users => Set<User>();
    public DbSet<UserProfile> UserProfiles => Set<UserProfile>();
    public DbSet<NutritionTarget> NutritionTargets => Set<NutritionTarget>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(FoodDbContext).Assembly);
    }
}
