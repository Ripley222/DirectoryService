using System.Text.RegularExpressions;
using CSharpFunctionalExtensions;
using DirectoryService.Domain.Shared;
using Shared.SharedKernel.Errors;
using Errors = Shared.SharedKernel.Errors.Errors;

namespace DirectoryService.Domain.Entities.DepartmentEntity.ValueObjects;

public record Identifier
{
    private static readonly Regex LatinOnlyRegex = new("^[A-Za-z]+$", RegexOptions.Compiled);
    
    public string Value { get; }

    private Identifier(string value)
    {
        Value = value;
    }

    public static Result<Identifier, Error> Create(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
            return Errors.General.ValueIsRequired("Identifier");
        
        if (value.Length < LengthConstants.Length3 || value.Length > LengthConstants.Length150)
            return Errors.General.ValueIsInvalid("Identifier");

        if (LatinOnlyRegex.IsMatch(value) is false)
            return Errors.General.ValueIsInvalid("Identifier");

        return new Identifier(value);
    }
}