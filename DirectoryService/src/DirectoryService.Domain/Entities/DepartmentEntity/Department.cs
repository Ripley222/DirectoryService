using CSharpFunctionalExtensions;
using DirectoryService.Domain.Entities.DepartmentEntity.ValueObjects;
using DirectoryService.Domain.Entities.Ids;
using DirectoryService.Domain.Entities.Relationships;
using DirectoryService.Domain.Shared;
using Shared.SharedKernel.Errors;
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

    public DepartmentId Id { get; } = null!;
    public DepartmentName DepartmentName { get; private set; } = null!;
    public Identifier Identifier { get; private set; } = null!;
    public DepartmentId? ParentId { get; private set; }
    public Department? Parent { get; private set; }
    public Path Path { get; private set; } = null!;
    public short Depth { get; private set; }

    public IReadOnlyList<Department> ChildDepartments => _childDepartments;
    public IReadOnlyList<DepartmentLocation> Locations => _locations;
    public IReadOnlyList<DepartmentPosition> Positions => _positions;

    public DateTime CreatedAt { get; } = DateTime.UtcNow;
    public DateTime UpdatedAt { get; private set; }
    public DateTime DeletedAt { get; private set; }

    public void SetLocations(IEnumerable<DepartmentLocation> departmentLocations)
    {
        _locations.Clear();
        _locations.AddRange(departmentLocations);
    }

    public UnitResult<Error> SetParent(Department? parentDepartment)
    {
        ParentId = parentDepartment?.Id;
        Parent = parentDepartment;

        var newPath = parentDepartment is null
            ? Path.Create(Identifier.Value)
            : Path.Create(parentDepartment.Path.Value + "." + Identifier.Value);

        if (newPath.IsFailure)
            return newPath.Error;

        Path = newPath.Value;

        var newDepth = parentDepartment?.Depth;
        Depth = (short?)(newDepth + 1) ?? 0;

        return Result.Success<Error>();
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

    public UnitResult<Error> Deactivate()
    {
        UpdatedAt = DateTime.UtcNow;
        DeletedAt = DateTime.UtcNow;

        _isActive = false;

        var newPath = Path.Value.Contains("deleted-")
            ? Path.Create(Path.Value)
            : Path.Create(Path.Value.Replace(Identifier.Value, "deleted-" + Identifier.Value));

        if (newPath.IsFailure)
            return newPath.Error;

        Path = newPath.Value;

        return UnitResult.Success<Error>();
    }
}