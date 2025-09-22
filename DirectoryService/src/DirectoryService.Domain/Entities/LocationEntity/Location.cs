using DirectoryService.Domain.Entities.Ids;
using DirectoryService.Domain.Entities.LocationEntity.ValueObjects;
using DirectoryService.Domain.Entities.Relationships;
using TimeZone = DirectoryService.Domain.Entities.LocationEntity.ValueObjects.TimeZone;

namespace DirectoryService.Domain.Entities.LocationEntity;

public class Location
{
    private readonly List<DepartmentLocation> _departments = [];
    
    private bool _isActive = true;

    //ef core ctor
    private Location()
    {
    }
    
    public Location(
        LocationId id, 
        LocationName locationName, 
        Address address, 
        TimeZone timeZone)
    {
        Id = id;
        LocationName = locationName;
        Address = address;
        TimeZone = timeZone;
    }

    public void AddDepartment(DepartmentLocation departmentLocation)
    {
        _departments.Add(departmentLocation);
    }
    
    public LocationId Id { get; } = null!;
    public LocationName LocationName { get; private set; } = null!;
    public Address Address { get; private set; } = null!;
    public TimeZone TimeZone { get; private set; } = null!;
    
    public IReadOnlyList<DepartmentLocation> Departments => _departments;
    
    public DateTime CreatedAt { get; } = DateTime.UtcNow;
    public DateTime UpdatedAt { get; private set; }
}