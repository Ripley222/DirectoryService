using CSharpFunctionalExtensions;

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
        
        if (value.Length < 3 ||  value.Length > 100)
            return Result.Failure<PositionName>("PositionName must be between 3 and 100 characters");

        return new PositionName(value);
    }
}