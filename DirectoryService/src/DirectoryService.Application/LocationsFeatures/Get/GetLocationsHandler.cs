using System.Linq.Expressions;
using CSharpFunctionalExtensions;
using DirectoryService.Application.Database;
using DirectoryService.Contracts.Locations;
using DirectoryService.Domain.Entities.Ids;
using DirectoryService.Domain.Entities.LocationEntity;
using DirectoryService.Domain.Shared;
using Microsoft.EntityFrameworkCore;

namespace DirectoryService.Application.LocationsFeatures.Get;

public class GetLocationsHandler(IReadDbContext readDbContext)
{
    public async Task<Result<GetLocationsDto?, ErrorList>> Handle(
        GetLocationsRequest query,
        CancellationToken cancellationToken)
    {
        var locationsQuery = readDbContext.LocationsRead;

        if (query.DepartmentsIds is not null)
        {
            var departmentsIds = query.DepartmentsIds.Select(DepartmentId.Create);
            
            locationsQuery = locationsQuery
                .Where(l => l.Departments
                    .Any(dl => departmentsIds.Contains(dl.DepartmentId)));
        }

        if (string.IsNullOrWhiteSpace(query.Search) is false)
            locationsQuery = locationsQuery
                .Where(l => EF.Functions
                    .Like(l.LocationName.Value.ToLower(),$"%{query.Search.ToLower()}%"));

        if (query.IsActive.HasValue)
            locationsQuery = locationsQuery
                .Where(l => EF.Property<bool>(l, "_isActive") == query.IsActive);
        
        locationsQuery = locationsQuery
            .Skip((query.Page - 1) * query.PageSize)
            .Take(query.PageSize);

        Expression<Func<Location, object>> keySelector = query.SortBy switch
        {
            "date" => l => l.CreatedAt,
            "name" => l => l.LocationName,
            _ => l => l.CreatedAt
        };

        locationsQuery = query.SortDirection == "asc"
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