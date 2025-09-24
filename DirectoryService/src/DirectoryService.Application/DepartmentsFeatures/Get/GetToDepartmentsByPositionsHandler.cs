using CSharpFunctionalExtensions;
using DirectoryService.Application.Database;
using DirectoryService.Contracts.Departments;
using DirectoryService.Domain.Shared;
using Microsoft.EntityFrameworkCore;

namespace DirectoryService.Application.DepartmentsFeatures.Get;

public class GetToDepartmentsByPositionsHandler(
    IReadDbContext readDbContext)
{
    private const int TAKE_NUMBER_OF_DEPARTMENTS = 5;
    
    public async Task<Result<GetDepartmentsDto?, ErrorList>> Handle(CancellationToken cancellationToken)
    {
        var departmentsQuery = readDbContext.DepartmentsRead;
        
        departmentsQuery = departmentsQuery.OrderByDescending(d => d.Positions.Count);

        departmentsQuery = departmentsQuery.Take(TAKE_NUMBER_OF_DEPARTMENTS);

        var departments = await departmentsQuery
            .Select(d => new DepartmentDto(
                d.Id.Value,
                d.DepartmentName.Value,
                d.Identifier.Value,
                d.ParentId == null ? null : d.ParentId.Value,
                d.Path.Value,
                d.Depth,
                d.CreatedAt,
                d.IsActive(),
                d.Positions.Count))
            .ToListAsync(cancellationToken);

        return new GetDepartmentsDto(departments);
    }
}