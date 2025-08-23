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

    public static Result<DepartmentName, Errors> Create(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
            return GeneralErrors.ValueIsRequired("DepartmentName").ToErrors();
        
        if (value.Length < LengthConstants.Length3 || value.Length > LengthConstants.Length150)
            return GeneralErrors.ValueIsInvalid("DepartmentName").ToErrors();
        
        return new DepartmentName(value);
    }
}