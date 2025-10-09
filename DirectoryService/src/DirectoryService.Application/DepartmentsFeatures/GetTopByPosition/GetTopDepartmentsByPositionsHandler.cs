using DirectoryService.Application.Database;
using DirectoryService.Contracts.Departments.DTOs;
using Microsoft.EntityFrameworkCore;

namespace DirectoryService.Application.DepartmentsFeatures.GetTopByPosition;

public class GetTopDepartmentsByPositionsHandler(
    IReadDbContext readDbContext)
{
    private const int TAKE_NUMBER_OF_DEPARTMENTS = 5;
    
    public async Task<IReadOnlyList<DepartmentDto>> Handle(CancellationToken cancellationToken = default)
    {
        var departmentsQuery = readDbContext.DepartmentsRead;
        
        departmentsQuery = departmentsQuery.OrderByDescending(d => d.Positions.Count);

        departmentsQuery = departmentsQuery.Take(TAKE_NUMBER_OF_DEPARTMENTS);

        var departments = await departmentsQuery
            .Select(d => new DepartmentDto(
                d.Id.Value,
                d.ParentId == null ? null : d.ParentId.Value,
                d.DepartmentName.Value,
                d.Identifier.Value,
                d.Path.Value,
                d.Depth,
                d.CreatedAt,
                d.UpdatedAt,
                d.IsActive(),
                d.Positions.Count))
            .ToListAsync(cancellationToken);

        return departments;
    }
}