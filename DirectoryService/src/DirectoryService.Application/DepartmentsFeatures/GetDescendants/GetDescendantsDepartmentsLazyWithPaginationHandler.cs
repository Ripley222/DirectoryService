using CSharpFunctionalExtensions;
using DirectoryService.Application.DistributedCaching;
using DirectoryService.Application.Queries;
using DirectoryService.Contracts.Departments.DTOs;
using DirectoryService.Contracts.Departments.Queries;
using DirectoryService.Domain.Entities.Ids;
using FluentValidation;
using Microsoft.Extensions.Caching.Distributed;
using Shared.Core.Abstractions.Caching;
using Shared.Core.Validation;
using Shared.SharedKernel.Errors;

namespace DirectoryService.Application.DepartmentsFeatures.GetDescendants;

public class GetDescendantsDepartmentsLazyWithPaginationHandler(
    IDepartmentsQueries departmentsQueries,
    ICacheService cacheService,
    ICacheOptions cacheOptions,
    IValidator<GetDescendantDepartmentsWithPaginationQuery> validator)
{
    private readonly DistributedCacheEntryOptions _entryOptions = new()
    {
        SlidingExpiration = TimeSpan.FromMinutes(cacheOptions.TimeToClearInMinutes)
    };
    
    public async Task<Result<IEnumerable<DescendantsDepartmentDto>, ErrorList>> Handle(
        GetDescendantDepartmentsWithPaginationQuery query,
        CancellationToken cancellationToken)
    {
        var validationResult = await validator.ValidateAsync(query, cancellationToken);
        if (validationResult.IsValid is false)
            return validationResult.GetErrors();

        var filters = $"filters=departmentId={query.DepartmentId}&page={query.Page}&size={query.Size}";

        var key = CacheConstants.CACHING_DEPARTMENTS_KEY + filters;
        
        var departments = await cacheService.GetOrSetAsync(
            key,
            _entryOptions,
            async () => await GetDepartmentsByFilters(query, cancellationToken),
            cancellationToken);

        return Result.Success<IEnumerable<DescendantsDepartmentDto>, ErrorList>(departments);
    }

    private async Task<IEnumerable<DescendantsDepartmentDto>> GetDepartmentsByFilters(
        GetDescendantDepartmentsWithPaginationQuery query,
        CancellationToken cancellationToken = default)
    {
        var parentId = DepartmentId.Create(query.DepartmentId);
        
        var departments = await departmentsQueries.GetDescendantsDepartmentsWithPagination(
            parentId,
            query.Page,
            query.Size,
            cancellationToken);
        
        return departments;
    }
}