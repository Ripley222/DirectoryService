using CSharpFunctionalExtensions;
using DirectoryService.Domain.Entities.LocationEntity;
using DirectoryService.Domain.Entities.LocationEntity.ValueObjects;
using DirectoryService.Domain.Shared;

namespace DirectoryService.Application.Repositories;

public interface ILocationsRepository
{
    Task<Result<Guid, Error>> Add(Location location, CancellationToken cancellationToken);
    
    Task<Result<Location, Error>> GetByName(LocationName locationName, CancellationToken cancellationToken);
    
    Task<Result<Location, Error>> GetByAddress(Address address, CancellationToken cancellationToken);
}