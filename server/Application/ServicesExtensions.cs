using Application.Interfaces;
using Application.Services;
using Microsoft.Extensions.DependencyInjection;


namespace Application;

public static class ServicesExtensions
{
    public static IServiceCollection RegisterApplicationServices(this IServiceCollection services)
    {
        // here you can register your application services, for example:
        // services.AddScoped<IMyService, MyService>();

        services.AddScoped<IFindingService, FindingService>();
        
        return services;
    }
}