using DirectoryService.Application.DepartmentsFeatures.Create;
using DirectoryService.Application.DepartmentsFeatures.UpdateLocations;
using DirectoryService.Application.DepartmentsFeatures.UpdateParent;
using DirectoryService.Application.LocationsFeatures.Create;
using DirectoryService.Application.PositionsFeatures.Create;
using FluentValidation;
using Microsoft.Extensions.DependencyInjection;

namespace DirectoryService.Application.Extensions;

public static class Inject
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        services.AddScoped<CreateLocationsHandler>();
        services.AddScoped<CreateDepartmentsHandler>();
        services.AddScoped<CreatePositionsHandler>();
        services.AddScoped<UpdateDepartmentLocationsHandler>();
        services.AddScoped<UpdateDepartmentParentHandler>();
        
        services.AddValidatorsFromAssembly(typeof(Inject).Assembly);
        
        return services;
    }
}