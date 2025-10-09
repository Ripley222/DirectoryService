namespace DirectoryService.Contracts.Departments.DTOs;

public record DepartmentDto(
    Guid DepartmentId,
    Guid? ParentId,
    string Name,
    string Identifier,
    string Path,
    short Depth,
    DateTime CreatedAt,
    DateTime UpdatedAt,
    bool IsActive,
    int PositionCount);