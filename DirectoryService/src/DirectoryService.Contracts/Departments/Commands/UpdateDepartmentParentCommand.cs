namespace DirectoryService.Contracts.Departments.Commands;

public record UpdateDepartmentParentCommand(
    Guid DepartmentId,
    Guid? ParentId);