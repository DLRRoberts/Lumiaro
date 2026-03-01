using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using LumiaroAdmin.Data.Repositories;
using RedZone.LumiaroAdmin.Data;

namespace LumiaroAdmin.Data;

/// <summary>
/// Extension methods for registering all data layer services in the DI container.
/// Call AddLumiaroData() from Program.cs in the web project.
/// </summary>
public static class ServiceCollectionExtensions
{
    /// <summary>
    /// Registers the LumiaroDbContext with SQL Server and all repository implementations.
    /// </summary>
    /// <param name="services">The service collection.</param>
    /// <param name="connectionString">SQL Server connection string.</param>
    /// <returns>The service collection for chaining.</returns>
    public static IServiceCollection AddLumiaroData(this IServiceCollection services, string connectionString)
    {
        services.AddDbContext<LumiaroDbContext>(options =>
        {
            options.UseSqlServer(connectionString, sqlOptions =>
            {
                sqlOptions.MigrationsAssembly(typeof(LumiaroDbContext).Assembly.FullName);
                sqlOptions.EnableRetryOnFailure(
                    maxRetryCount: 3,
                    maxRetryDelay: TimeSpan.FromSeconds(10),
                    errorNumbersToAdd: null);
                sqlOptions.CommandTimeout(60);
            });
        },  ServiceLifetime.Transient);
        
        services.AddScoped<Func<LumiaroDbContext>>(provider => provider.GetService<LumiaroDbContext>);

        // Register all repositories
        services.AddScoped<IRefereeRepository, RefereeRepository>();
        services.AddScoped<IDivisionRepository, DivisionRepository>();
        services.AddScoped<IFixtureRepository, FixtureRepository>();
        services.AddScoped<IOfficialAssignmentRepository, OfficialAssignmentRepository>();
        services.AddScoped<IMatchRepository, MatchRepository>();
        services.AddScoped<IAssessmentRepository, AssessmentRepository>();
        services.AddScoped<IMatchOfficialReportRepository, MatchOfficialReportRepository>();
        services.AddScoped<IInterventionReviewRepository, InterventionReviewRepository>();
        services.AddScoped<IActivityLogRepository, ActivityLogRepository>();

        return services;
    }

    /// <summary>
    /// Ensures the database is created and all migrations are applied.
    /// Call during application startup for development environments.
    /// </summary>
    public static async Task EnsureLumiaroDatabaseAsync(this IServiceProvider serviceProvider)
    {
        using var scope = serviceProvider.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<LumiaroDbContext>();
        await db.Database.MigrateAsync();
    }

    /// <summary>
    /// Ensures the database exists (creates if missing) without running migrations.
    /// Useful for development when you want code-first table creation.
    /// </summary>
    public static async Task EnsureLumiaroDatabaseCreatedAsync(this IServiceProvider serviceProvider)
    {
        using var scope = serviceProvider.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<LumiaroDbContext>();
        await db.Database.EnsureCreatedAsync();
    }
}
