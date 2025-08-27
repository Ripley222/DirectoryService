using CSharpFunctionalExtensions;
using DirectoryService.Application.Extensions;
using DirectoryService.Application.Repositories;
using DirectoryService.Domain.Entities.Ids;
using DirectoryService.Domain.Entities.LocationEntity;
using DirectoryService.Domain.Entities.LocationEntity.ValueObjects;
using DirectoryService.Domain.Shared;
using FluentValidation;
using Microsoft.Extensions.Logging;
using TimeZone = DirectoryService.Domain.Entities.LocationEntity.ValueObjects.TimeZone;

namespace DirectoryService.Application.LocationFeatures.Create;

public class CreateLocationsHandler(
    ILocationsRepository repository,
    IValidator<CreateLocationsCommand> validator,
    ILogger<CreateLocationsHandler> logger)
{
    public async Task<Result<Guid, ErrorList>> Handle(
        CreateLocationsCommand command,
        CancellationToken cancellationToken = default)
    {
        var validationResult = await validator.ValidateAsync(command, cancellationToken);
        if (validationResult.IsValid is false)
            return validationResult.GetErrors();
        
        var locationId = LocationId.New();
        var locationName = LocationName.Create(command.Name).Value;
        var address = Address.Create(command.City, command.Street, command.House, command.RoomNumber).Value;
        var timeZone = TimeZone.Create(command.TimeZone).Value;
        
        var locationTakesByNameResult = await repository.GetByName(locationName, cancellationToken);
        if (locationTakesByNameResult.IsSuccess)
            return Errors.Location.AlreadyExist().ToErrors();
        
        var locationTakesByAddressResult = await repository.GetByAddress(address, cancellationToken);
        if (locationTakesByAddressResult.IsSuccess)
            return Errors.Location.AlreadyExist().ToErrors();
        
        var location = new Location(
            locationId,
            locationName,
            address,
            timeZone);
        
        var result = await repository.Add(location, cancellationToken);
        if (result.IsFailure)
            return result.Error.ToErrors();

        logger.LogInformation("Created Location with id {id}", locationId.Value);
        
        return result.Value;
    }
}