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
    
    public PositionId Id { get; }
    public PositionName PositionName { get; private set; }
    public string? Description { get; private set; }
    
    public IReadOnlyList<DepartmentPosition> Departments => _departments;
    
    public DateTime CreatedAt { get; } = DateTime.Now;
    public DateTime UpdatedAt { get; private set; }

    public static Result<Position, Error> Create(
        PositionId id,
        PositionName name,
        string? description)
    {
        if (description is not null && description.Length > LengthConstants.Length1000)
            return GeneralErrors.ValueIsInvalid("description");
        
        return new Position(id, name, description);
    }
}