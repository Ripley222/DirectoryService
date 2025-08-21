using DirectoryService.Application.Repositories;
using DirectoryService.Domain.Entities.LocationEntity;

namespace DirectoryService.Infrastructure.Repositories;

public class LocationsRepository(DirectoryServiceDbContext dbContext) : ILocationsRepository
{
    public async Task<Guid> Add(Location location, CancellationToken cancellationToken)
    {
        await dbContext.AddAsync(location, cancellationToken);
        
        await dbContext.SaveChangesAsync(cancellationToken);
        
        return location.Id.Value;
    }
}