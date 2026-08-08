using Food.Domain.Activities;
using Food.Domain.Ingredients;
using Food.Domain.Logging;
using Food.Domain.Nutrition;
using Food.Domain.Recipes;
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
    public DbSet<Recipe> Recipes => Set<Recipe>();
    public DbSet<MealLog> MealLogs => Set<MealLog>();
    public DbSet<FavoriteMeal> FavoriteMeals => Set<FavoriteMeal>();
    public DbSet<ActivityLog> ActivityLogs => Set<ActivityLog>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(FoodDbContext).Assembly);
    }
}
