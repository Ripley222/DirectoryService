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

    public static Result<LocationName> Create(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
            return Result.Failure<LocationName>("Location name cannot be empty");
        
        if (value.Length < LengthConstants.Length3 ||  value.Length > LengthConstants.Length120)
            return Result.Failure<LocationName>("Location name must be between 3 and 120 characters");

        return new LocationName(value);
    }
}