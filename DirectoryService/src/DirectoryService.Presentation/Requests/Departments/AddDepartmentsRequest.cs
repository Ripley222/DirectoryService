using DirectoryService.Application.DepartmentsFeatures.Create;

namespace DirectoryService.Presentation.Requests.Departments;

public record AddDepartmentsRequest(
    string Name,
    string Identifier,
    Guid? ParentId,
    IEnumerable<Guid> LocationIds)
{
    public CreateDepartmentsCommand ToCommand() =>
        new (Name, Identifier, ParentId, LocationIds);
}