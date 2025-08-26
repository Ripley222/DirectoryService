using CSharpFunctionalExtensions;
using DirectoryService.Domain.Shared;

namespace DirectoryService.Domain.Entities.DepartmentEntity.ValueObjects;

public record DepartmentName
{
    public string Value { get; }

    private DepartmentName(string value)
    {
        Value = value;
    }

    public static Result<DepartmentName, Error> Create(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
            return GeneralErrors.ValueIsRequired("DepartmentName");
        
        if (value.Length < LengthConstants.Length3 || value.Length > LengthConstants.Length150)
            return GeneralErrors.ValueIsInvalid("DepartmentName");
        
        return new DepartmentName(value);
    }
}