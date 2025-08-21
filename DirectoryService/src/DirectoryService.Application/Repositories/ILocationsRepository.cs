using DirectoryService.Domain.Entities.LocationEntity;

namespace DirectoryService.Application.Repositories;

public interface ILocationsRepository
{
    public Task<Guid> Add(Location location, CancellationToken cancellationToken);
}