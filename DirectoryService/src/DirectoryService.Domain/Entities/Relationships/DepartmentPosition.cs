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
        PositionId positionId,
        Department department,
        Position position)
    {
        DepartmentId = departmentId;
        PositionId = positionId;
        Department = department;
        Position = position;
    }
    
    public DepartmentId DepartmentId { get; private set; }
    public PositionId PositionId { get; private set; }
    public Department Department { get; private set; }
    public Position Position { get; private set; }
}