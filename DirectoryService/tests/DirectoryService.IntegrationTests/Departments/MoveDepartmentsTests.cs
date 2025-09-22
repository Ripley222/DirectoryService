using DirectoryService.Application.DepartmentsFeatures.Create;
using DirectoryService.Application.DepartmentsFeatures.UpdateParent;
using DirectoryService.Contracts.Departments;
using DirectoryService.Domain.Entities.Ids;
using DirectoryService.IntegrationTests.Infrastructure;
using Microsoft.EntityFrameworkCore;

namespace DirectoryService.IntegrationTests.Departments;

public class MoveDepartmentsTests(DirectoryTestWebFactory factory) : ExecuteDepartmentsHandlers(factory)
{
    [Fact]
    public async Task MoveDepartment_With_Valid_Data_Should_Succeed()
    {
        // arrange
        var locationId = await CreateLocation();
        
        var cancellationToken = CancellationToken.None;

        var rootDepartment = await ExecuteCreateHandler(sut =>
        {
            var command = new CreateDepartmentsRequest(
                "Main department",
                "main",
                null,
                [locationId.Value]);

            return sut.Handle(command, cancellationToken);
        });
        
        var childDepartmentResult = await ExecuteCreateHandler(sut =>
        {
            var command = new CreateDepartmentsRequest(
                "Child department",
                "child",
                rootDepartment.Value,
                [locationId.Value]);

            return sut.Handle(command, cancellationToken);
        });
        
        var newRootDepartmentResult = await ExecuteCreateHandler(sut =>
        {
            var command = new CreateDepartmentsRequest(
                "Dev main department",
                "dev",
                null,
                [locationId.Value]);

            return sut.Handle(command, cancellationToken);
        });

        // act
        var updatedResult = await ExecuteUpdateParentHandler(sut =>
        {
            var command = new UpdateDepartmentParentRequest(
                childDepartmentResult.Value,
                newRootDepartmentResult.Value);

            return sut.Handle(command, cancellationToken);
        });

        // assert
        await ExecuteInDb(async dbContext =>
        {
            var department = await dbContext.Departments
                .FirstAsync(d => d.Id == DepartmentId.Create(childDepartmentResult.Value), cancellationToken);

            short depth = 1;
            var path = "dev.child";

            Assert.True(updatedResult.IsSuccess);
            
            Assert.NotNull(department.ParentId);
            
            Assert.Equal(department.ParentId.Value, newRootDepartmentResult.Value);
            Assert.Equal(depth, department.Depth);
            Assert.Equal(path, department.Path.Value);
        });
    }
    
    [Fact]
    public async Task MoveDepartment_With_Many_Hierarchy_With_Valid_Data_Should_Succeed()
    {
        // arrange
        var locationId = await CreateLocation();
        
        var cancellationToken = CancellationToken.None;

        var rootDepartmentResult = await ExecuteCreateHandler(sut =>
        {
            var command = new CreateDepartmentsRequest(
                "dev department",
                "dev",
                null,
                [locationId.Value]);

            return sut.Handle(command, cancellationToken);
        });
        
        var firstSubChildrenDepartmentResult = await ExecuteCreateHandler(sut =>
        {
            var command = new CreateDepartmentsRequest(
                "First sub children department",
                "first",
                rootDepartmentResult.Value,
                [locationId.Value]);

            return sut.Handle(command, cancellationToken);
        });
        
        var secondSubChildrenDepartmentResult = await ExecuteCreateHandler(sut =>
        {
            var command = new CreateDepartmentsRequest(
                "Second sub children department",
                "second",
                rootDepartmentResult.Value,
                [locationId.Value]);

            return sut.Handle(command, cancellationToken);
        });
        
        var newRootDepartmentResult = await ExecuteCreateHandler(sut =>
        {
            var command = new CreateDepartmentsRequest(
                "Main department",
                "main",
                null,
                [locationId.Value]);

            return sut.Handle(command, cancellationToken);
        });
        
        // act
        var updatedResult = await ExecuteUpdateParentHandler(sut =>
        {
            var command = new UpdateDepartmentParentRequest(
                rootDepartmentResult.Value,
                newRootDepartmentResult.Value);

            return sut.Handle(command, cancellationToken);
        });
        
        // assert
        await ExecuteInDb(async dbContext =>
        {
            var department = await dbContext.Departments
                .Include(d => d.ChildDepartments)
                .FirstAsync(d => d.Id == DepartmentId.Create(rootDepartmentResult.Value), cancellationToken);

            short newDepth = 1;
            short newDepthSubChildren = 2;
            
            var newPath = "main.dev";
            var newPathFirstSubChildren = "main.dev.first";
            var newPathSecondSubChildren = "main.dev.second";
            
            var countSubChildDepartment = 2;
            
            Assert.True(updatedResult.IsSuccess);
            
            Assert.NotNull(department.ParentId);
            
            Assert.Equal(department.ParentId.Value, newRootDepartmentResult.Value);
            Assert.Equal(newPath, department.Path.Value);
            Assert.Equal(newDepth, department.Depth);
            Assert.Equal(countSubChildDepartment, department.ChildDepartments.Count);
            Assert.Equal(newDepthSubChildren, department.ChildDepartments[0].Depth);
            Assert.Equal(newDepthSubChildren, department.ChildDepartments[1].Depth);
            Assert.Equal(department.ChildDepartments[0].Path.Value, newPathFirstSubChildren);
            Assert.Equal(department.ChildDepartments[1].Path.Value, newPathSecondSubChildren);
        });
    }
    
    [Fact]
    public async Task MoveDepartment_To_Its_Child_Should_Fail()
    {
        // arrange
        var locationId = await CreateLocation();
        
        var cancellationToken = CancellationToken.None;

        var rootDepartmentResult = await ExecuteCreateHandler(sut =>
        {
            var command = new CreateDepartmentsRequest(
                "Main department",
                "main",
                null,
                [locationId.Value]);

            return sut.Handle(command, cancellationToken);
        });
        
        var childDepartmentResult = await ExecuteCreateHandler(sut =>
        {
            var command = new CreateDepartmentsRequest(
                "Child department",
                "child",
                rootDepartmentResult.Value,
                [locationId.Value]);

            return sut.Handle(command, cancellationToken);
        });
        
        var subChildDepartmentResult = await ExecuteCreateHandler(sut =>
        {
            var command = new CreateDepartmentsRequest(
                "Subchild department",
                "subchild",
                childDepartmentResult.Value,
                [locationId.Value]);

            return sut.Handle(command, cancellationToken);
        });

        // act
        var updatedResult = await ExecuteUpdateParentHandler(sut =>
        {
            var command = new UpdateDepartmentParentRequest(
                childDepartmentResult.Value,
                subChildDepartmentResult.Value);

            return sut.Handle(command, cancellationToken);
        });

        // assert
        await ExecuteInDb(async dbContext =>
        {
            var department = await dbContext.Departments
                .FirstAsync(d => d.Id == DepartmentId.Create(childDepartmentResult.Value), cancellationToken);

            short depth = 1;
            var path = "main.child";
            
            short expectedDepth = 2;
            var expectedPath = "main.subchild.child";

            Assert.True(updatedResult.IsFailure);
            
            Assert.NotNull(department.ParentId);
            
            Assert.Equal(depth, department.Depth);
            Assert.Equal(path, department.Path.Value);
            
            Assert.NotEqual(department.ParentId.Value, subChildDepartmentResult.Value);
            Assert.NotEqual(department.Depth, expectedDepth);
            Assert.NotEqual(department.Path.Value, expectedPath);
        });
    }
}