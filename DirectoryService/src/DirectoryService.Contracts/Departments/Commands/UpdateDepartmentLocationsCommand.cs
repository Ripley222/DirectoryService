namespace DirectoryService.Contracts.Departments.Commands;

public record UpdateDepartmentLocationsCommand(
    Guid DepartmentId,
    IEnumerable<Guid> LocationIds);
