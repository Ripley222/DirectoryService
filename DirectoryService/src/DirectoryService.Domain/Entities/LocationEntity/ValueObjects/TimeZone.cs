using System.Text.RegularExpressions;
using CSharpFunctionalExtensions;

namespace DirectoryService.Domain.Entities.LocationEntity.ValueObjects;

public record TimeZone
{
    private static readonly Regex TimeZoneRegex = new(
        "^(Africa|America|Antarctica|Arctic|Asia|Atlantic|Australia|Europe|Indian|Pacific|Etc)(?:/[A-Za-z0-9._+-]+)+$");
    
    public string Value { get; }

    private TimeZone(string value)
    {
        Value = value;
    }

    public static Result<TimeZone> Create(string value)
    {
        if (TimeZoneRegex.IsMatch(value) is false)
            return Result.Failure<TimeZone>("Invalid timezone format");
        
        return new TimeZone(value);
    }
}