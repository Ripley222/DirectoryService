using System.Text.RegularExpressions;
using CSharpFunctionalExtensions;
using DirectoryService.Domain.Shared;

namespace DirectoryService.Domain.Entities.DepartmentEntity.ValueObjects;

public record Identifier
{
    private static readonly Regex LatinOnlyRegex = new("^[A-Za-z]+$", RegexOptions.Compiled);
    
    public string Value { get; }

    private Identifier(string value)
    {
        Value = value;
    }

    public static Result<Identifier> Create(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
            return Result.Failure<Identifier>("Identifier is required");
        
        if (value.Length < LengthConstants.Length3 || value.Length > LengthConstants.Length150)
            return Result.Failure<Identifier>("Identifier must be between 3 and 150 characters");

        if (LatinOnlyRegex.IsMatch(value) is false)
            return Result.Failure<Identifier>("Identifier must be only latin");

        return new Identifier(value);
    }
}