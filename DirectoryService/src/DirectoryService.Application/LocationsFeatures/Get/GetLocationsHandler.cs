using System.Linq.Expressions;
using CSharpFunctionalExtensions;
using DirectoryService.Application.Database;
using DirectoryService.Contracts.Locations.Commands;
using DirectoryService.Contracts.Locations.DTOs;
using DirectoryService.Contracts.Locations.Queries;
using DirectoryService.Domain.Entities.Ids;
using DirectoryService.Domain.Entities.LocationEntity;
using DirectoryService.Domain.Shared;
using Microsoft.EntityFrameworkCore;

namespace DirectoryService.Application.LocationsFeatures.Get;

public class GetLocationsHandler(IReadDbContext readDbContext)
{
    public async Task<Result<GetLocationsDto?, ErrorList>> Handle(
        GetLocationsQuery query,
        CancellationToken cancellationToken)
    {
        var locationsQuery = readDbContext.LocationsRead;

        if (query.Request.DepartmentsIds is not null)
        {
            var departmentsIds = query.Request.DepartmentsIds.Select(DepartmentId.Create);
            
            locationsQuery = locationsQuery
                .Where(l => l.Departments
                    .Any(dl => departmentsIds.Contains(dl.DepartmentId)));
        }

        if (string.IsNullOrWhiteSpace(query.Request.Search) is false)
            locationsQuery = locationsQuery
                .Where(l => EF.Functions
                    .Like(l.LocationName.Value.ToLower(),$"%{query.Request.Search.ToLower()}%"));

        if (query.Request.IsActive.HasValue)
            locationsQuery = locationsQuery
                .Where(l => EF.Property<bool>(l, "_isActive") == query.Request.IsActive);
        
        locationsQuery = locationsQuery
            .Skip((query.Request.Page - 1) * query.Request.PageSize)
            .Take(query.Request.PageSize);

        Expression<Func<Location, object>> keySelector = query.Request.SortBy switch
        {
            "date" => l => l.CreatedAt,
            "name" => l => l.LocationName,
            _ => l => l.CreatedAt
        };

        locationsQuery = query.Request.SortDirection == "asc"
            ? locationsQuery.OrderBy(keySelector)
            : locationsQuery.OrderByDescending(keySelector);

        var locations = await locationsQuery
            .Select(l => new LocationDto(
                l.Id.Value,
                l.LocationName.Value,
                $"{l.Address.City} {l.Address.Street} {l.Address.House} {l.Address.RoomNumber}",
                l.TimeZone.Value,
                l.CreatedAt))
            .ToListAsync(cancellationToken);

        return new GetLocationsDto(locations);
    }
}