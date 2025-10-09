using DirectoryService.Application.Database;
using DirectoryService.Application.Queries;
using DirectoryService.Application.Repositories;
using DirectoryService.Infrastructure.Database;
using DirectoryService.Infrastructure.Queries;
using DirectoryService.Infrastructure.Repositories.Departments;
using DirectoryService.Infrastructure.Repositories.Locations;
using DirectoryService.Infrastructure.Repositories.Positions;
using Microsoft.Extensions.DependencyInjection;

namespace DirectoryService.Infrastructure.Extensions;

public static class Inject
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services)
    {
        services.AddScoped<DirectoryServiceDbContext>();
        services.AddScoped<IReadDbContext, DirectoryServiceDbContext>();
        
        services.AddScoped<ILocationsRepository, LocationsRepository>();
        services.AddScoped<IDepartmentsRepository, DepartmentsRepository>();
        services.AddScoped<IPositionsRepository, PositionsRepository>();
        
        services.AddScoped<IDepartmentsQueries, DepartmentsQueries>();
        
        services.AddScoped<ITransactionManager, TransactionManager>();
        
        return services;
    }
}