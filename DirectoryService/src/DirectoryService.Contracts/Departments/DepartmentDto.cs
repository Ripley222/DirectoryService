namespace DirectoryService.Contracts.Departments;

public record DepartmentDto(
    Guid DepartmentId,
    string Name,
    string Identifier,
    Guid? ParentId,
    string Path,
    short Depth,
    DateTime CreatedAt,
    bool IsActive,
    int PositionCount);