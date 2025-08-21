using DirectoryService.Application.Repositories;
using DirectoryService.Infrastructure.Repositories;
using Microsoft.Extensions.DependencyInjection;

namespace DirectoryService.Infrastructure.Extensions;

public static class Inject
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services)
    {
        services.AddScoped<DirectoryServiceDbContext>();
        services.AddScoped<ILocationsRepository, LocationsRepository>();
        
        return services;
    }
}