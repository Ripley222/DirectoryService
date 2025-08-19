using DirectoryService.Domain.Entities.DepartmentEntity;
using DirectoryService.Domain.Entities.Ids;
using DirectoryService.Domain.Entities.LocationEntity;

namespace DirectoryService.Domain.Entities.Relationships;

public class DepartmentLocation
{
    //ef core ctor
    private DepartmentLocation()
    {
    }
    
    public DepartmentLocation(
        DepartmentId departmentId, 
        LocationId locationId,
        Department department,
        Location location)
    {
        DepartmentId = departmentId;
        LocationId = locationId;
        Department = department;
        Location = location;
    }
    
    public DepartmentId DepartmentId { get; private set; }
    public LocationId LocationId { get; private set; }
    public Department Department { get; private set; }
    public Location Location { get; private set; }
}