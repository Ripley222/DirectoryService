﻿namespace DirectoryService.Domain.Entities.Relationships;

public class DepartmentPosition
{
    public DepartmentPosition(Guid departmentId, Guid positionId)
    {
        DepartmentId = departmentId;
        PositionId = positionId;
    }
    
    public Guid DepartmentId { get; private set; }
    public Guid PositionId { get; private set; }
}