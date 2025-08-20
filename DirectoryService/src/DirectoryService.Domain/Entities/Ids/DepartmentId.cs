namespace DirectoryService.Domain.Entities.Ids;

public record DepartmentId
{
    public Guid Value { get; }

    private DepartmentId(Guid value)
    {
        Value = value;
    }

    public static DepartmentId New() => new DepartmentId(Guid.NewGuid());

    public static DepartmentId Create(Guid value) => new DepartmentId(value);
}