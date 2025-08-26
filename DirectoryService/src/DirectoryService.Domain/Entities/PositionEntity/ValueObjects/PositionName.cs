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

    public static Result<PositionName, Error> Create(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
            return GeneralErrors.ValueIsRequired("PositionName");
        
        if (value.Length < LengthConstants.Length3 ||  value.Length > LengthConstants.Length100)
            return GeneralErrors.ValueIsInvalid("LocationName");

        return new PositionName(value);
    }
}