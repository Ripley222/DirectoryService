using DirectoryService.Domain.Entities.DepartmentEntity;
using DirectoryService.Domain.Entities.Ids;
using DirectoryService.Domain.Entities.PositionEntity;

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
    
    public DepartmentId DepartmentId { get; private set; }
    public PositionId PositionId { get; private set; }
}