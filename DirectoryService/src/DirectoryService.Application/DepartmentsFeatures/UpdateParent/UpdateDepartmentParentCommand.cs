namespace DirectoryService.Application.DepartmentsFeatures.UpdateParent;

public record UpdateDepartmentParentCommand(
    Guid DepartmentId,
    Guid? ParentId);