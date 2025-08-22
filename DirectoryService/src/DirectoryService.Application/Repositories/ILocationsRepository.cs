using CSharpFunctionalExtensions;
using DirectoryService.Domain.Entities.LocationEntity;

namespace DirectoryService.Application.Repositories;

public interface ILocationsRepository
{
    public Task<Result<Guid, string>> Add(Location location, CancellationToken cancellationToken);
}