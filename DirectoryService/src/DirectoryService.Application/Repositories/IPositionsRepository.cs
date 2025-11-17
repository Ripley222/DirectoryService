using CSharpFunctionalExtensions;
using DirectoryService.Domain.Entities.PositionEntity;
using DirectoryService.Domain.Entities.PositionEntity.ValueObjects;
using Shared.SharedKernel.Errors;

namespace DirectoryService.Application.Repositories;

public interface IPositionsRepository
{
    Task<Result<Guid, Error>> Add(Position position, CancellationToken cancellationToken);

    Task<UnitResult<Error>> CheckActivePositionsByName(PositionName name, CancellationToken cancellationToken);
}