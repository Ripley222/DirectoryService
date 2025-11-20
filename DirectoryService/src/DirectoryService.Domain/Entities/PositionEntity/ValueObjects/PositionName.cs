using CSharpFunctionalExtensions;
using DirectoryService.Domain.Shared;
using Shared.SharedKernel.Errors;
using Errors = Shared.SharedKernel.Errors.Errors;

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
            return Errors.General.ValueIsRequired("PositionName");
        
        if (value.Length < LengthConstants.Length3 ||  value.Length > LengthConstants.Length100)
            return Errors.General.ValueIsInvalid("LocationName");

        return new PositionName(value);
    }
}