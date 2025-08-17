using CSharpFunctionalExtensions;
using DirectoryService.Domain.Entities.DepartmentEntity.ValueObjects;
using DirectoryService.Domain.Entities.LocationEntity;
using DirectoryService.Domain.Entities.PositionEntity;
using DirectoryService.Domain.Entities.Relationships;
using Path = DirectoryService.Domain.Entities.DepartmentEntity.ValueObjects.Path;

namespace DirectoryService.Domain.Entities.DepartmentEntity;

public class Department
{
    private readonly List<Department> _departments = [];
    private readonly List<DepartmentLocation> _locations = [];
    private readonly List<DepartmentPosition> _positions = [];
    
    private bool _isActive = true;
    
    private Department(
        Guid id, 
        DepartmentName departmentName, 
        Identifier identifier, 
        Guid? parentId, 
        Path path, 
        short depth)
    {
        Id = id;
        DepartmentName = departmentName;
        Identifier = identifier;
        ParentId = parentId;
        Path = path;
        Depth = depth;
    }
    
    public Guid Id { get; }
    public DepartmentName DepartmentName { get; private set; }
    public Identifier Identifier { get; private set; }
    public Guid? ParentId  { get; private set; }
    public Path Path { get; private set; }
    public short Depth { get; private set; }
    
    public IReadOnlyList<Department> Departments => _departments;
    public IReadOnlyList<DepartmentLocation> Locations => _locations;
    public IReadOnlyList<DepartmentPosition> Positions => _positions;
    
    public DateTime CreatedAt { get; } = DateTime.Now;
    public DateTime UpdatedAt { get; private set; }

    public static Result<Department> Create(
        Guid id,
        DepartmentName departmentName,
        Identifier identifier,
        Guid? parentId,
        Path path,
        short depth)
    {
        if (depth < 0)
            return Result.Failure<Department>("Invalid depth");
        
        return new Department(
            id,
            departmentName,
            identifier,
            parentId,
            path,
            depth);
    }
}