using CSharpFunctionalExtensions;
using DirectoryService.Domain.Entities.Ids;
using DirectoryService.Domain.Entities.PositionEntity.ValueObjects;
using DirectoryService.Domain.Entities.Relationships;
using DirectoryService.Domain.Shared;

namespace DirectoryService.Domain.Entities.PositionEntity;

public class Position
{
    private readonly List<DepartmentPosition> _departments = [];
    
    private bool _isActive = true;

    //ef core ctor
    public Position()
    {
    }
    
    private Position(
        PositionId id, 
        PositionName positionName, 
        string? description)
    {
        Id = id;
        PositionName = positionName;
        Description = description;
    }
    
    public PositionId Id { get; } = null!;
    public PositionName PositionName { get; private set; } = null!;
    public string? Description { get; private set; }
    
    public IReadOnlyList<DepartmentPosition> Departments => _departments;
    
    public DateTime CreatedAt { get; } = DateTime.UtcNow;
    public DateTime UpdatedAt { get; private set; }
    public DateTime DeletedAt { get; private set; }

    public bool IsActive()
    {
        return _isActive;
    }

    public void AddDepartment(DepartmentPosition departmentPosition)
    {
        _departments.Add(departmentPosition);
    }

    public static Result<Position, Error> Create(
        PositionId id,
        PositionName name,
        string? description)
    {
        if (description is not null && description.Length > LengthConstants.Length1000)
            return Errors.General.ValueIsInvalid("description");
        
        return new Position(id, name, description);
    }
}