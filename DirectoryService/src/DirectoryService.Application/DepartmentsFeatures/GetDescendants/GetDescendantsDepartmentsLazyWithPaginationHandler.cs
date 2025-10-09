using CSharpFunctionalExtensions;
using DirectoryService.Application.Extensions;
using DirectoryService.Application.Queries;
using DirectoryService.Contracts.Departments.DTOs;
using DirectoryService.Contracts.Departments.Queries;
using DirectoryService.Domain.Entities.Ids;
using DirectoryService.Domain.Shared;
using FluentValidation;

namespace DirectoryService.Application.DepartmentsFeatures.GetDescendants;

public class GetDescendantsDepartmentsLazyWithPaginationHandler(
    IDepartmentsQueries departmentsQueries,
    IValidator<GetDescendantDepartmentsWithPaginationQuery> validator)
{
    public async Task<Result<IReadOnlyList<DescendantsDepartmentDto>, ErrorList>> Handle(
        GetDescendantDepartmentsWithPaginationQuery query,
        CancellationToken cancellationToken = default)
    {
        var validationResult = await validator.ValidateAsync(query, cancellationToken);
        if (validationResult.IsValid is false)
            return validationResult.GetErrors();
        
        var parentId = DepartmentId.Create(query.DepartmentId);
        
        var departments = await departmentsQueries.GetDescendantsDepartmentsWithPagination(
            parentId,
            query.Page,
            query.Size,
            cancellationToken);

        return Result.Success<IReadOnlyList<DescendantsDepartmentDto>, ErrorList>(departments);
    }
}