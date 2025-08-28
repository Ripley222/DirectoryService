using CSharpFunctionalExtensions;
using DirectoryService.Application.Repositories;
using DirectoryService.Domain.Entities.DepartmentEntity;
using DirectoryService.Domain.Entities.DepartmentEntity.ValueObjects;
using DirectoryService.Domain.Entities.Ids;
using DirectoryService.Domain.Shared;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace DirectoryService.Infrastructure.Repositories.Departments;

public class DepartmentsRepository(
    DirectoryServiceDbContext dbContext,
    ILogger<DepartmentsRepository> logger) : IDepartmentsRepository
{
    public async Task<Result<Department, Error>> GetById(
        DepartmentId departmentId, CancellationToken cancellationToken)
    {
        try
        {
            var department = await dbContext.Departments
                .Include(d => d.ChildDepartments)
                .Include(d => d.Locations)
                .Include(d => d.Positions)
                .FirstOrDefaultAsync(d => d.Id == departmentId, cancellationToken);

            if (department is null)
                return Errors.Department.NotFound();

            return department;
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error getting Department");

            return Error.Failure("department.getting", "Failed getting Department");
        }
    }

    public async Task<Result<IReadOnlyList<Department>, Error>> GetManyById(IEnumerable<DepartmentId> departmentIds, CancellationToken cancellationToken)
    {
        try
        {
            var departments = await dbContext.Departments
                .Where(d => departmentIds.Contains(d.Id))
                .ToListAsync(cancellationToken);

            if (departments.Count == 0)
                return Errors.Department.NotFound();

            return departments;
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error getting Department");

            return Error.Failure("department.getting", "Failed getting Department");
        }
    }

    public async Task<Result<Guid, Error>> Add(
        Department department, CancellationToken cancellationToken)
    {
        try
        {
            await dbContext.Departments.AddAsync(department, cancellationToken);

            await dbContext.SaveChangesAsync(cancellationToken);

            return department.Id.Value;
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error adding Department");

            return Error.Failure("department.create", "Failed to add Department");
        }
    }

    public async Task<Result<Department, Error>> GetByIdentifier(
        Identifier identifier, CancellationToken cancellationToken)
    {
        try
        {
            var department = await dbContext.Departments
                .FirstOrDefaultAsync(d => d.Identifier == identifier, cancellationToken);

            if (department is null)
                return Errors.Department.NotFound();

            return department;
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error getting Department");

            return Error.Failure("department.getting", "Failed getting Department");
        }
    }
}