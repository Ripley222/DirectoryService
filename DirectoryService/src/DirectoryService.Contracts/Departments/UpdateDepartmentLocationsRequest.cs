namespace DirectoryService.Contracts.Departments;

public record UpdateDepartmentLocationsRequest(
    Guid DepartmentId,
    IEnumerable<Guid> LocationIds);
