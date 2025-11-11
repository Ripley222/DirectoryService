using Serilog;

namespace DirectoryService.Presentation.Extensions;

public static class Inject
{
    public static IServiceCollection AddPresentation(
        this IServiceCollection services, IConfiguration configuration)
    {
        services.AddControllers();  
        services.AddEndpointsApiExplorer();
        services.AddSwaggerGen();
        services.AddSerilog();
        services.AddDistributedCache(configuration);

        return services;
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