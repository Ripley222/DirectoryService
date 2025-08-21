using CSharpFunctionalExtensions;
using DirectoryService.Application.Repositories;
using DirectoryService.Domain.Entities.Ids;
using DirectoryService.Domain.Entities.LocationEntity;
using DirectoryService.Domain.Entities.LocationEntity.ValueObjects;
using TimeZone = DirectoryService.Domain.Entities.LocationEntity.ValueObjects.TimeZone;

namespace DirectoryService.Application.LocationFeatures.Add;

public class CreateLocationsHandler(
    ILocationsRepository repository)
{
    public async Task<Result<Guid, string>> Handle(
        CreateLocationsCommand command,
        CancellationToken cancellationToken = default)
    {
        var locationId = LocationId.New();
        var locationName = LocationName.Create(command.Name);
        var address = Address.Create(command.City, command.Street, command.House, command.RoomNumber);
        var timeZone = TimeZone.Create(command.TimeZone);
        
        var location = Location.Create(
            locationId,
            locationName.Value,
            address.Value,
            timeZone.Value);

        if (location.IsFailure)
            return location.Error;

        var result = await repository.Add(location.Value, cancellationToken);

        return result;
    }
}