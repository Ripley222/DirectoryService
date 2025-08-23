using CSharpFunctionalExtensions;
using DirectoryService.Application.Repositories;
using DirectoryService.Domain.Entities.LocationEntity;
using DirectoryService.Domain.Shared;

namespace DirectoryService.Infrastructure.Repositories;

public class LocationsRepository(DirectoryServiceDbContext dbContext) : ILocationsRepository
{
    public async Task<Result<Guid, Errors>> Add(Location location, CancellationToken cancellationToken)
    {
        try
        {
            await dbContext.AddAsync(location, cancellationToken);

            await dbContext.SaveChangesAsync(cancellationToken);

            return location.Id.Value;
        }
        catch (Exception ex)
        {
            return Error.Failure("location.create", "Failed to add location").ToErrors();
        }
    }
}