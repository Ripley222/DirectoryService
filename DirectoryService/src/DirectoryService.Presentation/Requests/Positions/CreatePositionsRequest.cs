using DirectoryService.Application.PositionsFeatures.Create;

namespace DirectoryService.Presentation.Requests.Positions;

public record CreatePositionsRequest(
    string Name,
    string Description,
    IEnumerable<Guid> DepartmentIds)
{
    public CreatePositionsCommand ToCommand() =>
        new (Name, Description, DepartmentIds);
}