using CSharpFunctionalExtensions;
using DirectoryService.Application.Repositories;
using DirectoryService.Domain.Entities.PositionEntity;
using DirectoryService.Domain.Entities.PositionEntity.ValueObjects;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Shared.SharedKernel.Errors;

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
            await dbContext.Positions.AddAsync(position, cancellationToken);

            await dbContext.SaveChangesAsync(cancellationToken);

            return position.Id.Value;
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error adding Position");

            return Error.Failure("position.create", "Failed to add Position");
        }
    }

    public async Task<UnitResult<Error>> CheckActivePositionsByName(
        PositionName name, CancellationToken cancellationToken = default)
    {
        var result = await dbContext.Positions
            .AnyAsync(p => EF.Property<bool>(p, "_isActive") 
                           && p.PositionName == name, cancellationToken);
        if (result)
            return Result.Success<Error>();
        
        return Errors.Position.NotFound();
    }
}