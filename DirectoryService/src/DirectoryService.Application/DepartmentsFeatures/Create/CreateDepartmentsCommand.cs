namespace DirectoryService.Application.DepartmentsFeatures.Create;

public record CreateDepartmentsCommand(
    string Name,
    string Identifier,
    Guid? ParentId,
    IEnumerable<Guid> LocationIds);