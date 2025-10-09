using DirectoryService.Application.Queries;
using DirectoryService.Contracts.Departments.DTOs;
using DirectoryService.Contracts.Departments.Queries;

namespace DirectoryService.Application.DepartmentsFeatures.GetRootsWithNChildren;

public class GetRootsWithNChildrenDepartmentsHandler(IDepartmentsQueries departmentsQueries)
{
    public async Task<IReadOnlyList<DepartmentWithChildrenDto>> Handle(
        GetNChildDepartmentsQuery query,
        CancellationToken cancellationToken = default)
    {
        var departments = await departmentsQueries
            .GetRootsWithNChildrenWithPagination(
                query.Page, 
                query.Size,
                query.Prefetch,
                cancellationToken);

        return departments;
    }
}