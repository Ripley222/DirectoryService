using CSharpFunctionalExtensions;
using DirectoryService.Domain.Entities.PositionEntity;
using DirectoryService.Domain.Entities.PositionEntity.ValueObjects;
using DirectoryService.Domain.Shared;

namespace DirectoryService.Application.Repositories;

public interface IPositionsRepository
{
    Task<Result<Guid, Error>> Add(Position position, CancellationToken cancellationToken);

    Task<Result<IReadOnlyList<Position>, Error>> GetPositionsByName(PositionName name, CancellationToken cancellationToken);
}