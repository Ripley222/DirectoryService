namespace DirectoryService.Contracts.Positions;

public record CreatePositionsRequest(
    string Name,
    string Description,
    IEnumerable<Guid> DepartmentIds);