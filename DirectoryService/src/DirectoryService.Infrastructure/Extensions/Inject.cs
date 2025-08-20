using Microsoft.Extensions.DependencyInjection;

namespace DirectoryService.Infrastructure.Extensions;

public static class Inject
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services)
    {
        services.AddScoped<DirectoryServiceDbContext>();
        
        return services;
    }
}