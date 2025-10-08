using DirectoryService.Application.Repositories;
using DirectoryService.Contracts.Departments.DTOs;
using DirectoryService.Contracts.Departments.Queries;

namespace DirectoryService.Application.DepartmentsFeatures.GetRootsWithNChildren;

public class GetRootsWithNChildrenDepartmentsHandler(IDepartmentsRepository departmentsRepository)
{
    public async Task<IReadOnlyList<DepartmentWithChildrenDto>> Handle(
        GetNChildDepartmentsQuery query,
        CancellationToken cancellationToken = default)
    {
        var departments = await departmentsRepository
            .GetRootsWithNChildren(
                query.Page, 
                query.Size,
                query.Prefetch,
                cancellationToken);

        return departments;
    }
}