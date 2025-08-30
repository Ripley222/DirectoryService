using CSharpFunctionalExtensions;
using DirectoryService.Domain.Entities.DepartmentEntity.ValueObjects;
using DirectoryService.Domain.Entities.Ids;
using DirectoryService.Domain.Entities.Relationships;
using DirectoryService.Domain.Shared;
using Path = DirectoryService.Domain.Entities.DepartmentEntity.ValueObjects.Path;

namespace DirectoryService.Domain.Entities.DepartmentEntity;

public class Department
{
    public const int MAIN_DEPARTMENT_DEPTH = 0;
    public const int CHILD_DEPARTMENT_DEPTH = 1;
    
    private readonly List<Department> _childDepartments = [];
    private readonly List<DepartmentLocation> _locations = [];
    private readonly List<DepartmentPosition> _positions = [];
    
    private bool _isActive = true;

    //ef core ctor
    private Department()
    {
    }
    
    private Department(
        DepartmentId id, 
        DepartmentName departmentName, 
        Identifier identifier, 
        Department? parent,
        Path path, 
        short depth)
    {
        Id = id;
        DepartmentName = departmentName;
        Identifier = identifier;
        ParentId = parent?.ParentId;
        Parent = parent;
        Path = path;
        Depth = depth;
    }
    
    public DepartmentId Id { get; }
    public DepartmentName DepartmentName { get; private set; }
    public Identifier Identifier { get; private set; }
    public DepartmentId? ParentId  { get; private set; }
    public Department? Parent { get; private set; }
    public Path Path { get; private set; }
    public short Depth { get; private set; }
    
    public IReadOnlyList<Department> ChildDepartments => _childDepartments;
    public IReadOnlyList<DepartmentLocation> Locations => _locations;
    public IReadOnlyList<DepartmentPosition> Positions => _positions;
    
    public DateTime CreatedAt { get; } = DateTime.UtcNow;
    public DateTime UpdatedAt { get; private set; }

    public void AddChildDepartment(Department department)
    {
        _childDepartments.Add(department);
    }
    
    public void AddLocation(DepartmentLocation departmentLocation)
    {
        _locations.Add(departmentLocation);
    }
    
    public void AddPosition(DepartmentPosition departmentPosition)
    {
        _positions.Add(departmentPosition);
    }

    public bool IsActive()
    {
        return _isActive;
    }

    public static Result<Department, Error> Create(
        DepartmentId id,
        DepartmentName departmentName,
        Identifier identifier, 
        Path path,
        short depth,
        Department? parent = null)
    {
        if (depth < LengthConstants.Length0)
            return Errors.General.ValueIsInvalid("depth");
        
        return new Department(
            id,
            departmentName,
            identifier,
            parent,
            path,
            depth);
    }
}