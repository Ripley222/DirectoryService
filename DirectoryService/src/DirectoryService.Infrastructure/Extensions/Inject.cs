using DirectoryService.Application.Database;
using DirectoryService.Application.DistributedCaching;
using DirectoryService.Application.Queries;
using DirectoryService.Application.Repositories;
using DirectoryService.Infrastructure.BackgroundServices;
using DirectoryService.Infrastructure.Database;
using DirectoryService.Infrastructure.DistributedCaching;
using DirectoryService.Infrastructure.Options;
using DirectoryService.Infrastructure.Queries;
using DirectoryService.Infrastructure.Repositories.Departments;
using DirectoryService.Infrastructure.Repositories.Locations;
using DirectoryService.Infrastructure.Repositories.Positions;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Shared.Core.Abstractions.Caching;
using Shared.Core.Abstractions.Database;

namespace DirectoryService.Infrastructure.Extensions;

public static class Inject
{
    public static IServiceCollection AddInfrastructure(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        services.AddScoped<DirectoryServiceDbContext>();
        services.AddScoped<IReadDbContext, DirectoryServiceDbContext>();
        
        services.AddScoped<ILocationsRepository, LocationsRepository>();
        services.AddScoped<IDepartmentsRepository, DepartmentsRepository>();
        services.AddScoped<IPositionsRepository, PositionsRepository>();
        
        services.AddScoped<IDepartmentsQueries, DepartmentsQueries>();
        
        services.AddScoped<ITransactionManager, TransactionManager>();
        
        services.AddScoped<ICacheService, CacheService>();
        services.AddSingleton<ICacheOptions, CacheOptionsAdapter>();

        services.AddHostedService<DepartmentsCleanerBackgroundService>();

        services.Configure<DepartmentsCleanerOptions>(
            configuration.GetSection(DepartmentsCleanerOptions.SectionName));
        
        services.Configure<CacheOptions>(
            configuration.GetSection(CacheOptions.SECTION_NAME));
        
        return services;
    }
}