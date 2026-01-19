using DirectoryService.Contracts.Locations.Commands;
using DirectoryService.Domain.Entities.LocationEntity.ValueObjects;
using FluentValidation;
using Shared.Core.Validation;
using TimeZone = DirectoryService.Domain.Entities.LocationEntity.ValueObjects.TimeZone;

namespace DirectoryService.Application.LocationsFeatures.Create;

public class CreateLocationCommandValidation : AbstractValidator<CreateLocationsCommand>
{
    public CreateLocationCommandValidation()
    {
        RuleFor(c => c.Request.Name)
            .MustBeValueObject(LocationName.Create);
        
        RuleFor(c => new { c.Request.City, c.Request.Street, c.Request.House, c.Request.RoomNumber })
            .MustBeValueObject(f => Address.Create(f.City, f.Street, f.House, f.RoomNumber));

        RuleFor(c => c.Request.TimeZone)
            .MustBeValueObject(TimeZone.Create);
    }
}