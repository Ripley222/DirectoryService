using CSharpFunctionalExtensions;
using DirectoryService.Domain.Entities.Ids;
using DirectoryService.Domain.Entities.LocationEntity;
using DirectoryService.Domain.Entities.LocationEntity.ValueObjects;
using DirectoryService.Domain.Shared;

namespace DirectoryService.Application.Repositories;

public interface ILocationsRepository
{
    Task<UnitResult<Error>> CheckManyByIds(
        IEnumerable<LocationId> locationIds, CancellationToken cancellationToken);

    Task<UnitResult<Error>> CheckActiveLocationsByIds(
        IEnumerable<LocationId> locationIds, CancellationToken cancellationToken);
    
    Task<Result<Guid, Error>> Add(Location location, CancellationToken cancellationToken);
    
    Task<UnitResult<Error>> CheckByName(LocationName locationName, CancellationToken cancellationToken);
    
    Task<UnitResult<Error>> CheckByAddress(Address address, CancellationToken cancellationToken);
}