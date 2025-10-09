namespace DirectoryService.Contracts.Departments.Requests;

public record CreateDepartmentsRequest(
    string Name,
    string Identifier,
    Guid? ParentId,
    IEnumerable<Guid> LocationIds);