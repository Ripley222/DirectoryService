using CSharpFunctionalExtensions;
using DirectoryService.Application.DistributedCaching;
using DirectoryService.Application.Queries;
using DirectoryService.Contracts.Departments.DTOs;
using DirectoryService.Contracts.Departments.Queries;
using DirectoryService.Domain.Shared;
using Microsoft.Extensions.Caching.Distributed;

namespace DirectoryService.Application.DepartmentsFeatures.GetRootsWithNChildren;

public class GetRootsWithNChildrenDepartmentsHandler(
    IDepartmentsQueries departmentsQueries,
    ICacheService cacheService,
    ICacheOptions cacheOptions)
{
    private readonly DistributedCacheEntryOptions _entryOptions = new()
    {
        SlidingExpiration = TimeSpan.FromMinutes(cacheOptions.TimeToClearInMinutes)
    };
    
    public async Task<Result<IEnumerable<DepartmentWithChildrenDto>, ErrorList>> Handle(
        GetNChildDepartmentsQuery query,
        CancellationToken cancellationToken = default)
    {
        var filters = $"{nameof(query.Page)}={query.Page}&{nameof(query.Size)}={query.Size}&{nameof(query.Prefetch)}={query.Prefetch}";
        
        var key = CacheConstants.CACHING_DEPARTMENTS_KEY + filters;

        var departments = await cacheService.GetOrSetAsync(
            key,
            _entryOptions,
            async () => await GetDepartmentsByFilters(query, cancellationToken),
            cancellationToken);

        if (departments is null)
            return Errors.Department.NotFound().ToErrors();

        return Result.Success<IEnumerable<DepartmentWithChildrenDto>, ErrorList>(departments);
    }

    private async Task<IEnumerable<DepartmentWithChildrenDto>> GetDepartmentsByFilters(
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