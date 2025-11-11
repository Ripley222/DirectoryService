using CSharpFunctionalExtensions;
using DirectoryService.Application.Database;
using DirectoryService.Application.DistributedCaching;
using DirectoryService.Contracts.Departments.DTOs;
using DirectoryService.Domain.Shared;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Options;

namespace DirectoryService.Application.DepartmentsFeatures.GetTopByPosition;

public class GetTopDepartmentsByPositionsHandler(
    IReadDbContext readDbContext,
    ICacheService cacheService,
    ICacheOptions cacheOptions)
{
    private readonly DistributedCacheEntryOptions _entryOptions = new()
    {
        SlidingExpiration = TimeSpan.FromMinutes(cacheOptions.TimeToClearInMinutes)
    };

    private const int TAKE_NUMBER_OF_DEPARTMENTS = 5;

    public async Task<Result<IEnumerable<DepartmentDto>, ErrorList>> Handle(
        CancellationToken cancellationToken = default)
    {
        var filters = $"filters={nameof(TAKE_NUMBER_OF_DEPARTMENTS)}={TAKE_NUMBER_OF_DEPARTMENTS}&OrderBy=Desc";

        var key = CacheConstants.CACHING_DEPARTMENTS_KEY + filters;

        var departments = await cacheService.GetOrSetAsync(
            key,
            _entryOptions,
            async () => await GetDepartmentsByFilters(cancellationToken),
            cancellationToken);

        if (departments is null)
            return Errors.Department.NotFound().ToErrors();

        return Result.Success<IEnumerable<DepartmentDto>, ErrorList>(departments);
    }

    private async Task<IEnumerable<DepartmentDto>> GetDepartmentsByFilters(
        CancellationToken cancellationToken = default)
    {
        var departmentsQuery = readDbContext.DepartmentsRead
            .OrderByDescending(d => d.Positions.Count)
            .Take(TAKE_NUMBER_OF_DEPARTMENTS);

        return await departmentsQuery
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
    }
}