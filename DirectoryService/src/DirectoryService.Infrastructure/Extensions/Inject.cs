using DirectoryService.Application.Repositories;
using DirectoryService.Infrastructure.Database;
using DirectoryService.Infrastructure.Repositories;
using Microsoft.Extensions.DependencyInjection;

namespace DirectoryService.Infrastructure.Extensions;

public static class Inject
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services)
    {
        //services.AddScoped<DirectoryServiceDbContext>();
        services.AddSingleton<NpgsqlConnectionFactory>();
        //services.AddScoped<ILocationsRepository, LocationsRepository>();
        services.AddSingleton<ILocationsRepository, LocationsRepositoryDapper>();
        
        return services;
    }
}