using CSharpFunctionalExtensions;
using Dapper;
using DirectoryService.Application.Repositories;
using DirectoryService.Contracts.Departments.DTOs;
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
        Path path)
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
            path = path.Value,
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

    public async Task<List<DepartmentWithChildrenDto>> GetRootsWithNChildren(
        int page, int size, int prefetch, 
        CancellationToken cancellationToken)
    {
        var connection = dbContext.Database.GetDbConnection();

        const string sql =
            """
            WITH roots AS (SELECT d.id,
                                  d.parent_id AS ParentId,
                                  d.depth,
                                  d.created_at AS CreatedAt,
                                  d.updated_at AS UpdatedAt,
                                  d.is_active AS IsActive,
                                  d.name,
                                  d.identifier,
                                  d.path
                           FROM departments AS d
                           WHERE parent_id is null
                             AND is_active = true
                           ORDER BY created_at
                           OFFSET @offset LIMIT @root_limit)
            SELECT *,
                   (EXISTS(SELECT 1 FROM departments WHERE parent_id = roots.id OFFSET @child_limit LIMIT 1)) AS HasMoreChildren
            FROM roots

            UNION ALL

            SELECT c.*,
                   (EXISTS(SELECT 1 FROM departments WHERE parent_id = c.id)) AS HasMoreChildren
            FROM roots r
                     CROSS JOIN LATERAL (SELECT d.id,
                                                d.parent_id AS ParentId,
                                                d.depth,
                                                d.created_at AS CreatedAt,
                                                d.updated_at AS UpdatedAt,
                                                d.is_active AS IsActive,
                                                d.name,
                                                d.identifier,
                                                d.path
                                         FROM departments d
                                         WHERE d.parent_id = r.id
                                           AND d.is_active = true
                                         ORDER BY created_at
                                         LIMIT @child_limit) c
            """;

        var departmentsRaws = (await connection.QueryAsync<DepartmentWithChildrenDto>(sql, new
        {
            offset = (page - 1) * size,
            root_limit = size,
            child_limit = prefetch
        })).ToList();

        var departmentsDictionary = departmentsRaws.ToDictionary(d => d.Id);
        
        var departmentsRoots = new List<DepartmentWithChildrenDto>();
        
        foreach (var row in departmentsRaws)
        {
            if (row.ParentId.HasValue && departmentsDictionary.TryGetValue(row.ParentId.Value, out var parent))
            {
                parent.Children.Add(departmentsDictionary[row.Id]);
            }
            else
            {
                departmentsRoots.Add(departmentsDictionary[row.Id]);
            }
        }

        return departmentsRoots;
    }

    public async Task<List<DescendantsDepartmentDto>> GetDescendantsDepartments(
        DepartmentId departmentId,
        int page,
        int size,
        CancellationToken cancellationToken = default)
    {
        var connection = dbContext.Database.GetDbConnection();

        const string sql =
            """
            SELECT d.id,
                   d.parent_id AS ParentId,
                   d.depth,
                   d.created_at AS CreatedAt,
                   d.updated_at AS UpdatedAt,
                   d.is_active AS IsActive,
                   d.name,
                   d.identifier,
                   d.path,
                   (EXISTS(SELECT 1 FROM departments WHERE parent_id = d.id OFFSET @offset LIMIT 1)) AS HasMoreChildren
            FROM departments d
            WHERE parent_id = @parent_id
            OFFSET @offset LIMIT @limit
            """;

        var departmentsRaws = (await connection.QueryAsync<DescendantsDepartmentDto>(sql, new
        {
            offset = (page - 1) * size,
            parent_id = departmentId.Value,
            limit = size
        })).ToList();
        
        return departmentsRaws;
    }
}