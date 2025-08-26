using CSharpFunctionalExtensions;
using Dapper;
using DirectoryService.Application.Repositories;
using DirectoryService.Domain.Entities.LocationEntity;
using DirectoryService.Domain.Shared;
using DirectoryService.Infrastructure.Database;
using Microsoft.Extensions.Logging;

namespace DirectoryService.Infrastructure.Repositories;

public class LocationsRepositoryDapper(
    NpgsqlConnectionFactory connectionFactory,
    ILogger<LocationsRepositoryDapper> logger) : ILocationsRepository
{
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
}