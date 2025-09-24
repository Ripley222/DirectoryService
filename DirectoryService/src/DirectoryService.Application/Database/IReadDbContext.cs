using DirectoryService.Domain.Entities.DepartmentEntity;
using DirectoryService.Domain.Entities.LocationEntity;

namespace DirectoryService.Application.Database;

public interface IReadDbContext
{
    IQueryable<Location> LocationsRead { get; }
    IQueryable<Department> DepartmentsRead { get; }
}