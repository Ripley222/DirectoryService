using CSharpFunctionalExtensions;
using DirectoryService.Domain.Entities.PositionEntity.ValueObjects;
using DirectoryService.Domain.Entities.Relationships;

namespace DirectoryService.Domain.Entities.PositionEntity;

public class Position
{
    private readonly List<DepartmentPosition> _departments = [];
    
    private bool _isActive = true;
    
    private Position(
        Guid id, 
        PositionName name, 
        string? description)
    {
        Id = id;
        Name = name;
        Description = description;
    }
    
    public Guid Id { get; }
    public PositionName Name { get; private set; }
    public string? Description { get; private set; }
    
    public IReadOnlyList<DepartmentPosition> Departments => _departments;
    
    public DateTime CreatedAt { get; } = DateTime.Now;
    public DateTime UpdatedAt { get; private set; }

    public static Result<Position> Create(
        Guid id,
        PositionName name,
        string? description)
    {
        if (description is not null && description.Length > 1000)
            return Result.Failure<Position>("Description must be 1000 characters");
        
        return new Position(id, name, description);
    }
}