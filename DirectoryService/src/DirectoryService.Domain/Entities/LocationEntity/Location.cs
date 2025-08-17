using CSharpFunctionalExtensions;
using DirectoryService.Domain.Entities.DepartmentEntity;
using DirectoryService.Domain.Entities.LocationEntity.ValueObjects;
using DirectoryService.Domain.Entities.Relationships;
using TimeZone = DirectoryService.Domain.Entities.LocationEntity.ValueObjects.TimeZone;

namespace DirectoryService.Domain.Entities.LocationEntity;

public class Location
{
    private readonly List<DepartmentLocation> _departments = [];
    
    private bool _isActive = true;
    
    private Location(
        Guid id, 
        LocationName name, 
        Address address, 
        TimeZone timeZone)
    {
        Id = id;
        Name = name;
        Address = address;
        TimeZone = timeZone;
    }
    
    public Guid Id { get; }
    public LocationName Name { get; private set; }
    public Address Address { get; private set; }
    public TimeZone TimeZone { get; private set; }
    
    public IReadOnlyList<DepartmentLocation> Departments => _departments;
    
    public DateTime CreatedAt { get; } = DateTime.Now;
    public DateTime UpdatedAt { get; private set; }

    public static Result<Location> Create(
        Guid id,
        LocationName name,
        Address address,
        TimeZone timeZone)
    {
        return new Location(id, name, address, timeZone);
    }
}