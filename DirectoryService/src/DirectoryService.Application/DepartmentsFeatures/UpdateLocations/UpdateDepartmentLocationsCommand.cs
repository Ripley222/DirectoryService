namespace DirectoryService.Application.DepartmentsFeatures.UpdateLocations;

public record UpdateDepartmentLocationsCommand(
    Guid DepartmentId,
    IEnumerable<Guid> LocationIds);