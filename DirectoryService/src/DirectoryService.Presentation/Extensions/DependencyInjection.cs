using Serilog;
using Shared.Framework.Extensions;

namespace DirectoryService.Presentation.Extensions;

public static class DependencyInjection
{
    public static IServiceCollection AddPresentation(
        this IServiceCollection services, IConfiguration configuration)
    {
        services.AddCors();
        services.AddControllers();  
        services.AddEndpointsApiExplorer();
        services.AddSwaggerGen();
        services.AddSerilog();
        services.AddDistributedCache(configuration);

        return services;
    }

    public static IApplicationBuilder AddConfiguration(this WebApplication app)
    {
        app.UseExceptionMiddleware();

        var corsOrigins = app.Configuration.GetSection("CorsURLS").Get<string[]>()
            ?? throw new ArgumentNullException(null, "Cors URLs not found.");
        
        app.UseCors(policyBuilder =>
        {
            policyBuilder.WithOrigins(corsOrigins)
                .AllowCredentials()
                .AllowAnyHeader()
                .AllowAnyMethod();
        });
        
        return app;
    }

    private static IServiceCollection AddDistributedCache(
        this IServiceCollection services, IConfiguration configuration)
    {
        services.AddStackExchangeRedisCache(options =>
        {
            string connectionString = configuration.GetConnectionString("Redis")
                                      ?? throw new ArgumentNullException(nameof(connectionString));

            options.Configuration = connectionString;
        });

        return services;
    }
}