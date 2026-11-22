using Api.Rest.Middleware;
using Api.Rest.Services;
using Application.Interfaces;

namespace Api.Rest;

public static class RestStartupExtensions
{
    public static IServiceCollection RegisterRestApiServices(this IServiceCollection services)
    {
        services.AddEndpointsApiExplorer();
        services.AddExceptionHandler<GlobalExceptionHandler>();
        services.AddProblemDetails();

        services.AddCors();
        
        services.AddHttpContextAccessor();
        services.AddScoped<ICurrentUser, CurrentUser>();

        var controllersAssembly = typeof(RestStartupExtensions).Assembly;

        services.AddControllers().AddApplicationPart(controllersAssembly);
        return services;
    }

    public static WebApplication ConfigureRestApi(this WebApplication app)
    {
        app.UseExceptionHandler();
        app.MapControllers();
        app.UseCors(opts => opts
            .WithOrigins("http://localhost:5173")
            .AllowAnyHeader()
            .AllowAnyMethod()
            .AllowCredentials());
        return app;
    }
}