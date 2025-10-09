using DirectoryService.Application.DepartmentsFeatures.Create;
using DirectoryService.Application.DepartmentsFeatures.UpdateLocations;
using DirectoryService.Contracts.Departments;
using DirectoryService.Contracts.Departments.Commands;
using DirectoryService.Contracts.Departments.Requests;
using DirectoryService.Domain.Entities.Ids;
using DirectoryService.IntegrationTests.Infrastructure;
using Microsoft.EntityFrameworkCore;

namespace DirectoryService.IntegrationTests.Departments;

public class UpdateDepartmentsLocationsTests(DirectoryTestWebFactory factory) : ExecuteDepartmentsHandlers(factory)
{
    [Fact]
    public async Task UpdateDepartment_With_Valid_Location_Should_Succeed()
    {
        // arrange
        var originalLocationId = await CreateLocation();
        var newLocationId = await CreateLocation();

        var cancellationToken = CancellationToken.None;

        var createResult = await ExecuteCreateHandler(sut =>
        {
            var request = new CreateDepartmentsRequest(
                "Department",
                "Test",
                null,
                [originalLocationId.Value]);

            return sut.Handle(new CreateDepartmentsCommand(request), cancellationToken);
        });

        // act
        var updateResult = await ExecuteUpdateHandler(sut =>
        {
            var command = new UpdateDepartmentLocationsCommand(
                createResult.Value,
                [newLocationId.Value]);
            
            return sut.Handle(command, cancellationToken);
        });

        // assert
        await ExecuteInDb(async dbContext =>
        {
            var department = await dbContext.Departments
                .Include(d => d.Locations)
                .FirstAsync(d => d.Id == DepartmentId.Create(createResult.Value), cancellationToken);

            Assert.True(updateResult.IsSuccess);
            Assert.Contains(department.Locations, dl => dl.LocationId == newLocationId);
            Assert.DoesNotContain(department.Locations, dl => dl.LocationId == originalLocationId);
        });
    }
    
    [Fact]
    public async Task UpdateDepartment_With_Invalid_Location_Should_Fail()
    {
        // arrange
        var originalLocationId = await CreateLocation();
        var newLocationId = LocationId.New();

        var cancellationToken = CancellationToken.None;

        var createResult = await ExecuteCreateHandler(sut =>
        {
            var request = new CreateDepartmentsRequest(
                "Department",
                "Test",
                null,
                [originalLocationId.Value]);

            return sut.Handle(new CreateDepartmentsCommand(request), cancellationToken);
        });

        // act
        var updateResult = await ExecuteUpdateHandler(sut =>
        {
            var command = new UpdateDepartmentLocationsCommand(
                createResult.Value,
                [newLocationId.Value]);
            
            return sut.Handle(command, cancellationToken);
        });

        // assert
        await ExecuteInDb(async dbContext =>
        {
            var department = await dbContext.Departments
                .Include(d => d.Locations)
                .FirstAsync(d => d.Id == DepartmentId.Create(createResult.Value), cancellationToken);

            Assert.True(updateResult.IsFailure);
            Assert.Contains(department.Locations, dl => dl.LocationId == originalLocationId);
            Assert.DoesNotContain(department.Locations, dl => dl.LocationId == newLocationId);
        });
    }
}