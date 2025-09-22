using DirectoryService.Application.Validation;
using DirectoryService.Contracts.Locations;
using DirectoryService.Domain.Entities.LocationEntity.ValueObjects;
using FluentValidation;
using TimeZone = DirectoryService.Domain.Entities.LocationEntity.ValueObjects.TimeZone;

namespace DirectoryService.Application.LocationsFeatures.Create;

public class CreateLocationCommandValidator : AbstractValidator<CreateLocationsRequest>
{
    public CreateLocationCommandValidator()
    {
        RuleFor(c => c.Name)
            .MustBeValueObject(LocationName.Create);
        
        RuleFor(c => new { c.City, c.Street, c.House, c.RoomNumber })
            .MustBeValueObject(f => Address.Create(f.City, f.Street, f.House, f.RoomNumber));

        RuleFor(c => c.TimeZone)
            .MustBeValueObject(TimeZone.Create);
    }
}