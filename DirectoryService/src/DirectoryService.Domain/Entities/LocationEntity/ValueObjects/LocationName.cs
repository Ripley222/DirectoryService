using CSharpFunctionalExtensions;
using DirectoryService.Domain.Shared;

namespace DirectoryService.Domain.Entities.LocationEntity.ValueObjects;

public record LocationName
{
    public string Value { get; }

    private LocationName(string value)
    {
        Value = value;
    }

    public static Result<LocationName, Error> Create(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
            return GeneralErrors.ValueIsRequired("LocationName");

        if (value.Length < LengthConstants.Length3 || value.Length > LengthConstants.Length120)
            return GeneralErrors.ValueIsInvalid("LocationName");

        return new LocationName(value);
    }
}