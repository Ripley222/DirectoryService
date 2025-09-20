using DirectoryService.Application.DepartmentsFeatures.Create;
using DirectoryService.Domain.Entities.Ids;
using DirectoryService.IntegrationTests.Infrastructure;
using Microsoft.EntityFrameworkCore;

namespace DirectoryService.IntegrationTests.Departments;

public class CreateDepartmentsTests(DirectoryTestWebFactory factory) : ExecuteDepartmentsHandlers(factory)
{
    [Fact]
    public async Task CreateDepartment_With_Valid_Data_Should_Succeed()
    {
        // arrange
        var locationId = await CreateLocation();
        
        var cancellationToken = CancellationToken.None;
        
        // act
        var result = await ExecuteCreateHandler(sut=>
        {
            var command = new CreateDepartmentsCommand(
                "Department",
                "Test",
                null,
                [locationId.Value]);
            
            return sut.Handle(command, cancellationToken);
        });
        
        // assert
        await ExecuteInDb(async dbContext =>
        {
            var department = await dbContext.Departments
                .FirstAsync(d => d.Id == DepartmentId.Create(result.Value), cancellationToken);

            Assert.NotNull(department);
            Assert.Equal(result.Value, department.Id.Value);
            
            Assert.True(result.IsSuccess);
            Assert.NotEqual(Guid.Empty, result.Value);
        });
    }

    [Fact]
    public async Task CreateDepartment_With_Invalid_ParentId_Should_Fail()
    {
        // arrange
        var locationId = await CreateLocation();
        
        var cancellationToken = CancellationToken.None;

        // act
        var result = await ExecuteCreateHandler(sut =>
        {
            var command = new CreateDepartmentsCommand(
                "Main department",
                "main",
                DepartmentId.New().Value,
                [locationId.Value]);
            
            return sut.Handle(command, cancellationToken);
        });
        
        // assert
        Assert.True(result.IsFailure);
        Assert.NotEmpty(result.Error);
    }
}