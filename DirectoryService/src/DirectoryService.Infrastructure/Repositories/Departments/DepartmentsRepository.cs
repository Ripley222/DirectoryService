using CSharpFunctionalExtensions;
using Dapper;
using DirectoryService.Application.Repositories;
using DirectoryService.Domain.Entities.DepartmentEntity;
using DirectoryService.Domain.Entities.DepartmentEntity.ValueObjects;
using DirectoryService.Domain.Entities.Ids;
using DirectoryService.Domain.Shared;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Path = DirectoryService.Domain.Entities.DepartmentEntity.ValueObjects.Path;

namespace DirectoryService.Infrastructure.Repositories.Departments;

public class DepartmentsRepository(
    DirectoryServiceDbContext dbContext,
    ILogger<DepartmentsRepository> logger) : IDepartmentsRepository
{
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

    public async Task<Result<Department, Error>> GetById(
        DepartmentId departmentId, CancellationToken cancellationToken)
    {
        try
        {
            var department = await dbContext.Departments
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

    public async Task<Result<Department, Error>> GetByIdWithLocations(
        DepartmentId departmentId, CancellationToken cancellationToken)
    {
        try
        {
            var department = await dbContext.Departments
                .Include(d => d.Locations)
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

    public async Task<Result<Department, Error>> GetByIdWithLock(
        DepartmentId departmentId, CancellationToken cancellationToken)
    {
        var department = await dbContext.Departments
            .FromSql($"SELECT * FROM departments WHERE id = {departmentId.Value} FOR UPDATE")
            .FirstOrDefaultAsync(cancellationToken);

        if (department is null)
            return Errors.Department.NotFound();

        return department;
    }

    public async Task<UnitResult<Error>> CheckActiveDepartmentsByIds(
        IEnumerable<DepartmentId> departmentIds, CancellationToken cancellationToken)
    {
        var result = await dbContext.Departments
            .AnyAsync(d => EF.Property<bool>(d, "_isActive")
                           && departmentIds.Contains(d.Id), cancellationToken);

        if (result)
            return Result.Success<Error>();

        return Errors.Department.NotFound();
    }

    public async Task<UnitResult<Error>> CheckByIdentifier(
        Identifier identifier, CancellationToken cancellationToken)
    {
        var result = await dbContext.Departments
            .AnyAsync(d => d.Identifier.Value == identifier.Value, cancellationToken);

        if (result)
            return Result.Success<Error>();

        return Errors.Department.NotFound();
    }

    public async Task<UnitResult<Error>> IsDescendants(
        DepartmentId rootDepartmentId,
        DepartmentId candidateChildDepartmentId)
    {
        var result = await dbContext.Departments
            .AnyAsync(d =>
                d.Id == rootDepartmentId &&
                d.ChildDepartments.Any(cd => cd.Id == candidateChildDepartmentId));

        if (result)
            return Errors.Department.HierarchyFailure();

        return Result.Success<Error>();
    }

    public async Task<UnitResult<Error>> LockDescendants(
        Path parentPath)
    {
        var connection = dbContext.Database.GetDbConnection();

        const string sql =
            """
            SELECT *
            FROM departments
            WHERE path <@ @path::ltree
            FOR UPDATE
            """;

        await connection.ExecuteAsync(sql, new
        {
            path = parentPath.Value,
        });

        return UnitResult.Success<Error>();
    }

    public async Task<UnitResult<Error>> UpdateDescendantDepartments(
        Department department,
        Path oldPath)
    {
        var connection = dbContext.Database.GetDbConnection();

        const string sql =
            """
            UPDATE departments
            SET depth = @departmentDepth + (depth - nlevel(@oldPath::ltree) + 1),
                path = @departmentPath::ltree || subpath(path, nlevel(@oldPath::ltree))
            WHERE path <@ @oldPath::ltree
            AND path != @oldPath::ltree
            """;

        await connection.ExecuteAsync(sql, new
        {
            departmentDepth = department.Depth,
            departmentPath = department.Path.Value,
            oldPath = oldPath.Value
        });

        return UnitResult.Success<Error>();
    }

    public async Task<UnitResult<Error>> UpdateRelationships(DepartmentId departmentId)
    {
        var connection = dbContext.Database.GetDbConnection();

        const string sql =
            """
            WITH dep_loc AS (SELECT departments_locations.location_id
                             FROM departments_locations
                             WHERE department_id = @department_id),
                 active_loc AS (SELECT DISTINCT dp.location_id
                                FROM departments_locations AS dp
                                         JOIN departments d
                                              ON d.id = dp.department_id
                                                  AND d.is_active = TRUE
                                                  AND d.id != @department_id)
            UPDATE locations l
            SET is_active  = FALSE,
                deleted_at = NOW(),
                updated_at = NOW()
            WHERE l.is_active = TRUE
              AND l.id IN (SELECT location_id FROM dep_loc)
              AND l.id NOT IN (SELECT location_id FROM active_loc);
            ----------------------------------------------------------------------------
            WITH dep_pos AS (SELECT position_id
                             FROM departments_positions
                             WHERE department_id = @department_id),
                 active_pos AS (SELECT DISTINCT dp.position_id
                                FROM departments_positions AS dp
                                         JOIN departments d
                                              ON d.id = dp.department_id
                                                  AND d.is_active = TRUE
                                                  AND d.id != @department_id)
            UPDATE positions p
            SET is_active  = FALSE,
                deleted_at = NOW(),
                updated_at = NOW()
            WHERE p.is_active = TRUE
              AND p.id IN (SELECT position_id FROM dep_pos)
              AND p.id NOT IN (SELECT position_id FROM active_pos);
            """;

        await connection.ExecuteAsync(sql, new
        {
            department_id = departmentId.Value,
        });

        return UnitResult.Success<Error>();
    }
}