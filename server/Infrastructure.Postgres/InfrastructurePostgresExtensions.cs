using Application;
using Infrastructure.Postgres.Scaffolding;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

namespace Infrastructure.PostGres;

public static class InfrastructurePostgresExtensions
{
    public static IServiceCollection AddDataSourceAndRepositories(this IServiceCollection services)
    {
        services.AddDbContext<AppDbContext>((service, options) =>
        {
            var provider = services.BuildServiceProvider();
            options.UseNpgsql(
                provider.GetRequiredService<IOptionsMonitor<AppOptions>>().CurrentValue.DbConnectionString);
            options.EnableSensitiveDataLogging();
        });

        services.AddScoped<Seeder>();

        return services;
    }
}