using DirectoryService.Application.DepartmentsFeatures.UpdateLocations;

namespace DirectoryService.Presentation.Requests.Departments;

public record UpdateDepartmentLocationsRequest(
    IEnumerable<Guid> LocationIds)
{
    public UpdateDepartmentLocationsCommand ToCommand(Guid departmentId) =>
        new (departmentId, LocationIds);
}
