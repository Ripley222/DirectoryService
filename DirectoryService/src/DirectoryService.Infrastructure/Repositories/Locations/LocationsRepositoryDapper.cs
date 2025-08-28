using CSharpFunctionalExtensions;
using Dapper;
using DirectoryService.Application.Repositories;
using DirectoryService.Domain.Entities.Ids;
using DirectoryService.Domain.Entities.LocationEntity;
using DirectoryService.Domain.Entities.LocationEntity.ValueObjects;
using DirectoryService.Domain.Shared;
using DirectoryService.Infrastructure.Database;
using Microsoft.Extensions.Logging;

namespace DirectoryService.Infrastructure.Repositories.Locations;

public class LocationsRepositoryDapper(
    NpgsqlConnectionFactory connectionFactory,
    ILogger<LocationsRepositoryDapper> logger) : ILocationsRepository
{
    public Task<Result<IReadOnlyList<Location>, Error>> GetManyByIds(
        IEnumerable<LocationId> locationIds, 
        CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }

    public async Task<Result<Guid, Error>> Add(Location location, CancellationToken cancellationToken)
    {
        using var connection = await connectionFactory.CreateConnectionAsync(cancellationToken);

        using var transaction = connection.BeginTransaction();

        try
        {
            const string sql = """
                                INSERT INTO locations (id, name, city, house, street, room_number, time_zone,
                                                       created_at, updated_at, is_active)
                                VALUES (@Id, @Name, @City, @House, @Street, @RoomNumber, @Timezone,
                                        @CreatedAt, @UpdatedAt, @IsActive)
                               """;
        
            var locationsInsertParams = new
            {
                Id = location.Id.Value,
                Name = location.LocationName.Value,
                City = location.Address.City,
                House = location.Address.House,
                Street = location.Address.Street,
                RoomNumber = location.Address.RoomNumber,
                TimeZone = location.TimeZone.Value,
                CreatedAt = location.CreatedAt,
                UpdatedAt = location.UpdatedAt,
                IsActive = true
            };
            
            await connection.ExecuteAsync(sql, locationsInsertParams);
            
            transaction.Commit();

            return location.Id.Value;
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error adding location");
            
            transaction.Rollback();

            return Error.Failure("location.create", "Failed to add location");
        }
    }

    public async Task<Result<Location, Error>> GetByName(LocationName locationName, CancellationToken cancellationToken)
    {
        var connection = await connectionFactory.CreateConnectionAsync(cancellationToken);

        const string sql = "SELECT * FROM locations WHERE name = @Name";

        var locationParamsSelect = new
        {
            Name = locationName.Value
        };
        
        var location = await connection.QuerySingleOrDefaultAsync(sql, locationParamsSelect);
        if (location is null)
            return Errors.Location.NotFound();
        
        return location;
    }

    public async Task<Result<Location, Error>> GetByAddress(Address address, CancellationToken cancellationToken)
    {
        var connection = await connectionFactory.CreateConnectionAsync(cancellationToken);

        const string sql = """
                           SELECT * FROM locations WHERE (
                               city = @City AND 
                               street = @Street AND 
                               house = @House AND 
                               room_number = @RoomNumber)
                           """;

        var locationParamsSelect = new
        {
            City = address.City,
            Street = address.Street,
            House = address.House,
            RoomNumber = address.RoomNumber,
        };
        
        var location = await connection.QuerySingleOrDefaultAsync(sql, locationParamsSelect);
        if (location is null)
            return Errors.Location.NotFound();
        
        return location;
    }
}