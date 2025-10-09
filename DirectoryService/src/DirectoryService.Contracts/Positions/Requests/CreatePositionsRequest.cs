namespace DirectoryService.Contracts.Positions.Requests;

public record CreatePositionsRequest(
    string Name,
    string Description,
    IEnumerable<Guid> DepartmentIds);