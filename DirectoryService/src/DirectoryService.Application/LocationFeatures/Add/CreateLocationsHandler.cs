using CSharpFunctionalExtensions;
using DirectoryService.Application.Repositories;
using DirectoryService.Domain.Entities.Ids;
using DirectoryService.Domain.Entities.LocationEntity;
using DirectoryService.Domain.Entities.LocationEntity.ValueObjects;
using DirectoryService.Domain.Shared;
using TimeZone = DirectoryService.Domain.Entities.LocationEntity.ValueObjects.TimeZone;

namespace DirectoryService.Application.LocationFeatures.Add;

public class CreateLocationsHandler(
    ILocationsRepository repository)
{
    public async Task<Result<Guid, Errors>> Handle(
        CreateLocationsCommand command,
        CancellationToken cancellationToken = default)
    {
        var locationId = LocationId.New();
        var locationName = LocationName.Create(command.Name);
        var address = Address.Create(command.City, command.Street, command.House, command.RoomNumber);
        var timeZone = TimeZone.Create(command.TimeZone);
        
        var location = new Location(
            locationId,
            locationName.Value,
            address.Value,
            timeZone.Value);

        var result = await repository.Add(location, cancellationToken);
        if (result.IsFailure)
            return result.Error;

        return result.Value;
    }
}