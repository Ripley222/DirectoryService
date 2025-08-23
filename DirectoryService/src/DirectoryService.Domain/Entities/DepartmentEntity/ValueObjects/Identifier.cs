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

    public static Result<Identifier, Errors> Create(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
            return GeneralErrors.ValueIsRequired("Identifier").ToErrors();
        
        if (value.Length < LengthConstants.Length3 || value.Length > LengthConstants.Length150)
            return GeneralErrors.ValueIsInvalid("Identifier").ToErrors();

        if (LatinOnlyRegex.IsMatch(value) is false)
            return GeneralErrors.ValueIsInvalid("Identifier").ToErrors();

        return new Identifier(value);
    }
}