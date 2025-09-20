using DirectoryService.Application.DepartmentsFeatures.Create;
using DirectoryService.Application.DepartmentsFeatures.UpdateLocations;
using DirectoryService.Application.DepartmentsFeatures.UpdateParent;
using DirectoryService.Domain.Entities.Ids;
using DirectoryService.Domain.Entities.LocationEntity;
using DirectoryService.Domain.Entities.LocationEntity.ValueObjects;
using DirectoryService.IntegrationTests.Infrastructure;
using Microsoft.Extensions.DependencyInjection;
using TimeZone = DirectoryService.Domain.Entities.LocationEntity.ValueObjects.TimeZone;

namespace DirectoryService.IntegrationTests.Departments;

public class ExecuteDepartmentsHandlers(DirectoryTestWebFactory factory) : DirectoryBaseTests(factory)
{
    protected async Task<T> ExecuteCreateHandler<T>(Func<CreateDepartmentsHandler, Task<T>> action)
    {
        await using var scope = Services.CreateAsyncScope();
        
        var sut = scope.ServiceProvider.GetRequiredService<CreateDepartmentsHandler>();
        
        return await action(sut);
    }
    
    protected async Task<T> ExecuteUpdateHandler<T>(Func<UpdateDepartmentLocationsHandler, Task<T>> action)
    {
        await using var scope = Services.CreateAsyncScope();
        
        var sut = scope.ServiceProvider.GetRequiredService<UpdateDepartmentLocationsHandler>();
        
        return await action(sut);
    }

    protected async Task<T> ExecuteUpdateParentHandler<T>(Func<UpdateDepartmentParentHandler, Task<T>> action)
    {
        await using var scope = Services.CreateAsyncScope();
        
        var sut = scope.ServiceProvider.GetRequiredService<UpdateDepartmentParentHandler>();
        
        return await action(sut);
    }
    
    protected async Task<LocationId> CreateLocation()
    {
        return await ExecuteInDb(async dbContext =>
        {
            var location = new Location(
                LocationId.New(),
                LocationName.Create("Location").Value,
                Address.Create("City", "Street", "House", "RoomNumber").Value,
                TimeZone.Create("Europe/Moscow").Value);

            dbContext.Locations.Add(location);

            await dbContext.SaveChangesAsync();

            return location.Id;
        });
    }
}