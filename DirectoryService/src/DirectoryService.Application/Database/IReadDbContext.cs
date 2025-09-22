using DirectoryService.Domain.Entities.LocationEntity;

namespace DirectoryService.Application.Database;

public interface IReadDbContext
{
    IQueryable<Location> LocationsRead { get; }
}