namespace DirectoryService.Application.PositionsFeatures.Create;

public record CreatePositionsCommand(
    string Name,
    string Description,
    IEnumerable<Guid>  DepartmentIds);