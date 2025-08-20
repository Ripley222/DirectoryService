namespace DirectoryService.Domain.Entities.Ids;

public record LocationId
{
    public Guid Value { get; }

    private LocationId(Guid value)
    {
        Value = value;
    }

    public static LocationId New() => new LocationId(Guid.NewGuid());

    public static LocationId Create(Guid value) => new LocationId(value);
}