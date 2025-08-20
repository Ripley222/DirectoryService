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

    public static Result<DepartmentName> Create(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
            return Result.Failure<DepartmentName>("DepartmentName is required");
        
        if (value.Length < LengthConstants.Length3 || value.Length > LengthConstants.Length150)
            return Result.Failure<DepartmentName>("DepartmentName must be between 3 and 150 characters");
        
        return new DepartmentName(value);
    }
}