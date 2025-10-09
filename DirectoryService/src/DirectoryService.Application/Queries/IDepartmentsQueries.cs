using DirectoryService.Contracts.Departments.DTOs;
using DirectoryService.Domain.Entities.Ids;

namespace DirectoryService.Application.Queries;

public interface IDepartmentsQueries
{
    Task<IReadOnlyList<DepartmentWithChildrenDto>> GetRootsWithNChildrenWithPagination(
        int page, int size, int prefetch, CancellationToken cancellationToken);
    
    Task<IReadOnlyList<DescendantsDepartmentDto>> GetDescendantsDepartmentsWithPagination(
        DepartmentId departmentId, int page, int size, CancellationToken cancellationToken);
}