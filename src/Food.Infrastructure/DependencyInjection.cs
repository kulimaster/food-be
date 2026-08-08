using Food.Application.Abstractions;
using Food.Application.Ingredients;
using Food.Application.Logging;
using Food.Application.Recipes;
using Food.Application.Users;
using Food.Infrastructure.Persistence;
using Food.Infrastructure.Persistence.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Food.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        var connectionString = configuration.GetConnectionString("FoodDb")
            ?? throw new InvalidOperationException("Connection string 'FoodDb' is not configured.");

        services.AddDbContext<FoodDbContext>(options => options.UseNpgsql(connectionString));

        services.AddScoped<IIngredientRepository, IngredientRepository>();
        services.AddScoped<IRecipeRepository, RecipeRepository>();
        services.AddScoped<IMealLogRepository, MealLogRepository>();
        services.AddScoped<IUserRepository, UserRepository>();
        services.AddScoped<IUserProfileRepository, UserProfileRepository>();
        services.AddScoped<INutritionTargetRepository, NutritionTargetRepository>();
        services.AddScoped<IUnitOfWork, UnitOfWork>();
        services.AddSingleton<IClock, SystemClock>();

        return services;
    }
}
