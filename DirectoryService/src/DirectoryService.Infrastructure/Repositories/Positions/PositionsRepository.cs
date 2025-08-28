using CSharpFunctionalExtensions;
using DirectoryService.Application.Repositories;
using DirectoryService.Domain.Entities.PositionEntity;
using DirectoryService.Domain.Entities.PositionEntity.ValueObjects;
using DirectoryService.Domain.Shared;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace DirectoryService.Infrastructure.Repositories.Positions;

public class PositionsRepository(
    DirectoryServiceDbContext dbContext,
    ILogger<PositionsRepository> logger) : IPositionsRepository
{
    public async Task<Result<Guid, Error>> Add(
        Position position, CancellationToken cancellationToken = default)
    {
        try
        {
            await dbContext.Position.AddAsync(position, cancellationToken);

            await dbContext.SaveChangesAsync(cancellationToken);

            return position.Id.Value;
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error adding Position");

            return Error.Failure("position.create", "Failed to add Position");
        }
    }

    public async Task<Result<IReadOnlyList<Position>, Error>> GetPositionsByName(
        PositionName name, CancellationToken cancellationToken = default)
    {
        try
        {
            var positions = await dbContext.Position
                .Where(p => p.PositionName == name)
                .Include(p => p.Departments)
                .ToListAsync(cancellationToken);

            if (positions.Count == 0)
                return Errors.Position.NotFound();

            return positions;
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error getting Position");

            return Error.Failure("position.get", "Failed getting Position");
        }
    }
}