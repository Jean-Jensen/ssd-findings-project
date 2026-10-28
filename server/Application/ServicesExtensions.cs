using Microsoft.Extensions.DependencyInjection;

namespace Application;

public static class ServicesExtensions
{
    public static IServiceCollection RegisterApplicationServices(this IServiceCollection services)
    {
        // here you can register your application services, for example:
        // services.AddScoped<IMyService, MyService>();
        
        return services;
    }
}