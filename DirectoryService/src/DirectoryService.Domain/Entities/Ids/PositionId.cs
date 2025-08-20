namespace DirectoryService.Domain.Entities.Ids;

public record PositionId
{
    public Guid Value { get; }

    private PositionId(Guid value)
    {
        Value = value;
    }

    public static PositionId New() => new PositionId(Guid.NewGuid());

    public static PositionId Create(Guid value) => new PositionId(value);
}