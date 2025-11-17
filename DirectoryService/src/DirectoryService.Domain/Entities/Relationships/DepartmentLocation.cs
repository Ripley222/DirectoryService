using DirectoryService.Domain.Entities.Ids;

namespace DirectoryService.Domain.Entities.Relationships;

public class DepartmentLocation
{
    //ef core ctor
    private DepartmentLocation()
    {
    }
    
    public DepartmentLocation(
        DepartmentId departmentId, 
        LocationId locationId)
    {
        DepartmentId = departmentId;
        LocationId = locationId;
    }
    
    public DepartmentId DepartmentId { get; private set; } = null!;
    public LocationId LocationId { get; private set; } = null!;
}