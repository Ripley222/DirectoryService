namespace DirectoryService.Contracts.Departments;

public record CreateDepartmentsRequest(
    string Name,
    string Identifier,
    Guid? ParentId,
    IEnumerable<Guid> LocationIds);