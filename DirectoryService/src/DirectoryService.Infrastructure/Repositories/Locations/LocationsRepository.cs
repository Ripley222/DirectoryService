using CSharpFunctionalExtensions;
using DirectoryService.Application.Repositories;
using DirectoryService.Domain.Entities.Ids;
using DirectoryService.Domain.Entities.LocationEntity;
using DirectoryService.Domain.Entities.LocationEntity.ValueObjects;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Shared.SharedKernel.Errors;

namespace DirectoryService.Infrastructure.Repositories.Locations;

public class LocationsRepository(
    DirectoryServiceDbContext dbContext,
    ILogger<LocationsRepository> logger) : ILocationsRepository
{
    public async Task<UnitResult<Error>> CheckActiveLocationsByIds(
        IEnumerable<LocationId> locationIds, CancellationToken cancellationToken)
    {
        var result = await dbContext.Locations
            .AnyAsync(l => locationIds.Contains(l.Id) && EF.Property<bool>(l, "_isActive"), 
                cancellationToken);

        if (result)
            return Result.Success<Error>();

        return Errors.Location.NotFound();
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

    public async Task<UnitResult<Error>> CheckByName(
        LocationName locationName, CancellationToken cancellationToken)
    {
        var result = await dbContext.Locations
            .AnyAsync(l => l.LocationName ==  locationName, cancellationToken);

        if (result)
            return Result.Success<Error>();
        
        return Errors.Location.NotFound();
    }

    public async Task<UnitResult<Error>> CheckByAddress(
        Address address, CancellationToken cancellationToken)
    {
        var result = await dbContext.Locations
            .AnyAsync(l => l.Address ==  address, cancellationToken);

        if (result)
            return Result.Success<Error>();
        
        return Errors.Location.NotFound();
    }
}