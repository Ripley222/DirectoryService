using System.Text.RegularExpressions;
using CSharpFunctionalExtensions;

namespace DirectoryService.Domain.Entities.DepartmentEntity.ValueObjects;

public record Path
{
    private static readonly Regex DenormalizedPath = new("^[a-zA-Z][a-zA-Z0-9-]*(.[a-zA-Z][a-zA-Z0-9-]*)*$");
    
    public string Value { get; }

    private Path(string value)
    {
        Value = value;
    }

    public static Result<Path> Create(string value)
    {
        if (DenormalizedPath.IsMatch(value) is false)
            return Result.Failure<Path>("Invalid path");

        return new Path(value);
    }
}