using DirectoryService.Domain.Entities.Ids;

namespace DirectoryService.Domain.Entities.Relationships;

public class DepartmentPosition
{
    //ef core ctor
    private DepartmentPosition()
    {
    }
    
    public DepartmentPosition(
        DepartmentId departmentId, 
        PositionId positionId)
    {
        DepartmentId = departmentId;
        PositionId = positionId;
    }
    
    public DepartmentId DepartmentId { get; private set; } = null!;
    public PositionId PositionId { get; private set; } = null!;
}