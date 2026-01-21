using CSharpFunctionalExtensions;
using DirectoryService.Application.Repositories;
using DirectoryService.Contracts.Locations.Commands;
using DirectoryService.Domain.Entities.Ids;
using DirectoryService.Domain.Entities.LocationEntity;
using DirectoryService.Domain.Entities.LocationEntity.ValueObjects;
using FluentValidation;
using Microsoft.Extensions.Logging;
using Shared.Core.Validation;
using Shared.SharedKernel.Errors;
using Errors = DirectoryService.Domain.Shared.Errors;
using TimeZone = DirectoryService.Domain.Entities.LocationEntity.ValueObjects.TimeZone;

namespace DirectoryService.Application.LocationsFeatures.Create;

public class CreateLocationsHandler(
    ILocationsRepository repository,
    IValidator<CreateLocationsCommand> validator,
    ILogger<CreateLocationsHandler> logger)
{
    public async Task<Result<Guid, ErrorList>> Handle(
        CreateLocationsCommand command,
        CancellationToken cancellationToken)
    {
        //валидация входных параметров
        var validationResult = await validator.ValidateAsync(command, cancellationToken);
        if (validationResult.IsValid is false)
            return validationResult.GetErrors();
        
        var locationId = LocationId.New();
        var locationName = LocationName.Create(command.Request.Name).Value;
        var timeZone = TimeZone.Create(command.Request.TimeZone).Value;
        var address = Address.Create(
            command.Request.City, command.Request.Street, command.Request.House, command.Request.RoomNumber).Value;
        
        //бизнес валидация
        //проверка на существование локации с таким же названием
        var locationTakesByNameResult = await repository.CheckByName(locationName, cancellationToken);
        if (locationTakesByNameResult.IsSuccess)
            return Errors.Location.AlreadyExist("Name").ToErrors();
        
        //проверка на существование локации с таким же адресом
        var locationTakesByAddressResult = await repository.CheckByAddress(address, cancellationToken);
        if (locationTakesByAddressResult.IsSuccess)
            return Errors.Location.AlreadyExist("Address").ToErrors();
        
        var location = new Location(
            locationId,
            locationName,
            address,
            timeZone);
        
        var result = await repository.Add(location, cancellationToken);
        if (result.IsFailure)
            return result.Error.ToErrors();

        logger.LogInformation("Created Location with id {id}", locationId.Value);
        
        return locationId.Value;
    }
}