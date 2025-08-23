using CSharpFunctionalExtensions;
using Dapper;
using DirectoryService.Application.Repositories;
using DirectoryService.Domain.Entities.LocationEntity;
using DirectoryService.Infrastructure.Database;

namespace DirectoryService.Infrastructure.Repositories;

public class LocationsRepositoryDapper(NpgsqlConnectionFactory connectionFactory) : ILocationsRepository
{
    public async Task<Result<Guid, string>> Add(Location location, CancellationToken cancellationToken)
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
            transaction.Rollback();

            return "Error sql insert dapper";
        }
    }
}