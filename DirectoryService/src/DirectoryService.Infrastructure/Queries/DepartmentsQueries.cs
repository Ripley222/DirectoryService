using Dapper;
using DirectoryService.Application.Queries;
using DirectoryService.Contracts.Departments.DTOs;
using DirectoryService.Domain.Entities.Ids;
using Microsoft.EntityFrameworkCore;

namespace DirectoryService.Infrastructure.Queries;

public class DepartmentsQueries(DirectoryServiceDbContext dbContext) : IDepartmentsQueries
{
    public async Task<IReadOnlyList<DepartmentWithChildrenDto>> GetRootsWithNChildrenWithPagination(
        int page,
        int size,
        int prefetch,
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

        var departmentsRaws = (
            await connection.QueryAsync<DepartmentWithChildrenDto>(
                sql,
                new
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

    public async Task<IReadOnlyList<DescendantsDepartmentDto>> GetDescendantsDepartmentsWithPagination(
        DepartmentId departmentId,
        int page,
        int size,
        CancellationToken cancellationToken)
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

        var departmentsRaws = (await connection.QueryAsync<DescendantsDepartmentDto>(
            sql,
            new
            {
                offset = (page - 1) * size,
                parent_id = departmentId.Value,
                limit = size
            })).ToList();

        return departmentsRaws;
    }
}