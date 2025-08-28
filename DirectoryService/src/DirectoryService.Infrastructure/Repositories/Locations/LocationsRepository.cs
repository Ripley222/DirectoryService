using CSharpFunctionalExtensions;
using DirectoryService.Application.Repositories;
using DirectoryService.Domain.Entities.Ids;
using DirectoryService.Domain.Entities.LocationEntity;
using DirectoryService.Domain.Entities.LocationEntity.ValueObjects;
using DirectoryService.Domain.Shared;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace DirectoryService.Infrastructure.Repositories.Locations;

public class LocationsRepository(
    DirectoryServiceDbContext dbContext,
    ILogger<LocationsRepository> logger) : ILocationsRepository
{
    public async Task<Result<IReadOnlyList<Location>, Error>> GetManyByIds(
        IEnumerable<LocationId> locationIds, CancellationToken cancellationToken)
    {
        try
        {
            var result = await dbContext.Location
                .Where(l => locationIds.Contains(l.Id))
                .ToListAsync(cancellationToken);

            if (result.Count != locationIds.Count())
                return Errors.Location.NotFound();

            return result;
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error getting Location");

            return Error.Failure("location.get", "Failed getting Location");
        }
    }

    public async Task<Result<Guid, Error>> Add(
        Location location, CancellationToken cancellationToken)
    {
        try
        {
            await dbContext.AddAsync(location, cancellationToken);

            await dbContext.SaveChangesAsync(cancellationToken);

            return location.Id.Value;
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error adding Location");

            return Error.Failure("location.create", "Failed to add Location");
        }
    }

    public async Task<Result<Location, Error>> GetByName(
        LocationName locationName, CancellationToken cancellationToken)
    {
        try
        {
            var location = await dbContext.Location
                .Include(l => l.Departments)
                .FirstOrDefaultAsync(l => l.LocationName == locationName, cancellationToken);

            if (location is null)
                return Errors.Location.NotFound();

            return location;
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error getting Location by Name");

            return Error.Failure("location.get", "Failed getting Location");
        }
    }

    public async Task<Result<Location, Error>> GetByAddress(
        Address address, CancellationToken cancellationToken)
    {
        try
        {
            var location = await dbContext.Location
                .Include(l => l.Departments)
                .FirstOrDefaultAsync(l => l.Address == address, cancellationToken);

            if (location is null)
                return Errors.Location.NotFound();

            return location;
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error getting Location by Address");

            return Error.Failure("location.get", "Failed getting Location");
        }
    }
}