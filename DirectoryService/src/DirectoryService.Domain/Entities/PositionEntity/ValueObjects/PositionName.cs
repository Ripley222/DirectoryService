using CSharpFunctionalExtensions;
using DirectoryService.Domain.Shared;

namespace DirectoryService.Domain.Entities.PositionEntity.ValueObjects;

public class PositionName
{
    public string Value { get; }

    private PositionName(string value)
    {
        Value = value;
    }

    public static Result<PositionName> Create(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
            return Result.Failure<PositionName>("PositionName must not be empty");
        
        if (value.Length < LengthConstants.Length3 ||  value.Length > LengthConstants.Length100)
            return Result.Failure<PositionName>("PositionName must be between 3 and 100 characters");

        return new PositionName(value);
    }
}