using CSharpFunctionalExtensions;
using DirectoryService.Domain.Entities.LocationEntity;
using DirectoryService.Domain.Shared;

namespace DirectoryService.Application.Repositories;

public interface ILocationsRepository
{
    public Task<Result<Guid, Error>> Add(Location location, CancellationToken cancellationToken);
}